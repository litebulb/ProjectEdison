using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages.Interfaces
{
    public interface IResponseUIUpdateRequested : IMessage
    {
        Guid ResponseId { get; set; }
        ResponseUIModel ResponseUI { get; set; }
    }
}
