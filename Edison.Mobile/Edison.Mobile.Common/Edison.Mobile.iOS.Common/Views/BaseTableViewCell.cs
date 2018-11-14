using System;
using UIKit;

namespace Edison.Mobile.iOS.Common.Views
{
    public class BaseTableViewCell : UITableViewCell
    {
        protected bool isInitialized;

        public BaseTableViewCell(IntPtr handle) : base(handle)
        {
        }
    }
}
