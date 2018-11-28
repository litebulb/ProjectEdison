using System;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Widget;
using Android.Views;
using Edison.Mobile.User.Client.Droid.Shared;

namespace Edison.Mobile.User.Client.Droid.Views
{
    public class ResponsesView : RelativeLayout
    {
        public ResponsesView(Context context) :
            base(context)
        {
            Initialize();
        }

        public ResponsesView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public ResponsesView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        void Initialize()
        {
            this.LayoutParameters = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            SetBackgroundColor(Constants.Gray);
            this.AddView(createTitleLabel());
        }

        TextView createTitleLabel()
        {
            return new TextView(this.Context)
            { 
                Text = "Right Now",
                LayoutParameters = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent),
                Gravity = GravityFlags.CenterHorizontal
            };
        }


    }
}
