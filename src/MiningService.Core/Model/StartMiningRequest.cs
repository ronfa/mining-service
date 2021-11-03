using System.Text.Json.Serialization;

namespace MiningService.Core.Model
{
    public class StartMiningRequest
    {
        [JsonPropertyName("JobId")]
        public string JobId { get; set; }
    }
}
