using System.Collections.Generic;

namespace Edison.Api.Config
{
    public class WebApiOptions
    {
        public BoundariesOptions Boundaries { get; set; }
        public ReportOptions ReportConfiguration { get; set; }
        public List<string> ClaimsId { get; set; }
    }
}
