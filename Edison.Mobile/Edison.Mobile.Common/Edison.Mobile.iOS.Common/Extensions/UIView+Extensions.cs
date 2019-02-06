using System;
using UIKit;

namespace Edison.Mobile.iOS.Common.Extensions
{
    public static class UIView_Extensions
    {
        public static UIResponder FindFirstResponder(this UIView view)
        {
            if (view.IsFirstResponder) return view;

            foreach (var subview in view.Subviews)
            {
                var responder = subview.FindFirstResponder();
                if (responder != null) return responder;
            }

            return null;
        }
    }
}
