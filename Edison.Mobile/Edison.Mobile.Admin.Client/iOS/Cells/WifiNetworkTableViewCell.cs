using System;
using Edison.Mobile.Admin.Client.iOS.Shared;
using Edison.Mobile.Common.WiFi;
using UIKit;

namespace Edison.Mobile.Admin.Client.iOS.Cells
{
    public class WifiNetworkTableViewCell : UITableViewCell
    {
        UILabel label;
        UIImageView arrowImageView;

        public WifiNetworkTableViewCell(IntPtr handle) : base(handle)
        {
        }

        public void Refresh(WifiNetwork wifiNetwork)
        {
            if (label == null)
            {
                label = new UILabel
                {
                    TranslatesAutoresizingMaskIntoConstraints = false,
                    TextColor = Constants.Color.DarkGray,
                    Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.Fourteen),
                };

                ContentView.AddSubview(label);

                label.CenterYAnchor.ConstraintEqualTo(ContentView.CenterYAnchor).Active = true;
                label.LeftAnchor.ConstraintEqualTo(ContentView.LeftAnchor, constant: Constants.Padding).Active = true;

                arrowImageView = new UIImageView
                {
                    TranslatesAutoresizingMaskIntoConstraints = false,
                    Image = Constants.Assets.ArrowRight,
                    ContentMode = UIViewContentMode.ScaleAspectFit,
                };

                ContentView.AddSubview(arrowImageView);

                arrowImageView.RightAnchor.ConstraintEqualTo(ContentView.RightAnchor, constant: -Constants.Padding * 2).Active = true;
                arrowImageView.CenterYAnchor.ConstraintEqualTo(ContentView.CenterYAnchor).Active = true;
                arrowImageView.WidthAnchor.ConstraintEqualTo(12).Active = true;
                arrowImageView.HeightAnchor.ConstraintEqualTo(arrowImageView.WidthAnchor).Active = true;

                label.RightAnchor.ConstraintEqualTo(arrowImageView.LeftAnchor, constant: Constants.Padding).Active = true;
            }

            label.Text = wifiNetwork.SSID;
        }
    }
}
