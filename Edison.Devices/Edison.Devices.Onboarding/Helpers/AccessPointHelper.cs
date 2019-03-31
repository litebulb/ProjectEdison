using System;
using System.Linq;

namespace Edison.Devices.Onboarding.Helpers
{
    public sealed class AccessPointHelper
    {
        private readonly static Guid AccessPointId = new Guid("c9df83d7-5c53-4c59-8b03-ff6ace20d370");
        private static OnboardingAccessPoint _AccessPoint = null;

        private static void InitAccessPoint()
        {
            AdapterInfo adapter = null;

            try
            {
                if (SecretManager.AccessPointSsid == Common.Helpers.SharedConstants.DEFAULT_AP_SSID)
                {
                    var listAdapters = AdaptersHelper.GetAdapters();
                    adapter = listAdapters.FindAll(p => p.Type == AdapterType.Wifi).OrderBy(p => p.Name).FirstOrDefault();
                    if (adapter == null)
                        adapter = listAdapters.OrderBy(p => p.Name).FirstOrDefault();

                    if (adapter == null)
                        throw new Exception("Could not retrieve MAC address");

                    SecretManager.AccessPointSsid = $"{Common.Helpers.SharedConstants.DEFAULT_AP_SSID}_{adapter.RawMAC}";
                }
            }
            catch (Exception e)
            {
                DebugHelper.LogError($"Could not retrieve MAC address: {e.Message}");
                return;
            }

            DebugHelper.LogInformation($"Access Point Id: {SecretManager.AccessPointSsid}");

            if (_AccessPoint != null)
                StopAccessPoint();
            _AccessPoint = new OnboardingAccessPoint(SecretManager.AccessPointSsid, SecretManager.AccessPointPassword, AccessPointId);
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
