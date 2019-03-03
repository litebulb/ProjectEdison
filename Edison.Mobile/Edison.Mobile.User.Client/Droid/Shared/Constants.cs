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
using Android.Util;
using System.Collections.Generic;

namespace Edison.Mobile.User.Client.Droid
{
    public static class Constants
    {
        public const string ClientId = "19cb746c-3066-4cd8-8cd2-e0ce1176ae33"; //"64531b8c-3d22-4c2a-8d72-bf37c8609fbe";

        // Response summary Map values
        internal const int UserLocationJitterThreshold = 3; //meters
        internal const int SingleLocationRefocusMapThreshold = 5000; // meters
        internal const float LocationThresholdPercent = 0.1f; // % as fraction

        internal const string EVENT_CHANNEL_ID = "edison_notification_channel";
        internal const string NotificationTagLabel = "NotificationTag";
        internal const string NotificationIdLabel = "NotificationId";
        internal const string NotificationTag = "Edison";
        internal const int NotificationId = 100;

        internal const string IntentSourceLabel = "Source";
        internal const string IntentActionLabel = "Action";
        internal const string IntentAuthenticatedLabel = "Authenticated";
        internal const string IntentDataResponseLabel= "Response";
        internal const string IntentDataUserLatLabel= "UserLat";
        internal const string IntentDataUserLongLabel= "UserLong";
        internal const string IntentSourceLogout = "Logout";
        internal const string IntentSourceBackgroundNotification = "BackgroundNotification";
        internal const string IntentSourceNotRunningNotification = "NotRunningNotification";
        internal const string IntentSourceForegroundNotification = "ForegrounddNotification";
        internal const string IntentSourceNotForegroundNotification = "NotForegrounddNotification";
        internal const string IntentAuthenticated = "True";
        internal const string IntentNotAuthenticated = "False";

        internal const string ActionEmergency = "Emergency";
        internal const string ActionActivity = "Activity";
        internal const string ActionSafe = "Safe";

        internal const string MessageTypeSilent = "silent";
        internal const string MessageDataAction = "action";
        internal const string MessageDataMessage = "message";

        internal const string CurrentResponseColorKey = "CurrentColor";


        public static int StatusBarHeightPx { get; private set; } = PixelSizeConverter.DpToPx(24);

        public static int ToolbarHeightPx { get; private set; } = -1;

        public static int AvailablePageHeightPx { get; private set; } = -1;


        public static int QuickChatIconHorizontalMarginPx { get; private set; } = -1;
        public static int QuickChatIconVerticalMarginsPx { get; private set; } = -1;
        public static int QuickChatIconLabelHeightPx { get; private set; } = -1;
        public static int BottomSheetThumbHeightPx { get; private set; } = -1;
        public static int QuickChatIconHorizontalMarginsPx { get; private set; } = -1;
        public static int QuickChatIconButtonDiameterPx { get; private set; } = -1;
        public static int QuickChatSmallIconButtonDiameterPx { get; private set; } = -1;
        public static int QuickChatIconButtonPaddingPx { get; private set; } = -1;
        public static int QuickChatSmallIconButtonPaddingPx { get; private set; } = -1;


        public static int BottomSheetPeekHeightPx { get; private set; } = -1;
        public static int BottomSheetHeightPx { get; private set; } = -1;
        public static int BottomSheetThumbTotalHeightPx { get; private set; } = -1;
        public static int BottomSheetContentHeightPx { get; private set; } = -1;

        public static int BottomSheetSmallPeekHeightPx { get; private set; } = -1;
        public static int AvailableDetailBottomSheetHeightPx { get; private set; } = -1;

        public static Padding PagePaddingPx { get; private set; }

        public static int EventGaugeAreaHeightPx { get; private set; } = -1;
        public static int EventGaugeSizePx { get; private set; } = -1;

        public static int EventResponseAreaHeightPx { get; private set; } = -1;
        public static int EventResponseCardWidthPx { get; private set; } = -1;
        public static int EventResponseCardSeperatorWidthPx { get; private set; } = -1;

