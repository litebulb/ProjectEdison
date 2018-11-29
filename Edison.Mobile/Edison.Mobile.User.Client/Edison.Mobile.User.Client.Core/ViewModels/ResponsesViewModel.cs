using System.Collections.ObjectModel;
using System.Linq;
using Autofac;
using System.Threading.Tasks;
using Edison.Mobile.User.Client.Core.CollectionItemViewModels;
using Edison.Mobile.User.Client.Core.Network;
using Edison.Mobile.Common.Ioc;
using Edison.Mobile.Common.ViewModels;

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

                var isSafetyCheckRequired = responses.Any(r => r.AcceptSafeStatus);
                if (isSafetyCheckRequired) 
                {
                    var chatViewModel = Container.Instance.Resolve<ChatViewModel>();
                    chatViewModel.ChatPromptTypes.Add(Shared.ChatPromptType.SafetyCheck);
                }
            }
        }
    }
}
