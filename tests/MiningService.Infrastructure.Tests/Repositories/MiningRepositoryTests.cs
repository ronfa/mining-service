using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using MiningService.Infrastructure.Model;
using MiningService.Infrastructure.Repositories;
using Moq;
using Xunit;

namespace MiningService.Infrastructure.Tests.Repositories
{
    public class MiningRepositoryTests
    {
        MiningRepository _repo;
        Mock<IDynamoDbClient> _dynamoDbClient;
        Mock<IDynamoDBContext> _dynamoDbContext;

        const string TestJobId = "1.1.1.1";
        readonly DateTime TestLastModified = DateTime.UtcNow;

        public MiningRepositoryTests()
        {
            _dynamoDbContext = new Mock<IDynamoDBContext>();

            _dynamoDbContext.Setup(x =>
                    x.SaveAsync(It.Is<MiningDbModel>(x => x.JobId == TestJobId), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new PutItemResponse() {HttpStatusCode = System.Net.HttpStatusCode.OK}))
                .Verifiable();

            _dynamoDbContext.Setup(x =>
                    x.LoadAsync<MiningDbModel>(It.Is<object>(x => x.Equals(TestJobId)), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new MiningDbModel() {JobId = TestJobId, CreateDate = TestLastModified}))
                .Verifiable();

            _dynamoDbContext.Setup(x =>
                    x.DeleteAsync(It.Is<MiningDbModel>(x => x.JobId == TestJobId), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new MiningDbModel() {JobId = TestJobId, CreateDate = TestLastModified}))
                .Verifiable();

            _dynamoDbClient = new Mock<IDynamoDbClient>();
            _dynamoDbClient.Setup(x => x.CreateDbContext()).Returns(_dynamoDbContext.Object);

            _repo = new MiningRepository(_dynamoDbClient.Object);
        }

        [Fact()]
        public async Task GetMiningJob()
        {
            var resp = await _repo.Get(TestJobId);

            Assert.Equal(TestJobId, resp.JobId);
            Assert.Equal(TestLastModified, resp.CreateDate);
            _dynamoDbContext.Verify(
                x => x.LoadAsync<MiningDbModel>(It.Is<object>(x => x.Equals(TestJobId)), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact()]
        public async Task SaveMiningJob()
        {
            var req = new MiningDbModel()
            {
                JobId = TestJobId,
                CreateDate = TestLastModified,
            };

            var resp = await _repo.Save(req);
            Assert.Equal(req, resp);
            _dynamoDbContext.Verify(
                x => x.SaveAsync(It.Is<MiningDbModel>(x => x.JobId == TestJobId), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact()]
        public async Task DeleteMiningJob()
        {
            var req = new MiningDbModel()
            {
                JobId = TestJobId,
                CreateDate = TestLastModified,
            };

            await _repo.Delete(req);
            _dynamoDbContext.Verify(
                x => x.DeleteAsync(It.Is<MiningDbModel>(x => x.JobId == TestJobId), It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}