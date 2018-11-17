using System.Threading.Tasks;
using Edison.Devices.Onboarding.Common.Models;

namespace Edison.Devices.Onboarding.Client.UWP
{
    public interface IStreamClient
    {
        Task<U> SendCommand<T, U>(CommandsEnum requestCommandType, T parameters, string passkey) where T : RequestCommand where U : ResultCommand, new();
        Task<T> SendCommand<T>(CommandsEnum requestCommandType, string passkey) where T : ResultCommand, new();
    }
}
