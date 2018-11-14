using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages.Interfaces
{
    public interface IActionLightSensorEvent : IMessage, IActionBaseEvent
    {
        double PrimaryRadius { get; set; }
        double SecondaryRadius { get; set; }
        Geolocation Epicenter { get; set; }
        string RadiusType { get; set; }
        int FlashFrequency { get; set; }
        string Color { get; set; }
        string State { get; set; }
    }
}
