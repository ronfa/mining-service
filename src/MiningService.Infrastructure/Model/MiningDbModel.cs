using System;
using Amazon.DynamoDBv2.DataModel;

namespace MiningService.Infrastructure.Model
{
    [DynamoDBTable("miningjobs")]
    public class MiningDbModel
    {
        [DynamoDBHashKey]
        public string JobId { get; set; }

        [DynamoDBProperty("CreateDate")]
        public DateTime CreateDate { get; set; }
    }
}
