using System.Collections.Generic;
using System.Net;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using Overleaf.Authentication.WebClient.Abstractions;
using Overleaf.Lambda;

namespace MiningService.WebApi
{
    public class AuthValidationReplacementHandler : ApiGatewayLambdaBase
    {
        private static string GetLogPrefix(string method) => $"{nameof(AuthValidationReplacementHandler)} | {method} |";

        /// <summary>
        /// Default constructor that Lambda will invoke.
        /// </summary>
        public AuthValidationReplacementHandler()
        {
        }

        public APIGatewayProxyResponse Handler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var logPrefix = GetLogPrefix(nameof(AuthValidationReplacementHandler));
            context.Logger.LogLine($"{logPrefix} Request: {request.Body} " +
                                  $"| Context: {context.FunctionName} {context.FunctionVersion}");

            var resp = new ValidationResponse()
            {
                ExpirationDate = new System.DateTime(2099, 12, 30, 23, 59, 59),
                OperatorIds = new List<long>() { 99999 },
                Scopes = new List<string>() { "FraudDetection.MiningService.FullAccess" },
                SystemId = 1
            };

            var respString = JsonConvert.SerializeObject(resp);

            context.Logger.LogLine($"{logPrefix} Response: {respString}");
            return new APIGatewayProxyResponse
            {
                Body = respString,
                StatusCode = (int)HttpStatusCode.OK,
            };
        }

    }
}
