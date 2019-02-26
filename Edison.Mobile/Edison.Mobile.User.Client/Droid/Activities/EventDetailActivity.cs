using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Android.App;
using Android.OS;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Provider;
using Android.Content.PM;
using Android.Runtime;
using Android.Support.V7.View.Menu;
using static Android.Widget.SeekBar;

using Java.Lang;

using Edison.Mobile.Android.Common;
using Edison.Mobile.Android.Common.Controls;
using Edison.Mobile.Android.Common.Utilities;
using Edison.Mobile.User.Client.Core.ViewModels;
using Edison.Mobile.User.Client.Droid.Adapters;
using Edison.Mobile.User.Client.Droid.Fragments;


using Toolbar = Android.Support.V7.Widget.Toolbar;
using Fragment = Android.Support.V4.App.Fragment;
using Math = System.Math;
using Newtonsoft.Json;
using Edison.Core.Common.Models;
using Android.Support.V4.Content.Res;
using Android.Support.V4.Graphics.Drawable;
using Android.Support.Constraints;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Edison.Mobile.Common.Geo;
using Android.Content.Res;
using Android.Support.V4.Graphics;

namespace Edison.Mobile.User.Client.Droid.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait, WindowSoftInputMode = SoftInput.AdjustResize)]
    public class EventDetailActivity : BaseActivity<ResponseDetailsViewModel>, IOnMapReadyCallback
    {

        private const string StateKey_ActionbarTitle = "actionBarTitle";

        private const int RequestNotificationPermissionId = 0;
        private const int RequestLocationPermissionId = 1;
        private const int RequestWriteSettingsPermissionId = 2;

        private CoordinatorLayout _coordinatorLayout;
        private Toolbar _toolbar;
        private AppCompatTextView _customToolbarTitle;
        private AppCompatTextView _customToolbarSubtitle;
        private LinearLayout _customToolbarTitleWrapper;

        private ConstraintLayout _pageContainer;
        private RecyclerView _recyclerView;
        private EventDetailsAdapter adapter;
        private ImageView _eventImageView;

//        private Fragment _pageFragment;
        private Fragment _chatFragment;
        
        private float _colorHue = -1;
    
        private LatLng userLocationOld = null;
        private LatLng _eventLocation = null;
        //      private LatLng _eventLocationOld = null;
        //       private double _latDelta;
        //       private double _longDelta;
        public Marker UserLocationMarker { get; set; }
        public Marker EventLocationMarker { get; set; }
        public GoogleMap GMap { get; private set; }
        public MapView Map { get; set; }
        
        private EdisonLocation _oldUserLocation = null;
        private LatLng userLocation = null;
   //     public LatLng UserLocation
   //     {
   //         get => userLocation;
   //         set
   //         {
   //             userLocation = value;
   //            // OnLocationChanged?.Invoke(null, new LocationChangedEventArgs(_oldUserLocation, new EdisonLocation(userLocation.Latitude, userLocation.Longitude)));
   ////             UpdateMap();
        //    }
        //}
        
        private static BitmapDescriptor userLocationIcon = null;
        private BitmapDescriptor UserLocationIcon
        {
            get
            {
                if (userLocationIcon== null)
                    userLocationIcon = CreateUserIconDrawable().ToBitmapDescriptor();
                return userLocationIcon;
            }
        }

        
        public LinearLayout BottomSheet { get; private set; }
        public Edison.Mobile.Android.Common.Behaviors.BottomSheetBehavior BottomSheetBehaviour { get; private set; }


        public static string Name { get; private set; }




        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var responseJson = Intent.GetStringExtra("response");
            var response = JsonConvert.DeserializeObject<ResponseModel>(responseJson);
            var latitude = Intent.GetDoubleExtra("USER_LAT", double.NaN);
            var longitude = Intent.GetDoubleExtra("USER_LON", double.NaN);
            
            userLocation = new LatLng(latitude, longitude);
            ViewModel.Response = response;
            
            if (ViewModel.Response.Geolocation != null)
            {
                _eventLocation = new LatLng((double)ViewModel.Response.Geolocation.Latitude, (double)ViewModel.Response.Geolocation.Longitude);
            }
            
            SetContentView(Resource.Layout.event_detail_activity);

            /*            Task.Run(async () => {
                            await Constants.UpdateBarDimensionsAsync(this);
                        }); */
            Constants.UpdateBarDimensions(this);

            Initialize(savedInstanceState);
 //           Window.SetStatusBarColor(new Color(ContextCompat.GetColor(this, Resource.Color.app_red)));
        }

        private void Initialize(Bundle savedInstanceState)
        {
            BindResources();
            SetUpToolbar();
            SetUpBottomSheet();
            BindEvents();
            AdjustViewPositions();

            RestoreState(savedInstanceState);

            Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);

            InitializeMapView();
        }


        private void BindResources()
        {
            _coordinatorLayout = FindViewById<CoordinatorLayout>(Resource.Id.nav_coordinator_content);
            _toolbar = FindViewById<Toolbar>(Resource.Id.nav_toolbar);
            _customToolbarTitleWrapper = FindViewById<LinearLayout>(Resource.Id.toolbar_title_wrapper);
            _customToolbarTitle = FindViewById<AppCompatTextView>(Resource.Id.toolbar_title);
            _customToolbarSubtitle = FindViewById<AppCompatTextView>(Resource.Id.toolbar_subtitle);
            _pageContainer = FindViewById<ConstraintLayout>(Resource.Id.page_container);
            
            var color = Constants.GetEventTypeColor(this, ViewModel.Response.Color);
            _eventImageView = FindViewById<ImageView>(Resource.Id.event_detail_image);
            _eventImageView.SetImageResource(GetIconResourceId(ViewModel.Response.Icon));
            _eventImageView.BackgroundTintList = ColorStateList.ValueOf(color);
            
            _recyclerView = FindViewById<RecyclerView>(Resource.Id.detailed_message_list);
            _recyclerView.BringToFront();
            adapter = new EventDetailsAdapter(this, new List<NotificationModel>(ViewModel.Notifications));
            _recyclerView.SetAdapter(adapter);

            BottomSheet = FindViewById<LinearLayout>(Resource.Id.bottom_sheet);
            Map = FindViewById<MapView>(Resource.Id.map_container);
            BottomSheetBehaviour = new Edison.Mobile.Android.Common.Behaviors.BottomSheetBehavior();
            CoordinatorLayout.LayoutParams lp = BottomSheet.LayoutParameters as CoordinatorLayout.LayoutParams;
            lp.Behavior = BottomSheetBehaviour;
            BottomSheet.LayoutParameters = lp;
        }


        private void AdjustViewPositions()
        {
            // Set the Bottom Sheet parameters - dependent on screen dimensions
            if (Constants.BottomSheetSmallPeekHeightPx > -1)
                BottomSheetBehaviour.PeekHeight = Constants.BottomSheetSmallPeekHeightPx;
            if (Constants.BottomSheetHeightPx > -1)
                BottomSheet.LayoutParameters.Height = Constants.BottomSheetHeightPx;

            _pageContainer.SetPadding(0, 0, 0, Constants.BottomSheetSmallPeekHeightPx);
        }


        /*
                private void OnButtonClick(object sender, EventArgs e)
                {
                    if (sender is AppCompatImageButton imgButton)
                        Toast.MakeText(this, (string)imgButton.Tag, ToastLength.Short).Show();

                }
        */


        private void SetUpToolbar()
        {
            // Not having raised AppBar area, so don't set SupportActionBar
            //SetSupportActionBar(_toolbar);

            // Set the page Title - Required because of the way that Android sets up app asset names
            _toolbar.Title = null;  // using the custom title so it can be centered
 //           SetToolbarCustomTitle(Resources.GetString(Resource.String.app_name));
             SetToolbarCustomTitle(ViewModel.Response.Name);
            // Set the subtitle
            //            SetToolbarCustomSubtitle("Subtitle");
            //            _toolbar.Subtitle = "Subtitle";
            var closeIcon = ResourcesCompat.GetDrawable(Resources, Resource.Drawable.lclose, null).Mutate();
            DrawableCompat.SetTint(closeIcon, Color.White);
            _toolbar.NavigationIcon = closeIcon;

            var toolbarColor = Constants.GetEventTypeColor(this, ViewModel.Response.Color);
            Window.SetStatusBarColor(toolbarColor);
            _toolbar.SetBackgroundColor(toolbarColor);

            //Set toolbar title colors);
            _toolbar.SetTitleTextColor(Color.White);
            _toolbar.SetSubtitleTextColor(Color.White);
             SetToolbarCustomTitleColor(Color.White);
             SetToolbarCustomSubtitleColor(Color.White);

        }


        private void SetUpBottomSheet()
        {
            //            _bottomSheetBehaviour.SetBottomSheetCallback(new IntelligentBottomSheetCallback());
            LoadChatFragment();
        }

        private void BindEvents()
        {
            _toolbar.MenuItemClick += OnMenuItemClicked;
            _toolbar.ViewTreeObserver.GlobalLayout += OnToolbarLayout;
            _toolbar.NavigationClick += OnToolbarNavigation;
            //           FragmentPoppedOnBack += OnFragmentPopped;
            ViewModel.OnLocationChanged += OnLocationChanged;
            BottomSheetBehaviour.Slide += OnBottomSheetSlide;
            ViewModel.Notifications.CollectionChanged += HandleNotificationsCollectionChanged;
        }

        private void UnbindEvents()
        {
            _toolbar.MenuItemClick -= OnMenuItemClicked;
            _toolbar.ViewTreeObserver.GlobalLayout -= OnToolbarLayout;
            _toolbar.NavigationClick -= OnToolbarNavigation;
            ViewModel.OnLocationChanged -= OnLocationChanged;
            BottomSheetBehaviour.Slide -= OnBottomSheetSlide;
            ViewModel.Notifications.CollectionChanged -= HandleNotificationsCollectionChanged;


 //           FragmentPoppedOnBack -= OnFragmentPopped;
        }
        
        void HandleNotificationsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            adapter.Notifications = new List<NotificationModel>(ViewModel.Notifications);
            adapter.NotifyDataSetChanged();
        }
