using Edison.Core.Common.Models;
using System.Net;
using System.Threading.Tasks;

namespace Edison.Core.Interfaces
{
    public interface ISignalRRestService
    {
        Task<bool> UpdateEventClusterUI(EventClusterUIModel eventClusterUIUpdate);
        Task<bool> UpdateDeviceUI(DeviceUIModel deviceUIUpdate);
        Task<bool> UpdateResponseUI(ResponseUIModel responseUIUpdate);
        Task<bool> UpdateActionCloseUI(ActionCloseUIModel actionCloseUIUpdate);
    }
}
