using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Edison.Api.Config
{
    public class ReportOptions
    {
        public List<ReportColumnOptions> ResponsesReport { get; set; }
        public List<ReportColumnOptions> EventsReport { get; set; }
    }
}
