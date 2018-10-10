using Edison.Core.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Edison.Core.Common.Models
{
    public class ResponseUIModel : IUIUpdateSignalR
    {
        public string UpdateType { get; set; }
        public Guid ResponseId { get; set; }
        public ResponseModel Response { get; set; }
    }
}
