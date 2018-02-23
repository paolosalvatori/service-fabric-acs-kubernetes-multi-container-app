REM Create a resource group for Kubernetes
az group create --name AksKubernetesResourceGroup --location westus2 --output jsonc

REM Create a Kubernetes cluster using ACS
az aks create --resource-group AksKubernetesResourceGroup --name AksKubernetes --agent-count 3 --generate-ssh-keys --output jsonc

REM Install kubectl on the local machine
az aks install-cli

REM Get credentials to connect to Kubernetes cluster using kubectl
az aks get-credentials --name AksKubernetes --resource-group AksKubernetesResourceGroup 

REM Show the dashboard for a Kubernetes cluster in a web browser
az aks browse --name AksKubernetes --resource-group AksKubernetesResourceGroup