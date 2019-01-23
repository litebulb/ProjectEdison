using System;
using Edison.Core.Common.Models;
using Edison.Mobile.Admin.Client.Core.ViewModels;
using Edison.Mobile.Admin.Client.iOS.Shared;
using Edison.Mobile.iOS.Common.Views;
using UIKit;
using MapKit;
using CoreLocation;
using Edison.Mobile.Admin.Client.iOS.Views;
using Edison.Mobile.Admin.Client.iOS.Extensions;
using Foundation;
using CoreGraphics;

namespace Edison.Mobile.Admin.Client.iOS.ViewControllers
{
    public class ManageDeviceViewController : BaseViewController<ManageDeviceViewModel>
    {
        MKMapView mapView;
        MKPinAnnotationView pinView;

        UIScrollView scrollView;

        TextFieldView buildingTextFieldView;
        TextFieldView floorTextFieldView;
        TextFieldView roomTextFieldView;
        TextFieldView wifiTextFieldView;

        UIButton doneButton;

        NSLayoutConstraint scrollViewBottomConstraint;

        public ManageDeviceViewController(DeviceModel deviceModel)
        {
            ViewModel.DeviceModel = deviceModel;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = Constants.Color.BackgroundLightGray;

            Title = "Manage Device";

            NavigationItem.LeftBarButtonItem = new UIBarButtonItem(Constants.Assets.ArrowLeft, UIBarButtonItemStyle.Plain, (sender, e) =>
            {
                NavigationController.PopViewController(true);
            });

            NavigationController.InteractivePopGestureRecognizer.Delegate = null;
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

            scrollView = new UIScrollView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                AlwaysBounceVertical = true,
            };

            View.AddSubview(scrollView);

