using System;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Autofac;
using Edison.Mobile.Common.Ioc;
using Edison.Mobile.Common.ViewModels;


namespace Edison.Mobile.Android.Common
{ 

    public class BaseActivity<T> : AppCompatActivity, ViewTreeObserver.IOnGlobalLayoutListener where T : BaseViewModel
    {
        public static event EventHandler GlobalLayout;
        public delegate void OrientationChangedEventHandler(object sender, OrientationChangedEventArgs e);
        public static event OrientationChangedEventHandler OrientationChanged;
        public static event EventHandler BackPressed;
        public delegate void KeyboardStatusChangeHandler(KeyboardStatusChangeEventArgs e);
        public static event KeyboardStatusChangeHandler KeyboardStatusChanged;

        private KeyboardStatusService KeyboardStatus { get; } = new KeyboardStatusService();


        T viewModel;

        protected T ViewModel => viewModel ?? (viewModel = Container.Instance.Resolve<T>());
        protected Rect VisibleDisplayRect { get; private set; } = new Rect();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            KeyboardStatus.Subscribe(this);
            KeyboardStatus.KeyboardStatusChangeEvent += OnKeyboardStatusChanged;
            Window.DecorView.ViewTreeObserver.AddOnGlobalLayoutListener(this);  // Why??
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
            KeyboardStatus.KeyboardStatusChangeEvent -= OnKeyboardStatusChanged;
            KeyboardStatus.Unsubscribe(this);
            UnBindEventHandlers();
            ViewModel?.ViewDestroyed();
            Window.DecorView.ViewTreeObserver.RemoveOnGlobalLayoutListener(this);
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
            var handler = GlobalLayout;
            handler?.Invoke(this, EventArgs.Empty);
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);

            var orientation = new OrientationChangedEventArgs(ScreenOrientation.Undefined);
            if (newConfig.Orientation == global::Android.Content.Res.Orientation.Landscape)
                orientation = new OrientationChangedEventArgs(ScreenOrientation.Landscape);
            else if (newConfig.Orientation == global::Android.Content.Res.Orientation.Portrait || newConfig.Orientation == global::Android.Content.Res.Orientation.Square)
                orientation = new OrientationChangedEventArgs(ScreenOrientation.Portrait);

            var handler = OrientationChanged;
            handler?.Invoke(this, orientation);
        }


        public override void OnBackPressed()
        {
            var handler = BackPressed;
            handler?.Invoke(this, EventArgs.Empty);
            base.OnBackPressed();
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == 16908332)  // Android.Resource.Id.BackButton - which is internal
            {
                var handler = BackPressed;
                handler?.Invoke(this, EventArgs.Empty);
            }
            return base.OnOptionsItemSelected(item);
        }


        private void OnKeyboardStatusChanged(KeyboardStatusChangeEventArgs e)
        {
            // Trigger the event
            var handler = KeyboardStatusChanged;
            handler?.Invoke(e);
        }
    }

    public class OrientationChangedEventArgs : EventArgs
    {
        public ScreenOrientation Orientation { get; set; } = ScreenOrientation.Undefined;

        public OrientationChangedEventArgs(ScreenOrientation orientation)
        {
            Orientation = orientation;
        }
        public OrientationChangedEventArgs(int width, int height)
        {
            if (width > height)
                Orientation = ScreenOrientation.Landscape;
            else
                Orientation = ScreenOrientation.Portrait;
        }

    }
}
