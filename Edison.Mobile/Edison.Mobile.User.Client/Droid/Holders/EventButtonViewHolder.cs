using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Edison.Mobile.Android.Common.Controls;

namespace Edison.Mobile.User.Client.Droid.Holders
{
    public class EventButtonViewHolder : RecyclerView.ViewHolder
    {
        private Action<int> _listener;

        public CircularImageButton Button { get; set; }
        public AppCompatTextView Label { get; set; }


        public EventButtonViewHolder(View item, Action<int> listener) : base(item)
        {
            _listener = listener;
            BindViews(item);
            BindEvents();
        }


        private void BindViews(View item)
        {
            Button = item.FindViewById<CircularImageButton>(Resource.Id.button);
            Label = item.FindViewById<AppCompatTextView>(Resource.Id.label);
        }

        private void BindEvents()
        {
            Button.Click -= OnButtonClick;  // unregister first, just in case
            Button.Click += OnButtonClick;
        }
        internal void UnbindEvents()
        {
            Button.Click -= OnButtonClick;
        }


        private void OnButtonClick(object s, EventArgs e)
        {
            _listener(base.LayoutPosition);
        }


        protected override void Dispose(bool disposing)
        {
            UnbindEvents();
            _listener = null;
            base.Dispose(disposing);
        }

    }
}