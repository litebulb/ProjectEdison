using System;
using System.Collections.Generic;
using System.Text;

namespace Edison.Core.Common.Interfaces
{
    public interface IActionModel
    {
        Guid ActionId { get; set; }
        string ActionType { get; set; }
        bool IsActive { get; set; }
        string Description { get; set; }
        Dictionary<string, string> Parameters { get; set; }
    }
}
