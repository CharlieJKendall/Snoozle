if(-not $Env:BUILD_SOURCEBRANCHNAME)
{
    Write-Error "The $Build.SourceBranchName environment variable must be set"
    exit 1
}

if(-not ($Env:BUILD_SOURCEBRANCH.StartsWith("refs/heads/release")))
{
    Write-Error "The $Build.SourceBranch environment variable must start with refs/heads/release"
    exit 1
}

if(-not $Env:BUILD_BUILDID)
{
    Write-Error "The $Build.BuildId environment variable must be set"
    exit 1
}

$MajorMinor = $Env:BUILD_SOURCEBRANCHNAME
Write-Verbose "Major.Minor found from branch name: $MajorMinor"

$MostRecentVersion = git describe --tags --abbrev=0
Write-Verbose "Most recent version from git describe: $MostRecentVersion"

$MajorMinorMostRecentVersion = $MostRecentVersion.Substring(0, $MostRecentVersion.LastIndexOf('.'))
$NewVersion

if ($MajorMinorMostRecentVersion -eq $MajorMinor)
{
    $Patch = [int]$MostRecentVersion.Substring($MostRecentVersion.LastIndexOf('.')) + 1
    $NewVersion = "$MajorMinor.$Patch.$Env:BUILD_BUILDID"
}
else
{
    $NewVersion = "$MajorMinor.0.$Env:BUILD_BUILDID"
}

Write-Verbose "New version set to $NewVersion"
Write-Host "##vso[task.setvariable variable=Version]$NewVersion"