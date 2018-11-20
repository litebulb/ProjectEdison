using System;
using System.Net;
using Edison.Core.Common.Models;
using Edison.DeviceProvisioning.Config;
using Edison.DeviceProvisioning.Helpers;
using Edison.DeviceProvisioning.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.KeyVault;
using System.Threading.Tasks;

namespace Edison.DeviceProvisionning.Controllers
{
    [Authorize(AuthenticationSchemes = "AzureAd", Policy = "Admin")]
    [Route("Security")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly DeviceProvisioningOptions _config;
        private readonly KeyVaultManager _keyVaultManager;

        public SecurityController(IOptions<DeviceProvisioningOptions> config, KeyVaultManager keyVaultManager)
        {
            _config = config.Value;
            _keyVaultManager = keyVaultManager;
        }

        [HttpGet("{deviceId}")]
        [Produces(typeof(DeviceSecretKeysModel))]
        public async Task<IActionResult> GetDeviceKeys(Guid deviceId)
        {
            var result = await _keyVaultManager.GetSecretForDevice(deviceId);
            if(result != null)
                return Ok(result);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

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
