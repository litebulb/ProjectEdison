using System;
using System.Collections.Generic;
using System.Text;

namespace Edison.Core.Common.Interfaces
{
    public interface IActionModel
    {
        string ActionType { get; set; }
        string Description { get; set; }
        Dictionary<string, string> Parameters { get; set; }
    }
}
