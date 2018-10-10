using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Edison.Simulators.Sensors.Models
{
    public class StoryMessage
    {
        public string DeviceId { get; set; }
        public int Timestamp { get; set; }
        public string EventType { get; set; }
    }
}