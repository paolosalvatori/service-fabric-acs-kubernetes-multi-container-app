REM Create a resource group for Key Vault
call az group create --name TodoListKeyVaultResourceGroup --location WestEurope

REM Create Key Vault
call az keyvault create --name TodoListKeyVault --resource-group TodoListKeyVaultResourceGroup