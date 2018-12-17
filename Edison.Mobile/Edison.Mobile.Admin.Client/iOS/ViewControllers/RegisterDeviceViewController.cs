using System;
using CoreGraphics;
using Edison.Mobile.Admin.Client.Core.Services;
using Edison.Mobile.Admin.Client.Core.Shared;
using Edison.Mobile.Admin.Client.Core.ViewModels;
using Edison.Mobile.Admin.Client.iOS.Shared;
using Edison.Mobile.Admin.Client.iOS.Views;
using Edison.Mobile.iOS.Common.Views;
using Foundation;
using UIKit;

namespace Edison.Mobile.Admin.Client.iOS.ViewControllers
{
    public class RegisterDeviceViewController : BaseViewController<RegisterDeviceViewModel>
    {
        CameraView cameraView;
        UIButton noQRCodeButton;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationItem.LeftBarButtonItem = new UIBarButtonItem(Constants.Assets.ArrowLeft, UIBarButtonItemStyle.Plain, (sender, e) =>
            {
                NavigationController.PopViewController(true);
            });

            NavigationController.NavigationBar.TitleTextAttributes = new UIStringAttributes
            {
                ForegroundColor = Constants.Color.White,
                Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.Eighteen),
            };

            NavigationController.InteractivePopGestureRecognizer.Delegate = null;

            View.BackgroundColor = Constants.Color.BackgroundLightGray;

            Title = $"Set Up {ViewModel.DeviceTypeAsString}";

            cameraView = new CameraView { TranslatesAutoresizingMaskIntoConstraints = false };

            View.AddSubview(cameraView);

            cameraView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor).Active = true;
            cameraView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor).Active = true;
            cameraView.RightAnchor.ConstraintEqualTo(View.RightAnchor).Active = true;
            cameraView.HeightAnchor.ConstraintEqualTo(View.HeightAnchor, multiplier: 0.55f).Active = true;

            var padding = Constants.Padding;
            var circleSize = Constants.CircleNumberSize;

            var circleView = new CircleNumberView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Number = 3,
            };

            View.AddSubview(circleView);

            circleView.TopAnchor.ConstraintEqualTo(cameraView.BottomAnchor, constant: padding).Active = true;
            circleView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, constant: padding).Active = true;
            circleView.WidthAnchor.ConstraintEqualTo(circleSize).Active = true;
            circleView.HeightAnchor.ConstraintEqualTo(circleView.WidthAnchor).Active = true;

            var scanLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextColor = Constants.Color.MidGray,
                Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.Eighteen),
                AdjustsFontSizeToFitWidth = true,
                MinimumScaleFactor = 0.1f,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0,
                Text = $"Scan the QR Code on the {ViewModel.DeviceTypeAsString.ToLower()} to connect it to this phone.",
            };

            View.AddSubview(scanLabel);

            scanLabel.LeftAnchor.ConstraintEqualTo(circleView.RightAnchor, constant: padding).Active = true;
            scanLabel.CenterYAnchor.ConstraintEqualTo(circleView.CenterYAnchor).Active = true;
            scanLabel.RightAnchor.ConstraintEqualTo(View.RightAnchor, constant: -padding).Active = true;

            var bottomLayoutGuide = new UILayoutGuide();

            View.AddLayoutGuide(bottomLayoutGuide);

            bottomLayoutGuide.TopAnchor.ConstraintEqualTo(scanLabel.BottomAnchor).Active = true;
            bottomLayoutGuide.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor).Active = true;

            noQRCodeButton = new UIButton
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = Constants.Color.DarkBlue,
            };

            noQRCodeButton.SetAttributedTitle(new NSAttributedString("NO QR CODE?", new UIStringAttributes
            {
                Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.Eighteen),
                ForegroundColor = Constants.Color.White,
            }), UIControlState.Normal);


            noQRCodeButton.Layer.ShadowColor = Constants.Color.DarkGray.CGColor;
            noQRCodeButton.Layer.ShadowRadius = 3;
            noQRCodeButton.Layer.ShadowOffset = new CGSize(1, 1);
            noQRCodeButton.Layer.ShadowOpacity = 0.4f;
            noQRCodeButton.Layer.MasksToBounds = false;

            View.AddSubview(noQRCodeButton);

            noQRCodeButton.WidthAnchor.ConstraintEqualTo(View.WidthAnchor, multiplier: 0.5f).Active = true;
            noQRCodeButton.HeightAnchor.ConstraintEqualTo(44).Active = true;
            noQRCodeButton.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;
            noQRCodeButton.CenterYAnchor.ConstraintEqualTo(bottomLayoutGuide.CenterYAnchor).Active = true;
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            if (!cameraView.IsRunning)
            {
                cameraView.Start();            
            }
        }

        protected override void BindEventHandlers()
        {
            base.BindEventHandlers();
            noQRCodeButton.TouchUpInside += HandleNoQRCodeButtonTouchUpInside;
        }

        protected override void UnBindEventHandlers()
        {
            base.UnBindEventHandlers();
            noQRCodeButton.TouchUpInside -= HandleNoQRCodeButtonTouchUpInside;
        }

        void HandleNoQRCodeButtonTouchUpInside(object sender, EventArgs e)
        {
            var manualConnectViewController = new ManualConnectViewController();
            var navController = new UINavigationController(manualConnectViewController);
            PresentViewController(navController, true, null);
        }
    }
}
