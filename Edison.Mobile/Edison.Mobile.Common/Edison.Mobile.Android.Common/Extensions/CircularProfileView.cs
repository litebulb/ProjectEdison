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
using Refractored.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Support.V4.Content.Res;

namespace Edison.Mobile.Android.Common.Controls
{
    public class CircularProfileView : FrameLayout
    {

        #region Events

        /// <summary>
        /// Event raised when profile is pressed.
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
        /// Profile Background
        /// </summary>
        /// <value>AppCompatImageView</value>
        /// 
        private AppCompatImageView _profileBackground;

        /// <summary>
        /// Profile Image
        /// </summary>
        /// <value>CircleImageView</value>
        private CircleImageView _profileImageView;

        /// <summary>
        /// ImageButton 
        /// </summary>
        /// <value>AppCompatTextView</value>
        public AppCompatTextView ProfileInitials { get; private set; }

        //       private bool _backgroundWrapped = false;

        #endregion


        #region Properties

        /// <summary>
        /// Button Background
        /// </summary>
        /// <value>Drawable</value>
        public new Drawable Background
        {
            get => _profileBackground?.Drawable;
            set
            {
                SetBackgroundDrawable(value);
            }
        }

        /// <summary>
        /// Profile Background Tint
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
        /// Profil Background Tint Mode
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
        /// Profile Image Drawable
        /// </summary>
        /// <value>Drawable</value>
        public Drawable ProfileDrawable
        {
            get { return _profileImageView.Drawable; }
            set
            {
                if (value != _profileImageView.Drawable)
                {
                    _profileImageView.SetImageDrawable(value);
                    if (_initialized)
                    {
                        SetVisibilties();
                        _profileImageView.Invalidate();
                    }
                }
             }
        }



        /// <summary>
        /// Profile Border Width
        /// </summary>
        /// <value>int</value>
        public int BorderWidth
        {
            get { return _profileImageView.BorderWidth; }
            set
            {
                if (value != _profileImageView.BorderWidth)
                {
                    _profileImageView.BorderWidth = value;
                    if (_initialized)
                        _profileImageView.Invalidate();
                }
            }
        }

        /// <summary>
        /// Profile Border Color
        /// </summary>
        /// <value>int</value>
        public Color BorderColor
        {
            get { return new Color(_profileImageView.BorderColor); }
            set
            {
                if (value != new Color(_profileImageView.BorderColor))
                {
                    _profileImageView.BorderColor = value;
                    if (_initialized)
                        _profileImageView.Invalidate();
                }
            }
        }


        /// <summary>
        /// Profile Initials Color
        /// </summary>
        /// <value>int</value>
        public ColorStateList TextColors
        {
            get { return ProfileInitials.TextColors; }
            set
            {
                if (value != ProfileInitials.TextColors)
                {
                    ProfileInitials.SetTextColor(value);
                    if (_initialized)
                        ProfileInitials.Invalidate();
                }
            }
        }


        /// <summary>
        /// Profile Initials Font
        /// </summary>
        /// <value>int</value>
        public Typeface TextTypeface
        {
            get { return ProfileInitials.Typeface; }
            set
            {
                if (value != ProfileInitials.Typeface)
                { 
                    ProfileInitials.Typeface = value;
                    if (_initialized)
                        ProfileInitials.Invalidate();
                }
            }
        }

        public void SetTextTypeface(Typeface typeface, TypefaceStyle style)
        {
            ProfileInitials.SetTypeface(typeface, style);
            if (_initialized)
                ProfileInitials.Invalidate();
        }



        /// <summary>
        /// Profile Initials AllCaps
        /// </summary>
        /// <value>int</value>
        public bool _textAllCaps = false;
        public bool TextAllCaps
        {
            get
            {
                return _textAllCaps;
            }
            set
            {
                _textAllCaps = value;
                ProfileInitials.SetAllCaps(value);
                if (_initialized)
                    ProfileInitials.Invalidate();
            }
        }



        /// <summary>
        /// Profile Initials
        /// </summary>
        /// <value>string</value>
        public string Text
        {
            get { return ProfileInitials.Text; }
            set
            {
                if (value != ProfileInitials.Text)
                {
                    ProfileInitials.Text = value;
                    if (_initialized)
                        ProfileInitials.Invalidate();
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
                    _profileImageView.Invalidate();
                }
            }
        }

        /*
                public override Java.Lang.Object Tag
                {
                    get => _button.Tag;
                    set
                    {
                        _button.Tag = value;
                    }
                }
        */
        #endregion





        #region Constructors
        public CircularProfileView(Context context) : base(context)
        {
            Initialize(context);
            SetRippleColor(RippleColor);
            SetVisibilties();
            _initialized = true;
        }


        public CircularProfileView(Context context, IAttributeSet attrs) : base(context, attrs, 0)
        {
            Initialize(context);
            ExtractAttributes(context, attrs, 0);
            SetRippleColor(RippleColor);
            SetVisibilties();
            _initialized = true;
        }


        public CircularProfileView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Initialize(context);
            ExtractAttributes(context, attrs, defStyleAttr);
            SetRippleColor(RippleColor);
            SetVisibilties();
            _initialized = true;
        }


