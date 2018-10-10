using Edison.Core.Common.Models;
using Edison.Common.Messages.Interfaces;
using System;

namespace Edison.Common.Messages.Interfaces
{
    public class ActionLightSensorEvent : ActionBase, IActionLightSensorEvent
    {
        public ActionLightSensorEvent(ActionModel model) : base(model) { }
        public double PrimaryRadius { get; set; }
        public double SecondaryRadius { get; set; }
        public Geolocation Epicenter { get; set; }
        public string RadiusType { get { return GetProperty<string>("radius"); } set { SetProperty("radius", value); } }
        public int FlashFrequency { get { return GetProperty<int>("flashfrequency"); } set { SetProperty("flashfrequency", value.ToString()); } }
        public string Color { get { return GetProperty<string>("color"); } set { SetProperty("color", value); } }
        public string State { get { return GetProperty<string>("state"); } set { SetProperty("state", value); } }
    }
}
