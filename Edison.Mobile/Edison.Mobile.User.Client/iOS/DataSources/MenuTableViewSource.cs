using System;
using Edison.Mobile.iOS.Common.Shared;
using Edison.Mobile.User.Client.Core.ViewModels;
using Edison.Mobile.User.Client.iOS.Shared;
using Edison.Mobile.User.Client.iOS.Views;
using Foundation;
using UIKit;

namespace Edison.Mobile.User.Client.iOS.DataSources
{
    public class MenuTableViewSource : UITableViewSource
    {
        WeakReference<MenuViewModel> viewModel;

        public MenuViewModel ViewModel
        {
            get
            {
                viewModel.TryGetTarget(out MenuViewModel vm);
                return vm;
            }
            set
            {
                viewModel = new WeakReference<MenuViewModel>(value);
            }
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell;
            switch (indexPath.Row)
            {
                case 0:
                    {
                        cell = tableView.DequeueReusableCell(typeof(MenuProfileTableViewCell).Name, indexPath) as MenuProfileTableViewCell;
                        (cell as MenuProfileTableViewCell).Initialize(ViewModel?.ProfileName, null, 12, ViewModel?.Initials);
                    }
                    break;
                default:
                    {
                        cell = tableView.DequeueReusableCell(typeof(MenuItemTableViewCell).Name, indexPath) as MenuItemTableViewCell;
                        var title = "Notifications";
                        var image = Constants.Assets.NotificationBell;
                        (cell as MenuItemTableViewCell).Initialize(title, image);
                    }
                    break;
            }

            cell.ContentView.BackgroundColor = UIColor.Clear;
            cell.BackgroundColor = UIColor.Clear;
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;

            return cell;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return 2;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Row == 0) return ((UIScreen.MainScreen.Bounds.Width - Constants.MenuRightMargin) / 2f) + Constants.MenuCellHeight;
            return Constants.MenuCellHeight;
        }

        public override void RowHighlighted(UITableView tableView, NSIndexPath rowIndexPath)
        {
            if (rowIndexPath.Row > 0)
            {
                var cell = tableView.CellAt(rowIndexPath);
                cell.Alpha = 0.6f;
            }
        }

        public override void RowUnhighlighted(UITableView tableView, NSIndexPath rowIndexPath)
        {
            if (rowIndexPath.Row > 0)
            {
                var cell = tableView.CellAt(rowIndexPath);
                UIView.Animate(PlatformConstants.AnimationDuration, () => cell.Alpha = 1);
            }
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Row == 1)
            {
                UIApplication.SharedApplication.OpenUrl(new NSUrl("App-prefs:root=NOTIFICATIONS_ID"), new NSDictionary(), null);
            }
        }
    }
}
