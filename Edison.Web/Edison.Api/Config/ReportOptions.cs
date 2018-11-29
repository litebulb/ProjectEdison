using System.Collections.Generic;
using NPOI.SS.UserModel;

namespace Edison.Api.Config
{
    public class ReportOptions
    {
        public List<ReportResponseHeaderRowOptions> ResponseHeader { get; set; }
        public List<ReportColumnOptions> UsersReport { get; set; }
        public List<ReportColumnOptions> ConversationsReport { get; set; }
        public List<ReportColumnOptions> EventsReport { get; set; }
        public int DefaultWidth { get; set; }
    }

    public enum ReportDataType
    {
        Unknown = 0,
        Text = 1,
        Integer = 2,
        Date = 3,
        Double = 4,
        RichText = 5
    }

    public class ReportColumnOptions
    {
        public string Name { get; set; }
        public int ColumnIndex { get; set; }
        public string HeaderName { get; set; }
        public bool IsHidden { get; set; }
        public string DataFormat { get; set; }
        public ReportStyleCell HeaderStyle { get; set; }
        public ReportStyleCell RowStyle { get; set; }
        public int Width { get; set; }
    }

    public class ReportResponseHeaderRowOptions
    {
        public int RowIndex { get; set; }
        public List<ReportResponseHeaderColumnOptions> Columns { get; set; }
    }

    public class ReportResponseHeaderColumnOptions
    {
        public int ColumnIndex { get; set; }
        public string Value { get; set; }
        public ReportStyleCell Style { get; set; }
    }

    public class ReportStyleCell
    {
        public string BackgroundColor { get; set; }
        public string FontColor { get; set; }
        public FontBoldWeight FontWeight { get; set; }
        public HorizontalAlignment HorizontalAlignment { get; set; }
        public string DataFormat { get; set; }
        public bool WrapText { get; set; }
    }
}
