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
using Android.Support.V4.Content.Res;
using Android.Views;
using Android.Widget;

namespace Edison.Mobile.Android.Common.Controls
{
    public static class Drawables
    {

        public static ShapeDrawable Circle(Color color, int intrinsicSize = 100)
        {
            ShapeDrawable drw = new ShapeDrawable(new OvalShape());
            drw.SetIntrinsicHeight(intrinsicSize);
            drw.SetIntrinsicWidth(intrinsicSize);
            drw.Paint.Color = color;
            return drw;
        }
        public static ShapeDrawable Circle(int intrinsicSize)
        {
            ShapeDrawable drw = new ShapeDrawable(new OvalShape());
            drw.SetIntrinsicHeight(intrinsicSize);
            drw.SetIntrinsicWidth(intrinsicSize);
            drw.Paint.Color = Color.White;
            return drw;
        }
        public static ShapeDrawable Circle()
        {
            ShapeDrawable drw = new ShapeDrawable(new OvalShape());
            drw.Paint.Color = Color.White;
            return drw;
        }
        public static ShapeDrawable Circle(ColorStateList colors, int intrinsicSize = 100)
        {
            var drw = Circle(intrinsicSize);
            drw.SetDrawableTint(colors);
            return drw;
        }


        public static LayerDrawable CircularIcon(Context ctx, int iconResId, Color iconColor, Color backgroundColor, int iconPadding = 10, int intrinsicSize = 100)
        {
            var icon = ResourcesCompat.GetDrawable(ctx.Resources, iconResId, null);
            icon?.SetTint(iconColor);
            var backgroundDrawable = Circle();
            backgroundDrawable?.SetTint(backgroundColor);
            var drw = new LayerDrawable(new Drawable[] { backgroundDrawable, icon });
            drw.SetLayerSize(0, intrinsicSize, intrinsicSize);
            drw.SetLayerInset(1, iconPadding, iconPadding, iconPadding, iconPadding);
            return drw;
        }
        public static LayerDrawable CircularIcon(Context ctx, int iconResId, ColorStateList iconColorList, ColorStateList backgroundColorList, int iconPadding = 10, int intrinsicSize = 100)
        {
            var icon = ResourcesCompat.GetDrawable(ctx.Resources, iconResId, null);
            icon?.SetTintList(iconColorList);
            var backgroundDrawable = Circle();
            backgroundDrawable?.SetTintList(backgroundColorList);
            var drw = new LayerDrawable(new Drawable[] { backgroundDrawable, icon });
            drw.SetLayerSize(0, intrinsicSize, intrinsicSize);
            drw.SetLayerInset(1, iconPadding, iconPadding, iconPadding, iconPadding);
            return drw;
        }

    }
}