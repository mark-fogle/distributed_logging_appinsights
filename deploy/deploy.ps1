function ZipDeploy-AzureFunction([string]$projectPath, [string]$resourceGroupName, [string]$functionAppName, [string]$publishFolder){
    
    az webapp config appsettings set -g $resourceGroupName -n $functionAppName --settings WEBSITE_RUN_FROM_PACKAGE=1
    az webapp config appsettings set -g $resourceGroupName -n $functionAppName --settings FUNCTIONS_EXTENSION_VERSION=~4
    az webapp config appsettings set -g $resourceGroupName -n $functionAppName --settings FUNCTIONS_WORKER_RUNTIME=dotnet

    dotnet publish -c Release $PSScriptRoot/../src/ApplicationInsightsTest.csproj
    $publishZip = "$($functionAppName).zip"
    $publishFolder = $projectPath + $publishFolder

    if(Test-path $publishZip) {Remove-item $publishZip}
    Compress-Archive -Path $publishFolder -DestinationPath $publishZip -CompressionLevel Optimal

    az functionapp deployment source config-zip `
        -g $resourceGroupName -n $functionAppName --src $publishZip

    Remove-Item $publishZip
}

$projectPrefix = $args[0]
$resourceGroupName = "$($projectPrefix)_rg"

dotnet restore $PSScriptRoot/../src/ApplicationInsightsTest.csproj

#Deploy Raw To Epoch
ZipDeploy-AzureFunction -projectPath "$($PSScriptRoot)/../src" `
    -resourceGroupName $resourceGroupName -functionAppName "$($projectPrefix)-functions" -publishFolder "/bin/Release/net6.0/publish/*"



