using Windows.ApplicationModel.Background;
using Edison.Devices.Onboarding.Helpers;

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
            
            //Start background task
            var app = new OnboardingServer();
            await app.Run();

            //Task completed
            deferral.Complete();
        }
    }
}
