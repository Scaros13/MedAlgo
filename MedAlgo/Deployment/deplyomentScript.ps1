$resourceGroupName = "rg-dev"
$functionAppName = "MedAlgo"
$storageAccountName = "storagerbdev"
$location = "northeurope"
$runtime = "powershell"
$zipPath = "C:\Artifact\artifact.zip"
$publishPath = ".\bin\Debug\net6.0\publish"

az login

# Optional resource creation
#az group create --name $resourceGroupName --location $location
#az storage account create --name $storageAccountName --location $location --resource-group $resourceGroupName --sku Standard_LRS

func init $functionAppName --worker-runtime $runtime
func new --name HttpTrigger --template "HTTP trigger"

Compress-Archive -Path $publishPath -DestinationPath $zipPath

az functionapp create --resource-group $resourceGroupName --consumption-plan-location $location --runtime $runtime --functions $functionAppName --storage-account $storageAccountName
az functionapp deployment source config-zip --resource-group $resourceGroupName --name $functionAppName --src $zipPath

