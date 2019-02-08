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
using Android.Util;
using Java.Lang;
using Edison.Mobile.Android.Common;

namespace Edison.Mobile.Android.Common.Behaviors
{
    public class BottomSheetAboveBottomNavigationViewBehavior : BottomSheetBehavior
    {
        private bool _dependsOnBottomBar = true;


        public BottomSheetAboveBottomNavigationViewBehavior() : base() { }

        public BottomSheetAboveBottomNavigationViewBehavior(Context context, IAttributeSet attrs) : base(context, attrs) { }

        protected BottomSheetAboveBottomNavigationViewBehavior(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }



        public override bool LayoutDependsOn(CoordinatorLayout parent, Java.Lang.Object child, View dependency)
        {
            return (dependency is BottomNavigationView) || base.LayoutDependsOn(parent, child, dependency);
        }

        public override bool OnDependentViewChanged(CoordinatorLayout parent, Java.Lang.Object jChild, View dependency)
        {

            if (dependency is BottomNavigationView bottomBar)
            {
                var child = jChild.JavaCast<View>();
                if (_dependsOnBottomBar)
                {
                    //TODO this 4dp margin is actual shadow layout height, which is 4 dp in bottomBar library ver. 2.0.2
                    float transitionY = bottomBar.TranslationY - bottomBar.Height + (State != StateExpanded ? PixelSizeConverter.DpToPx(4) : 0);
                    child.TranslationY = System.Math.Min(transitionY, 0);
                }

                if (bottomBar.TranslationY >= bottomBar.Height)
                {
                    _dependsOnBottomBar = false;
                    bottomBar.Visibility = ViewStates.Gone;
                }
                if (State != StateExpanded)
                {
                    _dependsOnBottomBar = true;
                    bottomBar.Visibility = ViewStates.Visible;
                }

                return false;
            }

            return base.OnDependentViewChanged(parent, jChild, dependency);
        }

        public class BottomSheetAboveViewBehavior<T> : BottomSheetBehavior where T : View
        {

            private bool _dependsOnView = true;


            public BottomSheetAboveViewBehavior() : base() { }

            public BottomSheetAboveViewBehavior(Context context, IAttributeSet attrs) : base(context, attrs) { }

            protected BottomSheetAboveViewBehavior(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }





            public override bool LayoutDependsOn(CoordinatorLayout parent, Java.Lang.Object child, View dependency)
            {
                return (dependency is T) || base.LayoutDependsOn(parent, child, dependency);
            }

            public override bool OnDependentViewChanged(CoordinatorLayout parent, Java.Lang.Object jChild, View dependency)
            {

                if (dependency is T view)
                {
                    var child = jChild.JavaCast<View>();
                    if (_dependsOnView)
                    {
                        //TODO this 4dp margin is actual shadow layout height, which is 4 dp in bottomBar library ver. 2.0.2
                        float transitionY = view.TranslationY - view.Height + (State != StateExpanded ? PixelSizeConverter.DpToPx(4) : 0);
                        child.TranslationY = System.Math.Min(transitionY, 0);
                    }

                    if (view.TranslationY >= view.Height)
                    {
                        _dependsOnView = false;
                        view.Visibility = ViewStates.Gone;
                    }
                    if (State != StateExpanded)
                    {
                        _dependsOnView = true;
                        view.Visibility = ViewStates.Visible;
                    }

                    return false;
                }

                return base.OnDependentViewChanged(parent, jChild, dependency);
            }


        }
    }
}