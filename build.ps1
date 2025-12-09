function Exec
{
    [CmdletBinding()]
    param(
        [Parameter(Position=0,Mandatory=1)][scriptblock]$cmd,
        [Parameter(Position=1,Mandatory=0)][string]$errorMessage = ($msgs.error_bad_command -f $cmd)
    )
    & $cmd
    if ($lastexitcode -ne 0) {
        throw ("Exec: " + $errorMessage)
    }
}

if(Test-Path .\artifacts) { Remove-Item .\artifacts -Force -Recurse }

exec { & dotnet restore }

#
# Determine version numbers
$branch = @{ $true = $env:APPVEYOR_REPO_BRANCH; $false = $(git symbolic-ref --short -q HEAD) }[$env:APPVEYOR_REPO_BRANCH -ne $NULL];
$revision = @{ $true = "{0:00000}" -f [convert]::ToInt32("0" + $env:APPVEYOR_BUILD_NUMBER, 10); $false = "local" }[$env:APPVEYOR_BUILD_NUMBER -ne $NULL];
$suffix = @{ $true = ""; $false = "$($branch.Substring(0, [math]::Min(10,$branch.Length)))-$revision"}[$branch -eq "master" -and $revision -ne "local"]
$commitHash = $(git rev-parse --short HEAD)
$buildSuffix = @{ $true = "$($suffix)-$($commitHash)"; $false = "$($branch)-$($commitHash)" }[$suffix -ne ""]
echo "build: Build version suffix is $buildSuffix"

exec { & dotnet build DommelExtended.sln -c Release --version-suffix=$buildSuffix /p:CI=true }

#
# Execute tests
echo "build: Executing tests"
#exec { & dotnet test test/DommelExtended.Tests -c Release --no-build }
#exec { & dotnet test test/DommelExtended.IntegrationTests -c Release --no-build }
#exec { & dotnet test test/DommelExtended.Json.Tests -c Release --no-build }
#exec { & dotnet test test/DommelExtended.Json.IntegrationTests -c Release --no-build }

echo "build: Calculating code coverage metrics"

#
# Test coverage for DommelExtended

# Create the first coverage in the coverlet JSON format to allow merging
#exec { & dotnet test test/DommelExtended.Tests -c Release --no-build /p:CollectCoverage=true }

# Merge this coverage output with the previous coverage output, this time
# create a report using the opencover format which codecov can parse
#Push-Location -Path "test/DommelExtended.IntegrationTests"
#exec { & dotnet test -c Release --no-build /p:CollectCoverage=true /p:MergeWith="..\DommelExtended.Tests\coverage.json" /p:CoverletOutputFormat=opencover }
if ($env:APPVEYOR_BUILD_NUMBER) {
    exec { & codecov -f "coverage.opencover.xml" }
}
Pop-Location

#
# Test coverage for DommelExtended.Json
#exec { & dotnet test test/DommelExtended.Json.Tests -c Release --no-build /p:CollectCoverage=true /p:Include="[DommelExtended.Json]*" }

Push-Location -Path "test/DommelExtended.Json.IntegrationTests"
#exec { & dotnet test -c Release --no-build /p:CollectCoverage=true /p:Include="[DommelExtended.Json]*" /p:MergeWith="..\DommelExtended.Json.Tests\coverage.json" /p:CoverletOutputFormat=opencover }
if ($env:APPVEYOR_BUILD_NUMBER) {
    exec { & codecov -f "coverage.opencover.xml" }
}
Pop-Location

#
# Create artifacts
if ($env:APPVEYOR_BUILD_NUMBER) {
    $versionSuffix = "beta.{0}" -f [convert]::ToInt32("0" + $env:APPVEYOR_BUILD_NUMBER, 10)
}
else {
    $versionSuffix = $suffix
}

echo "build: Creating NuGet package with suffix $versionSuffix"
exec { & dotnet pack .\src\DommelExtended\DommelExtended.csproj -c Release -o .\artifacts --no-build --version-suffix=$versionSuffix }
exec { & dotnet pack .\src\DommelExtended.Json\DommelExtended.Json.csproj -c Release -o .\artifacts --no-build --version-suffix=$versionSuffix }
