using Android.Content.Res;


namespace Edison.Mobile.Android.Common
{
    public static class PixelSizeConverter
    {
        private static float _density = -1f;
        public static float Density
        {
            get
            {
                if (Density == -1)
                    _density = Resources.System.DisplayMetrics.Density;
                return _density;
            }
        }

        public static int DpToPx(float dp)
        {
            return (int) (dp * Density);
        }
        public static float PxToDp(float px)
        {
            return px / Density;
        }




    }
}