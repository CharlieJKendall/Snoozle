if(-not $Env:BUILD_SOURCEBRANCHNAME)
{
    Write-Error "The $Build.SourceBranchName environment variable must be set"
    exit 1
}

if(-not $Env:BUILD_BUILDID)
{
    Write-Error "The $Build.BuildId environment variable must be set"
    exit 1
}

$MajorMinor = $Env:BUILD_SOURCEBRANCHNAME
Write-Host "Major.Minor found from branch name: $MajorMinor"

$MostRecentVersion = git describe --tags --abbrev=0
Write-Host "Most recent version from git describe: $MostRecentVersion"

$MostRecentVersionObject = New-Object System.Version($MostRecentVersion)
$NewMajorMinor = $MostRecentVersionObject.Major.ToString() + '.' + $MostRecentVersionObject.Minor.ToString()

$NewVersion

if ($NewMajorMinor -eq $MajorMinor)
{
    $Patch = $MostRecentVersionObject.Build + 1
    $NewVersion = "$MajorMinor.$Patch.$Env:BUILD_BUILDID"
}
else
{
    $NewVersion = "$MajorMinor.0.$Env:BUILD_BUILDID"
}

Write-Host "New version set to $NewVersion"
Write-Host "##vso[task.setvariable variable=Version]$NewVersion"