GITPATH=`pwd`
GIT_DIRPATH="$GITPATH/ProjectEdison/Edison.Web/Kubernetes/qa/Deployment"
GIT_INGRESS="$GIT_DIRPATH/Ingress_Custom"
BASEURL_VALUE=`head -24 input.txt | awk -F "\"" '{print $2}'| tail -1`
ADMINURL=`head -37 input.txt | awk -F "\"" '{print $2}'| tail -1`
ADMINSECRET=`head -38 input.txt | awk -F "\"" '{print $2}'| tail -1`
APISECRET=`head -39 input.txt | awk -F "\"" '{print $2}'| tail -1`
LOG="/tmp/deployhelm.log.`date +%d%m%Y_%T`"

#1- copy the certificates from local to host 
    su adminuser
    cd ~
    sudo apt install unzip
    sleep 20
    unzip Kubernetes_certs.zip
    ls
    cat 2289f3206db82816.crt gd_bundle-g2-g1.crt > kuberenets-cert.com.chained.crt
    sudo -i

#2-  Create the secret in the cluster

    cd /var/lib/waagent/custom-script/download/0
    kubectl create secret tls $ADMINSECRET --cert /home/adminuser/kuberenets-cert.com.chained.crt --key /home/adminuser/kuberenets-cert.key 
    kubectl create secret tls $APISECRET --cert /home/adminuser/kuberenets-cert.com.chained.crt --key /home/adminuser/kuberenets-cert.key
    kubectl get secrets

#3-update name of hosts and secrets
    
    sed -i -e 's/edisonadminportal.eastus.cloudapp.azure.com/'$ADMINURL'/g' ProjectEdison/Edison.Web/Kubernetes/qa/Deployment/Ingress_Custom/nginx-config-adminportal.yaml
    sed -i -e 's/tls-secret-adminportal/'$ADMINSECRET'/g' ProjectEdison/Edison.Web/Kubernetes/qa/Deployment/Ingress_Custom/nginx-config-adminportal.yaml
    sed -i -e 's/edisonapi.eastus.cloudapp.azure.com/'$BASEURL_VALUE'/g' ProjectEdison/Edison.Web/Kubernetes/qa/Deployment/Ingress_Custom/nginx-config-api.yaml
    sed -i -e 's/tls-secret-api/'$APISECRET'/g' ProjectEdison/Edison.Web/Kubernetes/qa/Deployment/Ingress_Custom/nginx-config-api.yaml
    sleep 10

#4-InstallIngressControllers

    az network public-ip create -g $MC_MO_basic_2304_akswih6_eastus2 -n $adminbotip --dns-name $dnsbotapi --allocation-method static
    sleep 10
    helm install --name nginx-ingress-admin stable/nginx-ingress --namespace kube-system --set rbac.create=false --set rbac.createRole=false --set rbac.createClusterRole=false --set controller.ingressClass=nginx-admin --set controller.service.loadBalancerIP=$ip
    sleep 10
    az network public-ip create -g $MC_MO_basic_2304_akswih6_eastus2 -n $apibotip --dns-name $dnsbotapi1 --allocation-method static
    sleep 10
    helm install --name nginx-ingress-api stable/nginx-ingress --namespace kube-system --set rbac.create=false --set rbac.createRole=false --set rbac.createClusterRole=false --set controller.ingressClass=nginx-api --set controller.service.loadBalancerIP=$ip
    sleep 20
    sleep 20
    cd $GIT_INGRESS
    kubectl create -f ./nginx-config-adminportal.yaml
    sleep 20s
    kubectl create -f ./nginx-config-api.yaml
    sleep 20s

    kubectl get ing
    kubectl get svc -n kube-system
    sleep 10s
