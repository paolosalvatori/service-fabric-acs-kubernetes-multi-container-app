REM Create a resource group for Kubernetes
az group create --name AcsKubernetesResourceGroup --location WestEurope --tags orchestrator=kubernetes

REM Create a Kubernetes cluster using ACS
az acs create --orchestrator-type kubernetes --name AcsKubernetes --resource-group AcsKubernetesResourceGroup --generate-ssh-keys --output jsonc

REM Install kubectl on the local machine
az acs kubernetes install-cli

REM Get credentials to connect to Kubernetes cluster using kubectl
az acs kubernetes get-credentials --name AcsKubernetes --resource-group AcsKubernetesResourceGroup 

REM Show the dashboard for a Kubernetes cluster in a web browser
az acs kubernetes browse --name AcsKubernetes --resource-group AcsKubernetesResourceGroup