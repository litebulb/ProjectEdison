using System.Collections.ObjectModel;
using System.Linq;
using Autofac;
using System.Threading.Tasks;
using Edison.Mobile.User.Client.Core.CollectionItemViewModels;
using Edison.Mobile.User.Client.Core.Network;
using Edison.Mobile.Common.Ioc;
using Edison.Mobile.Common.ViewModels;
using Edison.Core.Common.Models;
using System;
using Edison.Mobile.User.Client.Core.Shared;

namespace Edison.Mobile.User.Client.Core.ViewModels
{
    public class ResponsesViewModel : BaseViewModel
    {
        readonly ResponseRestService responseRestService;
        readonly string[] colorSeverity =
        {
               Constants.ColorName.Blue,
               Constants.ColorName.Yellow,
               Constants.ColorName.Red,
        };

        string currentAlertCircleColor;

        public ObservableRangeCollection<ResponseCollectionItemViewModel> Responses { get; } = new ObservableRangeCollection<ResponseCollectionItemViewModel>();

        public string CurrentAlertCircleColor
        {
            get => currentAlertCircleColor;
            set
            {
                currentAlertCircleColor = value;
                OnCurrentAlertCircleColorChanged(this, currentAlertCircleColor);
            }
        }

        public event EventHandler<string> OnCurrentAlertCircleColorChanged;

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

        public void HandleResponseModelReceived(ResponseModel responseModel)
        {
            var prevSeverityIndex = GetColorSeverityIndex(CurrentAlertCircleColor);
            var newSeverityIndex = GetColorSeverityIndex(responseModel.Color);
            var newCurrentColor = colorSeverity[Math.Max(prevSeverityIndex, newSeverityIndex)];
            CurrentAlertCircleColor = newCurrentColor;
        }

        public int GetColorSeverityIndex(string color) => string.IsNullOrEmpty(color) ? 0 : Array.IndexOf(colorSeverity, color);
    }
}