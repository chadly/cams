$xml = [Xml] (Get-Content src\cams.csproj)
$version = [String] $xml.Project.PropertyGroup.Version
$version = $version.Trim()

dotnet publish src -c Release -r win10-x64
Compress-Archive -Force -Path src\bin\Release\netcoreapp2.1\win10-x64\publish\* -DestinationPath cams-v${version}-win10-x64.zip

dotnet publish src -c Release -r ubuntu-x64
Compress-Archive -Force -Path src\bin\Release\netcoreapp2.1\ubuntu-x64\publish\* -DestinationPath cams-v${version}-ubuntu-x64.zip
