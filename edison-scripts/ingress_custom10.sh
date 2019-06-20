GITPATH=`pwd`
GIT_DIRPATH="$GITPATH/ProjectEdison/Edison.Web/Kubernetes/qa/Deployment"
GIT_INGRESS="$GIT_DIRPATH/Ingress_Custom"
LOG="/tmp/deployhelm.log.`date +%d%m%Y_%T`"

#1-InstallNGINXIngress

ls $GIT_INGRESS
if [ $? -eq 0 ]
then
        echo "------------------------------------" >> $LOG
        echo "The $GIT_INGRESS exists" >> $LOG
        cd $GIT_INGRESS

#  Install Helm

    curl https://raw.githubusercontent.com/helm/helm/master/scripts/get > get_helm.sh
    sleep 10
    chmod +x get_helm.sh
    ./get_helm.sh
    sleep 20 

# Run helm init for first time on your cluster

    helm init 
    sleep 10
    kubectl get po --all-namespaces | grep tiller



else
        echo "------------------------------------" >> $LOG
        echo "The $GIT_INGRESS doesn't exist " >> $LOG
fi

