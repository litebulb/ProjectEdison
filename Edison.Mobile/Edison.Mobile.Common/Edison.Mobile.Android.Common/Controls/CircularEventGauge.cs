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
using Android.Support.V4.Content;
using Android.Support.V4.Content.Res;
using Android.Support.V4.Graphics.Drawable;
using Android.Support.V4.View;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using static Android.Graphics.Drawables.ShapeDrawable;

namespace Edison.Mobile.Android.Common.Controls
{
    public class CircularEventGauge : FrameLayout
    {


        #region Events

        /// <summary>
        /// Event raised when profile is pressed.
        /// </summary>
        /// <value>Event</value>
        public new EventHandler Click;

        #endregion


        #region Fields

        public const int  RingThicknessDp = 5;
        public static readonly int RingThicknessPx = PixelSizeConverter.DpToPx(RingThicknessDp);


        private bool _initialized = false;

        private Drawable _circularEventGaugeDrawable = null;

        private Drawable _inactiveCircularEventGaugeDrawable = null;


        private GradientDrawable _ringDrawable = null;
        private GradientDrawable RingDrawable
        {
            get
            {
                if (_ringDrawable ==  null)
                    _ringDrawable = ContextCompat.GetDrawable(Context, Resource.Drawable.circular_event_gauge_ring) as GradientDrawable;
                return _ringDrawable;
            }
        }

        private static readonly int _countAutoTextSizeMin = PixelSizeConverter.DpToPx(16);
        private static readonly int _countAutoTextSizeMax = PixelSizeConverter.DpToPx(100);
        private static readonly int _countAutoTextSizeGranularity = PixelSizeConverter.DpToPx(1);

        private LinearLayout _button;

        #endregion


        #region Properties

        public ProgressBar Indicator { get; private set; }

        public AppCompatTextView Count { get; private set; }

        public AppCompatTextView Label { get; private set; }

        public AppCompatImageView Icon { get; private set; }


        public ShadingColorPair _ringColors = new ShadingColorPair(Color.DarkGray, Color.White);
        public ShadingColorPair RingColors
        {
            get { return _ringColors; }
            set
            {
                if (_ringColors != value)
                {
                    _ringColors = value;
                    if (_initialized)
                    {
                        CreateGauge();
                        Invalidate();
                    }
                }
            }
        }



        private ColorStateList _centerTint = ColorStateList.ValueOf(Color.DarkGray);

        public ColorStateList CenterTint
        {
            get { return _centerTint; }
            set
            {
                if (_centerTint != value)
                {
                    _centerTint = value;
                    if (_initialized)
                    {
                        CreateGauge();
                        Invalidate();
                    }
                }
            }
        }
        public void SetCenterTint(Color color)
        {
            CenterTint = ColorStateList.ValueOf(color);
        }


        private Drawable _centerDrawable = null;
        public Drawable CenterDrawable
        {
            get
            {
                if (_centerDrawable == null)
                    _centerDrawable = CreateCircleDrawable(Color.White);
                return _centerDrawable;
            }
            set
            {
                _centerDrawable = value;
                if (_initialized)
                {
                    CreateGauge();
                    Invalidate();
                }
            }
        }


        private int _ringGapPx = RingThicknessPx;
        public int RingGapPx
        {
            get { return _ringGapPx;  }
            set
            {
                if (_ringGapPx != value)
                {
                    _ringGapPx = value;
                    if (_initialized)
                    {
                        CreateGauge();
                        Invalidate();
                    }
                }
            }
        }



        /*
                // Cant change easily in shape drawable with shape= ring, would need top recreate with SkiaSharp
                public int GaugeRingThicknessPx { get; set; }
        */



        public Typeface CountTypeface
        {
            get { return Count.Typeface; }
            set
            {
                if (Count.Typeface != value)
                {
                    Count.Typeface = value;
                    if (_initialized)
                        Count.Invalidate();
                }
            }
        }
        public void SetCountTypeface(Typeface typeface, TypefaceStyle style)
        {
            Count.SetTypeface(typeface, style);
            if (_initialized)
                Count.Invalidate();
        }



