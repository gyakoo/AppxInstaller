param(
    [string]$packageName = $null,
    [switch]$InstallCert = $false
)
if($InstallCert){
    certutil.exe -addstore TrustedPeople "$PSScriptRoot\$packageName.cer"
    exit 0
}
else{
    $PackageSignature = Get-AuthenticodeSignature "$PSScriptRoot\$packageName.appx"
    $PackageCertificate = $PackageSignature.SignerCertificate

    if (!$PackageCertificate)
    {
    	throw "Usigned package"
    	exit -1
    }

    if ($PackageSignature.Status -ne "Valid")
    {
        $RelaunchArgs = '-ExecutionPolicy Unrestricted -file "' + "$PSScriptRoot\Install-UntrustedAppx.ps1" + '"' + " $packageName -InstallCert"
        $AdminProcess = Start-Process "powershell.exe" -Verb RunAs -WorkingDirectory $PSScriptRoot -ArgumentList $RelaunchArgs -Wait
    }

    $DependencyPackages = Get-ChildItem (Join-Path (Join-Path $PSScriptRoot "Dependencies") "*.appx")
    
    if ($DependencyPackages.Count -gt 0)
    {
    	Add-AppxPackage -Path "$PSScriptRoot\$packageName.appx" -DependencyPath $DependencyPackages.FullName -ForceApplicationShutdown -Verbose
    }
    else
    {
    	Add-AppxPackage -Path "$PSScriptRoot\$packageName.appx" -ForceApplicationShutdown -Verbose
    }
}
