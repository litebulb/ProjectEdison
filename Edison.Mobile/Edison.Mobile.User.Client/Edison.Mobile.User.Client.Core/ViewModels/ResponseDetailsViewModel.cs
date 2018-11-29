using System;
using System.Linq;
using System.Collections.ObjectModel;
using Edison.Core.Common.Models;
using Edison.Mobile.Common.Geo;
using Edison.Mobile.User.Client.Core.Network;
using Edison.Mobile.Common.ViewModels;

namespace Edison.Mobile.User.Client.Core.ViewModels
{
    public class ResponseDetailsViewModel : BaseViewModel
    {
        readonly ILocationService locationService;
        readonly ResponseRestService responseRestService;

        public ResponseModel Response { get; set; }
        public ObservableRangeCollection<NotificationModel> Notifications { get; set; } = new ObservableRangeCollection<NotificationModel>();

        public event EventHandler<LocationChangedEventArgs> OnLocationChanged;

        public ResponseDetailsViewModel(ILocationService locationService, ResponseRestService responseRestService)
        {
            this.locationService = locationService;
            this.responseRestService = responseRestService;
        }

        public async override void ViewAppearing()
        {
            base.ViewAppearing();

            OnLocationChanged?.Invoke(this, new LocationChangedEventArgs
            {
                CurrentLocation = locationService.LastKnownLocation,
            });

            var responseId = Response?.ResponseId.ToString();
            if (!string.IsNullOrEmpty(responseId))
            {
                var notifications = await responseRestService.GetNotifications(responseId);
                if (notifications != null) 
                {
                    Notifications.AddRange(notifications.OrderByDescending(n => n.CreationDate));
                }
            }
        }

        public override void BindEventHandlers()
        {
            base.BindEventHandlers();

            locationService.OnLocationChanged += HandleOnLocationChanged;
        }

        public override void UnBindEventHandlers()
        {
            base.UnBindEventHandlers();

            locationService.OnLocationChanged -= HandleOnLocationChanged;
        }

        void HandleOnLocationChanged(object sender, LocationChangedEventArgs e)
        {
            OnLocationChanged?.Invoke(this, e);
        }
    }
}
