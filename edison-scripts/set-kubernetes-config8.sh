GITPATH=`pwd`
GIT_DIRPATH="$GITPATH/ProjectEdison/Edison.Web/Kubernetes/qa/Config"
LOG="/tmp/Config.log.`date +%d%m%Y_%T`"
#--------------------------------------------------------
#Azure Container Registry Configuration
azureSubscriptionId=`head -34 input.txt | awk -F "\"" '{print $2}'| tail -1`
acrResourceGroup=`head -35 input.txt | awk -F "\"" '{print $2}'| tail -1`
acrSPName=`head -28 input.txt | awk -F "\"" '{print $2}'| tail -1`
acrSPClientId=`head -28 input.txt | awk -F "\"" '{print $2}'| tail -1`
acrSPPassword=`head -29 input.txt | awk -F "\"" '{print $2}'| tail -1`
acrContainerRegistryName=`head -28 input.txt | awk -F "\"" '{print $2}'| tail -1`
acrContainerRegistryUrl=`head -27 input.txt | awk -F "\"" '{print $2}'| tail -1`
acrAccountEmail=`head -32 input.txt | awk -F "\"" '{print $2}'| tail -1`
#------------------------------------------------------------
# move the kubernetes script to config path for execution
ls $GIT_DIRPATH
if [ $? -eq 0 ]
then
        echo "------------------------------------" >> $LOG
        echo "The $GIT_DIRPATH exists" >> $LOG
        mv set-kubernetes-config8.sh $GIT_DIRPATH
        cd $GIT_DIRPATH
else
        echo "------------------------------------" >> $LOG
        echo "The $GIT_DIRPATH doesn't exist " >> $LOG
fi
#-----------------------------------------------------------------
#Registering Azure Container Registry Credentials
#First create a service principal first
az ad sp create-for-rbac --scopes /subscriptions/$azureSubscriptionId/resourcegroups/$acrResourceGroup/providers/Microsoft.ContainerRegistry/registries/$acrContainerRegistryName --role Contributor --name $acrSPName
kubectl create secret docker-registry acr-authentication --docker-server=$acrContainerRegistryUrl --docker-email=$acrAccountEmail --docker-username=$acrSPClientId --docker-password=$acrSPPassword

#Registering Config and Secrets
cp -rvf config-edisonadminportal.json config.json
kubectl create configmap config-edisonadminportal --from-file=./config.json
rm -rf config.json

cp -rvf config-edisonapi.json config.json
kubectl create configmap config-edisonapi --from-file=./config.json
rm -rf config.json

cp -rvf config-edisonworkflows.json config.json
kubectl create configmap config-edisonworkflows --from-file=./config.json
rm -rvf config.json

cp -rvf config-edisoneventprocessorservice.json config.json
kubectl create configmap config-edisoneventprocessorservice --from-file=./config.json
rm -rvf config.json

cp -rvf config-edisondevicesynchronizationservice.json config.json
kubectl create configmap config-edisondevicesynchronizationservice --from-file=./config.json
rm -rvf config.json

cp -rvf config-edisoniothubcontrollerservice.json config.json
kubectl create configmap config-edisoniothubcontrollerservice --from-file=./config.json
rm -rvf config.json

cp -rvf config-edisonsignalrservice.json config.json
kubectl create configmap config-edisonsignalrservice --from-file=./config.json
rm -rvf config.json

cp -rvf config-edisonchatservice.json config.json
kubectl create configmap config-edisonchatservice --from-file=./config.json
rm -rvf config.json

cp -rvf config-edisonresponseservice.json config.json
kubectl create configmap config-edisonresponseservice --from-file=./config.json
rm -rvf config.json

cp -rvf config-edisonmessagedispatcherservice.json config.json
kubectl create configmap config-edisonmessagedispatcherservice --from-file=./config.json
rm -rvf config.json

cp -rvf config-edisondeviceprovisioning.json config.json
kubectl create configmap config-edisondeviceprovisioning --from-file=./config.json
rm -rvf config.json

cp -rvf Secrets/common.secrets secrets.json
kubectl create secret generic secrets-common --from-file=./secrets.json
rm -rvf  secrets.json

kubectl create secret generic rabbitmq-credentials --from-literal=Username=Admin --from-literal=Password=Edison1234
