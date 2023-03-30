using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using AzureFunctions.Extensions.Swashbuckle.Attribute;

namespace Corona.Pageant.API
{
    public static class GetSettings
    {
        [FunctionName("GetSettings")]
        [ProducesResponseType(typeof(List<Settings>), (int)HttpStatusCode.OK)]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "settings")] HttpRequest req,
            [SwaggerIgnore][Table("Settings")] TableClient settingsTable,
            ILogger log)
        {
            log.LogInformation("GetScript function processed a request.");

            List<Settings> settings = new();
            AsyncPageable<Settings> queryResults = settingsTable.QueryAsync<Settings>();
            await foreach (Settings entity in queryResults)
            {
                settings.Add(entity);
            }

            return new OkObjectResult(settings.Select(s => new
            {
                SettingType = s.PartitionKey,
                SettingId = s.RowKey,
                s.Setting
            }).OrderBy(s => s.SettingType).ThenBy(s => s.SettingId));
        }
    }
}
