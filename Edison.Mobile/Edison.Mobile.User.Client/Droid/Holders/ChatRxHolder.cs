using Android.Support.V7.Widget;
using Android.Views;
using Edison.Mobile.Android.Common.Controls;

namespace Edison.Mobile.User.Client.Droid.Holders
{
    public class ChatRxHolder : RecyclerView.ViewHolder
    {

        public CircularProfileView Avatar { get; private set; }
        public AppCompatTextView Text { get; private set; }

        public ChatRxHolder(View item) : base(item)
        {
            BindViews(item);
            BindEvents();
        }


        private void BindViews(View item)
        {
            Avatar = item.FindViewById<CircularProfileView>(Resource.Id.chat_avatar);
            Text = item.FindViewById<AppCompatTextView>(Resource.Id.chat_text);
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