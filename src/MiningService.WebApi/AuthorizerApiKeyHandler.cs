using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Overleaf.Authentication.WebClient;
using Overleaf.Configuration;
using Overleaf.Lambda;
using Overleaf.Lambda.Authentication.Authentication;
using Overleaf.Lambda.Authentication.Extensions;
using Overleaf.Lambda.Authentication.Model;
using Overleaf.Lambda.Authentication.Model.Auth;
using Environment = Overleaf.Configuration.Environment;

namespace MiningService.WebApi
{
    public class AuthorizerApiKeyHandler : ApiGatewayLambdaBase
    {
        public AuthorizerApiKeyHandler()
        {
#if DEBUG
            var localConfigPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "config");
            ApplicationSettings appSettings = new ApplicationSettings(Environment.Dev, "dev", "apikeyauthorizer", "1.0.0");
            Init(appSettings, localConfigPath, useJsonFileConfiguration: true);
#else
            Init();
#endif
        }

        //protected override void SetupConfiguration(ApplicationSettings initialSettings, string configurationBasePath = "", bool useJsonFileConfiguration = false)
        //{
        //    LambdaLoggerOptions options = new LambdaLoggerOptions() { Environment = "dev", LogLevel = LogLevel.Debug };
        //    var log2 = new LambdaLoggerProvider<PropertiesContextProvider>(options, new PropertiesContextProvider()).CreateLogger("");

        //    log2.LogInformation($"SetupConfiguration: {JsonConvert.SerializeObject(initialSettings)} {configurationBasePath} {useJsonFileConfiguration}");
        //    if (initialSettings == null)
        //    {
        //        var tempEnvConfig = new ConfigurationBuilder()
        //            .AddEnvironmentVariables().Build();

        //        initialSettings = new ApplicationSettings();
        //        tempEnvConfig.GetSection(Constants.ApplicationConfigurationKey).Bind(initialSettings);
        //    }
        //    try
        //    {
        //        if (useJsonFileConfiguration)
        //        {
        //            log2.LogInformation("LambdaBase - Base SetupConfiguration class. Trying to use json file configuration to set configuration object.");
        //            if (string.IsNullOrEmpty(configurationBasePath))
        //            {
        //                var executionPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        //                configurationBasePath = Path.Combine(executionPath, configFolderName);
        //            }

        //            var jsonFileBuilderConfig = GetJsonFileConfigurationObject(initialSettings, configurationBasePath);
        //            configuration = new ConfigurationBuilder()
        //                .AddEnvironmentVariables()
        //                .AddConfiguration(jsonFileBuilderConfig)
        //                .Build();
        //        }
        //        else
        //        {
        //            var relativePath = Overleaf.Configuration.ParameterStore.Extensions.ConfigurationExtensions.GetParameterPathFromApplicationSettingsAndRelativePath(initialSettings, initialSettings.Name.ToLower());
        //            var labelValue = Overleaf.Configuration.ParameterStore.Extensions.ConfigurationExtensions.GetLabelEqualValues(initialSettings);
        //            log2.LogInformation($"LambdaBase - Trying to parameter store to set configuration object. : {relativePath} {JsonConvert.SerializeObject(labelValue)}");
        //            configuration = new ConfigurationBuilder()
        //                .AddEnvironmentVariables()
        //                .AddParameterStoreParams(initialSettings, TimeSpan.FromMinutes(1), initialSettings.Name.ToLower(), logger)
        //                .Build();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var errorMessage = $"Lambda Base Setup Configuration Exception occurred ... {ex.Message}. Inner exception message {ex.InnerException?.Message}";
        //        Console.WriteLine(errorMessage);
        //        throw new Exception(errorMessage);
        //    }

        //}

        [ExcludeFromCodeCoverage]
        protected override ServiceProvider ConfigureServices(IServiceCollection services)
        {
            services
                .Configure<ScopesMappingsConfig>(configuration.GetSection(ScopesMappingsConfig.SectionKey))
                .Configure<AuthServiceConfig>(configuration.GetSection(AuthServiceConfig.SectionKey))
                .AddSingleton(logger)
                .AddLambdaAuthentication();

            return services.BuildServiceProvider();
        }

        public async Task<AuthPolicy> HandleApiKeyAsync(TokenAuthorizerContext input, ILambdaContext context)
        {
            logger.LogInformation($"{nameof(AuthorizerApiKeyHandler)} | Input: {input.Type} {input.MethodArn} Context: {context.FunctionName} {context.FunctionVersion}");
            var lambdaAuthorizationHandler = serviceProvider.GetService<ILambdaAuthorizationHandler>();
            return await lambdaAuthorizationHandler.HandleAsync(input);
        }
    }
}
