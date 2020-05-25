using System.Data;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Web.Admin.Components
{
    public static class ExcelControllerExtensions
    {
        public static ActionResult Excel(this Controller controller, IDataReader reader, string fileName)
        {
            return new ExcelResult(reader, fileName, null, null, null);
        }

        public static ActionResult Excel
            (
            this Controller controller,
            IDataReader reader,
            string fileName,
            TableStyle tableStyle,
            TableItemStyle headerStyle,
            TableItemStyle itemStyle
            )
        {
            return new ExcelResult(reader, fileName, tableStyle, headerStyle, itemStyle);
        }
    }
}