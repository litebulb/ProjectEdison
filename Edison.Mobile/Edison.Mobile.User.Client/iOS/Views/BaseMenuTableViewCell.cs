using System;
using Edison.Mobile.iOS.Common.Views;
using UIKit;

namespace Edison.Mobile.User.Client.iOS.Views
{
    public class BaseMenuTableViewCell : BaseTableViewCell
    {
        protected UILabel titleLabel;

        public BaseMenuTableViewCell(IntPtr handle) : base(handle) { }
    }
}
