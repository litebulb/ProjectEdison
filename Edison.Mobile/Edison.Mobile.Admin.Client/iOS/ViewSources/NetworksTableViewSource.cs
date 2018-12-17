using System;
using Edison.Mobile.Admin.Client.iOS.Cells;
using Edison.Mobile.Common.WiFi;
using Foundation;
using UIKit;

namespace Edison.Mobile.Admin.Client.iOS.ViewSources
{
    public class NetworksTableViewSource : UITableViewSource
    {
        public WifiNetwork[] AvailableNetworks { get; set; }

        public event EventHandler<WifiNetwork> OnWifiNetworkSelected;

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(typeof(WifiNetworkTableViewCell).Name, indexPath) as WifiNetworkTableViewCell;
            cell.Refresh(AvailableNetworks[indexPath.Row]);
            return cell;
        }

        public override nint RowsInSection(UITableView tableview, nint section) => AvailableNetworks?.Length ?? 0;

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var wifiNetwork = AvailableNetworks[indexPath.Row];
            OnWifiNetworkSelected?.Invoke(this, wifiNetwork);
        }
    }
}
