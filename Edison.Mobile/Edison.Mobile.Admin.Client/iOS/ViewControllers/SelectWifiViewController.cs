using CoreGraphics;
using Edison.Mobile.Admin.Client.Core.ViewModels;
using Edison.Mobile.Admin.Client.iOS.Cells;
using Edison.Mobile.Admin.Client.iOS.Shared;
using Edison.Mobile.Admin.Client.iOS.Views;
using Edison.Mobile.Admin.Client.iOS.ViewSources;
using Edison.Mobile.iOS.Common.Views;
using UIKit;

namespace Edison.Mobile.Admin.Client.iOS.ViewControllers
{
    public class SelectWifiViewController : BaseViewController<SelectWifiViewModel>
    {
        UITableView tableView;
        NetworksTableViewSource tableViewSource;

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
                Number = 4,
            };

            View.AddSubview(circleNumberView);
            circleNumberView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, constant: padding).Active = true;
            circleNumberView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, constant: padding).Active = true;
            circleNumberView.WidthAnchor.ConstraintEqualTo(Constants.CircleNumberSize).Active = true;
            circleNumberView.HeightAnchor.ConstraintEqualTo(circleNumberView.WidthAnchor).Active = true;

            var deviceTypeLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextColor = Constants.Color.MidGray,
                Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.Eighteen),
                AdjustsFontSizeToFitWidth = true,
                MinimumScaleFactor = 0.1f,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0,
                Text = $"Select a Wifi network the {ViewModel.DeviceTypeAsString} should use.",
            };

            View.AddSubview(deviceTypeLabel);
            deviceTypeLabel.LeftAnchor.ConstraintEqualTo(circleNumberView.RightAnchor, constant: padding).Active = true;
            deviceTypeLabel.CenterYAnchor.ConstraintEqualTo(circleNumberView.CenterYAnchor).Active = true;
            deviceTypeLabel.RightAnchor.ConstraintEqualTo(View.RightAnchor, constant: -padding).Active = true;

            var availableNetworksLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextColor = Constants.Color.MidGray,
                Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.Twelve),
                AdjustsFontSizeToFitWidth = true,
                MinimumScaleFactor = 0.1f,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0,
                Text = "Available Networks",
            };

            View.AddSubview(availableNetworksLabel);
            availableNetworksLabel.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, constant: padding).Active = true;
            availableNetworksLabel.TopAnchor.ConstraintEqualTo(circleNumberView.BottomAnchor, constant: padding * 2).Active = true;

            tableViewSource = new NetworksTableViewSource();

            tableView = new UITableView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                AlwaysBounceVertical = true,
                TableFooterView = new UIView(),
                Source = tableViewSource,
                SeparatorStyle = UITableViewCellSeparatorStyle.None,
            };

            var cellType = typeof(WifiNetworkTableViewCell);
            tableView.RegisterClassForCellReuse(cellType, cellType.Name);

            View.AddSubview(tableView);

            tableView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor).Active = true;
            tableView.RightAnchor.ConstraintEqualTo(View.RightAnchor).Active = true;
            tableView.TopAnchor.ConstraintEqualTo(availableNetworksLabel.BottomAnchor, constant: padding).Active = true;
            tableView.BottomAnchor.ConstraintEqualTo(View.BottomAnchor).Active = true;

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

            View.InsertSubviewBelow(tableViewBackgroundShadowView, tableView);

            tableViewBackgroundShadowView.LeftAnchor.ConstraintEqualTo(tableView.LeftAnchor).Active = true;
            tableViewBackgroundShadowView.RightAnchor.ConstraintEqualTo(tableView.RightAnchor).Active = true;
            tableViewBackgroundShadowView.TopAnchor.ConstraintEqualTo(tableView.TopAnchor).Active = true;
            tableViewBackgroundShadowView.BottomAnchor.ConstraintEqualTo(tableView.BottomAnchor).Active = true;
        }

        protected override void BindEventHandlers()
        {
            base.BindEventHandlers();
            ViewModel.OnAvailableWifiNetworksChanged += HandleOnAvailableWifiNetworksChanged;
            tableViewSource.OnWifiNetworkSelected += HandleOnWifiNetworkSelected;
        }

        protected override void UnBindEventHandlers()
        {
            base.UnBindEventHandlers();
            ViewModel.OnAvailableWifiNetworksChanged -= HandleOnAvailableWifiNetworksChanged;
            tableViewSource.OnWifiNetworkSelected -= HandleOnWifiNetworkSelected;
        }

        void HandleOnWifiNetworkSelected(object sender, Common.WiFi.WifiNetwork wifiNetwork)
        {
            NavigationController.PushViewController(new EnterWifiPasswordViewController(wifiNetwork), true);
        }

        void HandleOnAvailableWifiNetworksChanged()
        {
            var availableNetworks = ViewModel.AvailableWifiNetworks;
            tableViewSource.AvailableNetworks = availableNetworks.ToArray();
            InvokeOnMainThread(tableView.ReloadData);
        }
    }
}
