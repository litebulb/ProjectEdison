using System;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Widget;

namespace Edison.Mobile.User.Client.Droid.Views
{
    public class PulloutView : RelativeLayout
    {
        public PulloutView(Context context) :
            base(context)
        {
            Initialize();
        }

        public PulloutView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public PulloutView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        void Initialize()
        {
            SetBackgroundColor(Color.Orange);
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);
            Console.WriteLine(Width);
            Console.WriteLine(Height);
        }
    }
}