        public static int BrightnessContainerWidth { get; private set; } = -1;
        public static int BrightnessToolbarItemIconBottomPadding { get; private set; } = -1;



        public readonly static Color DefaultResponseColor = Color.Argb(255, 34, 240, 255);

        public const float DefaultResponseMapZoom = 5f;


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
            QuickChatIconHorizontalMarginPx =
                (int)act.Resources.GetDimension(Resource.Dimension.bottom_sheet_button_icon_padding);
            QuickChatIconVerticalMarginsPx = QuickChatIconHorizontalMarginPx +
                                                 (int)act.Resources.GetDimension(Resource.Dimension
                                                     .bottom_sheet_button_icon_toppadding);
            QuickChatIconLabelHeightPx = act.Resources.GetDimensionPixelSize(Resource.Dimension.bottom_sheet_button_text_size);
            BottomSheetThumbHeightPx =
                2 * (int)act.Resources.GetDimension(Resource.Dimension.bottom_sheet_thumb_padding) +
                (int)act.Resources.GetDimension(Resource.Dimension.bottom_sheet_thumb_height);
            QuickChatIconHorizontalMarginsPx = 2 *QuickChatIconHorizontalMarginPx;
            QuickChatIconButtonDiameterPx = (int)(displayWidthPx / 3 - QuickChatIconHorizontalMarginsPx);
            QuickChatSmallIconButtonDiameterPx = QuickChatIconButtonDiameterPx / 2;

            QuickChatIconButtonPaddingPx = act.Resources.GetDimensionPixelSize(Resource.Dimension.large_message_button_icon_padding);
            QuickChatSmallIconButtonPaddingPx = act.Resources.GetDimensionPixelSize(Resource.Dimension.small_message_button_icon_padding);

            BottomSheetPeekHeightPx = QuickChatIconButtonDiameterPx + QuickChatIconLabelHeightPx + BottomSheetThumbHeightPx +
                                      QuickChatIconVerticalMarginsPx;
            // BottomSheet Height
            BottomSheetHeightPx = displayHeightPx - act.Resources.GetDimensionPixelSize(Resource.Dimension.abc_action_bar_default_height_material);
            BottomSheetThumbTotalHeightPx = act.Resources.GetDimensionPixelSize(Resource.Dimension.bottom_sheet_thumb_height) +
                                                2 * act.Resources.GetDimensionPixelSize(Resource.Dimension.bottom_sheet_thumb_padding);
            BottomSheetContentHeightPx = BottomSheetHeightPx - BottomSheetThumbTotalHeightPx;

            BottomSheetSmallPeekHeightPx = QuickChatSmallIconButtonDiameterPx + BottomSheetThumbHeightPx + QuickChatIconVerticalMarginsPx;;
            AvailableDetailBottomSheetHeightPx = BottomSheetHeightPx - BottomSheetSmallPeekHeightPx;

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
            // Calculate the horizontal center position of the menu item
            var itemCenterX = itemLocation[0] - toolbarLocation[0] + menuItem.Width / 2;
            // Get or calculate the distance between the bottom of the menu item icon and the bottom of the toolbar
            // NOTE: TotalPaddingBottom is actually slightly less than toolbar height - icon height / 2, however it automatically handles if the icon gravity has been modified
            BrightnessToolbarItemIconBottomPadding = menuItem.TotalPaddingBottom;
            // int itemBottomPadding = (_toolbar.Height - menuItem.ItemData.Icon.Bounds.Height())/2;
            // Calculate the width of the brightness control container required top center under the menu item
            BrightnessContainerWidth = 2 * (toolbar.Width - itemCenterX);


        }



