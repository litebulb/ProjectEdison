using System;
using Edison.Mobile.iOS.Common.Views;
using UIKit;

namespace Edison.Mobile.User.Client.iOS.Views
{
    public class BaseMenuTableViewCell : BaseTableViewCell
    {
        protected bool isInitialized;
        protected UILabel titleLabel;

        public BaseMenuTableViewCell(IntPtr handle) : base(handle) { }
    }
}
