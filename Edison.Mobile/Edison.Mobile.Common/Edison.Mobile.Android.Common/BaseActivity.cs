using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Autofac;
using Edison.Mobile.Common.Ioc;
using Edison.Mobile.Common.Shared;

namespace Edison.Mobile.Android.Common
{ 

    public class BaseActivity<T> : Activity, ViewTreeObserver.IOnGlobalLayoutListener where T : BaseViewModel
    {
        T viewModel;

        protected T ViewModel => viewModel ?? (viewModel = Container.Instance.Resolve<T>());
        protected Rect VisibleDisplayRect { get; private set; } = new Rect();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Window.DecorView.ViewTreeObserver.AddOnGlobalLayoutListener(this);
            ViewModel?.ViewCreated();
        }

        protected override void OnStart()
        {
            base.OnStart();

            BindEventHandlers();

            ViewModel?.ViewAppearing();
        }

        protected override void OnResume()
        {
            base.OnResume();

            BindEventHandlers();

            ViewModel?.ViewAppeared();
        }

        protected override void OnPause()
        {
            base.OnPause();

            UnBindEventHandlers();

            ViewModel?.ViewDisappearing();
        }

        protected override void OnStop()
        {
            base.OnStop();

            UnBindEventHandlers();

            ViewModel?.ViewDisappeared();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            UnBindEventHandlers();

            Window.DecorView.ViewTreeObserver.RemoveOnGlobalLayoutListener(this);

            ViewModel?.ViewDestroyed();
        }

        protected virtual void BindEventHandlers() 
        {
            UnBindEventHandlers();


        }

        protected virtual void UnBindEventHandlers() 
        {

        }

        public virtual void OnGlobalLayout() 
        {
            var displayRect = new Rect();
            Window.DecorView.GetWindowVisibleDisplayFrame(displayRect);
            VisibleDisplayRect = displayRect;
        }
    }
}
