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
using Android.Support.V4.Content.Res;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Edison.Core.Common.Models;
using Edison.Mobile.Android.Common;
using Edison.Mobile.User.Client.Droid.Holders;
using Java.Lang;

namespace Edison.Mobile.User.Client.Droid.Adapters
{
    public class EventButtonsAdapter : RecyclerView.Adapter
    {

        public event EventHandler<int> ItemClick;

        private readonly Context _context;
        private Color _normalButtonBackgroundColor;
        private Color _selectedIconColor;

        public ObservableRangeCollection<ActionPlanListModel> EventButtons { get; set; } = new ObservableRangeCollection<ActionPlanListModel>();
    
        private int _selectedPosition = -1;
        public int SelectedPosition
        {
            get { return _selectedPosition; }
            set
            {
                if (value != SelectedPosition)
                {
                    PreviousSelectedPosition = _selectedPosition;
                    _selectedPosition = value;
                    if (PreviousSelectedPosition > -1)
                        NotifyItemChanged(PreviousSelectedPosition);
                    if (value > -1)
                        NotifyItemChanged(value);
                }
            }
        }
        public int PreviousSelectedPosition { get; set; } = -1;

        public override int ItemCount => EventButtons.Count;

        public EventButtonsAdapter(Context ctx, ObservableRangeCollection<ActionPlanListModel> buttons)
        {
            _context = ctx;
            _normalButtonBackgroundColor = new Color(ResourcesCompat.GetColor(_context.Resources, Resource.Color.icon_background_grey, null));
            _selectedIconColor = new Color(ResourcesCompat.GetColor(_context.Resources, Resource.Color.white, null));
            EventButtons = buttons;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Inflate the item
            View cell = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.chat_button_cell, parent, false);
            // Create a ViewHolder to find and hold these view references, and register OnClick with the view holder:
            EventButtonViewHolder vh = new EventButtonViewHolder(cell, OnClick);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            EventButtonViewHolder vh = holder as EventButtonViewHolder;
            Tuple<string, Color> buttonSettings = Constants.GetChatMessageButtonSettings(_context, EventButtons[position].Name, EventButtons[position].Icon);
            vh.Button.SetIconResource(GetIconResourceId(buttonSettings.Item1));
            var states = new int[][] { new int[] { global::Android.Resource.Attribute.StateSelected }, new int[] { -global::Android.Resource.Attribute.StateSelected } };
            ColorStateList backgroundCsl = new ColorStateList(states, new int[] { buttonSettings.Item2, _normalButtonBackgroundColor });
            ColorStateList iconCsl = new ColorStateList(states, new int[] {_selectedIconColor, buttonSettings.Item2 });
            vh.Button.BackgroundTint = backgroundCsl;
            vh.Button.IconTint = iconCsl;
            vh.Button.Selected = position == SelectedPosition;
            vh.Button.Tag = EventButtons[position].Name;
            vh.Label.Text = EventButtons[position].Name;
        }

        public override void OnViewRecycled(Java.Lang.Object holder)
        {
            base.OnViewRecycled(holder);
        }


        // Raise an event when the item-click takes place:
        void OnClick(int position)
        {
            SelectedPosition = position;
            ItemClick?.Invoke(this, position);
        }

        private int GetIconResourceId(string iconName)
        {
            var id = _context.GetDrawableId(iconName);
            return id == 0 ? Resource.Drawable.emergency : id;
        }

    }
}