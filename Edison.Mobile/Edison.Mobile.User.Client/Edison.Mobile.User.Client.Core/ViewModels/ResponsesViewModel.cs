using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Autofac;

using Edison.Core.Common.Models;
using Edison.Mobile.Common.Ioc;
using Edison.Mobile.Common.ViewModels;
using Edison.Mobile.Common.Geo;
using Edison.Mobile.User.Client.Core.CollectionItemViewModels;
using Edison.Mobile.User.Client.Core.Network;
using Edison.Mobile.User.Client.Core.LocalStorage;
using Edison.Mobile.User.Client.Core.Shared;





namespace Edison.Mobile.User.Client.Core.ViewModels
{
    public class ResponsesViewModel : BaseViewModel
    {

        public EventHandler<int> ResponseUpdated;
        public event EventHandler<string> OnCurrentAlertCircleColorChanged;
        public event EventHandler<LocationChangedEventArgs> LocationChanged;

        private readonly ILocationService _locationService;

        readonly ResponseRestService responseRestService;

        readonly string[] colorSeverity =
        {
               Constants.ColorName.Blue,
               Constants.ColorName.Yellow,
               Constants.ColorName.Red,
        };

        string currentAlertCircleColor;

        public List<Guid> PreviousResponseIds { get; private set; } = new List<Guid>();

        public List<Guid> CurrentResponseIds
        {
            get => Settings.CurrentResponseIds; 
            private set => Settings.CurrentResponseIds = value;
        }// = new List<Guid>();


        public List<ResponseLightModel> CurrentResponseSummaries
        {
            get => Settings.CurrentResponseSummaries;
            private set => Settings.CurrentResponseSummaries = value;
        }


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

        public EdisonLocation UserLocation { get; private set; }


        public ResponsesViewModel(ResponseRestService responseRestService)
        {
            this.responseRestService = responseRestService;
            _locationService = Container.Instance.Resolve<ILocationService>();

            // Start location service in case it is not already started
            Task.Run(async () =>
            {
                int timeoutMs = 1000;
                var task = _locationService.StartLocationUpdates();
                if (await Task.WhenAny(task, Task.Delay(timeoutMs)) == task)
                {
#if DEBUG
                    bool test = true;  // for use as breakpoint during testing
#endif
                }
                else
                {
#if DEBUG
                    bool test = true;  // for use as breakpoint during testing
#endif
                }
            });

            // Attempt to get the current user location
            UserLocation = _locationService.LastKnownLocation;
            if (UserLocation == null)
            {
                Task.Run(async () =>
                {
                    int timeoutMs = 1000;
                    var task = _locationService.GetLastKnownLocationAsync();
                    if (await Task.WhenAny(task, Task.Delay(timeoutMs)) == task)
                    {
                        UserLocation = await task;
#if DEBUG
                        bool test = true;  // for use as breakpoint during testing
#endif
                    }
                    else
                    {
#if DEBUG
                        bool test = true;  // for use as breakpoint during testing
#endif
                    }
                    //                   UserLocation = await _locationService.GetLastKnownLocationAsync();
                });
            }
        }

        public override void ViewCreated()
        {
            base.ViewCreated();
            BindEvents();
        }

        public override void ViewDestroyed()
        {
            UnBindEvents();
            base.ViewDestroyed();
        }


        public void BindEvents()
        {
            _locationService.OnLocationChanged += OnLocationChanged;
        }

        public void UnBindEvents()
        {
            _locationService.OnLocationChanged -= OnLocationChanged;
        }


        public override async void ViewAppeared()
        {
            base.ViewAppeared();

            if (Responses.Count == 0)
                await GetResponses();
        }

        public async Task GetResponses()
        {
            var responses = await GetResponsesAsync();
            if (responses != null)
            {
                PreviousResponseIds = CurrentResponseIds;

                Responses.ReplaceRange(0, Responses.Count, responses.Select(r => new ResponseCollectionItemViewModel(r)));
                // get the details for each response in the background
                UpdateResponseDetails(0, Responses.Count);

                var isSafetyCheckRequired = responses.Any(r => r.AcceptSafeStatus);
                if (isSafetyCheckRequired)
                {
                    var chatViewModel = Container.Instance.Resolve<ChatViewModel>();
                    chatViewModel.ChatPromptTypes.Add(ChatPromptType.SafetyCheck);
                }

                CurrentResponseIds = Responses.Select(i => i.Response.ResponseId).ToList();
                CurrentResponseSummaries = Responses.Select(i => i.ToLightModel()).ToList();
            }
        }

        private async Task<IEnumerable<ResponseLightModel>> GetResponsesAsync()
        {
            return await responseRestService.GetResponses().ConfigureAwait(false);
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


        public ResponseModel GetResponse(Guid responseId)
        {
            if (responseId == Guid.Empty) return null;
            var response = Responses.Where((r) => r.ResponseId == responseId).FirstOrDefault();
            return response?.Response;
        }
        public ResponseLightModel GetResponseSummary(Guid responseId)
        {
            if (responseId == Guid.Empty) return null;
            return CurrentResponseSummaries.Where((r) => r.ResponseId == responseId).FirstOrDefault();
        }



        void OnLocationChanged(object sender, LocationChangedEventArgs e)
        {
            UserLocation = e.CurrentLocation;
            LocationChanged?.Invoke(sender, e);
        }

    }
}