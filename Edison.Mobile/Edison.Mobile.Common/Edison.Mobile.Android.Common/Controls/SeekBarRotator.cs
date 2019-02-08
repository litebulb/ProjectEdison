using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using System;

namespace Edison.Mobile.Android.Common.Controls
{
    /// <summary>
    /// Port of the SeekBarRotator found here: https://android.googlesource.com/platform/packages/apps/MusicFX/+/master/src/com/android/musicfx?autodive=0%2F%2F%2F%2F%2F
    /// The following is the Copyright statement that has to be included with the file
    /// 
/*
 * Copyright (C) 2015 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
    /// </summary>



    public class SeekBarRotator : ViewGroup
    {


        public SeekBarRotator(Context context) : base(context) { }

        public SeekBarRotator(Context context, IAttributeSet attrs) : base(context, attrs) { }

        public SeekBarRotator(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr) { }

        public SeekBarRotator(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes) { }

        protected SeekBarRotator(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }


        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            View child = GetChildAt(0);
            if (child.Visibility == ViewStates.Gone)
                SetMeasuredDimension(ResolveSizeAndState(0, widthMeasureSpec, 0), ResolveSizeAndState(0, heightMeasureSpec, 0));
            else
            {
                // swap width and height for child
                MeasureChild(child, heightMeasureSpec, widthMeasureSpec);
                SetMeasuredDimension(child.MeasuredHeightAndState, child.MeasuredWidthAndState);
            }
        }

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            View child = GetChildAt(0);
            if (child.Visibility != ViewStates.Gone)
            {
                // rotate the child 90 degrees counterclockwise around its upper-left
                child.PivotX = 0;
                child.PivotY = 0;
                child.Rotation = -90;
                // place the child below this view, so it rotates into view
                //int mywidth = right - left;
                //int myheight = bottom - top;
                int childwidth = bottom - top;
                int childheight = right - left;
                child.Layout(0, childwidth, childwidth, childwidth + childheight);
            }
        }
    }
}