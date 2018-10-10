using Microsoft.Azure.Devices.Client;

namespace Edison.Devices.Common
{
    public class DirectMethodConfig
    {
        public string MethodName { get; set; }
        public MethodCallback CallbackDirectMethod { get; set; }
    }
}
