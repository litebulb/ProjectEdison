using System;
using CoreGraphics;
using Edison.Mobile.Admin.Client.Core.Shared;
using Edison.Mobile.Admin.Client.Core.ViewModels;
using Edison.Mobile.Admin.Client.iOS.Extensions;
using Edison.Mobile.Admin.Client.iOS.Shared;
using Edison.Mobile.Admin.Client.iOS.Views;
using Edison.Mobile.iOS.Common.Views;
using Foundation;
using UIKit;

namespace Edison.Mobile.Admin.Client.iOS.ViewControllers
{
    public class ChooseDeviceTypeViewController : BaseViewController<ChooseDeviceTypeViewModel>
    {
        CheckBoxItemView buttonCheckboxView;
        CheckBoxItemView soundSensorCheckboxView;
        CheckBoxItemView lightCheckboxView;

        UIButton nextButton;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = Constants.Color.BackgroundLightGray;

            Title = "Set Up New Device";

            NavigationItem.LeftBarButtonItem = new UIBarButtonItem(Constants.Assets.ArrowLeft, UIBarButtonItemStyle.Plain, (sender, e) =>
            {
                NavigationController.PopViewController(true);
            });
             
            NavigationController.InteractivePopGestureRecognizer.Delegate = null;

            var topHalfLayoutGuide = new UILayoutGuide();
            View.AddLayoutGuide(topHalfLayoutGuide);
            topHalfLayoutGuide.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor).Active = true;
            topHalfLayoutGuide.LeftAnchor.ConstraintEqualTo(View.LeftAnchor).Active = true;
            topHalfLayoutGuide.RightAnchor.ConstraintEqualTo(View.RightAnchor).Active = true;
            topHalfLayoutGuide.HeightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.HeightAnchor, multiplier: 0.5f).Active = true;

            var bottomHalfLayoutGuide = new UILayoutGuide();
            View.AddLayoutGuide(bottomHalfLayoutGuide);
            bottomHalfLayoutGuide.TopAnchor.ConstraintEqualTo(topHalfLayoutGuide.BottomAnchor).Active = true;
            bottomHalfLayoutGuide.LeftAnchor.ConstraintEqualTo(View.LeftAnchor).Active = true;
            bottomHalfLayoutGuide.RightAnchor.ConstraintEqualTo(View.RightAnchor).Active = true;
            bottomHalfLayoutGuide.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor).Active = true;

            var padding = Constants.Padding;
            var circleSize = Constants.CircleNumberSize;

            var oneCircleNumberView = new CircleNumberView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Number = 1,
            };

            View.AddSubview(oneCircleNumberView);
            oneCircleNumberView.TopAnchor.ConstraintEqualTo(topHalfLayoutGuide.TopAnchor, constant: padding).Active = true;
            oneCircleNumberView.LeftAnchor.ConstraintEqualTo(topHalfLayoutGuide.LeftAnchor, constant: padding).Active = true;
            oneCircleNumberView.WidthAnchor.ConstraintEqualTo(circleSize).Active = true;
            oneCircleNumberView.HeightAnchor.ConstraintEqualTo(oneCircleNumberView.WidthAnchor).Active = true;

            var deviceTypeLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextColor = Constants.Color.MidGray,
                Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.Eighteen),
                AdjustsFontSizeToFitWidth = true,
                MinimumScaleFactor = 0.1f,
                LineBreakMode = UILineBreakMode.Clip,
                Lines = 0,
                Text = "What type of device?",
            };

            View.AddSubview(deviceTypeLabel);
            deviceTypeLabel.LeftAnchor.ConstraintEqualTo(oneCircleNumberView.RightAnchor, constant: padding).Active = true;
            deviceTypeLabel.CenterYAnchor.ConstraintEqualTo(oneCircleNumberView.CenterYAnchor).Active = true;
            deviceTypeLabel.RightAnchor.ConstraintEqualTo(topHalfLayoutGuide.RightAnchor).Active = true;

            buttonCheckboxView = new CheckBoxItemView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextColor = Constants.Color.DarkGray,
                SelectedTextColor = Constants.Color.DarkBlue,
                Text = "Button",
            };

            buttonCheckboxView.OnTap += HandleCheckboxTap;

            View.AddSubview(buttonCheckboxView);

            buttonCheckboxView.LeftAnchor.ConstraintEqualTo(deviceTypeLabel.LeftAnchor, constant: -Constants.Padding).Active = true;
            buttonCheckboxView.TopAnchor.ConstraintEqualTo(deviceTypeLabel.BottomAnchor, constant: padding).Active = true;
            buttonCheckboxView.RightAnchor.ConstraintEqualTo(topHalfLayoutGuide.RightAnchor, constant: -padding).Active = true;
            buttonCheckboxView.HeightAnchor.ConstraintEqualTo(topHalfLayoutGuide.HeightAnchor, multiplier: 0.25f).Active = true;

            soundSensorCheckboxView = new CheckBoxItemView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextColor = Constants.Color.DarkGray,
                SelectedTextColor = Constants.Color.DarkBlue,
                Text = "Sound Sensor",
            };

            soundSensorCheckboxView.OnTap += HandleCheckboxTap;

            View.AddSubview(soundSensorCheckboxView);

            soundSensorCheckboxView.LeftAnchor.ConstraintEqualTo(deviceTypeLabel.LeftAnchor, constant: -Constants.Padding).Active = true;
            soundSensorCheckboxView.TopAnchor.ConstraintEqualTo(buttonCheckboxView.BottomAnchor, constant: padding).Active = true;
            soundSensorCheckboxView.RightAnchor.ConstraintEqualTo(topHalfLayoutGuide.RightAnchor, constant: -padding).Active = true;
            soundSensorCheckboxView.HeightAnchor.ConstraintEqualTo(topHalfLayoutGuide.HeightAnchor, multiplier: 0.25f).Active = true;

            lightCheckboxView = new CheckBoxItemView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextColor = Constants.Color.DarkGray,
                SelectedTextColor = Constants.Color.DarkBlue,
                Text = "Light",
            };

            lightCheckboxView.OnTap += HandleCheckboxTap;

            View.AddSubview(lightCheckboxView);

            lightCheckboxView.LeftAnchor.ConstraintEqualTo(deviceTypeLabel.LeftAnchor, constant: -Constants.Padding).Active = true;
            lightCheckboxView.TopAnchor.ConstraintEqualTo(soundSensorCheckboxView.BottomAnchor, constant: padding).Active = true;
            lightCheckboxView.RightAnchor.ConstraintEqualTo(topHalfLayoutGuide.RightAnchor, constant: -padding).Active = true;
            lightCheckboxView.HeightAnchor.ConstraintEqualTo(topHalfLayoutGuide.HeightAnchor, multiplier: 0.25f).Active = true;

            var twoCircleNumberView = new CircleNumberView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Number = 2,
            };

            View.AddSubview(twoCircleNumberView);
            twoCircleNumberView.TopAnchor.ConstraintEqualTo(bottomHalfLayoutGuide.TopAnchor, constant: padding).Active = true;
            twoCircleNumberView.LeftAnchor.ConstraintEqualTo(bottomHalfLayoutGuide.LeftAnchor, constant: padding).Active = true;
            twoCircleNumberView.WidthAnchor.ConstraintEqualTo(circleSize).Active = true;
            twoCircleNumberView.HeightAnchor.ConstraintEqualTo(twoCircleNumberView.WidthAnchor).Active = true;

            var powerLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextColor = Constants.Color.MidGray,
                Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.Eighteen),
                AdjustsFontSizeToFitWidth = true,
                MinimumScaleFactor = 0.1f,
                LineBreakMode = UILineBreakMode.Clip,
                Lines = 0,
                Text = "Turn on the device power.",
            };

            View.AddSubview(powerLabel);
            powerLabel.LeftAnchor.ConstraintEqualTo(twoCircleNumberView.RightAnchor, constant: padding).Active = true;
            powerLabel.CenterYAnchor.ConstraintEqualTo(twoCircleNumberView.CenterYAnchor).Active = true;
            powerLabel.RightAnchor.ConstraintEqualTo(bottomHalfLayoutGuide.RightAnchor).Active = true;

            nextButton = new UIButton
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = Constants.Color.DarkBlue,
                Enabled = false,
                Alpha = 0.4f,
            };

            nextButton.SetAttributedTitle(new NSAttributedString("NEXT", new UIStringAttributes
            {
                Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.Eighteen),
                ForegroundColor = Constants.Color.White,
            }), UIControlState.Normal);

            var buttonCenterLayoutGuide = new UILayoutGuide();
            View.AddLayoutGuide(buttonCenterLayoutGuide);
            buttonCenterLayoutGuide.TopAnchor.ConstraintEqualTo(powerLabel.BottomAnchor).Active = true;
            buttonCenterLayoutGuide.BottomAnchor.ConstraintEqualTo(bottomHalfLayoutGuide.BottomAnchor).Active = true;

            View.AddSubview(nextButton);
            nextButton.WidthAnchor.ConstraintEqualTo(bottomHalfLayoutGuide.WidthAnchor, multiplier: 0.5f).Active = true;
            nextButton.HeightAnchor.ConstraintEqualTo(44).Active = true;
            nextButton.CenterYAnchor.ConstraintEqualTo(buttonCenterLayoutGuide.CenterYAnchor).Active = true;
            nextButton.CenterXAnchor.ConstraintEqualTo(bottomHalfLayoutGuide.CenterXAnchor).Active = true;

            nextButton.AddStandardShadow();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            NavigationController.NavigationBar.SetBackgroundImage(null, UIBarMetrics.Default);
            NavigationController.NavigationBar.ShadowImage = null;
            NavigationController.NavigationBar.Translucent = true;
            NavigationController.NavigationBar.BackgroundColor = Constants.Color.DarkBlue;
            NavigationController.NavigationBar.BarTintColor = Constants.Color.DarkBlue;
            NavigationController.NavigationBar.TintColor = Constants.Color.White;

            NavigationController.NavigationBar.TitleTextAttributes = new UIStringAttributes
            {
                ForegroundColor = Constants.Color.White,
                Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.Eighteen),
            };
        }

        protected override void BindEventHandlers()
        {
            base.BindEventHandlers();
            nextButton.TouchUpInside += HandleNextButtonTouchUpInside;
        }

        protected override void UnBindEventHandlers()
        {
            base.UnBindEventHandlers();
            nextButton.TouchUpInside -= HandleNextButtonTouchUpInside;
        }

        void HandleCheckboxTap(object sender, EventArgs e)
        {
            var checkboxes = new CheckBoxItemView[]
            {
                buttonCheckboxView,
                lightCheckboxView,
                soundSensorCheckboxView,
            };

            foreach (var checkbox in checkboxes)
            {
                var match = checkbox == sender;
                checkbox.Selected = match;

                if (match)
                {
                    if (sender == buttonCheckboxView) ViewModel.SetDeviceType(DeviceType.Button);
                    if (sender == lightCheckboxView) ViewModel.SetDeviceType(DeviceType.Light);
                    if (sender == soundSensorCheckboxView) ViewModel.SetDeviceType(DeviceType.SoundSensor);
                }
            }

            nextButton.Enabled = true;
            nextButton.Alpha = 1;
        }

        void HandleNextButtonTouchUpInside(object sender, EventArgs e)
        {
            var registerDeviceViewController = new RegisterDeviceViewController();
            NavigationController.PushViewController(registerDeviceViewController, true);
        }
    }
}
