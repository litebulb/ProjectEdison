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
                        (cell as MenuProfileTableViewCell).Initialize(ViewModel?.ProfileName);
                    }
                    break;
                default:
                    {
                        cell = tableView.DequeueReusableCell(typeof(MenuItemTableViewCell).Name, indexPath) as MenuItemTableViewCell;
                        var title = indexPath.Row == 1 ? "My Info" : "Notifications";
                        var image = indexPath.Row == 1 ? Constants.Assets.PersonWhite : Constants.Assets.NotificationBell;
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
            return 3;
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
            var cell = tableView.CellAt(rowIndexPath);
            cell.Alpha = 0.6f;
        }

        public override void RowUnhighlighted(UITableView tableView, NSIndexPath rowIndexPath)
        {
            var cell = tableView.CellAt(rowIndexPath);
            UIView.Animate(PlatformConstants.AnimationDuration, () => cell.Alpha = 1);
        }
    }
}
