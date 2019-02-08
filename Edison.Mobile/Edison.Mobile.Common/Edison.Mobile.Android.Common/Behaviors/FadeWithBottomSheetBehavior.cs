using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Util;
using Java.Lang;

namespace Edison.Mobile.Android.Common.Behaviors
{
    public class FadeWithBottomSheetBehavior : CoordinatorLayout.Behavior
    {

        public FadeWithBottomSheetBehavior() : base() { }

        public FadeWithBottomSheetBehavior(Context context, IAttributeSet attrs) : base(context, attrs) { }

        protected FadeWithBottomSheetBehavior(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }


        public override bool LayoutDependsOn(CoordinatorLayout parent, Java.Lang.Object child, View dependency)
        {
            return (dependency is BottomNavigationView) || base.LayoutDependsOn(parent, child, dependency);
        }


        public override bool OnDependentViewChanged(CoordinatorLayout parent, Java.Lang.Object jChild, View dependency)
        {
            var behavior = GetBottomSheetBehavior(dependency);

            if (behavior != null)
            {
                int peekHeight = behavior.PeekHeight;
                // The default peek height is -1, which 
                // gets resolved to a 16:9 ratio with the parent
                var actualPeek = behavior.PeekHeight >= 0 ? behavior.PeekHeight : (int)(9 * (double)parent.Height / 16);

                // Only perform alpha change when view is between "hidden"/"collapsed"  and open states
                if (dependency.Top <= actualPeek)
                {
                    // calculate effective view area 
                    var fullHeight = actualPeek - parent.Height;
                    //  var dy = dependency.Top - parent.Height;
                    var fractionPosition = (float)dependency.Top / fullHeight;

                    if (fractionPosition >= 0 && fractionPosition <= 1)
                    {
                        var child = jChild.JavaCast<View>();
                        child.Alpha = fractionPosition;
                        return true;
                    }
                }
            }
            return false;
        }



        // Bottom Sheet is not a class, it can be implemented by any view that has a BottomSheetBehaviour assicated with it
        // so chekc for that
        private static bool IsBottomSheet(View view)
        {
            if (view.LayoutParameters is CoordinatorLayout.LayoutParams lp)
                return (lp.Behavior is BottomSheetBehavior);
            return false;
        }
        private static BottomSheetBehavior GetBottomSheetBehavior(View view)
        {
            if (view.LayoutParameters is CoordinatorLayout.LayoutParams lp)
                if (lp.Behavior is BottomSheetBehavior behavior)
                    return behavior;
            return null;
        }




    }
}