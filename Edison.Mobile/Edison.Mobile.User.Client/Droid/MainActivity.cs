using Android.App;
using Android.Widget;
using Android.OS;
using Edison.Mobile.Common.Ioc;
using Edison.Mobile.User.Client.Droid.Ioc;
using Edison.Mobile.User.Client.Core.Ioc;

namespace Edison.Mobile.User.Client.Droid
{
    [Activity(Label = "Edison.Mobile.User.Client", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        int count = 1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Container.Initialize(new AndroidContainerRegistrar(), new CoreContainerRegistrar());

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.myButton);

            button.Click += delegate { button.Text = $"{count++} clicks!"; };
        }
    }
}