        public ColorStateList CountColors
        {
            get { return Count.TextColors; }
            set
            {
                if (Count.TextColors != value)
                {
                    Count.SetTextColor(value);
                    if (_initialized)
                        Count.Invalidate();
                }
            }
        }
        public void SetCountColor(Color color)
        {
            if (Count.TextColors != ColorStateList.ValueOf(color))
            {
                Count.SetTextColor(color);
                if (_initialized)
                    Count.Invalidate();
            }
        }



        public bool CountAutoSize
        {
            get { return Count.AutoSizeTextType == AutoSizeTextType.Uniform; }
            set
            {
                if (value != (Count.AutoSizeTextType == AutoSizeTextType.Uniform))
                {
                    if (value)
                        Count.SetAutoSizeTextTypeUniformWithConfiguration(_countAutoTextSizeMin, _countAutoTextSizeMax, _countAutoTextSizeGranularity, (int)ComplexUnitType.Sp);
                    else
                        Count.SetAutoSizeTextTypeWithDefaults(AutoSizeTextType.None);
                    if (_initialized)
                        Count.Invalidate();
                }
            }
        }

        public int CountTextSizePx
        {
            get
            {
                return (int)Count.TextSize;
            }
            set
            {
                if ((int)Count.TextSize != value)
                {
                    Count.TextSize = value;
                    if (_initialized)
                        Count.Invalidate();
                }
            }
        }
        public float CountTextSizeSp
        {
            get
            {
                return PixelSizeConverter.PxToSp(Count.TextSize);
            }
            set
            {
                if (PixelSizeConverter.PxToSp(Count.TextSize) != value)
                {
                    Count.TextSize = PixelSizeConverter.SpToPx(value);
                    if (_initialized && !CountAutoSize)
                        Count.Invalidate();
                }
            }
        }


        private bool _countAllCaps = true;
        public bool CountAllCaps
        {
            get { return _countAllCaps; }
            set
            {
                if (_countAllCaps != value)
                {
                    _countAllCaps = value;
                    Count.SetAllCaps(value);
                    if (_initialized)
                        Count.Invalidate();
                }
            }
        }


        public string CountText
        {
            get { return Count.Text; }
            set
            {
                if (Count.Text != value)
                {
                    Count.Text = value;
                    if (_initialized)
                        Count.Invalidate();
                }
            }
        }



        public Typeface LabelTypeface
        {
            get { return Label.Typeface; }
            set
            {
                if (Label.Typeface != value)
                {
                    Label.Typeface = value;
                    if (_initialized)
                        Label.Invalidate();
                }
            }
        }
        public void SetLabelTypeface(Typeface typeface, TypefaceStyle style)
        {
            Label.SetTypeface(typeface, style);
            if (_initialized)
                Label.Invalidate();
        }


        public ColorStateList LabelColors
        {
            get { return Label.TextColors; }
            set
            {
                if (Label.TextColors != value)
                {
                    Label.SetTextColor(value);
                    if (_initialized)
                        Label.Invalidate();
                }
            }
        }
        public void SetLabelColor(Color color)
        {
            if (Label.TextColors != ColorStateList.ValueOf(color))
            {
                Label.SetTextColor(color);
                if (_initialized)
                    Label.Invalidate();
            }
        }


        public int LabelTextSizePx
        {
            get
            {
                return (int)Label.TextSize;
            }
            set
            {
                if ((int)Label.TextSize != value)
                {
                    Label.TextSize = value;
                    if (_initialized)
                        Label.Invalidate();
                }
            }
        }
        public float LabelTextSizeSp
        {
            get
            {
                return PixelSizeConverter.PxToSp(Label.TextSize);
            }
            set
            {
                if (PixelSizeConverter.PxToSp(Label.TextSize) != value)
                {
                    Label.TextSize = PixelSizeConverter.SpToPx(value);
                    if (_initialized)
                        Label.Invalidate();
                }
            }
        }


        private bool _labelAllCaps = true;
        public bool LabelAllCaps
        {
            get { return _labelAllCaps; }
            set
            {
                if ( _labelAllCaps != value)
                {
                    _labelAllCaps = value;
                    Label.SetAllCaps(value);
                    if (_initialized)
                        Label.Invalidate();
                }
            }
        }



