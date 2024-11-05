using Newtonsoft.Json;

namespace OWP.Infosite.Models;

public class SyncIsLiveToInfoSite
{
    [JsonProperty(PropertyName = "live_configs")]
    public IList<LiveConfig> Matches { get; set; }
}