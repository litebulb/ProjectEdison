using IoTOnboardingUtils;
using System;

namespace Edison.Devices.Onboarding.Helpers
{
    public sealed class AccessPointHelper
    {
        private readonly static Guid AccessPointId = new Guid("c9df83d7-5c53-4c59-8b03-ff6ace20d370");
        private static OnboardingAccessPoint _AccessPoint = null;

        private static void InitAccessPoint()
        {
            
            string mac = MACFinder.GetWiFiAdapterMAC();
            if (_AccessPoint != null)
                StopAccessPoint();
            _AccessPoint = new OnboardingAccessPoint($"{SecretManager.AccessPointSsid}_{mac}", SecretManager.AccessPointPassword, AccessPointId);
        }

        public static void StartAccessPoint()
        {
            DebugHelper.LogInformation("Starting Access Point...");
            if (_AccessPoint == null)
                InitAccessPoint();
            _AccessPoint.Start();
            DebugHelper.LogInformation("Access Point started.");
        }

        public static void StopAccessPoint()
        {
            DebugHelper.LogInformation("Stopping Access Point...");
            _AccessPoint.Stop();
            _AccessPoint = null;
            DebugHelper.LogInformation("Access Point stopping.");
        }
    }
}
