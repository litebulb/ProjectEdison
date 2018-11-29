using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Edison.Core.Common;
using Edison.DeviceProvisioning.Config;
using Edison.DeviceProvisioning.Helpers;
using Edison.DeviceProvisioning.Models;

namespace Edison.DeviceProvisionning.Controllers
{
    /// <summary>
    /// Controller to handle security for the onboarding app
    /// </summary>
    [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureAD, Policy = AuthenticationRoles.Admin)]
    [Route("Security")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly DeviceProvisioningOptions _config;
        private readonly KeyVaultManager _keyVaultManager;

        /// <summary>
        /// DI Controller
        /// </summary>
        public SecurityController(IOptions<DeviceProvisioningOptions> config, KeyVaultManager keyVaultManager)
        {
            _config = config.Value;
            _keyVaultManager = keyVaultManager;
        }

        /// <summary>
        /// Retrieve the set of security keys for a particular device
        /// </summary>
        /// <param name="deviceId">Device Id</param>
        /// <returns>DeviceSecretKeysModel</returns>
        [HttpGet("{deviceId}")]
        [Produces(typeof(DeviceSecretKeysModel))]
        public async Task<IActionResult> GetDeviceKeys(Guid deviceId)
        {
            var result = await _keyVaultManager.GetSecretForDevice(deviceId);
            if(result != null)
                return Ok(result);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        /// <summary>
        /// Generate a new set of keys for a particular device, and store it to Azure Key Vault.
        /// The Previous keys will be erased.
        /// </summary>
        /// <param name="deviceId">Device Id</param>
        /// <returns>DeviceSecretKeysModel</returns>
        [HttpPost]
        [Produces(typeof(DeviceSecretKeysModel))]
        public async Task<IActionResult> GenerateDeviceKeys([FromBody]Guid deviceId)
        {
            var result = await _keyVaultManager.SetSecretForDevice(deviceId);
            if (result != null)
                return Ok(result);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
