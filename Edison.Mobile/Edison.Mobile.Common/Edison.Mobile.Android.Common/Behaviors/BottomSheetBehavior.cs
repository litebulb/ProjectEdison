using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace Edison.Mobile.Android.Common.Behaviors
{
    public class BottomSheetBehavior : global::Android.Support.Design.Widget.BottomSheetBehavior
    {

        public EventHandler<float> Slide;

        public EventHandler<SheetState> StateChanged;

        private bool _nestedScrollingViewTouched = false;

        public bool Enabled { get; set; } = true;

        public List<int> NestedScrollingViewIds { get; } = new List<int>();


        public BottomSheetBehavior() : base()
        {
            SetBottomSheetCallback(new BottomSheetEventCallback(this));
        }

        public BottomSheetBehavior(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            SetBottomSheetCallback(new BottomSheetEventCallback(this));
        }

        protected BottomSheetBehavior(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            SetBottomSheetCallback(new BottomSheetEventCallback(this));
        }


        public static new BottomSheetBehavior From(Java.Lang.Object view)
        {
            var behavior = global::Android.Support.Design.Widget.BottomSheetBehavior.From(view);
            if (behavior is BottomSheetBehavior behaviour)
                return behaviour;
            else
                throw new System.Exception("The view is not associated with the custom BottomSheetBehavior");
        }


        public override bool OnInterceptTouchEvent(CoordinatorLayout parent, Java.Lang.Object child, MotionEvent ev)
        {
            _nestedScrollingViewTouched &= ev.ActionMasked != MotionEventActions.Cancel;

            return Enabled ? !_nestedScrollingViewTouched && base.OnInterceptTouchEvent(parent, child, ev) : false;
        }

        public override bool OnTouchEvent(CoordinatorLayout parent, Java.Lang.Object child, MotionEvent ev)
        {
            return Enabled ? base.OnTouchEvent(parent, child, ev) : false;
        }

        public override bool OnStartNestedScroll(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View directTargetChild, View target, int axes)
        {
            _nestedScrollingViewTouched = NestedScrollingViewIds.Contains(target.Id);
            return Enabled ? !_nestedScrollingViewTouched && base.OnStartNestedScroll(coordinatorLayout, child, directTargetChild, target, axes) : false;
        }

        public override bool OnStartNestedScroll(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View directTargetChild, View target, int axes, int type)
        {
            _nestedScrollingViewTouched = NestedScrollingViewIds.Contains(target.Id);
            return Enabled ? base.OnStartNestedScroll(coordinatorLayout, child, directTargetChild, target, axes, type) : false;
        }

        public override void OnNestedPreScroll(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View target, int dx, int dy, int[] consumed)
        {
            if (Enabled) base.OnNestedPreScroll(coordinatorLayout, child, target, dx, dy, consumed);
        }

        public override void OnNestedPreScroll(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View target, int dx, int dy, int[] consumed, int type)
        {
            if (Enabled) base.OnNestedPreScroll(coordinatorLayout, child, target, dx, dy, consumed, type);
        }

        public override void OnNestedScroll(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View target, int dxConsumed, int dyConsumed, int dxUnconsumed, int dyUnconsumed)
        {
            if (Enabled) base.OnNestedScroll(coordinatorLayout, child, target, dxConsumed, dyConsumed, dxUnconsumed, dyUnconsumed);
        }

        public override void OnNestedScroll(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View target, int dxConsumed, int dyConsumed, int dxUnconsumed, int dyUnconsumed, int type)
        {
            if (Enabled) base.OnNestedScroll(coordinatorLayout, child, target, dxConsumed, dyConsumed, dxUnconsumed, dyUnconsumed, type);
        }

        public override bool OnNestedPreFling(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View target, float velocityX, float velocityY)
        {
            return Enabled ? base.OnNestedPreFling(coordinatorLayout, child, target, velocityX, velocityY) : false;
        }

        public override bool OnNestedFling(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View target, float velocityX, float velocityY, bool consumed)
        {
            return Enabled ? base.OnNestedFling(coordinatorLayout, child, target, velocityX, velocityY, consumed) : false;
        }



        private class BottomSheetEventCallback : BottomSheetBehavior.BottomSheetCallback
        {

            private readonly BottomSheetBehavior _parent;

            internal BottomSheetEventCallback(BottomSheetBehavior parent)
            {
                _parent = parent;
            }

            public override void OnSlide(View bottomSheet, float slideOffset)
            {
                _parent.Slide?.Invoke(bottomSheet, slideOffset);
            }

            public override void OnStateChanged(View bottomSheet, int newState)
            {
                _parent.StateChanged?.Invoke(bottomSheet, (SheetState)newState);
            }
        }



        public enum SheetState
        {
            Unknown = 0,
            Dragging = 1,
            Settling = 2,
            Expanded = 3,
            Collapsed = 4,
            Hidden = 5
        }


    }
}