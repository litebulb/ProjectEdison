GITPATH=`pwd`
GIT_DIRPATH="$GITPATH/ProjectEdison/Edison.Web/Kubernetes/qa/Deployment"
LOG="/tmp/updateyaml.log.`date +%d%m%Y_%T`"
TAG=`head -30 input.txt | awk -F "\"" '{print $2}'| tail -1`
ACR_SERVERNAME=`head -27 input.txt | awk -F "\"" '{print $2}'| tail -1`

# Change Tags and ACR server name in deploy.yaml files
ls $GIT_DIRPATH
if [ $? -eq 0 ]
then
        echo "------------------------------------" >> $LOG
        echo "The $GIT_DIRPATH exists" >> $LOG
        cd $GIT_DIRPATH
        sed -i -e 's/latest/'$TAG'/g' edison.kubernetes.deploy.yaml
        sed -i -e 's/edisoncontainerregistry.azurecr.io/'$ACR_SERVERNAME'/g' edison.kubernetes.deploy.yaml
        kubectl create -f edison.kubernetes.deploy.yaml
        sleep 20s
        kubectl create -f edison.kubernetes.services.yaml
        sleep 40s
        echo "--------------------kubernetes pods-----------------------"
        kubectl get pods
        sleep 20s
        echo "--------------------kubernetes services-----------------------"
        kubectl get svc
        sleep 20s
        kubectl get svc -n kube-system >> services.txt
        sleep 10s
       
else
        echo "------------------------------------" >> $LOG
        echo "The $GIT_DIRPATH doenot exists" >> $LOG
fi