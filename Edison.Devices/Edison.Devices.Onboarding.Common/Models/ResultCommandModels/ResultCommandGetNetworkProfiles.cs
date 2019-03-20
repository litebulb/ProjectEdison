using System;

namespace Edison.Devices.Onboarding.Common.Models
{
    public sealed class ResultCommandGetNetworkProfiles : ResultCommand
    {
        public IPAdapters IPAdapters { get; set; }
    }
}