/*
        private void OnFragmentPopped(object s, Fragment fragment)
        {
            _pageFragment = fragment;
        }
*/


        /*
                private enum BottomSheetSettledState
                {
                    Closed,
                    Intermediate,
                    Open
                }
                private static BottomSheetSettledState _currentBottomDrawerState = BottomSheetSettledState.Closed;
                private class IntelligentBottomSheetCallback : BottomSheetBehavior.BottomSheetCallback
                {
                    private int _lastState;

                    public override void OnStateChanged(View bottomSheet, int newState)
                    {
                        var behaviour = BottomSheetBehavior.From(bottomSheet);
                        if (behaviour != null)
                        { 
                            //if hidden set to collapsed, peek height = 10dp
                            if (newState == BottomSheetBehavior.StateCollapsed || newState == BottomSheetBehavior.StateExpanded || newState == BottomSheetBehavior.StateHidden)
                                _lastState = newState;

                            switch (_currentBottomDrawerState)
                            {

                                case BottomSheetSettledState.Closed:
                                    if (newState == BottomSheetBehavior.StateExpanded)
                                    {
                                        behaviour.PeekHeight = PixelSizeConverter.DpToPx(180);
                                        behaviour.Hideable = true;
                                        _currentBottomDrawerState = BottomSheetSettledState.Intermediate;
                                        bottomSheet.LayoutParameters.Height = ViewGroup.LayoutParams.MatchParent;
                                        bottomSheet.Invalidate();
                                        bool test = true;
                                    }
                                    break;

                                case BottomSheetSettledState.Intermediate:
                                    if (newState == BottomSheetBehavior.StateExpanded)
                                    {
        //                                bottomSheet.LayoutParameters.Height = ViewGroup.LayoutParams.MatchParent;
        //                                behaviour.PeekHeight = PixelSizeConverter.DpToPx(180);
                                        behaviour.Hideable = true;
         //                               behaviour.State = BottomSheetBehavior.StateCollapsed;
                                        _currentBottomDrawerState = BottomSheetSettledState.Open;
                                        bool test = true;
                                    }
                                    else if (newState == BottomSheetBehavior.StateHidden)
                                    {
                                        behaviour.PeekHeight = PixelSizeConverter.DpToPx(20);
                                        behaviour.Hideable = false;
                                        behaviour.State = BottomSheetBehavior.StateCollapsed;
                                        _currentBottomDrawerState = BottomSheetSettledState.Closed;
                                        bottomSheet.LayoutParameters.Height = PixelSizeConverter.DpToPx(180);
                                        bottomSheet.Invalidate();
                                        bool test = true;

                                    }



                                    break;


                                case BottomSheetSettledState.Open:
                                    if (newState == BottomSheetBehavior.StateCollapsed)
                                    {
         //                               bottomSheet.LayoutParameters.Height = ViewGroup.LayoutParams.MatchParent;
         //                               behaviour.PeekHeight = PixelSizeConverter.DpToPx(180);
                                        behaviour.Hideable = true;
                                        behaviour.State = BottomSheetBehavior.StateCollapsed;
                                        _currentBottomDrawerState = BottomSheetSettledState.Intermediate;
                                        bool test = true;
                                    }
                                    else if (newState == BottomSheetBehavior.StateHidden)
                                    {

                                        behaviour.PeekHeight = PixelSizeConverter.DpToPx(20);
                                        behaviour.Hideable = false;
                                        behaviour.State = BottomSheetBehavior.StateCollapsed;
                                        _currentBottomDrawerState = BottomSheetSettledState.Closed;
                                        bottomSheet.LayoutParameters.Height = PixelSizeConverter.DpToPx(180);
                                        bottomSheet.Invalidate();
                                        bool test = true;
                                    }

                                    break;

                            }


                        }

                    }

                    public override void OnSlide(View bottomSheet, float slideOffset)
                    {
                        var offset = slideOffset;
                    }



                }
        */

