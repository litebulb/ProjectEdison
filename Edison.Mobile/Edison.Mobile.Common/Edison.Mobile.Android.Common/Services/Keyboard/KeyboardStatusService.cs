using System;
using Android.App;
using Android.Views.InputMethods;
using System.Threading.Tasks;
using Android.Content;
using Android.Views;

namespace Edison.Mobile.Android.Common
{
    public class KeyboardStatusService
    {

        public delegate void KeyboardStatusChangeEventHandler(KeyboardStatusChangeEventArgs e);
        public event KeyboardStatusChangeEventHandler KeyboardStatusChangeEvent;


        public KeyboardStatus Status { get; private set; } = KeyboardStatus.Closed;  // Default to closed when the app starts

        private bool keyboardAppearedProcessed = false;
        private bool keyboardDisppearedProcessed = true;

        public bool Subscribed { get; private set; } = false;

        public void Subscribe()
        {
            Subscribe(BaseApplication.CurrentActivity);
        }

        public void Subscribe(Activity activity)
        {
            if (!Subscribed)
            {
                var rootLayout = activity?.Window.DecorView.FindViewById(global::Android.Resource.Id.Content);
                if (rootLayout != null)
                {
                    // Subscribe to ViewTreeObserver.GlobalLayout
                    rootLayout.ViewTreeObserver.GlobalLayout += KeyboardStatusChangeDetected;
                    Subscribed = true;  // so we only ever subscribe once
                }
            }
        }

        private async void KeyboardStatusChangeDetected(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                KeyboardStatusChangeDetected();
            }).ConfigureAwait(false);
        }

        private void KeyboardStatusChangeDetected()
        {
            if (BaseApplication.CurrentActivity?.GetSystemService(Context.InputMethodService) is InputMethodManager imm)
            {
                var open = imm.IsAcceptingText;
                if (Status == KeyboardStatus.Closed && open)  // was closed. now open
                {
                    Status = KeyboardStatus.Open;
                    // Trigger the event
                    var handler = KeyboardStatusChangeEvent;
                    handler?.Invoke(new KeyboardStatusChangeEventArgs(Status));

                }
                else if (Status == KeyboardStatus.Open && !open)  // was open. now closed
                {
                    Status = KeyboardStatus.Closed;
                    // Trigger the event
                    var handler = KeyboardStatusChangeEvent;
                    handler?.Invoke(new KeyboardStatusChangeEventArgs(Status));
                }
                // else repeating the current status, so ignore
            }
        }



        public void Unsubscribe()
        {
            Unsubscribe(BaseApplication.CurrentActivity);
        }
        public void Unsubscribe(Activity activity)
        {
            var rootLayout = activity?.Window.DecorView.FindViewById(global::Android.Resource.Id.Content);
            if (rootLayout != null)
            {
                rootLayout.ViewTreeObserver.GlobalLayout -= KeyboardStatusChangeDetected;  // Don't need to check for subscribed as wont cause an issue
                Subscribed = false;
            }
        }


        public static void DismissKeyboard(Activity activity, View view)
        {
            InputMethodManager iMM = (InputMethodManager)activity.GetSystemService(Context.InputMethodService);
            iMM.HideSoftInputFromWindow(view.WindowToken, 0);
        }
        public static void ToggleKeyboard(Activity activity)
        {
            InputMethodManager iMM = (InputMethodManager)activity.GetSystemService(Context.InputMethodService);
            iMM.ToggleSoftInput(ShowFlags.Implicit, HideSoftInputFlags.ImplicitOnly);
        }
        public static void ShowKeyboard(Activity activity, View view)
        {
            InputMethodManager iMM = (InputMethodManager)activity.GetSystemService(Context.InputMethodService);
            iMM.ShowSoftInput(view, ShowFlags.Implicit);
        }


    }

    public enum KeyboardStatus
    {
        Closed,
        Open,
        Unknown
    }

    public class KeyboardStatusChangeEventArgs : EventArgs
    {
        public KeyboardStatus Status { get; private set; } = KeyboardStatus.Unknown;
        public KeyboardStatusChangeEventArgs(KeyboardStatus status)
        {
            Status = status;
        }
    }
}