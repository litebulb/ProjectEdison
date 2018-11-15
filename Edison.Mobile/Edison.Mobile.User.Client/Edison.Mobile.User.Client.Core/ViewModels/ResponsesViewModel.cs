using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Edison.Mobile.Common.Network;
using Edison.Mobile.Common.Shared;
using Edison.Mobile.User.Client.Core.CollectionItemViewModels;
using Edison.Mobile.User.Client.Core.Network;

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
                await GetResponses();
            }
        }

        public async Task GetResponses() 
        {
            var responses = await responseRestService.GetResponses();
            if (responses != null)
            {
                Responses.AddRange(responses.Select(r => new ResponseCollectionItemViewModel(r)));
            }
        }
    }
}
