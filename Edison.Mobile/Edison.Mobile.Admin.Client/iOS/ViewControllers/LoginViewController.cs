using System;
using Edison.Mobile.Admin.Client.Core.ViewModels;
using Edison.Mobile.Admin.Client.iOS.Shared;
using Edison.Mobile.iOS.Common.Shared;
using Edison.Mobile.iOS.Common.Views;
using Foundation;
using UIKit;

namespace Edison.Mobile.Admin.Client.iOS.ViewControllers
{
    public class LoginViewController : BaseViewController<LoginViewModel>
    {   
        UIButton signInButton;
        UIImageView sensorsImageView;
        UIImageView logoImageView;
        UILabel deviceSetupLabel;
        UIView containerView;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = Constants.Color.White;

            containerView = new UIView { TranslatesAutoresizingMaskIntoConstraints = false, Alpha = 0 };
            View.AddSubview(containerView);
            containerView.TopAnchor.ConstraintEqualTo(View.LayoutMarginsGuide.TopAnchor).Active = true;
            containerView.BottomAnchor.ConstraintEqualTo(View.LayoutMarginsGuide.BottomAnchor).Active = true;
            containerView.LeftAnchor.ConstraintEqualTo(View.LayoutMarginsGuide.LeftAnchor).Active = true;
            containerView.RightAnchor.ConstraintEqualTo(View.LayoutMarginsGuide.RightAnchor).Active = true;

            var deviceSetupLayoutGuide = new UILayoutGuide();
            containerView.AddLayoutGuide(deviceSetupLayoutGuide);
            deviceSetupLayoutGuide.CenterXAnchor.ConstraintEqualTo(containerView.CenterXAnchor).Active = true;
            deviceSetupLayoutGuide.CenterYAnchor.ConstraintEqualTo(containerView.CenterYAnchor).Active = true;

            sensorsImageView = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = Constants.Assets.SensorsGray,
            };
            containerView.AddSubview(sensorsImageView);

            sensorsImageView.CenterXAnchor.ConstraintEqualTo(containerView.CenterXAnchor).Active = true;
            sensorsImageView.CenterYAnchor.ConstraintEqualTo(containerView.CenterYAnchor).Active = true;

            deviceSetupLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.Eighteen),
                TextColor = Constants.Color.MidGray,
                Text = "Device Setup",
            };
            containerView.AddSubview(deviceSetupLabel);

            deviceSetupLabel.CenterXAnchor.ConstraintEqualTo(containerView.CenterXAnchor).Active = true;
            deviceSetupLabel.TopAnchor.ConstraintEqualTo(sensorsImageView.BottomAnchor, constant: Constants.Padding).Active = true;

            signInButton = new UIButton
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };

            signInButton.SetAttributedTitle(new NSAttributedString("SIGN IN", new UIStringAttributes
            {
                Font = Constants.Fonts.RubikMediumOfSize(Constants.Fonts.Size.Eighteen),
                ForegroundColor = Constants.Color.White,
            }), UIControlState.Normal);
            containerView.AddSubview(signInButton);
            signInButton.CenterXAnchor.ConstraintEqualTo(containerView.CenterXAnchor).Active = true;
            signInButton.TopAnchor.ConstraintEqualTo(deviceSetupLabel.BottomAnchor, constant: 60).Active = true;

            var signInButtonBackground = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = Constants.Color.DarkBlue,
            };
            containerView.InsertSubviewBelow(signInButtonBackground, signInButton);
            signInButtonBackground.CenterXAnchor.ConstraintEqualTo(signInButton.CenterXAnchor).Active = true;
            signInButtonBackground.CenterYAnchor.ConstraintEqualTo(signInButton.CenterYAnchor).Active = true;
            signInButtonBackground.HeightAnchor.ConstraintEqualTo(44).Active = true;
            signInButtonBackground.WidthAnchor.ConstraintEqualTo(containerView.WidthAnchor, multiplier: 0.5f).Active = true;

            var browserLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Text = "A browser window will open",
                TextColor = Constants.Color.MidGray,
                Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.Twelve),
            };
            containerView.AddSubview(browserLabel);
            browserLabel.CenterXAnchor.ConstraintEqualTo(containerView.CenterXAnchor).Active = true;
            browserLabel.TopAnchor.ConstraintEqualTo(signInButtonBackground.BottomAnchor, constant: 30).Active = true;

            logoImageView = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = Constants.Assets.LoginLogo,
            };
            containerView.AddSubview(logoImageView);
            logoImageView.BottomAnchor.ConstraintEqualTo(sensorsImageView.TopAnchor, constant: -60).Active = true;
            logoImageView.CenterXAnchor.ConstraintEqualTo(containerView.CenterXAnchor).Active = true;
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

        void OnDisplayLogin() => UIView.Animate(PlatformConstants.AnimationDuration, () => containerView.Alpha = 1);

        void OnNavigateToMainViewModel() => UIApplication.SharedApplication.KeyWindow.RootViewController = new UINavigationController(new MainViewController());
    }
}
