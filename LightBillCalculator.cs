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

namespace azure
{
    public static class LightBillCalculator
    {
        [FunctionName("LightBillCalculator")]
        [OpenApiOperation()]
        [OpenApiParameter(name: "Unit", Required = true, In = ParameterLocation.Query, Type = typeof(int))]
        [OpenApiParameter(name: "UnitPricePerArea", Required = true, In = ParameterLocation.Query, Type = typeof(int))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/text", bodyType: typeof(string))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous,"post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //dynamic data = JsonConvert.DeserializeObject(requestBody);

            var unit = req.Query["unit"];
            var price = req.Query["unitPricePerArea"];


            //if (unit == null || price == null)
            //{
            //    return new BadRequestObjectResult("Please pass unit and price_per_unit in the request body");
            //}

            int totalBill = Convert.ToInt32(unit) * Convert.ToInt32 (price);

            return new OkObjectResult(
     
                $"Your Total bill is = {totalBill}"
            );
        }
    }
}
