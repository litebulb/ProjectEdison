using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Edison.Mobile.Android.Common.Controls;

namespace Edison.Mobile.User.Client.Droid.Holders
{
    public class ChatTxHolder : RecyclerView.ViewHolder
    {

        public CircularProfileView Avatar { get; private set; }
        public AppCompatTextView Title { get; private set; }
        public AppCompatImageView Icon { get; private set; }
        public AppCompatTextView Text { get; private set; }
        public LinearLayout IndicatorHolder { get; private set; }
        public AppCompatImageView IndicatorIcon { get; private set; }
        public AppCompatTextView IndicatorText { get; private set; }

        public ChatTxHolder(View item) : base(item)
        {
            BindViews(item);
            BindEvents();
        }


        private void BindViews(View item)
        {
            Avatar = item.FindViewById<CircularProfileView>(Resource.Id.chat_avatar);
            Icon = item.FindViewById<AppCompatImageView>(Resource.Id.chat_icon);
            Title = item.FindViewById<AppCompatTextView>(Resource.Id.chat_title);
            Text = item.FindViewById<AppCompatTextView>(Resource.Id.chat_text);
            IndicatorHolder = item.FindViewById<LinearLayout>(Resource.Id.chat_indicator_holder);
            IndicatorIcon = item.FindViewById<AppCompatImageView>(Resource.Id.chat_indicator_icon);
            IndicatorText = item.FindViewById<AppCompatTextView>(Resource.Id.chat_indicator_text);
        }

        private void BindEvents()
        {

        }
        internal void UnbindEvents()
        {

        }





        protected override void Dispose(bool disposing)
        {
            UnbindEvents();
            base.Dispose(disposing);
        }



    
    }
}