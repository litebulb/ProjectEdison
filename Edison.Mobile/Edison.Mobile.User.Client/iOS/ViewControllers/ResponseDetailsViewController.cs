using System;
using Edison.Core.Common.Models;
using Edison.Mobile.iOS.Common.Shared;
using Edison.Mobile.iOS.Common.Views;
using Edison.Mobile.User.Client.Core.CollectionItemViewModels;
using Edison.Mobile.User.Client.Core.ViewModels;
using Edison.Mobile.User.Client.iOS.Shared;
using MapKit;
using UIKit;

namespace Edison.Mobile.User.Client.iOS.ViewControllers
{
    public class ResponseDetailsViewController : BaseViewController<ResponseDetailsViewModel>
    {
        MKMapView mapView;

        public ResponseDetailsViewController(ResponseCollectionItemViewModel response)
        {
            ViewModel.Response = response.Response;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = ViewModel.Response?.ActionPlan?.Name;

            View.BackgroundColor = PlatformConstants.Color.BackgroundGray;

            NavigationItem.LeftBarButtonItem = new UIBarButtonItem(Constants.Assets.CloseX, UIBarButtonItemStyle.Plain, (sender, e) =>
            {
                NavigationController.PopViewController(false);
            });
        }
    }
}
