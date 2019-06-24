Set-ExecutionPolicy -ExecutionPolicy RemoteSigned  -Force

$folderName = (Get-Date).tostring("dd-MM-yyyyThh-mm-ss")            
New-Item -itemType Directory -Path C:\Tmp -Name $folderName

$githubUrl = "https://github.com/litebulb/ProjectEdison.git"
$location = "C:\Tmp\$folderName"
git clone $githubUrl $location
Start-Sleep -s 180

$Path = "C:\Tmp\values.txt"
$values = Get-Content $Path | Out-String | ConvertFrom-StringData
$values.COSMOSDBEP
$values.CosmosDbSRT
$values.AzureServiceBusCONN
$values.ServiceBusAccesKey
$values.AzureAdSRT
$values.SignalCONN
$values.ApplicationInsightsKey
$values.IoTHubControllerSRT
$values.BotAppPassword
$values.BOTSECRETTOKEN
$values.BotStorageep
$values.NotificationHubSRT

$path1 = "$location\Edison.Devices\Edison.Simulators.Sensors\appsettings.json"
$json = Get-Content $path1 | ConvertFrom-Json

$json.CosmosDb.Endpoint = $values.COSMOSDBEP -replace ‘[\"]’,''
$json.CosmosDb.AuthKey = $values.CosmosDbSRT -replace ‘[\"]’,''
$json.AzureServiceBus.ConnectionString = $values.AzureServiceBusCONN -replace ‘[\"]’,''
$json.ServiceBusAzure.ConnectionString = $values.ServiceBusAccesKey -replace ‘[\"]’,''
$json.RestService.AzureAd.ClientSecret = $values.AzureAdSRT -replace ‘[\"]’,''
$json.SignalR.ConnectionString = $values.SignalCONN -replace ‘[\"]’,''
$json.ApplicationInsights.InstrumentationKey = $values.ApplicationInsightsKey -replace ‘[\"]’,''
$json.Simulator.IoTHubConnectionString = $values.IoTHubControllerSRT -replace ‘[\"]’,''
$json.BotConfigOptions.MicrosoftAppPassword = $values.BotAppPassword -replace ‘[\"]’,''
$json.BotConfigOptions.BotSecret = $values.BOTSECRETTOKEN -replace ‘[\"]’,''
$json.BotConfigOptions.AzureStorageConnectionString = $values.BotStorageep -replace ‘[\"]’,''
$json.NotificationHub.ConnectionString =$values.NotificationHubSRT -replace ‘[\"]’,''

$json | ConvertTo-Json | Out-File $location\Edison.Devices\Edison.Simulators.Sensors\appsettings.json
