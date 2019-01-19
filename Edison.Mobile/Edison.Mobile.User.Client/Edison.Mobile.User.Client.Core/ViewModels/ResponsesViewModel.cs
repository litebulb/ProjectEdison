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

        public EventHandler<int> ResponseUpdated;
        public event EventHandler<string> OnCurrentAlertCircleColorChanged;

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


        public ResponsesViewModel(ResponseRestService responseRestService)
        {
            this.responseRestService = responseRestService;
        }

        public override async void ViewAppeared()
        {
            base.ViewAppeared();

            if (Responses.Count == 0)
                await GetResponses();
        }

        public async Task GetResponses()
        {
            var responses = await responseRestService.GetResponses();
            if (responses != null)
            {
                Responses.ReplaceRange(0, Responses.Count, responses.Select(r => new ResponseCollectionItemViewModel(r)));
                // get the details for each response in the background
                UpdateResponseDetails(0, Responses.Count);

                var isSafetyCheckRequired = responses.Any(r => r.AcceptSafeStatus);
                if (isSafetyCheckRequired)
                {
                    var chatViewModel = Container.Instance.Resolve<ChatViewModel>();
                    chatViewModel.ChatPromptTypes.Add(ChatPromptType.SafetyCheck);
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


        private void UpdateResponseDetails(int startIndex, int count)
        {
            Task.Run(async () =>
            {
                await GetResponseDetailsAsync(startIndex, count);
            });
        }
        private async Task GetResponseDetailsAsync(int startIndex, int count)
        {
            for (int i = startIndex; i < startIndex + count; i++)
            {
                await Responses[i].GetResponse();
                ResponseUpdated?.Invoke(null, i);
            }
        }
    }
}