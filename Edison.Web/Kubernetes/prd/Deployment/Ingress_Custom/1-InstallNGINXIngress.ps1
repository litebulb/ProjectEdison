#Install Helm if necessary
#Run helm init the first time on your cluster
helm install --name nginx-ingress-admin stable/nginx-ingress --namespace kube-system --set rbac.create=true --set rbac.createRole=false --set rbac.createClusterRole=false --set controller.ingressClass=nginx-admin
helm install --name nginx-ingress-api stable/nginx-ingress --namespace kube-system --set rbac.create=true --set rbac.createRole=false --set rbac.createClusterRole=false --set controller.ingressClass=nginx-api