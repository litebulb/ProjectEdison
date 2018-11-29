using System;
using Edison.Mobile.Common.ViewModels;
using Edison.Mobile.iOS.Common.Shared;
using Edison.Mobile.iOS.Common.Views;
using Edison.Mobile.User.Client.Core.ViewModels;
using Edison.Mobile.User.Client.iOS.Shared;
using UIKit;

namespace Edison.Mobile.User.Client.iOS.ViewControllers
{
    public class LoginViewController : BaseViewController<LoginViewModel>
    {
        UIButton signInButton;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = Constants.Color.White;

            signInButton = new UIButton
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Alpha = 0,
            };

            signInButton.SetTitle("Sign In", UIControlState.Normal);

            View.AddSubview(signInButton);
            signInButton.LeftAnchor.ConstraintEqualTo(View.LeftAnchor).Active = true;
            signInButton.RightAnchor.ConstraintEqualTo(View.RightAnchor).Active = true;
            signInButton.HeightAnchor.ConstraintEqualTo(60).Active = true;
            signInButton.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;
            signInButton.CenterYAnchor.ConstraintEqualTo(View.CenterYAnchor).Active = true;
            signInButton.SetTitleColor(UIColor.Blue, UIControlState.Normal);
        }

        protected override void BindEventHandlers()
        {
            base.BindEventHandlers();
            signInButton.TouchUpInside += OnSignInButtonTouchUpInside;
            ViewModel.OnDisplayLogin += OnDisplayLogin;
            ViewModel.OnNavigateToMainViewModel += OnNavigateToMainViewModel;
        }

        protected override void UnBindEventHandlers()
        {
            base.UnBindEventHandlers();
            signInButton.TouchUpInside -= OnSignInButtonTouchUpInside;
            ViewModel.OnDisplayLogin -= OnDisplayLogin;
            ViewModel.OnNavigateToMainViewModel -= OnNavigateToMainViewModel;
        }

        async void OnSignInButtonTouchUpInside(object sender, EventArgs e)
        {
            await ViewModel.SignIn();
        }

        void OnDisplayLogin() => UIView.Animate(PlatformConstants.AnimationDuration, () => signInButton.Alpha = 1);

        void OnNavigateToMainViewModel() => UIApplication.SharedApplication.KeyWindow.RootViewController = new MainViewController();
    }
}
