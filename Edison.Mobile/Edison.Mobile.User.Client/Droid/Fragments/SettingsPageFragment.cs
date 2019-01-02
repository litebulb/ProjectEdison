using Android.OS;
using Android.Views;
using Edison.Mobile.Android.Common;
using Edison.Mobile.User.Client.Core.ViewModels;

namespace Edison.Mobile.User.Client.Droid.Fragments
{
    public class SettingsPageFragment : BaseFragment<MainViewModel>  // use MainViewModel as a dummy ViewMiodel
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var root = inflater.Inflate(Resource.Layout.page_settings, container, false);
            return root;
        }
    }
}