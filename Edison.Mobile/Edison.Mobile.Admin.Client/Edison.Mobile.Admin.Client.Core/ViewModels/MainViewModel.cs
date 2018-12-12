using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Edison.Core.Common.Models;
using Edison.Mobile.Admin.Client.Core.Network;
using Edison.Mobile.Common.ViewModels;

namespace Edison.Mobile.Admin.Client.Core.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        readonly DeviceRestService deviceRestService;

        public ObservableRangeCollection<DeviceModel> NearDevices { get; private set; } = new ObservableRangeCollection<DeviceModel>();

        public MainViewModel(DeviceRestService deviceRestService)
        {
            this.deviceRestService = deviceRestService;
        }

        public override async void ViewAppeared()
        {
            base.ViewAppeared();

            await GetNearDevices();
        }

        async Task GetNearDevices()
        {
            var devices = await deviceRestService.GetDevices();
            if (devices != null)
            {
                NearDevices.AddRange(devices);
            }

            //await Task.Delay(3000);
            //NearDevices.AddRange(new DeviceModel[]
            //{
            //    new DeviceModel
            //    {
            //        Online = true,
            //        Sensor = true,
            //        Name = "Kitchen Device",
            //    },
            //    new DeviceModel
            //    {
            //        Online = false,
            //        Sensor = false,
            //        Name = "Office Device",
            //    },
            //    new DeviceModel
            //    {
            //        Online = true,
            //        Sensor = false,
            //        Name = "Upstairs Device",
            //    },
            //});
        }
    }
}
