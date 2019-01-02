using System;

using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V4.Content;
using Android.Support.V4.Graphics.Drawable;
using Android.Support.V4.View;
using Android.Support.V7.Widget;



namespace Edison.Mobile.Android.Common.Controls
{
    public class CircularImageButton : FrameLayout
    {

#region Events

        /// <summary>
        /// Event raised when Button is pressed.
        /// </summary>
        /// <value>Event</value>
        public new EventHandler Click;

#endregion


#region Fields

        /// <summary>
        /// Initialized flag
        /// </summary>
        /// <value>bool</value>
        private bool _initialized = false;

        /// <summary>
        /// Button Background
        /// </summary>
        /// <value>AppCompatImageView</value>
        private AppCompatImageView _buttonBackground;

        /// <summary>
        /// ImageButton 
        /// </summary>
        /// <value>AppCompatImageView</value>
        private AppCompatImageButton _button;

 //       private bool _backgroundWrapped = false;

#endregion


#region Properties

        /// <summary>
        /// Button Background
        /// </summary>
        /// <value>Drawable</value>
        public new Drawable Background
        {
            get => _buttonBackground?.Drawable;
            set
            {
                SetBackgroundDrawable(value);
            }
        }

        /// <summary>
        /// Button Background Tint
        /// </summary>
        /// <value>ColorStateList</value>
        private ColorStateList _backgroundTint;
        public ColorStateList BackgroundTint
        {
            get { return _backgroundTint; }
            set
            {
                SetBackgroundTint(value);
            }
        }

        /// <summary>
        /// Button Background Tint Mode
        /// </summary>
        /// <value>PorterDuff.Mode</value>
        public PorterDuff.Mode _backgroundTintMode;
        public new PorterDuff.Mode BackgroundTintMode
        {
            get { return _backgroundTintMode; }
            set
            {
                if (_backgroundTintMode != value)
                {
                    _backgroundTintMode = value;
                    BackgroundTintMode = value;
                    if (_initialized)
                        Invalidate();
                }

            }
        }


        /// <summary>
        /// Button Icon
        /// </summary>
        /// <value>Drawable</value>
        private Drawable _icon;
        public Drawable Icon
        {
            get { return _icon; }
            set
            {
                SetIconDrawable(value);
            }
        }