        protected CircularProfileView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }


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
            inflator.Inflate(Resource.Layout.circular_profile_view, this, true);
            // Assign the control sub-components
            _profileBackground = FindViewById<AppCompatImageView>(Resource.Id.profile_background);
            _profileImageView = FindViewById<CircleImageView>(Resource.Id.profile_image);
            ProfileInitials = FindViewById<AppCompatTextView>(Resource.Id.profile_initials);
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
            using (TypedArray a = context.ObtainStyledAttributes(attrs, Resource.Styleable.CircularProfileView, defStyleAttr, 0))
            {
                ApplyStylableAttributes(context, a, Resource.Styleable.CircularProfileView);
            }
        }

        public void ApplyStylableAttributes(Context context, TypedArray a, int[] styleableRes)
        {
            // Dont need to load NavigationIcon or OverfowIcon from attributes here because will have been laoded from base class and
            // any tinting wll be done if tints etc are loaded here

            if (styleableRes == Resource.Styleable.CircularProfileView)
            {
                // Background Attributes

                if (a.HasValue(Resource.Styleable.CircularProfileView_android_background))
                    Background = a.GetDrawable(Resource.Styleable.CircularProfileView_android_background);

                if (a.HasValue(Resource.Styleable.CircularProfileView_android_backgroundTint))
                    BackgroundTint = a.GetColorStateList(Resource.Styleable.CircularProfileView_android_backgroundTint);

                if (a.HasValue(Resource.Styleable.CircularProfileView_android_backgroundTintMode))
                    BackgroundTintMode = (PorterDuff.Mode)a.GetInt(Resource.Styleable.CircularProfileView_android_backgroundTintMode, -1);


                // Profile Image attributes
                if (a.HasValue(Resource.Styleable.CircularProfileView_android_src))
                    SetProfileDrawable(a.GetDrawable(Resource.Styleable.CircularProfileView_android_src));

                if (a.HasValue(Resource.Styleable.CircularProfileView_borderWidth))
                {
                    var borderWidth = a.GetDimensionPixelSize(Resource.Styleable.CircularProfileView_borderWidth, -1);
                    BorderWidth = borderWidth > 0 ? borderWidth : 0;
                }

                if (a.HasValue(Resource.Styleable.CircularProfileView_borderColor))
                {
                    BorderColor = a.GetColor(Resource.Styleable.CircularProfileView_borderColor, Color.Transparent);
                }




                // Text Atttributes

                bool fontPropertiesSet = false;
                Typeface typeface = null;
                string familyName = null;
                string styleName = null;

                if (a.HasValue(Resource.Styleable.CircularProfileView_android_typeface))
                {
                    typeface = (Typeface)a.GetInt(Resource.Styleable.CircularProfileView_android_typeface, 0);
                    fontPropertiesSet = true;
                }

                if (a.HasValue(Resource.Styleable.CircularProfileView_android_fontFamily))
                {
                    Typeface tf = null;
                    // Try to resolve as a font resource
                    try
                    {
                        var resId = a.GetResourceId((Resource.Styleable.CircularProfileView_android_fontFamily), 0);
                        tf = ResourcesCompat.GetFont(Context, resId);
                    }
                    catch (Exception ex)
                    {
                        bool test = true;
                    }
                    
                    if (tf == null)
                        // Try to resolve as a font asset or system font
                        familyName = a.GetString(Resource.Styleable.CircularProfileView_android_fontFamily);
                    else
                        typeface = tf;
                    fontPropertiesSet = true;
                }

                if (a.HasValue(Resource.Styleable.CircularProfileView_android_textStyle))
                {
                    styleName = a.GetString(Resource.Styleable.CircularProfileView_android_textStyle);
                    fontPropertiesSet = true;
                }

                if (fontPropertiesSet)
                    SetTypefaceFromAttrs(typeface, familyName, styleName);


                if (a.HasValue(Resource.Styleable.CircularProfileView_android_textAllCaps))
                    TextAllCaps = a.GetBoolean(Resource.Styleable.CircularProfileView_android_textAllCaps, false);


                if (a.HasValue(Resource.Styleable.CircularProfileView_android_textColor))
                {
                    var csl = a.GetColorStateList(Resource.Styleable.CircularProfileView_android_textColor);
                    if (csl != null)
                        ProfileInitials.SetTextColor(csl);
                }


                if (a.HasValue(Resource.Styleable.CircularProfileView_android_text))
                    ProfileInitials.Text = a.GetString(Resource.Styleable.CircularProfileView_android_text);


                if (a.HasValue(Resource.Styleable.CircularProfileView_textPadding))
                {
                    var padding = a.GetDimensionPixelSize(Resource.Styleable.CircularProfileView_textPadding, int.MinValue);
                    if (padding != int.MinValue)
                        ProfileInitials.SetPadding(padding, padding, padding, padding);
                }

                if (a.HasValue(Resource.Styleable.CircularProfileView_controlRippleColor))
                    RippleColor = a.GetColor(Resource.Styleable.CircularProfileView_controlRippleColor, ContextCompat.GetColor(Context, Resource.Color.ripple_material_light));
            }

        }


        public void SetProfileDrawable(Drawable drw)
        {
            _profileImageView.SetImageDrawable(drw);
            SetVisibilties();
        }

        public void SetProfileResource(int resId)
        {
            _profileImageView.SetImageResource(resId);
            SetVisibilties();
        }

        public void SetProfileUri(Uri uri)
        {
            _profileImageView.SetImageURI(global::Android.Net.Uri.Parse(uri.OriginalString));  // needs testing
            SetVisibilties();
        }

        public void SetProfileUri(global::Android.Net.Uri uri)
        {
            _profileImageView.SetImageURI(uri);
            SetVisibilties();
        }

        public void SetProfileBitmap(Bitmap bitmap)
        {
            _profileImageView.SetImageBitmap(bitmap);
            SetVisibilties();
        }

        public void SetProfileIcon(Icon icon)
        {
            _profileImageView.SetImageIcon(icon);
            SetVisibilties();
        }


        public void SetTextColor(Color color)
        {
            ProfileInitials.SetTextColor(color);
            if (_initialized)
                ProfileInitials.Invalidate();
        }


        private void SetTypefaceFromAttrs(Typeface typeface, string fontFamily, string styleName)
        {
            bool resolved = false;
            if (fontFamily != null)
            {
                Typeface normalTypeface = null;
                if (fontFamily.Contains(".ttf")||fontFamily.Contains(".otf"))
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
                {
                    ResolveStyleAndSetTypeface(normalTypeface, styleName);
                    resolved = true;
                }
            }
            if (!resolved)
                ResolveStyleAndSetTypeface(typeface, styleName);
        }

        private void ResolveStyleAndSetTypeface(Typeface typeface, string styleName)
        {
            if (typeface == null)
                typeface = Typeface.Default;

            if (string.IsNullOrWhiteSpace(styleName))
                ProfileInitials.Typeface = typeface;
            else
            {
                TypefaceStyle style = ResolveTypefaceStyle(styleName);
                ProfileInitials.SetTypeface(typeface, style);
            }
        }

        private TypefaceStyle ResolveTypefaceStyle(string styleName)
        {
            TypefaceStyle style = TypefaceStyle.Normal;
            if (!string.IsNullOrWhiteSpace(styleName))
            {
                // try to split style name (as may have bold, normal and italic in it). Ignore invalid names. Deafult to Normal.
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


        /// <summary>
        /// Method to set the Profile Text as is
        /// </summary>
        /// <params> 
        /// string
        /// </params>
        public void SetText(string text)
        {
            ProfileInitials.Text = text;
            SetVisibilties();
            if (_initialized)
                ProfileInitials.Invalidate();
        }
        /// <summary>
        /// Method to set the Profile Text as is
        /// </summary>
        /// <params> 
        /// string
        /// </params>
        public void SetText(int resId)
        {
            ProfileInitials.SetText(resId);
            SetVisibilties();
            if (_initialized)
                ProfileInitials.Invalidate();
        }

        /// <summary>
        /// Method to set the Profile Initials
        /// </summary>
        /// <params> 
        /// string
        /// </params>
        public void SetInitials(string text, int maxChars = 2)
        {
            if (string.IsNullOrWhiteSpace(text))
                SetText(null);
            else
            {
                if (maxChars < 1)
                    maxChars = 2;
                var punctuation = text.Where(char.IsPunctuation).Distinct().ToArray();
                var words = text.Split().Select(x => x.Trim(punctuation));

                StringBuilder sb = new StringBuilder();
                if (words.Count() <= maxChars)
                {
                    foreach (string word in words)
                    {
                        sb.Append(word.Substring(0, 1));
                    }
                }
                else
                {
                    for (int i= 0; i < maxChars-2; i++)
                    {
                        sb.Append(words.ElementAt(i).Substring(0, 1));
                    }
                    sb.Append(words.Last().Substring(0, 1));
                }
                SetText(sb.ToString());
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
                    _profileBackground.SetImageDrawable(background);
   //                 _profileBackground.SupportImageTintList = _backgroundTint;
                }
                else
                    _profileBackground?.SetImageDrawable(background);

                if (_initialized)
                    _profileBackground?.Invalidate();
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
                    _profileBackground.SupportImageTintList = _backgroundTint;
                    if (_initialized)
                        _profileBackground?.Invalidate();
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



        private void SetVisibilties()
        {
            if (_profileImageView.Drawable == null)
            {
                ProfileInitials.Visibility = ViewStates.Visible;
            }
            else
            {
                ProfileInitials.Visibility = ViewStates.Gone;
            }
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
            ViewCompat.SetBackground(_profileImageView, RippleDrawables.CircularCompat(rippleColor));
        }


        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            _profileImageView.Click += OnClick;
        }

        protected override void OnDetachedFromWindow()
        {
            base.OnDetachedFromWindow();
            _profileImageView.Click -= OnClick;
        }

        private void OnClick(object s, EventArgs e)
        {
            Click?.Invoke(this, e);
        }


        #endregion



    }
}