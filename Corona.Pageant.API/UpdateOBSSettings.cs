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
    public static class UpdateOBSSettings
    {
        [FunctionName("UpdateOBSSettings")]
        [return: Table("Settings")]
        public static async Task<Settings> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "obs/{scene}")] HttpRequest req,
            string scene,
            [Table("Settings", "OBS", "{scene}")] Settings setting,
            ILogger log)
        {
            if (setting == null)
            {
                log.LogInformation("Settings for obs {0} not found, creating...", scene);
                setting = new Settings
                {
                    PartitionKey = "OBS",
                    RowKey = scene
                };
            }

            IFormCollection data = await req.ReadFormAsync().ConfigureAwait(false);
            if (data != null)
            {
                setting.Setting = data["scene"].ToString();
            }

            log.LogInformation("UpdateOBSSettings: {0} to {1}", setting.RowKey, setting.Setting);

            return new Settings
            {
                PartitionKey = "OBS",
                RowKey = scene,
                Setting = setting.Setting,
                ETag = setting.ETag
            };
        }
    }
}
