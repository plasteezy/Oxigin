using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Web.Admin.Components
{
    public class CsvResult : ActionResult
    {
        public CsvResult(IEnumerable<dynamic> data, string fileName, string path, params string[] fields)
        {
            FileName = fileName;
            Data = data;
            Fields = fields;
            Path = path;
            Headers = fields;
        }

        public CsvResult(IEnumerable<dynamic> data, string fileName, string path)
        {
            FileName = fileName;
            Data = data;
            Path = path;
            Fields = ((Type)data.First().GetType()).GetProperties().Select(x => x.Name).ToArray();
            Headers = GetHeaders(data.First());
        }

        public string FileName { get; set; }

        public string Path { get; set; }

        public IEnumerable<dynamic> Data { get; set; }

        public string[] Fields { get; set; }

        public string[] Headers { get; set; }

        private string GetValue(dynamic item, string field)
        {
            dynamic type = item.GetType();
            dynamic prop = type.GetProperty(field);
            dynamic value = prop.GetValue(item, new object[] { });
            return null == value ? "NULL" : value.ToString();
        }

        private string[] GetHeaders(dynamic obj)
        {
            Type type = obj.GetType();
            var fields = type.GetProperties().ToList();
            var headers = new List<string>();
            fields.ForEach(f =>
            {
                var buffer = new StringBuilder();
                var x = f.Name;
                foreach (char c in x)
                {
                    if (x.IndexOf(c) == 0)
                    {
                        buffer.Append(c);
                        continue;
                    }

                    if (Char.IsUpper(c))
                        buffer.Append(" ");
                    buffer.Append(c);
                }
                headers.Add(buffer.ToString());
            });
            return headers.ToArray();
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var sb = new StringBuilder();
            //write header row
            string header = string.Join(",", Headers);
            sb.AppendLine(header);

            foreach (dynamic item in Data)
            {
                string line = "";
                var fdata = new List<string>();
                foreach (string field in Fields)
                    fdata.Add(String.Concat("\"" + GetValue(item, field) + "\""));
                line = string.Join(",", fdata);
                sb.AppendLine(line);
            }

            File.WriteAllText(context.HttpContext.Server.MapPath(Path), sb.ToString(), Encoding.UTF8);
            var file = new FileInfo(context.HttpContext.Server.MapPath(Path));

            context.HttpContext.Response.Buffer = true;
            context.HttpContext.Response.Clear();
            context.HttpContext.Response.AddHeader("Content-Disposition", "attachment; filename=" + FileName);
            context.HttpContext.Response.AddHeader("Content-Length", file.Length.ToString());
            context.HttpContext.Response.ContentType = "application/octet-stream";
            context.HttpContext.Response.WriteFile(context.HttpContext.Server.MapPath(Path));
        }
    }
}