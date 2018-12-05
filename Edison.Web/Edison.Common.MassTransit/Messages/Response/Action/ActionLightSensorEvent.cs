using Edison.Core.Common.Models;

namespace Edison.Common.Messages.Interfaces
{
    public class ActionLightSensorEvent : ActionBase, IActionLightSensorEvent
    {
        public ActionLightSensorEvent(ResponseActionModel model) : base(model) { }
        public double PrimaryRadius { get; set; }
        public double SecondaryRadius { get; set; }
        public Geolocation GeolocationPoint { get; set; }
        public string RadiusType { get { return GetProperty<string>("radius"); } set { SetProperty("radius", value); } }
        public int FlashFrequency { get { return GetProperty<int>("flashfrequency"); } set { SetProperty("flashfrequency", value.ToString()); } }
        public string Color { get { return GetProperty<string>("color"); } set { SetProperty("color", value); } }
        public string State { get { return GetProperty<string>("state"); } set { SetProperty("state", value); } }
        public string LightState { get { return GetProperty<string>("lightstate"); } set { SetProperty("lightstate", value); } }
    }
}
