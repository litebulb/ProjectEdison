using System;
using System.Collections.Specialized;
using CoreGraphics;
using Edison.Core.Common.Models;
using Edison.Mobile.Admin.Client.Core.ViewModels;
using Edison.Mobile.Admin.Client.iOS.Cells;
using Edison.Mobile.Admin.Client.iOS.Extensions;
using Edison.Mobile.Admin.Client.iOS.Shared;
using Edison.Mobile.Admin.Client.iOS.Views;
using Edison.Mobile.Admin.Client.iOS.ViewSources;
using Edison.Mobile.iOS.Common.Views;
using UIKit;

namespace Edison.Mobile.Admin.Client.iOS.ViewControllers
{
    public class MainViewController : BaseViewController<MainViewModel>
    {
        SetupButtonView newDeviceView;
        SetupButtonView manageDeviceView;
        UILabel nearYouLabel;
        UITableView devicesTableView;
        NearDevicesTableViewSource nearDevicesTableViewSource;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = Constants.Color.BackgroundLightGray;

            Title = "Edison Device Setup";

            var sidePadding = 32f;
            var buttonHeight = 100f;
            var verticalPadding = View.Bounds.Height * 0.05f;

            newDeviceView = new SetupButtonView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Text = "Set Up New Device",
                SmallIconImage = Constants.Assets.Plus,
                LargeIconImage = Constants.Assets.SensorsGray,
                BackgroundColor = Constants.Color.White,
            };

