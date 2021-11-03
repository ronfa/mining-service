using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using MiningService.Infrastructure.Model;

namespace MiningService.Infrastructure.Repositories
{
    public interface IMiningRepository
    {
        Task<MiningDbModel> Get(string ipAddress);
        Task<MiningDbModel> Save(MiningDbModel tokenModel);
        Task Delete(MiningDbModel tokenModel);
        Task<List<MiningDbModel>> GetAll();
    }

    public class MiningRepository : IMiningRepository
    {
        private IDynamoDbClient _dynamoDbClient;

        public MiningRepository(IDynamoDbClient dynamoDbClient)
        {
            _dynamoDbClient = dynamoDbClient;
        }

        public async Task Delete(MiningDbModel tokenModel)
        {
            using (var context = _dynamoDbClient.CreateDbContext())
            {
                await context.DeleteAsync(tokenModel);
            }
        }

        public async Task<MiningDbModel> Get(string ipAddress)
        {
            using (var context = _dynamoDbClient.CreateDbContext())
            {
                var resp = await context.LoadAsync<MiningDbModel>(ipAddress);
                return resp;
            }
        }

        public async Task<List<MiningDbModel>> GetAll()
        {
            using (var context = _dynamoDbClient.CreateDbContext())
            {
                var result = await context.ScanAsync<MiningDbModel>(new List<ScanCondition>()).GetNextSetAsync();
                return result;
            }
        }

        public async Task<MiningDbModel> Save(MiningDbModel tokenModel)
        {
            using (var context = _dynamoDbClient.CreateDbContext())
            {
                await context.SaveAsync(tokenModel);
                return tokenModel;
            }
        }
    }
}
