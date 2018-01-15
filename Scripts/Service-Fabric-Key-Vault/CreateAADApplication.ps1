# Login to Azure Resource Manager
Login-AzureRmAccount

# Select a default subscription for your current session in case your account has multiple Azure subscriptions
Get-AzureRmSubscription –SubscriptionName "SUBSCRIPTION-NAME" | Select-AzureRmSubscription

# Variables
$pfxFile = $PSScriptRoot + '\KeyVaultCertificate.cer'
$displayName = "ServiceFabricTodoListApp"
$appUrl = "http://ServiceFabricTodoListApp"
$keyVaultName = "TodoListKeyVault"
$keyVaultResourceGroup = "TodoListKeyVaultResourceGroup"

# Get certificate from file
$x509 = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2
$x509.Import($pfxFile)

# Get certificate raw data in base64 format
$credValue = [System.Convert]::ToBase64String($x509.GetRawCertData())

# Create a new Azure AD application for TodoListApp
$adapp = New-AzureRmADApplication -DisplayName $displayName `
                                  -HomePage $appUrl `
                                  -IdentifierUris $appUrl `
                                  -CertValue $credValue `
                                  -StartDate $x509.NotBefore `
                                  -EndDate $x509.NotAfter

# Create a new Azure AD service proncipal for the Azure AD TodoListApp application
$sp = New-AzureRmADServicePrincipal -ApplicationId $adapp.ApplicationId

# Grants permissions for a user, application, or security group to perform operations with a key vault.
Set-AzureRmKeyVaultAccessPolicy -VaultName $keyVaultName `
                                -ResourceGroupName $keyVaultResourceGroup `
                                -ServicePrincipalName $adapp.ApplicationId `
                                -PermissionsToSecrets all

# get the thumbprint to use in your app settings
$x509.Thumbprint

# get the application id to use in your app settings
$adapp.ApplicationId