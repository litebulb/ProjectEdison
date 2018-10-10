using UIKit;

namespace Edison.Mobile.iOS.Common.Shared
{
    public static class PlatformConstants
    {
        public static readonly double AnimationDuration = 0.35;

        public static class Color 
        {
            public static UIColor White = UIColor.White;
            public static UIColor Black = UIColor.Black;
            public static UIColor BackgroundGray = new UIColor(242 / 255f, 242 / 255f, 244 / 255f, 1);
            public static UIColor DarkGray = new UIColor(62 / 255f, 61 / 255f, 74 / 255f, 1);
            public static UIColor MidGray = new UIColor(136 / 255f, 134 / 255f, 160 / 255f, 1);
            public static UIColor LightGray = new UIColor(237 / 255f, 237/ 255f, 240 / 255f, 1);
            public static UIColor Red = new UIColor(255 / 255f, 49 / 255f, 34 / 255f, 1);
            public static UIColor Blue  = new UIColor(34 / 255f, 130 / 255f, 255 / 255f, 1);
            public static UIColor DarkBlue = new UIColor(51 / 255f, 34 / 255f, 255 / 255f, 1);
            public static UIColor Green = new UIColor(40 / 255f, 203 / 255f, 78 / 255f, 1);
        }
    }
}
