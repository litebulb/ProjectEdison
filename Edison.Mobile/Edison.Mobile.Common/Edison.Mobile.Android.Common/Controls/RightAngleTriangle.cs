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
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Edison.Mobile.Android.Common.Controls
{
    public class RightAngleTriangle : View
    {

        private bool _initialized = false;

        private RightAnglePosition _rightAnglePosition = RightAnglePosition.BottomLeft;
        public RightAnglePosition RightAnglePosition
        {
            get { return _rightAnglePosition; }
            set
            {
                if (value != _rightAnglePosition)
                {
                    _rightAnglePosition = value;
                    if (!_initialized)
                        Invalidate();
                }
            }
        }

        private Color _fillColor = Color.White;
        public Color FillColor
        {
            get { return _fillColor; }
            set
            {
                if (value != _fillColor)
                {
                    _fillColor = value;
                    if (!_initialized)
                        Invalidate();
                }
            }
        }

        private Color _borderColor = Color.Transparent;
        public Color BorderColor
        {
            get { return _borderColor; }
            set
            {
                if (value != _borderColor)
                {
                    _borderColor = value;
                    if (!_initialized)
                        Invalidate();
                }
            }
        }

        private int _borderThicknessPx  = 0;
        public int BorderThicknessPx
        {
            get { return _borderThicknessPx; }
            set
            {
                if (value != _borderThicknessPx)
                {
                    _borderThicknessPx = value;
                    if (!_initialized)
                        Invalidate();
                }
            }
        }



        public RightAngleTriangle(Context context) : base(context)
        {
            _initialized = true;
        }

        public RightAngleTriangle(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            ExtractAttributes(context, attrs);
            _initialized = true;
        }


        public RightAngleTriangle(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            ExtractAttributes(context, attrs, defStyle);
            _initialized = true;
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
            using (TypedArray a = context.ObtainStyledAttributes(attrs, Resource.Styleable.RightAngleTriangle, defStyleAttr, 0))
            {
                ApplyStylableAttributes(context, a, Resource.Styleable.RightAngleTriangle);
            }
        }

        public void ApplyStylableAttributes(Context context, TypedArray a, int[] styleableRes)
        {
            if (styleableRes == Resource.Styleable.RightAngleTriangle)
            {
                if (a.HasValue(Resource.Styleable.RightAngleTriangle_rightAnglePosition))
                    RightAnglePosition = (RightAnglePosition)a.GetInt(Resource.Styleable.RightAngleTriangle_rightAnglePosition, 0);

                if (a.HasValue(Resource.Styleable.RightAngleTriangle_fillColor))
                    FillColor = a.GetColor(Resource.Styleable.RightAngleTriangle_fillColor, Color.White);

                if (a.HasValue(Resource.Styleable.RightAngleTriangle_borderWidth))
                {
                    var borderWidth = a.GetDimensionPixelSize(Resource.Styleable.RightAngleTriangle_borderWidth, 0);
                    BorderThicknessPx = borderWidth > 0 ? borderWidth : 0;
                }

                if (a.HasValue(Resource.Styleable.RightAngleTriangle_borderColor))
                    BorderColor = a.GetColor(Resource.Styleable.RightAngleTriangle_borderColor, Color.Transparent);
            }
        }



        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            Path path = new Path();

            switch (RightAnglePosition)
            {
                case (RightAnglePosition.TopLeft):
                    path.MoveTo(0, 0);
                    path.LineTo(Width, 0);
                    path.LineTo(0, Height);
                    path.LineTo(0, 0);
                    break;

                case (RightAnglePosition.TopRight):
                    path.MoveTo(0, 0);
                    path.LineTo(Width, 0);
                    path.LineTo(Width, Height);
                    path.LineTo(0, 0);
                    break; 

                case (RightAnglePosition.BottomRight):
                    path.MoveTo(Width, 0);
                    path.LineTo(Width, Height);
                    path.LineTo(0, Height);
                    path.LineTo(Width, 0);
                    break;

                default:   // BottomLeft
                    path.MoveTo(0, 0);
                    path.LineTo(Width, Height);
                    path.LineTo(0, Height);
                    path.LineTo(0, 0);
                    break;
            }

            path.Close();

            Paint p = new Paint();

            if (FillColor != Color.Transparent)
            {
                p.SetStyle(Paint.Style.Fill);
                p.Color = FillColor;
                canvas.DrawPath(path, p);
            }

            if (BorderThicknessPx > 0 && BorderColor != Color.Transparent)
            {
                p.SetStyle(Paint.Style.Stroke);
                p.StrokeWidth = BorderThicknessPx;
                p.StrokeJoin = Paint.Join.Miter;
                p.Color = BorderColor;
                canvas.DrawPath(path, p);
            }
        }

    }

    public enum RightAnglePosition
    {
        BottomLeft = 0,
        TopLeft = 1,
        TopRight = 2,
        BottomRight = 3,
    }

}