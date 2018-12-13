using System;
using System.Collections.Generic;

using Android.App;
using Android.OS;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V4.View;

using Java.Lang;

using Edison.Mobile.Android.Common;
using Edison.Mobile.User.Client.Core.ViewModels;
using Edison.Mobile.User.Client.Droid.Adapters;
using Edison.Mobile.User.Client.Droid.Fragments;
using Edison.Mobile.User.Client.Droid.Ioc;
using Edison.Mobile.User.Client.Core.Ioc;
using Edison.Mobile.Android.Common.Ioc;
using Edison.Mobile.Common.Ioc;

using Toolbar = Android.Support.V7.Widget.Toolbar;
using Fragment = Android.Support.V4.App.Fragment;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;

namespace Edison.Mobile.User.Client.Droid.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar")]
//    [Activity(Label = "@string/app_name", MainLauncher = true, NoHistory = true, Exported = true, ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait, Theme = "@style/AppTheme.NoActionBar")]
    public class MainActivity : BaseActivity<MainViewModel>
    {

        private const string StateKey_ActionbarTitle = "actionBarTitle";
        private const int TransitionDelayMs = 80;

        private Context _context;
        private Toolbar _toolbar;
        private AppCompatTextView _customToolbarTitle;
        private AppCompatTextView _customToolbarSubtitle;
        private LinearLayout _customToolbarTitleWrapper;
        private DrawerLayout _drawer;
        private LinearLayout _bottomSheet;
        private BottomSheetBehavior _bottomSheetBehaviour;
//        private BottomSheet4StateBehaviour _bottomSheetBehaviour;
        private Fragment _fragment;
        private NavMenuExpandableListAdapter _navDrawerListAdapter;
        private ExpandableListView _navDrawerListview;
        private List<TextImageResourcePair> _listDataGroups;
        private Dictionary<string, List<TextImageResourcePair>> _listDataItems;
        private int _previousGroup = -1;


        private List<AppCompatImageButton> _imageButtons = new List<AppCompatImageButton>();


        // Navigation Drawer Group Data
        private static readonly List<ResourcePair> NAV_DRAWER_GROUP_RESOURCES = new List<ResourcePair>
        {
            new ResourcePair(Resource.String.home, Resource.Drawable.abc_ic_menu_copy_mtrl_am_alpha),
            new ResourcePair(Resource.String.my_info, Resource.Drawable.abc_ic_menu_copy_mtrl_am_alpha),
            new ResourcePair(Resource.String.notifications, Resource.Drawable.abc_ic_menu_copy_mtrl_am_alpha),
            new ResourcePair(Resource.String.settings, Resource.Drawable.abc_ic_menu_copy_mtrl_am_alpha),
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
 //           Container.Initialize(new CoreContainerRegistrar(), new PlatformCommonContainerRegistrar(this), new PlatformContainerRegistrar());

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.main_activity);
            Initialize(savedInstanceState);
  //          Window.SetStatusBarColor(Resources.GetColor(Resource.Color.app_background));
            Window.SetStatusBarColor(new Color(ContextCompat.GetColor(this, Resource.Color.app_blue)));
        }



        private void Initialize(Bundle savedInstanceState)
        {
            BindResources();
            SetUpToolbar();
            SetUpDrawer();
            SetUpBottomSheet();
            RestoreState(savedInstanceState);
            Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
        }


        private void BindResources()
        {
            _context = this;
            _drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            _toolbar = FindViewById<Toolbar>(Resource.Id.nav_toolbar);
            _customToolbarTitleWrapper = FindViewById<LinearLayout>(Resource.Id.toolbar_title_wrapper);
            _customToolbarTitle = FindViewById<AppCompatTextView>(Resource.Id.toolbar_title);
            _customToolbarSubtitle = FindViewById<AppCompatTextView>(Resource.Id.toolbar_subtitle);
            _bottomSheet = FindViewById<LinearLayout>(Resource.Id.bottom_sheet);
            _bottomSheetBehaviour = BottomSheetBehavior.From(_bottomSheet);
            //            _bottomSheetBehaviour = BottomSheet4StateBehaviour.From(_bottomSheet);


            _imageButtons.Add(FindViewById<AppCompatImageButton>(Resource.Id.qc_emergency));
            _imageButtons.Add(FindViewById<AppCompatImageButton>(Resource.Id.qc_activity));
            _imageButtons.Add(FindViewById<AppCompatImageButton>(Resource.Id.qc_safe));
            foreach (var button in _imageButtons)
            {
                button.Click += OnButtonClick;
            }


        }

        private void OnButtonClick(object sender, EventArgs e)
        {
            if (sender is AppCompatImageButton imgButton)
                Toast.MakeText(this, (string)imgButton.Tag, ToastLength.Short).Show();

        }

        private void SetUpToolbar()
        {
            // Not having reaised AppBar area, so dont set SupportActionBar
            //SetSupportActionBar(_toolbar);

            // Set the page Title - Required because of the way that Android sets up app asset names
            _toolbar.Title = null;  // using the custom title so it can be centered
            SetToolbarCustomTitle(Resources.GetString(Resource.String.app_name));
            // Set the subtitle
//            SetToolbarCustomSubtitle("Subtitle");
//            _toolbar.Subtitle = "Subtitle";

            //Set tolbar title colors
//            Color col = Resources.GetColor(Resource.Color.app_blue);
            Color col = new Color(ContextCompat.GetColor(this, Resource.Color.app_blue));
            _toolbar.SetTitleTextColor(col);
            _toolbar.SetSubtitleTextColor(col);

            // Manually add the menu to the toolbar and set the menu item click event(not done automatically because not setting SupportActionBar)
            OnCreateOptionsMenu(_toolbar.Menu);
            _toolbar.MenuItemClick += OnMenuItemClicked;

            _toolbar.ViewTreeObserver.GlobalLayout += OnToolbarLayout;
        }

        private void SetUpDrawer()
        {
            // Set up navigation drawer

            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, _drawer, _toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            _drawer.AddDrawerListener(toggle);
            toggle.SyncState();
          //  toggle.DrawerArrowDrawable.Color = Resources.GetColor(Resource.Color.app_blue);
            toggle.DrawerArrowDrawable.Color = new Color(ContextCompat.GetColor(this, Resource.Color.app_blue));

            _navDrawerListview = FindViewById<global::Android.Widget.ExpandableListView>(Resource.Id.nav_list);
            var navMenuData = GetNavDrawerContents();
            _listDataGroups = navMenuData.Item1;
            _listDataItems = navMenuData.Item2;
            _navDrawerListAdapter = new NavMenuExpandableListAdapter(this, _listDataGroups, _listDataItems);
            _navDrawerListview.SetAdapter(_navDrawerListAdapter);
            _navDrawerListview.GroupExpand += OnGroupExpand;
            _navDrawerListview.GroupCollapse += OnGroupCollapse;
            _navDrawerListview.ChildClick += OnChildClicked;
            _navDrawerListview.GroupClick += OnGroupClicked;
        }

        private void SetUpBottomSheet()
        {
//            _bottomSheetBehaviour.SetBottomSheetCallback(new IntelligentBottomSheetCallback());
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
            _fragment = new HomePageFragment();
            SupportFragmentManager.BeginTransaction().Replace(Resource.Id.content_container, _fragment, null).Commit();
//            ReplaceFragmentWithDelay(_fragment);
        }

        // Handle any title changes that come with Fragments, if OnCreate has been called due to a change in screen orientation
        private void RestoreState(Bundle savedInstanceState)
        {
            // This allow us to know if the activity was recreated after orientation change and restore the Toolbar title
            if (savedInstanceState == null)
                ShowDefaultFragment();
            else
                SupportActionBar.Title = savedInstanceState.GetCharSequence(StateKey_ActionbarTitle);
        }



        public override void OnBackPressed()
        {
            if (_drawer == null)
                _drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            if (_drawer.IsDrawerOpen(GravityCompat.Start))
                _drawer.CloseDrawer(GravityCompat.Start);
            else
                base.OnBackPressed();
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
                        if (!(_fragment is HomePageFragment))
                        {
                            _fragment = new HomePageFragment();
                            ReplaceFragmentWithDelay(_fragment);
                        }
                        // Close the drawer
                        drawer.CloseDrawer(GravityCompat.Start);
                        break;

                    case Resource.String.my_info:
                        if (!(_fragment is ProfilePageFragment))
                        {
                            _fragment = new ProfilePageFragment();
                            ReplaceFragmentWithDelay(_fragment);
                        }
                        // Close the drawer
                        drawer.CloseDrawer(GravityCompat.Start);
                        break;

                    case Resource.String.notifications:
                        if (!(_fragment is NotificationsPageFragment))
                        {
                            _fragment = new NotificationsPageFragment();
                            ReplaceFragmentWithDelay(_fragment);
                        }
                        // Close the drawer
                        drawer.CloseDrawer(GravityCompat.Start);
                        break;

                    case Resource.String.settings:
                        if (!(_fragment is SettingsPageFragment))
                        {
                            _fragment = new SettingsPageFragment();
                            ReplaceFragmentWithDelay(_fragment);
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



        // Start Fragment transaction with delay to avoid any graphics issues while closing the drawer
        private void ReplaceFragmentWithDelay(Fragment fragment, string tag = null)
        {
            new Handler().PostDelayed(() =>
            {
                SupportFragmentManager.BeginTransaction().Replace(Resource.Id.content_container, fragment, tag).Commit();
            }, TransitionDelayMs);
        }


        // Start this activity with delay to avoid any graphics issues while closing the drawer
        private void StartActivityWithDelay(Class activity)
        {
            new Handler().PostDelayed(() =>
            {
                StartActivity(new Intent(_context, activity));

            }, TransitionDelayMs);
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
                    Toast.MakeText(this, "Brightness clicked", ToastLength.Short).Show();
                    return true;




            }
            return base.OnOptionsItemSelected(item);
        }


        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.actionBarMenu, menu);
            _toolbar.UpdateMenuItemTint(Resources.GetColor(Resource.Color.app_blue));

            return base.OnCreateOptionsMenu(menu);
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


        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutCharSequence(StateKey_ActionbarTitle, SupportActionBar.Title);
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
            _toolbar.MenuItemClick -= OnMenuItemClicked;
            _navDrawerListview.GroupExpand -= OnGroupExpand;
            _navDrawerListview.GroupCollapse -= OnGroupCollapse;
            _navDrawerListview.ChildClick -= OnChildClicked;
            _navDrawerListview.GroupClick -= OnGroupClicked;

            foreach (var button in _imageButtons)
            {
                button.Click -= OnButtonClick;
            }

            _toolbar.ViewTreeObserver.GlobalLayout -= OnToolbarLayout;

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


