using System.Collections.ObjectModel;
using System.Linq;
using Edison.Mobile.Common.Network;
using Edison.Mobile.Common.Shared;
using Edison.Mobile.User.Client.Core.CollectionItemViewModels;

namespace Edison.Mobile.User.Client.Core.ViewModels
{
    public class ResponsesViewModel : BaseViewModel
    {
        readonly ResponseRestService responseRestService;

        public ObservableRangeCollection<ResponseCollectionItemViewModel> Responses { get; } = new ObservableRangeCollection<ResponseCollectionItemViewModel>();

        public ResponsesViewModel(ResponseRestService responseRestService)
        {
            this.responseRestService = responseRestService;
        }

        public override async void ViewAppeared()
        {
            base.ViewAppeared();

            if (Responses.Count == 0)
            {
                var responses = await responseRestService.GetResponses();
                if (responses != null)
                {
                    Responses.AddRange(responses.Select(r => new ResponseCollectionItemViewModel(r)));
                }
            }
        }
    }
}