            scrollView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor).Active = true;
            scrollView.RightAnchor.ConstraintEqualTo(View.RightAnchor).Active = true;
            scrollView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor).Active = true;
            scrollViewBottomConstraint = scrollView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor);
            scrollViewBottomConstraint.Active = true;

            var deviceTypeImageView = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = ViewModel.DeviceModel?.Sensor ?? false ? Constants.Assets.Lines : Constants.Assets.Power,
            };

            scrollView.AddSubview(deviceTypeImageView);

            deviceTypeImageView.TopAnchor.ConstraintEqualTo(scrollView.TopAnchor, constant: Constants.Padding).Active = true;
            deviceTypeImageView.LeftAnchor.ConstraintEqualTo(scrollView.LeftAnchor, constant: Constants.Padding).Active = true;
            deviceTypeImageView.WidthAnchor.ConstraintEqualTo(32).Active = true;
            deviceTypeImageView.HeightAnchor.ConstraintEqualTo(deviceTypeImageView.WidthAnchor).Active = true;

            var deviceLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Text = ViewModel.DeviceModel.Name,
                Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.TwentyFour),
                TextColor = Constants.Color.DarkGray,
            };

            scrollView.AddSubview(deviceLabel);

            deviceLabel.CenterYAnchor.ConstraintEqualTo(deviceTypeImageView.CenterYAnchor).Active = true;
            deviceLabel.LeftAnchor.ConstraintEqualTo(deviceTypeImageView.RightAnchor, constant: Constants.Padding).Active = true;
            deviceLabel.RightAnchor.ConstraintEqualTo(scrollView.RightAnchor, constant: -Constants.Padding).Active = true;

            var locationLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Text = "Device Location",
                Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.Fourteen),
                TextColor = Constants.Color.DarkGray,
            };

            scrollView.AddSubview(locationLabel);

            locationLabel.TopAnchor.ConstraintEqualTo(deviceTypeImageView.BottomAnchor, constant: Constants.Padding).Active = true;
            locationLabel.LeftAnchor.ConstraintEqualTo(scrollView.LeftAnchor, constant: Constants.Padding).Active = true;

            mapView = new MKMapView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                CenterCoordinate = ViewModel.DeviceModel.Geolocation != null
                    ? new CLLocationCoordinate2D(ViewModel.DeviceModel.Geolocation.Latitude, ViewModel.DeviceModel.Geolocation.Longitude)
                    : new CLLocationCoordinate2D(),
            };

            scrollView.AddSubview(mapView);

            mapView.AddStandardShadow();

            mapView.TopAnchor.ConstraintEqualTo(locationLabel.BottomAnchor, constant: Constants.Padding).Active = true;
            mapView.LeftAnchor.ConstraintEqualTo(scrollView.LeftAnchor).Active = true;
            mapView.WidthAnchor.ConstraintEqualTo(scrollView.WidthAnchor).Active = true;
            mapView.HeightAnchor.ConstraintEqualTo(scrollView.HeightAnchor, multiplier: 0.25f).Active = true;

            pinView = new MKPinAnnotationView(CGRect.Empty)
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };

            var pinHeight = 39;

            scrollView.AddSubview(pinView);

            pinView.CenterXAnchor.ConstraintEqualTo(mapView.CenterXAnchor, constant: pinView.CenterOffset.X).Active = true;
            pinView.CenterYAnchor.ConstraintEqualTo(mapView.CenterYAnchor, constant: pinView.CenterOffset.Y).Active = true;
            pinView.HeightAnchor.ConstraintEqualTo(pinHeight).Active = true;
            pinView.WidthAnchor.ConstraintEqualTo(pinHeight).Active = true;

            var moveLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                TextColor = Constants.Color.MidGray,
                Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.Twelve),
                Text = "MOVE THE PIN AS NEEDED TO MAKE SURE ITS LOCATION ACCURATELY REPRESENTS A PRECISE SPOT WHERE THE DEVICE WILL BE.",
            };

            scrollView.AddSubview(moveLabel);

            moveLabel.LeftAnchor.ConstraintEqualTo(scrollView.LeftAnchor, constant: Constants.Padding).Active = true;
            moveLabel.TopAnchor.ConstraintEqualTo(mapView.BottomAnchor, constant: Constants.Padding).Active = true;
            moveLabel.WidthAnchor.ConstraintEqualTo(View.WidthAnchor, constant: -Constants.Padding).Active = true;

            //buildingTextFieldView = new TextFieldView
            //{
            //    TranslatesAutoresizingMaskIntoConstraints = false,
            //    LabelText = "Building",
            //};

            //scrollView.AddSubview(buildingTextFieldView);

            //buildingTextFieldView.TopAnchor.ConstraintEqualTo(moveLabel.BottomAnchor, constant: Constants.Padding).Active = true;
            //buildingTextFieldView.LeftAnchor.ConstraintEqualTo(scrollView.LeftAnchor).Active = true;
            //buildingTextFieldView.RightAnchor.ConstraintEqualTo(scrollView.RightAnchor).Active = true;

            //floorTextFieldView = new TextFieldView
            //{
            //    TranslatesAutoresizingMaskIntoConstraints = false,
            //    LabelText = "Floor",
            //};

            //scrollView.AddSubview(floorTextFieldView);

            //floorTextFieldView.TopAnchor.ConstraintEqualTo(buildingTextFieldView.BottomAnchor, constant: Constants.Padding / 2).Active = true;
            //floorTextFieldView.LeftAnchor.ConstraintEqualTo(scrollView.LeftAnchor).Active = true;
            //floorTextFieldView.RightAnchor.ConstraintEqualTo(scrollView.RightAnchor).Active = true;

            //roomTextFieldView = new TextFieldView
            //{
            //    TranslatesAutoresizingMaskIntoConstraints = false,
            //    LabelText = "Room",
            //};

            //scrollView.AddSubview(roomTextFieldView);

            //roomTextFieldView.TopAnchor.ConstraintEqualTo(floorTextFieldView.BottomAnchor, constant: Constants.Padding / 2).Active = true;
            //roomTextFieldView.LeftAnchor.ConstraintEqualTo(scrollView.LeftAnchor).Active = true;
            //roomTextFieldView.RightAnchor.ConstraintEqualTo(scrollView.RightAnchor).Active = true;

            //wifiTextFieldView = new TextFieldView
            //{
            //    TranslatesAutoresizingMaskIntoConstraints = false,
            //    LabelText = "WiFi",
            //};

            //scrollView.AddSubview(wifiTextFieldView);

            //wifiTextFieldView.TopAnchor.ConstraintEqualTo(roomTextFieldView.BottomAnchor, constant: Constants.Padding * 2).Active = true;
            //wifiTextFieldView.LeftAnchor.ConstraintEqualTo(scrollView.LeftAnchor).Active = true;
            //wifiTextFieldView.RightAnchor.ConstraintEqualTo(scrollView.RightAnchor).Active = true;

            //var bottomAreaLayoutGuide = new UILayoutGuide();

            //scrollView.AddLayoutGuide(bottomAreaLayoutGuide);

            //bottomAreaLayoutGuide.TopAnchor.ConstraintEqualTo(wifiTextFieldView.BottomAnchor).Active = true;
            //bottomAreaLayoutGuide.BottomAnchor.ConstraintEqualTo(scrollView.SafeAreaLayoutGuide.BottomAnchor).Active = true;

            //doneButton = new UIButton
            //{
            //    TranslatesAutoresizingMaskIntoConstraints = false,
            //    BackgroundColor = Constants.Color.DarkBlue,
            //};

            //doneButton.SetAttributedTitle(new NSAttributedString("DONE", new UIStringAttributes
            //{
            //    Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.Eighteen),
            //    ForegroundColor = Constants.Color.White,
            //}), UIControlState.Normal);

            //doneButton.AddStandardShadow();

            //scrollView.AddSubview(doneButton);

            //doneButton.WidthAnchor.ConstraintEqualTo(scrollView.WidthAnchor, multiplier: 0.5f).Active = true;
            //doneButton.HeightAnchor.ConstraintEqualTo(44).Active = true;
            //doneButton.CenterXAnchor.ConstraintEqualTo(scrollView.CenterXAnchor).Active = true;
            //doneButton.CenterYAnchor.ConstraintEqualTo(bottomAreaLayoutGuide.CenterYAnchor).Active = true;
        }
    }
}
