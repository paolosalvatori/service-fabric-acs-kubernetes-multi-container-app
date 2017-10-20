---
services: service-fabric, container-service, kubernetes, cosmos-db, service-bus, application-insights, ontainer-registry, dns
platforms: docker, dotnet-core, aspnet-core
author: paolosalvatori
---

# Multi-Container Sample with Service Fabric and Kubernetes #
This sample demonstrates how create a multi-container application using [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/) and [Docker](https://www.docker.com/) and deploy it on an [Azure Service Fabric Linux cluster](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-deploy-anywhere) with the [DNS Service](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-dnsservice) or on an [Azure Container Service Kubernetes cluster on Linux](https://docs.microsoft.com/en-us/azure/container-service/kubernetes/container-service-intro-kubernetes).

# Azure Service Fabric and ACS/Kubernetes Environments #
This repository contains a sample multi-container application and the scripts to deploy it on the following environments: 

- Service Fabric Linux cluster on Azure with the DNS service.
- Azure Container Service Kubernetes cluster on Linux 

# Prerequisites for development machine #

1. Install [Microsoft Visual Studio 2017](https://www.visualstudio.com/) with .NET Core workload. For more information, see [Visual Studio Tools for Docker](https://docs.microsoft.com/en-us/aspnet/core/publishing/visual-studio-tools-for-docker).
2. Install [Docker for Windows](https://docs.docker.com/docker-for-windows/install/) and configure it to use Linux containers.
2. [Set up the developer environment](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-get-started-linux) 
3. Create a **Service Fabric Linux cluster on Azure** with a minimum of five nodes and the [DNS Service](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-dnsservice) enabled. The demo requires a cluster running on Azure. For the purposes of this sample, you can eventually use the [party cluster](https://try.servicefabric.azure.com/).
4. Clone or download this container solution into a directory on the local machine.
5. Specify the value of the application parameters (see the **Configuration** section for more information). 


# Visual Studio Solution #
The solution has three projects. 

- **TodoWeb**: this project is an ASP.NET Core Web application that represents the frontend of the solution. The user interface is composes of a set of Razor pages that can be used to browse, create, delete, update and see the details of a collection of todo items stored in a Document DB collection. The frontend service is configured to send logs, events, traces, requests, dependencies and exceptions to **Application Insights**. 
- **TodoApi**: this project contains an ASP.NET Core REST service that is invoked by the **TodoWeb** frontend service to access the data stroed in the Document DB database. Each time a CRUD operation is performed by any of the methods exposed bu the **TodoController**, the backend service sends a notification message to a **Service Bus queue**. You can use my [Service Bus Explorer](https://github.com/paolosalvatori/ServiceBusExplorer) to read messages from the queue. The frontend service is configured to send logs, events, traces, requests, dependencies and exceptions to **Application Insights**. The backend service adopts [Swagger](https://swagger.io/) ro expose a machine-readable representation of its RESTful API.
- **TodoApp**: this project contains a **Service Fabric** application that is used to deploy the containerized services. Before deploying the application to an **Azure Service Fabric Cluster on Linux**, make sure to configure  the value of the parameters used by the frontend and backend services in the  **Cloud.xml** file under the **ApplicationParameters** folder.

The solution also contains the following solution some folders with the scripts used to deploy the application to a **Service Fabric Linux** cluster or an **Azure Container Service Kubernetes** cluster on Azure.

**Note**: both the frontend and backend containerized services use the **microsoft/aspnetcore:2.0** as base Docker image. For more information, see [Official .NET Docker images](https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/net-core-net-framework-containers/official-net-docker-images)
 
# Service Fabric Architecture #

The following picture shows the architecture of the **Service Fabric** application.

![Architecture](Images/ServiceFabricArchitecture.png)  

# Kubernetes Architecture #

The following picture shows the architecture of the **Kubernetes** application running on **Azure Container Service**.

![Architecture](Images/KubernetesArchitecture.png) 

# Configuration #
In ASP.NET Core, the configuration API provides a way of configuring an app based on a list of name-value pairs. Configuration is read at runtime from multiple sources. The name-value pairs can be grouped into a multi-level hierarchy. There are configuration providers for: 

- File formats (INI, JSON, and XML)
- ommand-line arguments
- Environment variables
- In-memory .NET objects
- An encrypted user store
- Azure Key Vault
- Custom providers, which you install or create

## TodoApi Service Configuration ##
The following table contains the configuration of the TodoApi service defined in the **appsettings.json** file. 

```json
{
    "RepositoryService": {
        "CosmosDb": {
            "EndpointUri": "",
            "PrimaryKey": "",
            "DatabaseName": "",
            "CollectionName": ""
        }
    },
    "NotificationService": {
        "ServiceBus": {
            "ConnectionString": "",
            "QueueName": ""
        }
    },
    "ApplicationInsights": {
        "InstrumentationKey": ""
    },
    "Logging": {
        "IncludeScopes": false,
        "Debug": {
            "LogLevel": {
                "Default": "Information"
            }
        },
        "Console": {
            "LogLevel": {
                "Default": "Information"
            }
        },
        "EventSource": {
            "LogLevel": {
                "Default": "Warning"
            }
        },
        "ApplicationInsights": {
            "LogLevel": {
                "Default": "Information"
            }
        }
    }
}
```

**Notes**

- The **RepositoryService** element contains the **EndpointUri**, **PrimaryKey**, **DatabaseName** and **CollectionName** of the DocumentDB database holding the data.
- The **NotificationService** element contains the **ConnectionString** and **QueueName** of the Service Bus queue where the backend service sends a message any time a CRUD is performed on a document.
- The **Application Insights** element contains the **InstrumentationKey** of the **Application Insights** used by the service for diagnostics, logging, performance monitoring, analytics and alerting.
- The **Logging*** element contains the log level for the various logging providers.

## TodoWeb Service Configuration ##
The following table contains the configuration of the TodoApi service defined in the **appsettings.json ** file. 

```json
{
  "TodoApiService": {
    "EndpointUri": ""
  },
  "ApplicationInsights": {
    "InstrumentationKey": ""
  },
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Warning"
    },
    "ApplicationInsights": {
      "LogLevel": {
        "Default": "Information"
      }
    }
  }  
}
```

**Notes**

- The **TodoApiService** element contains the **EndpointUri** of the **TodoApi**. In **Service Fabric** this setting will be the DNS names assigned to the **TodoApi** service. For more information, see [DNS Service in Azure Service Fabric](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-dnsservice). In **Kubernetes** this setting will contain the name of the **TodoApi** service. For more information on Kubernetes Services, see [Services](https://kubernetes.io/docs/concepts/services-networking/service/).
- The **Application Insights** element contains the **InstrumentationKey** of the **Application Insights** used by the service for diagnostics, logging, performance monitoring, analytics and alerting.
- The **Logging*** element contains the log level for the various logging providers.

## How Configuration works in ASP.NET Core ##
The [CreateDefaultBuilder](https://andrewlock.net/exploring-program-and-startup-in-asp-net-core-2-preview1-2/) extension method in an ASP.NET Core 2.x app adds configuration providers for reading JSON files and system configuration sources:

- appsettings.json
- appsettings.<EnvironmentName>.json
- environment variables

Configuration consists of a hierarchical list of name-value pairs in which the nodes are separated by a colon. To retrieve a value, access the Configuration indexer with the corresponding item's key. For example, if you want to retrieve the value of the **QueueName** setting from the configuration of the **TodoApi** service, you have to use the following format.

```csharp
var queueName = Configuration["NotificationService:ServiceBus:QueueName"];
```

If you want to create an environment variable to provide a value for a setting defined in the **appsettings.json** file, you can replace : (colon) with __ (double underscore).

```Dockerfile
NotificationService__ServiceBus__QueueName=todoapi
```


The [CreateDefaultBuilder](https://andrewlock.net/exploring-program-and-startup-in-asp-net-core-2-preview1-2/)  helper method specifies environment variables last, so that the local environment can override anything set in deployed configuration files. This allows allows to define settings in the **appsettings.json** file, but leave their value empty, and specify their value using environment variables. 

For more information on configuration, see the following resources:

- [Configuration in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration?tabs=basicconfiguration)
- [Managing ASP.NET Core App Settings on Kubernetes](https://anthonychu.ca/post/aspnet-core-appsettings-secrets-kubernetes/)
- [ASP.NET Core and Docker Environment Variables](https://www.scottbrady91.com/Docker/ASPNET-Core-and-Docker-Environment-Variables)

## How to define the Docker images and containers ##
Using [Visual Studio Tools for Docker](https://docs.microsoft.com/en-us/aspnet/core/publishing/visual-studio-tools-for-docker), I built an image based the on the **microsoft/aspnetcore:2.0** standard image.
Then, the tool creates a Dockerfile for both the frontend and backend service that you can later customize at will. The Dockerfile contains instructions for setting up the environment inside your container, loading the application you want to run, and mapping ports. The Dockerfile is the input to the docker build command, which creates the image. Below you can see the **Dockerfile** of the **TodoApi** and **TodoWeb** services.

**TodoApi Dockerfile**
```dockerfile
FROM microsoft/aspnetcore:2.0
ARG source
WORKDIR /app
EXPOSE 80
COPY ${source:-obj/Docker/publish} .
ENTRYPOINT ["dotnet", "TodoApi.dll"]

```

**TodoWeb Dockerfile** 
```dockerfile
FROM microsoft/aspnetcore:2.0
ARG source
WORKDIR /app
EXPOSE 80
COPY ${source:-obj/Docker/publish} .
ENTRYPOINT ["dotnet", "TodoWeb.dll"]

```

The [Visual Studio Tools for Docker](https://docs.microsoft.com/en-us/aspnet/core/publishing/visual-studio-tools-for-docker) also creates the **docker-compose.yml** and **docker-compose-override.yml** files that you can use to test the application locally.

**docker-compose.yml**
```yaml
version: '3'

services:
  todoapi:
    image: todoapi
    build:
      context: ./TodoApi
      dockerfile: Dockerfile

  todoweb:
    image: todoweb
    build:
      context: ./TodoWeb
      dockerfile: Dockerfile

```

**docker-compose-override.yml**
```yaml
version: '3'

services:
  todoapi:
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - RepositoryService__CosmosDb__EndpointUri=https://todoapi.documents.azure.com:443/
      - RepositoryService__CosmosDb__PrimaryKey=Document-Db-Primary-Key
      - RepositoryService__CosmosDb__DatabaseName=TodoApiDb
      - RepositoryService__CosmosDb__CollectionName=TodoApiCollection
      - NotificationService__ServiceBus__ConnectionString=Azure-Service-Bus-Connection-String
      - NotificationService__ServiceBus__QueueName=todoapi
      - ApplicationInsights__InstrumentationKey=Application-Insights-Instrumentation-Key

    ports:
      - "80"

  todoweb:
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - TodoApiService__EndpointUri=todoapi
      - ApplicationInsights__InstrumentationKey=Application-Insights-Instrumentation-Key
    ports:
      - "80"
```

## Deploy the service images to Docker Hub ##
You can execute the following command file to tag and register the images in your repository on [Docker Hub](https://hub.docker.com).

**publish-images-to-docker-hub.cmd**
```xml
REM login to docker hub
docker login -u username -p password

REM tag the local todoapi:latest image into your repository on docker hub with latest tag
docker tag todoapi:latest repository/todoapi:latest

REM publish repository/todoapi:latest to your repository on docker hub
docker push repository/todoapi:latest

REM tag the local todoweb:latest image into your repository with latest tag
docker tag todoweb:latest repository/todoweb:latest

REM publish repository/todoweb:latest to your repository on docker hub
docker push repository/todoweb:latest

REM browse to https://hub.docker.com/r/repository/
start chrome https://hub.docker.com/r/repository/
```

Alternatively, you can register and deploy your images from the [Azure Container Registry](https://docs.microsoft.com/en-gb/azure/container-registry/container-registry-intro).

## Service Fabric Deployment with Application Manifest and Service Manifests ## 
Here are the complete service and application manifests used in the Service Fabric solution.

**TodoApi ServiceManifest.xml**
```xml
<?xml version="1.0" encoding="utf-8"?>
<ServiceManifest Name="TodoApiPkg"
                 Version="1.0.0"
                 xmlns="http://schemas.microsoft.com/2011/01/fabric"
                 xmlns:xsd="http://www.w3.org/2001/XMLSchema"
                 xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <ServiceTypes>
    <!-- This is the name of your ServiceType.
         The UseImplicitHost attribute indicates this is a guest service. -->
    <StatelessServiceType ServiceTypeName="TodoApiType" UseImplicitHost="true" />
  </ServiceTypes>

  <!-- Code package is your service executable. -->
  <CodePackage Name="Code" Version="1.0.0">
    <EntryPoint>
      <!-- Follow this link for more information about deploying Windows containers to Service Fabric: https://aka.ms/sfguestcontainers -->
      <ContainerHost>
        <ImageName>paolosalvatori/todoapi</ImageName>
      </ContainerHost>
    </EntryPoint>
    <!-- Pass environment variables to your container: -->
    <EnvironmentVariables>
      <EnvironmentVariable Name="ASPNETCORE_ENVIRONMENT" Value=""/>
      <EnvironmentVariable Name="RepositoryService__CosmosDb__EndpointUri" Value=""/>
      <EnvironmentVariable Name="RepositoryService__CosmosDb__PrimaryKey" Value=""/>
      <EnvironmentVariable Name="RepositoryService__CosmosDb__DatabaseName" Value=""/>
      <EnvironmentVariable Name="RepositoryService__CosmosDb__CollectionName" Value=""/>
      <EnvironmentVariable Name="NotificationService__ServiceBus__ConnectionString" Value=""/>
      <EnvironmentVariable Name="NotificationService__ServiceBus__QueueName" Value=""/>
      <EnvironmentVariable Name="ApplicationInsights__InstrumentationKey" Value=""/>
    </EnvironmentVariables>
  </CodePackage>

  <!-- Config package is the contents of the Config directoy under PackageRoot that contains an 
       independently-updateable and versioned set of custom configuration settings for your service. -->
  <ConfigPackage Name="Config" Version="1.0.0" />

  <Resources>
    <Endpoints>
      <!-- This endpoint is used by the communication listener to obtain the port on which to 
           listen. Please note that if your service is partitioned, this port is shared with 
           replicas of different partitions that are placed in your code. -->
      <Endpoint Name="TodoApiEndpoint" Port="80" UriScheme="http" Protocol="http"/>
    </Endpoints>
  </Resources>
</ServiceManifest>
```
**TodoWeb ServiceManifest.xml**
```xml
<?xml version="1.0" encoding="utf-8"?>
<?xml version="1.0" encoding="utf-8"?>
<ServiceManifest Name="TodoWebPkg"
                 Version="1.0.0"
                 xmlns="http://schemas.microsoft.com/2011/01/fabric"
                 xmlns:xsd="http://www.w3.org/2001/XMLSchema"
                 xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <ServiceTypes>
    <!-- This is the name of your ServiceType.
         The UseImplicitHost attribute indicates this is a guest service. -->
    <StatelessServiceType ServiceTypeName="TodoWebType" UseImplicitHost="true" />
  </ServiceTypes>

  <!-- Code package is your service executable. -->
  <CodePackage Name="Code" Version="1.0.0">
    <EntryPoint>
      <!-- Follow this link for more information about deploying Windows containers to Service Fabric: https://aka.ms/sfguestcontainers -->
      <ContainerHost>
        <ImageName>paolosalvatori/todoweb</ImageName>
      </ContainerHost>
    </EntryPoint>
    <!-- Pass environment variables to your container: -->
    <EnvironmentVariables>
      <EnvironmentVariable Name="ASPNETCORE_ENVIRONMENT" Value=""/>
      <EnvironmentVariable Name="TodoApiService__EndpointUri" Value=""/>
      <EnvironmentVariable Name="ApplicationInsights__InstrumentationKey" Value=""/>
    </EnvironmentVariables>
  </CodePackage>

  <!-- Config package is the contents of the Config directoy under PackageRoot that contains an 
       independently-updateable and versioned set of custom configuration settings for your service. -->
  <ConfigPackage Name="Config" Version="1.0.0" />

  <Resources>
    <Endpoints>
      <!-- This endpoint is used by the communication listener to obtain the port on which to 
           listen. Please note that if your service is partitioned, this port is shared with 
           replicas of different partitions that are placed in your code. -->
      <Endpoint Name="TodoWebEndpoint" Port="8080" UriScheme="http" Protocol="http"/>
    </Endpoints>
  </Resources>
</ServiceManifest>
```
**ApplicationManifest.xml**
```xml
<?xml version="1.0" encoding="utf-8"?>
<ApplicationManifest ApplicationTypeName="TodoAppType"
                     ApplicationTypeVersion="1.0.0"
                     xmlns="http://schemas.microsoft.com/2011/01/fabric"
                     xmlns:xsd="http://www.w3.org/2001/XMLSchema"
                     xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <Parameters>
    <Parameter Name="TodoWeb_InstanceCount" DefaultValue="-1" />
    <Parameter Name="TodoWeb_ASPNETCORE_ENVIRONMENT" DefaultValue="Development"/>
    <Parameter Name="TodoWeb_TodoApiService__EndpointUri" DefaultValue=""/>
    <Parameter Name="TodoWeb_ApplicationInsights__InstrumentationKey" DefaultValue=""/>
    <Parameter Name="TodoApi_InstanceCount" DefaultValue="-1" />
    <Parameter Name="TodoApi_ASPNETCORE_ENVIRONMENT" DefaultValue="Development"/>
    <Parameter Name="TodoApi_RepositoryService__CosmosDb__EndpointUri" DefaultValue=""/>
    <Parameter Name="TodoApi_RepositoryService__CosmosDb__PrimaryKey" DefaultValue=""/>
    <Parameter Name="TodoApi_RepositoryService__CosmosDb__DatabaseName" DefaultValue=""/>
    <Parameter Name="TodoApi_RepositoryService__CosmosDb__CollectionName" DefaultValue=""/>
    <Parameter Name="TodoApi_NotificationService__ServiceBus__ConnectionString" DefaultValue=""/>
    <Parameter Name="TodoApi_NotificationService__ServiceBus__QueueName" DefaultValue=""/>
    <Parameter Name="TodoApi_ApplicationInsights__InstrumentationKey" DefaultValue=""/>
  </Parameters>
  <!-- Import the ServiceManifest from the ServicePackage. The ServiceManifestName and ServiceManifestVersion 
       should match the Name and Version attributes of the ServiceManifest element defined in the 
       ServiceManifest.xml file. -->
  <ServiceManifestImport>
    <ServiceManifestRef ServiceManifestName="TodoWebPkg" ServiceManifestVersion="1.0.0" />
    <ConfigOverrides />
    <EnvironmentOverrides CodePackageRef="Code">
      <EnvironmentVariable Name="ASPNETCORE_ENVIRONMENT" Value="[TodoWeb_ASPNETCORE_ENVIRONMENT]"/>
      <EnvironmentVariable Name="TodoApiService__EndpointUri" Value="[TodoWeb_TodoApiService__EndpointUri]"/>
      <EnvironmentVariable Name="ApplicationInsights__InstrumentationKey" Value="[TodoWeb_ApplicationInsights__InstrumentationKey]"/>
    </EnvironmentOverrides>
    <Policies>
      <ContainerHostPolicies CodePackageRef="Code">
        <RepositoryCredentials AccountName="paolosalvatori" Password="trustno1" PasswordEncrypted="false"/>
        <PortBinding ContainerPort="80" EndpointRef="TodoWebEndpoint" />
      </ContainerHostPolicies>
    </Policies>
  </ServiceManifestImport>
  <ServiceManifestImport>
    <ServiceManifestRef ServiceManifestName="TodoApiPkg" ServiceManifestVersion="1.0.0" />
    <ConfigOverrides />
    <EnvironmentOverrides CodePackageRef="Code">
      <EnvironmentVariable Name="ASPNETCORE_ENVIRONMENT" Value="[TodoApi_ASPNETCORE_ENVIRONMENT]"/>
      <EnvironmentVariable Name="RepositoryService__CosmosDb__EndpointUri" Value="[TodoApi_RepositoryService__CosmosDb__EndpointUri]"/>
      <EnvironmentVariable Name="RepositoryService__CosmosDb__PrimaryKey" Value="[TodoApi_RepositoryService__CosmosDb__PrimaryKey]"/>
      <EnvironmentVariable Name="RepositoryService__CosmosDb__DatabaseName" Value="[TodoApi_RepositoryService__CosmosDb__DatabaseName]"/>
      <EnvironmentVariable Name="RepositoryService__CosmosDb__CollectionName" Value="[TodoApi_RepositoryService__CosmosDb__CollectionName]"/>
      <EnvironmentVariable Name="NotificationService__ServiceBus__ConnectionString" Value="[TodoApi_NotificationService__ServiceBus__ConnectionString]"/>
      <EnvironmentVariable Name="NotificationService__ServiceBus__QueueName" Value="[TodoApi_NotificationService__ServiceBus__QueueName]"/>
      <EnvironmentVariable Name="ApplicationInsights__InstrumentationKey" Value="[TodoApi_ApplicationInsights__InstrumentationKey]"/>
    </EnvironmentOverrides>
    <Policies>
      <ContainerHostPolicies CodePackageRef="Code">
        <RepositoryCredentials AccountName="paolosalvatori" Password="trustno1" PasswordEncrypted="false"/>
        <PortBinding ContainerPort="80" EndpointRef="TodoApiEndpoint" />
      </ContainerHostPolicies>
    </Policies>
  </ServiceManifestImport>
  <DefaultServices>
    <!-- The section below creates instances of service types, when an instance of this 
         application type is created. You can also create one or more instances of service type using the 
         ServiceFabric PowerShell module.
         
         The attribute ServiceTypeName below must match the name defined in the imported ServiceManifest.xml file. -->
    <Service Name="TodoWeb" ServicePackageActivationMode="ExclusiveProcess" ServiceDnsName="todoweb.todoapp">
      <StatelessService ServiceTypeName="TodoWebType" InstanceCount="[TodoWeb_InstanceCount]">
        <SingletonPartition />
      </StatelessService>
    </Service>
    <Service Name="TodoApi" ServicePackageActivationMode="ExclusiveProcess" ServiceDnsName="todoapi.todoapp">
      <StatelessService ServiceTypeName="TodoApiType" InstanceCount="[TodoApi_InstanceCount]">
        <SingletonPartition />
      </StatelessService>
    </Service>
  </DefaultServices>
</ApplicationManifest>
```
**ApplicationParameteres\Cloud.xml**
```xml
<?xml version="1.0" encoding="utf-8"?>
<?xml version="1.0" encoding="utf-8"?>
<ServiceManifest Name="TodoWebPkg"
                 Version="1.0.0"
                 xmlns="http://schemas.microsoft.com/2011/01/fabric"
                 xmlns:xsd="http://www.w3.org/2001/XMLSchema"
                 xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <ServiceTypes>
    <!-- This is the name of your ServiceType.
         The UseImplicitHost attribute indicates this is a guest service. -->
    <StatelessServiceType ServiceTypeName="TodoWebType" UseImplicitHost="true" />
  </ServiceTypes>

  <!-- Code package is your service executable. -->
  <CodePackage Name="Code" Version="1.0.0">
    <EntryPoint>
      <!-- Follow this link for more information about deploying Windows containers to Service Fabric: https://aka.ms/sfguestcontainers -->
      <ContainerHost>
        <ImageName>paolosalvatori/todoweb</ImageName>
      </ContainerHost>
    </EntryPoint>
    <!-- Pass environment variables to your container: -->
    <EnvironmentVariables>
      <EnvironmentVariable Name="ASPNETCORE_ENVIRONMENT" Value=""/>
      <EnvironmentVariable Name="TodoApiService__EndpointUri" Value=""/>
      <EnvironmentVariable Name="ApplicationInsights__InstrumentationKey" Value=""/>
    </EnvironmentVariables>
  </CodePackage>

  <!-- Config package is the contents of the Config directoy under PackageRoot that contains an 
       independently-updateable and versioned set of custom configuration settings for your service. -->
  <ConfigPackage Name="Config" Version="1.0.0" />

  <Resources>
    <Endpoints>
      <!-- This endpoint is used by the communication listener to obtain the port on which to 
           listen. Please note that if your service is partitioned, this port is shared with 
           replicas of different partitions that are placed in your code. -->
      <Endpoint Name="TodoWebEndpoint" Port="8080" UriScheme="http" Protocol="http"/>
    </Endpoints>
  </Resources>
</ServiceManifest>
```
**Observations**:
- Both the **TodoApi** and **TodoWeb** containerized services are defined as stateless services.
- The **ApplicationManifest.xml** defines a **ServiceDnsName="todoapi.todoapp"** for the **TodoApi** service. This **DNS** name and port used by the **TodoApi** backend service are passed as value to the **TodoApiService__EndpointUri** (e.g. todoapi.todoapp:8081) environment variable and the value of the environment variable is used by the **TodoWeb** frontend service to create the http address of the  **TodoApi** backend service.
- The instance count for both services is equal to -1. This means that a container for each service is created on each **Service Fabric** cluster node.

The following picture shows the multi-container application using the **Service Fabric Explorer**.

![Manifests](Images/Manifests.png) 

## Service Fabric Deployment with Docker Compose ##  
**Docker** uses the **docker-compose.yml** file for defining multi-container applications. To make it easy for developers familiar with **Docker** to orchestrate existing container applications on **Service Fabric**, now you can use **docker compose** to deploy your multi-container application to a **Service Fabric** cluster on Azure. For more information, see [Docker Compose deployment support in Azure Service Fabric](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-docker-compose).

**Note**: Service Fabric can accept version 3 and later of docker-compose.yml files.

Below you can see the **servicefabric-docker-compose.yml** used to deploy the multi-container application to a **Service Fabric Linux Cluster on Azure with DNS Service**.

**servicefabric-docker-compose.yml**
```yaml
version: '3'

services:
  todoapi:
    image: paolosalvatori/todoapi:latest
    deploy:
      mode: replicated
      replicas: 5
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - RepositoryService__CosmosDb__EndpointUri=https://todoapi.documents.azure.com:443/
      - RepositoryService__CosmosDb__PrimaryKey=Document-Db-Primary-Key
      - RepositoryService__CosmosDb__DatabaseName=TodoApiDb
      - RepositoryService__CosmosDb__CollectionName=TodoApiCollection
      - NotificationService__ServiceBus__ConnectionString=Azure-Service-Bus-Connection-String
      - NotificationService__ServiceBus__QueueName=todoapi
      - ApplicationInsights__InstrumentationKey=Application-Insights-Instrumentation-Key
    ports:
      - "8081:80"

  todoweb:
    image: paolosalvatori/todoweb:latest
    deploy:
      mode: replicated
      replicas: 5
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - TodoApiService__EndpointUri=todoapi.todoapp:8081
      - ApplicationInsights__InstrumentationKey=Application-Insights-Instrumentation-Key
    ports:
      - "8082:80"
```
You can deploy the multi-container application to a **Service Fabric Linux Cluster on Azure with DNS service** using the [Azure Service Fabric CLI](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-cli) as shown in the following command file.

**servicefabric-create-deployment.cmd**
```Batchfile
REM Create a Service Fabric Compose deployment from a docker-compose.yml file
sfctl compose create --name DockerComposeTodoApp --file-path .\servicefabric-docker-compose.yml
```

Alternatively, you use the following PowerShell script to accomplish the same task.

**servicefabric-create-deployment.ps1**
```PowerShell
$connectionEndpoint = "bladerunner.westeurope.cloudapp.azure.com:19000"
$dockerComposeFile = $PSScriptRoot + '\servicefabric-docker-compose.yml'


Connect-ServiceFabricCluster -ConnectionEndpoint $connectionEndpoint 

New-ServiceFabricComposeDeployment -DeploymentName DockerComposeTodoApp -Compose $dockerComposeFile
```
**Observations**:
- Both the **TodoApi** and **TodoWeb** containerized services are defined as stateless services.
- The **ApplicationManifest.xml** defines a **ServiceDnsName="todoapi.todoapp"** for the **TodoApi** service. This **DNS** name and port used by the **TodoApi** backend service are passed as value to the **TodoApiService__EndpointUri** (e.g. todoapi.todoapp:8081) environment variable and the value of the environment variable is used by the **TodoWeb** frontend service to create the http address of the  **TodoApi** backend service.
- The instance count for both services is equal to -1. This means that a container for each service is created on each **Service Fabric** cluster node.

The following picture shows the multi-container application using the **Service Fabric Explorer**.

![Architecture](Images/DockerCompose.png) 

# Deploying the multi-container on ab Azure Container Service Kubernetes Cluster #
First of all, you have to create an **Azure Container Service Kubernetes cluster**. For more information, see [Deploy a Kubernetes cluster in Azure Container Service](https://docs.microsoft.com/en-us/azure/container-service/kubernetes/container-service-tutorial-kubernetes-deploy-cluster).

You can create a **Azure Container Service Kubernetes cluster** an using the [Azure CLI](https://azure.github.io/projects/clis/) and then use the [kubectl](https://kubernetes.io/docs/user-guide/kubectl-overview/) command line interface to run commands against the **Kubernetes** cluster on Azure.

**create-kubernetes-cluster.cmd**
```Batchfile
REM Create a resource group for Kubernetes
az group create --name AcsKubernetesResourceGroup --location WestEurope --tags orchestrator=kubernetes

REM Create a Kubernetes cluster using ACS
az acs create --orchestrator-type kubernetes --name AcsKubernetes --resource-group AcsKubernetesResourceGroup --generate-ssh-keys --output jsonc

REM Install kubectl on the local machine
az acs kubernetes install-cli

REM Get credentials to connect to Kubernetes cluster using kubectl
az acs kubernetes get-credentials --name AcsKubernetes --resource-group AcsKubernetesResourceGroup 

REM Launch the Kubernetes web UI 
az acs kubernetes browse --name AcsKubernetes --resource-group AcsKubernetesResourceGroup
```
The last command launches the Kubernetes web UI that allows to manage all the Kubernetes entities (services, pods, deployments, etc.).

![Architecture](Images/K8sDashboard.png) 

## Multi-Document Template File ##
On Kubernetes the multi-container application is composed by two services, one ofr the frontend service and one for the backend service, and 5 pods for each service. Each pod contains just a container or the **TodoApi** or **TodoWeb** ASP.NET Core apps. The following YAML file contains the definition for the necessary services and deployments.

**kubernetes-deployments-and-services.yml**
```yaml
apiVersion: apps/v1beta1
kind: Deployment
metadata:
  name: todoapi
  labels:
    app: todoapi
spec:
  replicas: 3
  selector:
    matchLabels:
      app: todoapi
  strategy:
    rollingUpdate:
      maxSurge: 1
      maxUnavailable: 1
  minReadySeconds: 5
  template:
    metadata:
      labels:
        app: todoapi
    spec:
      containers:
      - name: todoapi
        image: paolosalvatori/todoapi:latest
        ports:
        - containerPort: 80
        resources:
          requests:
            cpu: 250m
          limits:
            cpu: 500m
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Development"
        - name: RepositoryService__CosmosDb__EndpointUri
          value: "https://todoapi.documents.azure.com:443/"
        - name: RepositoryService__CosmosDb__PrimaryKey
          value: "Document-Db-Primary-Key"
        - name: RepositoryService__CosmosDb__DatabaseName
          value: "TodoApiDb"
        - name: RepositoryService__CosmosDb__CollectionName
          value: "TodoApiCollection"
        - name: NotificationService__ServiceBus__ConnectionString
          value: "Azure-Service-Bus-Connection-String"
        - name: NotificationService__ServiceBus__QueueName
          value: "todoapi"
        - name: ApplicationInsights__InstrumentationKey
          value: "Application-Insights-Instrumentation-Key"
---
apiVersion: v1
kind: Service
metadata:
  name: todoapi
spec:
  type: LoadBalancer
  ports:
  - protocol: TCP
    port: 80
  selector:
    app: todoapi
---
apiVersion: apps/v1beta1
kind: Deployment
metadata:
  name: todoweb
  labels:
    app: todoapi
spec:
  replicas: 3
  selector:
    matchLabels:
      app: todoweb
  strategy:
    rollingUpdate:
      maxSurge: 1
      maxUnavailable: 1
  minReadySeconds: 5 
  template:
    metadata:
      labels:
        app: todoweb
    spec:
      containers:
      - name: todoweb
        image: paolosalvatori/todoweb:latest
        ports:
        - containerPort: 80
        resources:
          requests:
            cpu: 250m
          limits:
            cpu: 500m
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Development"
        - name: TodoApiService__EndpointUri
          value: "todoapi"
        - name: ApplicationInsights__InstrumentationKey
          value: "Application-Insights-Instrumentation-Key"
---
apiVersion: v1
kind: Service
metadata:
  name: todoweb
spec:
  type: LoadBalancer
  ports:
  - protocol: TCP
    port: 80
  selector:
    app: todoweb
```
**Note**: in a Azure Container Service Kubernetes cluster, a Kubernetes service is implemented as a separate Load Balancer with a separate Public or Private IP.

The following script can be used to deploy the services and deployments.
```Batchfile
REM Deploy azure-vote sample application  
kubectl create --filename .\kubernetes-deployments-and-services.yml --record
```
[kubectl](https://kubernetes.io/docs/user-guide/kubectl-overview/) command line interface to list the newly created services and deployments, as shown in the following picture.

![Kubectl](Images/Kubectl.png) 

# Assign a Custom DNS to the public IP of the frontend service #
As you can see in the previous picture, I could use the public IP exposed by the **frontend** service, that is 13.93.46.58, to access the web UI. However, you can register a public domain using domain registrar like [GoDaddy][https://uk.godaddy.com/] and associate this public domain to the address exposed by the Kubernetes service using the Azure DNS service. In fact, the Azure DNS allows you to host a DNS zone and manage the DNS records for a domain in Azure. In order for DNS queries for a custom domain to reach Azure DNS, the domain has to be delegated to Azure DNS from the parent domain. Keep in mind Azure DNS is not the domain registrar. 

For more information on how to create an Azure DNS service and use the Azure CLI to manage zones and records, please see the following resources:

- [How to manage DNS Zones in Azure DNS using the Azure CLI 2.0](https://docs.microsoft.com/en-gb/azure/dns/dns-operations-dnszones-cli)
- [Manage DNS records and recordsets in Azure DNS using the Azure CLI 2.0](https://docs.microsoft.com/en-gb/azure/dns/dns-operations-recordsets-cli)

For more information to delegate a public domain to Azure DNS, see the following article:

- [Delegation of DNS zones with Azure DNS](https://docs.microsoft.com/en-us/azure/dns/dns-domain-delegation)
- [Delegate a domain to Azure DNS](https://docs.microsoft.com/en-us/azure/dns/dns-delegate-domain-azure-dns)

I personally proceeded as follows:
- I create a public domain using [GoDaddy][https://uk.godaddy.com/] called babosbird.com.
- Then I run the following script to create the **Azure DNS** and related record.

```Batchfile
REM Create a resource group for the DNS Zone and records
az group create --name DnsResourceGroup --location westeurope

REM Create a DNS zone called babosbird.com in the resource group DnsResourceGroup 
az network dns zone create --name babosbird.com --resource-group DnsResourceGroup --tags environment=K8s type=DNS

REM Create an A record for the public ip exposed by the public load balancer created for the TodoApi service in Kubernetes
az network dns record-set a add-record --resource-group DnsResourceGroup --zone-name babosbird.com --record-set-name www --ipv4-address 13.93.46.58
az network dns record-set a add-record --resource-group DnsResourceGroup --zone-name babosbird.com --record-set-name * --ipv4-address 13.93.46.58

REM View records
az network dns record-set list --resource-group DnsResourceGroup --zone-name babosbird.com

REM View records records at the top of the zone.
az network dns record-set list --resource-group DnsResourceGroup --zone-name babosbird.com --query "[?name=='@' && ends_with(type, 'NS')]|[0]"

REM Retrieve the name servers for your zone. These name servers should be configured with 
REM the domain name registrar where you purchased the domain name.
az network dns zone list --resource-group DnsResourceGroup --query "[?name=='babosbird.com']|[0].nameServers" --output json
```
- The last command in particular, can be used to retrieve the name servers. In fact, Azure DNS automatically creates authoritative NS records in your zone containing the assigned name servers. You need these name servers to delegate the custom domain to Azure DNS.
- Now that the DNS zone is created and you have the name servers, the parent domain needs to be updated with the Azure DNS name servers. Each registrar has their own DNS management tools to change the name server records for a domain. In the registrar's DNS management page, edit the NS records and replace the NS records with the ones Azure DNS created. In the following picture, you can see how I replaced the original NS records provided by GoDaddy with the names servers provided by the Azure DNS.
![GoDaddy](Images/godaddy.png)
- Execute the following command to make sure that it's the frontend service IP to respond to the public domain you delegated to your Azure DNS.
```Batchfile
nslookup -type=A www.babosbird.com
```
- Now I can browse the frontend web site using the custom domain I registered.
![CustomDomain](Images/publicdomain.png)

# Services #
This section provides a brief introduction of the services used by the solution.

## Azure Service Fabric ##
Azure Service Fabric is a distributed systems platform that makes it easy to package, deploy, and manage scalable and reliable microservices and containers. Service Fabric also addresses the significant challenges in developing and managing cloud native applications. Developers and administrators can avoid complex infrastructure problems and focus on implementing mission-critical, demanding workloads that are scalable, reliable, and manageable. Service Fabric represents the next-generation platform for building and managing these enterprise-class, tier-1, cloud-scale applications running in containers. For more information, see [Overview of Azure Service Fabric](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-overview).

## Kubernetes ##
Kubernetes is a powerful system for managing containerized applications in a clustered environment. It aims to provide better ways of managing related, distributed components across varied infrastructure.
Kubernetes is an open-source platform designed to automate deploying, scaling, and operating application containers. With Kubernetes, you are able to quickly and efficiently respond to customer demand:

- Deploy your applications quickly and predictably.
- Scale your applications on the fly.
- Self-healing
- Roll out new features seamlessly.
- Limit hardware usage to required resources only.

For more information, see [What is Kubernetes?](https://kubernetes.io/docs/concepts/overview/what-is-kubernetes/).

## Azure Container Service for Kubernetes ##
Azure Container Service for Kubernetes makes it simple to create, configure, and manage a cluster of virtual machines that are preconfigured to run containerized applications. This enables you to use your existing skills, or draw upon a large and growing body of community expertise, to deploy and manage container-based applications on Microsoft Azure. For more information, see [Azure Container Service for Kubernetes](https://docs.microsoft.com/en-us/azure/container-service/kubernetes/container-service-intro-kubernetes).


## Application Insights ##
Application Insights is an extensible Application Performance Management (APM) service for web developers on multiple platforms. Use it to monitor your live web application. It will automatically detect performance anomalies. It includes powerful analytics tools to help you diagnose issues and to understand what users actually do with your app. It's designed to help you continuously improve performance and usability. It works for apps on a wide variety of platforms including .NET, Node.js and J2EE, hosted on-premises or in the cloud. For more information, see [What is Application Insights?](https://docs.microsoft.com/en-us/azure/application-insights/app-insights-overview).

## Service Bus Messaging ##
Microsoft Azure Service Bus is a reliable message delivery service. The purpose of this service is to make communication easier. When two or more parties want to exchange data, they need a communication facilitator. Service Bus is a brokered, or third-party communication mechanism. Service Bus messaging with queues, topics, and subscriptions can be thought of as asynchronous, or "temporally decoupled." Producers (senders) and consumers (receivers) do not have to be online at the same time. The messaging infrastructure reliably stores messages in a "broker" (for example, a queue) until the consuming party is ready to receive them. For more information, see [Service Bus Messaging](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-messaging-overview).

## Azure Cosmos DB ##
Azure Cosmos DB is Microsoft's globally distributed, multi-model database. With the click of a button, Azure Cosmos DB enables you to elastically and independently scale throughput and storage across any number of Azure's geographic regions. Azure Cosmos DB supports multiple data models and popular APIs for accessing and querying data. The sample uses the Document DB API which provides a schema-less JSON database engine with SQL querying capabilities. For more information, see [About Azure Cosmos DB](https://docs.microsoft.com/en-us/azure/cosmos-db/introduction)

## Azure Container Registry ##
Azure Container Registry is a managed Docker registry service based on the open-source Docker Registry 2.0. Create and maintain Azure container registries to store and manage your private Docker container images. Use container registries in Azure with your existing container development and deployment pipelines, and draw on the body of Docker community expertise. For more information, see [Introduction to private Docker container registries in Azure](https://docs.microsoft.com/en-gb/azure/container-registry/container-registry-intro).

## Azure DNS Service ##
The Domain Name System, or DNS, is responsible for translating (or resolving) a website or service name to its IP address. Azure DNS is a hosting service for DNS domains, providing name resolution using Microsoft Azure infrastructure. By hosting your domains in Azure, you can manage your DNS records using the same credentials, APIs, tools, and billing as your other Azure services. For more information, see [Azure DNS overview](https://docs.microsoft.com/en-gb/azure/dns/dns-overview).

# More information
The [Service Fabric container documentation](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-linux-overview) provides details on the container features and scenarios.

The following are other useful links which contain more in depth information

- [Create a Linux Container Application](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-get-started-containers-linux)
- [Deploy an Azure Service Fabric Linux container application on Azure](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-quickstart-containers-linux)

The [Azure Container Service with Kubernetes Documentation](https://docs.microsoft.com/en-us/aspnet/core/tutorials/razor-pages/razor-pages-start) provides details on the container features and scenarios.

The following are other useful links which contain more in depth information

- [Deploy Kubernetes cluster for Linux containers](https://docs.microsoft.com/en-us/azure/container-service/kubernetes/container-service-kubernetes-walkthrough)
- [Using the Kubernetes web UI with Azure Container Service](https://docs.microsoft.com/en-us/azure/container-service/kubernetes/container-service-kubernetes-ui)
- [Tutorial: how to create and deploy a a multi-container application on ACS and Kubernetes](https://docs.microsoft.com/en-us/azure/container-service/kubernetes/container-service-tutorial-kubernetes-prepare-app)

For more information on Docker, see the following resources:

- [Introduction to Containers and Docker](https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/container-docker-introduction/index)
- [What is Docker?](https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/container-docker-introduction/docker-defined)
- [Building Docker Images for .NET Core Applications](https://docs.microsoft.com/en-us/dotnet/core/docker/building-net-docker-images)
- [Docker File Reference](https://docs.docker.com/engine/reference/builder/)
- [Docker Compose](https://docs.docker.com/compose/)
- [Securing .NET Microservices and Web Applications](https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/secure-net-microservices-web-applications/)

For more information on ASP.NET, see the following resources:

- [Configuration in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration?tabs=basicconfiguration)
- [Introduction to Logging in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging?tabs=aspnetcore2x)
- [Introduction to Error Handling in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/error-handling)
- [Introduction to Error Handling in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/mvc/razor-pages/?tabs=visual-studio)
- [Securing .NET Microservices and Web Applications](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x#tabpanel_fXL5YCOYDa_netcore2x)

# MSFT OSS Code Of Conduct Notice #

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
