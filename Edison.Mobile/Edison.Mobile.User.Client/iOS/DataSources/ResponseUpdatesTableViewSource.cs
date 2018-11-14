using System;
using System.Collections.Generic;
using CoreGraphics;
using Edison.Core.Common.Models;
using Edison.Mobile.User.Client.iOS.Views;
using Foundation;
using UIKit;

namespace Edison.Mobile.User.Client.iOS.DataSources
{
    public class TableViewScrolledEventArgs : EventArgs 
    {
        public CGPoint ContentOffset { get; set; }
    }

    public class ResponseUpdatesTableViewSource : UITableViewSource
    {
        public event EventHandler<TableViewScrolledEventArgs> OnTableViewScrolled;

        public List<NotificationModel> Notifications { get; set; }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(typeof(ResponseUpdateTableViewCell).Name, indexPath) as ResponseUpdateTableViewCell;
            var notification = Notifications[indexPath.Row];
            var isFirstCell = indexPath.Row == 0;

            cell.Initialize(notification, !isFirstCell, isFirstCell);

            return cell;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return Notifications?.Count ?? 0;
        }

        public override void Scrolled(UIScrollView scrollView)
        {
            OnTableViewScrolled?.Invoke(this, new TableViewScrolledEventArgs
            {
                ContentOffset = scrollView.ContentOffset,
            });
        }
    }
}
