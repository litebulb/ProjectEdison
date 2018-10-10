using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages.Interfaces
{
    public interface IDeleteById : IMessage
    {
        Guid Id { get; set; }
    }
}
