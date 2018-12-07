using Android.OS;
using Android.Views;

using Fragment = global::Android.Support.V4.App.Fragment;

namespace Edison.Mobile.User.Client.Droid.Fragments
{
    public class SettingsPageFragment : Fragment
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