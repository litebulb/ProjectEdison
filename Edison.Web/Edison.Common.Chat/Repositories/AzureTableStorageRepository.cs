using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Edison.Common.Chat.Repositories
{
    public class AzureTableStorageRepository
    {      
        public static CloudTable GetTable(string connectionString, string tableName)
        {
            CloudStorageAccount cloudStorageAccount = null;

            try
            {
                cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            }
            catch
            {
                throw;
            }

            CloudTableClient cloudTableClient = cloudStorageAccount?.CreateCloudTableClient();
            return cloudTableClient?.GetTableReference(tableName);
        }
        
        public static bool Insert<T>(CloudTable cloudTable, T entryToInsert) where T : TableEntity
        {
            TableOperation insertOperation = TableOperation.Insert(entryToInsert);
            TableResult insertResult = null;

            try
            {
                insertResult = cloudTable.ExecuteAsync(insertOperation).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to insert the given entity into the table: {e.Message}");
                return false;
            }

            return (insertResult?.Result != null);
        }
      
        public static bool DeleteEntry<T>(CloudTable cloudTable, string partitionKey, string rowKey) where T : TableEntity
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            TableResult retrieveResult = cloudTable.ExecuteAsync(retrieveOperation).GetAwaiter().GetResult();

            if (retrieveResult.Result is T entityToDelete)
            {
                TableOperation deleteOperation = TableOperation.Delete(entityToDelete);
                cloudTable.ExecuteAsync(deleteOperation).GetAwaiter().GetResult();
                return true;
            }

            return false;
        }
    }
}
