#!/bin/bash
#Comment - connect to the cluster
#------------------------------------
TenantId=`head -31 input.txt | awk -F "\"" '{print $2}'| tail -1`
AzureUserId=`head -32 input.txt | awk -F "\"" '{print $2}'| tail -1`
AzurePwd=`head -33 input.txt | awk -F "\"" '{print $2}'| tail -1`
SubscriptionId=`head -34 input.txt | awk -F "\"" '{print $2}'| tail -1`
ResourceGroupName=`head -35 input.txt | awk -F "\"" '{print $2}'| tail -1`
ClusterName=`head -36 input.txt | awk -F "\"" '{print $2}'| tail -1`
#-----------------------------------------------------------
az login --tenant $TenantId --username $AzureUserId --password $AzurePwd
az account set -s $SubscriptionId
echo "Use Url http://localhost:8001/api/v1/namespaces/kube-system/services/kubernetes-dashboard/proxy/#"
az aks get-credentials --resource-group $ResourceGroupName --name $ClusterName
kubectl proxy