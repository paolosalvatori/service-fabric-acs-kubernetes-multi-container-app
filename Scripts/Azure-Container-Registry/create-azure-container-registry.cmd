REM Create a resource group for the Azure Container Registry
az group create --name ContainerRegistryResourceGroup --location westus2 --output jsonc

REM Create an Azure Container Registry. The name of the Container Registry must be unique
az acr create --resource-group ContainerRegistryResourceGroup --name AZURE_CONTAINER_REGISTRY --sku Basic --admin-enabled true

REM Login to the newly created Azure Container Registry
az acr login --name AZURE_CONTAINER_REGISTRY