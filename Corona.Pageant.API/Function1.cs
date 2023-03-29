using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using AzureFunctions.Extensions.Swashbuckle.Attribute;

namespace Corona.Pageant.API
{
    public static class Function1
    {
        [FunctionName("Function1")]
        [QueryStringParameter("name", "this is name", DataType = typeof(string), Required = true)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }

        [FunctionName("Function2")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        public static async Task<IActionResult> F2([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)][RequestBodyType(typeof(string), "the name")] string name, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }

        [FunctionName("Function3")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        public static async Task<IActionResult> F3([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)][RequestBodyType(typeof(string), "the name")] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = string.Empty;

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //dynamic data = JsonConvert.DeserializeObject(requestBody);
            //name = name ?? data?.name;
            name = name ?? requestBody;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }

        [FunctionName("Function4")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Created)]
        public static async Task<IActionResult> F4(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "testandget/{id}")]
            [RequestBodyType(typeof(string), "test model")]
            HttpRequest httpRequest,
            long id)
        {
            if (httpRequest.Method.Equals("post", StringComparison.OrdinalIgnoreCase))
            {
                using (var reader = new StreamReader(httpRequest.Body))
                {
                    var json = await reader.ReadToEndAsync();
                    //var testModel = JsonConvert.DeserializeObject<string>(json);
                    //return new CreatedResult("", testModel);
                    return new CreatedResult("", json);
                }
            }

            return new OkResult();
        }
    }
}
