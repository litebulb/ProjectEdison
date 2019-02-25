using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Provider;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Edison.Mobile.Android.Common.Utilities
{
    public static class ScreenUtilities
    {

        public static ScreenBrightness GetScreenBrightnessMode(Context ctx)
        {
            return (ScreenBrightness)Settings.System.GetInt(ctx.ContentResolver, Settings.System.ScreenBrightnessMode);
        }

        public static void SetScreenBrightnessMode(Context ctx, ScreenBrightness mode)
        {
            Settings.System.PutInt(ctx.ContentResolver, Settings.System.ScreenBrightnessMode, (int)mode);
        }

        public static int GetManualScreenBrightnessLevel(Context ctx)
        {
            // force current mode to manual
            var currentMode = GetScreenBrightnessMode(ctx);
            if (currentMode == ScreenBrightness.ModeAutomatic)
                SetScreenBrightnessMode(ctx, ScreenBrightness.ModeManual);
            var brightness =  Settings.System.GetInt(ctx.ContentResolver, Settings.System.ScreenBrightness, -1);
            if (currentMode == ScreenBrightness.ModeAutomatic)
                // Change mode back
                SetScreenBrightnessMode(ctx, ScreenBrightness.ModeAutomatic);
            return brightness;
        }

        public static float GetManualScreenBrightnessLevel(Activity act)
        {
            // force current mode to manual
            var currentMode = GetScreenBrightnessMode(act);
            if (currentMode == ScreenBrightness.ModeAutomatic)
                SetScreenBrightnessMode(act, ScreenBrightness.ModeManual);
            var brightness =  act.Window.Attributes.ScreenBrightness;
            if (currentMode == ScreenBrightness.ModeAutomatic)
                // Change mode back
                SetScreenBrightnessMode(act, ScreenBrightness.ModeAutomatic);
            return brightness;
        }

        public static void SetManualScreenBrightnessLevel(Context ctx, byte value)
        {
            //Only works in Manual mode, so ensure manual mode
            SetScreenBrightnessMode(ctx, ScreenBrightness.ModeManual);
            Settings.System.PutInt(ctx.ContentResolver, Settings.System.ScreenBrightness, value);
        }

        public static void SetManualScreenBrightnessLevel(Activity act, float value)
        {
            if (value < 0 || value > 1)
                return;
            //Only works in Manual mode, so ensure manual mode
            SetScreenBrightnessMode(act, ScreenBrightness.ModeManual);
            if (value == 0)
                value = 0.01f; // for somereasona vaue of 0 sets it back to full briughtness
            // need to reassign attributes, not just update them
            var lp = act.Window.Attributes;
            lp.ScreenBrightness = value;
            act.Window.Attributes = lp;

        }



        public static float GetScreenBrightnessWithMode(Context ctx, ScreenBrightness mode)
        {
            // force current mode to manual
            var currentMode = GetScreenBrightnessMode(ctx);
            if (currentMode == ScreenBrightness.ModeAutomatic)
                SetScreenBrightnessMode(ctx, ScreenBrightness.ModeManual);
            var brightnes = GetManualScreenBrightnessLevel(ctx);
            if (mode == ScreenBrightness.ModeAutomatic)
                SetScreenBrightnessMode(ctx, ScreenBrightness.ModeAutomatic);
            return brightnes;
        }

        public static float GetScreenBrightnessWithMode(Activity act, ScreenBrightness mode)
        {
           bool canWritteSettings = Settings.System.CanWrite(act);

            // force current mode to manual
            var currentMode = (ScreenBrightness)Settings.System.GetInt(act.ContentResolver, Settings.System.ScreenBrightnessMode);
            if (currentMode == ScreenBrightness.ModeAutomatic)
                Settings.System.PutInt(act.ContentResolver, Settings.System.ScreenBrightnessMode, (int)ScreenBrightness.ModeManual);
            var brightnes = GetManualScreenBrightnessLevel(act);
            if (mode == ScreenBrightness.ModeAutomatic)
                Settings.System.PutInt(act.ContentResolver, Settings.System.ScreenBrightnessMode, (int)ScreenBrightness.ModeAutomatic);
            return brightnes;
        }


        public static ScreenStatus GetScreenStatus(Context ctx)
        {
            PowerManager powerManager = (PowerManager)ctx.GetSystemService(Context.PowerService);
            var isScreenAwake = Build.VERSION.SdkInt < BuildVersionCodes.KitkatWatch ? powerManager.IsScreenOn : powerManager.IsInteractive;
            KeyguardManager kM = (KeyguardManager)ctx.GetSystemService(Context.KeyguardService);

            if (kM.IsDeviceLocked)
            {
                if (isScreenAwake)
                    return kM.IsDeviceSecure ? ScreenStatus.OnLockedSecure : ScreenStatus.OnLockedInsecure;
                return kM.IsDeviceSecure ? ScreenStatus.OffLockedSecure : ScreenStatus.OffLockedInsecure;
            }
            return isScreenAwake ? ScreenStatus.OnUnlocked : ScreenStatus.OffUnlocked;
        }




    }

    public enum ScreenStatus
    {
        OnLockedSecure,
        OnLockedInsecure,
        OffLockedSecure,
        OffLockedInsecure,
        OnUnlocked,
        OffUnlocked
    }


}