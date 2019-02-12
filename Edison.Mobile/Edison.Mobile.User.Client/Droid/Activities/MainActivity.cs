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










namespace Edison.Mobile.User.Client.Droid.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait, WindowSoftInputMode = SoftInput.AdjustResize )]
//  [Activity(Label = "@string/app_name", MainLauncher = true, Exported = true, ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait, Theme = "@style/AppTheme.NoActionBar")]
    public class MainActivity : BaseActivity<MenuViewModel>
    {

        private const string StateKey_ActionbarTitle = "actionBarTitle";
        private const int DeafultTransitionDelayMs = 80;

        private const int RequestNotificationPermissionId = 0;
        private const int RequestLocationPermissionId = 1;
        private const int RequestWriteSettingsPermissionId = 2;

        private Context _context;
        private CoordinatorLayout _coordinatorLayout;
        private Toolbar _toolbar;
        private AppCompatTextView _customToolbarTitle;
        private AppCompatTextView _customToolbarSubtitle;
        private LinearLayout _customToolbarTitleWrapper;
        private DrawerLayout _drawer;
        private CircularProfileView _profileView;
        private AppCompatTextView _profileNameView;
        private FrameLayout _page_container;

        private LinearLayout _brightnessControlContainer;
        private AppCompatSeekBar _brightnessControl;


        private Fragment _pageFragment;
        private Fragment _chatFragment;

        private SimpleModalAlertDialogFragment _dialog;

        private NavMenuExpandableListAdapter _navDrawerListAdapter;
        private ExpandableListView _navDrawerListview;
        private List<TextImageResourcePair> _listDataGroups;
        private Dictionary<string, List<TextImageResourcePair>> _listDataItems;
        private int _previousGroup = -1;

        public LinearLayout BottomSheet { get; private set; }
        public Edison.Mobile.Android.Common.Behaviors.BottomSheetBehavior BottomSheetBehaviour { get; private set; }


        public static string Name { get; private set; }



        // Navigation Drawer Group Data
        private static readonly List<ResourcePair> NAV_DRAWER_GROUP_RESOURCES = new List<ResourcePair>
        {
            new ResourcePair(Resource.String.home, Resource.Drawable.ic_home_24),
            new ResourcePair(Resource.String.my_info, Resource.Drawable.user),
            new ResourcePair(Resource.String.notifications, Resource.Drawable.notification),
            new ResourcePair(Resource.String.settings, Resource.Drawable.settings),
        };


        // Navigation Drawer Item Data
        private static readonly Dictionary<int, List<ResourcePair>> NAV_DRAWER_ITEM_RESOURCES = new Dictionary<int, List<ResourcePair>>
        {

            {Resource.String.home, null },
            {Resource.String.my_info, null },
            {Resource.String.notifications, null },
            {Resource.String.settings, null },
            /*
            {Resource.String.nav_info, new List<ResourcePair>
                {
                    new ResourcePair(Resource.String.actionbar, Resource.Drawable.abc_ic_menu_copy_mtrl_am_alpha),
                    new ResourcePair(Resource.String.navigation_drawer, Resource.Drawable.abc_ic_menu_copy_mtrl_am_alpha),
                }
            },
            {Resource.String.nav_notifications, new List<ResourcePair>
                {
                    new ResourcePair(Resource.String.label, Resource.Drawable.abc_ic_menu_copy_mtrl_am_alpha),
                    new ResourcePair(Resource.String.activity_indicator, Resource.Drawable.abc_ic_menu_copy_mtrl_am_alpha),
                    new ResourcePair(Resource.String.progress_bar, Resource.Drawable.abc_ic_menu_copy_mtrl_am_alpha)
                }
            },
            */
        };



        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);

            //Initialization now done in MainApplication
            //Container.Initialize(new CoreContainerRegistrar(), new PlatformCommonContainerRegistrar(this), new PlatformContainerRegistrar());

#if DEBUG
            // Used when testing UI without having to login each time
            if (!LoginActivity.UsingLogon)
            {
                Task.Run(async () => {
                    await Constants.CalculateUIDimensionsAsync(this);
                });
            }
# endif

            SetContentView(Resource.Layout.main_activity);

