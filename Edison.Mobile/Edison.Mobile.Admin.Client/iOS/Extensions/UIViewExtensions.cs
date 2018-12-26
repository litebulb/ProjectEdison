using System;
using CoreGraphics;
using Edison.Mobile.Admin.Client.iOS.Shared;
using UIKit;

namespace Edison.Mobile.Admin.Client.iOS.Extensions
{
    public static class UIViewExtensions
    {
        public static void AddStandardShadow(this UIView view)
        {
            view.Layer.ShadowColor = Constants.Color.DarkGray.CGColor;
            view.Layer.ShadowRadius = 2;
            view.Layer.ShadowOffset = new CGSize(1, 1);
            view.Layer.ShadowOpacity = 0.3f;
            view.Layer.MasksToBounds = false;
        }
    }
}
