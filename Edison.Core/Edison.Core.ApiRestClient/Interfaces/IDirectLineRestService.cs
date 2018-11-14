using Edison.Core.Common.Models;
using System.Threading.Tasks;

namespace Edison.Core.Interfaces
{
    public interface IDirectLineRestService
    {
        Task<TokenConversationResult> GenerateToken(TokenConversationParameters tokenParameters);
    }
}
