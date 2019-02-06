using System;
using Edison.Mobile.Admin.Client.Core.ViewModels;
using Edison.Mobile.Admin.Client.iOS.Shared;
using Edison.Mobile.Admin.Client.iOS.Views;
using Edison.Mobile.iOS.Common.Views;
using MapKit;
using UIKit;

namespace Edison.Mobile.Admin.Client.iOS.ViewControllers
{
    public class CompleteRegistrationViewController : BaseViewController<CompleteRegistrationViewModel>
    {
        MKMapView mapView;
        MKPinAnnotationView pinView;

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
                Number = 6,
            };

            View.AddSubview(circleNumberView);
            circleNumberView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, constant: padding).Active = true;
            circleNumberView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, constant: padding).Active = true;
            circleNumberView.WidthAnchor.ConstraintEqualTo(Constants.CircleNumberSize).Active = true;
            circleNumberView.HeightAnchor.ConstraintEqualTo(circleNumberView.WidthAnchor).Active = true;

            var importantDetailsLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextColor = Constants.Color.MidGray,
                Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.Eighteen),
                AdjustsFontSizeToFitWidth = true,
                MinimumScaleFactor = 0.1f,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0,
                Text = "Let's give this device some important details...",
            };

            View.AddSubview(importantDetailsLabel);
            importantDetailsLabel.LeftAnchor.ConstraintEqualTo(circleNumberView.RightAnchor, constant: padding).Active = true;
            importantDetailsLabel.CenterYAnchor.ConstraintEqualTo(circleNumberView.CenterYAnchor).Active = true;
            importantDetailsLabel.RightAnchor.ConstraintEqualTo(View.RightAnchor, constant: -padding).Active = true;


        }
    }
}
