using System;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Autofac;
using Edison.Mobile.Common.Ioc;
using Edison.Mobile.Common.ViewModels;

using Fragment = Android.Support.V4.App.Fragment;
using DialogFragment = Android.Support.V4.App.DialogFragment;
using Java.Lang;
using Android.Content;

namespace Edison.Mobile.Android.Common
{ 

    public class BaseActivity<T> : AppCompatActivity, ViewTreeObserver.IOnGlobalLayoutListener where T : BaseViewModel
    {
        public event EventHandler GlobalLayout;
        public delegate void OrientationChangedEventHandler(object sender, OrientationChangedEventArgs e);
        public event OrientationChangedEventHandler OrientationChanged;
        public event EventHandler BackPressed;
        public delegate void KeyboardStatusChangeHandler(KeyboardStatusChangeEventArgs e);
        public event KeyboardStatusChangeHandler KeyboardStatusChanged;

        public EventHandler<Fragment> FragmentPoppedOnBack;

        private const int DeafultTransitionDelayMs = 80;

        private Context _context;
        private KeyboardStatusService KeyboardStatus { get; } = new KeyboardStatusService();


        private T _viewModel;

        protected T ViewModel => _viewModel ?? (_viewModel = Container.Instance.Resolve<T>());
        protected Rect VisibleDisplayRect { get; private set; } = new Rect();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _context = this;
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
            GlobalLayout?.Invoke(this, EventArgs.Empty);
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);

            var orientation = new OrientationChangedEventArgs(ScreenOrientation.Undefined);
            if (newConfig.Orientation == global::Android.Content.Res.Orientation.Landscape)
                orientation = new OrientationChangedEventArgs(ScreenOrientation.Landscape);
            else if (newConfig.Orientation == global::Android.Content.Res.Orientation.Portrait || newConfig.Orientation == global::Android.Content.Res.Orientation.Square)
                orientation = new OrientationChangedEventArgs(ScreenOrientation.Portrait);

            OrientationChanged?.Invoke(this, orientation);
        }


        public override void OnBackPressed()
        {
            BackPressed?.Invoke(this, EventArgs.Empty);
            base.OnBackPressed();
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == 16908332)  // Android.Resource.Id.BackButton - which is internal
                BackPressed?.Invoke(this, EventArgs.Empty);
            return base.OnOptionsItemSelected(item);
        }


        private void OnKeyboardStatusChanged(KeyboardStatusChangeEventArgs e)
        {
            // Trigger the event
            KeyboardStatusChanged?.Invoke(e);
        }

        // To support using the Fragment Backstack with back press
        // Override OnBackPress and have it call this utility method
        public void OnBackPressWithFragmentManagement()
        {
            BackPressed?.Invoke(this, EventArgs.Empty);
            if (SupportFragmentManager.BackStackEntryCount > 1)
            {
                // get the fragment to be popped
                SupportFragmentManager.PopBackStackImmediate();
                var entry = SupportFragmentManager.GetBackStackEntryAt(SupportFragmentManager.BackStackEntryCount - 1);
                var frag = SupportFragmentManager.FindFragmentByTag(entry.Name);
                FragmentPoppedOnBack?.Invoke(null, frag);
            }
            else
            {
                if (SupportFragmentManager.BackStackEntryCount == 1)
                    SupportFragmentManager.PopBackStackImmediate();
                base.OnBackPressed();
            }
        }





        protected Fragment GetFragmentFromBackstack(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag)) return null;
            return SupportFragmentManager.FindFragmentByTag(tag);
        }


        protected void ReplaceFragment(Fragment fragment, int fragmentTargetResId, bool addToBackstack = true, string tag = null)
        {
            if (addToBackstack)
                SupportFragmentManager.BeginTransaction().Replace(fragmentTargetResId, fragment, tag).AddToBackStack(tag).Commit();
            else
                SupportFragmentManager.BeginTransaction().Replace(fragmentTargetResId, fragment, tag).Commit();
            SupportFragmentManager.ExecutePendingTransactions();
        }


        // Start Fragment transaction with delay to avoid any graphics issues while closing the drawer
        protected void ReplaceFragmentWithDelay(Fragment fragment, int fragmentTargetResId, bool addToBackstack = true, string tag = null, int transitionDelayMs = DeafultTransitionDelayMs)
        {
            new Handler().PostDelayed(() =>
            {
                //SupportFragmentManager.BeginTransaction().Replace(Resource.Id.content_container, fragment, tag).Commit();
                ReplaceFragment(fragment, fragmentTargetResId, addToBackstack, tag);
            }, transitionDelayMs);
        }

        
        protected void DisplayDialogFragment(DialogFragment dialog, string tag)
        {
            var ft = SupportFragmentManager.BeginTransaction();
            // Ensure fragment is not already on the back stack otherwise it will crash. Using "dialog' as the Tag
            var prev = GetFragmentFromBackstack(tag);
            if (prev != null)
                ft.Remove(prev);
            ft.AddToBackStack(null);
            dialog.Show(ft, tag);
        }


        // Start this activity with delay to avoid any graphics issues while closing the drawer
        private void StartActivityWithDelay(Class activity, int transitionDelayMs = DeafultTransitionDelayMs)
        {
            new Handler().PostDelayed(() =>
            {
                StartActivity(new Intent(_context, activity));

            }, transitionDelayMs);
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
