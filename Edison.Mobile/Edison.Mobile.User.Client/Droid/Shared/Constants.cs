using Android.Graphics;
using System;
namespace Edison.Mobile.User.Client.Droid.Shared
{
    public static class Constants
    {
        public static string ClientId = "64531b8c-3d22-4c2a-8d72-bf37c8609fbe";

        static readonly string TAG = "MainActivity";

        internal static readonly string CHANNEL_ID = "my_notification_channel";
        internal static readonly int NOTIFICATION_ID = 100;


        public static readonly float PulloutTopMargin = 80;
        public static readonly float PulloutBottomMargin = 180;
        public static readonly float PulloutVelocityThreshold = 200;
        public static readonly float PulloutTopBarHeight = 30;
        public static readonly float CornerRadius = 20;

        public static readonly Color BackgroundColor = Color.Argb(0, 255, 255, 255);
    }
}
