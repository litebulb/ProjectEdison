
using Android.Content.Res;
using Android.Graphics;


namespace Edison.Mobile.Android.Common
{ 
    public class TextAppearance
    {
        public int TextSizePx;
        public ColorStateList TextColor;
        public Typeface Typeface;
        public TypefaceStyle Style;
        public float ShadowRadius;
        public float ShadowDx;
        public float ShadowDy;
        public Color ShadowColor;

        public TextAppearance(int textSizePx, ColorStateList textColor, Typeface typeface, TypefaceStyle style, float shadowRadius, float shadowDx, float shadowDy, Color shadowColor)
        {
            TextSizePx = textSizePx;
            TextColor = textColor;
            Typeface = typeface;
            Style = style;
            ShadowRadius = shadowRadius;
            ShadowDx = shadowDx;
            ShadowDy = shadowDy;
            ShadowColor = shadowColor;
        }

        public TextAppearance(int textSizePx, ColorStateList textColor, Typeface typeface, float shadowRadius, float shadowDx, float shadowDy, Color shadowColor)
        {
            TextSizePx = textSizePx;
            TextColor = textColor;
            Typeface = typeface;
            Style = TypefaceStyle.Normal;
            ShadowRadius = shadowRadius;
            ShadowDx = shadowDx;
            ShadowDy = shadowDy;
            ShadowColor = shadowColor;
        }

        public TextAppearance(int textSizePx, ColorStateList textColor, Typeface typeface, TypefaceStyle style, float shadowRadius, float shadowDx, float shadowDy)
        {
            TextSizePx = textSizePx;
            TextColor = textColor;
            Typeface = typeface;
            Style = style;
            ShadowRadius = shadowRadius;
            ShadowDx = shadowDx;
            ShadowDy = shadowDy;
            ShadowColor = Color.Gray;
        }

        public TextAppearance(int textSizePx, ColorStateList textColor, Typeface typeface, float shadowRadius, float shadowDx, float shadowDy)
        {
            TextSizePx = textSizePx;
            TextColor = textColor;
            Typeface = typeface;
            Style = TypefaceStyle.Normal;
            ShadowRadius = shadowRadius;
            ShadowDx = shadowDx;
            ShadowDy = shadowDy;
            ShadowColor = Color.Gray;
        }

        public TextAppearance(int textSizePx, ColorStateList textColor, float shadowRadius, float shadowDx, float shadowDy, Color shadowColor)
        {
            TextSizePx = textSizePx;
            TextColor = textColor;
            Typeface = null;
            Style = TypefaceStyle.Normal;
            ShadowRadius = shadowRadius;
            ShadowDx = shadowDx;
            ShadowDy = shadowDy;
            ShadowColor = shadowColor;
        }

        public TextAppearance(int textSizePx, ColorStateList textColor, float shadowRadius, float shadowDx, float shadowDy)
        {
            TextSizePx = textSizePx;
            TextColor = textColor;
            Typeface = null;
            Style = TypefaceStyle.Normal;
            ShadowRadius = shadowRadius;
            ShadowDx = shadowDx;
            ShadowDy = shadowDy;
            ShadowColor = Color.Gray;
        }


        public TextAppearance(int textSizePx, ColorStateList textColor, Typeface typeface, TypefaceStyle style)
        {
            TextSizePx = textSizePx;
            TextColor = textColor;
            Typeface = typeface;
            Style = style;
            ShadowRadius = 0f;
            ShadowDx = 0f;
            ShadowDy = 0f;
            ShadowColor = Color.Transparent;
        }

        public TextAppearance(int textSizePx, ColorStateList textColor, Typeface typeface)
        {
            TextSizePx = textSizePx;
            TextColor = textColor;
            Typeface = typeface;
            Style = TypefaceStyle.Normal;
            ShadowRadius = 0f;
            ShadowDx = 0f;
            ShadowDy = 0f;
            ShadowColor = Color.Transparent;
        }


        public TextAppearance(int textSizePx, ColorStateList textColor)
        {
            TextSizePx = textSizePx;
            TextColor = textColor;
            Typeface = null;
            Style = TypefaceStyle.Normal;
            ShadowRadius = 0f;
            ShadowDx = 0f;
            ShadowDy = 0f;
            ShadowColor = Color.Transparent;
        }

        public TextAppearance()
        {
            TextSizePx = -1;
            TextColor = null;
            Typeface = null;
            Style = TypefaceStyle.Normal;
            ShadowRadius = -1f;
            ShadowDx = float.MinValue;
            ShadowDy = float.MinValue;
            ShadowColor = Color.Transparent;
        }

    }
}