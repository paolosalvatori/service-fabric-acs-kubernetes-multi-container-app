REM delete all pods and services withj label app=todoapi
kubectl delete pods,services,deployments -l app=todoapi

REM delete all pods and services withj label app=todoweb
kubectl delete pods,services,deployments -l app=todoweb