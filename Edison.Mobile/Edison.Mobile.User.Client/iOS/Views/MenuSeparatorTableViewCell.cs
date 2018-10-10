using System;
using Edison.Mobile.iOS.Common.Shared;
using UIKit;

namespace Edison.Mobile.User.Client.iOS.Views
{
    public class MenuSeparatorTableViewCell : BaseMenuTableViewCell
    {
        UIView separatorView;

        public MenuSeparatorTableViewCell(IntPtr handle): base(handle) { }

        public void Initialize() 
        {
            if (!isInitialized) 
            {
                separatorView = new UIView
                {
                    TranslatesAutoresizingMaskIntoConstraints = false,
                    BackgroundColor = PlatformConstants.Color.White.ColorWithAlpha(0.4f),
                };

                ContentView.AddSubview(separatorView);

                separatorView.LeftAnchor.ConstraintEqualTo(ContentView.LeftAnchor).Active = true;
                separatorView.RightAnchor.ConstraintEqualTo(ContentView.RightAnchor).Active = true;
                separatorView.HeightAnchor.ConstraintEqualTo(0.5f).Active = true;
                separatorView.CenterYAnchor.ConstraintEqualTo(ContentView.CenterYAnchor).Active = true;

                isInitialized = true;
            }
        }
    }
}