/*
        private void ShowDefaultFragment()
        {
            // Assign initial Fragment
            _pageFragment = new HomePageFragment();
            ReplaceFragment(_pageFragment, Resource.Id.content_container, true, Resource.String.home.ToString());
            //           SupportFragmentManager.BeginTransaction().Replace(Resource.Id.content_container, _pageFragment, null).Commit();
            //            ReplaceFragmentWithDelay(_fragment);
        }
*/

        private void LoadChatFragment()
        {
            _chatFragment = new ChatFragment(Resource.Layout.event_detail_activity);
            ReplaceFragment(_chatFragment, Resource.Id.bottom_sheet_fragment_container, false);
            //           SupportFragmentManager.BeginTransaction().Replace(Resource.Id.bottom_sheet_fragment_container, _chatFragment, null).Commit();
        }



        // Handle any title changes that come with Fragments, if OnCreate has been called due to a change in screen orientation
        private void RestoreState(Bundle savedInstanceState)
        {

/*
            // This allow us to know if the activity was recreated after orientation change and restore the Toolbar title
            if (savedInstanceState == null)
               ShowDefaultFragment();
            else
            {
               _customToolbarTitle.Text = savedInstanceState.GetCharSequence(StateKey_ActionbarTitle);
                //                _customToolbarSubtitle.Text = savedInstanceState.GetCharSequence(StateKey_ActionbarSubtitle);
            }
*/

        }


        private void OnToolbarNavigation(object s, Toolbar.NavigationClickEventArgs e)
        {
            OnBackPressed();
        }


        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }



        private void OnMenuItemClicked(object sender, Toolbar.MenuItemClickEventArgs e)
        {
            OnOptionsItemSelected(e.Item);
        }


        public override bool OnOptionsItemSelected(IMenuItem item)
        {

            switch (item.ItemId)
            {
                default:

    //                return true;
                    break;


            }
            return base.OnOptionsItemSelected(item);
        }



        public bool SetToolbarCustomTitle(string title)
        {
            if (_customToolbarTitle == null)
                return false;
            _customToolbarTitle.Text = title;
            if (title == null)
                _customToolbarTitle.Visibility = ViewStates.Gone;
            else
                _customToolbarTitle.Visibility = ViewStates.Visible;
            return true;
        }
        public bool SetToolbarCustomSubtitle(string subtitle)
        {
            if (_customToolbarSubtitle == null)
                return false;
            _customToolbarSubtitle.Text = subtitle;
            if (subtitle == null)
                _customToolbarSubtitle.Visibility = ViewStates.Gone;
            else
                _customToolbarSubtitle.Visibility = ViewStates.Visible;
            return true;
        }

        public void SetToolbarCustomTitleColor(Color color)
        {
            _customToolbarTitle?.SetTextColor(color);
        }
        public void SetToolbarCustomSubtitleColor(Color color)
        {
            _customToolbarSubtitle?.SetTextColor(color);
        }


        public void OnBottomSheetSlide(object s, float slideOffset)
        {
            if (slideOffset >= 0 && slideOffset <= 1)
                _pageContainer.Alpha = 1 - slideOffset * 0.8f;
        }


        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutCharSequence(StateKey_ActionbarTitle, _customToolbarTitle.Text);
            //            outState.PutCharSequence(StateKey_ActionbarSubtitle, _customToolbarSubtitle.Text);
            base.OnSaveInstanceState(outState);
        }

        public override void OnAttachedToWindow()
        {

            base.OnAttachedToWindow();
        }

        public override void OnDetachedFromWindow()
        {

            base.OnDetachedFromWindow();
        }

        protected override void OnDestroy()
        {
            UnbindEvents();
            base.OnDestroy();
        }

        private bool toolbarMeasured = false;
        public void OnToolbarLayout(object sender, EventArgs e)
        {
            if (!toolbarMeasured)
            {
                var w = _toolbar.Width;
                if (w > 0)
                {
                    var inset_start = _toolbar.ContentInsetStart;
                    var inset_start_nav = _toolbar.ContentInsetStartWithNavigation;
                    var diff = inset_start_nav - inset_start;
                    var numItems = _toolbar.Menu.Size();
                    var rightPaddng = numItems * diff + inset_start;
                    var margin = System.Math.Max(inset_start_nav, rightPaddng);
                    var lp = _customToolbarTitleWrapper.LayoutParameters;
                    lp.Width = w - (2 * margin);
                    _customToolbarTitleWrapper.LayoutParameters = lp;
                    toolbarMeasured = true;
                }
            }

        }
        
        private void InitializeMapView()
        {
            Map?.OnCreate(null);
            Map?.GetMapAsync(this);
        }

        public async void OnMapReady(GoogleMap googleMap)
        {
            MapsInitializer.Initialize(Application.Context);
            GMap = googleMap;
            GMap.UiSettings.CompassEnabled = false;
            GMap.UiSettings.MyLocationButtonEnabled = false;
            GMap.UiSettings.MapToolbarEnabled = false;
            
            var color = Constants.GetEventTypeColor(this, ViewModel.Response.Color);
            float[] hsl = new float[3];
            ColorUtils.ColorToHSL(color, hsl);
            _colorHue = hsl[0];
            
            await DrawMapAsync();
        }
        
        
        private async Task DrawMapAsync(bool moveMap = true)
        {
            if (GMap == null) return;

            if (_eventLocation == null && userLocation == null) return;

            await Task.Run(() =>
            {
                // Calculate the map position and zoom/size
                CameraUpdate cameraUpdate = null;
                if (moveMap)
                {
                    if (userLocation == null)
                        cameraUpdate = CameraUpdateFactory.NewLatLngZoom(_eventLocation, Constants.DefaultResponseMapZoom);
                    else if (_eventLocation == null)
                        cameraUpdate = CameraUpdateFactory.NewLatLngZoom(userLocation, Constants.DefaultResponseMapZoom);
                    else
                    {
                        var latDelta = Math.Abs(_eventLocation.Latitude - userLocation.Latitude);
                        var longDelta = Math.Abs(_eventLocation.Longitude - userLocation.Longitude);
                        var minLat = Math.Min(_eventLocation.Latitude, userLocation.Latitude) - latDelta / 4;
                        var maxLat = Math.Max(_eventLocation.Latitude, userLocation.Latitude) + latDelta / 4;
                        var minLong = Math.Min(_eventLocation.Longitude, userLocation.Longitude) - longDelta / 4;
                        var maxLong = Math.Max(_eventLocation.Longitude, userLocation.Longitude) + longDelta / 4;

                        LatLngBounds.Builder builder = new LatLngBounds.Builder();
                        builder.Include(new LatLng(minLat, minLong));
                        builder.Include(new LatLng(maxLat, maxLong));
                        // shouldn't need to include these but we'll include them just in case
                        builder.Include(new LatLng(_eventLocation.Latitude, _eventLocation.Longitude));
                        builder.Include(new LatLng(userLocation.Latitude, userLocation.Longitude));
                        LatLngBounds bounds = builder.Build();
                        cameraUpdate = CameraUpdateFactory.NewLatLngBounds(bounds, 0);
                    }
                    // Set the map position
                    this.RunOnUiThread(() => { GMap.MoveCamera(cameraUpdate); });
                    
                }
            });

            await Task.Run(() =>
            {
                // Add a markers
                if (_eventLocation != null)
                {
                    if (EventLocationMarker == null)
                    {
                        var markerOptions = new MarkerOptions();
                        markerOptions.SetPosition(_eventLocation);
                        if (_colorHue > -1)
                        {
                            var bmDescriptor = BitmapDescriptorFactory.DefaultMarker(_colorHue);
                            this.RunOnUiThread(() => { markerOptions.SetIcon(bmDescriptor); });
                        }
                        this.RunOnUiThread(() => { EventLocationMarker = GMap.AddMarker(markerOptions); });
                        
                    }
                    else
                    {
                        var bmDescriptor = BitmapDescriptorFactory.DefaultMarker(_colorHue);
                        this.RunOnUiThread(() => {
                            EventLocationMarker.SetIcon(bmDescriptor);
                            EventLocationMarker.Position = _eventLocation;
                        });

                    }
                }
                if (userLocation != null)
                {
                    this.RunOnUiThread(() => {
                        if (UserLocationMarker == null)
                        {
                            var markerOptions0 = new MarkerOptions();
                            markerOptions0.SetPosition(userLocation);
                            markerOptions0.SetIcon(UserLocationIcon);
                            markerOptions0.Anchor(0.5f, 0.5f);
                            UserLocationMarker = GMap.AddMarker(markerOptions0);
                        
                        }
                        else
                        {
                            UserLocationMarker.Position = userLocation;
                        }
                    });

                }
            });
            // Set the map type back to normal.
            this.RunOnUiThread(() => { GMap.MapType = GoogleMap.MapTypeNormal;});
        }
        
        public async Task UpdateMapAsync(LatLng userLocation)
        {
            if (userLocation != null)
            {
                if (userLocation != null)
                {
                    await Task.Run(async () =>
                    {
                        var latDelta = Math.Abs(userLocation.Latitude - userLocation.Latitude);
                        var longDelta = Math.Abs(userLocation.Longitude - userLocation.Longitude);
                        // Check for user location jitter check the user has moved more than 3m to update map
                        var latDistanceDelta = latDelta * 111111;
                        var longDistanceDelta = longDelta * 111111 * Math.Cos(userLocation.Latitude);
                        if (latDistanceDelta > Constants.UserLocationJitterThreshold || longDistanceDelta > Constants.UserLocationJitterThreshold)
                        {
                            // Greater than jitter threshold, so process.
                            bool moveMap = true;

                            if (userLocationOld == null)
                                userLocationOld = userLocation;

                            userLocation = userLocation;

                            latDelta = Math.Abs(userLocation.Latitude - userLocationOld.Latitude);
                            longDelta = Math.Abs(userLocation.Longitude - userLocationOld.Longitude);

                            if (_eventLocation == null)
                            {
                                // if eventLocation is null, then Only User Location is available, so use single location threshold
                                // Check to see if location has moved enough to move the map
                                if (latDistanceDelta <= Constants.SingleLocationRefocusMapThreshold && longDistanceDelta <= Constants.SingleLocationRefocusMapThreshold)
                                    // has so move map and update previous position
                                    userLocationOld = userLocation;
                                else
                                    // hasn't so don't move map
                                    moveMap = false;
                            }
                            else
                            {
                                // if event Location is not null, then both locations are available so use threshold that is % of distance between them
                                var latDeltaThreshold = Constants.LocationThresholdPercent * Math.Abs(_eventLocation.Latitude - userLocation.Latitude);
                                var longDeltaThreshold = Constants.LocationThresholdPercent * Math.Abs(_eventLocation.Longitude - userLocation.Longitude);
                                // Check to see if location has moved enough to move the map
                                if (latDelta > latDeltaThreshold || longDelta > longDeltaThreshold)
                                    // has so move map and update previous position
                                    userLocationOld = userLocation;
                                else
                                    // hasn't so don't move map - use previous location
                                    moveMap = false;
                            }
                            await DrawMapAsync(moveMap);
                        }
                        else
                        {
                            userLocation = userLocation;
                            await DrawMapAsync(true);
                        }
                    });
                }
            }
        }
        
        private Drawable CircleDrawable(Color color, int intrinsicSize = -1)
        {
            ShapeDrawable drw = new ShapeDrawable(new OvalShape());
            if (intrinsicSize > -1)
            {
                drw.SetIntrinsicWidth(intrinsicSize);
                drw.SetIntrinsicWidth(intrinsicSize);
            }
            drw.Paint.Color = color;
            return drw;
        }

        private Drawable CreateUserIconDrawable()
        {

            int dp16 = PixelSizeConverter.DpToPx(16);
            int dp12 = PixelSizeConverter.DpToPx(12);
            int dp4 = PixelSizeConverter.DpToPx(4);
            var fillColor = new Color(ResourcesCompat.GetColor(this.Resources, Resource.Color.user_location, null));
            var ringColor = new Color(ResourcesCompat.GetColor(this.Resources, Resource.Color.user_location_stroke, null));
            var pointColor = new Color(ResourcesCompat.GetColor(this.Resources, Resource.Color.user_location_center, null));
            LayerDrawable drw = new LayerDrawable(new Drawable[] {CircleDrawable(ringColor, dp16), CircleDrawable(fillColor, dp12), CircleDrawable(pointColor, dp4) } );
            drw.SetLayerSize(0, dp16, dp16);
            drw.SetLayerSize(1, dp12, dp12);
            drw.SetLayerSize(2, dp4, dp4);
            drw.SetLayerGravity(0, GravityFlags.Center);
            drw.SetLayerGravity(1, GravityFlags.Center);
            drw.SetLayerGravity(2, GravityFlags.Center);
            return drw;
        }
        
        private int GetIconResourceId(string iconName)
        {
            var id = this.GetDrawableId(iconName);
            return id == 0 ? Resource.Drawable.emergency : id;
        }


        public async void OnLocationChanged(object s, LocationChangedEventArgs e)
        {
            _oldUserLocation = e.LastLocation;
            var newLocation = new LatLng(e.CurrentLocation.Latitude, e.CurrentLocation.Longitude);
            await UpdateMapAsync(newLocation);
        }
    }
}