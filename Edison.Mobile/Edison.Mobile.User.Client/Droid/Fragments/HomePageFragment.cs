using System;
using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;
using Android.Content;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Edison.Core.Common.Models;
using Edison.Mobile.Android.Common;
using Edison.Mobile.Android.Common.Controls;
using Edison.Mobile.Common.Geo;
using Edison.Mobile.User.Client.Core.ViewModels;
using Edison.Mobile.User.Client.Droid.Activities;
using Edison.Mobile.User.Client.Droid.Adapters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Edison.Mobile.User.Client.Droid.Fragments
{
    public class HomePageFragment : BaseFragment<ResponsesViewModel>  // use MainViewModel as a dummy ViewMiodel
    {

        public event EventHandler OnViewResponseDetails;  // not sure if we need this
        public event EventHandler OnDismissResponseDetails;// not sure if we need this

        private static bool _isInitialAppearance = true;  // Not sure we need this

        private static Color _currentResponseColor = Constants.DefaultResponseColor;

        private CircularEventGauge _eventGauge;

        private RecyclerView _responsesCarousel;
        private RecyclerView.LayoutManager _layoutManager;
        private ResponsesAdapter _responsesAdapter;


        public bool IsShowingDetails { get; private set; }// not sure if we need this



        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            if (savedInstanceState != null)
            {
                int col = savedInstanceState.GetInt(Constants.CurrentResponseColorKey, -1);
                if (col != -1)
                    _currentResponseColor = new Color(col);
            }
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            // Save the current color
            outState.PutInt(Constants.CurrentResponseColorKey, _currentResponseColor);
            base.OnSaveInstanceState(outState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var root = inflater.Inflate(Resource.Layout.page_home, container, false);
            BindViews(root);
            SetupCarousel();
            BindEvents();
            AdjustSizes();

            // Set the Title
            if (Activity is MainActivity act)
                act.SetToolbarCustomTitle(act.Resources.GetString(Resource.String.right_now));

            // Set the initial event count and event gauge color
            _eventGauge.EventCount = ViewModel.Responses.Count;
            if (ViewModel.Responses.Count > 0)
                // restore the current event gauge color
                SetEventGaugeColor(new ShadingColorPair(Color.White, _currentResponseColor));

            return root;
        }


        public override void OnStart()
        {
            base.OnStart();
            _responsesAdapter?.PropogateActivityState(ActivityState.Started);
            // Not sure we need any of this
            if (_isInitialAppearance)
            {
                IsShowingDetails = false;
                OnDismissResponseDetails?.Invoke(this, new EventArgs());
                _isInitialAppearance = false;
            }

        }

        public override void OnDestroyView()
        {
            UnBindEvents();
            base.OnDestroyView();
        }


        private void BindViews(View root)
        {
            _eventGauge = root.FindViewById<CircularEventGauge>(Resource.Id.event_gauge);
            _eventGauge.Indicator.Clickable = true;

            _responsesCarousel = root.FindViewById<RecyclerView>(Resource.Id.event_carousel);
        }


        private void SetupCarousel()
        {
            _responsesCarousel.HasFixedSize = true;
            _layoutManager = new LinearLayoutManager(Activity, LinearLayoutManager.Horizontal, false);
            _responsesCarousel.SetLayoutManager(_layoutManager);
            _responsesAdapter = new ResponsesAdapter(Activity, ViewModel.Responses, ViewModel.UserLocation);
            _responsesCarousel.SetAdapter(_responsesAdapter);
        }


        protected void BindEvents()
        {
            ViewModel.Responses.CollectionChanged += OnResponsesChanged;
            ViewModel.ResponseUpdated += OnResponseUpdated;
            ViewModel.OnCurrentAlertCircleColorChanged += OnCurrentEventCircleColorChanged;
            ViewModel.LocationChanged += OnLocationChanged;
            _eventGauge.Click += OnEventGaugeClicked;
            _responsesAdapter.ItemClick += OnResponseSelected;
        }

        protected void UnBindEvents()
        {
            ViewModel.Responses.CollectionChanged -= OnResponsesChanged;
            ViewModel.ResponseUpdated -= OnResponseUpdated;
            ViewModel.OnCurrentAlertCircleColorChanged -= OnCurrentEventCircleColorChanged;
            ViewModel.LocationChanged -= OnLocationChanged;
            _eventGauge.Click -= OnEventGaugeClicked;
            _responsesAdapter.ItemClick -= OnResponseSelected;
        }


        private void AdjustSizes()
        {
            _eventGauge.LayoutParameters.Width= Constants.EventGaugeSizePx;
            _eventGauge.LayoutParameters.Height = Constants.EventGaugeSizePx;
            _eventGauge.Invalidate();
        }


        void OnEventsReceived()
        {
            if (ViewModel.Responses == null)
                return;

            ViewModel.Responses.CollectionChanged -= OnResponsesChanged;
            ViewModel.Responses.CollectionChanged += OnResponsesChanged;
        }


        private void OnResponsesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var haveResponses = ViewModel.Responses.Count > 0;

            // Do we want to change event gauge label if no events??
            // Do we want to change event gauge alpha if no events??
            _eventGauge.EventCount = ViewModel.Responses.Count;

/*          // TODO Change notification based on action - need to test
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex > 1)
                        _responsesAdapter.NotifyItemRangeInserted(e.NewStartingIndex, e.NewItems.Count);
                    else
                        _responsesAdapter.NotifyDataSetChanged();
                    break;
                case NotifyCollectionChangedAction.Move:
                    _responsesAdapter.NotifyItemMoved(e.OldStartingIndex, e.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex > 1)
                        _responsesAdapter.NotifyItemRangeRemoved(e.OldStartingIndex, e.OldItems.Count);
                    else
                        _responsesAdapter.NotifyDataSetChanged();
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.NewStartingIndex > 1)
                        _responsesAdapter.NotifyItemRangeChanged(e.NewStartingIndex, e.NewItems.Count);
                    else
                        _responsesAdapter.NotifyDataSetChanged();
                    break;
                case NotifyCollectionChangedAction.Reset:// prob need to change to range
                    _responsesAdapter.NotifyDataSetChanged();
                    break;
                default:
                    _responsesAdapter.NotifyDataSetChanged();
                    break;
            }  */
            // inform adapter of change
            _responsesAdapter.NotifyDataSetChanged();
        }


        private void OnResponseUpdated(object s, int e)
        {
            // inform adapter of change
            _responsesAdapter.NotifyItemChanged(e);
        }



        private void OnCurrentEventCircleColorChanged(object sender, string newColor)
        {
            var color = Constants.GetEventTypeColor(Activity, newColor);
            SetEventGaugeColor(new ShadingColorPair(Color.White, color));
        }
        private void SetEventGaugeColor(ShadingColorPair colors)
        {
            _eventGauge.RingColors = colors;
            _eventGauge.SetCenterTint(colors.EndColor);
            _currentResponseColor = colors.EndColor;
        }


        private async void OnResponseSelected(object sender, int index)
        {
            IsShowingDetails = true;
            OnViewResponseDetails?.Invoke(this, new EventArgs());
            var response = ViewModel.Responses[index].Response;
            await NavigateToEventDetails(response);
        }

        private async Task NavigateToEventDetails(ResponseModel response)
        {
            string responseJson = JsonConvert.SerializeObject(response);

            var intent = new Intent(Activity, typeof(EventDetailActivity));
            intent.PutExtra(Constants.IntentDataResponseLabel, responseJson);
            if (_responsesAdapter.UserLocation != null)
            {
                intent.PutExtra(Constants.IntentDataUserLatLabel, (double)_responsesAdapter.UserLocation.Latitude);
                intent.PutExtra(Constants.IntentDataUserLongLabel, (double)_responsesAdapter.UserLocation.Longitude);
            }

            StartActivity(intent);
        }



        private void OnEventGaugeClicked(object sender, EventArgs e)
        {

        }


        public void OnLocationChanged(object s, LocationChangedEventArgs e)
        {
            _responsesAdapter.UserLocation = new LatLng(e.CurrentLocation.Latitude, e.CurrentLocation.Longitude);
        }


        public override void OnResume()
        {
            base.OnResume();
            _responsesAdapter?.PropogateActivityState(ActivityState.Resumed);
        }


        public override void OnStop()
        {
            _responsesAdapter?.PropogateActivityState(ActivityState.Stopped);
            base.OnStop();
        }

        public override void OnPause()
        {
            _responsesAdapter?.PropogateActivityState(ActivityState.Paused);
            base.OnPause();
        }

        public override void OnDestroy()
        {
            _responsesAdapter?.PropogateActivityState(ActivityState.Destroyed);
            base.OnDestroy();
        }

        public override void OnLowMemory()
        {
            _responsesAdapter?.PropogateActivityState(ActivityState.LowMemory);
            base.OnLowMemory();
        }


    }
}