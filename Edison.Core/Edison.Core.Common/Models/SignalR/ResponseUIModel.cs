using Edison.Core.Common.Interfaces;
using System;

namespace Edison.Core.Common.Models
{
    public class ResponseUIModel : IUIUpdateSignalR
    {
        public string UpdateType { get; set; }
        public Guid ResponseId { get; set; }
        public ResponseModel Response { get; set; }
    }
}
