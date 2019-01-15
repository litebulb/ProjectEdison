using System;
namespace Edison.Mobile.Admin.Client.Core.Models
{
    public sealed class ResultCommandGetDeviceId : ResultCommand
    {
        public Guid DeviceId { get; set; }
    }
}
