using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Edison.Mobile.User.Client.Droid.Holders;
using Edison.Mobile.User.Client.Core.CollectionItemViewModels;
using Edison.Mobile.User.Client.Core.ViewModels;
using Android.Runtime;
using Android.Support.V7.CardView;

namespace Edison.Mobile.User.Client.Droid.Adapters
{
    public class ResponsesAdapter : RecyclerView.Adapter   
    {
        private Context context;
        readonly ObservableRangeCollection<ResponseCollectionItemViewModel> responses;

        public event EventHandler<int> OnResponseSelected;

        public ResponsesAdapter(Context context)
        {
            this.context = context;
            //this.responses = responses;
        }

        public override int ItemCount => 5; //responses.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) {}

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View responseCard = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.response_card, parent, false);
            return new ResponseViewHolder(responseCard);
        }
    }

}
