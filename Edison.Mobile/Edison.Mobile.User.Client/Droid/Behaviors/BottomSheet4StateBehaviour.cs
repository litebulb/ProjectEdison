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
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace Edison.Mobile.User.Client.Droid
{
    public class BottomSheet4StateBehaviour : CoordinatorLayout.Behavior
    {
        /**
         * Callback for monitoring events about bottom sheets.
         */
        public abstract class BottomSheetCallback
        {

            /**
             * Called when the bottom sheet changes its state.
             *
             * @param bottomSheet The bottom sheet view.
             * @param newState    The new state. This will be one of {@link #STATE_DRAGGING},
             *                    {@link #STATE_SETTLING}, {@link #STATE_ANCHOR_POINT},
             *                    {@link #STATE_EXPANDED},
             *                    {@link #STATE_COLLAPSED}, or {@link #STATE_HIDDEN}.
             */
            public abstract void OnStateChanged(View bottomSheet, int newState);

            /**
             * Called when the bottom sheet is being dragged.
             *
             * @param bottomSheet The bottom sheet view.
             * @param slideOffset The new offset of this bottom sheet within its range, from 0 to 1
             *                    when it is moving upward, and from 0 to -1 when it moving downward.
             */
            public abstract void OnSlide(View bottomSheet, float slideOffset);
        }



        /**
         * The bottom sheet is dragging.
         */
        public const int StateDragging = 1;

        /**
         * The bottom sheet is settling.
         */
        public const int StateSettling = 2;

        /**
         * The bottom sheet is expanded_half_way.
         */
        public const int StateAnchorPoint = 3;

        /**
         * The bottom sheet is expanded.
         */
        public const int StateExpanded = 4;

        /**
         * The bottom sheet is collapsed.
         */
        public const int StateCollapsed = 5;

        /**
         * The bottom sheet is hidden.
         */
        public const int StateHidden = 6;


        private const float HideThreshold = 0.5f;
        private const float HideFriction = 0.1f;

        private float _minimumVelocity;



        private int _peekHeight;
        public int PeekHeight
        {
            get { return _peekHeight; }
            set
            {
                if (value >= 0)
                {
                    _peekHeight = value;
                    _maxOffset = _parentHeight - value;
                }
            }
        }




        private int _minOffset;
        private int _maxOffset;

        private const int DefaultAnchorPoint = 900;


        public int AnchorPoint { get; set; }

        public bool Hideable { get; set; }
        public bool IsHideable() { return Hideable; }

        public bool Collapsible { get; set; }
        public bool IsCollapsible() { return Collapsible; }





        private int _state = StateAnchorPoint; //StateCollapsed;
        public int State
        {
            get { return _state; }
            set { SetState(value); }
        }

        private int _lastStableState = StateAnchorPoint; //StateCollapsed;

        private ViewDragHelper _viewDragHelper;

        private bool _ignoreEvents;

        private bool _nestedScrolled;

        private int _parentHeight;

        private WeakReference<View> _viewRef;

        private WeakReference<View> _nestedScrollingChildRef;

        private List<BottomSheetCallback> _callback;

        private int _activePointerId;

        private int _initialY;

        private bool _touchingScrollingChild;





        public BottomSheet4StateBehaviour()
        {
            _dragCallback = new ViewDragHelperCallback(this);
        }



        public BottomSheet4StateBehaviour(Context context, IAttributeSet attrs ) : base (context, attrs)
        {
            using (TypedArray a = context.ObtainStyledAttributes(attrs, Resource.Styleable.BottomSheetBehavior_Layout))
            {
                PeekHeight = a.GetDimensionPixelSize(Resource.Styleable.BottomSheetBehavior_Layout_behavior_peekHeight, 0);

                Hideable = a.GetBoolean(Resource.Styleable.BottomSheetBehavior_Layout_behavior_hideable, false);
            }

            /**
             * Getting the anchorPoint...
             */
            AnchorPoint = DefaultAnchorPoint > PeekHeight ? DefaultAnchorPoint : 0;
            Collapsible = true;


            using (TypedArray a1 = context.ObtainStyledAttributes(attrs, Resource.Styleable.CustomBottomSheetBehavior))
            {
                AnchorPoint = (int)a1.GetDimension(Resource.Styleable.CustomBottomSheetBehavior_anchorPoint, DefaultAnchorPoint);
//                _state = a1.GetInt(Resource.Styleable.CustomBottomSheetBehavior_defaultState, StateCollapsed);
                _state = a1.GetInt(Resource.Styleable.CustomBottomSheetBehavior_defaultState, StateAnchorPoint);
            }

            ViewConfiguration configuration = ViewConfiguration.Get(context);
            _minimumVelocity = configuration.ScaledMinimumFlingVelocity;

            _dragCallback = new ViewDragHelperCallback(this);
        }


        public override IParcelable OnSaveInstanceState(CoordinatorLayout parent, Java.Lang.Object child)
        {
            return base.OnSaveInstanceState(parent, child);
        }


        public override void OnRestoreInstanceState(CoordinatorLayout parent, Java.Lang.Object child, IParcelable state)
        {
            SavedState ss = (SavedState)state;
            base.OnRestoreInstanceState(parent, child, ss.SuperState);
            if (ss.State == StateDragging || ss.State == StateSettling)
                _state = StateCollapsed;
            else
                _state = ss.State;
            _lastStableState = _state;
        }



        public override bool OnLayoutChild(CoordinatorLayout parent, Java.Lang.Object cChild, int layoutDirection)
        {
            //           return base.OnLayoutChild(parent, child, layoutDirection);
            var child = cChild.JavaCast<View>();
            // First let the parent lay it out
            if (_state != StateDragging && _state != StateSettling)
            {
                if (parent.FitsSystemWindows && !child.FitsSystemWindows)
                    child.SetFitsSystemWindows(true);
                parent.OnLayoutChild(child, layoutDirection);
            }
            // Offset the bottom sheet
            _parentHeight = parent.Height;
            _minOffset = System.Math.Max(0, _parentHeight - child.Height);
            _maxOffset = System.Math.Max(_parentHeight - _peekHeight, _minOffset);

            /**
             * New behavior
             */
            switch (_state)
            {
                case StateAnchorPoint:
                    ViewCompat.OffsetTopAndBottom(child, AnchorPoint);
                    break;
                case StateExpanded:
                    ViewCompat.OffsetTopAndBottom(child, _minOffset);
                    break;
                case StateHidden:
                    if (Hideable)
                        ViewCompat.OffsetTopAndBottom(child, _parentHeight);
                    break;
                case StateCollapsed:
                    ViewCompat.OffsetTopAndBottom(child, _maxOffset);
                    break;
            };

            if (_viewDragHelper == null)
            {
                _viewDragHelper = ViewDragHelper.Create(parent, _dragCallback);
            }
            _viewRef = new WeakReference<View>(child);
            _nestedScrollingChildRef = new WeakReference<View>(FindScrollingChild(child));
            return true;
        }



        public override bool OnInterceptTouchEvent(CoordinatorLayout parent, Java.Lang.Object cChild, MotionEvent ev)
        {
            var child = cChild.JavaCast<View>();

            if (!child.IsShown)
            {
                _ignoreEvents = true;
                return false;
            }

            var action = ev.ActionMasked;
            if (action == MotionEventActions.Down ) 
                Reset();

            switch (action)
            { 
                case MotionEventActions.Up:

                case MotionEventActions.Cancel:
                    _touchingScrollingChild = false;
                    _activePointerId = MotionEvent.InvalidPointerId;
                    // Reset the ignore flag
                    if (_ignoreEvents) {
                        _ignoreEvents = false;
                        return false;
                    }
                    break;

                case MotionEventActions.Down:
                    _scrollVelocityTracker.Clear();
                    int initialX = (int) ev.GetX();
                    _initialY = (int) ev.GetY();
                    if (_state == StateAnchorPoint)
                    {
                        _activePointerId = ev.GetPointerId(ev.ActionIndex);
                        _touchingScrollingChild = true;
                    }
                    else 
                    {
                        _nestedScrollingChildRef.TryGetTarget(out View scroll);
                        if (scroll != null && parent.IsPointInChildBounds(scroll, initialX, _initialY))
                        {
                            _activePointerId = ev.GetPointerId(ev.ActionIndex);
                            _touchingScrollingChild = true;
                        }
                    }
                    _ignoreEvents = _activePointerId == MotionEvent.InvalidPointerId &&
                                    !parent.IsPointInChildBounds(child, initialX, _initialY);
                    break;

                case MotionEventActions.Move:
                    break;
            }

            if ( ! _ignoreEvents  &&  _viewDragHelper.ShouldInterceptTouchEvent(ev) ) 
                return true;

            // We have to handle cases that the ViewDragHelper does not capture the bottom sheet because
            // it is not the top most view of its parent. This is not necessary when the touch event is
            // happening over the scrolling content as nested scrolling logic handles that case.

            _nestedScrollingChildRef.TryGetTarget(out View scroll1);
            return  action == MotionEventActions.Move && scroll1 != null &&
                    !_ignoreEvents && _state != StateDragging &&
                    !parent.IsPointInChildBounds(scroll1, (int) ev.GetX(), (int) ev.GetY()) &&
                    System.Math.Abs(_initialY - ev.GetY()) > _viewDragHelper.TouchSlop;

        }



        public override bool OnTouchEvent(CoordinatorLayout parent, Java.Lang.Object cChild, MotionEvent ev)
        {
            var child = cChild.JavaCast<View>();

            if (!child.IsShown)
                return false;

            var action = ev.ActionMasked;
            if (_state == StateDragging  &&  action == MotionEventActions.Down )
                return true;

            // Detect scroll direction for ignoring collapsible
            if (_lastStableState == StateAnchorPoint && action == MotionEventActions.Move) {
                if (ev.GetY() > _initialY && !Collapsible)
                {
                    Reset();
                    return false;
                }
            }
  
            if (_viewDragHelper == null)
                _viewDragHelper = ViewDragHelper.Create(parent, _dragCallback);
        
            _viewDragHelper.ProcessTouchEvent(ev);
        
            if ( action == MotionEventActions.Down )
                Reset();

            // The ViewDragHelper tries to capture only the top-most View. We have to explicitly tell it
            // to capture the bottom sheet in case it is not captured and the touch slop is passed.
            if ( action == MotionEventActions.Move  && !_ignoreEvents && System.Math.Abs(_initialY - ev.GetY()) > _viewDragHelper.TouchSlop)
                    _viewDragHelper.CaptureChildView(child, ev.GetPointerId(ev.ActionIndex));


            return !_ignoreEvents;

        }



        public override bool OnStartNestedScroll(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View directTargetChild, View target, int axes, int type)
        {
            _nestedScrolled = false;
            return (axes & ViewCompat.ScrollAxisVertical) != 0;
        }


        private ScrollVelocityTracker _scrollVelocityTracker = new ScrollVelocityTracker();

        private class ScrollVelocityTracker
        {
            private long _previousScrollTime = 0;

            public float ScrollVelocity { get; private set; } = 0;


            public ScrollVelocityTracker() { }

            public void RecordScroll(int dy)
            {
                long now = DateTime.Now.Ticks;  

                if (_previousScrollTime != 0)
                {
                    long elapsed = now - _previousScrollTime;  //mS
                    ScrollVelocity = (float)dy / elapsed * 1000; // pixels per sec
                }

                _previousScrollTime = now;
            }

            public void Clear()
            {
                _previousScrollTime = 0;
                ScrollVelocity = 0;
            }


        }





        public override void OnNestedPreScroll(CoordinatorLayout coordinatorLayout, Java.Lang.Object cChild, View target, int dx, int dy, int[] consumed, int type)
        {
            var child = cChild.JavaCast<View>();

            _nestedScrollingChildRef.TryGetTarget(out View scrollingChild);
            if (target != scrollingChild)
                return;

            _scrollVelocityTracker.RecordScroll(dy);

            int currentTop = child.Top;
            int newTop = currentTop - dy;

            // Force stop at the anchor - do not go from collapsed to expanded in one scroll
            if ((_lastStableState == StateCollapsed && newTop < AnchorPoint) || 
                      (_lastStableState == StateExpanded && newTop > AnchorPoint))
            {
                consumed[1] = dy;
                ViewCompat.OffsetTopAndBottom(child, AnchorPoint - currentTop);
                DispatchOnSlide(child.Top);
                _nestedScrolled = true;
                return;
            }

            if (dy > 0)
            { // Upward
                if (newTop < _minOffset)
                {
                    consumed[1] = currentTop - _minOffset;
                    ViewCompat.OffsetTopAndBottom(child, -consumed[1]);
                    SetStateInternal(StateExpanded);
                }
                else
                {
                    consumed[1] = dy;
                    ViewCompat.OffsetTopAndBottom(child, -dy);
                    SetStateInternal(StateDragging);
                }
            }
            else if (dy < 0)
            { // Downward
                if (!ViewCompat.CanScrollVertically(target, -1))
                {
                    if (newTop <= _maxOffset || Hideable)
                    {
                        // Restrict STATE_COLLAPSED if restrictedState is set
                        if (Collapsible == true || (Collapsible == false && (AnchorPoint - newTop) >= 0))
                        {
                            consumed[1] = dy;
                            ViewCompat.OffsetTopAndBottom(child, -dy);
                            SetStateInternal(StateDragging);
                        }
                    }
                    else
                    {
                        consumed[1] = currentTop - _maxOffset;
                        ViewCompat.OffsetTopAndBottom(child, -consumed[1]);
                        SetStateInternal(StateCollapsed);
                    }
                }
            }
            DispatchOnSlide(child.Top);
            _nestedScrolled = true;
        }


        public override void OnStopNestedScroll(CoordinatorLayout coordinatorLayout, Java.Lang.Object cChild, View target, int type)
        {
            var child = cChild.JavaCast<View>();

            if (child.Top == _minOffset)
            {
                SetStateInternal(StateExpanded);
                _lastStableState = StateExpanded;
                return;
            }

            _nestedScrollingChildRef.TryGetTarget(out View nestedChildRedTarget);
            if (target != nestedChildRedTarget || !_nestedScrolled)
                return;

            int top;
            int targetState;

            // Are we flinging up?
            float scrollVelocity = _scrollVelocityTracker.ScrollVelocity;
            if (scrollVelocity > _minimumVelocity)
            {
                if (_lastStableState == StateCollapsed)
                {
                    // Fling from collapsed to anchor
                    top = AnchorPoint;
                    targetState = StateAnchorPoint;
                }
                else if (_lastStableState == StateAnchorPoint)
                {
                    // Fling from anchor to expanded
                    top = _minOffset;
                    targetState = StateExpanded;
                }
                else
                {
                    // We are already expanded
                    top = _minOffset;
                    targetState = StateExpanded;
                }
            }
            else
                // Are we flinging down?
                if (scrollVelocity < -_minimumVelocity)
            {
                if (_lastStableState == StateExpanded)
                {
                    // Fling to from expanded to anchor
                    top = AnchorPoint;
                    targetState = StateAnchorPoint;
                }
                else if (Collapsible == true)
                {
                    if (_lastStableState == StateAnchorPoint)
                    {
                        // Fling from anchor to collapsed
                        top = _maxOffset;
                        targetState = StateCollapsed;
                    }
                    else
                    {
                        // We are already collapsed
                        top = _maxOffset;
                        targetState = StateCollapsed;
                    }
                }
                else
                {
                    top = AnchorPoint;
                    targetState = StateAnchorPoint;
                }
            }
            // Not flinging, just settle to the nearest state
            else
            {
                // Collapse?
                int currentTop = child.Top;
                if (currentTop > AnchorPoint * 1.25 && Collapsible == true)
                { // Multiply by 1.25 to account for parallax. The currentTop needs to be pulled down 50% of the anchor point before collapsing.
                    top = _maxOffset;
                    targetState = StateCollapsed;
                }
                // Expand?
                else
                if (currentTop < AnchorPoint * 0.5)
                {
                    top = _minOffset;
                    targetState = StateExpanded;
                }
                // Snap back to the anchor
                else
                {
                    top = AnchorPoint;
                    targetState = StateAnchorPoint;
                }
            }

            _lastStableState = targetState;
            if (_viewDragHelper.SmoothSlideViewTo(child, child.Left, top))
            {
                SetStateInternal(StateSettling);
                ViewCompat.PostOnAnimation(child, new SettleRunnable(this, child, targetState));
            }
            else
            {
                SetStateInternal(targetState);
            }
            _nestedScrolled = false;





        }


        public override bool OnNestedPreFling(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View target, float velocityX, float velocityY)
        {
            _nestedScrollingChildRef.TryGetTarget(out View nestedChildRedTarget);
            return target == nestedChildRedTarget && (_state != StateExpanded || base.OnNestedPreFling(coordinatorLayout, child, target, velocityX, velocityY));
        }


        /**
        * Adds a callback to be notified of bottom sheet events.
        *
        * @param callback The callback to notify when bottom sheet events occur.
        */
        public void AddBottomSheetCallback(BottomSheetCallback callback)
        {
            if (_callback == null)
                _callback = new List<BottomSheetCallback>();

            _callback.Add(callback);
        }



        /**
         * Sets the state of the bottom sheet. The bottom sheet will transition to that state with animation.
         *
         * @param state One of {@link #STATE_COLLAPSED}, {@link #STATE_ANCHOR_POINT}, {@link #STATE_EXPANDED} or {@link #STATE_HIDDEN}.
         */
        public void SetState(int state)
        {
            if (state == _state)
                return;

            /**
             * New behavior (added: state == STATE_ANCHOR_POINT ||)
             */
            if (state == StateCollapsed || state == StateExpanded || state == StateAnchorPoint || (Hideable && state == StateHidden))
            {
                _state = state;
                _lastStableState = state;
            }

            View child = null;
            if (_viewRef == null)
                _viewRef.TryGetTarget(out  child);
            if (child == null)
                    return;

            int top;
            if (state == StateCollapsed)
                top = _maxOffset;
            else if (state == StateAnchorPoint)
                top = AnchorPoint;
            else if (state == StateExpanded)
                top = _minOffset;
            else if (Hideable && state == StateHidden)
                top = _parentHeight;
            else
                throw new IllegalArgumentException("Illegal state argument: " + state);

            SetStateInternal(StateSettling);
            if (_viewDragHelper.SmoothSlideViewTo(child, child.Left, top))
            {
                ViewCompat.PostOnAnimation(child, new SettleRunnable(this, child, state));
            }
        }


        private void SetStateInternal(int state)
        {
            if (_state != state)
            {
                _state = state;
                _viewRef.TryGetTarget(out View bottomSheet);
                if (bottomSheet != null && _callback != null)
                {
                    //            mCallback.onStateChanged(bottomSheet, state);
                    NotifyStateChangedToListeners(bottomSheet, state);
                }
            }
        }

        private void NotifyStateChangedToListeners(View bottomSheet, int newState)
        {
            foreach (BottomSheetCallback bottomSheetCallback in _callback)
            {
                bottomSheetCallback.OnStateChanged(bottomSheet, newState);
            }
        }

        private void NotifyOnSlideToListeners(View bottomSheet, float slideOffset)
        {
            foreach (BottomSheetCallback bottomSheetCallback in _callback)
            {
                bottomSheetCallback.OnSlide(bottomSheet, slideOffset);
            }
        }



        private void Reset()
        {
            _activePointerId = ViewDragHelper.InvalidPointer;
        }

        private bool ShouldHide(View child, float yvel)
        {
            if (child.Top < _maxOffset)
                // It should not hide, but collapse.
                return false;
            float newTop = child.Top + yvel * HideFriction;
            return System.Math.Abs(newTop - _maxOffset) / (float)_peekHeight > HideThreshold;
        }

        private View FindScrollingChild(View view)
        {
            if (view is INestedScrollingChild)
                return view;

            if (view is ViewGroup group)
            {
                for (int i = 0, count = group.ChildCount; i < count; i++)
                {
                    View scrollingChild = FindScrollingChild(group.GetChildAt(i));
                    if (scrollingChild != null)
                        return scrollingChild;
                }
            }
            return null;
        }



        private ViewDragHelperCallback _dragCallback;

        public class ViewDragHelperCallback: ViewDragHelper.Callback
        {
            BottomSheet4StateBehaviour _behavior;

            public ViewDragHelperCallback(BottomSheet4StateBehaviour behavior) :base()
            {
                _behavior = behavior;
            }

            public override bool TryCaptureView(View child, int pointerId)
            {
                if (_behavior._state == StateDragging)
                    return false;

                if (_behavior._touchingScrollingChild)
                    return false;

                if (_behavior._state == StateExpanded && _behavior._activePointerId == pointerId)
                {
                    _behavior._nestedScrollingChildRef.TryGetTarget(out View scroll);
                    if (scroll != null && scroll.CanScrollVertically(-1))
                        // Let the content scroll up
                        return false;
                }
                if (_behavior._viewRef != null)
                {
                    _behavior._viewRef.TryGetTarget(out View view);
                    return view == child;
                }
                return false;
            }

            public override void OnViewPositionChanged(View changedView, int left, int top, int dx, int dy)
            {
                _behavior.DispatchOnSlide(top);
            }


            public override void OnViewDragStateChanged(int state)
            {
                if (state == ViewDragHelper.StateDragging)
                    _behavior.SetStateInternal(StateDragging);
            }


            public override void OnViewReleased(View releasedChild, float xvel, float yvel)
            {
                int top;
                int targetState;
                if (yvel < 0)
                { // Moving up
                    top = _behavior._minOffset;
                    targetState = StateExpanded;
                }
                else
                if (_behavior.Hideable && _behavior.ShouldHide(releasedChild, yvel))
                {
                    top = _behavior._parentHeight;
                    targetState = StateHidden;
                }
                else
                if (yvel == 0)
                {
                    int currentTop = releasedChild.Top;
                    if (System.Math.Abs(currentTop - _behavior._minOffset) < System.Math.Abs(currentTop - _behavior._maxOffset))
                    {
                        top = _behavior._minOffset;
                        targetState = StateExpanded;
                    }
                    else
                    {
                        top = _behavior._maxOffset;
                        targetState = StateCollapsed;
                    }
                }
                else
                {
                    top = _behavior._maxOffset;
                    targetState = StateCollapsed;
                }

                // Restrict Collapsed view (optional)
                if (!_behavior.Collapsible && targetState == StateCollapsed)
                {
                    top = _behavior.AnchorPoint;
                    targetState = StateAnchorPoint;
                }

                if (_behavior._viewDragHelper.SettleCapturedViewAt(releasedChild.Left, top))
                {
                    _behavior.SetStateInternal(StateSettling);
                    ViewCompat.PostOnAnimation(releasedChild, new SettleRunnable(_behavior, releasedChild, targetState));
                }
                else
                    _behavior.SetStateInternal(targetState);
            }


            public override int ClampViewPositionVertical(View child, int top, int dy)
            {
                return Constrain(top, _behavior._minOffset, _behavior.Hideable ? _behavior._parentHeight : _behavior._maxOffset);
            }

            private int Constrain(int amount, int low, int high)
            {
                return amount < low ? low : (amount > high ? high : amount);
            }

            public override int ClampViewPositionHorizontal(View child, int left, int dx)
            {
                return child.Left;
            }

            public override int GetViewVerticalDragRange(View child)
            {
                if (_behavior.Hideable)
                    return _behavior._parentHeight - _behavior._minOffset;
                else
                    return _behavior._maxOffset - _behavior._minOffset;
            }

            public override void OnEdgeTouched(int edgeFlags, int pointerId)
            {
                base.OnEdgeTouched(edgeFlags, pointerId);
            }
            public override void OnEdgeDragStarted(int edgeFlags, int pointerId)
            {
                base.OnEdgeDragStarted(edgeFlags, pointerId);
            }
            public override bool OnEdgeLock(int edgeFlags)
            {
                return base.OnEdgeLock(edgeFlags);
            }
            public override void OnViewCaptured(View capturedChild, int activePointerId)
            {
                base.OnViewCaptured(capturedChild, activePointerId);
            }

        }




        private void DispatchOnSlide(int top)
        {
            _viewRef.TryGetTarget(out View bottomSheet);
            if (bottomSheet != null && _callback != null)
            {
                if (top > _maxOffset)
                    NotifyOnSlideToListeners(bottomSheet, (float)(_maxOffset - top) / _peekHeight);
                else
                    NotifyOnSlideToListeners(bottomSheet, (float)(_maxOffset - top) / ((_maxOffset - _minOffset)));
            }
        }


        public class SettleRunnable : Java.Lang.Object, IRunnable, IJavaObject, IDisposable
        {
            private View _view;
            private int _targetState;
            private BottomSheet4StateBehaviour _behavior;
            
            public SettleRunnable(BottomSheet4StateBehaviour behavior, View view, int targetState)
            {
                _behavior = behavior;
                _view = view;
                _targetState = targetState;
            }


            public void Run()
            {
                if (_behavior._viewDragHelper != null && _behavior._viewDragHelper.ContinueSettling(true))
                    ViewCompat.PostOnAnimation(_view, this);
                else
                    _behavior.SetStateInternal(_targetState);
            }
        }




        protected class SavedState : View.BaseSavedState
        {
            public int State { get; private set; }

            public SavedState(Parcel source) : base (source)
            {

                // noinspection ResourceType
                State = source.ReadInt();
            }

            public SavedState(IParcelable superState, int state) : base(superState)
            { 
                State = state;
            }


            public override void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
            {
                base.WriteToParcel(dest, flags);
                dest.WriteInt(State);

                
            }

            public readonly IParcelableCreator Creator = new SavedStateParcableCreator();



            public class SavedStateParcableCreator : Java.Lang.Object, IParcelableCreator
            {

                private readonly Func<Parcel, SavedState> _createFunc;

                public SavedStateParcableCreator()
                {

                }

                public SavedStateParcableCreator(Func<Parcel,  SavedState> createFromParcelFunc)
                {
                    _createFunc = createFromParcelFunc;
                }



                /// <summary>
                /// Create a parcelable from a parcel.
                /// </summary>
                public Java.Lang.Object CreateFromParcel(Parcel parcel)
                {
                    return new SavedState(parcel);
                }

                /// <summary>
                /// Create an array from the parcelable class.
                /// </summary>
                public Java.Lang.Object[] NewArray(int size)
                {
                    return new SavedState[size];
                }
            }

        };


        public static BottomSheet4StateBehaviour From(View view)
        {
            ViewGroup.LayoutParams lp = view.LayoutParameters;
            if (lp is CoordinatorLayout.LayoutParams clp)
            {
                CoordinatorLayout.Behavior behavior = clp.Behavior;
                if (behavior is BottomSheet4StateBehaviour bsb)
                {
                    return bsb;
                }
                else
                    throw new IllegalArgumentException("The view is not associated with BottomSheet4StateBehaviour");
            }
            else
                throw new IllegalArgumentException("The view is not a child of CoordinatorLayout");
        }




        public override WindowInsetsCompat OnApplyWindowInsets(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, WindowInsetsCompat insets)
        {
            return base.OnApplyWindowInsets(coordinatorLayout, child, insets);
        }

        public override void OnAttachedToLayoutParams(CoordinatorLayout.LayoutParams @params)
        {
            base.OnAttachedToLayoutParams(@params);
        }

        public override bool OnDependentViewChanged(CoordinatorLayout parent, Java.Lang.Object child, View dependency)
        {
            return base.OnDependentViewChanged(parent, child, dependency);
        }

        public override void OnDependentViewRemoved(CoordinatorLayout parent, Java.Lang.Object child, View dependency)
        {
            base.OnDependentViewRemoved(parent, child, dependency);
        }

        public override void OnDetachedFromLayoutParams()
        {
            base.OnDetachedFromLayoutParams();
        }

        public override bool OnMeasureChild(CoordinatorLayout parent, View child, int parentWidthMeasureSpec, int widthUsed, int parentHeightMeasureSpec, int heightUsed)
        {
            return base.OnMeasureChild(parent, child, parentWidthMeasureSpec, widthUsed, parentHeightMeasureSpec, heightUsed);
        }

        public override bool OnNestedFling(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View target, float velocityX, float velocityY, bool consumed)
        {
            return base.OnNestedFling(coordinatorLayout, child, target, velocityX, velocityY, consumed);
        }

        public override void OnNestedPreScroll(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View target, int dx, int dy, int[] consumed)
        {
            base.OnNestedPreScroll(coordinatorLayout, child, target, dx, dy, consumed);
        }

        public override void OnNestedScroll(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View target, int dxConsumed, int dyConsumed, int dxUnconsumed, int dyUnconsumed)
        {
            base.OnNestedScroll(coordinatorLayout, child, target, dxConsumed, dyConsumed, dxUnconsumed, dyUnconsumed);
        }

        public override void OnNestedScroll(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View target, int dxConsumed, int dyConsumed, int dxUnconsumed, int dyUnconsumed, int type)
        {
            base.OnNestedScroll(coordinatorLayout, child, target, dxConsumed, dyConsumed, dxUnconsumed, dyUnconsumed, type);
        }

        public override void OnNestedScrollAccepted(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View directTargetChild, View target, int axes)
        {
            base.OnNestedScrollAccepted(coordinatorLayout, child, directTargetChild, target, axes);
        }

        public override void OnNestedScrollAccepted(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View directTargetChild, View target, int axes, int type)
        {
            base.OnNestedScrollAccepted(coordinatorLayout, child, directTargetChild, target, axes, type);
        }


        public override bool OnRequestChildRectangleOnScreen(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, Rect rectangle, bool immediate)
        {
            return base.OnRequestChildRectangleOnScreen(coordinatorLayout, child, rectangle, immediate);
        }

        public override bool OnStartNestedScroll(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View directTargetChild, View target, int axes)
        {
            return base.OnStartNestedScroll(coordinatorLayout, child, directTargetChild, target, axes);
        }

        public override void OnStopNestedScroll(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View target)
        {
            base.OnStopNestedScroll(coordinatorLayout, child, target);
        }
        public override bool LayoutDependsOn(CoordinatorLayout parent, Java.Lang.Object child, View dependency)
        {
            return base.LayoutDependsOn(parent, child, dependency);
        }



    }




}