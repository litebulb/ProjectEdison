kubectl delete clusterissuer letsencrypt-prod
kubectl delete certificate tls-secret-edisonadminportal
kubectl delete certificate tls-secret-edisonapi
kubectl apply -f .\nginx-tls-issuer.yaml
#kubectl apply -f .\certificate-edisonadminportal.yaml
#kubectl apply -f .\certificate-edisonadminapi.yaml