using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edison.Devices.Onboarding.Common.Models
{
    public sealed class ResultCommandEncryptionStatus : ResultCommand
    {
        public bool Enabled { get; set; }
    }
}
