# Copy the package and register application type of version 1.0
$connectionEndpoint = "CLUSTER_NAME.CLUSTER_LOCATION.cloudapp.azure.com:19000"
$dockerComposeFile = $PSScriptRoot + '\servicefabric-docker-compose-from-docker-hub.yml'


Connect-ServiceFabricCluster -ConnectionEndpoint $connectionEndpoint 

New-ServiceFabricComposeDeployment -DeploymentName DockerComposeTodoApp -Compose $dockerComposeFile -RegistryUserName DOCKER_HUB_USERNAME -RegistryPassword DOCKER_HUB_PASSWORD