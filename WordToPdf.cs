using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO;
using Syncfusion.DocIORenderer;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using System.Net;

namespace azure
{
    public static class WordToPdf
    {
        [FunctionName("WordToPdf")]
        [OpenApiOperation()]
        [OpenApiRequestBody(contentType: "multipart/form-data", bodyType: typeof(MultiPartFormDataModel), Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/pdf", bodyType: typeof(byte[]))]
        public static async Task<IActionResult> Run(
     [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
     ILogger log)
        {
            var textFile = req.Form.Files[0];

            using var wordStream = new MemoryStream();

            await textFile.CopyToAsync(wordStream);

            wordStream.Position = 0;

            var word = new WordDocument(wordStream, FormatType.Docx);

            using var pdfStream = new MemoryStream();

            using (DocIORenderer renderer = new DocIORenderer())
            {
                var pdf = renderer.ConvertToPDF(word);
                pdf.Save(pdfStream);
                pdf.Close();
            }

            word.Close();

            pdfStream.Position = 0;

            string contentType = "application/pdf";
            string fileName = "Document.pdf";

            req.HttpContext.Response.Headers.Add("Content-Disposition", $"attachment; filename={fileName}");

            return new FileContentResult(pdfStream.ToArray(), contentType);

        }
    }
}