using Newtonsoft.Json;

namespace Corona.Pageant.API;

public class SignalRCommunication
{
    [JsonProperty("sessionId", NullValueHandling = NullValueHandling.Ignore)]
    public int SessionId { get; set; }

    [JsonProperty("record", NullValueHandling = NullValueHandling.Ignore)]
    public object Record { get; set; }
}