        /// <summary>
        /// Button Icon Tint
        /// </summary>
        /// <value>ColorStateList</value>
        private ColorStateList _iconTint;
        public ColorStateList IconTint
        {
            get { return _iconTint; }
            set
            {
                if (_iconTint != value)
                {
                    _iconTint = value;
                    if (Icon != null && value != null && _button != null)
                    {
                        _button.SupportImageTintList = value;
                        if (_initialized)
                            _button.Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// Button IconTint Mode
        /// </summary>
        /// <value>PorterDuff.Mode</value>
        private PorterDuff.Mode _iconTintMode;
        public PorterDuff.Mode IconTintMode
        {
            get { return _iconTintMode; }
            set
            {
                if (_iconTintMode != value)
                {
                    _iconTintMode = value;
                    if (_button != null)
                    {
                        _button.ImageTintMode = value;
                        if (_initialized)
                            _button.Invalidate();
                    }
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
                    _button.Invalidate();
                }
            }
        }


        public override Java.Lang.Object Tag
        {
            get => _button.Tag;
            set
            {
                _button.Tag = value;
            }
        }

#endregion





#region Constructors
        public CircularImageButton(Context context) : base(context)
        {
            Initialize(context);
            SetRippleColor(RippleColor);
            _initialized = true;
        }


        public CircularImageButton(Context context, IAttributeSet attrs) : base(context, attrs, 0)
        {
            Initialize(context);
            ExtractAttributes(context, attrs, 0);
            SetRippleColor(RippleColor);
            _initialized = true;
        }


        public CircularImageButton(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Initialize(context);
            ExtractAttributes(context, attrs, defStyleAttr);
            SetRippleColor(RippleColor);
            _initialized = true;
        }


        protected CircularImageButton(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }


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
            inflator.Inflate(Resource.Layout.circular_image_button, this, true);
            // Assign the control sub-components
            _buttonBackground = FindViewById<AppCompatImageView>(Resource.Id.button_background);
            _button = FindViewById<AppCompatImageButton>(Resource.Id.button);
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
            using (TypedArray a = context.ObtainStyledAttributes(attrs, Resource.Styleable.CircularImageButton, defStyleAttr, 0))
            {
                ApplyStylableAttributes(context, a, Resource.Styleable.CircularImageButton);
            }
        }

        public void ApplyStylableAttributes(Context context, TypedArray a, int[] styleableRes)
        {
            // Dont need to load NavigationIcon or OverfowIcon from attributes here because will have been laoded from base class and
            // any tinting wll be done if tints etc are loaded here

            if (styleableRes == Resource.Styleable.CircularImageButton)
            {
                // Background Attributes

                if (a.HasValue(Resource.Styleable.CircularImageButton_buttonBackground))
                    Background = a.GetDrawable(Resource.Styleable.CircularImageButton_buttonBackground);

                if (a.HasValue(Resource.Styleable.CircularImageButton_buttonBackgroundTint))
                    BackgroundTint = a.GetColorStateList(Resource.Styleable.CircularImageButton_buttonBackgroundTint);

                if (a.HasValue(Resource.Styleable.CircularImageButton_buttonBackgroundTintMode))
                    BackgroundTintMode = (PorterDuff.Mode)a.GetInt(Resource.Styleable.CircularImageButton_buttonBackgroundTintMode, -1);


                // Button attributes

                bool iconPaddingSet = false;
                int iconPaddingLeft = int.MinValue;
                int iconPaddingTop = int.MinValue;
                int iconPaddingRight = int.MinValue;
                int iconPaddingBottom = int.MinValue;
                if (a.HasValue(Resource.Styleable.CircularImageButton_iconPadding))
                {
                    var padding = a.GetDimensionPixelSize(Resource.Styleable.CircularImageButton_iconPadding, -1);
                    iconPaddingLeft = padding;
                    iconPaddingTop = padding;
                    iconPaddingRight = padding;
                    iconPaddingBottom = padding;
                    iconPaddingSet = true;
                }
                if (a.HasValue(Resource.Styleable.CircularImageButton_iconPaddingLeft))
                {
                    iconPaddingLeft = a.GetDimensionPixelSize(Resource.Styleable.CircularImageButton_iconPaddingLeft, -1);
                    iconPaddingSet = true;
                }
                if (a.HasValue(Resource.Styleable.CircularImageButton_iconPaddingTop))
                {
                    iconPaddingTop = a.GetDimensionPixelSize(Resource.Styleable.CircularImageButton_iconPaddingTop, -1);
                    iconPaddingSet = true;
                }
                if (a.HasValue(Resource.Styleable.CircularImageButton_iconPaddingRight))
                {
                    iconPaddingRight = a.GetDimensionPixelSize(Resource.Styleable.CircularImageButton_iconPaddingRight, -1);
                    iconPaddingSet = true;
                }
                if (a.HasValue(Resource.Styleable.CircularImageButton_iconPaddingBottom))
                {
                    iconPaddingBottom = a.GetDimensionPixelSize(Resource.Styleable.CircularImageButton_iconPaddingBottom, -1);
                    iconPaddingSet = true;
                }
                if(iconPaddingSet)
                {
                    iconPaddingLeft = iconPaddingLeft == int.MinValue ? _button.PaddingLeft : iconPaddingLeft;
                    iconPaddingTop = iconPaddingTop == int.MinValue ? _button.PaddingTop : iconPaddingTop;
                    iconPaddingRight = iconPaddingRight == int.MinValue ? _button.PaddingRight : iconPaddingRight;
                    iconPaddingBottom = iconPaddingBottom == int.MinValue ? _button.PaddingBottom : iconPaddingBottom;
                    _button.SetPadding(iconPaddingLeft, iconPaddingTop, iconPaddingRight, iconPaddingBottom);
                }

                if (a.HasValue(Resource.Styleable.CircularImageButton_android_tag))
                    Tag = a.GetString(Resource.Styleable.CircularImageButton_android_tag);


                if (a.HasValue(Resource.Styleable.CircularImageButton_iconSrc))
                    SetIconDrawable(a.GetDrawable(Resource.Styleable.CircularImageButton_iconSrc));

                if (a.HasValue(Resource.Styleable.CircularImageButton_iconTint))
                    IconTint = a.GetColorStateList(Resource.Styleable.CircularImageButton_iconTint);

                if (a.HasValue(Resource.Styleable.CircularImageButton_iconTintMode))
                    IconTintMode = (PorterDuff.Mode)a.GetInt(Resource.Styleable.CircularImageButton_iconTintMode, -1);

                if (a.HasValue(Resource.Styleable.CircularImageButton_buttonRippleColor))
                    RippleColor = a.GetColor(Resource.Styleable.CircularImageButton_buttonRippleColor, ContextCompat.GetColor(Context, Resource.Color.ripple_material_light));


            }

        }


        /// <summary>
        /// Method to set the background drawable
        /// </summary>
        /// <params> 
        /// Drawable
        /// </params>
        public new void SetBackgroundDrawable(Drawable background)
        {
            if (Background != background && DrawableCompat.Unwrap(Background) != background)
            {
                if (background != null)
                {
                    _buttonBackground.SetImageDrawable(background);
                    _buttonBackground.SupportImageTintList = _backgroundTint;
                }
                else
                    _buttonBackground?.SetImageDrawable(background);

                if (_initialized)
                    _buttonBackground?.Invalidate();
            }
        }

        /// <summary>
        /// Method to set the background drawable via resource id
        /// </summary>
        /// <params> 
        /// int
        /// </params>
        public new void SetBackgroundResource(int resId)
        {
            var drw = ContextCompat.GetDrawable(Context, resId);
            SetBackgroundDrawable(drw);
        }


        /// <summary>
        /// Method to set the background Tint
        /// </summary>
        /// <params> 
        /// ColorStateList
        /// </params>
        public void SetBackgroundTint(ColorStateList tint)
        {
            if (_backgroundTint != tint)
            {
                _backgroundTint = tint;
                if (Background != null && tint != null)
                {
                    _buttonBackground.SupportImageTintList = _backgroundTint;
                    if (_initialized)
                        _buttonBackground?.Invalidate();
                }
            }
        }

        /// <summary>
        /// Method to set the background Tint
        /// </summary>
        /// <params> 
        /// ColorStateList
        /// </params>
        public void SetBackgroundTint(Color tint)
        {
            SetBackgroundTint(ColorStateList.ValueOf(tint));
        }


        /// <summary>
        /// Method to set the icon drawable
        /// </summary>
        /// <params> 
        /// Drawable
        /// </params>
        public void SetIconDrawable(Drawable icon)
        {
            if (_icon != icon && DrawableCompat.Unwrap(_icon) != icon)
            {
/*
                if (icon != null)
                {
                    _icon = DrawableCompat.Wrap(icon.Mutate());
                    _icon.SetDrawableTint(_iconTint);
                }
                else
                    _icon = icon;
                _button?.SetImageDrawable(_icon);
                if (_initialized)
                    _button?.Invalidate();
*/
                if (icon != null)
                    _icon = DrawableCompat.Wrap(icon.Mutate());
                else
                    _icon = icon;
                _button?.SetImageDrawable(_icon);
                _button.SupportImageTintList = _iconTint;
                if (_initialized)
                    _button?.Invalidate();






            }
        }

        /// <summary>
        /// Method to set the icon drawable via resource id
        /// </summary>
        /// <params> 
        /// int
        /// </params>
        public void SetIconResource(int resId)
        {
            var icon = ContextCompat.GetDrawable(Context, resId);
            SetIconDrawable(icon);
        }


        /// <summary>
        /// Method to set the padding between the icon and the edge of the control
        /// </summary>
        /// <params> 
        /// int
        /// </params>
        public void SetIconPadding(int padding)
        {
            _button?.SetPadding(padding, padding, padding, padding);
            Invalidate();
        }
        /// <summary>
        /// Method to set the padding between the icon and the edge of the control
        /// </summary>
        /// <params> 
        /// int
        /// </params>
        public void SetIconPadding(int iconPaddingLeft, int iconPaddingTop, int iconPaddingRight, int iconPaddingBottom)
        {
            _button?.SetPadding(iconPaddingLeft, iconPaddingTop, iconPaddingRight, iconPaddingBottom);
            Invalidate();
        }


        /// <summary>
        /// Method to set the color ofthe material design ripple when the button is clicked.
        /// If the Android version is less than Lollipop (5.0) then the ripple will be a solid color
        /// Note: the default is the defaut for material design.
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

        private RippleDrawable GetRippleDrawable(Color rippleColor)
        {
            return new RippleDrawable(ColorStateList.ValueOf(rippleColor), null, GetCircleDrawable(Color.White));
        }

        private ShapeDrawable GetStateDrawable(Color pressedColor)
        {
            var drw = GetCircleDrawable(Color.White);
            int[][] states = new int[][] {  new int[] { global::Android.Resource.Attribute.StatePressed },
                                            new int[] { global::Android.Resource.Attribute.StateSelected },
                                            new int[] { } };
            int[] colors = new int[] { pressedColor, pressedColor, Color.Transparent };
            ColorStateList csl = new ColorStateList(states, colors);
            drw.SetDrawableTint(csl);
            return drw;
        }

        private ShapeDrawable GetCircleDrawable(Color color, int diameterPx = 10)
        {
            ShapeDrawable drw = new ShapeDrawable(new OvalShape());
            drw.SetIntrinsicHeight(diameterPx);
            drw.SetIntrinsicWidth(diameterPx);
            drw.Paint.Color = color;
            return drw;
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
}