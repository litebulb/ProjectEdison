using System;
using Edison.Mobile.Admin.Client.Core.ViewModels;
using Edison.Mobile.Admin.Client.iOS.Shared;
using Edison.Mobile.Admin.Client.iOS.Views;
using Edison.Mobile.iOS.Common.Views;
using UIKit;

namespace Edison.Mobile.Admin.Client.iOS.ViewControllers
{
    public class ChooseDeviceTypeViewController : BaseViewController<ChooseDeviceTypeViewModel>
    {
        CheckBoxItemView buttonCheckboxView;
        CheckBoxItemView soundSensorCheckboxView;
        CheckBoxItemView lightCheckboxView;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = Constants.Color.BackgroundLightGray;

            Title = "Set Up New Device";

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

            var padding = 16f;
            var circleSize = 32f;

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

            View.AddSubview(lightCheckboxView);

            lightCheckboxView.LeftAnchor.ConstraintEqualTo(deviceTypeLabel.LeftAnchor, constant: -Constants.Padding).Active = true;
            lightCheckboxView.TopAnchor.ConstraintEqualTo(soundSensorCheckboxView.BottomAnchor, constant: padding).Active = true;
            lightCheckboxView.RightAnchor.ConstraintEqualTo(topHalfLayoutGuide.RightAnchor, constant: -padding).Active = true;
            lightCheckboxView.HeightAnchor.ConstraintEqualTo(topHalfLayoutGuide.HeightAnchor, multiplier: 0.25f).Active = true;

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
    }
}
