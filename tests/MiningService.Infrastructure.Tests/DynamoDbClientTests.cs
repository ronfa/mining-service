using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MiningService.Infrastructure.Model;
using Moq;
using Xunit;

namespace MiningService.Infrastructure.Tests
{
    public class DynamoDbClientTests
    {
        Mock<ILogger> _logger;
        DynamoDbClient dynamoDbClient;
        private const string _serviceUrl = "https://local";
        private const string _tablePrefix = "prefix";

        public DynamoDbClientTests()
        {
            _logger = new Mock<ILogger>();
        }

        [Fact()]
        public void DynamoDbClient_CreateClient_WithValidConfig_Test()
        {
            var model = new MiningDbConfigModel()
            {
                ServiceUrl = _serviceUrl,
                TablePrefix = _tablePrefix
            };
            dynamoDbClient = new DynamoDbClient(Options.Create(model), _logger.Object);

            var context = dynamoDbClient.CreateDbContext();
            Assert.NotNull(context);
        }

        [Fact()]
        public void DynamoDbClient_CreateClient_WithInvalidConfig_Test()
        {
            var model = new MiningDbConfigModel();
            dynamoDbClient = new DynamoDbClient(Options.Create(model), _logger.Object);
            Assert.Throws<MissingMemberException>(() => dynamoDbClient.CreateDbContext());
        }
    }
}