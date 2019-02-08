using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Edison.Core.Common.Models;
using Edison.Mobile.Android.Common;
using Edison.Mobile.User.Client.Core.Chat;
using Edison.Mobile.User.Client.Droid.Holders;

namespace Edison.Mobile.User.Client.Droid.Adapters
{
    class ChatAdapter : RecyclerView.Adapter
    {

        private const int Tx = 0;
        private const int Rx = 1;
        private readonly Context _context;


        public ObservableRangeCollection<ChatMessage> Messages { get; private set; } = new ObservableRangeCollection<ChatMessage>();

        public string Initials { get; set; } = null;
        public Uri ProfileImageUri { get; set; } = null;
        public string Email { get; set; } = null;


        public override int ItemCount => Messages.Count;

        public ChatAdapter(Context ctx, ObservableRangeCollection<ChatMessage> messages)
        {
            _context = ctx;
            Messages = messages;
        }

        public override int GetItemViewType(int position)
        {
            return Messages[position].UserModel.Role == ChatUserRole.Consumer ? Tx : Rx;
        }


        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            RecyclerView.ViewHolder vh = null;
            View cell = null;

            switch (viewType)
            {
                case Tx:
                    // Inflate the item
                    cell = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.chat_cell_tx, parent, false);
                    // Create a ViewHolder to find and hold these view references, and register OnClick with the view holder:
                    vh = new ChatTxHolder(cell);
                    break;

                case Rx:
                    // Inflate the item
                    cell = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.chat_cell_rx, parent, false);
                    // Create a ViewHolder to find and hold these view references, and register OnClick with the view holder:
                    vh = new ChatRxHolder(cell);
                    break;
            }
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            switch (holder.ItemViewType)
            {
                case Tx:
                    BindTxHolder(holder as ChatTxHolder, position);
                    break;

                case Rx:
                    BindRxHolder(holder as ChatRxHolder, position);
                    break;
            }
        }

        private void BindTxHolder(ChatTxHolder vh, int position)
        {
            var message = Messages[position];
            if (vh != null && message != null)
            {

                // Set up the Avatar
#if DEBUG
                // for Testing

                if (ProfileImageUri != null)
                {
                    // TODOD
                    //get image resource and set

                }
                else if (Email == null || Email.Contains("bluemetal.com"))
                    vh.Avatar.SetProfileResource(Resource.Drawable.greyhound_kimmie);
                else if (!string.IsNullOrWhiteSpace(Initials))
                    vh.Avatar.SetText(Initials);
                else
                    vh.Avatar.SetInitials("ALISON SUMMERFIELD");
#else
                if (ProfileImageUri != null)
                {
                    // TODOD
                    //get image resource and set

                }
                else
                    vh.Avatar.SetText(Initials);
#endif

                // Set up the Icon and Title
                var isNewActionPlan = message.IsNewActionPlan && message.ActionPlan != null;
                if (isNewActionPlan)
                {
                    Tuple<string, Color> messageSettings = Constants.GetChatMessageButtonSettings(_context, message.ActionPlan.Name, message.ActionPlan.Icon);
                    vh.Icon.SetImageResource(GetIconResourceId(messageSettings.Item1));
                    vh.Icon.BackgroundTintList = ColorStateList.ValueOf(messageSettings.Item2);
                    vh.Title.Text = message.ActionPlan.Name;
                    vh.Icon.Visibility = ViewStates.Visible;
                    vh.Title.Visibility = ViewStates.Visible;
                    vh.IndicatorHolder.Visibility = ViewStates.Visible;
                }
                else
                {
                    vh.Icon.Visibility = ViewStates.Gone;
                    vh.Title.Visibility = ViewStates.Gone;
                    vh.IndicatorHolder.Visibility = ViewStates.Gone;
                }

                // Set up the Message Text
                vh.Text.Text = message.Text;
            }
        }

        private void BindRxHolder(ChatRxHolder vh, int position)
        {
            if (vh != null)
                vh.Text.Text = Messages[position]?.Text;
        }



        public override void OnViewRecycled(Java.Lang.Object holder)
        {
            base.OnViewRecycled(holder);
        }




        private int GetIconResourceId(string iconName)
        {
            var id = _context.GetDrawableId(iconName);
            return id == 0 ? Resource.Drawable.emergency : id;
        }

    }
}