        private static readonly bool _eventRedSet = false;
        private static Color _eventRed;
        private static readonly bool _eventYellowSet = false;
        private static Color _eventYellow;
        private static readonly bool _eventBlueSet = false;
        private static Color _eventBlue;
        public static Color GetEventTypeColor(Context ctx, string colorName)
        {
            switch (colorName)
            {
                case Core.Shared.Constants.ColorName.Red:
                    if (!_eventRedSet)
                        _eventRed = new Color(ResourcesCompat.GetColor(ctx.Resources, Resource.Color.icon_red, null));
                    return _eventRed;
                case Core.Shared.Constants.ColorName.Yellow:
                    if (!_eventYellowSet)
                        _eventYellow = new Color(ResourcesCompat.GetColor(ctx.Resources, Resource.Color.app_yellow, null));
                    return _eventYellow;
                case Core.Shared.Constants.ColorName.Blue:
                    if (!_eventBlueSet)
                        _eventBlue = new Color(ResourcesCompat.GetColor(ctx.Resources, Resource.Color.icon_blue, null));
                    return _eventBlue;
                default:
                    if (!_eventBlueSet)
                        _eventBlue = new Color(ResourcesCompat.GetColor(ctx.Resources, Resource.Color.icon_blue, null));
                    return _eventBlue;
            }
        }

        public static readonly Dictionary<string, string> ChatMessageButtonNameToColorMap = new Dictionary<string, string>()
        {
            { "Fire", Core.Shared.Constants.ColorName.Red},
            { "fire", Core.Shared.Constants.ColorName.Red},
            { "gun", Core.Shared.Constants.ColorName.Red},
            { "Gun", Core.Shared.Constants.ColorName.Red},
            { "shooter", Core.Shared.Constants.ColorName.Red},
            { "Shooter", Core.Shared.Constants.ColorName.Red},
            { "Active Shooter", Core.Shared.Constants.ColorName.Red},
            { "active shooter", Core.Shared.Constants.ColorName.Red},
            { "health", Core.Shared.Constants.ColorName.Blue},
            { "Health", Core.Shared.Constants.ColorName.Blue},
            { "Health Check", Core.Shared.Constants.ColorName.Blue},
            { "health check", Core.Shared.Constants.ColorName.Blue},
            { "pollution", Core.Shared.Constants.ColorName.Yellow},
            { "Pollution", Core.Shared.Constants.ColorName.Yellow},
            { "Air Quality", Core.Shared.Constants.ColorName.Yellow},
            { "air quality", Core.Shared.Constants.ColorName.Yellow},
            { "protest", Core.Shared.Constants.ColorName.Blue},
            { "Protest", Core.Shared.Constants.ColorName.Blue},
            { "package", Core.Shared.Constants.ColorName.Red},
            { "Package", Core.Shared.Constants.ColorName.Red},
            { "suspicious package", Core.Shared.Constants.ColorName.Red},
            { "Suspicious Package", Core.Shared.Constants.ColorName.Red},
            { "Bomb", Core.Shared.Constants.ColorName.Red},
            { "bomb", Core.Shared.Constants.ColorName.Red},
            { "tornado", Core.Shared.Constants.ColorName.Yellow},
            { "Tornado", Core.Shared.Constants.ColorName.Yellow},
            { "vip", Core.Shared.Constants.ColorName.Blue},
            { "Vip", Core.Shared.Constants.ColorName.Blue},
            { "VIP", Core.Shared.Constants.ColorName.Blue},
            { "emergency", Core.Shared.Constants.ColorName.Red},
            { "Emergency", Core.Shared.Constants.ColorName.Red}
        };

        public static readonly Dictionary<string, string> ChatMessageButtonNameToIconMap = new Dictionary<string, string>()
        {
            { "Fire", "fire"},
            { "fire", "fire"},
            { "gun", "gun"},
            { "Gun", "gun"},
            { "shooter", "gun"},
            { "Shooter", "gun"},
            { "Active Shooter", "gun"},
            { "active shooter", "gun"},
            { "health", "health_check"},
            { "Health", "health_check"},
            { "Health Check", "health_check"},
            { "health check", "health_check"},
            { "pollution", "air_quality"},
            { "Pollution", "air_quality"},
            { "Air Quality", "air_quality"},
            { "air quality", "air_quality"},
            { "protest", "protest"},
            { "Protest", "protest"},
            { "package", "suspicious_package"},
            { "Package", "suspicious_package"},
            { "Suspicious Package", "suspicious_package"},
            { "suspicious package", "suspicious_package"},
            { "Bomb", "suspicious_package"},
            { "bomb", "suspicious_package"},
            { "tornado", "tornado"},
            { "Tornado", "tornado"},
            { "vip", "vip"},
            { "Vip", "vip"},
            { "VIP", "vip"},
            { "emergency", "emergency"},
            { "Emergency", "emergency"}
        };



