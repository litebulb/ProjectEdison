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
using Edison.Mobile.Common.Geo;

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
                    bool Test = true;
#endif
                }
                else
                {
#if DEBUG
                    bool Test = true;
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
                        bool Test = true;
#endif
                    }
                    else
                    {
#if DEBUG
                        bool Test = true;
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


        void OnLocationChanged(object sender, LocationChangedEventArgs e)
        {
            UserLocation = e.CurrentLocation;
            LocationChanged?.Invoke(sender, e);
        }

    }
}