using System;
using Edison.Core.Common.Models;
using Edison.Mobile.Admin.Client.Core.ViewModels;
using Edison.Mobile.Admin.Client.iOS.Cells;
using Foundation;
using UIKit;
namespace Edison.Mobile.Admin.Client.iOS.ViewSources
{
    public class NearDevicesTableViewSource : UITableViewSource
    {
        readonly WeakReference<MainViewModel> weakViewModel;

        MainViewModel ViewModel
        {
            get
            {
                weakViewModel.TryGetTarget(out var viewModel);
                return viewModel;
            }
        }

        public event EventHandler<DeviceModel> OnDeviceSelected;

        public NearDevicesTableViewSource(MainViewModel viewModel)
        {
            weakViewModel = new WeakReference<MainViewModel>(viewModel);
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(typeof(NearDeviceTableViewCell).Name, indexPath) as NearDeviceTableViewCell;
            var deviceModel = ViewModel.NearDevices[indexPath.Row];
            cell.Update(deviceModel);
            return cell;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return ViewModel.NearDevices.Count;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 44;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var deviceModel = ViewModel.NearDevices[indexPath.Row];
            
            ViewModel.CurrentDeviceModel.DeviceId = deviceModel.DeviceId;
            
            OnDeviceSelected?.Invoke(this, deviceModel);
        }
    }
}
