using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Edison.Core.Common.Models;
using Edison.Mobile.Common.ViewModels;

namespace Edison.Mobile.Admin.Client.Core.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public ObservableRangeCollection<DeviceModel> NearDevices { get; private set; } = new ObservableRangeCollection<DeviceModel>();

        public override async void ViewAppeared()
        {
            base.ViewAppeared();

            await Task.Delay(3000);
            NearDevices.AddRange(new DeviceModel[]
            {
                new DeviceModel
                {
                    Online = true,
                    Sensor = true,
                    Name = "Kitchen Device",
                },
                new DeviceModel
                {
                    Online = false,
                    Sensor = false,
                    Name = "Office Device",
                },
                new DeviceModel
                {
                    Online = true,
                    Sensor = false,
                    Name = "Upstairs Device",
                },
            });
        }
    }
}
