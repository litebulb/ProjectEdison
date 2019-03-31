using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Net.Wifi;
using Edison.Mobile.Common.WiFi;
using System.Linq;
using System.Text;

namespace Edison.Mobile.Android.Common.WiFi
{

    public class PlatformWifiService : IWifiService
    {
        private readonly WifiManager wifiManager;
        public PlatformWifiService()
        {
            wifiManager = (WifiManager)Application.Context.GetSystemService(Context.WifiService);
        }

        public event EventHandler<ConnectionFailedEventArgs> ConnectionFailed;

        public event EventHandler<CheckingConnectionStatusUpdatedEventArgs> CheckingConnectionStatusUpdated;
        private async Task<bool> Connect(int networkId, string ssid)
        {
            if(networkId == -1)
            {
                ConnectionFailed?.Invoke(this, new ConnectionFailedEventArgs("Network wasn't found"));
                Console.WriteLine("Network Id was -1");
                return false;
            }

            StringBuilder stringBuilder = new StringBuilder();
            bool connected;
            WifiInfo info = default(WifiInfo);
            int tryCount = 0;
            do
            {
                if (tryCount < 50)
                {
                    stringBuilder.Append(".");
                    CheckingConnectionStatusUpdated?.Invoke(this, new CheckingConnectionStatusUpdatedEventArgs($"Connecting{stringBuilder.ToString()}", ssid, false));
                }
                else
                {
                    ConnectionFailed?.Invoke(this, new ConnectionFailedEventArgs("Didn't connect quickly enough."));
                    return false;
                }

                connected = wifiManager.EnableNetwork(networkId, true);
                await Task.Delay(500);
                tryCount++;

                info = wifiManager.ConnectionInfo;                
            }
            while ((info.NetworkId != networkId || info.SupplicantState != SupplicantState.Completed));

            CheckingConnectionStatusUpdated?.Invoke(this, new CheckingConnectionStatusUpdatedEventArgs($"Connected", ssid, true));

            return true;
        }

        public async Task<bool> ConnectToWifiNetwork(string ssid, string passphrase)
        {   
            
            WifiConfiguration wifiConfig = new WifiConfiguration();
            wifiConfig.Ssid = string.Format("\"{0}\"", ssid);
            wifiConfig.PreSharedKey = string.Format("\"{0}\"", passphrase);            

            // Use ID
            var existing = wifiManager.ConfiguredNetworks.FirstOrDefault(i => i.Ssid == wifiConfig.Ssid);
            int netId = -1;
            if (existing != null)
            {
                netId = existing.NetworkId;
                wifiManager.UpdateNetwork(wifiConfig);                
            }
            else
            {
                netId = wifiManager.AddNetwork(wifiConfig);
            }

            return await Connect(netId, wifiConfig.Ssid);           
        }
        
        public async Task<bool> ConnectToWifiNetwork(string ssid)
        {

            if (ssid.IndexOf('"') == -1)
            {
                ssid = string.Format("\"{0}\"", ssid);
            }

            // Use ID
            var existing = wifiManager.ConfiguredNetworks.FirstOrDefault(i => i.Ssid == ssid);
            if(existing == null)
            {
                ConnectionFailed?.Invoke(this, new ConnectionFailedEventArgs("Network didn't exist"));
                return false;
            }

            int netId = existing.NetworkId;
            
            return await Connect(netId, ssid);
        }

        public async Task DisconnectFromWifiNetwork(WifiNetwork wifiNetwork)
        {
            wifiManager.Disconnect();
            await Task.FromResult(true);
            
        }

        public Task<WifiNetwork> GetCurrentlyConnectedWifiNetwork()
        {
            var network = new WifiNetwork()
            {
                SSID = wifiManager.ConnectionInfo.SSID
            };
            return Task.FromResult(network);
        }


    }
    public class WifiReciever : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            String action = intent.Action;
            if (action.Equals(WifiManager.SupplicantStateChangedAction))
            {
                SupplicantState supl_state = (SupplicantState)intent.GetParcelableExtra(WifiManager.ExtraNewState);

                //var supl_error = supl_state == SupplicantState.Completed;
                //if (supl_error == WifiManager.ErrorAuthenticating)
                //{
                //
                //}

                /*switch (supl_state.Name())
                {

                    case SupplicantState.Associated:

                        break;
                    case SupplicantState.Associating:

                        break;
                    case SupplicantState.Authenticating:

                        break;
                    case COMPLETED:
                        Log.i("SupplicantState", "Connected");
                        break;
                    case DISCONNECTED:
                        Log.i("SupplicantState", "Disconnected");
                        break;
                    case DORMANT:
                        Log.i("SupplicantState", "DORMANT");
                        break;
                    case FOUR_WAY_HANDSHAKE:
                        Log.i("SupplicantState", "FOUR_WAY_HANDSHAKE");
                        break;
                    case GROUP_HANDSHAKE:
                        Log.i("SupplicantState", "GROUP_HANDSHAKE");
                        break;
                    case INACTIVE:
                        Log.i("SupplicantState", "INACTIVE");
                        break;
                    case INTERFACE_DISABLED:
                        Log.i("SupplicantState", "INTERFACE_DISABLED");
                        break;
                    case INVALID:
                        Log.i("SupplicantState", "INVALID");
                        break;
                    case SCANNING:
                        Log.i("SupplicantState", "SCANNING");
                        break;
                    case UNINITIALIZED:
                        Log.i("SupplicantState", "UNINITIALIZED");
                        break;
                    default:
                        Log.i("SupplicantState", "Unknown");
                        break;

                }*/
                int supl_error = intent.GetIntExtra(WifiManager.ExtraSupplicantError, -1);
                if (supl_error == WifiManager.ErrorAuthenticating)
                {

                }
            }
        }
    }


}
