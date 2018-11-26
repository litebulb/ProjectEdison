using Edison.Core.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Edison.Core.Common.Models
{
    public class ActionCloseUIModel : IUIUpdateSignalR
    {
        public string UpdateType { get; set; }
        public Guid ResponseId { get; set; }
        public Guid ActionId { get; set; }
        public bool IsSuccessful { get; set; }
        public bool IsSkipped { get; set; }
        public string ErrorMessage { get; set; }
    }
}
