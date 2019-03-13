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
using Edison.Mobile.iOS.Common.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Edison.Mobile.Admin.Client.Core.ViewModels.ManageDeviceViewModel;

namespace Edison.Mobile.Admin.Client.iOS.ViewControllers
{
    public class ManageDeviceViewController : BaseViewController<ManageDeviceViewModel>, IMKMapViewDelegate
    {
        bool isZoomedIn;

        MKMapView mapView;
        MKPinAnnotationView pinView;

        UIScrollView scrollView;

        TextFieldView nameTextFieldView;
        SwitchFieldView enabledSwitchFieldView;
        TextFieldView buildingTextFieldView;
        TextFieldView floorTextFieldView;
        TextFieldView roomTextFieldView;
        TextFieldView wifiTextFieldView;

        NSObject keyboardWillShowNotificationToken;
        NSObject keyboardWillHideNotificationToken;

        NSLayoutConstraint scrollViewBottomConstraint;

        TextFieldView textFieldViewFirstResponder;

        List<TextFieldView> textFieldViews;
        
        private DeviceModel deviceModel;

        public ManageDeviceViewController(DeviceModel deviceModel = null)
        {
            this.deviceModel = deviceModel;
            ViewModel.IsOnboardingStepSix = deviceModel == null;
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

            NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Done, async (object sender, EventArgs e) =>
            {
                NavigationItem.RightBarButtonItem.Enabled = false;

                UpdateCurrentDeviceAttributes();

                await ViewModel.UpdateDevice();

                NavigationItem.RightBarButtonItem.Enabled = true;
            });

            NavigationController.InteractivePopGestureRecognizer.Delegate = null;


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

            var padding = Constants.Padding;
            var halfPadding = padding / 2;
            var doublePadding = padding * 2;

            scrollView = new UIScrollView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                AlwaysBounceVertical = true,
                ShowsVerticalScrollIndicator = true,
            };

            View.AddSubview(scrollView);

