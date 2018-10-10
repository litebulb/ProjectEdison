kubectl delete clusterissuer letsencrypt-prod
kubectl create -f .\nginx-tls-issuer.yaml