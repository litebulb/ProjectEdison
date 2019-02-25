using System;
using System.Threading.Tasks;
using Android;
using Android.App;
using Edison.Core.Common.Models;
using Edison.Mobile.Common.Network;
using Edison.Mobile.Common.Notifications;
using Android.Support.V4.Content;
using Android.Content.PM;
using Android.Support.V4.App;
using Android.Gms.Common;
using Android.Content;
using Android.OS;
using Android.Graphics;
using Android.Graphics.Drawables;
using Edison.Mobile.Android.Common.Controls;
using Android.Text;
using Android.Text.Style;
using System.Collections.Generic;
using Android.Widget;

namespace Edison.Mobile.Android.Common.Notifications
{
    public class NotificationService : INotificationService
    {
        readonly NotificationRestService notificationRestService;
 //       readonly Activity mainActivity;
        readonly string permission = Manifest.Permission.AccessNotificationPolicy;

//        public NotificationService(NotificationRestService notificationRestService, Activity mainActivity)
        public NotificationService(NotificationRestService notificationRestService)
        {
//            this.mainActivity = mainActivity;
            this.notificationRestService = notificationRestService;
        }

        public Task<bool> HasNotificationPrivileges()
        {
 //           return Task.FromResult(ContextCompat.CheckSelfPermission(this.mainActivity.ApplicationContext, permission) == Permission.Granted);
            return Task.FromResult(ContextCompat.CheckSelfPermission(BaseApplication.CurrentActivity?.ApplicationContext, permission) == Permission.Granted);
        }

        public Task<bool> RequestNotificationPrivileges()
        {
            if (ContextCompat.CheckSelfPermission(BaseApplication.CurrentActivity?.ApplicationContext, permission) == Permission.Granted)
            {
                return Task.FromResult(true);
            }

            ActivityCompat.RequestPermissions(BaseApplication.CurrentActivity, new string[] { permission }, 0);
            return Task.FromResult(true);
        }

        public Task<DeviceMobileModel> RegisterForNotifications(DeviceRegistrationModel deviceRegistrationModel)
        {
            return notificationRestService.Register(deviceRegistrationModel);
        }



        public static void CreateNotificationChannel(Context ctx, string channelId, string channelName, string channelDescription, NotificationImportance importance)
        {
            // Create the NotificationChannel, but only on API 26+ because
            // the NotificationChannel class is new and not in the support library
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                NotificationChannel channel = new NotificationChannel(channelId, channelName, importance);
                channel.Description = channelDescription;

                // Register the channel with the system; you can't change the importance
                // or other notification behaviors after this
                var notificationManager = NotificationManager.FromContext(ctx);
                notificationManager.CreateNotificationChannel(channel);
 //               notificationManager.GetNotificationChannel(channelId).Importance
            }
        }






