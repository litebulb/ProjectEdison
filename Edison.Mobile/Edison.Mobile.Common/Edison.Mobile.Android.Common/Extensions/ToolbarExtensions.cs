using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Graphics.Drawable;
using Android.Views;
using Android.Widget;

using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Edison.Mobile.Android.Common
{
    public static class ToolbarExtensions
    {
        public static void UpdateMenuItemTint(this Toolbar toolbar, ColorStateList tint)
        {
            if (tint != null)
            {
                TintMenuActionIcons(toolbar, tint);
                TintOverflowIcon(toolbar, tint);
            }
        }

        public static void UpdateMenuItemTint(this Toolbar toolbar, Color tint)
        {
            TintMenuActionIcons(toolbar, tint);
            TintOverflowIcon(toolbar, tint);
        }

        private static void TintOverflowIcon(Toolbar toolbar, ColorStateList color, PorterDuff.Mode tintMode = null)
        {
            if (color != null && toolbar.OverflowIcon != null)
            {
                if (tintMode != null)
                    DrawableCompat.SetTintMode(toolbar.OverflowIcon, tintMode);
                toolbar.OverflowIcon.SetDrawableTint(color);
            }
        }
        private static void TintOverflowIcon(Toolbar toolbar, Color color, PorterDuff.Mode tintMode = null)
        {
            if (color != null && toolbar.OverflowIcon != null)
            {
                if (tintMode != null)
                    DrawableCompat.SetTintMode(toolbar.OverflowIcon, tintMode);
                toolbar.OverflowIcon.SetDrawableTint(color);
            }
        }


        private static void TintMenuActionIcons(Toolbar toolbar, ColorStateList color, PorterDuff.Mode tintMode = null)
        {
            if (color != null)
            {
                for (int i = 0; i < toolbar.Menu.Size(); i++)
                {
                    var item = toolbar.Menu.GetItem(i);
                    var drw = item.Icon;
                    if (drw != null)
                    {
                        if (tintMode != null)
                            DrawableCompat.SetTintMode(drw, tintMode);
                        item.SetIcon(drw.GetTintedDrawable(color, true, false, true));
                    }
                    if (item.ActionView is global::Android.Support.V7.Widget.SearchView sv)
                    {
                        ImageView iv = sv.FindViewById<ImageView>(Resource.Id.search_button);
                        if (iv != null)
                        {
                            if (tintMode != null)
                                DrawableCompat.SetTintMode(iv.Drawable, tintMode);
                            iv.Drawable.SetDrawableTint(color);
                        }
                    }
                }
            }
        }

        public static void TintMenuActionIcons(Toolbar toolbar, Color color, PorterDuff.Mode tintMode = null)
        {
            for (int i = 0; i < toolbar.Menu.Size(); i++)
            {
                var item = toolbar.Menu.GetItem(i);
                var drw = item.Icon;
                if (drw != null)
                {
                    if (tintMode != null)
                        DrawableCompat.SetTintMode(drw, tintMode);
                    item.SetIcon(drw.GetTintedDrawable(color, true, false, true));
                }
                if (item.ActionView is global::Android.Support.V7.Widget.SearchView sv)
                {
                    ImageView iv = sv.FindViewById<ImageView>(Resource.Id.search_button);
                    if (iv != null)
                    {
                        if (tintMode != null)
                            DrawableCompat.SetTintMode(iv.Drawable, tintMode);
                        iv.Drawable.SetDrawableTint(color);
                    }
                }
            }
        }




    }
}