# Zmienne
$resourceGroupName = "rg-dev"
$functionAppName = "MedAlgo"
$storageAccountName = "storagerbdev"
$location = "northeurope"
$runtime = "powershell"

# Logowanie do Azure
az login

# Tworzenie grupy zasobów
az group create --name $resourceGroupName --location $location

# Tworzenie konta Storage
az storage account create --name $storageAccountName --location $location --resource-group $resourceGroupName --sku Standard_LRS

# Tworzenie funkcji
func init $functionAppName --worker-runtime $runtime
cd $functionAppName
func new --name HttpTrigger --template "HTTP trigger"

# Wdrożenie funkcji do Azure
az functionapp create --resource-group $resourceGroupName --consumption-plan-location $location --runtime $runtime --functions $functionAppName --storage-account $storageAccountName
az functionapp deployment source config-zip --resource-group $resourceGroupName --name $functionAppName --src .\bin\Debug\netcoreapp3.1\publish.zip

Compress-Archive -Path "C:\Users\scaro\source\repos\MedAlgo\MedAlgo" -DestinationPath "C:\Artifact1"

$resourceGroupName2 = "DefaultResourceGroup-NEU"
DefaultResourceGroup-NEU
$sourcePath = "C:\Users\scaro\source\repos\MedAlgo\MedAlgo"
$zipFilePath = "C:\Users\scaro\source\repos\MedAlgo"
Add-Type -AssemblyName System.IO.Compression.FileSystem
[IO.Compression.ZipFile]::CreateFromDirectory($sourcePath, $zipFilePath)

az functionapp create --resource-group $resourceGroupName --consumption-plan-location $location --runtime $runtime --functions 4 --storage-account $storageAccountName --name $functionAppName

<TargetFramework>net6.0</TargetFramework>