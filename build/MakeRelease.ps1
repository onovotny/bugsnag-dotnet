$baseDir = Resolve-Path "$PSScriptRoot/.."
$msbuildExe = "C:\Program Files (x86)\MSBuild\12.0\bin\MsBuild.exe"
$nugetExe = "$baseDir\.nuget\NuGet.exe"

$configsToBuild = {"Release", "MonoRelease"}

if ($env:APPVEYOR) {
  $testExe = "xunit.console.clr4"
  $testFlags = '/appveyor'
} else {
  $testExe = "$baseDir\tools\xunit\xunit.console.clr4.exe"
  $testFlags = ''
}

git clean -xdf
& "$nugetExe" restore "$baseDir\Bugsnag.sln"

foreach-object $configsToBuild | % { & $msbuildExe "$baseDir/Bugsnag.sln" "/p:Configuration=$_" }

$filesToTest = ls -r "$baseDir\test\bin\**\Bugsnag.Test.dll"
echo $filesToTest | % { & $testExe "$_" $testFlags }
