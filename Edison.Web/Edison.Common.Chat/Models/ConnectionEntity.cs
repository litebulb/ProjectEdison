using Microsoft.WindowsAzure.Storage.Table;

namespace Edison.Common.Chat.Models
{
    public class ConnectionEntity : TableEntity
    {
        public ConnectionEntity(){}

        public string Owner { get; set; }
        public string Client { get; set; }
     
        public ConnectionEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }
    }
}