        public string LabelText
        {
            get { return Label.Text; }
            set
            {
                if (Label.Text != value)
                {
                    Label.Text = value;
                    if (_initialized)
                        Label.Invalidate();
                }
            }
        }




        public Drawable IconDrawable
        {
            get { return Icon.Drawable; }
            set
            {
                if (Icon.Drawable != value && DrawableCompat.Unwrap(Icon.Drawable) != value)
                {
                    Icon.SetImageDrawable(value.Mutate());
                    if (_initialized)
                        Icon.Invalidate();
                }
            }
        }
        public void SetIconResource(int resId)
        {
            var drw = ResourcesCompat.GetDrawable(Resources, resId, null);
            if (drw != null)
                IconDrawable = drw;
        }



        public int IconSizePx
        {
            get { return Icon.Height; }
            set
            {
                if (Icon.Height != value && value > 0)
                {
                    Icon.LayoutParameters.Height = value;
                    Icon.LayoutParameters.Width = value;
                    if (_initialized)
                        Icon.Invalidate();
                }
            }
        }



        public ColorStateList IconTint
        {
            get { return Icon.SupportImageTintList; }
            set
            {
                if (Icon.SupportImageTintList != value)
                {
                    Icon.SupportImageTintList = value;
                    if (_initialized)
                        Icon.Invalidate();
                }
            }
        }
        public void SetIconColor(Color color)
        {
            if (IconTint != ColorStateList.ValueOf(color))
            {
                Icon.SupportImageTintList = ColorStateList.ValueOf(color);
                if (_initialized)
                    Icon.Invalidate();
            }
        }

        public PorterDuff.Mode IconTintMode
        {
            get { return Icon.ImageTintMode; }
            set
            {
                if (Icon.ImageTintMode != value)
                {
                    Icon.ImageTintMode = value;
                    if (_initialized)
                        Icon.Invalidate();
                }
            }
        }





        public int EventCount
        {
            get
            {
                if (int.TryParse(CountText, out int result))
                    return result;
                else
                    return -1;
            }
            set
            {
                if (value >= 0)
                {
                    CountText = value.ToString();
                    if (value == 0)
                        Active = false;
                    else
                        Active = true;
                }
            }
        }



        private bool _active = false;
        public bool Active
        {
            get { return _active; }
            set
            {
                if (_active != value)
                {
                    if (value)
                        Indicator.Visibility = ViewStates.Visible;
                    else
                        Indicator.Visibility = ViewStates.Gone;
                }
            }
        }



        private bool _rippleColorSet = false;
        private Color _rippleColor;
        public Color RippleColor
        {
            get
            {
                if (!_rippleColorSet)
                    return new Color(ContextCompat.GetColor(Context, Resource.Color.ripple_material_light));
                return _rippleColor;
            }
            set
            {
                _rippleColor = value;
                _rippleColorSet = true;
                if (_initialized)
                {
                    SetRippleColor(_rippleColor);
                    Indicator.Invalidate();
                }
            }
        }




        // Hide the Background property so it cant be called as is used for the inactive gauge drawable
        private new Drawable Background
        {
            get { return base.Background; }
            set { base.Background = value; }
        }

        // Hide the Foreground property so it cant be called as is used for the inactive gauge drawable
        private new Drawable Foreground
        {
            get { return base.Foreground; }
            set { base.Foreground = value; }
        }


        #endregion



        #region Constructors
        public CircularEventGauge(Context context) : base(context)
        {
            Initialize(context);
            SetRippleColor(RippleColor);
            //SetVisibilties();
            _initialized = true;
        }


        public CircularEventGauge(Context context, IAttributeSet attrs) : base(context, attrs, 0)
        {
            Initialize(context);
            ExtractAttributes(context, attrs, 0);
            SetRippleColor(RippleColor);
            //SetVisibilties();
            _initialized = true;
        }


        public CircularEventGauge(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Initialize(context);
            ExtractAttributes(context, attrs, defStyleAttr);
            SetRippleColor(RippleColor);
            //SetVisibilties();
            _initialized = true;

        }


        protected CircularEventGauge(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }


        #endregion




        #region Methods

