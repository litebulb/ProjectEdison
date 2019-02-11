using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace Edison.Mobile.Android.Common.Behaviors
{
    public class BottomNavigationViewBehaviour : CoordinatorLayout.Behavior
    {

        public BottomNavigationViewBehaviour() : base() { }

        public BottomNavigationViewBehaviour(Context context, IAttributeSet attrs) : base(context, attrs) { }

        protected BottomNavigationViewBehaviour(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }


        public override bool OnStartNestedScroll(CoordinatorLayout coordinatorLayout, Java.Lang.Object jChild, View directTargetChild, View target, int axes, int type)
        {
            return axes == ViewCompat.ScrollAxisVertical;
        }

        public override void OnNestedPreScroll(CoordinatorLayout coordinatorLayout, Java.Lang.Object jChild, View target, int dx, int dy, int[] consumed, int type)
        {
            base.OnNestedPreScroll(coordinatorLayout, jChild, target, dx, dy, consumed, type);
            var child = jChild.JavaCast<View>();
            child.TranslationY = System.Math.Max(child.Height, child.TranslationY + dy);
        }


    }
}