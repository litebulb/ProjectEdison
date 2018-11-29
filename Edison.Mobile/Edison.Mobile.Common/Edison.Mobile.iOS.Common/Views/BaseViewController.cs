using Autofac;
using Edison.Mobile.Common.Ioc;
using UIKit;
using Edison.Mobile.Common.ViewModels;

namespace Edison.Mobile.iOS.Common.Views
{
    public class BaseViewController<T> : UIViewController where T : BaseViewModel
    {
        T viewModel;
        public T ViewModel => viewModel ?? (viewModel = Container.Instance.Resolve<T>());

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            ViewModel?.ViewCreated();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            BindEventHandlers();

            ViewModel?.ViewAppearing();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            ViewModel?.ViewAppeared();
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            UnBindEventHandlers();

            ViewModel?.ViewDisappearing();
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            ViewModel?.ViewDisappeared();
        }

        public override void DidMoveToParentViewController(UIViewController parent)
        {
            base.DidMoveToParentViewController(parent);

            if (parent == null)
            {
                ViewModel?.ViewDestroyed();
            }
        }

        protected virtual void BindEventHandlers()
        {
            UnBindEventHandlers();
        }

        protected virtual void UnBindEventHandlers()
        {

        }
    }
}
