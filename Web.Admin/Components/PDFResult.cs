using System;
using System.Diagnostics;
using System.IO;
using System.Web.Mvc;

namespace Web.Admin.Components
{
    public class PDFResult : ViewResult
    {
        private readonly string _view;
        private readonly dynamic _model;

        public PDFResult(string view, dynamic model, string controller = "")
        {
            _view = view;
            _model = model;
            ViewName = _view;
            ViewData.Model = model;
            _model = model;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            //append user qstring item so the out of process wkhtml call can
            //indeed verify that the user owns the invoice.
            var url = context.HttpContext.Request.Url.ToString()
                .Replace("invoicepdf", "invoicehtml") + "&user=" +
                context.HttpContext.User.Identity.Name;
            string fileName;

            var file = WKHtmlToPdf(url);
            if (context.HttpContext.Request.QueryString["type"] == "Invoice")
                fileName = string.Format("Invoice.{0}.{1}.pdf", _model.InvoiceId, DateTime.Now.Ticks);
            else
                fileName = string.Format("Receipt.{0}.{1}.pdf", _model.InvoiceId, DateTime.Now.Ticks);

            if (file != null)
            {
                context.HttpContext.Response.Buffer = true;
                context.HttpContext.Response.Clear();
                context.HttpContext.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                context.HttpContext.Response.AddHeader("Content-Length", file.Length.ToString());
                context.HttpContext.Response.ContentType = "BulkSms/pdf";
                context.HttpContext.Response.BinaryWrite(file);
                context.HttpContext.Response.End();
            }
        }

        public byte[] WKHtmlToPdf(string url)
        {
            //wkpdftohtml allows redirecting of output by using ' - ' as the filename
            const string fileName = " - ";
            const string wkhtmlDir = "C:\\Program Files (x86)\\wkhtmltopdf\\";
            const string wkhtml = "C:\\Program Files (x86)\\wkhtmltopdf\\wkhtmltopdf.exe";
            var p = new Process();

            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.FileName = wkhtml;
            p.StartInfo.WorkingDirectory = wkhtmlDir;

            string switches = "";
            switches += "--print-media-type ";
            switches += "--margin-top 0mm --margin-bottom 0mm --margin-right 0mm --margin-left 0mm ";
            switches += "--page-size A4 ";
            p.StartInfo.Arguments = switches + " " + url + " " + fileName;
            p.Start();

            //read output
            byte[] buffer = new byte[32768];
            byte[] file;
            using (var ms = new MemoryStream())
            {
                while (true)
                {
                    int read = p.StandardOutput.BaseStream.Read(buffer, 0, buffer.Length);

                    if (read <= 0)
                    {
                        break;
                    }
                    ms.Write(buffer, 0, read);
                }
                file = ms.ToArray();
            }

            // wait or exit
            p.WaitForExit(60000);

            // read the exit code, close process
            int returnCode = p.ExitCode;
            p.Close();

            return returnCode == 0 ? file : null;
        }
    }
}