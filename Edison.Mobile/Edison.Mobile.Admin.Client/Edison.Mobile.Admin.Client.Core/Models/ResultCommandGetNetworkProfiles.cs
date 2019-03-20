using System;
namespace Edison.Mobile.Admin.Client.Core.Models
{
    public sealed class ResultCommandGetNetworkProfiles : ResultCommand
    {
        public IPAdapters IPAdapters { get; set; }
    }
}
