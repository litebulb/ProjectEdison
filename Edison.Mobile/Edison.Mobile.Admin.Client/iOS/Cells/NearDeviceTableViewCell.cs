using System;
using CoreGraphics;
using Edison.Core.Common.Models;
using Edison.Mobile.Admin.Client.iOS.Shared;
using Edison.Mobile.Admin.Client.iOS.Views;
using UIKit;

namespace Edison.Mobile.Admin.Client.iOS.Cells
{
    public class NearDeviceTableViewCell : UITableViewCell
    {
        UILabel titleLabel;
        OnlineIndicatorView onlineIndicatorView;
        UIImageView isSensorImageView;
        UIImageView arrowImageView;

        public NearDeviceTableViewCell(IntPtr handle) : base(handle)
        {
        }

        public void Update(DeviceModel deviceModel)
        {
            if (onlineIndicatorView == null)
            {
                onlineIndicatorView = new OnlineIndicatorView { TranslatesAutoresizingMaskIntoConstraints = false, BackgroundColor = Constants.Color.White };
                ContentView.AddSubview(onlineIndicatorView);
                onlineIndicatorView.LeftAnchor.ConstraintEqualTo(ContentView.LeftAnchor).Active = true;
                onlineIndicatorView.TopAnchor.ConstraintEqualTo(ContentView.TopAnchor).Active = true;
                onlineIndicatorView.BottomAnchor.ConstraintEqualTo(ContentView.BottomAnchor).Active = true;
                onlineIndicatorView.WidthAnchor.ConstraintEqualTo(onlineIndicatorView.HeightAnchor).Active = true;

                isSensorImageView = new UIImageView { TranslatesAutoresizingMaskIntoConstraints = false };
                ContentView.AddSubview(isSensorImageView);
                isSensorImageView.LeftAnchor.ConstraintEqualTo(onlineIndicatorView.RightAnchor).Active = true;
                isSensorImageView.CenterYAnchor.ConstraintEqualTo(ContentView.CenterYAnchor).Active = true;
                isSensorImageView.HeightAnchor.ConstraintEqualTo(20).Active = true;
                isSensorImageView.WidthAnchor.ConstraintEqualTo(isSensorImageView.HeightAnchor).Active = true;

                titleLabel = new UILabel
                {
                    TranslatesAutoresizingMaskIntoConstraints = false,
                    TextColor = Constants.Color.DarkGray,
                    Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.Fourteen),
                };
                ContentView.AddSubview(titleLabel);
                titleLabel.CenterYAnchor.ConstraintEqualTo(ContentView.CenterYAnchor).Active = true;
                titleLabel.LeftAnchor.ConstraintEqualTo(isSensorImageView.RightAnchor, constant: Constants.Padding).Active = true;

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

                titleLabel.RightAnchor.ConstraintEqualTo(arrowImageView.LeftAnchor, constant: -Constants.Padding).Active = true;
            }

            onlineIndicatorView.IsOnline = deviceModel.Online;
            isSensorImageView.Image = deviceModel.Sensor ? Constants.Assets.Lines : Constants.Assets.Power;
            titleLabel.Text = deviceModel.Name;
        }
    }
}
