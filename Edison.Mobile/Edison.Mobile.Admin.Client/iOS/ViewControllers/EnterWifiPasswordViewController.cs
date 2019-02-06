using System;
using System.Threading.Tasks;
using Edison.Mobile.Admin.Client.Core.ViewModels;
using Edison.Mobile.Admin.Client.iOS.Shared;
using Edison.Mobile.Admin.Client.iOS.Views;
using Edison.Mobile.Common.WiFi;
using Edison.Mobile.iOS.Common.Shared;
using Edison.Mobile.iOS.Common.Views;
using Foundation;
using UIKit;

namespace Edison.Mobile.Admin.Client.iOS.ViewControllers
{
    public class EnterWifiPasswordViewController : BaseViewController<EnterWifiPasswordViewModel>
    {
        readonly WifiNetwork selectedWifiNetwork;

        UIButton showPasswordButton;
        TextFieldView passwordTextFieldView;

        UILayoutGuide bottomLayoutGuide;
        NSLayoutConstraint bottomLayoutConstraint;

        NSObject keyboardWillShowNotificationToken;
        NSObject keyboardWillHideNotificationToken;

        public EnterWifiPasswordViewController(WifiNetwork selectedWifiNetwork)
        {
            this.selectedWifiNetwork = selectedWifiNetwork;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = Constants.Color.BackgroundLightGray;

            Title = $"Set Up {ViewModel.DeviceTypeAsString}";

            NavigationItem.LeftBarButtonItem = new UIBarButtonItem(Constants.Assets.ArrowLeft, UIBarButtonItemStyle.Plain, (sender, e) =>
            {
                NavigationController.PopViewController(true);
            });

            var padding = Constants.Padding;

            var circleNumberView = new CircleNumberView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Number = 5,
            };

