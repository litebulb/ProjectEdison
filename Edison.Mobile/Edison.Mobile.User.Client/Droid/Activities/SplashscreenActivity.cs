using System;
using System.Collections.Generic;

using Android.App;
using Android.OS;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V4.View;
using Android.Support.V7.Widget;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;

using Java.Lang;

using Edison.Mobile.Android.Common;
using Edison.Mobile.User.Client.Core.ViewModels;
using Edison.Mobile.User.Client.Droid.Adapters;
using Edison.Mobile.User.Client.Droid.Fragments;
using Edison.Mobile.User.Client.Droid.Ioc;
using Edison.Mobile.User.Client.Core.Ioc;
using Edison.Mobile.Android.Common.Ioc;
using Edison.Mobile.Common.Ioc;
using System.Threading.Tasks;

//using Toolbar = Android.Support.V7.Widget.Toolbar;
//using Fragment = Android.Support.V4.App.Fragment;




#if DEBUG
using Android.Util;
#endif


namespace Edison.Mobile.User.Client.Droid.Activities
{
    //, Theme = "@style/AppTheme.NoActionBar.Splash"
    [Activity(Label = "@string/app_name", MainLauncher = true, Icon = "@mipmap/edison_launcher", NoHistory = true, Exported = true, 
        ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait)]
    public class SplashScreenActivity : BaseActivity<MainViewModel>
    {
        protected override void OnCreate(Bundle bundle)
        {
            Container.Initialize(new CoreContainerRegistrar(), new PlatformCommonContainerRegistrar(this), new PlatformContainerRegistrar());
 //           SetTheme(Resource.Style.AppTheme_NoActionBar);
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.splashscreen);

#if DEBUG
            Log.Debug("ACTIVITY", "***************************************");
            Log.Debug("ACTIVITY", "***************************************");
            Log.Debug("ACTIVITY", "***************************************");
            Log.Debug("ACTIVITY", "***************************************");
            Log.Debug("ACTIVITY", "*******  SPLASHSCREEN ACTIVITY  *******");
            Log.Debug("ACTIVITY", "***************************************");
            Log.Debug("ACTIVITY", "***************************************");
            Log.Debug("ACTIVITY", "***************************************");
            Log.Debug("ACTIVITY", "***************************************");
#endif




            Task.Run( async () => {
                // Try to Authenitcate silently
                // Simulate time to authenticate silrently
                await Task.Delay(5000);
                // If authenticated launch MainActivity
                var intent = new Intent(this, typeof(MainActivity));
                intent.AddFlags(ActivityFlags.NoAnimation);
                StartActivity(intent);

                // If not Authtenticated launch login activity





                Finish();
            });













/*

            // Try to get current location in background so is available when App starts

            Task task = Task.Run( async () => {
                if ( await PermissionsService.Instance.GetLocationStatusAsync() == PermissionStatus.Granted)
                    await QXFUtilities.GeoLocationServices.GeolocationService.Instance.GetLazyCurrentLocationAsync().ConfigureAwait(false);
            });
            task.ConfigureAwait(false);


            SetContentView(Resource.Layout.Splashscreen);

            // Get details on the display on a background thread
            DaveyTree.Mobile.App.GetDisplayDetailsBackground();



            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.NoAnimation);
            StartActivity(intent);

*/




        }

/*
        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            // uncomment  when start notification added
            //var myextravalue = intent.GetStringExtra("somevalue");
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, global::Android.Content.PM.Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
*/

    }
}