$resourceGroupName = "rg-dev"
$functionAppName = "MedAlgo"
$storageAccountName = "storagerbdev"
$location = "northeurope"
$runtime = "powershell"
$zipPath = "Path TODO"
$publishPath = "Path TODO"

az login

# Opcjonalne utworzenie zasobów
# az group create --name $resourceGroupName --location $location
# az storage account create --name $storageAccountName --location $location --resource-group $resourceGroupName --sku Standard_LRS

# Inicjalizacja funkcji bez tworzenia funkcji HttpTrigger
func init $functionAppName --worker-runtime $runtime --no-functions

# Kompresja archiwum
Compress-Archive -Path $publishPath\* -DestinationPath $zipPath

# Utworzenie funkcji
az functionapp create --resource-group $resourceGroupName --consumption-plan-location $location --runtime $runtime --functions 4 --name $functionAppName --storage-account $storageAccountName

# Wgranie archiwum
az functionapp deployment source config-zip --resource-group $resourceGroupName --name $functionAppName --src $zipPath
#az functionapp update --resource-group $resourceGroupName --name $functionAppName --set properties.runtimeVersion=6