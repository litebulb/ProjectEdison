using System;
using System.Collections.Generic;
using System.Text;

namespace Edison.Repositories.Config
{
    public class CosmosDBOptions
    {
        public string Endpoint { get; set; }
        public string AuthKey { get; set; }
        public string Database { get; set; }
        public string Collection { get; set; }


        public CosmosDBCollections Collections { get; set; }

        //public string TemplatesBase { get; set; }
    }
}
