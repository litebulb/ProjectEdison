using System;
using System.Drawing;
using Android.Content.Res;
using Android.App;
using System.Collections.Generic;
using Android.Graphics;
using Android.Views;

namespace Edison.Mobile.Android.Common
{
    public static class DisplayDetails
    {
        //Display Info
        private static int _displayHeightPx = -1;
        public static int DisplayHeightPx
        {
            get
            {
                if (_displayHeightPx == -1)
                    _displayHeightPx = GetDisplayHeightPx();
                return _displayHeightPx;
            }
        }

        private static int _displayWidthPx = -1;
        public static int DisplayWidthPx
        {
            get
            {
                if (_displayWidthPx == -1)
                    _displayWidthPx = GetDisplayWidthPx();
                return _displayWidthPx;
            }
        }

        /*
        // point not used in Android
        public float DisplayHeightPoints { get { return -1f; } }

        public float DisplayWidthPoints { get { return -1f; } }

        // Note View Pixels NOT used in Android
        public int DisplayHeightViewPx { get { return -1; } }
        public int DisplayWidthViewPx { get { return -1; } }
        */

        private static int _displayHeightDip = -1;
        public static int DisplayHeightDip
        {
            get
            {
                if (_displayHeightDip == -1)
                    _displayHeightDip = GetDisplayHeightDip();
                return _displayHeightDip;
            }
        }

        private static int _displayWidthDip = -1;
        public static int DisplayWidthDip
        {
            get
            {
                if (_displayWidthDip == -1)
                    _displayWidthDip = GetDisplayWidthDip();
                return _displayWidthDip;
            }
        }


        private static float _displayHeightInches = -1f;
        public static float DisplayHeightInches
        {
            get
            {
                if (_displayHeightInches == -1f)
                    _displayHeightInches = GetScreenHeightInches();
                return _displayHeightInches;
            }
        }

        private static float _displayWidthInches = -1f;
        public static float DisplayWidthInches
        {
            get
            {
                if (_displayWidthInches == -1f)
                    _displayWidthInches = GetScreenWidthInches();
                return _displayWidthInches;
            }
        }


        private static float _displayHeightCm = -1f;
        public static float DisplayHeightCm
        {
            get
            {
                if (_displayHeightCm == -1f)
                    _displayHeightCm = GetScreenHeightCm();
                return _displayHeightCm;
            }
        }

        private static float _displayWidthCm = -1f;
        public static float DisplayWidthCm
        {
            get
            {
                if (_displayWidthCm == -1f)
                    _displayWidthCm = GetScreenWidthCm();
                return _displayWidthCm;
            }
        }


        private static float _screenDensity = -1f;
        public static float ScreenDensity
        {
            get
            {
                if (_screenDensity == -1f)
                    _screenDensity = GetScreenDensity();
                return _screenDensity;
            }
        }


        private static ScreenDensityCatagory _screenDensityCatagory = ScreenDensityCatagory.Undefined;
        public static ScreenDensityCatagory ScreenDensityCatagory
        {
            get
            {
                if (_screenDensityCatagory == ScreenDensityCatagory.Undefined)
                    _screenDensityCatagory = GetScreenDensityCatagory();
                return _screenDensityCatagory;
            }
        }


        private static int _displaySmallestDimensionPx = -1;
        public static int DisplaySmallestDimensionPx
        {
            get
            {
                if (_displaySmallestDimensionPx == -1)
                    _displaySmallestDimensionPx = GetDisplaySmallestDimensionPixels();
                return _displaySmallestDimensionPx;
            }
        }

        private static int _displayLargestDimensionPx = -1;
        public static int DisplayLargestDimensionPx
        {
            get
            {
                if (_displayLargestDimensionPx == -1)
                    _displayLargestDimensionPx = GetDisplayLargestDimensionPixels();
                return _displayLargestDimensionPx;
            }
        }

        public static float DisplayXToYScaling
        {
            get { return GetDisplayXYScale(); }
        }


        //Device Info

