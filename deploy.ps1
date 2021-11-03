[Environment]::SetEnvironmentVariable("AWS_ACCESS_KEY_ID",  "AKIAYWUZVITHROFJL5K5", "Process")
[Environment]::SetEnvironmentVariable("AWS_SECRET_ACCESS_KEY", "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", "Process")
[Environment]::SetEnvironmentVariable("AWS_DEFAULT_REGION", "eu-central-1", "Process")

mkdir ./publish
mkdir ./artifacts
dotnet build -c Release
dotnet publish ./src/MiningService.GetMine/MiningService.GetMine.csproj --framework netcoreapp3.1 -o ./publish/MiningService.GetMine -c Release
dotnet publish ./src/MiningService.PostMine/MiningService.PostMine.csproj --framework netcoreapp3.1 -o ./publish/MiningService.PostMine -c Release
Compress-Archive -Path ./publish/MiningService.GetMine/* ./artifacts/MiningService.GetMine.zip
Compress-Archive -Path ./publish/MiningService.PostMine/* ./artifacts/MiningService.PostMine.zip

aws cloudformation package --template-file ./template.yaml --s3-bucket mining-lambda-artifacts --output-template-file ./packaged-template.yaml
aws cloudformation deploy --template-file ./packaged-template.yaml --stack-name dev-illuvium-mining-webapi --tags environment=dev platform=illuvium system=mining subsystem=webapi
