param(
    [string]$packageName = $null
)
$PackageSignature = Get-AuthenticodeSignature "$PSScriptRoot\$packageName.appx"
$PackageCertificate = $PackageSignature.SignerCertificate

if (!$PackageCertificate)
{
	throw "Usigned package"
	exit -1
}

if ($PackageSignature.Status -ne "Valid")
{
	Write-Host "installing cert!"
	certutil.exe -addstore TrustedPeople "$PSScriptRoot\$packageName.cer"
}

$DependencyPackagesDir = Join-Path $PSScriptRoot "Dependencies"
$DependencyPackages = @()

if (Test-Path $DependencyPackagesDir)
{
	$DependencyPackages += Get-ChildItem (Join-Path $DependencyPackagesDir "*.appx") | Where-Object { $_.Mode -NotMatch "d" }

	if (($Env:Processor_Architecture -eq "x86") -and (Test-Path (Join-Path $DependencyPackagesDir "x86")))
	{
		$DependencyPackages += Get-ChildItem (Join-Path $DependencyPackagesDir "x86\*.appx") | Where-Object { $_.Mode -NotMatch "d" }
	}
	if (($Env:Processor_Architecture -eq "amd64") -and (Test-Path (Join-Path $DependencyPackagesDir "x64")))
	{
		$DependencyPackages += Get-ChildItem (Join-Path $DependencyPackagesDir "x64\*.appx") | Where-Object { $_.Mode -NotMatch "d" }
	}
	if (($Env:Processor_Architecture -eq "arm") -and (Test-Path (Join-Path $DependencyPackagesDir "arm")))
	{
		$DependencyPackages += Get-ChildItem (Join-Path $DependencyPackagesDir "arm\*.appx") | Where-Object { $_.Mode -NotMatch "d" }
	}
}

$DependencyPackages.FullName

if ($DependencyPackages.FullName.Count -gt 0)
{
	Add-AppxPackage -Path "$PSScriptRoot\$packageName.appx" -DependencyPath $DependencyPackages.FullName -ForceApplicationShutdown -Verbose
}
else
{
	Add-AppxPackage -Path "$PSScriptRoot\$packageName.appx" -ForceApplicationShutdown -Verbose
}