        /// <summary>
        /// Method to initialize the control
        /// </summary>
        /// <params> 
        /// Context
        /// </params>
        private void Initialize(Context context)
        {
            // Inflate  the control
            LayoutInflater inflator = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
            inflator.Inflate(Resource.Layout.circular_event_gauge, this, true);
            BindViews();
            // Bug in Xamarin bindings - these aren't bound correctly so expressed manually as readonly 
   //         _countAutoTextSizeMin = Count.AutoSizeMinTextSize;
   //         _countAutoTextSizeMax = Count.AutoSizeMaxTextSize;
   //         _countAutoTextSizeGranularity = Count.AutoSizeStepGranularity;

    }


        private void BindViews()
        {
            Indicator = FindViewById<ProgressBar>(Resource.Id.event_indicator);
            Count = FindViewById<AppCompatTextView>(Resource.Id.event_count);
            Label = FindViewById<AppCompatTextView>(Resource.Id.event_label);
            Icon = FindViewById<AppCompatImageView>(Resource.Id.event_icon);
            _button = FindViewById<LinearLayout>(Resource.Id.info_holder);
        }


        /// <summary>
        /// Method to extract the controls custom attributes.
        /// </summary>
        /// <params> 
        /// Context
        /// IAttributeSet
        /// int
        /// </params>
        public void ExtractAttributes(Context context, IAttributeSet attrs, int defStyleAttr = 0)
        {
            using (TypedArray a = context.ObtainStyledAttributes(attrs, Resource.Styleable.CircularEventGauge, defStyleAttr, 0))
            {
                ApplyStylableAttributes(context, a, Resource.Styleable.CircularEventGauge);
            }
        }

