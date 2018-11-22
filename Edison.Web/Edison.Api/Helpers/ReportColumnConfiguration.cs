using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Edison.Api.Helpers
{
    public class ReportColumnConfiguration
    {
        public int ColumnIndex { get; set; }
        public XSSFCellStyle HeaderStyle { get; set; }
        public XSSFCellStyle BodyStyle { get; set; }
    }
}