/*            Task.Run(async () => {
                await Constants.UpdateBarDimensionsAsync(this);
            }); */
            Constants.UpdateBarDimensions(this);

            Initialize(savedInstanceState);
            Window.SetStatusBarColor(new Color(ContextCompat.GetColor(this, Resource.Color.app_background)));
        }



        private void Initialize(Bundle savedInstanceState)
        {
            BindResources();
            SetUpToolbar();
            SetUpDrawer();
            BindProfileData();
            SetUpBottomSheet();
            BindEvents();
            AdjustViewPositions();
            RestoreState(savedInstanceState);
            Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
        }


        private void BindResources()
        {
            _coordinatorLayout = FindViewById<CoordinatorLayout>(Resource.Id.nav_coordinator_content);
            _drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            _toolbar = FindViewById<Toolbar>(Resource.Id.nav_toolbar);
            _customToolbarTitleWrapper = FindViewById<LinearLayout>(Resource.Id.toolbar_title_wrapper);
            _customToolbarTitle = FindViewById<AppCompatTextView>(Resource.Id.toolbar_title);
            _customToolbarSubtitle = FindViewById<AppCompatTextView>(Resource.Id.toolbar_subtitle);
            _profileView = FindViewById<CircularProfileView>(Resource.Id.img_profile);
            _profileNameView = FindViewById<AppCompatTextView>(Resource.Id.profile_name);
            _page_container = FindViewById<FrameLayout>(Resource.Id.page_container);
            BottomSheet = FindViewById<LinearLayout>(Resource.Id.bottom_sheet);
            //BottomSheetBehaviour = Edison.Mobile.Android.Common.Behaviors.BottomSheetBehavior.From(_bottomSheet);
            //            _bottomSheetBehaviour = BottomSheet4StateBehaviour.From(_bottomSheet);
            BottomSheetBehaviour = new Edison.Mobile.Android.Common.Behaviors.BottomSheetBehavior();
            CoordinatorLayout.LayoutParams lp = BottomSheet.LayoutParameters as CoordinatorLayout.LayoutParams;
            lp.Behavior = BottomSheetBehaviour;
            BottomSheet.LayoutParameters = lp;




            _brightnessControlContainer = FindViewById<LinearLayout>(Resource.Id.brightness_slider_container);
            _brightnessControl = FindViewById<AppCompatSeekBar>(Resource.Id.brightness_slider);





        }


        private void AdjustViewPositions()
        {
            // Set the Bottom Sheet parameters - dependent on screen dimensions
            if (Constants.BottomSheetPeekHeightPx > -1)
                BottomSheetBehaviour.PeekHeight = Constants.BottomSheetPeekHeightPx;
            if (Constants.BottomSheetHeightPx > -1)
                BottomSheet.LayoutParameters.Height = Constants.BottomSheetHeightPx;

            var pageContainer = FindViewById<FrameLayout>(Resource.Id.page_container);
            pageContainer.SetPadding(0, 0, 0, Constants.BottomSheetPeekHeightPx);

            _brightnessControl.LayoutParameters.Width = Constants.EventGaugeAreaHeightPx;  // Transposed 90deg so Width actually Height
  //          var mode = ScreenUtilities.GetScreenBrightnessMode(this);
  //          var brightness = ScreenUtilities.GetScreenBrightnessWithMode(this, mode);
 //           // Get the Current Brightness level and adjust brightness slider if necessary
 //           GetAndSetBrightnessBrightness();
        }
        

        private void BindProfileData()
        {
            //Get data from MenuViewModel
            Name = ViewModel.ProfileName;
            var initials = ViewModel.Initials;
            var profileImageUri = ViewModel.ProfileImageUri;

            // Allocate Data to views
            _profileNameView.Text = Name;
#if DEBUG
            if (string.IsNullOrWhiteSpace(Name))
                _profileNameView.Text = "ALISON SUMMERFIELD";

            // for Testing
//_profileView.ProfileInitials.SetTypeface(ResourcesCompat.GetFont(this, Resource.Font.rubik_blackitalic), TypefaceStyle.BoldItalic);
if (ViewModel.Email == null || ViewModel.Email.Contains("bluemetal.com"))
            _profileView.SetProfileResource(Resource.Drawable.greyhound_kimmie);
//            _profileView.SetInitials("ALISON SUMMERFIELD");

#endif
            if (profileImageUri == null)  // Currently will always be null
            {
                
                _profileView.SetText(initials);
#if DEBUG
                if (string.IsNullOrWhiteSpace(initials))
                    _profileView.SetInitials("ALISON SUMMERFIELD");
#endif

            }
            else
            {
                // TODO - Fetch Image -  actually probably pre-fetch as a part of login and store as local file - or allocate field name if selected form file system
                // Will require Data service to be added to MenuViewModel - prob to return an object which we can cast to a Drawable/Bitmap. etc
                _profileView.SetProfileUri(profileImageUri);
            }

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
            SetToolbarCustomTitle(Resources.GetString(Resource.String.app_name));
            // Set the subtitle
//            SetToolbarCustomSubtitle("Subtitle");
//            _toolbar.Subtitle = "Subtitle";

            //Set toolbar title colors
            Color col = new Color(ContextCompat.GetColor(this, Resource.Color.app_blue));
            _toolbar.SetTitleTextColor(col);
            _toolbar.SetSubtitleTextColor(col);
            SetToolbarCustomTitleColor(col);
            SetToolbarCustomSubtitleColor(col);

            // Manually add the menu to the toolbar and set the menu item click event(not done automatically because not setting SupportActionBar)
            OnCreateOptionsMenu(_toolbar.Menu);

        }

        private void SetUpDrawer()
        {
            // Set up navigation drawer

            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, _drawer, _toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            _drawer.AddDrawerListener(toggle);
            toggle.SyncState();
            toggle.DrawerArrowDrawable.Color = new Color(ContextCompat.GetColor(this, Resource.Color.app_blue));

            _navDrawerListview = FindViewById<global::Android.Widget.ExpandableListView>(Resource.Id.nav_list);
            var navMenuData = GetNavDrawerContents();
            _listDataGroups = navMenuData.Item1;
            _listDataItems = navMenuData.Item2;
            _navDrawerListAdapter = new NavMenuExpandableListAdapter(this, _listDataGroups, _listDataItems);
            _navDrawerListview.SetAdapter(_navDrawerListAdapter);

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

            _navDrawerListview.GroupExpand += OnGroupExpand;
            _navDrawerListview.GroupCollapse += OnGroupCollapse;
            _navDrawerListview.ChildClick += OnChildClicked;
            _navDrawerListview.GroupClick += OnGroupClicked;

            _brightnessControl.ProgressChanged += OnBrightnessChanged;

            FragmentPoppedOnBack += OnFragmentPopped;

            BottomSheetBehaviour.Slide += OnBottomSheetSlide;
        }

        private void UnbindEvents()
        {
            _toolbar.MenuItemClick -= OnMenuItemClicked;
            _toolbar.ViewTreeObserver.GlobalLayout -= OnToolbarLayout;

            _navDrawerListview.GroupExpand -= OnGroupExpand;
            _navDrawerListview.GroupCollapse -= OnGroupCollapse;
            _navDrawerListview.ChildClick -= OnChildClicked;
            _navDrawerListview.GroupClick -= OnGroupClicked;

            _brightnessControl.ProgressChanged -= OnBrightnessChanged;

            BottomSheetBehaviour.Slide -= OnBottomSheetSlide;


            _brightnessControl.ProgressChanged -= OnBrightnessChanged;

            FragmentPoppedOnBack -= OnFragmentPopped;
        }

        private void OnFragmentPopped(object s, Fragment fragment)
        {
            _pageFragment = fragment;
        }



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

        private void ShowDefaultFragment()
        {
            // Assign initial Fragment
            _pageFragment = new HomePageFragment();
            ReplaceFragment(_pageFragment, Resource.Id.content_container, true, Resource.String.home.ToString());
 //           SupportFragmentManager.BeginTransaction().Replace(Resource.Id.content_container, _pageFragment, null).Commit();
//            ReplaceFragmentWithDelay(_fragment);
        }

        private void LoadChatFragment()
        {
            _chatFragment = new ChatFragment(Resource.Layout.main_activity);
            ReplaceFragment(_chatFragment, Resource.Id.bottom_sheet_fragment_container, false);
 //           SupportFragmentManager.BeginTransaction().Replace(Resource.Id.bottom_sheet_fragment_container, _chatFragment, null).Commit();
        }



        // Handle any title changes that come with Fragments, if OnCreate has been called due to a change in screen orientation
        private void RestoreState(Bundle savedInstanceState)
        {
            // This allow us to know if the activity was recreated after orientation change and restore the Toolbar title
            if (savedInstanceState == null)
                ShowDefaultFragment();
            else
            {
                _customToolbarTitle.Text = savedInstanceState.GetCharSequence(StateKey_ActionbarTitle);
                //                _customToolbarSubtitle.Text = savedInstanceState.GetCharSequence(StateKey_ActionbarSubtitle);
            }
        }



        public override void OnBackPressed()
        {
            if (_drawer == null)
                _drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            if (_drawer.IsDrawerOpen(GravityCompat.Start))
                _drawer.CloseDrawer(GravityCompat.Start);
            else
                base.OnBackPressWithFragmentManagement();
        }


        public void OnGroupExpand(object sender, global::Android.Widget.ExpandableListView.GroupExpandEventArgs e)
        {
            if (e.GroupPosition != _previousGroup)
                _navDrawerListview.CollapseGroup(_previousGroup);
            _previousGroup = e.GroupPosition;
        }
        public void OnGroupCollapse(object sender, global::Android.Widget.ExpandableListView.GroupCollapseEventArgs e)
        {
#if DEBUG
            bool test = true;
#endif
        }
        public void OnGroupClicked(object sender, global::Android.Widget.ExpandableListView.GroupClickEventArgs e)
        {
            if (e.GroupPosition == _previousGroup)
            {
                _navDrawerListview.CollapseGroup(e.GroupPosition);
                _previousGroup = -1;
            }
            else
                _navDrawerListview.ExpandGroup(e.GroupPosition);

            OnNavigationItemSelected(NAV_DRAWER_GROUP_RESOURCES[e.GroupPosition].Resource1, -1, _drawer);
        }
        public void OnChildClicked(object sender, global::Android.Widget.ExpandableListView.ChildClickEventArgs e)
        {
            OnNavigationItemSelected(NAV_DRAWER_GROUP_RESOURCES[e.GroupPosition].Resource1,
                                    NAV_DRAWER_ITEM_RESOURCES[NAV_DRAWER_GROUP_RESOURCES[e.GroupPosition].Resource1][e.ChildPosition].Resource1,
                                    _drawer);
        }





        private Tuple<List<TextImageResourcePair>, Dictionary<string, List<TextImageResourcePair>>> GetNavDrawerContents()
        {

            List<TextImageResourcePair> groups = new List<TextImageResourcePair>();
            Dictionary<string, List<TextImageResourcePair>> items = new Dictionary<string, List<TextImageResourcePair>>();



            foreach (var group in NAV_DRAWER_GROUP_RESOURCES)
            {
                string groupText = this.GetString(group.Resource1);
                var groupIconResource = group.Resource2;
                groups.Add(new TextImageResourcePair(groupText, groupIconResource));

                var itemsResourceList = NAV_DRAWER_ITEM_RESOURCES[group.Resource1];
                if (itemsResourceList == null)
                    items.Add(groupText, null);
                else
                {
                    List<TextImageResourcePair> itemsList = new List<TextImageResourcePair>();
                    foreach (var item in itemsResourceList)
                    {
                        string itemText = this.GetString(item.Resource1);
                        var itemIconResource = item.Resource2;
                        itemsList.Add(new TextImageResourcePair(itemText, itemIconResource));
                    }
                    items.Add(groupText, itemsList);
                }

            }

            return new Tuple<List<TextImageResourcePair>, Dictionary<string, List<TextImageResourcePair>>>(groups, items);
        }


        // This needs to be moved to NavigationManager
        public void OnNavigationItemSelected(int groupResource, int itemResource, DrawerLayout drawer)
        {

            if (itemResource == -1)
            {
                // group clicked
                // process groups that have no children - navigate to fragment
                switch (groupResource)
                {
                    case Resource.String.home:
                        if (!(_pageFragment is HomePageFragment))
                        {
                            _pageFragment = GetFragmentFromBackstack(Resource.String.home.ToString());
                            if (_pageFragment ==  null)
                                _pageFragment = new HomePageFragment();
                            ReplaceFragmentWithDelay(_pageFragment, Resource.Id.content_container, true, Resource.String.home.ToString());
                        }
                        // Close the drawer
                        drawer.CloseDrawer(GravityCompat.Start);
                        break;

                    case Resource.String.my_info:
                        if (!(_pageFragment is ProfilePageFragment))
                        {
                            _pageFragment = GetFragmentFromBackstack(Resource.String.my_info.ToString());
                            if (_pageFragment == null)
                                _pageFragment = new ProfilePageFragment();
                            ReplaceFragmentWithDelay(_pageFragment, Resource.Id.content_container, true, Resource.String.my_info.ToString());
                        }
                        // Close the drawer
                        drawer.CloseDrawer(GravityCompat.Start);
                        break;

                    case Resource.String.notifications:
                        if (!(_pageFragment is NotificationsPageFragment))
                        {
                            _pageFragment = GetFragmentFromBackstack(Resource.String.notifications.ToString());
                            if (_pageFragment == null)
                                _pageFragment = new NotificationsPageFragment();
                            ReplaceFragmentWithDelay(_pageFragment, Resource.Id.content_container, true, Resource.String.notifications.ToString());
                        }
                        // Close the drawer
                        drawer.CloseDrawer(GravityCompat.Start);
                        break;

                    case Resource.String.settings:
                        if (!(_pageFragment is SettingsPageFragment))
                        {
                            _pageFragment = GetFragmentFromBackstack(Resource.String.settings.ToString());
                            if (_pageFragment == null)
                                _pageFragment = new SettingsPageFragment();
                            ReplaceFragmentWithDelay(_pageFragment, Resource.Id.content_container, true, Resource.String.settings.ToString());
                        }
                        // Close the drawer
                        drawer.CloseDrawer(GravityCompat.Start);
                        break;

                    default:
                        // Do nothing as a group that expands and/or doesn't navigate was clicked
                        break;
                }
            }
            else
            {
                // child clicked
                // navigate to fragment
                switch (itemResource)
                {


                }

                // Close the drawer
                drawer.CloseDrawer(GravityCompat.Start);
            }

        }

        private void OnMenuItemClicked(object sender, Toolbar.MenuItemClickEventArgs e)
        {
            OnOptionsItemSelected(e.Item);
        }


        public override bool OnOptionsItemSelected(IMenuItem item)
        {

            switch (item.ItemId)
            {
                case global::Android.Resource.Id.Home:
                    _drawer.OpenDrawer(GravityCompat.Start);
                    return true;


                case Resource.Id.action_brightness:
                    // Need to adjust the Brightness control position here as can guarantee the toolbar has been laid out
                    UpdateBightnessPosition();
                    // Update the control visibility
                    if (_brightnessControlContainer.Visibility == ViewStates.Visible)
                        _brightnessControlContainer.Visibility = ViewStates.Invisible;
                    else
                    {
                        if (EnsureWriteSettingsPrivilege())
                            HandleBrightnessPermissionGranted();
                    }
                    return true;




            }
            return base.OnOptionsItemSelected(item);
        }

        private void HandleBrightnessPermissionGranted()
        {
             GetAndSetBrightnessBrightness();
            _brightnessControlContainer.Visibility = ViewStates.Visible;
        }

        private void GetAndSetBrightnessBrightness()
        {
            // get current brightness mode
            var mode = ScreenUtilities.GetScreenBrightnessMode(this);
            var brightness = ScreenUtilities.GetScreenBrightnessWithMode(this, mode);
            var brightnessInt = (int)Math.Round(brightness * 100);
            // set the level on the control if different from current position (user or system may have altered the brightness outside the app)
            // <0 check done in case auto value returned
            if (brightnessInt >= 0 && brightnessInt != _brightnessControl.Progress)
                _brightnessControl.Progress = brightnessInt;
            else if (brightnessInt < 0 && _brightnessControl.Progress != 100)  // Done for initialization purposed - especially on emulators
                _brightnessControl.Progress = 100;
        }


        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.actionBarMenu, menu);
            _toolbar.UpdateMenuItemTint(new Color(ContextCompat.GetColor(this, Resource.Color.app_blue)));

            return base.OnCreateOptionsMenu(menu);
        }

        private void UpdateBightnessPosition()
        {
            // Only update position if not previously updated
            if (Constants.BrightnessContainerWidth == -1)

            {
                Constants.UpdateBrightnessControlDimensions(_toolbar, FindViewById<ActionMenuItemView>(Resource.Id.action_brightness));
                // Set the brightness control container width
                _brightnessControlContainer.LayoutParameters.Width = Constants.BrightnessContainerWidth;
                // Set the layout margin for the brightness control container to bring it just under the menu item icon
                var lp = (ViewGroup.MarginLayoutParams)_brightnessControlContainer.LayoutParameters;
                lp.SetMargins(0, -Constants.BrightnessToolbarItemIconBottomPadding, 0, 0);
                _brightnessControlContainer.LayoutParameters = lp;
                _brightnessControlContainer.Invalidate();
            }
        }

        private void OnBrightnessChanged(object sender, ProgressChangedEventArgs e)
        {
            var prog = e.Progress;
            // change brightness
            float brightness = (float)(e.Progress) / 100f;
            ScreenUtilities.SetManualScreenBrightnessLevel(this, brightness);
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

        public bool EnsureWriteSettingsPrivilege()
        {
            var permission = global::Android.Manifest.Permission.WriteSettings;
            if (ContextCompat.CheckSelfPermission(this, permission) == Permission.Granted || Settings.System.CanWrite(this))
                return true;
            // Ask for permission - causes OnRequestPermissionsResult to be called with the result
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                // Need to open up the system permissions settings page.to give permissions (Android requirement for 6.0 onwards)
                // Display a dialog to warn the user otherwise the will be confused
                _dialog = new SimpleModalAlertDialogFragment
                {
                    DialogTitle = Resources.GetString(Resource.String.permission_required),
                    DialogMessage = Resources.GetString(Resource.String.screen_brightness_dialog_message),
                    DialogPositiveText = Resources.GetString(Resource.String.yes),
                    DialogNegitiveText = Resources.GetString(Resource.String.no)
                };
                _dialog.DialogResponse += OnPerimissionDialogResponse;
                DisplayDialogFragment(_dialog, "dialog");
            }
            else
                global::Android.Support.V4.App.ActivityCompat.RequestPermissions(this, new string[] { permission }, RequestWriteSettingsPermissionId);
            return false;
        }


        private void OnPerimissionDialogResponse(object sender, DialogClickEventArgs e)
        {
            if (e.Which == -1) // Yes
            {
                // Open up the system permissions settings page.to give permission (Android requirement for 6.0 onwards)
                Intent intent = new Intent(Settings.ActionManageWriteSettings);
                intent.SetData(global::Android.Net.Uri.Parse("package:" + this.PackageName));
                this.StartActivityForResult(intent, RequestWriteSettingsPermissionId);
            }
            // Dismiss & cleanup the DialogFragement
            _dialog.DialogResponse -= OnPerimissionDialogResponse;
            _dialog.Dismiss();
            _dialog.Dispose();
        }

        // Called when user responds to in app permissions request
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            switch (requestCode)
            {
                case RequestWriteSettingsPermissionId:
                    if (Settings.System.CanWrite(this))
                        //Permission granted
                        HandleBrightnessPermissionGranted();
                    else
                    {
                        //Permission Denied
                        var snack = Snackbar.Make(_coordinatorLayout, Resources.GetString(Resource.String.app_permissions_error2), Snackbar.LengthShort);
                        snack.View.Elevation = Resources.GetDimensionPixelSize(Resource.Dimension.snack_bar_elevation);
                        snack.Show();
                    }
                    break;

            }
        }
        // Called when user responds to in app permissions request
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            switch (requestCode)
            {
                case RequestWriteSettingsPermissionId:
                    {
                        var len = grantResults.Length;
                        if (len > 0 && grantResults[0] == Permission.Granted)
                        {
                            //Permission granted
                            HandleBrightnessPermissionGranted();
                        }
                        else
                        {
                            //Permission Denied
                            var snack = Snackbar.Make(_coordinatorLayout, Resources.GetString(Resource.String.app_permissions_error2), Snackbar.LengthShort);
                            snack.View.Elevation = Resources.GetDimensionPixelSize(Resource.Dimension.snack_bar_elevation);
                            snack.Show();
                        }
                    }
                    break;
            }
        }



        public void OnBottomSheetSlide(object s, float slideOffset)
        {
            if (slideOffset >= 0 && slideOffset <= 1)
                _page_container.Alpha = 1 - slideOffset * 0.8f;
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


    }
}


