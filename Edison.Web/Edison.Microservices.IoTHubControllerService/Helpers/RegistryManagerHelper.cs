using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using Edison.IoTHubControllerService.Config;

namespace Edison.IoTHubControllerService.Helpers
{
    /// <summary>
    /// Manager for the IoT Hub Registry
    /// </summary>
    public class RegistryManagerHelper
    {
        private readonly RegistryManager _registryManager;
        private readonly JobClient _jobClient;
        private readonly IoTHubControllerOptions _config;
        private readonly ILogger<RegistryManagerHelper> _logger;

        /// <summary>
        /// DI Constructor
        /// </summary>
        public RegistryManagerHelper(IOptions<IoTHubControllerOptions> config, ILogger<RegistryManagerHelper> logger)
        {
            _config = config.Value;
            _registryManager = RegistryManager.CreateFromConnectionString(_config.IoTHubConnectionString);
            _jobClient = JobClient.CreateFromConnectionString(_config.IoTHubConnectionString);
            _logger = logger;
        }

        /// <summary>
        /// Create a new device
        /// </summary>
        /// <param name="deviceId">Device Id</param>
        /// <param name="jsonTags">Tags for the device, in json format</param>
        /// <param name="jsonDesired">Desired properties for the device, in json format</param>
        /// <returns>True if the device was successfully created</returns>
        public async Task<bool> CreateDevice(Guid deviceId, string jsonTags, string jsonDesired)
        {
            try
            {
                Device device = await _registryManager.GetDeviceAsync(deviceId.ToString());
                if (device == null)
                {
                    _logger.LogDebug($"Create new device '{deviceId}'");
                    device = await _registryManager.AddDeviceAsync(new Device(deviceId.ToString()));
                }
                else
                {
                    _logger.LogDebug($"The device '{deviceId}' already exist.");
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"CreateDevice: {e.Message}");
                return false;
            }
            return await UpdateDevice(deviceId, jsonTags, jsonDesired);
        }