        public static Tuple<string, Color> GetChatMessageButtonSettings(Context ctx, string name, string fallbackIconName = null)
        {
            string iconName;
            if (!ChatMessageButtonNameToIconMap.TryGetValue(name, out iconName))
            {
                if (!string.IsNullOrWhiteSpace(fallbackIconName) && !ChatMessageButtonNameToIconMap.TryGetValue(fallbackIconName, out iconName))
                    iconName = fallbackIconName;
            }
            if (string.IsNullOrWhiteSpace(iconName))
                iconName = "emergency";
            string colorName;
            if (!ChatMessageButtonNameToColorMap.TryGetValue(name, out colorName))
            {
                if (!ChatMessageButtonNameToColorMap.TryGetValue(fallbackIconName, out colorName))
                    colorName = Core.Shared.Constants.ColorName.Blue;
            }
            return new Tuple<string, Color>(iconName, GetEventTypeColor(ctx, colorName));
        }


        public static Tuple<string, Color> GetIconSettingsFromTitle(Context ctx, string title)
        {
            var name = InferNameFromTitle(ctx, title);
            return GetChatMessageButtonSettings(ctx, name);
        }

        public static string InferNameFromTitle(Context ctx, string title)
        {
            var _title = title.ToLowerInvariant();
            if (_title.Contains(ctx.Resources.GetString(Resource.String.shooter)) || _title.Contains(ctx.Resources.GetString(Resource.String.gun)))
                return "Active Shooter";
            if (_title.Contains(ctx.Resources.GetString(Resource.String.suspicious)) || _title.Contains(ctx.Resources.GetString(Resource.String.pckg)) || _title.Contains(ctx.Resources.GetString(Resource.String.bomb)))
                return "Suspicious Package";
            if (_title.Contains(ctx.Resources.GetString(Resource.String.fire_)))
                return "Fire";
            if (_title.Contains(ctx.Resources.GetString(Resource.String.protest_)) || _title.Contains(ctx.Resources.GetString(Resource.String.demonstration)) ||
                _title.Contains(ctx.Resources.GetString(Resource.String.disturbance)) || _title.Contains(ctx.Resources.GetString(Resource.String.riot)) ||
                _title.Contains(ctx.Resources.GetString(Resource.String.violence)) || _title.Contains(ctx.Resources.GetString(Resource.String.barricade)))
                return "Protest";
            if (_title.Contains(ctx.Resources.GetString(Resource.String.health)) || _title.Contains(ctx.Resources.GetString(Resource.String.virus)) ||
                _title.Contains(ctx.Resources.GetString(Resource.String.infection)) || _title.Contains(ctx.Resources.GetString(Resource.String.illness)))
                return "Health Check";
            if (_title.Contains(ctx.Resources.GetString(Resource.String.air)) || _title.Contains(ctx.Resources.GetString(Resource.String.pollution)) || _title.Contains(ctx.Resources.GetString(Resource.String.pollen)))
                return "Air Quality";
            if (_title.Contains(ctx.Resources.GetString(Resource.String.tornado_)) || _title.Contains(ctx.Resources.GetString(Resource.String.wind)))
                return "Tornado";
            if (_title.Contains(ctx.Resources.GetString(Resource.String.vip_)) || _title.Contains(ctx.Resources.GetString(Resource.String.celebrity)) || _title.Contains(ctx.Resources.GetString(Resource.String.famous)))
                return "VIP";
            return "Emergency";
        }




    }
}