        public void ApplyStylableAttributes(Context context, TypedArray a, int[] styleableRes)
        {

            if (styleableRes == Resource.Styleable.CircularEventGauge)
            {

                // Background Gauge Attributes

                bool ringColorsSet = false;
                Color startColor = Color.DarkGray;
                Color endColor = Color.White;
                if (a.HasValue(Resource.Styleable.CircularEventGauge_ringStartColor))
                {
                    startColor = a.GetColor(Resource.Styleable.CircularEventGauge_ringStartColor, Color.DarkGray);
                    ringColorsSet = true;
                }
                if (a.HasValue(Resource.Styleable.CircularEventGauge_ringEndColor))
                {
                    endColor = a.GetColor(Resource.Styleable.CircularEventGauge_ringEndColor, Color.White);
                    ringColorsSet = true;
                }
                if (ringColorsSet)
                    RingColors = new ShadingColorPair(startColor, endColor);

                if (a.HasValue(Resource.Styleable.CircularEventGauge_centerTint))
                {
                    var csl = a.GetColorStateList(Resource.Styleable.CircularEventGauge_centerTint);
                    if (csl != null)
                        CenterTint = csl;
                }

                if (a.HasValue(Resource.Styleable.CircularEventGauge_centerSrc))
                    CenterDrawable = a.GetDrawable(Resource.Styleable.CircularEventGauge_centerSrc);

                if (a.HasValue(Resource.Styleable.CircularEventGauge_ringGap))
                    RingGapPx = a.GetDimensionPixelSize(Resource.Styleable.CircularEventGauge_ringGap, RingThicknessPx);


                // Count Attributes

                bool fontPropertiesSet = false;
                Typeface typeface = null;
                string familyName = null;
                string styleName = null;

                if (a.HasValue(Resource.Styleable.CircularEventGauge_countTypeface))
                {
                    typeface = (Typeface)a.GetInt(Resource.Styleable.CircularEventGauge_countTypeface, 0);
                    fontPropertiesSet = true;
                }

                if (a.HasValue(Resource.Styleable.CircularEventGauge_countTextStyle))
                {
                    styleName = a.GetString(Resource.Styleable.CircularEventGauge_countTextStyle);
                    fontPropertiesSet = true;
                }

                if (a.HasValue(Resource.Styleable.CircularEventGauge_countFontFamily))
                {
                    Typeface tf = null;
                    // Try to resolve as a font resource
                    try
                    {
                        var resId = a.GetResourceId(Resource.Styleable.CircularEventGauge_countFontFamily, 0);
                        tf = ResourcesCompat.GetFont(Context, resId);
                    }
                    catch (Exception ex) { }

                    if (tf == null)
                        // Try to resolve as a font asset or system font
                        familyName = a.GetString(Resource.Styleable.CircularEventGauge_countFontFamily);
                    else
                        typeface = tf;
                    fontPropertiesSet = true;
                }

                if (fontPropertiesSet)
                {
                    var typefaceAndStyle = GetTypefaceFromAttrs(typeface, familyName, styleName);
                    if (typefaceAndStyle?.Item1 != null && typefaceAndStyle?.Item1 == null)
                        CountTypeface = typefaceAndStyle.Item1;
                    else if (typefaceAndStyle?.Item1 != null && typefaceAndStyle?.Item1 != null)
                        SetCountTypeface(typefaceAndStyle.Item1, (TypefaceStyle)typefaceAndStyle.Item2);
                }


                if (a.HasValue(Resource.Styleable.CircularEventGauge_countColor))
                {
                    var csl = a.GetColorStateList(Resource.Styleable.CircularEventGauge_countColor);
                    if (csl != null)
                        CountColors = csl;
                }

                if (a.HasValue(Resource.Styleable.CircularEventGauge_countAutosize))
                    CountAutoSize = a.GetBoolean(Resource.Styleable.CircularEventGauge_countAutosize, true);

                if (a.HasValue(Resource.Styleable.CircularEventGauge_countTextSize))
                    CountTextSizePx = a.GetDimensionPixelSize(Resource.Styleable.CircularEventGauge_countTextSize, PixelSizeConverter.SpToPx(20));

                if (a.HasValue(Resource.Styleable.CircularEventGauge_countAllCaps))
                    CountAllCaps = a.GetBoolean(Resource.Styleable.CircularEventGauge_countAllCaps, true);

                if (a.HasValue(Resource.Styleable.CircularEventGauge_countText))
                    CountText = a.GetString(Resource.Styleable.CircularEventGauge_countText);



                // Label Attributes

                fontPropertiesSet = false;
                typeface = null;
                familyName = null;
                styleName = null;

                if (a.HasValue(Resource.Styleable.CircularEventGauge_labelTypeface))
                {
                    typeface = (Typeface)a.GetInt(Resource.Styleable.CircularEventGauge_labelTypeface, 0);
                    fontPropertiesSet = true;
                }

                if (a.HasValue(Resource.Styleable.CircularEventGauge_labelTextStyle))
                {
                    styleName = a.GetString(Resource.Styleable.CircularEventGauge_labelTextStyle);
                    fontPropertiesSet = true;
                }

                if (a.HasValue(Resource.Styleable.CircularEventGauge_labelFontFamily))
                {
                    Typeface tf = null;
                    // Try to resolve as a font resource
                    try
                    {
                        var resId = a.GetResourceId(Resource.Styleable.CircularEventGauge_labelFontFamily, 0);
                        tf = ResourcesCompat.GetFont(Context, resId);
                    }
                    catch (Exception ex) { }

                    if (tf == null)
                        // Try to resolve as a font asset or system font
                        familyName = a.GetString(Resource.Styleable.CircularEventGauge_labelFontFamily);
                    else
                        typeface = tf;
                    fontPropertiesSet = true;
                }

                if (fontPropertiesSet)
                {
                    var typefaceAndStyle = GetTypefaceFromAttrs(typeface, familyName, styleName);
                    if (typefaceAndStyle?.Item1 != null && typefaceAndStyle?.Item1 == null)
                        LabelTypeface = typefaceAndStyle.Item1;
                    else if (typefaceAndStyle?.Item1 != null && typefaceAndStyle?.Item1 != null)
                        SetLabelTypeface(typefaceAndStyle.Item1, (TypefaceStyle)typefaceAndStyle.Item2);
                }


                if (a.HasValue(Resource.Styleable.CircularEventGauge_labelColor))
                {
                    var csl = a.GetColorStateList(Resource.Styleable.CircularEventGauge_labelColor);
                    if (csl != null)
                        LabelColors = csl;
                }

                if (a.HasValue(Resource.Styleable.CircularEventGauge_labelTextSize))
                    LabelTextSizePx = a.GetDimensionPixelSize(Resource.Styleable.CircularEventGauge_labelTextSize, PixelSizeConverter.SpToPx(10));

                if (a.HasValue(Resource.Styleable.CircularEventGauge_labelAllCaps))
                    LabelAllCaps = a.GetBoolean(Resource.Styleable.CircularEventGauge_labelAllCaps, true);

                if (a.HasValue(Resource.Styleable.CircularEventGauge_labelText))
                    LabelText = a.GetString(Resource.Styleable.CircularEventGauge_labelText);


                // Icon attributes

                if (a.HasValue(Resource.Styleable.CircularEventGauge_iconSrc))
                    IconDrawable = a.GetDrawable(Resource.Styleable.CircularEventGauge_iconSrc);

                if (a.HasValue(Resource.Styleable.CircularEventGauge_iconSize))
                    IconSizePx = a.GetDimensionPixelSize(Resource.Styleable.CircularEventGauge_iconSize, PixelSizeConverter.SpToPx(18));

                if (a.HasValue(Resource.Styleable.CircularEventGauge_iconTint))
                {
                    var csl = a.GetColorStateList(Resource.Styleable.CircularEventGauge_iconTint);
                    if (csl != null)
                        IconTint = csl;
                }

                if (a.HasValue(Resource.Styleable.CircularEventGauge_iconTintMode))
                {
                    var mode = a.GetInt(Resource.Styleable.CircularEventGauge_iconTintMode, -1);
                    if (mode != -1)
                        IconTintMode = (PorterDuff.Mode)mode;
                }



                // Other Attributes

                if (a.HasValue(Resource.Styleable.CircularEventGauge_count))
                {
                    var count = a.GetInt(Resource.Styleable.CircularEventGauge_count, -1);
                    if (count > -1)
                        EventCount = count;
                }

                if (a.HasValue(Resource.Styleable.CircularEventGauge_active))
                    Active = a.GetBoolean(Resource.Styleable.CircularEventGauge_active, true);


                if (a.HasValue(Resource.Styleable.CircularEventGauge_controlRippleColor))
                    RippleColor = a.GetColor(Resource.Styleable.CircularEventGauge_controlRippleColor, ContextCompat.GetColor(Context, Resource.Color.ripple_material_light));


                CreateGauge();
            }

        }

