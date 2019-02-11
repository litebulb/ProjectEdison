using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace Edison.Mobile.Android.Common.Controls
{
    public class SpaceItemDecoration : RecyclerView.ItemDecoration
    {
        private readonly int _size;
        private readonly Orientation _orientation;
        private readonly bool _endSpace = false;

        public SpaceItemDecoration (int sizePx, Orientation orientation = Orientation.Vertical, bool endSpace = false)
        {
            _size = sizePx;
            _orientation = orientation;
            _endSpace = endSpace;
        }

        public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
        {
            if (!_endSpace || (_endSpace && parent.GetChildAdapterPosition(view) != parent.GetAdapter().ItemCount - 1))
            {
                if (_orientation == Orientation.Vertical)
                    outRect.Bottom = _size;
                else
                    outRect.Right = _size;
            }
        }
    }


}