            scrollView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor).Active = true;
            scrollView.RightAnchor.ConstraintEqualTo(View.RightAnchor).Active = true;
            scrollView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor).Active = true;
            scrollViewBottomConstraint = scrollView.BottomAnchor.ConstraintEqualTo(View.BottomAnchor);
            scrollViewBottomConstraint.Active = true;

            var deviceTypeImageView = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = ViewModel.CurrentDeviceModel?.Sensor ?? false ? Constants.Assets.Lines : Constants.Assets.Power,
            };

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

            if (!ViewModel.IsOnboardingStepSix)
            {
                scrollView.AddSubview(deviceTypeImageView);

                deviceTypeImageView.TopAnchor.ConstraintEqualTo(scrollView.TopAnchor, constant: padding).Active = true;
                deviceTypeImageView.LeftAnchor.ConstraintEqualTo(scrollView.LeftAnchor, constant: padding).Active = true;
                deviceTypeImageView.WidthAnchor.ConstraintEqualTo(32).Active = true;
                deviceTypeImageView.HeightAnchor.ConstraintEqualTo(deviceTypeImageView.WidthAnchor).Active = true;

                var deviceLabel = new UILabel
                {
                    TranslatesAutoresizingMaskIntoConstraints = false,
                    Text = ViewModel.CurrentDeviceModel.Name,
                    Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.TwentyFour),
                    TextColor = Constants.Color.DarkGray,
                };

                scrollView.AddSubview(deviceLabel);

                deviceLabel.CenterYAnchor.ConstraintEqualTo(deviceTypeImageView.CenterYAnchor).Active = true;
                deviceLabel.LeftAnchor.ConstraintEqualTo(deviceTypeImageView.RightAnchor, constant: padding).Active = true;
                deviceLabel.RightAnchor.ConstraintEqualTo(scrollView.RightAnchor, constant: -padding).Active = true;
            }
            else
            {
                var circleNumberView = new CircleNumberView
                {
                    TranslatesAutoresizingMaskIntoConstraints = false,
                    Number = 6,
                };

                scrollView.AddSubview(circleNumberView);
                circleNumberView.TopAnchor.ConstraintEqualTo(scrollView.TopAnchor, constant: padding).Active = true;
                circleNumberView.LeftAnchor.ConstraintEqualTo(scrollView.LeftAnchor, constant: padding).Active = true;
                circleNumberView.WidthAnchor.ConstraintEqualTo(Constants.CircleNumberSize).Active = true;
                circleNumberView.HeightAnchor.ConstraintEqualTo(circleNumberView.WidthAnchor).Active = true;



                scrollView.AddSubview(importantDetailsLabel);
                importantDetailsLabel.LeftAnchor.ConstraintEqualTo(circleNumberView.RightAnchor, constant: padding).Active = true;
                importantDetailsLabel.CenterYAnchor.ConstraintEqualTo(circleNumberView.CenterYAnchor).Active = true;
                importantDetailsLabel.RightAnchor.ConstraintEqualTo(View.RightAnchor, constant: -padding).Active = true;
            }

            var locationLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Text = "Device Location",
                Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.Fourteen),
                TextColor = Constants.Color.DarkGray,
            };

            scrollView.AddSubview(locationLabel);

            locationLabel.TopAnchor.ConstraintEqualTo(!ViewModel.IsOnboardingStepSix ? deviceTypeImageView.BottomAnchor : importantDetailsLabel.BottomAnchor, constant: padding).Active = true;
            locationLabel.LeftAnchor.ConstraintEqualTo(scrollView.LeftAnchor, constant: padding).Active = true;

            mapView = new MKMapView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                ShowsUserLocation = true,
                CenterCoordinate = ViewModel.CurrentDeviceModel.Geolocation != null
                    ? new CLLocationCoordinate2D(ViewModel.CurrentDeviceModel.Geolocation.Latitude, ViewModel.CurrentDeviceModel.Geolocation.Longitude)
                    : new CLLocationCoordinate2D(),
                Delegate = this,
            };

            scrollView.AddSubview(mapView);

            mapView.AddStandardShadow();

            mapView.TopAnchor.ConstraintEqualTo(locationLabel.BottomAnchor, constant: padding).Active = true;
            mapView.LeftAnchor.ConstraintEqualTo(scrollView.LeftAnchor).Active = true;
            mapView.WidthAnchor.ConstraintEqualTo(scrollView.WidthAnchor).Active = true;
            mapView.HeightAnchor.ConstraintEqualTo(View.HeightAnchor, multiplier: 0.25f).Active = true;

            if (ViewModel.CurrentDeviceModel.Geolocation == null)
            {
                Task.Run(async () =>
                {
                    var edisonLocation = await ViewModel.GetLastKnownLocation();

                    if (edisonLocation == null) return;

                    InvokeOnMainThread(() => mapView.CenterCoordinate = new CLLocationCoordinate2D
                    {
                        Latitude = edisonLocation.Latitude,
                        Longitude = edisonLocation.Longitude,
                    });
                });
            }

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

            moveLabel.LeftAnchor.ConstraintEqualTo(scrollView.LeftAnchor, constant: padding).Active = true;
            moveLabel.TopAnchor.ConstraintEqualTo(mapView.BottomAnchor, constant: padding).Active = true;
            moveLabel.WidthAnchor.ConstraintEqualTo(View.WidthAnchor, constant: -padding).Active = true;

            nameTextFieldView = new TextFieldView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                LabelText = "Name",
            };

            scrollView.AddSubview(nameTextFieldView);

            nameTextFieldView.TopAnchor.ConstraintEqualTo(moveLabel.BottomAnchor, constant: padding).Active = true;
            nameTextFieldView.LeftAnchor.ConstraintEqualTo(scrollView.LeftAnchor).Active = true;
            nameTextFieldView.WidthAnchor.ConstraintEqualTo(View.WidthAnchor).Active = true;

            enabledSwitchFieldView = new SwitchFieldView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                LabelText = "Enabled",
            };

            scrollView.AddSubview(enabledSwitchFieldView);

            enabledSwitchFieldView.TopAnchor.ConstraintEqualTo(nameTextFieldView.BottomAnchor, constant: padding).Active = true;
            enabledSwitchFieldView.LeftAnchor.ConstraintEqualTo(scrollView.LeftAnchor).Active = true;
            enabledSwitchFieldView.WidthAnchor.ConstraintEqualTo(View.WidthAnchor).Active = true;

            buildingTextFieldView = new TextFieldView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                LabelText = "Building",
            };

            scrollView.AddSubview(buildingTextFieldView);

            buildingTextFieldView.TopAnchor.ConstraintEqualTo(enabledSwitchFieldView.BottomAnchor, constant: padding).Active = true;
            buildingTextFieldView.LeftAnchor.ConstraintEqualTo(scrollView.LeftAnchor).Active = true;
            buildingTextFieldView.WidthAnchor.ConstraintEqualTo(View.WidthAnchor).Active = true;

            floorTextFieldView = new TextFieldView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                LabelText = "Floor",
            };

            scrollView.AddSubview(floorTextFieldView);

            floorTextFieldView.TopAnchor.ConstraintEqualTo(buildingTextFieldView.BottomAnchor, constant: halfPadding).Active = true;
            floorTextFieldView.LeftAnchor.ConstraintEqualTo(scrollView.LeftAnchor).Active = true;
            floorTextFieldView.WidthAnchor.ConstraintEqualTo(View.WidthAnchor).Active = true;

            roomTextFieldView = new TextFieldView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                LabelText = "Room",
            };

            scrollView.AddSubview(roomTextFieldView);

            roomTextFieldView.TopAnchor.ConstraintEqualTo(floorTextFieldView.BottomAnchor, constant: halfPadding).Active = true;
            roomTextFieldView.LeftAnchor.ConstraintEqualTo(scrollView.LeftAnchor).Active = true;
            roomTextFieldView.WidthAnchor.ConstraintEqualTo(View.WidthAnchor).Active = true;

            if (ViewModel.IsOnboardingStepSix)
            {
                roomTextFieldView.BottomAnchor.ConstraintEqualTo(scrollView.BottomAnchor, constant: -halfPadding).Active = true;
            }
            else
            {
                wifiTextFieldView = new TextFieldView
                {
                    TranslatesAutoresizingMaskIntoConstraints = false,
                    LabelText = "WiFi",
                };

                scrollView.AddSubview(wifiTextFieldView);

                wifiTextFieldView.TopAnchor.ConstraintEqualTo(roomTextFieldView.BottomAnchor, constant: padding).Active = true;
                wifiTextFieldView.LeftAnchor.ConstraintEqualTo(scrollView.LeftAnchor).Active = true;
                wifiTextFieldView.WidthAnchor.ConstraintEqualTo(View.WidthAnchor).Active = true;
                wifiTextFieldView.BottomAnchor.ConstraintEqualTo(scrollView.BottomAnchor, constant: -halfPadding).Active = true;
            }

            textFieldViews = new List<TextFieldView>
            {
                nameTextFieldView,
                buildingTextFieldView,
                floorTextFieldView,
                roomTextFieldView,
            };

            textFieldViews.ForEach(t => t.ReturnKeyType = UIReturnKeyType.Done);
            
        }

        protected override void BindEventHandlers()
        {
            base.BindEventHandlers();

            keyboardWillShowNotificationToken = UIKeyboard.Notifications.ObserveWillShow(HandleKeyboardWillShow);
            keyboardWillHideNotificationToken = UIKeyboard.Notifications.ObserveWillHide(HandleKeyboardWillHide);

            if (mapView.Delegate == null) mapView.Delegate = this;

            enabledSwitchFieldView.OnSwitchValueChanged += EnabledSwitchFieldViewOnSwitchValueChanged;

            ViewModel.OnDeviceUpdated += HandleOnDeviceUpdated;
            ViewModel.OnDeviceLoaded += HandleOnDeviceLoaded;
            ViewModel.OnDeviceLoadFail += HandleOnDeviceLoadFail;

            foreach (var textFieldView in textFieldViews)
            {
                textFieldView.OnEditingBegan += OnTextFieldEditingBegan;
                textFieldView.OnTextFieldViewReturned += OnTextFieldDone;
                textFieldView.OnEditingEnded += OnTextFieldEditingEnded;
            }
        }

        protected override void UnBindEventHandlers()
        {
            base.UnBindEventHandlers();

            if (keyboardWillShowNotificationToken != null) NSNotificationCenter.DefaultCenter.RemoveObserver(keyboardWillShowNotificationToken);
            if (keyboardWillHideNotificationToken != null) NSNotificationCenter.DefaultCenter.RemoveObserver(keyboardWillHideNotificationToken);

            mapView.Delegate = null;

            ViewModel.OnDeviceUpdated -= HandleOnDeviceUpdated;
            ViewModel.OnDeviceLoaded -= HandleOnDeviceLoaded;
            ViewModel.OnDeviceLoadFail -= HandleOnDeviceLoadFail;

            enabledSwitchFieldView.OnSwitchValueChanged -= EnabledSwitchFieldViewOnSwitchValueChanged;

            foreach (var textFieldView in textFieldViews)
            {
                textFieldView.OnEditingBegan -= OnTextFieldEditingBegan;
                textFieldView.OnTextFieldViewReturned -= OnTextFieldDone;
                textFieldView.OnEditingEnded -= OnTextFieldEditingEnded;
            }
        }

        [Export("mapView:didUpdateUserLocation:")]
        public void DidUpdateUserLocation(MKMapView mapView, MKUserLocation userLocation)
        {
            if (!isZoomedIn)
            {
                var mapRegion = new MKCoordinateRegion(mapView.UserLocation.Coordinate, new MKCoordinateSpan(0.001, 0.001));
                mapView.SetRegion(mapRegion, true);
                isZoomedIn = true;
            }
        }

        [Export("mapView:regionDidChangeAnimated:")]
        public void RegionChanged(MKMapView mapView, bool animated)
        {
            if (!isZoomedIn) return;

            var centerCoordinate = mapView.Region.Center;
            ViewModel.CurrentDeviceModel.Geolocation = new Geolocation
            {
                Latitude = centerCoordinate.Latitude,
                Longitude = centerCoordinate.Longitude,
            };
        }

        void HandleKeyboardWillShow(object sender, UIKeyboardEventArgs e)
        {
            scrollViewBottomConstraint.Constant = -e.FrameEnd.Height;

            UIView.BeginAnimations(null);
            UIView.SetAnimationDuration(e.AnimationDuration);
            UIView.SetAnimationCurve(e.AnimationCurve);
            UIView.SetAnimationBeginsFromCurrentState(true);
            View.LayoutIfNeeded();

            EnsureFirstResponderTextFieldIsInView(e.FrameEnd.Height);

            UIView.CommitAnimations();
        }
        
        void UpdateCurrentDeviceAttributes()
        {
            if (textFieldViews != null && textFieldViews.Count > 0)
            {
                for (int i = 0; i < textFieldViews.Count; i++)
                {
                    var textFieldView = textFieldViews[i];
                    switch (i)
                    {
                        case 0:
                            if (textFieldView.Text != ViewModel.CurrentDeviceModel.Name)
                                ViewModel.CurrentDeviceModel.Name = textFieldView.Text;
                            break;
                        case 1:
                            if (textFieldView.Text != ViewModel.CurrentDeviceModel.Location1)
                                ViewModel.CurrentDeviceModel.Location1 = textFieldView.Text;
                            break;
                        case 2:
                            if (textFieldView.Text != ViewModel.CurrentDeviceModel.Location2)
                                ViewModel.CurrentDeviceModel.Location2 = textFieldView.Text;
                            break;
                        case 3:
                            if (textFieldView.Text != ViewModel.CurrentDeviceModel.Location3)
                                ViewModel.CurrentDeviceModel.Location3 = textFieldView.Text;
                            break;   
                    }
                }
            }
        }

        void EnsureFirstResponderTextFieldIsInView(nfloat keyboardHeight, bool animated = false)
        {
            if (textFieldViewFirstResponder != null)
            {
                var textFieldFrame = View.ConvertRectFromView(textFieldViewFirstResponder.Frame, scrollView);
                var keyboardAreaY = View.Bounds.Height - keyboardHeight;
                var textViewBottomMargin = 0;
                if (textFieldFrame.Bottom + textViewBottomMargin >= keyboardAreaY)
                {
                    var offsetDelta = (float)Math.Abs((textFieldFrame.Bottom + textViewBottomMargin) - keyboardAreaY);
                    scrollView.SetContentOffset(new CGPoint
                    {
                        X = 0,
                        Y = scrollView.ContentOffset.Y + offsetDelta,
                    }, animated);
                }
            }
        }

        void HandleKeyboardWillHide(object sender, UIKeyboardEventArgs e)
        {
            scrollViewBottomConstraint.Constant = 0;

            UIView.BeginAnimations(null);
            UIView.SetAnimationDuration(e.AnimationDuration);
            UIView.SetAnimationCurve(e.AnimationCurve);
            UIView.SetAnimationBeginsFromCurrentState(true);
            View.LayoutIfNeeded();
            UIView.CommitAnimations();
        }

        void OnTextFieldDone(object sender, EventArgs e)
        {
            (sender as TextFieldView).ResignFirstResponder();
            textFieldViewFirstResponder = null;
        }

        void OnTextFieldEditingBegan(object sender, EventArgs e)
        {
            if (sender is TextFieldView textFieldView)
            {
                textFieldViewFirstResponder = textFieldView;
            }
        }
        
        void HandleOnDeviceLoaded(object sender, OnDeviceLoadedEventArgs e)
        {
            InvokeOnMainThread(() =>
            {
                nameTextFieldView.Text = e.DeviceModel.Name;
                enabledSwitchFieldView.On = e.DeviceModel.Enabled;
                buildingTextFieldView.Text = e.DeviceModel.Location1;
                floorTextFieldView.Text = e.DeviceModel.Location2;
                roomTextFieldView.Text = e.DeviceModel.Location3;
                wifiTextFieldView.Text = e.DeviceModel.SSID;
            });        
        }
                                                                                                                                                                                                   
        void HandleOnDeviceUpdated(object sender, bool success)
        {
            UIAlertController alertController = null;
            UIAlertAction action = null;
            if (success)
            {
                alertController = UIAlertController.Create("Setup Complete", "You can make changes to this device from 'Manage Your Devices' on the home screen.", UIAlertControllerStyle.Alert);
                action = UIAlertAction.Create("OK", UIAlertActionStyle.Default, a =>
                {
                    NavigationController.PopToRootViewController(true);
                });
            }
            else
            {
                alertController = UIAlertController.Create(null, "Could not update device. Please try again.", UIAlertControllerStyle.Alert);
                action = UIAlertAction.Create("OK", UIAlertActionStyle.Default, null);
            }

            alertController.AddAction(action);
            PresentViewController(alertController, true, null);
        }
        
        void HandleOnDeviceLoadFail(object sender, EventArgs e)
        {
            UIAlertController failAlertController = null;
            failAlertController = UIAlertController.Create (null, "Could not load the device. Please try again.", UIAlertControllerStyle.Alert);
            failAlertController.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, a =>
            { 
                NavigationController.PopToRootViewController(true);
            }));
       
            PresentViewController (failAlertController, true, null);
        }

        void OnTextFieldEditingEnded(object sender, UITextFieldDidEndEditingReason reason)
        {
            if (!(sender is TextFieldView textFieldView) || reason != UITextFieldDidEndEditingReason.Committed) return;
            if (textFieldView == nameTextFieldView) ViewModel.CurrentDeviceModel.Name = textFieldView.Text;
            if (textFieldView == buildingTextFieldView) ViewModel.CurrentDeviceModel.Location1 = textFieldView.Text;
            if (textFieldView == floorTextFieldView) ViewModel.CurrentDeviceModel.Location2 = textFieldView.Text;
            if (textFieldView == roomTextFieldView) ViewModel.CurrentDeviceModel.Location3 = textFieldView.Text;
        }

        void EnabledSwitchFieldViewOnSwitchValueChanged(object sender, bool enabled)
        {
            ViewModel.CurrentDeviceModel.Enabled = enabled;
        }
    }
}
