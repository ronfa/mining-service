using System;
using System.Diagnostics.CodeAnalysis;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime.CredentialManagement;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using Microsoft.Extensions.Logging;
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
        private ILogger _logger;

        public DynamoDbClient(IOptions<MiningDbConfigModel> dbConfig, ILogger logger)
        {
            _dbConfig = dbConfig.Value;
            _logger = logger;
        }

        public IDynamoDBContext CreateDbContext()
        {
            if (!string.IsNullOrWhiteSpace(_dbConfig.ServiceUrl) && !string.IsNullOrWhiteSpace(_dbConfig.TablePrefix))
            {
                AmazonDynamoDBClient client;
                AmazonDynamoDBConfig clientConfig = new AmazonDynamoDBConfig();
                clientConfig.ServiceURL = _dbConfig.ServiceUrl;
#if DEBUG
                client = GetLocalClient(clientConfig);
#else
                client = new AmazonDynamoDBClient(clientConfig);
#endif
                DynamoDBContextConfig config = new DynamoDBContextConfig() { TableNamePrefix = _dbConfig.TablePrefix };
                return new DynamoDBContext(client, config);
            }
            throw new MissingMemberException("Database configuration is missing");
        }

        [ExcludeFromCodeCoverage]
        private AmazonDynamoDBClient GetLocalClient(AmazonDynamoDBConfig config)
        {
            _logger.LogInformation("Create dynamodb client for local env");
            Credentials localCredentials = null;
            if (new CredentialProfileStoreChain().TryGetProfile("playpenprofile", out CredentialProfile credProfile))
            {
                var tokenService = new AmazonSecurityTokenServiceClient(credProfile.Options.AccessKey, credProfile.Options.SecretKey);
                AssumeRoleRequest request = new AssumeRoleRequest
                {
                    DurationSeconds = 3600,
                    RoleArn = credProfile.Options.RoleArn,
                    RoleSessionName = "localDevelopment",
                };
                var response = tokenService.AssumeRoleAsync(request).Result;
                localCredentials = response.Credentials;
            }

            return new AmazonDynamoDBClient(localCredentials, config);
        }
    }
}
