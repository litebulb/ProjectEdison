using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Edison.Devices.Onboarding.Helpers
{
    internal class AdaptersHelper
    {
        const int MAX_ADAPTER_DESCRIPTION_LENGTH = 128;
        const int ERROR_BUFFER_OVERFLOW = 111;
        const int MAX_ADAPTER_NAME_LENGTH = 256;
        const int MAX_ADAPTER_ADDRESS_LENGTH = 8;
        const int MIB_IF_TYPE_OTHER = 1;
        const int MIB_IF_TYPE_ETHERNET = 6;
        const int MIB_IF_TYPE_TOKENRING = 9;
        const int MIB_IF_TYPE_FDDI = 15;
        const int MIB_IF_TYPE_PPP = 23;
        const int MIB_IF_TYPE_LOOPBACK = 24;
        const int MIB_IF_TYPE_SLIP = 28;
        const int MIB_IF_TYPE_WIFI = 71;

        [DllImport("iphlpapi.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
        public static extern int GetAdaptersInfo(IntPtr pAdapterInfo, ref Int64 pBufOutLen);

        public static List<AdapterInfo> GetAdapters()
        {
            var adapters = new List<AdapterInfo>();

            long structSize = Marshal.SizeOf(typeof(IP_ADAPTER_INFO));
            IntPtr pArray = Marshal.AllocHGlobal(new IntPtr(structSize));

            int ret = GetAdaptersInfo(pArray, ref structSize);

            if (ret == ERROR_BUFFER_OVERFLOW) // ERROR_BUFFER_OVERFLOW == 111
            {
                // Buffer was too small, reallocate the correct size for the buffer.
                pArray = Marshal.ReAllocHGlobal(pArray, new IntPtr(structSize));

                ret = GetAdaptersInfo(pArray, ref structSize);
            }

            if (ret == 0)
            {
                // Call Succeeded
                IntPtr pEntry = pArray;

                do
                {
                    var adapter = new AdapterInfo();

                    // Retrieve the adapter info from the memory address
                    var entry = (IP_ADAPTER_INFO)Marshal.PtrToStructure(pEntry, typeof(IP_ADAPTER_INFO));

                    // Adapter Type
                    switch (entry.Type)
                    {
                        case MIB_IF_TYPE_ETHERNET:
                            adapter.Type = AdapterType.Ethernet;
                            break;
                        case MIB_IF_TYPE_TOKENRING:
                            adapter.Type = AdapterType.TokenRing;
                            break;
                        case MIB_IF_TYPE_FDDI:
                            adapter.Type = AdapterType.FDDI;
                            break;
                        case MIB_IF_TYPE_PPP:
                            adapter.Type = AdapterType.PPP;
                            break;
                        case MIB_IF_TYPE_LOOPBACK:
                            adapter.Type = AdapterType.Loopback;
                            break;
                        case MIB_IF_TYPE_SLIP:
                            adapter.Type = AdapterType.Slip;
                            break;
                        case MIB_IF_TYPE_WIFI:
                            adapter.Type = AdapterType.Wifi;
                            break;
                        default:
                            adapter.Type = AdapterType.Unknown;
                            break;
                    } // switch

                    adapter.Name = entry.AdapterName;
                    adapter.Description = entry.AdapterDescription;

                    // MAC Address (data is in a byte[])
                    adapter.MAC = string.Join("-", Enumerable.Range(0, (int)entry.AddressLength).Select(s => string.Format("{0:X2}", entry.Address[s])));
                    adapter.RawMAC = string.Join("", Enumerable.Range(0, (int)entry.AddressLength).Select(s => string.Format("{0:X2}", entry.Address[s])));

                    // Get next adapter (if any)

                    adapters.Add(adapter);

                    pEntry = entry.Next;
                }
                while (pEntry != IntPtr.Zero);

                Marshal.FreeHGlobal(pArray);
            }
            else
            {
                Marshal.FreeHGlobal(pArray);
                throw new InvalidOperationException("GetAdaptersInfo failed: " + ret);
            }

            return adapters;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct IP_ADAPTER_INFO
        {
            public IntPtr Next;
            public Int32 ComboIndex;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_ADAPTER_NAME_LENGTH + 4)]
            public string AdapterName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_ADAPTER_DESCRIPTION_LENGTH + 4)]
            public string AdapterDescription;
            public UInt32 AddressLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_ADAPTER_ADDRESS_LENGTH)]
            public byte[] Address;
            public Int32 Index;
            public UInt32 Type;
            public UInt32 DhcpEnabled;
            public IntPtr CurrentIpAddress;
            public IP_ADDR_STRING IpAddressList;
            public IP_ADDR_STRING GatewayList;
            public IP_ADDR_STRING DhcpServer;
            public bool HaveWins;
            public IP_ADDR_STRING PrimaryWinsServer;
            public IP_ADDR_STRING SecondaryWinsServer;
            public Int32 LeaseObtained;
            public Int32 LeaseExpires;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct IP_ADDR_STRING
        {
            public IntPtr Next;
            public IP_ADDRESS_STRING IpAddress;
            public IP_ADDRESS_STRING IpMask;
            public Int32 Context;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct IP_ADDRESS_STRING
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string Address;
        }
    }

    internal class AdapterInfo
    {
        public AdapterType Type { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }

        public string MAC { get; set; }
        public string RawMAC { get; set; }
    }

    internal enum AdapterType
    {
        Unknown = 0,
        Ethernet = 1,
        TokenRing = 2,
        FDDI = 3,
        PPP = 4,
        Loopback = 5,
        Slip = 6,
        Wifi = 7
    }
}
