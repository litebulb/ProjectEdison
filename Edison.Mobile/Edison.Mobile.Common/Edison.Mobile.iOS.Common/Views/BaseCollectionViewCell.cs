using System;
using Edison.Mobile.Common.Shared;
using UIKit;

namespace Edison.Mobile.iOS.Common.Views
{
    public class BaseCollectionViewCell : UICollectionViewCell
    {
        protected bool isInitialized;

        public BaseCollectionViewCell(IntPtr handle) : base(handle) { }
    }

    public class BaseCollectionViewCell<T> : BaseCollectionViewCell where T : BaseCollectionItemViewModel
    {
        public T ViewModel { get; set; }

        public BaseCollectionViewCell(IntPtr handle) : base(handle) { }

        public override void WillMoveToSuperview(UIView newsuper)
        {
            if (newsuper == null) 
            {
                UnbindEventHandlers();
                ViewModel?.ViewDisappearing();
            }
            else 
            {
                BindEventHandlers();
                ViewModel?.ViewAppearing();
            }
        }

        public override void MovedToSuperview()
        {
            base.MovedToSuperview();

            if (Superview == null) 
            {
                ViewModel?.ViewDisappeared();
            }
            else 
            {
                ViewModel?.ViewAppeared();
            }
        }

        public virtual void BindEventHandlers() 
        {
            UnbindEventHandlers();
        }

        public virtual void UnbindEventHandlers() 
        {

        }
    }
}
