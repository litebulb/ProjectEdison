using System;
using System.Collections.Generic;

namespace Edison.Mobile.Admin.Client.Core.Models
{
    public class ResultCommandAvailableNetworks : ResultCommand
    {
        public IEnumerable<string> Networks { get; set; }
    }
}
