using Windows.ApplicationModel.Background;
using Edison.Devices.Onboarding.Helpers;
using System;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;
using System.Text;
using System.Net;
using Edison.Devices.Onboarding.Services;

namespace Edison.Devices.Onboarding
{
    public sealed class StartupTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            //Deferral task to allow for async
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            //Init debugging channel
            DebugHelper.Init();

            //Init Portal ApI
            PortalApiHelper.Init("Administrator", "testTBU1");
            
            //Start background task
            var app = new OnboardingServer();
            await app.Run();

            //Task completed
            deferral.Complete();
        }
    }
}
