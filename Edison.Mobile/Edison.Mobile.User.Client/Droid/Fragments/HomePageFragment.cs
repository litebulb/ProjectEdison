using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Edison.Mobile.Android.Common;
using Edison.Mobile.Android.Common.Controls;
using Edison.Mobile.User.Client.Core.ViewModels;
using Edison.Mobile.User.Client.Droid.Activities;
using System;
using System.Collections.Specialized;

namespace Edison.Mobile.User.Client.Droid.Fragments
{
    public class HomePageFragment : BaseFragment<ResponsesViewModel>
    {

        private CircularEventGauge _eventGauge;


        public event EventHandler OnViewResponseDetails;  // not sure if we need this
        public event EventHandler OnDismissResponseDetails;// not sure if we need this


        public bool IsShowingDetails { get; private set; }// not sure if we need this



        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var root = inflater.Inflate(Resource.Layout.page_home, container, false);
            BindViews(root);
            AdjustSizes();


            // Set the Title
            if (Activity is MainActivity act)
                act.SetToolbarCustomTitle(act.Resources.GetString(Resource.String.right_now));


            // get event/reponse data
            _eventGauge.EventCount = ViewModel.Responses.Count + 1;









            return root;
        }


        private void BindViews(View root)
        {
            _eventGauge = root.FindViewById<CircularEventGauge>(Resource.Id.event_gauge);
            _eventGauge.Indicator.Clickable = true;

        }

        protected override void BindEventHandlers()
        {
            base.BindEventHandlers();

            ViewModel.Responses.CollectionChanged += OnResponsesChanged;
            ViewModel.OnCurrentAlertCircleColorChanged += OnCurrentEventCircleColorChanged;
           
            _eventGauge.Click += OnEventGaugeClicked;

        }

        protected override void UnBindEventHandlers()
        {
            base.UnBindEventHandlers();

            ViewModel.Responses.CollectionChanged -= OnResponsesChanged;
            ViewModel.OnCurrentAlertCircleColorChanged -= OnCurrentEventCircleColorChanged;
           
            _eventGauge.Click -= OnEventGaugeClicked;

        }


        private void AdjustSizes()
        {
            _eventGauge.LayoutParameters.Width= Constants.EventGaugeSizePx;
            _eventGauge.LayoutParameters.Height = Constants.EventGaugeSizePx;
            _eventGauge.Invalidate();
        }












        private void OnBrightnessClicked(object sender, EventArgs e)
        {

        }

        void OnBrightnessSliderValueChanged(object sender, EventArgs e)
        {

        }

        void OnEventsReceived()
        {
            if (ViewModel.Responses == null) return;

            ViewModel.Responses.CollectionChanged -= OnResponsesChanged;  // Why???? - ask Alex
            ViewModel.Responses.CollectionChanged += OnResponsesChanged;
        }



        private void OnResponsesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var haveResponses = ViewModel.Responses.Count > 0;
            _eventGauge.EventCount = ViewModel.Responses.Count;

  //         _eventsList.ReloadData();
 
        }

        private void OnCurrentEventCircleColorChanged(object sender, string newColor)
        {
            var color = Constants.GetEventTypeColor(Activity, newColor);
            _eventGauge.RingColors = new ShadingColorPair(Color.White, color);
            _eventGauge.SetCenterTint(color);
        }


        private void OnEventSelected(object sender, int index)
        {
            IsShowingDetails = true;

            OnViewResponseDetails?.Invoke(this, new EventArgs());

            var response = ViewModel.Responses[index];

            // Navigate to details

        }


        private void OnEventGaugeClicked(object sender, EventArgs e)
        {

        }



        }
}