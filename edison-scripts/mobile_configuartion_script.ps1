#clone the github repository
$location = "$1"

$Path = "$2"
$values = Get-Content $Path | Out-String | ConvertFrom-StringData
$values.TENANTID
$values.B2CDOMAIN
$values.SIGNUPSIGNINPOLICYID 
$values.AdDomainName
$values.ADCLIENTID 
$values.BASEURL_VALUE
$values.NotificationHubSRT
$values.NOTIFICATIONHUBPATH
$values.B2CCLIENTID
$values.package


# Create variables to store the path and endpoints.
$path2 = "$location\Edison.Mobile\Edison.Mobile.Common\Edison.Mobile.Common\Auth\AuthConfig.cs"
$path3 = "$location\Edison.Mobile\Edison.Mobile.Common\Edison.Mobile.Common\Shared\Constants.cs"
$path4 = "$location\Edison.Mobile\Edison.Mobile.User.Client\Edison.Mobile.User.Client.Core\Shared\Constants.cs"
$path5 = "$location\Edison.Mobile\Edison.Mobile.User.Client\Droid\Shared\Constants.cs"
$path6 = "$location\Edison.Mobile\Edison.Mobile.User.Client\iOS\Shared\Constants.cs"
$path7 = "$location\Edison.Mobile\Edison.Mobile.Admin.Client\Edison.Mobile.Admin.Client.Core\Shared\DeviceConfig.cs"
$path8 = "$location\Edison.Mobile\Edison.Mobile.Admin.Client\Droid\Shared\Constants.cs"
$path9 = "$location\Edison.Mobile\Edison.Mobile.Admin.Client\iOS\Shared\Constants.cs"
$path10 = "$location\Edison.Mobile\Edison.Mobile.User.Client\Droid\Properties\AndroidManifest.xml"
$path11 = "$location\Edison.Mobile\Edison.Mobile.User.Client\iOS\Info.plist"
$path12 = "$location\Edison.Mobile\Edison.Mobile.Admin.Client\Droid\Properties\AndroidManifest.xml"


# Update Edison.mobile.common\AuthConfig.cs file values
(Get-Content -path $path2 -Raw) -replace '1114b48d-24b1-4492-970a-d07d610a741c', $values.TENANTID.Trim('"') | Set-Content -Path $path2
(Get-Content -path $path2 -Raw) -replace 'edisondevb2c', $values.B2CDOMAIN.Trim('"') | Set-Content -Path $path2
(Get-Content -path $path2 -Raw) -replace 'b2c_1_edision_signinandsignup', $values.SIGNUPSIGNINPOLICYID.Trim('"') | Set-Content -Path $path2
(Get-Content -path $path2 -Raw) -replace 'com.onmicrosoft.edisonadmin', $values.AdDomainName.Trim('"') | Set-Content -Path $path2
(Get-Content -path $path2 -Raw) -replace '2373be1e-6d0b-4e38-9115-e0bd01dadd61', $values.ADCLIENTID.Trim('"') | Set-Content -Path $path2

# Update Edison.Mobile.Common\constants.cs file values
(Get-Content -path $path3 -Raw) -replace 'edisonapidev.eastus.cloudapp.azure.com', $values.BASEURL_VALUE.Trim('"') | Set-Content -Path $path3

# Update Edison.Mobile.User.Client.Core\Constants.cs file values
(Get-Content -path $path4 -Raw) -replace 'Endpoint=sb://edisondev.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=CNCM1xn79hHuUUj6GiAct1JJe5kdzGuPmzBOaVoSGsA=', $values.NotificationHubSRT.Trim('"') | Set-Content -Path $path4
(Get-Content -path $path4 -Raw) -replace 'edisondevnotificationhub', $values.NOTIFICATIONHUBPATH.Trim('"') | Set-Content -Path $path4

# Update Edison.Mobile.User.Client.Droid\Constants.cs file values
(Get-Content -path $path5 -Raw) -replace '19cb746c-3066-4cd8-8cd2-e0ce1176ae33', $values.B2CCLIENTID.Trim('"') | Set-Content -Path $path5

# Update Edison.Mobile.User.Client.iOS\Constants.cs file values
(Get-Content -path $path6 -Raw) -replace '19cb746c-3066-4cd8-8cd2-e0ce1176ae33', $values.B2CCLIENTID.Trim('"') | Set-Content -Path $path6

# Update Edison.Mobile.Admin.Client.Core\DeviceConfig.cs file values
(Get-Content -path $path7 -Raw) -replace 'edisonapidev.eastus.cloudapp.azure.com', $values.BASEURL_VALUE.Trim('"') | Set-Content -Path $path7

# Update Edison.Mobile.Admin.Client.Droid\Constants.cs file values
(Get-Content -path $path8 -Raw) -replace '2373be1e-6d0b-4e38-9115-e0bd01dadd61', $values.ADCLIENTID.Trim('"') | Set-Content -Path $path8

# Update Edison.Mobile.Admin.Client.iOS\Constants.cs file values
(Get-Content -path $path9 -Raw) -replace '2373be1e-6d0b-4e38-9115-e0bd01dadd61', $values.ADCLIENTID.Trim('"')| Set-Content -Path $path9

# Update Edison.Mobile.User.Client.Droid\AndroidManifest.xml file values
(Get-Content -path $path10 -Raw) -replace 'edisondevb2c', $values.B2CDOMAIN.Trim('"') | Set-Content -Path $path10
(Get-Content -path $path10 -Raw) -replace 'com.bluemetal.Edison_Mobile_User_Client', $values.package.Trim('"')  | Set-Content -Path $path10

# Update Edison.Mobile.User.Client.iOS\Info.plist file values
(Get-Content -path $path11 -Raw) -replace 'edisondevb2c', $values.B2CDOMAIN.Trim('"') | Set-Content -Path $path11

# Update Edison.Mobile.Admin.Client.Droid\AndroidManifest.xml file values 
(Get-Content -path $path12 -Raw) -replace '2373be1e-6d0b-4e38-9115-e0bd01dadd61', $values.ADCLIENTID.Trim('"') | Set-Content -Path $path12
(Get-Content -path $path12 -Raw) -replace 'com.onmicrosoft.edisonbluemetal', $values.B2CDOMAIN.Trim('"') | Set-Content -Path $path12