        /// <summary>
        /// Update an existing device
        /// </summary>
        /// <param name="deviceId">Device Id</param>
        /// <param name="jsonTags">Tags for the device, in json format</param>
        /// <param name="jsonDesired">Desired properties for the device, in json format</param>
        /// <returns>True if the device was successfully updated</returns>
        public async Task<bool> UpdateDevice(Guid deviceId, string jsonTags, string jsonDesired)
        {
            try
            {
                Twin twin = ComposeTwin(jsonTags, jsonDesired);
                await _registryManager.UpdateTwinAsync(deviceId.ToString(), twin, twin.ETag);
                return true;
            }
            catch(Exception e)
            {
                _logger.LogError($"UpdateDevice: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Update a set of devices using a job
        /// </summary>
        /// <param name="deviceId">List of Device Ids</param>
        /// <param name="jsonTags">Tags for the devices, in json format</param>
        /// <param name="jsonDesired">Desired properties for the devices, in json format</param>
        /// /// <param name="waitForCompletion">True if the call should wait for the completion of the job before returning the result</param>
        /// <returns>True if the devices were successfully updated</returns>
        public async Task<bool> UpdateDevices(List<Guid> deviceIds, string jsonTags, string jsonDesired, bool waitForCompletion)
        {
            if (deviceIds == null || deviceIds.Count == 0 ||
                (string.IsNullOrEmpty(jsonTags) && string.IsNullOrEmpty(jsonDesired)))
            {
                _logger.LogDebug("UpdateDevices: Nothing to update...");
                return true;
            }

            if (deviceIds.Count == 1)
                return await UpdateDevice(deviceIds[0], jsonTags, jsonDesired);
                
            string jobId = Guid.NewGuid().ToString();
            try
            {
                Twin twin = ComposeTwin(jsonTags, jsonDesired);
                JobResponse result = await _jobClient.ScheduleTwinUpdateAsync(jobId, GetJobQuery(deviceIds), twin, DateTime.Now, GetJobTimeout(deviceIds));
                return waitForCompletion ? await WaitForJobCompletion(jobId, result) : true;
            }
            catch (IotHubThrottledException te)
            {
                if(te.Code == ErrorCode.ThrottlingException)
                {
                    _logger.LogError($"UpdateDevices Throttling Exception: {te.Message}. Retrying in 15 seconds...");
                    await Task.Delay(15000);
                    return await UpdateDevices(deviceIds, jsonTags, jsonDesired, waitForCompletion);
    }
                throw te;
            }
            catch (Exception e)
            {
                _logger.LogError($"UpdateDevices: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Launch a direct method on a set of devices
        /// </summary>
        /// <param name="deviceIds">List of Device Ids</param>
        /// <param name="methodName">Method name</param>
        /// <param name="payload">Payload of the method, in json format</param>
        /// <param name="waitForCompletion">True if the call should wait for the completion of the job before returning the result</param>
        /// <returns>True if the direct method was successfully launch on all devices</returns>
        public async Task<bool> LaunchDirectMethods(List<Guid> deviceIds, string methodName, string payload, bool waitForCompletion)
        {
            if (deviceIds == null || deviceIds.Count == 0 || string.IsNullOrEmpty(methodName))
            {
                _logger.LogDebug("LaunchDirectMethod: Nothing to launch...");
                return true;
            }

            string jobId = Guid.NewGuid().ToString();
            try
            {
                CloudToDeviceMethod method = new CloudToDeviceMethod(methodName);
                if (!string.IsNullOrEmpty(payload))
                    method.SetPayloadJson(payload);
                method.ResponseTimeout = TimeSpan.FromSeconds(_config.DirectMethodTimeout);

                JobResponse result = await _jobClient.ScheduleDeviceMethodAsync(jobId, GetJobQuery(deviceIds), method, DateTime.Now, GetJobTimeout(deviceIds));
                return waitForCompletion ? await WaitForJobCompletion(jobId, result) : true;
            }
            catch (IotHubThrottledException te)
            {
                if(te.Code == ErrorCode.ThrottlingException)
                {
                    _logger.LogError($"LaunchDirectMethods Throttling Exception: {te.Message}. Retrying in 15 seconds...");
                    await Task.Delay(15000);
                    return await LaunchDirectMethods(deviceIds, methodName, payload, waitForCompletion);
                }
                throw te;
            }
            catch (Exception e)
            {
                _logger.LogError($"LaunchDirectMethods: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Delete a device
        /// </summary>
        /// <param name="deviceId">Device Id</param>
        /// <returns>True if the device was successfully deleted</returns>
        public async Task<bool> DeleteDevice(Guid deviceId)
        {
            try
            {
                Device device = await _registryManager.GetDeviceAsync(deviceId.ToString());
                if (device != null)
                    await _registryManager.RemoveDeviceAsync(device);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"DeleteDeviceAsync: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get a twin object from a json string of tags/desired
        /// </summary>
        /// <param name="jsonTags">Tags for the device, in json format</param>
        /// <param name="jsonDesired">Desired properties for the device, in json format</param>
        /// <returns></returns>
        private Twin ComposeTwin(string jsonTags, string jsonDesired)
        {
            Twin twin = new Twin();
            if (!string.IsNullOrEmpty(jsonDesired))
                twin.Properties.Desired = JsonConvert.DeserializeObject<TwinCollection>(jsonDesired);
            if (!string.IsNullOrEmpty(jsonTags))
                twin.Tags = JsonConvert.DeserializeObject<TwinCollection>(jsonTags);
            twin.ETag = "*";

            return twin;
        }

        /// <summary>
        /// Generate a job query from a list of devices ids
        /// </summary>
        /// <param name="deviceIds">List of Device Ids</param>
        /// <returns>Job query</returns>
        private string GetJobQuery(List<Guid> deviceIds)
        {
            return "deviceId IN ['" + String.Join("','", deviceIds) + "']";
        }

        /// <summary>
        /// Get a timeout integer based on the number of devices to process in the job
        /// </summary>
        /// <param name="deviceIds">List of Device Ids</param>
        /// <returns>Timeout count</returns>
        private int GetJobTimeout(List<Guid> deviceIds)
        {
            int timeout = _config.JobTimeout;
            if (deviceIds.Count > timeout)
                timeout = deviceIds.Count * 5;
            return timeout;
        }

        /// <summary>
        /// Wait for a job to complete
        /// </summary>
        /// <param name="jobId">Job Id</param>
        /// <param name="job">Job Response object</param>
        /// <returns>True if the job succeeded</returns>
        private async Task<bool> WaitForJobCompletion(string jobId, JobResponse job)
        {
            while ((job.Status != JobStatus.Completed) && (job.Status != JobStatus.Failed))
            {
                job = await _jobClient.GetJobAsync(jobId);
                await Task.Delay(2000);
            }
            return job.Status == JobStatus.Completed;
        }
    }
}
