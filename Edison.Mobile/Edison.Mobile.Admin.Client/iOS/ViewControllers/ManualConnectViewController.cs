using System;
using System.Linq;
using CoreGraphics;
using Edison.Mobile.Admin.Client.Core.ViewModels;
using Edison.Mobile.Admin.Client.iOS.Cells;
using Edison.Mobile.Admin.Client.iOS.Shared;
using Edison.Mobile.Admin.Client.iOS.Views;
using Edison.Mobile.Admin.Client.iOS.ViewSources;
using Edison.Mobile.Common.WiFi;
using Edison.Mobile.iOS.Common.Views;
using UIKit;

namespace Edison.Mobile.Admin.Client.iOS.ViewControllers
{
    public class ManualConnectViewController : BaseViewController<ManualConnectViewModel>
    {
        NetworksTableViewSource networksTableViewSource;
        UITableView networksTableView;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = Constants.Color.BackgroundLightGray;

            Title = $"Find Your {ViewModel.DeviceTypeAsString}'s Network";

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

            NavigationItem.LeftBarButtonItem = new UIBarButtonItem(Constants.Assets.CloseX, UIBarButtonItemStyle.Plain, (sender, e) =>
            {
                DismissViewController(true, null);
            });

            var circleNumberView = new CircleNumberView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Number = 3,
            };

            View.AddSubview(circleNumberView);

            circleNumberView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, constant: Constants.Padding).Active = true;
            circleNumberView.WidthAnchor.ConstraintEqualTo(Constants.CircleNumberSize).Active = true;
            circleNumberView.HeightAnchor.ConstraintEqualTo(circleNumberView.WidthAnchor).Active = true;

            var findLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextColor = Constants.Color.MidGray,
                Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.Eighteen),
                AdjustsFontSizeToFitWidth = true,
                MinimumScaleFactor = 0.1f,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0,
                Text = $"The {ViewModel.DeviceTypeAsString} should be emitting a WiFi network of the format \"EDISON_XXXX.\"",
            };

            View.AddSubview(findLabel);

            findLabel.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, constant: Constants.Padding).Active = true;
            findLabel.LeftAnchor.ConstraintEqualTo(circleNumberView.RightAnchor, constant: Constants.Padding).Active = true;
            findLabel.CenterYAnchor.ConstraintEqualTo(circleNumberView.CenterYAnchor).Active = true;
            findLabel.RightAnchor.ConstraintEqualTo(View.RightAnchor, constant: -Constants.Padding).Active = true;

            networksTableViewSource = new NetworksTableViewSource();
            networksTableView = new UITableView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                AlwaysBounceVertical = true,
                BackgroundColor = Constants.Color.White,
                TableHeaderView = new UIView(),
                Source = networksTableViewSource,
                SeparatorStyle = UITableViewCellSeparatorStyle.None,
            };

            var type = typeof(WifiNetworkTableViewCell);
            networksTableView.RegisterClassForCellReuse(type, type.Name);

            View.AddSubview(networksTableView);

            networksTableView.TopAnchor.ConstraintEqualTo(findLabel.BottomAnchor, constant: Constants.Padding).Active = true;
            networksTableView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor).Active = true;
            networksTableView.RightAnchor.ConstraintEqualTo(View.RightAnchor).Active = true;
            networksTableView.BottomAnchor.ConstraintEqualTo(View.BottomAnchor).Active = true;

            var tableViewBackgroundShadowView = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = Constants.Color.White,
            };

            tableViewBackgroundShadowView.Layer.ShadowColor = Constants.Color.DarkGray.CGColor;
            tableViewBackgroundShadowView.Layer.ShadowRadius = 3;
            tableViewBackgroundShadowView.Layer.ShadowOffset = new CGSize(1, 1);
            tableViewBackgroundShadowView.Layer.ShadowOpacity = 0.4f;
            tableViewBackgroundShadowView.Layer.MasksToBounds = false;

            View.InsertSubviewBelow(tableViewBackgroundShadowView, networksTableView);

            tableViewBackgroundShadowView.LeftAnchor.ConstraintEqualTo(networksTableView.LeftAnchor).Active = true;
            tableViewBackgroundShadowView.RightAnchor.ConstraintEqualTo(networksTableView.RightAnchor).Active = true;
            tableViewBackgroundShadowView.TopAnchor.ConstraintEqualTo(networksTableView.TopAnchor).Active = true;
            tableViewBackgroundShadowView.BottomAnchor.ConstraintEqualTo(networksTableView.BottomAnchor).Active = true;
        }

        protected override void BindEventHandlers()
        {
            base.BindEventHandlers();
            ViewModel.AvailableWifiNetworks.CollectionChanged += HandleWifiNetworksCollectionChanged;
            networksTableViewSource.OnWifiNetworkSelected += HandleOnWifiNetworkSelected;
        }

        protected override void UnBindEventHandlers()
        {
            base.UnBindEventHandlers();
            ViewModel.AvailableWifiNetworks.CollectionChanged -= HandleWifiNetworksCollectionChanged;
            networksTableViewSource.OnWifiNetworkSelected -= HandleOnWifiNetworkSelected;
        }

        void HandleWifiNetworksCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            networksTableViewSource.AvailableNetworks = ViewModel.AvailableWifiNetworks.ToArray();
            networksTableView.ReloadData();
        }

        async void HandleOnWifiNetworkSelected(object sender, WifiNetwork wifiNetwork)
        {
            var success = await ViewModel.ConnectToDeviceHotspot(wifiNetwork);
            if (success)
            {
                DismissViewController(true, null);
            }
            else
            {
                var alertController = UIAlertController.Create(null, $"Could not join {wifiNetwork.SSID}", UIAlertControllerStyle.Alert);
                var action = UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null);
                alertController.AddAction(action);
                PresentViewController(alertController, true, null);
            }
        }
    }
}
