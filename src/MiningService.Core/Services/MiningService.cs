using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MiningService.Core.Model;
using MiningService.Infrastructure.Model;
using MiningService.Infrastructure.Repositories;

namespace MiningService.Core.Services
{
    public interface IMiningService
    {
        Task<(MiningResponse Result, StatusCodes ErrorCodes)> StoreMiningJob(StartMiningRequest request);
        Task<(MiningResponse Result, StatusCodes ErrorCodes)> GetMiningJob(string jobId);
    }

    public class MiningService : IMiningService
    {
        private IMiningRepository _miningRepo;

        public MiningService(IMiningRepository miningRepo)
        {
            _miningRepo = miningRepo;
        }

        public async Task<(MiningResponse Result, StatusCodes ErrorCodes)> StoreMiningJob(StartMiningRequest request)
        {
            // Generate new job id
            var jobId = Guid.NewGuid().ToString();

            var db = new MiningDbModel
            {
                JobId = jobId,
                CreateDate = DateTime.UtcNow
            };

            var resp = await _miningRepo.Save(db);
            return (new MiningResponse(resp), StatusCodes.NoError);
        }

        public async Task<(MiningResponse Result, StatusCodes ErrorCodes)> GetMiningJob(string jobId)
        {
            var jobDb = await GetJobAsDbModel(jobId);

            if (jobDb.Result == null) return (null, jobDb.ErrorCodes);

            var response = new MiningResponse(jobDb.Result) { ReferenceNumber = 0 };

            // Checks that the start mining call executed between 30 and 90 seconds ago:
            // If so it returns a number between 1 and 99 and removes the GUID entry from the DynamoDB table.
            // If not it returns 0.

            var totalSeconds = (DateTime.UtcNow - jobDb.Result.CreateDate).TotalSeconds;

            if (totalSeconds <= 90 && totalSeconds >= 30)
            {
                await _miningRepo.Delete(jobDb.Result);
                response.ReferenceNumber = new Random().Next(1, 100);
            }

            return (response, StatusCodes.NoError);
        }

        public async Task<(MiningResponse Result, StatusCodes ErrorCodes)> DeleteIpAddress(string ipAddress, string username, string reason)
        {
            var ipDb = await GetJobAsDbModel(ipAddress);
            if (ipDb.Result == null) return (null, ipDb.ErrorCodes);

            await _miningRepo.Delete(ipDb.Result);
            return (null, StatusCodes.NoError);
        }

        private async Task<(MiningDbModel Result, StatusCodes ErrorCodes)> GetJobAsDbModel(string jobId)
        {
            if (string.IsNullOrWhiteSpace(jobId)) return (null, StatusCodes.EmptyFieldJobId);

            var resp = await _miningRepo.Get(jobId);
            if (resp == null) return (null, StatusCodes.JobNotFound);

            return (resp, StatusCodes.NoError);
        }
    }
}
