using System;
using Android.App;
using Android.Views;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using Edison.Mobile.Android.Common;
using Edison.Mobile.Android.Common.Ioc;
using Edison.Mobile.Common.Ioc;
using Edison.Mobile.User.Client.Core.Ioc;
using Edison.Mobile.User.Client.Core.ViewModels;
using Edison.Mobile.User.Client.Droid.Ioc;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Identity.Client;
using Android.Views.Animations;

namespace Edison.Mobile.User.Client.Droid.Activities
{
    [Activity(MainLauncher = true, Icon = "@mipmap/icon", Theme = "@android:style/Theme.NoTitleBar.Fullscreen")]
    public class LoginActivity : BaseActivity<LoginViewModel>
    {
        Button signInButton;
        

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Container.Initialize(new CoreContainerRegistrar(), new PlatformCommonContainerRegistrar(this), new PlatformContainerRegistrar());

            base.OnCreate(savedInstanceState);

            AppCenter.Start("959d5bd2-9e29-4f17-aed6-68885af8c63d", typeof(Analytics), typeof(Crashes));

            var relativeLayout = new RelativeLayout(this);
            var relativeLayoutParams = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            relativeLayout.LayoutParameters = relativeLayoutParams;
            relativeLayout.SetBackgroundColor(Color.White);
            signInButton = new Button(this)
            {
                Id = 1,
                Alpha = 0,
                Text = "Sign In",
            };

            var signInParams = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            signInParams.AddRule(LayoutRules.CenterInParent);
            signInButton.LayoutParameters = signInParams;

            relativeLayout.AddView(signInButton);

            SetContentView(relativeLayout);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
        }

        protected override void BindEventHandlers()
        {
            base.BindEventHandlers();

            ViewModel.OnDisplayLogin += OnDisplayLogin;
            ViewModel.OnNavigateToMainViewModel += OnNavigateToMainViewModel;
            signInButton.Click += OnSignInButtonClick;
        }

        protected override void UnBindEventHandlers()
        {
            base.UnBindEventHandlers();

            ViewModel.OnDisplayLogin -= OnDisplayLogin;
            ViewModel.OnNavigateToMainViewModel -= OnNavigateToMainViewModel;
            signInButton.Click -= OnSignInButtonClick;
        }

        void OnDisplayLogin()
        {
            signInButton.Animate().Alpha(1).SetDuration(1000).Start();
        }

        void OnNavigateToMainViewModel()
        {
            var intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
            Finish();
        }

        async void OnSignInButtonClick(object sender, EventArgs e)
        {
            await ViewModel.SignIn();
        }
    }
}
