using Edison.Core.Common.Models;
using Microsoft.Azure.NotificationHubs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Edison.Core.Interfaces
{
    public interface ITwilioRestService
    {
        Task<TwilioModel> EmergencyCall(TwilioModel model);
        Task<string> Interconnect();
    }
}
