using System.Collections.Generic;

namespace Edison.Common.Config
{
    public class CosmosDBPageResponse<T>
    {
        public IEnumerable<T> List { get; set; }
        
        public string ContinuationToken { get; set; }
    }
}
