using Android.App;
using Android.OS;
using Edison.Mobile.Admin.Client.Core.Ioc;
using Edison.Mobile.Admin.Client.Core.ViewModels;
using Edison.Mobile.Android.Common;
using Edison.Mobile.Android.Common.Ioc;
using Edison.Mobile.Common.Ioc;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace Edison.Mobile.Admin.Client.Droid.Activities
{
    [Activity(Label = "", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : BaseActivity<MainViewModel>
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Container.Initialize(new PlatformCommonContainerRegistrar(), new CoreContainerRegistrar());

            base.OnCreate(savedInstanceState);

            AppCenter.Start("5530b00a-a54a-4200-881b-5fb986fe412c", typeof(Analytics), typeof(Crashes));

            SetContentView(Resource.Layout.Main);
        }
    }
}

