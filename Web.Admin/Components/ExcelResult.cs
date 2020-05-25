using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web.Admin.Components
{
    public class ExcelResult : ActionResult
    {
        private readonly IDataReader _reader;
        private string[] _headers;
        private readonly string _fileName;

        private readonly TableStyle _tableStyle;
        private readonly TableItemStyle _headerStyle;
        private readonly TableItemStyle _itemStyle;

        public ExcelResult(IDataReader reader, string fileName, TableStyle tableStyle, TableItemStyle headerStyle, TableItemStyle itemStyle)
        {
            _reader = reader;
            _fileName = fileName;
            _tableStyle = tableStyle;
            _headerStyle = headerStyle;
            _itemStyle = itemStyle;

            // provide defaults
            if (_tableStyle == null)
            {
                _tableStyle = new TableStyle();
                _tableStyle.BorderStyle = BorderStyle.Solid;
                _tableStyle.BorderColor = Color.LightGray;
                _tableStyle.BorderWidth = Unit.Parse("1px");
            }
            if (_headerStyle == null)
            {
                _headerStyle = new TableItemStyle();
                _headerStyle.BackColor = Color.LightGray;
                _headerStyle.Font.Bold = true;
            }
        }

        private string[] GetHeaders()
        {
            var data = _reader.GetSchemaTable();
            List<string> headrs = new List<string>();
            for (int i = 0; i < data.Rows.Count; i++)
            {
                headrs.Add(data.Rows[i][0].ToString());
            }
            return headrs.ToArray();
        }

        public override void ExecuteResult(ControllerContext context)
        {
            // Create HtmlTextWriter
            StringWriter sw = new StringWriter();
            HtmlTextWriter tw = new HtmlTextWriter(sw);

            // Build HTML Table from Items
            if (_tableStyle != null)
                _tableStyle.AddAttributesToRender(tw);
            tw.RenderBeginTag(HtmlTextWriterTag.Table);

            // Generate headers from table
            if (_headers == null)
            {
                _headers = GetHeaders();
            }

            // Create Header Row
            tw.RenderBeginTag(HtmlTextWriterTag.Thead);
            foreach (String header in _headers)
            {
                if (_headerStyle != null)
                    _headerStyle.AddAttributesToRender(tw);
                tw.RenderBeginTag(HtmlTextWriterTag.Th);
                tw.Write(header);
                tw.RenderEndTag();
            }
            tw.RenderEndTag();

            List<List<object>> rows = new List<List<object>>();
            List<object> rowItems;
            while (_reader.Read())
            {
                rowItems = new List<object>();
                for (int i = 0; i < _headers.Length; i++)
                    rowItems.Add(_reader.IsDBNull(i) ? "" : _reader[i]);
                rows.Add(rowItems);
            }

            _reader.Close();

            // Create Data Rows
            tw.RenderBeginTag(HtmlTextWriterTag.Tbody);
            foreach (List<object> row in rows)
            {
                tw.RenderBeginTag(HtmlTextWriterTag.Tr);
                for (int i = 0; i < _headers.Length; i++)
                {
                    string strValue = row[i].ToString();
                    strValue = ReplaceSpecialCharacters(strValue);
                    if (_itemStyle != null)
                        _itemStyle.AddAttributesToRender(tw);
                    tw.RenderBeginTag(HtmlTextWriterTag.Td);
                    tw.Write(HttpUtility.HtmlEncode(strValue));
                    tw.RenderEndTag();
                }
                tw.RenderEndTag();
            }
            tw.RenderEndTag(); // tbody

            tw.RenderEndTag(); // table
            WriteFile(_fileName, "application/ms-excel", sw.ToString());
        }

        private static string ReplaceSpecialCharacters(string value)
        {
            value = value.Replace("’", "'");
            value = value.Replace("“", "\"");
            value = value.Replace("”", "\"");
            value = value.Replace("–", "-");
            value = value.Replace("…", "...");
            return value;
        }

        private static void WriteFile(string fileName, string contentType, string content)
        {
            HttpContext context = HttpContext.Current;
            context.Response.Clear();
            context.Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
            context.Response.Charset = "";
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.ContentType = contentType;
            context.Response.Write(content);
            context.Response.End();
        }
    }
}