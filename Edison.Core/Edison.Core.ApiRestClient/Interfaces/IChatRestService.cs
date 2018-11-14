using Edison.Core.Common.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Edison.Core.Interfaces
{
    public interface IChatRestService
    {
        Task<ChatUserTokenContext> GetToken();
    }
}
