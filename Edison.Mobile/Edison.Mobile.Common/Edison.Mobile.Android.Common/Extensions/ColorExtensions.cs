using System;
using System.Collections.Generic;
using Android.Graphics;


namespace Edison.Mobile.Android.Common
{
    public static class ColorExtensions
    {
        public static int[] ToRGBAArray(this Color color)
        {
            int[] colors = new int[4];
            colors[0] = color.R;
            colors[1] = color.G;
            colors[2] = color.B;
            colors[3] = color.A;
            return colors;
        }
    }
}