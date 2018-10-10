az login
az account set --subscription "EdisonBlueMetal"
Write-Host "Use Url http://localhost:8001/api/v1/namespaces/kube-system/services/kubernetes-dashboard/proxy/#"
az aks get-credentials --resource-group edison --name edisonakscluster
kubectl proxy