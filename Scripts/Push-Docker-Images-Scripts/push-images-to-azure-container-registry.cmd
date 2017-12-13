REM Login to the newly created Azure Container Registry
call az acr login --name AZURE_CONTAINER_REGISTRY

REM Each container image needs to be tagged with the loginServer name of the registry. 
REM This tag is used for routing when pushing container images to an image registry.
REM Save the loginServer name to the AKS_CONTAINER_REGISTRY environment variable
for /f "delims=" %%a in ('call az acr list --resource-group ContainerRegistryResourceGroup --query "[].{acrLoginServer:loginServer}" --output tsv') do @set AKS_CONTAINER_REGISTRY=%%a

REM tag the local todoapi:latest image with the loginServer of the container registry
docker tag todoapi:latest %AKS_CONTAINER_REGISTRY%/todoapi:v1

REM publish <container registry>/todoapi:latest to the container registry on Azure
docker push %AKS_CONTAINER_REGISTRY%/todoapi:v1

REM tag the local todoweb:latest image with the loginServer of the container registry
docker tag todoweb:latest %AKS_CONTAINER_REGISTRY%/todoweb:v1

REM publish <container registry>/todoweb:latest to the container registry on Azure
docker push %AKS_CONTAINER_REGISTRY%/todoweb:v1

REM List images in the container registry on Azure
call az acr repository list --name AZURE_CONTAINER_REGISTRY --output table