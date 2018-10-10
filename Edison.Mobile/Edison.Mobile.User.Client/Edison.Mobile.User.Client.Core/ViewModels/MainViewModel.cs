using System;
using Edison.Mobile.Common.Auth;
using Edison.Mobile.Common.Geolocation;
using Edison.Mobile.Common.Shared;

namespace Edison.Mobile.User.Client.Core.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        readonly ILocationService locationService;
        readonly AuthService authService;

        public MainViewModel(ILocationService locationService, AuthService authService)
        {
            this.locationService = locationService;
            this.authService = authService;
        }

        public override void ViewCreated()
        {
            base.ViewCreated();
            locationService.RequestLocationPrivileges();
        }

        public override async void ViewAppearing()
        {
            base.ViewAppearing();
            locationService.OnLocationChanged += OnLocationChanged;
            await locationService.StartLocationUpdates();
        }

        public override void ViewDisappeared()
        {
            base.ViewDisappeared();
            locationService.StopLocationUpdates();
            locationService.OnLocationChanged -= OnLocationChanged;
        }

        void OnLocationChanged(object sender, LocationChangedEventArgs e)
        {
            Console.WriteLine(e);
        }
    }
}
