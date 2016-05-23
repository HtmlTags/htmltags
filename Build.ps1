function Restore-Packages
{
    param([string] $DirectoryName)
    & dotnet restore ("""" + $DirectoryName + """")
}

function Build-Projects
{
    param([string] $DirectoryName)
    & dotnet build ("""" + $DirectoryName + """") --configuration Release --output .\artifacts\testbin --framework netstandard1.5; if($LASTEXITCODE -ne 0) { exit 1 }
    & dotnet pack ("""" + $DirectoryName + """") --configuration Release --output .\artifacts\packages; if($LASTEXITCODE -ne 0) { exit 1 }
}

function Build-TestProjects
{
    param([string] $DirectoryName) 
    & dotnet build ("""" + $DirectoryName + """") --configuration Release --output .\artifacts\testbin --framework netstandard1.5; if($LASTEXITCODE -ne 0) { exit 1 }
}

function Test-Projects
{
    param([string] $DirectoryName)
    & dotnet test ("""" + $DirectoryName + """"); if($LASTEXITCODE -ne 0) { exit 2 }
}

function Remove-PathVariable
{
    param([string] $VariableToRemove)
    $path = [Environment]::GetEnvironmentVariable("PATH", "User")
    $newItems = $path.Split(';') | Where-Object { $_.ToString() -inotlike $VariableToRemove }
    [Environment]::SetEnvironmentVariable("PATH", [System.String]::Join(';', $newItems), "User")
    $path = [Environment]::GetEnvironmentVariable("PATH", "Process")
    $newItems = $path.Split(';') | Where-Object { $_.ToString() -inotlike $VariableToRemove }
    [Environment]::SetEnvironmentVariable("PATH", [System.String]::Join(';', $newItems), "Process")
}

Push-Location $PSScriptRoot


# Clean
if(Test-Path .\artifacts) { Remove-Item .\artifacts -Force -Recurse }


# Package restore
Get-ChildItem -Path . -Filter *.xproj -Recurse | ForEach-Object { Restore-Packages $_.DirectoryName }

# Build/package
Get-ChildItem -Path .\src -Filter *.xproj -Recurse | ForEach-Object { Build-Projects $_.DirectoryName }
Get-ChildItem -Path .\test -Filter *.xproj -Recurse | ForEach-Object { Build-TestProjects $_.DirectoryName }

# Test
Get-ChildItem -Path .\test -Filter *.xproj -Recurse | ForEach-Object { Test-Projects $_.DirectoryName }

# Test again
Get-ChildItem -Path .\test -Filter *.xproj -Recurse | ForEach-Object { Test-Projects $_.DirectoryName }

Pop-Location