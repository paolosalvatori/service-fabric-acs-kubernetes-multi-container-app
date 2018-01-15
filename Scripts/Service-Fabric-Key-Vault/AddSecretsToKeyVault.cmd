REM add Cosmos DB Endpoint URI secret to Key Vault
call az keyvault secret set --name RepositoryService--CosmosDb--EndpointUri --vault-name TodoListKeyVault  --value "COSMOS_DB_ENDPOINT_URI" --description "Cosmos DB endpoint URI"

REM add Cosmos DB Primary Key secret to Key Vault
call az keyvault secret set --name RepositoryService--CosmosDb--PrimaryKey --vault-name TodoListKeyVault  --value "COSMOS_DB_PRIMARY_KEY --description "Cosmos DB primary key"

REM add Cosmos DB Database Name secret to Key Vault
call az keyvault secret set --name RepositoryService--CosmosDb--DatabaseName --vault-name TodoListKeyVault  --value "COSMOS_DB_DATABASE_NAME" --description "Cosmos DB database name"

REM add Cosmos DB Collection Name secret to Key Vault
call az keyvault secret set --name RepositoryService--CosmosDb--CollectionName --vault-name TodoListKeyVault  --value "COSMOS_DB_COLLECTION_NAME" --description "Cosmos DB collection name"

REM add Service Bus Connection String secret to Key Vault
call az keyvault secret set --name NotificationService--ServiceBus--ConnectionString --vault-name TodoListKeyVault  --value "SERVICE_BUS_CONNECTION_STRING" --description "Service Bus connection string"

REM add Data Protection Blob Storage Connection String secret to Key Vault
call az keyvault secret set --name DataProtection--BlobStorage--ConnectionString --vault-name TodoListKeyVault  --value "STORAGE_ACCOUNT_CONNECTION_STRING" --description "Data Protection blob storage connection string"

REM add Application Insights Instrumentation Key secret to Key Vault
call az keyvault secret set --name ApplicationInsights--InstrumentationKey --vault-name TodoListKeyVault  --value "APPLICATION_INSIGHTS_INSTRUMENTATION_KEY" --description "Application Insights instrumentation key"

REM List secrets in Key Vault
call az keyvault secret list --vault-name TodoListKeyVault --output table