        public static SizeF GetDisplaySizePx()
        {
            return new SizeF(Resources.System.DisplayMetrics.WidthPixels, Resources.System.DisplayMetrics.HeightPixels);
        }

        public static int GetDisplayHeightPx()
        {
            return Resources.System.DisplayMetrics.HeightPixels;
        }

        public static int GetDisplayWidthPx()
        {
            return Resources.System.DisplayMetrics.WidthPixels;
        }

        public static float GetScreenDensity()
        {
            return Resources.System.DisplayMetrics.Density;

        }

        public static ScreenDensityCatagory GetScreenDensityCatagory()
        {
            return ScreenDensityCatagory.PixelsPerInch;
        }

        private static SizeF GetDisplaySizeDip()
        {
            float sd = GetScreenDensity();
            return new SizeF(GetDisplayWidthPx() / sd, GetDisplayHeightPx() / sd);
        }

        public static int GetDisplayHeightDip()
        {
            return (int)(GetDisplayHeightPx() / GetScreenDensity());
        }

        public static int GetDisplayWidthDip()
        {
            return (int)(GetDisplayWidthPx() / GetScreenDensity());
        }


        public static float GetScreenHeightInches()
        {
            return (float)(GetDisplayHeightPx() / Resources.System.DisplayMetrics.Ydpi);
        }

        public static float GetScreenWidthInches()
        {
            return (float)(GetDisplayWidthPx() / Resources.System.DisplayMetrics.Xdpi);
        }

        public static float GetScreenHeightCm()
        {
            return (float)(2.54f * GetDisplayHeightPx() / Resources.System.DisplayMetrics.Ydpi);
        }

        public static float GetScreenWidthCm()
        {
            return (float)(2.54f * GetDisplayWidthPx() / Resources.System.DisplayMetrics.Xdpi);
        }


        // Gets the devices smallest screen dimensions in pixels
        public static int GetDisplaySmallestDimensionPixels()
        {
            return (int)Math.Min(GetDisplayWidthPx(), GetDisplayHeightPx());
        }

        // Gets the devices largest screen dimensions in pixels
        public static int GetDisplayLargestDimensionPixels()
        {
            return (int)Math.Max(GetDisplayWidthPx(), GetDisplayHeightPx());
        }


        private static float GetDisplayXYScale()
        {
            return (float)(GetDisplayWidthPx() / GetDisplayHeightPx());
        }



        public static Dictionary<BarType, int> GetBarHeightsPx(Activity act)
        {
            var result = new Dictionary<BarType, int>();
            Rect displayFrame = new Rect();
            act.Window.DecorView.GetWindowVisibleDisplayFrame(displayFrame);
            var statusBarHeight = displayFrame.Top;
            if (statusBarHeight <= 0)
                statusBarHeight = PixelSizeConverter.DpToPx(24);

            int contentViewTop = act.Window.FindViewById(Window.IdAndroidContent).Top;
            int titleBarHeight = contentViewTop - statusBarHeight;
            //           var toolbarHeight = (int)act.Resources.GetDimension(Resource.Dimension.abc_action_bar_default_height_material);
            result.Add(BarType.StatusBar, statusBarHeight);
            result.Add(BarType.Toolbar, titleBarHeight);
            return result;
        }



    }

    public enum BarType
    {
        StatusBar,
        Toolbar
    }


    public enum ScreenDensityCatagory
    {
        Undefined,
        PixelsPerInch,
        PointsPerInch,
        PixelsPerPoint,
        PixeslPerViewPixel, // UWP
        Scale           // iOS scale factor - linear pixels per point
    }

    public enum DisplayCatagory
    {
        Undefined,
        Ldpi,
        Mdpi,
        Tvdpi,
        Hdpi,
        Xhdpi,
        Xxhdpi,
        Xxxhdpi,
        Xxxxhdpi,
        Retina
    }

    public enum DisplayUnit
    {
        Dip,
        Dp,
        Pt,
        Px,
        Sp,
        VPx
    }

}