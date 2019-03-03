using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using BaseToolbar = Android.Support.V7.Widget.Toolbar;

namespace Edison.Mobile.Admin.Client.Droid.Toolbars
{

    [Register("com.bluemetal.Edison_Mobile_Admin_Client.CenteredToolbar")]
    public class CenteredToolbar : BaseToolbar
    {
        private TextView titleView;
        
        public CenteredToolbar(Context context)
            :this(context, null)
        {
        }
        public CenteredToolbar(Context context, IAttributeSet attrs)
            :this(context, attrs, Resource.Attribute.toolbarStyle)
        {
            
        }
        

        public CenteredToolbar(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            titleView = new TextView(context);

            int textAppearanceStyleResId;
            int textColorResId;

            TypedArray a = context.Theme.ObtainStyledAttributes(attrs,
                    new int[] { Resource.Attribute.titleTextAppearance }, defStyleAttr, 0);
            TypedArray b = context.Theme.ObtainStyledAttributes(attrs,
                    new int[] { Resource.Attribute.titleTextColor }, defStyleAttr, 0);
            try
            {
                textAppearanceStyleResId = a.GetResourceId(0, 0);
                textColorResId = b.GetResourceId(0, 0);
            }
            finally
            {
                a.Recycle();
            }
            if (textAppearanceStyleResId > 0)
            {
                titleView.SetTextAppearance(context, textAppearanceStyleResId);
                titleView.SetTextColor(Resources.GetColor(textColorResId));
            }

            AddView(titleView, new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent));
        }

        public override void SetSubtitle(int resId)
        {
            base.SetSubtitle(resId);
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);
            
            titleView.SetX((Width - titleView.Width) / 2);
        }

        public override void SetTitle(int resId)
        {
            base.SetTitle(Resource.String.blank_title);
            titleView.SetText(resId);
        }        
    }
}