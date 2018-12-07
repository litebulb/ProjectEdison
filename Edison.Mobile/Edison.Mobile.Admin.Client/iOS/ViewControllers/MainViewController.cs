using Edison.Mobile.Admin.Client.Core.ViewModels;
using Edison.Mobile.Admin.Client.iOS.Shared;
using Edison.Mobile.Admin.Client.iOS.Views;
using Edison.Mobile.iOS.Common.Views;
using UIKit;

namespace Edison.Mobile.Admin.Client.iOS.ViewControllers
{
    public class MainViewController : BaseViewController<MainViewModel>
    {
        SetupButtonView newDeviceView;
        SetupButtonView manageDeviceView;
        UILabel nearYouLabel;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = Constants.Color.BackgroundLightGray;

            Title = "Edison Device Setup";

            NavigationController.NavigationBar.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);
            NavigationController.NavigationBar.ShadowImage = new UIImage();
            NavigationController.NavigationBar.Translucent = true;
            NavigationController.NavigationBar.BackgroundColor = UIColor.Clear;

            NavigationItem.LeftBarButtonItem = new UIBarButtonItem(Constants.Assets.Menu, UIBarButtonItemStyle.Plain, null);
            NavigationController.NavigationBar.TintColor = Constants.Color.DarkBlue;

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
        }
    }
}
