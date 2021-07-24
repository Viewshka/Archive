using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web;
using System.Web.Http;
using Microsoft.Office.Interop.Word;
using PdfSharp.Pdf.IO;

namespace ConverterV2.Controllers
{
    [Route("api/converter")]
    public class ConverterController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage Post()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);

            var httpContext = HttpContext.Current;
            var file = httpContext.Request.Files[0];

            var currentDirectory = HttpContext.Current.Server.MapPath("~/App_Data");
            var directory = $"{currentDirectory}/Files";

            ClearTempDirectory(directory);

            var pathDocx = $"{directory}/{file?.FileName}";
            file?.SaveAs(pathDocx);

            var application = new Application();

            try
            {
                var document = application.Documents.Open(pathDocx, ReadOnly: true);
                var pathPdf = $"{directory}/result.pdf";
                document.ExportAsFixedFormat(pathPdf, WdExportFormat.wdExportFormatPDF);
                document.Close();

                response.Content = new StreamContent(new FileStream(pathPdf, FileMode.Open));
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = "result.pdf";
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                application.Quit();
            }

            return response;
        }

        private static void ClearTempDirectory(string path)
        {
            var dirInfo = new DirectoryInfo(path);
            foreach (var file in dirInfo.GetFiles())
                file.Delete();
        }
    }
}