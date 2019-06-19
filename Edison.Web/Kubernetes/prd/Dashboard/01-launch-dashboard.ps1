az account set --subscription "DI_NA_Solutions_012"
Write-Host "Use Url http://localhost:8001/api/v1/namespaces/kube-system/services/kubernetes-dashboard/proxy/#"
az aks get-credentials --resource-group edison_d_001 --name edisonakscluster
kubectl proxy