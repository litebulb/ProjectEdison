#Azure Container Registry Configuration
$azureSubscriptionId = '00000000-0000-0000-0000-000000000000'
$acrResourceGroup = 'edisondev'
$acrSPName = 'SPEdisonDevContainerRegistry'
$acrSPClientId = '00000000-0000-0000-0000-000000000000'
$acrSPPassword = '00000000-0000-0000-0000-000000000000'
$acrContainerRegistryName = 'edisondevcontainerregistry'
$acrContainerRegistryUrl = 'edisondevcontainerregistry.azurecr.io'
$acrAccountEmail = 'email@company.com'

#Registering Azure Container Registry Credentials
#First create a service principal first
#az ad sp create-for-rbac --scopes /subscriptions/$azureSubscriptionId/resourcegroups/$acrResourceGroup/providers/Microsoft.ContainerRegistry/registries/$acrContainerRegistryName --role Contributor --name $acrSPName
#kubectl create secret docker-registry acr-authentication --docker-server=$acrContainerRegistryUrl --docker-email=$acrAccountEmail --docker-username=$acrSPClientId --docker-password=$acrSPPassword

#Registering Config and Secrets
Copy-Item "config-edisonadminportal.json" -Destination "config.json"
kubectl create configmap config-edisonadminportal --from-file=./config.json
Remove-Item "config.json"

Copy-Item "config-edisonapi.json" -Destination "config.json"
kubectl create configmap config-edisonapi --from-file=./config.json
Remove-Item "config.json"

Copy-Item "config-edisonworkflows.json" -Destination "config.json"
kubectl create configmap config-edisonworkflows --from-file=./config.json
Remove-Item "config.json"

Copy-Item "config-edisoneventprocessorservice.json" -Destination "config.json"
kubectl create configmap config-edisoneventprocessorservice --from-file=./config.json
Remove-Item "config.json"

Copy-Item "config-edisondevicesynchronizationservice.json" -Destination "config.json"
kubectl create configmap config-edisondevicesynchronizationservice --from-file=./config.json
Remove-Item "config.json"

Copy-Item "config-edisoniothubcontrollerservice.json" -Destination "config.json"
kubectl create configmap config-edisoniothubcontrollerservice --from-file=./config.json
Remove-Item "config.json"

Copy-Item "config-edisonsignalrservice.json" -Destination "config.json"
kubectl create configmap config-edisonsignalrservice --from-file=./config.json
Remove-Item "config.json"

Copy-Item "config-edisonchatservice.json" -Destination "config.json"
kubectl create configmap config-edisonchatservice --from-file=./config.json
Remove-Item "config.json"

Copy-Item "config-edisonresponseservice.json" -Destination "config.json"
kubectl create configmap config-edisonresponseservice --from-file=./config.json
Remove-Item "config.json"

Copy-Item "config-edisonmessagedispatcherservice.json" -Destination "config.json"
kubectl create configmap config-edisonmessagedispatcherservice --from-file=./config.json
Remove-Item "config.json"

Copy-Item "config-edisondeviceprovisioning.json" -Destination "config.json"
kubectl create configmap config-edisondeviceprovisioning --from-file=./config.json
Remove-Item "config.json"

Copy-Item "Secrets/common.secrets" -Destination "secrets.json"
kubectl create secret generic secrets-common --from-file=./secrets.json
Remove-Item "secrets.json"

kubectl create secret generic rabbitmq-credentials --from-literal=Username=Admin --from-literal=Password=Edison1234