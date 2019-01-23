namespace Edison.Mobile.Admin.Client.Core.Models
{
    public sealed class Command
    {
        public CommandsEnum BaseCommand { get; set; }
        public string Data { get; set; }
    }
}