            manageDeviceView = new SetupButtonView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Text = "Manage Your Devices",
                SmallIconImage = Constants.Assets.Gear,
                LargeIconImage = Constants.Assets.SensorsBlue,
                BackgroundColor = Constants.Color.White,
            };

            View.AddSubview(newDeviceView);
            newDeviceView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, constant: verticalPadding).Active = true;
            newDeviceView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, constant: sidePadding).Active = true;
            newDeviceView.RightAnchor.ConstraintEqualTo(View.RightAnchor, constant: -sidePadding).Active = true;
            newDeviceView.HeightAnchor.ConstraintEqualTo(buttonHeight).Active = true;

            View.AddSubview(manageDeviceView);
            manageDeviceView.TopAnchor.ConstraintEqualTo(newDeviceView.BottomAnchor, constant: sidePadding / 2).Active = true;
            manageDeviceView.LeftAnchor.ConstraintEqualTo(newDeviceView.LeftAnchor).Active = true;
            manageDeviceView.RightAnchor.ConstraintEqualTo(newDeviceView.RightAnchor).Active = true;
            manageDeviceView.HeightAnchor.ConstraintEqualTo(newDeviceView.HeightAnchor).Active = true;

            nearYouLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextColor = Constants.Color.DarkGray,
                Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.Fourteen),
                Text = "Devices Near You",
            };
            View.AddSubview(nearYouLabel);
            nearYouLabel.TopAnchor.ConstraintEqualTo(manageDeviceView.BottomAnchor, constant: verticalPadding).Active = true;
            nearYouLabel.LeftAnchor.ConstraintEqualTo(manageDeviceView.LeftAnchor).Active = true;

            nearDevicesTableViewSource = new NearDevicesTableViewSource(ViewModel);

            devicesTableView = new UITableView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                AlwaysBounceVertical = true,
                BackgroundColor = Constants.Color.White ,
                TableHeaderView = new UIView(),
                TableFooterView = new UIView(),
                Source = nearDevicesTableViewSource,
                SeparatorStyle = UITableViewCellSeparatorStyle.None,
            };

            var type = typeof(NearDeviceTableViewCell);
            devicesTableView.RegisterClassForCellReuse(type, type.Name);

            View.AddSubview(devicesTableView);
            devicesTableView.TopAnchor.ConstraintEqualTo(nearYouLabel.BottomAnchor, constant: Constants.Padding).Active = true;
            devicesTableView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor).Active = true;
            devicesTableView.RightAnchor.ConstraintEqualTo(View.RightAnchor).Active = true;
            devicesTableView.BottomAnchor.ConstraintEqualTo(View.BottomAnchor).Active = true;

            var tableViewBackgroundShadowView = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = Constants.Color.White,
            };

            tableViewBackgroundShadowView.AddStandardShadow();

            View.InsertSubviewBelow(tableViewBackgroundShadowView, devicesTableView);

            tableViewBackgroundShadowView.LeftAnchor.ConstraintEqualTo(devicesTableView.LeftAnchor).Active = true;
            tableViewBackgroundShadowView.RightAnchor.ConstraintEqualTo(devicesTableView.RightAnchor).Active = true;
            tableViewBackgroundShadowView.TopAnchor.ConstraintEqualTo(devicesTableView.TopAnchor).Active = true;
            tableViewBackgroundShadowView.BottomAnchor.ConstraintEqualTo(devicesTableView.BottomAnchor).Active = true;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            NavigationController.NavigationBar.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);
            NavigationController.NavigationBar.ShadowImage = new UIImage();
            NavigationController.NavigationBar.Translucent = true;
            NavigationController.NavigationBar.BackgroundColor = UIColor.Clear;

            NavigationItem.LeftBarButtonItem = new UIBarButtonItem("Logout", UIBarButtonItemStyle.Plain, HandleLogoutTapped);
            NavigationController.NavigationBar.TintColor = Constants.Color.DarkBlue;
            NavigationController.NavigationBar.TitleTextAttributes = new UIStringAttributes
            {
                ForegroundColor = Constants.Color.DarkBlue,
                Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.Eighteen),
            };
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            UIView.Animate(0.35, () =>
            {
                foreach (var cell in devicesTableView.VisibleCells)
                {
                    cell.Selected = false;
                }
            });
        }

        protected override void BindEventHandlers()
        {
            base.BindEventHandlers();
            ViewModel.NearDevices.CollectionChanged += HandleNearDevicesCollectionChanged;
            newDeviceView.OnTap += HandleNewDeviceViewOnTap;
            manageDeviceView.OnTap += HandleManageDeviceViewOnTap;
            nearDevicesTableViewSource.OnDeviceSelected += HandleNearDevicesTableViewSourceOnDeviceSelected;
        }

        protected override void UnBindEventHandlers()
        {
            base.UnBindEventHandlers();
            ViewModel.NearDevices.CollectionChanged -= HandleNearDevicesCollectionChanged;
            newDeviceView.OnTap -= HandleNewDeviceViewOnTap;
            manageDeviceView.OnTap -= HandleManageDeviceViewOnTap;
            nearDevicesTableViewSource.OnDeviceSelected -= HandleNearDevicesTableViewSourceOnDeviceSelected;
        }
        
        void HandleLogoutTapped(object sender, EventArgs e)
        {
            var alertController = UIAlertController.Create(null, "Are you sure you'd like to sign out?", UIAlertControllerStyle.Alert);
            var yesAction = UIAlertAction.Create("Yes", UIAlertActionStyle.Destructive, async action =>
            {
                await ViewModel.SignOut();

                UIApplication.SharedApplication.KeyWindow.RootViewController = new LoginViewController();
            });
            var cancelAction = UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null);
            alertController.AddAction(yesAction);
            alertController.AddAction(cancelAction);

            PresentViewController(alertController, true, null);
        }

        void HandleNearDevicesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            devicesTableView.ReloadData();
        }

        void HandleNewDeviceViewOnTap(object sender, EventArgs e)
        {
            var chooseDeviceTypeViewController = new ChooseDeviceTypeViewController();
            NavigationController.PushViewController(chooseDeviceTypeViewController, true);
        }

        void HandleManageDeviceViewOnTap(object sender, EventArgs e)
        {

        }

        void HandleNearDevicesTableViewSourceOnDeviceSelected(object sender, DeviceModel deviceModel)
        {
            var manageDeviceViewController = new ManageDeviceViewController(deviceModel);
            ViewModel.CurrentDeviceModel.Name = deviceModel.Name;
            NavigationController.PushViewController(manageDeviceViewController, true);
        }
    }
}
