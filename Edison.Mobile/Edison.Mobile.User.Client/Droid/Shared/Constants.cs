using Android.Graphics;
using System;
using Edison.Mobile.Android.Common;
using System.Threading.Tasks;
using Android.Support.V7.App;
using Android.Views;
using Android.App;

namespace Edison.Mobile.User.Client.Droid
{
    public static class Constants
    {
        public static string ClientId = "64531b8c-3d22-4c2a-8d72-bf37c8609fbe";

        internal static readonly string CHANNEL_ID = "my_notification_channel";
        internal static readonly int NOTIFICATION_ID = 100;

        public static int StatusBarHeightPx = PixelSizeConverter.DpToPx(24);

        public static int ToolbarHeightPx = -1;

        public static int AvailablePageHeightPx = -1;

        public static int BottomSheetPeekHeightPx { get; private set; } = -1;
        public static int BottomSheetHeightPx { get; private set; } = -1;


        public static Padding PagePaddingPx { get; private set; }

        public static int EventGaugeAreaHeightPx { get; private set; } = -1;
        public static int EventResponseAreaHeightPx { get; private set; } = -1;
        public static int EventGaugeSizePx { get; private set; } = -1;




        public static async Task CalculateUIDimensionsAsync(Activity act)
        {
            await Task.Run(() => {
                CalculateUIDimensions(act);
                return;
            }).ConfigureAwait(false);
        }
        public static void CalculateUIDimensions(Activity act)
        {

            var displayHeightPx = DisplayDetails.DisplayHeightPx;
            var displayWidthPx = DisplayDetails.DisplayWidthPx;

            // Calculate current bar heights
            UpdateBarDimensions(act);

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

            PagePaddingPx = new Padding(0, 0, 0, BottomSheetPeekHeightPx);

            AvailablePageHeightPx = displayHeightPx - StatusBarHeightPx - ToolbarHeightPx - BottomSheetPeekHeightPx;

            // Home Page dimensions
            EventGaugeAreaHeightPx = (int)Math.Round((double)(2 * AvailablePageHeightPx / 5));
            EventResponseAreaHeightPx = AvailablePageHeightPx - EventGaugeAreaHeightPx;
            // Need to use this directly as can be a race condition issue (or Xamarin bug) when adjusting CircularEventGauge size via OnSizeChanged
            EventGaugeSizePx = EventGaugeAreaHeightPx - 2 * act.Resources.GetDimensionPixelSize(Resource.Dimension.event_guage_area_padding);



        }

        public static async Task UpdateBarDimensionsAsync(Activity act)
        {
            await Task.Run(() => {
                UpdateBarDimensions(act);
                return;
            }).ConfigureAwait(false);
        }
        public static void UpdateBarDimensions(Activity act)
        {
            Rect displayFrame = new Rect();
            act.Window.DecorView.GetWindowVisibleDisplayFrame(displayFrame);
            var statusBarHeight = displayFrame.Top;
            if (statusBarHeight > 0)
                StatusBarHeightPx = statusBarHeight;

            int contentViewTop = act.Window.FindViewById(Window.IdAndroidContent).Top;
            int titleBarHeight = contentViewTop - statusBarHeight;
            ToolbarHeightPx = (int)act.Resources.GetDimension(Resource.Dimension.abc_action_bar_default_height_material);
            if (titleBarHeight > ToolbarHeightPx)
                ToolbarHeightPx = titleBarHeight;
        }



    }
}
