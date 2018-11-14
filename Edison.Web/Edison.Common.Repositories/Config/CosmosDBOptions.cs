namespace Edison.Common.Config
{
    public class CosmosDBOptions
    {
        public string Endpoint { get; set; }
        public string AuthKey { get; set; }
        public string Database { get; set; }
        public string Collection { get; set; }


        public CosmosDBCollections Collections { get; set; }
    }
}
