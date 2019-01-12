using Newtonsoft.Json;

namespace Edison.Devices.Onboarding.Common.Models
{
    public sealed class Command
    {
        public Command() { }
        public CommandsEnum BaseCommand { get; set; }
        public object Data { get; set; }

        public static Command CreateErrorCommand(string message)
        {
            return new Command()
            {
                BaseCommand = CommandsEnum.ResultError,
                Data = ResultCommand.CreateFailedCommand(message)
            };
        }
    }
}
