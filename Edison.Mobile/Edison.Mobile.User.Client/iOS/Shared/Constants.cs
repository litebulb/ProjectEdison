using System;
using UIKit;
using Edison.Mobile.User.Client.Core.Shared;

namespace Edison.Mobile.User.Client.iOS.Shared
{
    public static class Constants
    {
        //public static readonly string ClientId = "fc3bf201-80f6-404d-9a70-8c19b8774a8b"; // edison ios user 
        public static readonly string ClientId = "19cb746c-3066-4cd8-8cd2-e0ce1176ae33";

        public static readonly string ListenConnectionString = "Endpoint=sb://edisondev.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=CNCM1xn79hHuUUj6GiAct1JJe5kdzGuPmzBOaVoSGsA=";
        public static readonly string NotificationHubName = "edisondevnotificationhub";

        public static nfloat PulloutTopMargin { get; set; }
        public static nfloat PulloutBottomMargin { get; set; }
        public static nfloat PulloutMinBottomMargin { get; set; }

        public static readonly nfloat PulloutVelocityThreshold = 200;
        public static readonly nfloat PulloutTopBarHeight = 30;
        public static readonly nfloat CornerRadius = 10;

        public static nfloat MenuRightMargin { get; set; } = 100;

        public static nfloat ChatMessageTypeHeight { get; set; } = 78;

        public static readonly nfloat Padding = 16;

        public static nfloat MenuCellHeight = 54f;

        public static class Color
        {
            public static UIColor White = UIColor.White;
            public static UIColor Black = UIColor.Black;
            public static UIColor BackgroundGray = new UIColor(242 / 255f, 242 / 255f, 244 / 255f, 1);
            public static UIColor BackgroundDarkGray = new UIColor(34/ 255f, 34 / 255f, 39 / 255f, 1);
            public static UIColor DarkGray = new UIColor(62 / 255f, 61 / 255f, 74 / 255f, 1);
            public static UIColor MidGray = new UIColor(136 / 255f, 134 / 255f, 160 / 255f, 1);
            public static UIColor LightGray = new UIColor(237 / 255f, 237 / 255f, 240 / 255f, 1);
            public static UIColor Red = new UIColor(255 / 255f, 49 / 255f, 34 / 255f, 1);
            public static UIColor Blue = new UIColor(34 / 255f, 130 / 255f, 255 / 255f, 1);
            public static UIColor DarkBlue = new UIColor(51 / 255f, 34 / 255f, 255 / 255f, 1);
            public static UIColor Green = new UIColor(40 / 255f, 203 / 255f, 78 / 255f, 1);
            public static UIColor YellowWarning = new UIColor(255 / 255f, 159 / 255f, 34 / 255f, 1);

            public static UIColor MapFromActionPlanColor(string colorName) 
            {
                switch (colorName) 
                {
                    case Core.Shared.Constants.ColorName.Red: return Red;
                    case Core.Shared.Constants.ColorName.Yellow: return YellowWarning;
                    case Core.Shared.Constants.ColorName.Blue: return Blue;
                    default: return null;
                }
            }
        }

        public static class Assets 
        {
            public static UIImage Menu => UIImage.FromBundle("Menu");
            public static UIImage Brightness => UIImage.FromBundle("Brightness");
            public static UIImage EmergencyWhite => UIImage.FromBundle("EmergencyWhite");
            public static UIImage EmergencyRed => UIImage.FromBundle("EmergencyRed");
            public static UIImage ChatWhite => UIImage.FromBundle("ChatWhite");
            public static UIImage ChatBlue => UIImage.FromBundle("ChatBlue");
            public static UIImage PersonBlue => UIImage.FromBundle("PersonBlue");
            public static UIImage PersonWhite => UIImage.FromBundle("PersonWhite");
            public static UIImage Logo => UIImage.FromBundle("Logo");
            public static UIImage Location => UIImage.FromBundle("Location");
            public static UIImage CloseX => UIImage.FromBundle("CloseX");
            public static UIImage FireWhite => UIImage.FromBundle("FireWhite");
            public static UIImage FireRed => UIImage.FromBundle("FireRed");
            public static UIImage NotificationBell => UIImage.FromBundle("NotificationBell");
            public static UIImage GunWhite => UIImage.FromBundle("GunWhite");
            public static UIImage GunRed => UIImage.FromBundle("GunRed");
            public static UIImage SpamWhite => UIImage.FromBundle("SpamWhite");
            public static UIImage SpamYellow => UIImage.FromBundle("SpamYellow");
            public static UIImage SafetyCheckBlue => UIImage.FromBundle("SafetyCheckBlue");
            public static UIImage SafetyCheckWhite => UIImage.FromBundle("SafetyCheckWhite");
            public static UIImage HealthWhite => UIImage.FromBundle("HealthWhite");
            public static UIImage HealthBlue => UIImage.FromBundle("HealthBlue");
            public static UIImage LocationSent => UIImage.FromBundle("LocationSent");
            public static UIImage ProtestWhite => UIImage.FromBundle("ProtestWhite");
            public static UIImage ProtestBlue => UIImage.FromBundle("ProtestBlue");
            public static UIImage BrightnessMoon => UIImage.FromBundle("BrightnessMoon");
            public static UIImage TornadoWhite => UIImage.FromBundle("TornadoWhite");
            public static UIImage TornadoYellow => UIImage.FromBundle("TornadoYellow");
            public static UIImage PackageWhite => UIImage.FromBundle("PackageWhite");
            public static UIImage PackageRed => UIImage.FromBundle("PackageRed");
            public static UIImage VipBlue => UIImage.FromBundle("VIPBlue");
            public static UIImage VipWhite => UIImage.FromBundle("VIPWhite");

            public static UIImage MapFromActionPlanIcon(string str, bool colored = false) 
            {
                switch (str) 
                {
                    case Core.Shared.Constants.IconName.Fire: return colored ? FireRed : FireWhite;
                    case Core.Shared.Constants.IconName.Gun: return colored ? GunRed : GunWhite;
                    case Core.Shared.Constants.IconName.Protest: return colored ? ProtestBlue : ProtestWhite;
                    case Core.Shared.Constants.IconName.Pollution: return colored ? HealthBlue : HealthWhite;
                    case Core.Shared.Constants.IconName.Health: return colored ? HealthBlue : HealthWhite;
                    case Core.Shared.Constants.IconName.Tornado: return colored ? TornadoYellow : TornadoWhite;
                    case Core.Shared.Constants.IconName.Package: return colored ? PackageRed : PackageWhite;
                    case Core.Shared.Constants.IconName.Vip: return colored ? VipBlue : VipWhite;
                    case Core.Shared.Constants.IconName.Emergency: return colored ? EmergencyRed : EmergencyWhite;
                    default: return null;
                }
            }
        }

        public static class Fonts 
        {
            public static UIFont RubikOfSize(float size) => UIFont.FromName("Rubik-Regular", size);
            public static UIFont RubikMediumOfSize(float size) => UIFont.FromName("Rubik-Medium", size);

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
    }
}
