using Edison.Devices.Onboarding.Helpers;
using Edison.Devices.Onboarding.Common.Helpers;
using System;
using System.Threading.Tasks;
using System.Net.NetworkInformation;

namespace Edison.Devices.Onboarding.Services
{
    internal abstract class BaseDeviceApiServer
    {
        private const uint BufferSize = 8192;

        protected async Task<NetworkInterface> WaitForAPInterface()
        {
            NetworkInterface networkAP = GetAPNetworkInterface();
            while(networkAP == null || networkAP.OperationalStatus != OperationalStatus.Up)
            {
                await Task.Delay(1000);
                DebugHelper.LogVerbose($"Waiting for AP network to be up...");
                networkAP = GetAPNetworkInterface();
            }
            return networkAP;
        }

        protected NetworkInterface GetAPNetworkInterface()
        {
            try
            {
                foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    {
                        foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork &&
                                ip.Address.ToString() == SharedConstants.DEVICE_HOST_API_IP)
                            {
                                return ni;
                            }
                        }
                    }
                }
            } catch(Exception e)
            {
                DebugHelper.LogError($"GetAPNetworkInterface: {e.Message}");
                DebugHelper.LogError($"GetAPNetworkInterface: {e.StackTrace}");
            }
            return null;
        }
    }
}
