using System.Threading.Tasks;
using MiningService.Infrastructure.Model;

namespace MiningService.Infrastructure.Repositories
{
    public interface IMiningRepository
    {
        Task<MiningDbModel> Get(string jobId);
        Task<MiningDbModel> Save(MiningDbModel jobModel);
        Task Delete(MiningDbModel jobModel);
    }

    public class MiningRepository : IMiningRepository
    {
        private IDynamoDbClient _dynamoDbClient;

        public MiningRepository(IDynamoDbClient dynamoDbClient)
        {
            _dynamoDbClient = dynamoDbClient;
        }

        public async Task Delete(MiningDbModel jobModel)
        {
            using (var context = _dynamoDbClient.CreateDbContext())
            {
                await context.DeleteAsync(jobModel);
            }
        }

        public async Task<MiningDbModel> Get(string jobId)
        {
            using (var context = _dynamoDbClient.CreateDbContext())
            {
                var resp = await context.LoadAsync<MiningDbModel>(jobId);
                return resp;
            }
        }

        public async Task<MiningDbModel> Save(MiningDbModel jobModel)
        {
            using (var context = _dynamoDbClient.CreateDbContext())
            {
                await context.SaveAsync(jobModel);
                return jobModel;
            }
        }
    }
}
