using Edison.Core.Common.Models;

namespace Edison.Common.Messages.Interfaces
{
    public interface IActionLightSensorEvent : IMessage, IActionBaseEvent
    {
        double PrimaryRadius { get; set; }
        double SecondaryRadius { get; set; }
        Geolocation GeolocationPoint { get; set; }
        string RadiusType { get; set; }
        int FlashFrequency { get; set; }
        string Color { get; set; }
        string State { get; set; }
    }
}
