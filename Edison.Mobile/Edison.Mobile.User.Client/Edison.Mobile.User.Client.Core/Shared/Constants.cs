using System;
namespace Edison.Mobile.User.Client.Core.Shared
{
    public static class Constants
    {
        public static int UpdateLocationTimerInterval { get; } = 10000;

        public static class ColorName
        {
            public const string Red = "red";
            public const string Yellow = "yellow";
            public const string Blue = "blue";
        }

        public static class IconName
        {
            public const string Fire = "fire";
            public const string Gun = "gun";
            public const string Protest = "protest";
            public const string Pollution = "pollution";
            public const string Health = "health";
            public const string Package = "package";
            public const string Tornado = "tornado";
            public const string Vip = "vip";
            public const string Emergency = "emergency";
        }
    }
}
