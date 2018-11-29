using System;

namespace Edison.Core.Common.Models
{
    public class ReportCreationModel
    {
        public DateTime? MinimumDate { get; set; }
        public DateTime? MaximumDate { get; set; }
        public ReportCreationType Type { get; set; }
        public string Filename { get; set; }
    }
}
