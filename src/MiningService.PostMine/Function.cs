using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MiningService.Core;
using MiningService.Core.Model;
using MiningService.Core.Services;
using MiningService.Infrastructure;
using MiningService.Infrastructure.Model;
using MiningService.Infrastructure.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using StatusCodes = MiningService.Core.Model.StatusCodes;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace MiningService.PostMine
{
    public class Function
    {
        protected IConfiguration configuration { get; set; }
        protected ServiceProvider serviceProvider { get; set; }
        private IMiningService _miningService;

        public Function()
        {
            serviceProvider = ConfigureServices(new ServiceCollection());
            _miningService = serviceProvider.GetService<IMiningService>();
        }

        protected virtual ServiceProvider ConfigureServices(IServiceCollection services)
        {
            configuration = new ConfigurationBuilder()
                .AddConfiguration(new ConfigurationBuilder().AddJsonFile(@"appsettings.json").Build())
                .Build();

            services.AddSingleton(configuration);
            services.AddTransient<IMiningService, Core.Services.MiningService>();
            services.AddSingleton<IDynamoDbClient, DynamoDbClient>();
            services.AddTransient<IMiningRepository, MiningRepository>();
            services.Configure<MiningDbConfigModel>(options => configuration.GetSection(MiningDbConfigModel.Key).Bind(options));
            return services.BuildServiceProvider();
        }

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogLine($"Process request: {request?.HttpMethod} {request.Body}");

            try
            {
                //var miningRequest = JsonConvert.DeserializeObject<StartMiningRequest>(request.Body);
                var miningRequest = new StartMiningRequest();
                var resp = await _miningService.StoreMiningJob(miningRequest);

                return resp.Result != null
                    ? CreateResponse((int)HttpStatusCode.Created, JsonConvert.SerializeObject(resp.Result))
                    : CreateResponse((int)HttpStatusCode.BadRequest, JsonConvert.SerializeObject(new ErrorResponse((int)resp.ErrorCodes, resp.ErrorCodes.DescriptionAttr(), "")));
            }
            catch (Exception ex)
            {
                context.Logger.LogLine($"Exception occured while starting mining job: {ex.Message}");
                var err = new ErrorResponse((int)StatusCodes.InternalServerError, ex.Message, ex.StackTrace);
                return CreateResponse((int)HttpStatusCode.InternalServerError, JsonConvert.SerializeObject(err));
            }
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
