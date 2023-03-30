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
    public static class UpdateCameraSettings
    {
        [FunctionName("UpdateCameraSettings")]
        [return: Table("Settings")]
        public static async Task<Settings> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "camera/{cameraNumber}")] HttpRequest req,
            string cameraNumber,
            [Table("Settings", "Camera", "{cameraNumber}")] Settings setting,
            ILogger log)
        {
            if (setting == null)
            {
                log.LogInformation("Settings for camera {0} not found, creating...", cameraNumber);
                setting = new Settings
                {
                    PartitionKey = "Camera",
                    RowKey = cameraNumber
                };
            }

            IFormCollection data = await req.ReadFormAsync().ConfigureAwait(false);
            if (data != null)
            {
                setting.Setting = data["ipAddress"].ToString();
            }

            log.LogInformation("UpdateCameraSettings: {0} to {1}", setting.RowKey, setting.Setting);

            return setting;
        }
    }
}
