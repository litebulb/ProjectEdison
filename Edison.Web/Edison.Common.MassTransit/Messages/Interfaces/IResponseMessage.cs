using Edison.Core.Common.Models;

namespace Edison.Common.Messages.Interfaces
{
    public interface IResponseMessage
    {
        ResponseModel Response { get; set; }
        ActionStatus ActionStatus { get; set; }
    }
}
