using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Edison.Mobile.Android.Common.Controls
{
    public static class RippleDrawables
    {

        public static Drawable Circular(Color color)
        {
            ShapeDrawable drw = new ShapeDrawable(new OvalShape());
            drw.SetIntrinsicHeight(100);
            drw.SetIntrinsicWidth(100);
            drw.Paint.Color = Color.White;
            return new RippleDrawable(ColorStateList.ValueOf(color), null, drw);
        }

        public static Drawable CircularCompat(Color color)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                return Circular(color);
            ShapeDrawable drw = new ShapeDrawable(new OvalShape());
            drw.SetIntrinsicHeight(100);
            drw.SetIntrinsicWidth(100);
            drw.Paint.Color = Color.White;
            int[][] states = new int[][] {  new int[] { global::Android.Resource.Attribute.StatePressed },
                                            new int[] { global::Android.Resource.Attribute.StateSelected },
                                            new int[] { } };
            int[] colors = new int[] { color, color, Color.Transparent };
            ColorStateList csl = new ColorStateList(states, colors);
            drw.SetDrawableTint(csl);
            return drw;
        }


    }
}