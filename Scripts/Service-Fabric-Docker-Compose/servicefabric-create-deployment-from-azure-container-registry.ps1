# Copy the package and register application type of version 1.0
$connectionEndpoint = "bladerunner.westeurope.cloudapp.azure.com:19000"
$dockerComposeFile = $PSScriptRoot + '\servicefabric-docker-compose-from-azure-container-registry.yml'


Connect-ServiceFabricCluster -ConnectionEndpoint $connectionEndpoint 

New-ServiceFabricComposeDeployment -DeploymentName DockerComposeTodoApp -Compose $dockerComposeFile -RegistryUserName AZURE_CONTAINER_REGISTRY_USERNAME -RegistryPassword AZURE_CONTAINER_REGISTRY_PASSWORD