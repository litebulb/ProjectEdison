using System;

namespace Edison.Devices.Onboarding.Common.Models
{
    public sealed class ResultCommandGetDeviceId : ResultCommand
    {
        public Guid DeviceId { get; set; }
    }
}
