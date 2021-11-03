using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MiningService.Core;
using MiningService.Core.Model;
using MiningService.Core.Services;
using MiningService.Infrastructure;
using MiningService.Infrastructure.Model;
using MiningService.Infrastructure.Repositories;
using Newtonsoft.Json;
using Overleaf.Configuration;
using Overleaf.Lambda;
using StatusCodes = MiningService.Core.Model.StatusCodes;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
namespace MiningService.WebApi
{
    public class WebApiHandler : ApiGatewayLambdaBase
    {
        private IMiningService _miningService;
        const string Param_JobId = "jobid";

        private string GetLogPrefix(string method) => $"{nameof(WebApiHandler)} | {method} |";

        public WebApiHandler()
        {

#if DEBUG
            var localConfigPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "config");
            ApplicationSettings appSettings = new ApplicationSettings(Overleaf.Configuration.Environment.Dev, "dev", "deleteip", "1.0.0");
            Init(appSettings, localConfigPath, useJsonFileConfiguration: true);
#else
            Init();
#endif
            _miningService = serviceProvider.GetService<IMiningService>();
        }

        protected override ServiceProvider ConfigureServices(IServiceCollection services)
        {
            //specify all interfaces to be injected
            services.AddSingleton(logger);
            services.AddSingleton(configuration);
            services.AddTransient<IMiningService, Core.Services.MiningService>();
            services.AddSingleton<IDynamoDbClient, DynamoDbClient>();
            services.AddTransient<IMiningRepository, MiningRepository>();
            services.Configure<MiningDbConfigModel>(options => configuration.GetSection(MiningDbConfigModel.Key).Bind(options));

            //build the service provider
            serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }

        /// <summary>
        /// A Lambda function to respond to HTTP Get methods from API Gateway
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The API Gateway response.</returns>
        public async Task<APIGatewayProxyResponse> PostHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var logPrefix = GetLogPrefix(nameof(PostHandler));

            logger.LogDebug($"{logPrefix} Process request: {request?.HttpMethod} {request.Body}");

            if (request?.HttpMethod?.ToLower() == HttpMethods.Options.ToLower())
            {
                logger.LogDebug($"{logPrefix} Called with OPTIONS http method");
                return CreateResponse((int)HttpStatusCode.OK, "");
            }

            try
            {
                var miningRequest = JsonConvert.DeserializeObject<StartMiningRequest>(request.Body);
                var resp = await _miningService.StoreMiningJob(miningRequest);

                return resp.Result != null
                    ? CreateResponse((int)HttpStatusCode.Created, JsonConvert.SerializeObject(resp.Result))
                    : CreateResponse((int)HttpStatusCode.BadRequest, JsonConvert.SerializeObject(new ErrorResponse((int)resp.ErrorCodes, resp.ErrorCodes.DescriptionAttr(), "")));
            }
            catch (Exception ex)
            {
                logger.LogError($"{logPrefix} Exception occured while starting mining job: {ex.Message}");
                var err = new ErrorResponse((int)StatusCodes.InternalServerError, ex.Message, ex.StackTrace);
                return CreateResponse((int)HttpStatusCode.InternalServerError, JsonConvert.SerializeObject(err));
            }
        }

        public async Task<APIGatewayProxyResponse> GetHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var logPrefix = GetLogPrefix(nameof(GetHandler));

            logger.LogDebug($"{logPrefix} Process request: {request?.HttpMethod} {request.Body}");
            try
            {
                var jobIdFromQueryParameters = GetJobIdFromQueryParameters(request);
                if (jobIdFromQueryParameters.ErrorCode != StatusCodes.NoError)
                {
                    var errResponse = new ErrorResponse((int) jobIdFromQueryParameters.ErrorCode,
                        jobIdFromQueryParameters.ErrorCode.DescriptionAttr(), "");
                    return CreateResponse((int) HttpStatusCode.BadRequest, JsonConvert.SerializeObject(errResponse));
                }

                var resp = await _miningService.GetMiningJob(jobIdFromQueryParameters.Ip);

                if (resp.Result == null && resp.ErrorCodes == StatusCodes.JobNotFound)
                    return CreateResponse((int) HttpStatusCode.NotFound, null);

                return resp.Result == null
                    ? CreateResponse((int) HttpStatusCode.BadRequest,
                        JsonConvert.SerializeObject(new ErrorResponse((int) resp.ErrorCodes,
                            resp.ErrorCodes.DescriptionAttr(), "")))
                    : CreateResponse((int) HttpStatusCode.OK, JsonConvert.SerializeObject(resp.Result));
            }
            catch (Exception ex)
            {
                logger.LogError(
                    $"{logPrefix} Exception occured while retrieving results for request: {request.Body}. Error: {ex.Message}");
                var err = new ErrorResponse((int) StatusCodes.InternalServerError, ex.Message, ex.StackTrace);
                return CreateResponse((int) HttpStatusCode.InternalServerError, JsonConvert.SerializeObject(err));
            }
        }

        private (string Ip, StatusCodes ErrorCode) GetJobIdFromQueryParameters(APIGatewayProxyRequest request)
        {
            if (request.QueryStringParameters == null || request.QueryStringParameters.Count() < 0 || !request.QueryStringParameters.TryGetValue(Param_JobId, out string ip))
            {
                return (null, StatusCodes.EmptyFieldJobId);
            }
            return (ip, StatusCodes.NoError);
        }

        private (string Value, StatusCodes ErrorCode) GetValueFromHeaders(APIGatewayProxyRequest request, string key, StatusCodes errorCode)
        {
            if (request.Headers == null || request.Headers.Count() < 0 || !request.Headers.TryGetValue(key, out string value))
            {
                return (null, errorCode);
            }
            return (value, StatusCodes.NoError);
        }

        private APIGatewayProxyResponse CreateResponse(int statusCode, string message)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = statusCode,
                Body = message,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }

    }
}