        public static void SendLocalNotification(Context ctx, string notificationChannelId, string title, int smallIconResId, Color smallIconColor, string notificationCatagory, 
                                                int notificationVisibility, int notificationId = 0, string notificationTag = "Edison", PendingIntent pendingIntent = null, bool autoCancel = true, 
                                                string summaryContent = null, int contentIconResId = 0, int color = -1, NotificationCompat.Style style = null, string groupKey = null)
        {
            var notificationManager = NotificationManager.FromContext(ctx);
            var channel = notificationManager.GetNotificationChannel(notificationChannelId);
            if (channel == null) return;

            //Map channel importance (Android 8+) to notification importance (android 7)
            int priority = NotificationCompat.PriorityDefault;
            switch (channel.Importance)
            {
                case NotificationImportance.Max:
                    priority = NotificationCompat.PriorityMax;
                    break;
                case NotificationImportance.High:
                    priority = NotificationCompat.PriorityHigh;
                    break;
                case NotificationImportance.Low:
                    priority = NotificationCompat.PriorityLow;
                    break;
                case NotificationImportance.Min:
                    priority = NotificationCompat.PriorityMin;
                    break;
                default:
                    priority = NotificationCompat.PriorityDefault;
                    break;
            }
            // ensure visibility is valid
            if (notificationVisibility > NotificationCompat.VisibilityPublic || notificationVisibility < NotificationCompat.VisibilitySecret)
                notificationVisibility = NotificationCompat.VisibilityPrivate;

            var notificationBuilder = new NotificationCompat.Builder(ctx, notificationChannelId)
                .SetContentTitle(title)
                .SetSmallIcon(smallIconResId)
                .SetColor(smallIconColor)
                .SetPriority(priority)
                .SetCategory(notificationCatagory)
                .SetVisibility(notificationVisibility)
                .SetAutoCancel(autoCancel);

            if (!string.IsNullOrWhiteSpace(summaryContent))
                notificationBuilder.SetContentText(summaryContent);
            if (pendingIntent != null)
                notificationBuilder.SetContentIntent(pendingIntent);

            Bitmap icon = null;
            if (contentIconResId != 0)
            {
                if (color != -1)
                {
                    Color col = new Color(color);
                    if (contentIconResId != 0)
                    {
                        // build circular icon
                        Drawable iconDrawable = Drawables.CircularIcon(ctx, contentIconResId, Color.White, col, PixelSizeConverter.DpToPx(15), PixelSizeConverter.DpToPx(140));
                        // try to get bitmap
                        icon = iconDrawable.ToBitmap();
                    }
                }
                else
                    // try to get bitmap
                    icon = BitmapFactory.DecodeResource(ctx.Resources, contentIconResId);

                if (icon != null)
                    notificationBuilder.SetLargeIcon(icon);
            }

            if (style != null)
                notificationBuilder.SetStyle(style);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.N && !string.IsNullOrWhiteSpace(groupKey))
                notificationBuilder.SetGroup(groupKey);

            notificationManager.Notify(notificationTag, notificationId, notificationBuilder.Build());

