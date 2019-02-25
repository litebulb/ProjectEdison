using System;
using Android.App;
using Android.OS;
using Android.Support.Constraints;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Views;
using Android.Widget;
using Edison.Mobile.Android.Common;
using Edison.Mobile.Android.Common.Controls;
using Edison.Mobile.User.Client.Core.ViewModels;

namespace Edison.Mobile.User.Client.Droid.Activities
{
//    [Activity(Label = "@string/app_name", Theme = "@style/Theme.AppCompat.Light.Dialog.MinWidth", ExcludeFromRecents = true, ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait)]
    [Activity(Theme = "@style/RemoteNotificationRelpyTheme", ExcludeFromRecents = true, ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait, TaskAffinity = "Edison.Mobile.RemoteReply")]
    public class RemoteNotificationRelpyActivity : BaseActivity<ChatViewModel>
    {

        private ConstraintLayout _messageInputHolder;
        private AppCompatEditText _messageInput;
        private CircularImageButton _sendButton;
        private CircularImageButton _cancelButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Window.SetLayout(DisplayDetails.DisplayWidthPx, -2);
            Window.SetGravity(GravityFlags.Top | GravityFlags.Left);
            var wlp = Window.Attributes;
            wlp.X = 0;
            wlp.Y = 0;
            wlp.Width = DisplayDetails.DisplayWidthPx;
            wlp.HorizontalMargin = 0;
            wlp.VerticalMargin = 0;
            Window.Attributes = wlp;

            SetContentView(Resource.Layout.remote_notification_reply);
            SetFinishOnTouchOutside(false);
            BindViews();
            BindEvents();
            _messageInput.RequestFocus();
            KeyboardStatusService.ShowKeyboard(this, _messageInput);
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
            UnbindEvents();
        }

        public override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            Window.AddFlags(WindowManagerFlags.ShowWhenLocked);

            if (_messageInput != null)
            {
                _messageInput.RequestFocus();
                KeyboardStatusService.ShowKeyboard(ApplicationContext, _messageInput, true);
            }
        }

        private void BindViews()
        {
            _messageInputHolder = FindViewById<ConstraintLayout>(Resource.Id.message_input_holder);
            _messageInput = FindViewById<AppCompatEditText>(Resource.Id.message_input);
            _sendButton = FindViewById<CircularImageButton>(Resource.Id.send_button);
            _cancelButton = FindViewById<CircularImageButton>(Resource.Id.cancel_button);



  //          var lp = _messageInputHolder.LayoutParameters;
 //           lp.Width = DisplayDetails.DisplayWidthPx - PixelSizeConverter.DpToPx(48);
 //           _messageInputHolder.LayoutParameters = lp;

        }


        private void BindEvents()
        {
            _messageInput.TextChanged += OnTextChanged;
            _sendButton.Click += OnSendClicked;
            _cancelButton.Click += OnCancelClicked;
        }

        private void UnbindEvents()
        {
            _messageInput.TextChanged -= OnTextChanged;
            _sendButton.Click -= OnSendClicked;
            _cancelButton.Click -= OnCancelClicked;
        }

        private async void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.AfterCount == 0)
            {
                _sendButton.Alpha = 0.5f;
                _sendButton.Enabled = false;
            }
            else
            {
                _sendButton.Alpha = 1f;
                _sendButton.Enabled = true;
            }
        }

        private async void OnSendClicked(object sender, EventArgs e)
        {
/*
            var act = MainApplication.CurrentActivity;
            ActivityManager activityManager = (ActivityManager)GetSystemService(Context.ActivityService);
            var runningProcesses = activityManager?.RunningAppProcesses;
            var appTasks = activityManager.AppTasks;
            var num = appTasks.Count;
            var runningTasks = activityManager?.GetRunningTasks(100);
*/
            //send the contents of the edit text
            var msg = _messageInput.Text;
            // if text is empty do nothing
            if (string.IsNullOrWhiteSpace(msg)) return;

            // Dismiss the keyboard
            KeyboardStatusService.DismissKeyboard(this, _messageInput);
            // send message
            var success = await ViewModel.SendMessage(msg);
            // if not sent reinstate text field contents
            if (success)
                Finish();
            else
            {
                Toast.MakeText(this, Resource.String.send_error, ToastLength.Short);
                _messageInput.Text = msg;
            }
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            // Dismiss the keyboard
            KeyboardStatusService.DismissKeyboard(this, _messageInput);
            Finish();
        }

    }
}