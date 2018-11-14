using System;
using Edison.Core.Common.Models;
using Edison.Mobile.User.Client.iOS.Shared;
using UIKit;
namespace Edison.Mobile.User.Client.iOS.Views
{
    public class ResponseUpdateTableViewCell : UITableViewCell
    {
        public static float DotSize { get; } = 12;
        public static float CellPadding { get; } = 8;
        public static float LineWidth { get; } = 1;

        bool isInitialized;

        UILabel headerLabel;
        UILabel contentLabel;
        UIView dotView;
        UIView aboveDotLineView;
        UIView belowDotLineView;

        public ResponseUpdateTableViewCell(IntPtr handle) : base(handle) { }

        public void Initialize(NotificationModel notificationModel, bool showTopLine = true, bool makeDotRed = false)
        {
            if (!isInitialized)
            {
                BackgroundColor = Constants.Color.BackgroundDarkGray;

                headerLabel = new UILabel
                {
                    TranslatesAutoresizingMaskIntoConstraints = false,
                    Lines = 1,
                    Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.Twelve),
                    TextColor = Constants.Color.LightGray,
                    BackgroundColor = UIColor.Clear,
                };

                ContentView.AddSubview(headerLabel);
                headerLabel.TopAnchor.ConstraintEqualTo(ContentView.TopAnchor, CellPadding * 2).Active = true;
                headerLabel.RightAnchor.ConstraintEqualTo(ContentView.RightAnchor, -CellPadding).Active = true;

                dotView = new UIView
                {
                    TranslatesAutoresizingMaskIntoConstraints = false,
                    BackgroundColor = makeDotRed ? Constants.Color.Red : Constants.Color.LightGray,
                };

                ContentView.AddSubview(dotView);
                dotView.LeftAnchor.ConstraintEqualTo(ContentView.LeftAnchor, CellPadding).Active = true;
                dotView.TopAnchor.ConstraintEqualTo(ContentView.TopAnchor, CellPadding * 2).Active = true;
                dotView.WidthAnchor.ConstraintEqualTo(DotSize).Active = true;
                dotView.HeightAnchor.ConstraintEqualTo(DotSize).Active = true;
                dotView.Layer.CornerRadius = DotSize / 2;

                headerLabel.LeftAnchor.ConstraintEqualTo(dotView.RightAnchor, CellPadding).Active = true;
                headerLabel.CenterYAnchor.ConstraintEqualTo(dotView.CenterYAnchor).Active = true;

                aboveDotLineView = new UIView
                {
                    TranslatesAutoresizingMaskIntoConstraints = false,
                    BackgroundColor = Constants.Color.DarkGray,
                    Alpha = showTopLine ? 1 : 0,
                };

                ContentView.InsertSubviewBelow(aboveDotLineView, dotView);
                aboveDotLineView.TopAnchor.ConstraintEqualTo(ContentView.TopAnchor).Active = true;
                aboveDotLineView.BottomAnchor.ConstraintEqualTo(dotView.CenterYAnchor).Active = true;
                aboveDotLineView.CenterXAnchor.ConstraintEqualTo(dotView.CenterXAnchor).Active = true;
                aboveDotLineView.WidthAnchor.ConstraintEqualTo(LineWidth).Active = true;

                belowDotLineView = new UIView
                {
                    TranslatesAutoresizingMaskIntoConstraints = false,
                    BackgroundColor = Constants.Color.DarkGray,
                };

                ContentView.InsertSubviewBelow(belowDotLineView, dotView);
                belowDotLineView.TopAnchor.ConstraintEqualTo(dotView.CenterYAnchor).Active = true;
                belowDotLineView.BottomAnchor.ConstraintEqualTo(ContentView.BottomAnchor).Active = true;
                belowDotLineView.CenterXAnchor.ConstraintEqualTo(dotView.CenterXAnchor).Active = true;
                belowDotLineView.WidthAnchor.ConstraintEqualTo(LineWidth).Active = true;

                contentLabel = new UILabel
                {
                    TranslatesAutoresizingMaskIntoConstraints = false,
                    Lines = 0,
                    LineBreakMode = UILineBreakMode.WordWrap,
                    Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.Twelve),
                    TextColor = Constants.Color.White,
                    BackgroundColor = UIColor.Clear,
                };

                ContentView.AddSubview(contentLabel);

                contentLabel.TopAnchor.ConstraintEqualTo(headerLabel.BottomAnchor, CellPadding).Active = true;
                contentLabel.BottomAnchor.ConstraintEqualTo(ContentView.BottomAnchor).Active = true;
                contentLabel.LeftAnchor.ConstraintEqualTo(headerLabel.LeftAnchor).Active = true;
                contentLabel.RightAnchor.ConstraintEqualTo(ContentView.RightAnchor, -CellPadding).Active = true;

                SelectionStyle = UITableViewCellSelectionStyle.None;

                isInitialized = true;
            }

            headerLabel.Text = $"MESSAGE - {notificationModel.CreationDate}";
            contentLabel.Text = notificationModel.NotificationText;
        }
    }
}