            icon?.Dispose();
        }




        public static void SendLocalNotification(Context ctx, string notificationChannelId, string title, int smallIconResId, Color smallIconColor, string notificationCatagory,
                                        int notificationVisibility, int notificationId, string notificationTag, PendingIntent pendingIntent, bool autoCancel, string summaryTitle,
                                        string summaryContent, int contentIconResId, Color contentIconBackgroundColor, string groupKey = null, List<NotificationCompat.Action> actions = null)
        {
            var notificationManager = NotificationManager.FromContext(ctx);
            var channel = notificationManager.GetNotificationChannel(notificationChannelId);
            if (channel == null) return;

            //Map channel importance (Android 8+) to notification importance (android 7)
            int priority = MapToPriority(channel.Importance);

            // ensure visibility is valid
            if (notificationVisibility > NotificationCompat.VisibilityPublic || notificationVisibility < NotificationCompat.VisibilitySecret)
                notificationVisibility = NotificationCompat.VisibilityPrivate;

            var notificationBuilder = new NotificationCompat.Builder(ctx, notificationChannelId)
                .SetContentTitle(title)
                .SetSmallIcon(smallIconResId)
                .SetColor(smallIconColor)
                .SetPriority(priority)
                .SetCategory(notificationCatagory)
                .SetVisibility(notificationVisibility)
                .SetAutoCancel(autoCancel);

            if (!string.IsNullOrWhiteSpace(summaryTitle))
            {
                SpannableStringBuilder sb = new SpannableStringBuilder(summaryTitle);
                sb.SetSpan(new StyleSpan(TypefaceStyle.Bold), 0, summaryTitle.Length, SpanTypes.ExclusiveExclusive);
                notificationBuilder.SetContentText(sb);
                var content = summaryContent;
                if (!string.IsNullOrWhiteSpace(content))
                {
                    sb = new SpannableStringBuilder(summaryTitle);
                    sb.Append("\n");
                    sb.Append(summaryContent);
                    sb.SetSpan(new StyleSpan(TypefaceStyle.Bold), 0, summaryTitle.Length, SpanTypes.ExclusiveExclusive);
                    notificationBuilder.SetStyle(new NotificationCompat.BigTextStyle().BigText(sb));
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(summaryContent))
                {
                    notificationBuilder.SetContentText(summaryContent);
                    if (!string.IsNullOrWhiteSpace(summaryContent))
                        notificationBuilder.SetStyle(new NotificationCompat.BigTextStyle().BigText(summaryContent));
                }
            }


            if (pendingIntent != null)
                notificationBuilder.SetContentIntent(pendingIntent);

            Bitmap icon = null;
            if (contentIconResId != 0)
            {
                // build circular icon
                Drawable iconDrawable = Drawables.CircularIcon(ctx, contentIconResId, Color.White, contentIconBackgroundColor, PixelSizeConverter.DpToPx(15), PixelSizeConverter.DpToPx(140));
                // try to get bitmap
                icon = iconDrawable.ToBitmap();
                if (icon != null)
                    notificationBuilder.SetLargeIcon(icon);
            }


            if (Build.VERSION.SdkInt >= BuildVersionCodes.N && !string.IsNullOrWhiteSpace(groupKey))
                notificationBuilder.SetGroup(groupKey);

            if (actions != null && actions.Count > 0)
            {
                foreach (var action in actions)
                {
                    notificationBuilder.AddAction(action);
                }
            }

            notificationManager.Notify(notificationTag, notificationId, notificationBuilder.Build());
            icon?.Dispose();
        }






        public static void SendLocalEdisonNotification(Context ctx, string notificationChannelId, string title, int smallIconResId, Color smallIconColor, string notificationCatagory,
                                        int notificationVisibility, int notificationId, string notificationTag, PendingIntent pendingIntent, bool autoCancel, string summaryTitle,
                                        string messageText, int contentIconResId, Color contentIconBackgroundColor, 
                                        int collapsedLayoutResId, int expandedLayoutResId, int headsupLayoutResId, 
                                        int largeIconResId, int titleResId, int responseTitleResId, int messageTextResId, int emergencyButtonResId, int activityButtonResId, int safeButtonResId,
                                        PendingIntent emergencyIntent, PendingIntent activityIntent, PendingIntent safeIntent, string groupKey = null)
        {
            var notificationManager = NotificationManager.FromContext(ctx);
            var channel = notificationManager.GetNotificationChannel(notificationChannelId);
            if (channel == null) return;

            //Map channel importance (Android 8+) to notification importance (android 7)
            int priority = MapToPriority(channel.Importance);

            // ensure visibility is valid
            if (notificationVisibility > NotificationCompat.VisibilityPublic || notificationVisibility < NotificationCompat.VisibilitySecret)
                notificationVisibility = NotificationCompat.VisibilityPrivate;

            var notificationBuilder = new NotificationCompat.Builder(ctx, notificationChannelId)
                .SetContentTitle(title)
                .SetSmallIcon(smallIconResId)
                .SetColor(smallIconColor)
                .SetPriority(priority)
                .SetCategory(notificationCatagory)
                .SetVisibility(notificationVisibility)
                .SetAutoCancel(autoCancel);

            SpannableStringBuilder sb = null;
            if (!string.IsNullOrWhiteSpace(summaryTitle))
            {
                sb = new SpannableStringBuilder(summaryTitle);
                sb.SetSpan(new StyleSpan(TypefaceStyle.Bold), 0, summaryTitle.Length, SpanTypes.ExclusiveExclusive);
                notificationBuilder.SetContentText(sb);
                if (!string.IsNullOrWhiteSpace(messageText))
                {
                    sb = new SpannableStringBuilder(summaryTitle);
                    sb.Append("\n");
                    sb.Append(messageText);
                    sb.SetSpan(new StyleSpan(TypefaceStyle.Bold), 0, summaryTitle.Length, SpanTypes.ExclusiveExclusive);
                    notificationBuilder.SetStyle(new NotificationCompat.BigTextStyle().BigText(sb));
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(messageText))
                {
                    notificationBuilder.SetContentText(messageText);
                    if (!string.IsNullOrWhiteSpace(messageText))
                        notificationBuilder.SetStyle(new NotificationCompat.BigTextStyle().BigText(messageText));
                }
            }


            if (pendingIntent != null)
                notificationBuilder.SetContentIntent(pendingIntent);

            Bitmap icon = null;
            if (contentIconResId != 0)
            {
                // build circular icon
                Drawable iconDrawable = Drawables.CircularIcon(ctx, contentIconResId, Color.White, contentIconBackgroundColor, PixelSizeConverter.DpToPx(15), PixelSizeConverter.DpToPx(140));
                // try to get bitmap
                icon = iconDrawable.ToBitmap();
                // Don't set the large icon otherwise it takes up the right hand side of the entire notification. Instead will set in the custom view
   //             if (icon != null)
   //                 notificationBuilder.SetLargeIcon(icon);
            }


            if (Build.VERSION.SdkInt >= BuildVersionCodes.N && !string.IsNullOrWhiteSpace(groupKey))
                notificationBuilder.SetGroup(groupKey);


            notificationBuilder.SetStyle(new NotificationCompat.DecoratedCustomViewStyle());

            RemoteViews collapsedContent = new RemoteViews(ctx.PackageName, collapsedLayoutResId);
            if (!string.IsNullOrWhiteSpace(title))
            {
                var titleText = title;
                if (!string.IsNullOrWhiteSpace(summaryTitle))
                    titleText = titleText + ": " + summaryTitle;
                collapsedContent.SetTextViewText(titleResId, titleText);
            }
            else if (!string.IsNullOrWhiteSpace(summaryTitle))
                collapsedContent.SetTextViewText(titleResId, summaryTitle);
            if (icon != null)
                collapsedContent.SetImageViewBitmap(largeIconResId, icon);
            collapsedContent.SetOnClickPendingIntent(emergencyButtonResId, emergencyIntent);
            collapsedContent.SetOnClickPendingIntent(activityButtonResId, activityIntent);
            collapsedContent.SetOnClickPendingIntent(safeButtonResId, safeIntent);
            notificationBuilder.SetCustomContentView(collapsedContent);
            
            RemoteViews headsupContent = new RemoteViews(ctx.PackageName, headsupLayoutResId);
            if (!string.IsNullOrWhiteSpace(title))
            {
                var titleText = title;
                if (!string.IsNullOrWhiteSpace(summaryTitle))
                    titleText = titleText + ": " + summaryTitle;
                headsupContent.SetTextViewText(titleResId, titleText);
            }
            else if (!string.IsNullOrWhiteSpace(summaryTitle))
                headsupContent.SetTextViewText(titleResId, summaryTitle);
            if (icon != null)
                headsupContent.SetImageViewBitmap(largeIconResId, icon);
            headsupContent.SetOnClickPendingIntent(emergencyButtonResId, emergencyIntent);
            headsupContent.SetOnClickPendingIntent(activityButtonResId, activityIntent);
            headsupContent.SetOnClickPendingIntent(safeButtonResId, safeIntent);
            notificationBuilder.SetCustomHeadsUpContentView(headsupContent);


            RemoteViews expandedContent = new RemoteViews(ctx.PackageName, expandedLayoutResId);
            if (!string.IsNullOrWhiteSpace(title))
                expandedContent.SetTextViewText(titleResId, title);
            if (!string.IsNullOrWhiteSpace(summaryTitle))
                expandedContent.SetTextViewText(responseTitleResId, summaryTitle);
            if (icon != null)
                expandedContent.SetImageViewBitmap(largeIconResId, icon);
            if (messageText != null)
                expandedContent.SetTextViewText(messageTextResId,  messageText);
            expandedContent.SetOnClickPendingIntent(emergencyButtonResId, emergencyIntent);
            expandedContent.SetOnClickPendingIntent(activityButtonResId, activityIntent);
            expandedContent.SetOnClickPendingIntent(safeButtonResId, safeIntent);
            notificationBuilder.SetCustomBigContentView(expandedContent);

            notificationManager.Notify(notificationTag, notificationId, notificationBuilder.Build());
            icon?.Dispose();
        }






        private static int MapToPriority(NotificationImportance importance)
        {
            //Map channel importance (Android 8+) to notification importance (android 7)
            int priority = NotificationCompat.PriorityDefault;
            switch (importance)
            {
                case NotificationImportance.Max:
                    priority = NotificationCompat.PriorityMax;
                    break;
                case NotificationImportance.High:
                    priority = NotificationCompat.PriorityHigh;
                    break;
                case NotificationImportance.Low:
                    priority = NotificationCompat.PriorityLow;
                    break;
                case NotificationImportance.Min:
                    priority = NotificationCompat.PriorityMin;
                    break;
                default:
                    priority = NotificationCompat.PriorityDefault;
                    break;
            }
            return priority;
        }


    }
}
