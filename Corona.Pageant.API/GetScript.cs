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

namespace Corona.Pageant.API
{
    public static class GetScript
    {
        [FunctionName("GetScript")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "script")] HttpRequest req,
            [Table("Script")] TableClient scriptTable,
            ILogger log)
        {
            log.LogInformation("GetScript function processed a request.");

            List<Script> scripts = new();
            AsyncPageable<Script> queryResults = scriptTable.QueryAsync<Script>();
            await foreach (Script entity in queryResults)
            {
                scripts.Add(entity);
            }

            if (scripts.Count == 0)
            {
                scripts.Add(new Script
                {
                    PartitionKey = "Init",
                    RowKey = "Init",
                    Text = "No script found"
                });
            }

            return new OkObjectResult(scripts.Select(s => new
            {
                Act = s.PartitionKey,
                Scene = s.RowKey,
                s.Text,
                s.SwitchToScene,
                s.Camera1Action,
                s.Camera1Position,
                s.Camera2Action,
                s.Camera2Position,
                s.Camera3Action,
                s.Camera3Position
            }).OrderBy(s => s.Act).ThenBy(s => s.Scene));
        }
    }
}
