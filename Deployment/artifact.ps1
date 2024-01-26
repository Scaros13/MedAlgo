$sourcePath = "C:\Users\scaro\source\repos\MedAlgo\MedAlgo"
$zipFilePath = "C:\Users\scaro\source\repos\MedAlgo"
Add-Type -AssemblyName System.IO.Compression.FileSystem
[IO.Compression.ZipFile]::CreateFromDirectory($sourcePath, $zipFilePath)