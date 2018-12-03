using System;
using UIKit;

namespace Edison.Mobile.Admin.Client.iOS.Shared
{
    public static class Constants
    {
        public static readonly string ClientId = "19cb746c-3066-4cd8-8cd2-e0ce1176ae33";

        public static nfloat Padding = 8;

        public static class Color
        {
            public static UIColor White = UIColor.White;
            public static UIColor MidGray = new UIColor(136 / 255f, 134 / 255f, 160 / 255f, 1);
            public static UIColor BackgroundLightGray = new UIColor(249 / 255f, 249 / 255f, 250 / 255f, 1);
            public static UIColor DarkGray = new UIColor(62 / 255f, 61 / 255f, 74 / 255f, 1);
            public static UIColor DarkBlue = new UIColor(51 / 255f, 34 / 255f, 255 / 255f, 1);
        }

        public static class Fonts
        {
            public static UIFont RubikOfSize(float size) => UIFont.FromName("Rubik-Regular", size);
            public static UIFont RubikMediumOfSize(float size) => UIFont.FromName("Rubik-Light", size);

            public static class Size
            {
                public static float Eight = 8;
                public static float Ten = 10;
                public static float Twelve = 12;
                public static float Fourteen = 14;
                public static float Sixteen = 16;
                public static float Eighteen = 18;
                public static float TwentyFour = 24;
                public static float SeventyTwo = 72;
            }
        }

        public static class Assets
        {
            public static UIImage Sensors => UIImage.FromBundle("Sensors");
            public static UIImage LoginLogo => UIImage.FromBundle("LoginLogo");
        }
    }
}
