# Copy the package and register application type of version 1.0
$connectionEndpoint = "bladerunner.westeurope.cloudapp.azure.com:19000"
$applicationTypeName = "TodoAppType"
$applicationName = "fabric:/TodoApp"
$ImageStoreConnectionString = "fabric:ImageStore"
$localFolder = $PSScriptRoot + '\TodoApp\pkg\Release' 
$applicationPathInImageStore = "TodoApp"
$applicationTypeVersion = "1.0.0"


Connect-ServiceFabricCluster -ConnectionEndpoint $connectionEndpoint

Copy-ServiceFabricApplicationPackage -ApplicationPackagePath "$localFolder\" `
                                     -ImageStoreConnectionString $ImageStoreConnectionString `
                                     -ApplicationPackagePathInImageStore $applicationPathInImageStore `

Register-ServiceFabricApplicationType -ApplicationPathInImageStore $applicationPathInImageStore

New-ServiceFabricApplication -ApplicationName $applicationName `
                             -ApplicationTypeName $applicationTypeName `
                             -ApplicationTypeVersion $applicationTypeVersion