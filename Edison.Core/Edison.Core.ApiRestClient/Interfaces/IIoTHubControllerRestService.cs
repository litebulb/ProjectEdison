using Edison.Core.Common.Models;
using System.Threading.Tasks;

namespace Edison.Core.Interfaces
{
    public interface IIoTHubControllerRestService
    {
        Task<bool> UpdateDevicesDesired(DevicesUpdateDesiredModel devices);
    }
}
