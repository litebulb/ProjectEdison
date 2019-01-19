using System;
using System.Threading.Tasks;
using Autofac;
using Edison.Core.Common.Models;
using Edison.Mobile.Common.Geo;
using Edison.Mobile.Common.Ioc;
using Edison.Mobile.Common.Shared;
using Edison.Mobile.Common.ViewModels;
using Edison.Mobile.User.Client.Core.Network;
using Edison.Mobile.User.Client.Core.ViewModels;

namespace Edison.Mobile.User.Client.Core.CollectionItemViewModels
{
    public class ResponseCollectionItemViewModel : BaseCollectionItemViewModel
    {

        public event ViewNotification ResponseReceived;
        public event EventHandler<LocationChangedEventArgs> LocationChanged;

        readonly ILocationService locationService;

        public Guid ResponseId { get; private set; }
        public ResponseModel Response { get; private set; }
        public Geolocation Geolocation { get; private set; }



        public ResponseCollectionItemViewModel(ResponseLightModel responseLightModel)
        {
            ResponseId = responseLightModel.ResponseId;
            Geolocation = responseLightModel.Geolocation;
            Response = new ResponseModel
            {
                Color = responseLightModel.Color,
                Icon = responseLightModel.Icon,
                Name = responseLightModel.Name,
                StartDate = responseLightModel.StartDate,
                EndDate = responseLightModel.EndDate,
                EventClusterIds = responseLightModel.EventClusterIds,
                Geolocation = responseLightModel.Geolocation,
                PrimaryEventClusterId = responseLightModel.PrimaryEventClusterId,
                ResponderUserId = responseLightModel.ResponderUserId,
                ResponseId = responseLightModel.ResponseId,
                ResponseState = responseLightModel.ResponseState
            };
            locationService = Container.Instance.Resolve<ILocationService>();
        }

        public async Task GetResponse() 
        {
            var responseRestService = Container.Instance.Resolve<ResponseRestService>();
            var responsesViewModel = Container.Instance.Resolve<ResponsesViewModel>();
            var response = await responseRestService.GetResponse(ResponseId);
            Response = response;
            ResponseReceived?.Invoke();
            responsesViewModel.HandleResponseModelReceived(response);
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
            LocationChanged?.Invoke(sender, e);
        }
    }
}
