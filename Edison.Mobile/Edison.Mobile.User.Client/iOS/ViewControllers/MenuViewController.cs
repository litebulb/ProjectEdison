using CoreGraphics;
using Edison.Mobile.iOS.Common.Shared;
using Edison.Mobile.iOS.Common.Views;
using Edison.Mobile.User.Client.Core.ViewModels;
using Edison.Mobile.User.Client.iOS.DataSources;
using Edison.Mobile.User.Client.iOS.Shared;
using Edison.Mobile.User.Client.iOS.Views;
using UIKit;

namespace Edison.Mobile.User.Client.iOS.ViewControllers
{
    public class MenuViewController : BaseViewController<MenuViewModel>
    {
        readonly float maxShadowOpacity = 0.45f;

        UITableView tableView;
        UIButton logoutButton;
        UIVisualEffectView blurView;
        MenuTableViewSource menuTableViewSource;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.Layer.MasksToBounds = false;
            View.Layer.ShadowColor = Constants.Color.Black.CGColor;
            View.Layer.ShadowOffset = new CGSize(2, 2);
            View.Layer.ShadowRadius = 5;

            View.BackgroundColor = UIColor.Clear;

            blurView = new UIVisualEffectView(UIBlurEffect.FromStyle(UIBlurEffectStyle.ExtraLight))
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };

            View.AddSubview(blurView);
            blurView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor).Active = true;
            blurView.RightAnchor.ConstraintEqualTo(View.RightAnchor).Active = true;
            blurView.TopAnchor.ConstraintEqualTo(View.TopAnchor).Active = true;
            blurView.BottomAnchor.ConstraintEqualTo(View.BottomAnchor).Active = true;

            logoutButton = new UIButton
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };

            logoutButton.SetTitle("LOGOUT", UIControlState.Normal);
            logoutButton.TitleLabel.Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.Fourteen);
            logoutButton.SetTitleColor(Constants.Color.Red, UIControlState.Normal);

            View.AddSubview(logoutButton);
            logoutButton.BottomAnchor.ConstraintEqualTo(View.BottomAnchor).Active = true;
            logoutButton.RightAnchor.ConstraintEqualTo(View.RightAnchor).Active = true;
            logoutButton.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, Constants.MenuRightMargin).Active = true;
            logoutButton.HeightAnchor.ConstraintEqualTo(Constants.MenuCellHeight * 2).Active = true;

            menuTableViewSource = new MenuTableViewSource();
            tableView = new UITableView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                AlwaysBounceVertical = true,
                Source = menuTableViewSource,
                SeparatorStyle = UITableViewCellSeparatorStyle.None,
                TableFooterView = new UIView(),
                BackgroundColor = UIColor.Clear,
                DelaysContentTouches = false,
            };

            tableView.RegisterClassForCellReuse(typeof(MenuProfileTableViewCell), typeof(MenuProfileTableViewCell).Name);
            tableView.RegisterClassForCellReuse(typeof(MenuItemTableViewCell), typeof(MenuItemTableViewCell).Name);
            tableView.RegisterClassForCellReuse(typeof(MenuSeparatorTableViewCell), typeof(MenuSeparatorTableViewCell).Name);

            View.AddSubview(tableView);

            tableView.RightAnchor.ConstraintEqualTo(View.RightAnchor).Active = true;
            tableView.TopAnchor.ConstraintEqualTo(View.TopAnchor).Active = true;
            tableView.BottomAnchor.ConstraintEqualTo(logoutButton.TopAnchor).Active = true;
            tableView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor).Active = true;
        }

        protected override void BindEventHandlers()
        {
            base.BindEventHandlers();

            logoutButton.TouchUpInside += HandleLogoutButtonTouchUpInside;
            menuTableViewSource.ViewModel = ViewModel;
        }

        protected override void UnBindEventHandlers()
        {
            base.UnBindEventHandlers();

            logoutButton.TouchUpInside -= HandleLogoutButtonTouchUpInside;
            menuTableViewSource.ViewModel = null;
        }

        public void SetPercentMaximized(float percent)
        {
            View.Layer.ShadowOpacity = maxShadowOpacity * percent;
        }

        void HandleLogoutButtonTouchUpInside(object sender, System.EventArgs e)
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
    }
}
