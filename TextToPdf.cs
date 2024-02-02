using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using System.Net;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
namespace azure
{
    public static class TextToPdf
    {
        [FunctionName("TextToPdf")]
        [OpenApiOperation()]
        [OpenApiParameter(name:"Text",Required =true,In = ParameterLocation.Query,Type=typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/pdf", bodyType:typeof(string))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string Text = req.Query["Text"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            Text = Text ?? data?.Text;
             
            using PdfDocument document = new PdfDocument();
            PdfPage page = document.Pages.Add();

            PdfGraphics grpahics = page.Graphics;
            grpahics.DrawString(Text,new PdfStandardFont(PdfFontFamily.Helvetica,20),
            PdfBrushes.Black,
            new Syncfusion.Drawing.PointF(0,0));

            using MemoryStream outputPdfStream = new MemoryStream();
            document.Save(outputPdfStream);
            outputPdfStream.Position = 0;
            document.Dispose();

            string contentType = "application/pdf";
            string fileName = "document.pdf";

            req.HttpContext.Response.Headers.Add("Content-Disposition",$"attachment;{fileName}");
           

            return new FileContentResult(outputPdfStream.ToArray(),contentType);
        }
    }
}
