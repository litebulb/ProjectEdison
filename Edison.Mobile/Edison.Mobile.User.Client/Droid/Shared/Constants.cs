using Android.Graphics;
using System;
using Edison.Mobile.Android.Common;
using System.Threading.Tasks;
using Android.Support.V7.App;

namespace Edison.Mobile.User.Client.Droid
{
    public static class Constants
    {
        public static string ClientId = "64531b8c-3d22-4c2a-8d72-bf37c8609fbe";

        static readonly string TAG = "MainActivity";

        internal static readonly string CHANNEL_ID = "my_notification_channel";
        internal static readonly int NOTIFICATION_ID = 100;


        public static int BottomSheetPeekHeightPx { get; private set; } = -1;
        public static int BottomSheetHeightPx { get; private set; } = -1;


        public static async Task CalculateUIDimensionsAsync(AppCompatActivity act)
        {
            await Task.Run(() => {
                CalculateUIDimensions(act);
                return;
            }).ConfigureAwait(false);
        }
        public static void CalculateUIDimensions(AppCompatActivity act)
        {

            var displayHeightPx = DisplayDetails.DisplayHeightPx;
            var displayWidthPx = DisplayDetails.DisplayWidthPx;

            // BottomSheet PeekHeight
            int quickChatIconHorizontalMarginPx = (int)act.Resources.GetDimension(Resource.Dimension.bottom_sheet_button_icon_padding);
            int quickChatIconVerticalMarginsPx = quickChatIconHorizontalMarginPx + (int)act.Resources.GetDimension(Resource.Dimension.bottom_sheet_button_icon_toppadding);
            int labelHeightPx = act.Resources.GetDimensionPixelSize(Resource.Dimension.bottom_sheet_button_text_size);
            int bottomSheetThumbHeightPx = 2 * (int)act.Resources.GetDimension(Resource.Dimension.bottom_sheet_thumb_padding) + (int)act.Resources.GetDimension(Resource.Dimension.bottom_sheet_thumb_height);
            int quickChatIconHorizontalMarginsPx = 2 * quickChatIconHorizontalMarginPx;
            int quickChatIconDiameterPx = (int)(displayWidthPx / 3 - quickChatIconHorizontalMarginsPx);
            BottomSheetPeekHeightPx = quickChatIconDiameterPx + labelHeightPx + bottomSheetThumbHeightPx + quickChatIconVerticalMarginsPx;
            // BottomSheet Height
            BottomSheetHeightPx = displayHeightPx - (int)act.Resources.GetDimension(Resource.Dimension.abc_action_bar_default_height_material);

        }





    }
}
