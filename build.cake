#load nuget:https://www.myget.org/F/parknow-dotnet/auth/d1aff6eb-bff8-4cc8-bf86-4322ab7ab9be/api/v2?package=MasterCake&version=0.0.18&include=scripts/include.cake

#addin nuget:?package=Cake.Json
#addin nuget:?package=Newtonsoft.Json&version=11.0.2
#addin nuget:?package=Cake.DotNetCoreEf

//this field is required.
string serverlessArtifactNuspecFileName = "MiningService.nuspec";

//application name is going to be registered in Sonar Cube
applicationName = "MiningService";
dotCoverFilters = "+:MiningService.*,-:MiningService.*.Tests";
dotnetCorePublishProjects = new List<string>(){
	"MiningService.WebApi.csproj"
};
   
Task("Default")
    .Description("Runs serverless pipeline including local deployment")
    .IsDependentOn("GitVersion")
    .IsDependentOn("Common-Clean")
    .IsDependentOn("DotnetCore-Restore")
    .IsDependentOn("DotnetCore-Build")
    //.IsDependentOn("DotNetCore-TestAndCover")
    .IsDependentOn("DotnetCore-Publish")
    .IsDependentOn("Serverless-Artifact")
    .IsDependentOn("Teamcity-PublishArtifact")
    .IsDependentOn("Serverless-UploadParameters")
    .IsDependentOn("Serverless-Deploy")
    .IsDependentOn("GitVersion-Finalize")
    .Does(() => {
         Information("Serverless pipeline is finished");
    });

  Task("TestCover")
    .Description("Only triggers test and cover")
    .IsDependentOn("Common-Clean")
    .IsDependentOn("DotnetCore-Restore")
    .IsDependentOn("DotnetCore-Build")
    .IsDependentOn("DotNetCore-TestAndCover")
    .Does(() => {
         Information("Test & cover is finished");
    });

  Task("Deploy")
    .Description("Only triggers deploy")
    .IsDependentOn("Serverless-UploadParameters")
    .IsDependentOn("Serverless-Deploy")    
    .Does(() => {
         Information("Deployment is finished");
    });

RunTarget(target);
