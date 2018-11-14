using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Edison.Common.Interfaces;

namespace Edison.Common
{
    public abstract class CosmosDBBaseRepository
    {
        protected readonly ILogger<CosmosDBBaseRepository> _logger;
        protected static DocumentClient _client;
        protected string _databaseId;
        protected string _collectionId;
        private bool isInitialized = false;

        protected CosmosDBBaseRepository(string endPoint, string authKey, string databaseId, string collectionId, ILogger<CosmosDBBaseRepository> logger)
        {
            _logger = logger;
            _databaseId = databaseId;
            _collectionId = collectionId;
            InitializeClient(endPoint, authKey);
        }

        private static DocumentClient InitializeClient(string endPoint, string authKey)
        {
            if (_client == null)
            {
                Uri endpointUri = new Uri(endPoint);
                _client = new DocumentClient(endpointUri, authKey);
            }
            return _client;
        }

        protected async Task EnsureDatabaseAndCollectionExists()
        {
            if (!isInitialized)
            {
                await CreateDatabaseIfNotExists();
                await CreateCollectionIfNotExists();
                isInitialized = true;
            }
        }

        private async Task CreateDatabaseIfNotExists()
        {
            try
            {
                await _client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(_databaseId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await _client.CreateDatabaseAsync(new Database { Id = _databaseId });
                }
                else
                {
                    _logger.LogError($"CreateDatabaseIfNotExists: {e.Message}");
                    throw;
                }
            }
        }

        private async Task CreateCollectionIfNotExists()
        {
            try
            {
                await _client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await _client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(_databaseId),
                        new DocumentCollection { Id = _collectionId },
                        new RequestOptions { OfferThroughput = 1000 });
                }
                else
                {
                    _logger.LogError($"CreateCollectionIfNotExists: {e.Message}");
                    throw;
                }
            }
        }
    }
}
