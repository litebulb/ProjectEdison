kubectl config use-context docker-for-desktop
kubectl create -f 'https://raw.githubusercontent.com/kubernetes/dashboard/master/src/deploy/recommended/kubernetes-dashboard.yaml'
kubectl create -f dashboard-port-service.yaml
Write-Host "You can now access the dashboard at url: https://localhost:31000" -ForegroundColor Green
Write-Host "If bad certificate issue on Chrome: Allow invalid certificates for resources loaded from localhost." -BackgroundColor Yellow -ForegroundColor Blue