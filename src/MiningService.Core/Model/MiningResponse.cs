using System;
using System.Text.Json.Serialization;
using MiningService.Infrastructure.Model;

namespace MiningService.Core.Model
{
    public class MiningResponse
    {
        [JsonPropertyName("JobId")]
        public string JobId { get; set; }

        [JsonPropertyName("CreateDate")]
        public DateTime CreateDate { get; set; }

        [JsonPropertyName("ReferenceNumber")]
        public int ReferenceNumber { get; set; }

        public MiningResponse() { }

        public MiningResponse(MiningDbModel dbModel)
        {
            JobId = dbModel.JobId;
            CreateDate = dbModel.CreateDate;
        }
    }
}