            View.AddSubview(circleNumberView);
            circleNumberView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, constant: padding).Active = true;
            circleNumberView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, constant: padding).Active = true;
            circleNumberView.WidthAnchor.ConstraintEqualTo(Constants.CircleNumberSize).Active = true;
            circleNumberView.HeightAnchor.ConstraintEqualTo(circleNumberView.WidthAnchor).Active = true;

            var enterPasswordLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextColor = Constants.Color.MidGray,
                Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.Eighteen),
                AdjustsFontSizeToFitWidth = true,
                MinimumScaleFactor = 0.1f,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0,
                Text = $"Enter the password for {selectedWifiNetwork.SSID}",
            };

            View.AddSubview(enterPasswordLabel);
            enterPasswordLabel.LeftAnchor.ConstraintEqualTo(circleNumberView.RightAnchor, constant: padding).Active = true;
            enterPasswordLabel.CenterYAnchor.ConstraintEqualTo(circleNumberView.CenterYAnchor).Active = true;
            enterPasswordLabel.RightAnchor.ConstraintEqualTo(View.RightAnchor, constant: -padding).Active = true;

            bottomLayoutGuide = new UILayoutGuide();

            View.AddLayoutGuide(bottomLayoutGuide);

            bottomLayoutGuide.TopAnchor.ConstraintEqualTo(enterPasswordLabel.BottomAnchor).Active = true;
            bottomLayoutConstraint = bottomLayoutGuide.BottomAnchor.ConstraintEqualTo(View.BottomAnchor);
            bottomLayoutConstraint.Active = true;

            passwordTextFieldView = new TextFieldView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                SecureTextEntry = true,
                LabelText = "Password",
                ReturnKeyType = UIReturnKeyType.Go,
            };

            View.AddSubview(passwordTextFieldView);

            passwordTextFieldView.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;
            passwordTextFieldView.CenterYAnchor.ConstraintEqualTo(bottomLayoutGuide.CenterYAnchor).Active = true;
            passwordTextFieldView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor).Active = true;
            passwordTextFieldView.RightAnchor.ConstraintEqualTo(View.RightAnchor).Active = true;

            showPasswordButton = new UIButton { TranslatesAutoresizingMaskIntoConstraints = false };
            showPasswordButton.SetTitle("Show Password", UIControlState.Normal);
            showPasswordButton.TitleLabel.Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.Fourteen);
            showPasswordButton.SetTitleColor(Constants.Color.DarkBlue, UIControlState.Normal);

            View.AddSubview(showPasswordButton);

            showPasswordButton.TopAnchor.ConstraintEqualTo(passwordTextFieldView.BottomAnchor, constant: padding / 2).Active = true;
            showPasswordButton.RightAnchor.ConstraintEqualTo(passwordTextFieldView.RightAnchor, constant: -padding).Active = true;
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            passwordTextFieldView.BecomeFirstResponder();
        }

        protected override void BindEventHandlers()
        {
            base.BindEventHandlers();

            showPasswordButton.TouchUpInside += HandleShowPasswordButtonTouchUpInside;
            passwordTextFieldView.OnTextFieldViewReturned += HandlePasswordTextFieldViewOnTextFieldViewReturned;

            keyboardWillShowNotificationToken = UIKeyboard.Notifications.ObserveWillShow(HandleKeyboardWillShow);
            keyboardWillHideNotificationToken = UIKeyboard.Notifications.ObserveWillHide(HandleKeyboardWillHide);
        }

        protected override void UnBindEventHandlers()
        {
            base.UnBindEventHandlers();

            showPasswordButton.TouchUpInside -= HandleShowPasswordButtonTouchUpInside;
            passwordTextFieldView.OnTextFieldViewReturned -= HandlePasswordTextFieldViewOnTextFieldViewReturned;

            if (keyboardWillShowNotificationToken != null) NSNotificationCenter.DefaultCenter.RemoveObserver(keyboardWillShowNotificationToken);
            if (keyboardWillHideNotificationToken != null) NSNotificationCenter.DefaultCenter.RemoveObserver(keyboardWillHideNotificationToken);
        }

        private void HandleKeyboardWillHide(object sender, UIKeyboardEventArgs e)
        {
            bottomLayoutConstraint.Constant = 0;

            UIView.BeginAnimations(null);
            UIView.SetAnimationDuration(e.AnimationDuration);
            UIView.SetAnimationCurve(e.AnimationCurve);
            UIView.SetAnimationBeginsFromCurrentState(true);
            View.LayoutIfNeeded();
            UIView.CommitAnimations();
        }

        private void HandleKeyboardWillShow(object sender, UIKeyboardEventArgs e)
        {
            bottomLayoutConstraint.Constant = -e.FrameEnd.Height;

            UIView.BeginAnimations(null);
            UIView.SetAnimationDuration(e.AnimationDuration);
            UIView.SetAnimationCurve(e.AnimationCurve);
            UIView.SetAnimationBeginsFromCurrentState(true);
            View.LayoutIfNeeded();
            UIView.CommitAnimations();
        }

        async void HandlePasswordTextFieldViewOnTextFieldViewReturned(object sender, EventArgs e)
        {
            var textFieldView = sender as TextFieldView;
            var text = textFieldView.Text;
            textFieldView.ResignFirstResponder();

            textFieldView.UserInteractionEnabled = false;
            showPasswordButton.UserInteractionEnabled = false;

            UIView.Animate(PlatformConstants.AnimationDuration, () =>
            {
                textFieldView.Alpha = PlatformConstants.DisabledAlpha;
                showPasswordButton.Alpha = PlatformConstants.DisabledAlpha;
            });

            await Task.Run(async () =>
            {
                var connected = await ViewModel.ConnectDeviceToNetwork(selectedWifiNetwork.SSID, text);
                if (connected)
                {
                    await ViewModel.DisconnectFromDevice();
                    InvokeOnMainThread(() => NavigationController.PushViewController(new ManageDeviceViewController(null), true));
                }
                else
                {
                    InvokeOnMainThread(() =>
                    {
                        UIView.Animate(PlatformConstants.AnimationDuration, () => 
                        {
                            textFieldView.Alpha = 1;
                            showPasswordButton.Alpha = 1;
                        });

                        textFieldView.UserInteractionEnabled = true;
                        showPasswordButton.UserInteractionEnabled = true;
                        textFieldView.BecomeFirstResponder();

                        var alertController = UIAlertController.Create(null, "Could not connect. Try again!", UIAlertControllerStyle.Alert);
                        alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                        PresentViewController(alertController, true, null);
                    });
                }
            });
        }

        void HandleShowPasswordButtonTouchUpInside(object sender, EventArgs e)
        {
            passwordTextFieldView.SecureTextEntry = !passwordTextFieldView.SecureTextEntry;
            showPasswordButton.SetTitle(passwordTextFieldView.SecureTextEntry ? "Show Password" : "Hide Password", UIControlState.Normal);
        }
    }
}
