using NPOI.SS.UserModel;

namespace Edison.Api.Config
{
    public class ReportColumnOptions
    {
        public string Name { get; set; }
        public int ColumnIndex { get; set; }
        public string DisplayName { get; set; }
        public bool IsHidden { get; set; }
        public string BackgroundColor { get; set; }
        public string Color { get; set; }
        public int Width { get; set; }
    }
}
