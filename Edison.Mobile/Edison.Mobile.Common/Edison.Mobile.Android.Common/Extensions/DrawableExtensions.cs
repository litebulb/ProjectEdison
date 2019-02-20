using Android.Content.Res;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V4.Graphics.Drawable;

namespace Edison.Mobile.Android.Common
{
    public static class DrawableExtensions
    {

        public static void SetDrawableTint(this Drawable drawable, ColorStateList tint, bool correctEnabled = false)
        {
            if (tint != null)
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                    drawable.SetTintList(tint);
                else
                    DrawableCompat.SetTintList(drawable, tint);

                // A bug means  _drawableCenter seems to only be picking up the disabled colour form the ColorStateList, so set color manually
                if (correctEnabled)
                    CorrectDrawableTint(drawable, tint);
            }
        }

        public static void SetDrawableTint(this Drawable drawable, Color tint)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                drawable.SetTint(tint);
            else
                DrawableCompat.SetTint(drawable, tint);
        }


        public static Drawable GetTintedDrawable(this Drawable drawable, ColorStateList tint, bool mutateDrawable = false, bool correctEnabled = false, bool wrap = false)
        {
            Drawable drwbl = drawable;
            if (mutateDrawable)
                drwbl = drawable.Mutate();

            Drawable drw = drwbl;
            if (wrap || !(drawable is DrawableWrapper))
                drw = DrawableCompat.Wrap(drwbl);

            drw.SetDrawableTint(tint, correctEnabled);

            return drw;
        }

        public static Drawable GetTintedDrawable(this Drawable drawable, Color tint, bool mutateDrawable = false, bool correctEnabled = false, bool wrap = false)
        {
            Drawable drwbl = drawable;
            if (mutateDrawable)
                drwbl = drawable.Mutate();

            Drawable drw = drwbl;
            if (wrap || !(drawable is DrawableWrapper))
                drw = DrawableCompat.Wrap(drwbl);

            drw.SetDrawableTint(tint);

            return drw;
        }


        private static void CorrectDrawableTint(Drawable drw, ColorStateList tint)
        {
            // A bug means  _drawableCenter seems to only be picking up the disabled colour from the ColorStateList, so set color manually
            // Mutating the drawable doesn't fix the issue, not sure what is causing it.
            Color defaultColor = new Color(tint.DefaultColor);
            int enabledColorInt = tint.GetColorForState(new int[] { global::Android.Resource.Attribute.StateEnabled }, defaultColor);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                drw.SetTint(enabledColorInt);
            else
                DrawableCompat.SetTint(drw, enabledColorInt);
        }


        public static Drawable MutateAndWrap(this Drawable drawable)
        {
            return DrawableCompat.Wrap(drawable.Mutate());
        }


        public static BitmapDescriptor ToBitmapDescriptor(this Drawable drawable)
        {
            if (drawable is BitmapDrawable bitmapDrawable) {
                if (bitmapDrawable.Bitmap != null)
                    return BitmapDescriptorFactory.FromBitmap(bitmapDrawable.Bitmap);
            }

            Bitmap bitmap = null;
            if (drawable.IntrinsicWidth <= 0 || drawable.IntrinsicHeight <= 0)
                bitmap = Bitmap.CreateBitmap(1, 1, Bitmap.Config.Argb8888); // Single color bitmap will be created of 1x1 pixel
            else
                bitmap = Bitmap.CreateBitmap(drawable.IntrinsicWidth, drawable.IntrinsicHeight, Bitmap.Config.Argb8888);

            Canvas canvas = new Canvas(bitmap);
            drawable.Bounds = new Rect(0, 0, canvas.Width, canvas.Height);
            drawable.Draw(canvas);
            return BitmapDescriptorFactory.FromBitmap(bitmap);
        }

        public static Bitmap ToBitmap(this Drawable drw)
        {
            Bitmap bitmap = null;

            if (drw is BitmapDrawable bmDrw && bmDrw.Bitmap != null)
                    return bmDrw.Bitmap;

            if (drw.IntrinsicWidth <= 0 || drw.IntrinsicHeight <= 0)
                bitmap = Bitmap.CreateBitmap(1, 1, Bitmap.Config.Argb8888); // Single color bitmap will be created of 1x1 pixel
            else
                bitmap = Bitmap.CreateBitmap(drw.IntrinsicWidth, drw.IntrinsicHeight, Bitmap.Config.Argb8888);

            Canvas canvas = new Canvas(bitmap);
            drw.SetBounds(0, 0, canvas.Width, canvas.Height);
            drw.Draw(canvas);
            return bitmap;
        }




    }
}