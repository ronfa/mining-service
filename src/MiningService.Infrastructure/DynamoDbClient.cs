using System;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.Options;
using MiningService.Infrastructure.Model;

namespace MiningService.Infrastructure
{
    public interface IDynamoDbClient
    {
        IDynamoDBContext CreateDbContext();
    }

    public class DynamoDbClient : IDynamoDbClient
    {
        private MiningDbConfigModel _dbConfig;

        public DynamoDbClient(IOptions<MiningDbConfigModel> dbConfig)
        {
            _dbConfig = new MiningDbConfigModel()
                {ServiceUrl = dbConfig.Value.ServiceUrl, TablePrefix = dbConfig.Value.TablePrefix};
        }

        public IDynamoDBContext CreateDbContext()
        {
            if (!string.IsNullOrWhiteSpace(_dbConfig.ServiceUrl) && !string.IsNullOrWhiteSpace(_dbConfig.TablePrefix))
            {
                AmazonDynamoDBClient client;
                AmazonDynamoDBConfig clientConfig = new AmazonDynamoDBConfig();
                clientConfig.ServiceURL = _dbConfig.ServiceUrl;
                client = new AmazonDynamoDBClient(clientConfig);
                DynamoDBContextConfig config = new DynamoDBContextConfig() { TableNamePrefix = _dbConfig.TablePrefix };
                return new DynamoDBContext(client, config);
            }
            throw new MissingMemberException("Database configuration is missing");
        }
    }
}
