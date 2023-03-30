using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Corona.Pageant.API
{
    public static class UpdateScriptPart
    {
        [FunctionName("UpdateScriptPart")]
        [return: Table("Script")]
        public static async Task<Script> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "script/{act}/{scene}")] HttpRequest req,
            string act,
            string scene,
            [Table("Script", "{act}", "{scene}")] Script script,
            ILogger log)
        {
            if (script == null)
            {
                log.LogInformation("Settings for act {0} scene {1} not found, creating...", act, scene);
                script = new Script
                {
                    PartitionKey = act,
                    RowKey = scene
                };
            }

            IFormCollection data = await req.ReadFormAsync().ConfigureAwait(false);
            if (data != null)
            {
                script.Text = data["text"];
                script.Camera1Action = data["camera1Action"].ToString();
                script.Camera1Position = data["camera1Position"].ToString();
                script.Camera2Action = data["camera2Action"].ToString();
                script.Camera2Position = data["camera2Position"].ToString();
                script.Camera3Action = data["camera3Action"].ToString();
                script.Camera3Position = data["camera3Position"].ToString();
                script.SwitchToScene = data["switchToScene"].ToString();
            }

            log.LogInformation("UpdateScriptPart: act {0} scene {1}", script.PartitionKey, script.RowKey);

            return script;
        }
    }
}