        private void CreateGauge()
        {
            RingDrawable?.SetColors(new int[] { RingColors.StartColor, RingColors.EndColor });
            DrawableCompat.SetTintList(CenterDrawable, CenterTint);
            _inactiveCircularEventGaugeDrawable = CreateGaugeDrawable();
            _circularEventGaugeDrawable = CreateRotateDrawable(_inactiveCircularEventGaugeDrawable);
            Indicator.IndeterminateDrawable = _circularEventGaugeDrawable;
            Background = _inactiveCircularEventGaugeDrawable;
        }


        private Tuple<Typeface, TypefaceStyle?> GetTypefaceFromAttrs(Typeface typeface, string fontFamily, string styleName)
        {
            if (fontFamily != null)
            {
                Typeface normalTypeface = null;
                if (fontFamily.Contains(".ttf") || fontFamily.Contains(".otf"))
                {
                    // should be a custom font to try to get from font assets or file
                    // try to get from assets
                    try
                    {
                        normalTypeface = Typeface.CreateFromAsset(Context.Assets, fontFamily);
                    }
                    catch
                    {
                        // try to get from files
                        try
                        {
                            normalTypeface = Typeface.CreateFromFile(fontFamily);
                        }
                        catch
                        {
                            throw new Exception("Neither Font Asset nor Font File found at " + fontFamily);
                        }
                    }
                }
                else
                {
                    // should be a system font or a font-family xml resource
                    normalTypeface = Typeface.Create(fontFamily, TypefaceStyle.Normal);
                }
                // if typeface found
                if (normalTypeface != null)
                    return ResolveStyleAndSetTypeface(normalTypeface, styleName);
            }
            return ResolveStyleAndSetTypeface(typeface, styleName);
        }

