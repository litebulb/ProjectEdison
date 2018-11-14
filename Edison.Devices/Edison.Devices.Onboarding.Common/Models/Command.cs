namespace Edison.Devices.Onboarding.Common.Models
{
    public sealed class Command
    {
        public Command() { }
        public CommandsEnum BaseCommand { get; set; }
        public string Data { get; set; }
    }
}
