using Newtonsoft.Json;

namespace OWP.Infosite.Models;

public class LiveConfig
{
    [JsonProperty(PropertyName = "match_id")]
    public int MatchId { get; set; }

    [JsonProperty(PropertyName = "is_live")]
    public bool IsLive { get; set; }

    [JsonProperty(PropertyName = "subtype_list")]
    public IList<int> Subtypes { get; set; }
}