        private Tuple<Typeface, TypefaceStyle?> ResolveStyleAndSetTypeface(Typeface typeface, string styleName)
        {
            if (typeface == null)
                typeface = Typeface.Default;

            TypefaceStyle? style = ResolveTypefaceStyle(styleName);
            return new Tuple<Typeface, TypefaceStyle?>(typeface, style);

        }

        private TypefaceStyle? ResolveTypefaceStyle(string styleName)
        {
            if (string.IsNullOrWhiteSpace(styleName))
                return null;
            TypefaceStyle style = TypefaceStyle.Normal;
            if (!string.IsNullOrWhiteSpace(styleName))
            {
                // try to split style name (as may have bold, normal and italic in it). Ignore invalid names. Default to Normal.
                var styleNames = styleName.Split("|");
                if (styleNames.Contains("bold") && styleNames.Contains("italic"))
                    style = TypefaceStyle.BoldItalic;
                else if (styleNames.Contains("bold"))
                {
                    style = TypefaceStyle.Bold;
                }
                else if (styleNames.Contains("italic"))
                {
                    style = TypefaceStyle.Italic;
                }
            }
            return style;
        }




        private RotateDrawable CreateRotateDrawable(Drawable drw)
        {
            var rotate = new RotateDrawable();
            rotate.Drawable = drw;
            return rotate;
        }

        private Drawable CreateGaugeDrawable()
        {
            LayerDrawable drw = new LayerDrawable(new Drawable[] { RingDrawable, CenterDrawable });
            var inset = RingThicknessPx + RingGapPx;
            drw.SetLayerInset(0, RingThicknessPx, RingThicknessPx, RingThicknessPx, RingThicknessPx);
            drw.SetLayerInset(1, inset, inset, inset, inset);
            return drw;
        }

        private Drawable CreateCircleDrawable(Color color)
        {
            ShapeDrawable drw = new ShapeDrawable(new OvalShape());
            drw.SetIntrinsicHeight(100);
            drw.SetIntrinsicWidth(100);
            drw.Paint.Color = color;
            return drw;
        }



        private RippleDrawable GetRippleDrawable(Color rippleColor)
        {
            return new RippleDrawable(ColorStateList.ValueOf(rippleColor), null, CreateCircleDrawable(Color.White));
        }


        /// <summary>
        /// Method to set the color of the material design ripple when the button is clicked.
        /// If the Android version is less than Lollipop (5.0) then the ripple will be a solid color
        /// Note: the default is the default for material design.
        /// </summary>
        /// <params> 
        /// Color
        /// </params>
        public void SetRippleColor(Color rippleColor)
        {
            ViewCompat.SetBackground(_button, GetImageButtonBackground(rippleColor));
        }
        private Drawable GetImageButtonBackground(Color rippleColor)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                return GetRippleDrawable(rippleColor);
            return GetStateDrawable(rippleColor);
        }

        private Drawable GetStateDrawable(Color pressedColor)
        {
            var drw = CreateCircleDrawable(Color.White);
            int[][] states = new int[][] {  new int[] { global::Android.Resource.Attribute.StatePressed },
                                            new int[] { global::Android.Resource.Attribute.StateSelected },
                                            new int[] { } };
            int[] colors = new int[] { pressedColor, pressedColor, Color.Transparent };
            ColorStateList csl = new ColorStateList(states, colors);
            drw.SetDrawableTint(csl);
            return drw;
        }



        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            // Ensure the gauge is circular and not oval
            if (w > 0 && h > 0 && w != h)
            {
                var s = Math.Min(w, h);
                LayoutParameters.Width = s;
                LayoutParameters.Height = s;
                Invalidate();
            }
        }


        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            _button.Click += OnClick;
        }

        protected override void OnDetachedFromWindow()
        {
            base.OnDetachedFromWindow();
            _button.Click -= OnClick;
        }

        private void OnClick(object s, EventArgs e)
        {
            Click?.Invoke(this, e);
        }


        #endregion


    }

    public class ShadingColorPair
    {
        public Color StartColor { get; set; }
        public Color EndColor { get; set; }

        public ShadingColorPair(Color startColor, Color endColor)
        {
            StartColor = startColor;
            EndColor = endColor;
        }
    }


}