using Android.OS;
using Android.Views;
using Edison.Mobile.Android.Common;
using Edison.Mobile.Android.Common.Controls;
using Edison.Mobile.User.Client.Core.ViewModels;


namespace Edison.Mobile.User.Client.Droid.Fragments
{
    public class HomePageFragment : BaseFragment<ResponsesViewModel>
    {

        private CircularEventGauge _eventGauge;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var root = inflater.Inflate(Resource.Layout.page_home, container, false);
            BindViews(root);
            AdjustSizes();
            return root;
        }


        private void BindViews(View root)
        {
            _eventGauge = root.FindViewById<CircularEventGauge>(Resource.Id.event_gauge);
        }


        private void AdjustSizes()
        {
            _eventGauge.LayoutParameters.Width = Constants.EventGaugeSizePx;
            _eventGauge.LayoutParameters.Height = Constants.EventGaugeSizePx;
            _eventGauge.Invalidate();
        }


    }
}