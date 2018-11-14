using System;
using Edison.Mobile.Common.Auth;
using Edison.Mobile.Common.Geo;
using Edison.Mobile.Common.Notifications;
using Edison.Mobile.Common.Shared;

namespace Edison.Mobile.User.Client.Core.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        readonly ILocationService locationService;
        readonly INotificationService notificationService;
        readonly AuthService authService;

        public event EventHandler<bool> OnRequestedPermissions;

        public MainViewModel(ILocationService locationService, AuthService authService, INotificationService notificationService)
        {
            this.locationService = locationService;
            this.authService = authService;
            this.notificationService = notificationService;
        }

        public override async void ViewAppeared()
        {
            base.ViewAppeared();


        }

        public override void ViewDisappearing()
        {
            base.ViewDisappearing();
            locationService.StopLocationUpdates();
        }
    }
}
