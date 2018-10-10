using Edison.Common.Messages.Interfaces;
using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages
{
    public class DeleteById : IDeleteById
    {
        public Guid Id { get; set; }
    }
}
