using System;
using System.Threading.Tasks;

using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.App;
using Android.Support.V4.Content.Res;
using Android.Support.V7.Widget;
using Android.Support.V7.View.Menu;

using Edison.Mobile.Android.Common;


namespace Edison.Mobile.User.Client.Droid
{
    public static class Constants
    {
        public static string ClientId = "19cb746c-3066-4cd8-8cd2-e0ce1176ae33"; //"64531b8c-3d22-4c2a-8d72-bf37c8609fbe";

        internal static readonly string CHANNEL_ID = "my_notification_channel";
        internal static readonly int NOTIFICATION_ID = 100;

        public static int StatusBarHeightPx = PixelSizeConverter.DpToPx(24);

        public static int ToolbarHeightPx = -1;

        public static int AvailablePageHeightPx = -1;

        public static int BottomSheetPeekHeightPx { get; private set; } = -1;
        public static int BottomSheetHeightPx { get; private set; } = -1;


        public static Padding PagePaddingPx { get; private set; }

        public static int EventGaugeAreaHeightPx { get; private set; } = -1;
        public static int EventGaugeSizePx { get; private set; } = -1;

        public static int EventResponseAreaHeightPx { get; private set; } = -1;
        public static int EventResponseCardWidthPx { get; private set; } = -1;
        public static int EventResponseCardSeperatorWidthPx { get; private set; } = -1;


        public static int BrightnessContainerWidth { get; private set; } = -1;
        public static int BrightnessToolbarItemIconBottomPadding { get; private set; } = -1;



        public readonly static Color DefaultResponseColor = Color.Argb(255, 34, 240, 255);


        public static async Task CalculateUIDimensionsAsync(Activity act)
        {
            await Task.Run(() =>
            {
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
            int quickChatIconHorizontalMarginPx =
                (int)act.Resources.GetDimension(Resource.Dimension.bottom_sheet_button_icon_padding);
            int quickChatIconVerticalMarginsPx = quickChatIconHorizontalMarginPx +
                                                 (int)act.Resources.GetDimension(Resource.Dimension
                                                     .bottom_sheet_button_icon_toppadding);
            int labelHeightPx = act.Resources.GetDimensionPixelSize(Resource.Dimension.bottom_sheet_button_text_size);
            int bottomSheetThumbHeightPx =
                2 * (int)act.Resources.GetDimension(Resource.Dimension.bottom_sheet_thumb_padding) +
                (int)act.Resources.GetDimension(Resource.Dimension.bottom_sheet_thumb_height);
            int quickChatIconHorizontalMarginsPx = 2 * quickChatIconHorizontalMarginPx;
            int quickChatIconDiameterPx = (int)(displayWidthPx / 3 - quickChatIconHorizontalMarginsPx);
            BottomSheetPeekHeightPx = quickChatIconDiameterPx + labelHeightPx + bottomSheetThumbHeightPx +
                                      quickChatIconVerticalMarginsPx;
            // BottomSheet Height
            BottomSheetHeightPx = displayHeightPx - (int)act.Resources.GetDimensionPixelSize(Resource.Dimension.abc_action_bar_default_height_material);

            PagePaddingPx = new Padding(0, 0, 0, BottomSheetPeekHeightPx);

            AvailablePageHeightPx = displayHeightPx - StatusBarHeightPx - ToolbarHeightPx - BottomSheetPeekHeightPx;

            // Home Page dimensions
            EventGaugeAreaHeightPx = (int)Math.Round((double)(2 * AvailablePageHeightPx / 5));
            EventResponseAreaHeightPx = AvailablePageHeightPx - EventGaugeAreaHeightPx;
            // Need to use this directly as can be a race condition issue (or Xamarin bug) when adjusting CircularEventGauge size via OnSizeChanged
            EventGaugeSizePx = EventGaugeAreaHeightPx - 2 * act.Resources.GetDimensionPixelSize(Resource.Dimension.event_guage_area_padding);

            EventResponseCardWidthPx = (int)(displayWidthPx * 0.65);
            EventResponseCardSeperatorWidthPx = (int)((displayWidthPx - EventResponseCardWidthPx) / 2);


        }

        public static async Task UpdateBarDimensionsAsync(Activity act)
        {
            await Task.Run(() =>
            {
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


        public static void UpdateBrightnessControlDimensions(Toolbar toolbar, ActionMenuItemView menuItem)
        {
            // Get the position of the menu item
            int[] itemLocation = new int[2];
            menuItem?.GetLocationInWindow(itemLocation);
            // Get the position of the toolbar
            int[] toolbarLocation = new int[2];
            toolbar.GetLocationInWindow(toolbarLocation);
            // Calculate the horixontal center postion of the menu item
            var itemCenterX = itemLocation[0] - toolbarLocation[0] + menuItem.Width / 2;
            // Get or calculate the distance between the bottom of the menu item icon and the bottom of the toolbar
            // NOTE: TotalPaddingBottom is actually slightly less than toolbar height - icon height / 2, however it automatically handles if the icon gravity has been modified
            BrightnessToolbarItemIconBottomPadding = menuItem.TotalPaddingBottom;
            // int itemBottomPadding = (_toolbar.Height - menuItem.ItemData.Icon.Bounds.Height())/2;
            // Calculate the width of the brightness control container required top center under the menu item
            BrightnessContainerWidth = 2 * (toolbar.Width - itemCenterX);
        }


        public static Color GetEventTypeColor(Context ctx, string colorName)
        {
            switch (colorName)
            {
                case Core.Shared.Constants.ColorName.Red: return new Color(ResourcesCompat.GetColor(ctx.Resources, Resource.Color.icon_red, null));
                case Core.Shared.Constants.ColorName.Yellow: return new Color(ResourcesCompat.GetColor(ctx.Resources, Resource.Color.app_yellow, null));
                case Core.Shared.Constants.ColorName.Blue: return new Color(ResourcesCompat.GetColor(ctx.Resources, Resource.Color.icon_blue, null));
                default: return new Color(ResourcesCompat.GetColor(ctx.Resources, Resource.Color.icon_blue, null));
            }
        }


    }
}

