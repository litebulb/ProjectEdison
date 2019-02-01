$CommandPath = Split-Path $MyInvocation.InvocationName


Connect-AzureRmAccount -TenantId '1114b48d-24b1-4492-970a-d07d610a741c'

$secret = Get-AzureKeyVaultSecret -VaultName edisondevkeyvault -Name com-google-android-maps-v2-APIKEY

$secretPath = "$CommandPath\..\Edison.Mobile.User.Client\Droid\Resources\values\secrets.xml"

$exists = Test-Path $secretPath

if(-not $exists)
{
    Copy-Item -Path "$CommandPath\secrets-template.xml" -Destination $secretPath
}

[xml] $config = get-content  $secretPath
$item = $config.resources.string | ? { $_.name -eq "api_key" }
$item.InnerText = $secret.SecretValueText
Set-Content  $secretPath -Value $config.InnerXml -Force