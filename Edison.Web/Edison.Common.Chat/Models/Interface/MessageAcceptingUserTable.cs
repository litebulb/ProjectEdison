
using Microsoft.WindowsAzure.Storage.Table;

namespace Edison.Common.Chat.Models
{

    public class MessageAcceptingUserTable : TableEntity
    {
        public MessageAcceptingUserTable() { }

        public bool IsAcceptingMessage { get; set; }

        public MessageAcceptingUserTable(string userName, bool isAccept)
        {
            PartitionKey = userName;
            RowKey = userName;
            IsAcceptingMessage = isAccept;
        }
    }
}

