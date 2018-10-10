using Edison.Core.Common.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Edison.Core.Interfaces
{
    public interface IIoTHubControllerRestService
    {
        Task<bool> UpdateDevicesDesired(DevicesUpdateDesiredModel devices);
    }
}
