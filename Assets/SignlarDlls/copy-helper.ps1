#download nuget.exe https://www.nuget.org/downloads
#nuget install Microsoft.AspNetCore.SignalR.Client -Version 6.0.8 -OutputDirectory ./signalr -Framework netstandard2.0
# source: https://github.com/evanlindsey/Unity-WebGL-SignalR/blob/master/Unity/Assets/Plugins/SignalR/lib/signalr.ps1

# powershell -ExecutionPolicy Bypass -NonInteractive -File copy.ps1
$pluginDir = './plugin';
$outDir = './signalr';
$target = "netstandard2.0"

New-Item -Path $pluginDir -ItemType Directory
$packages = Get-ChildItem -Path $outDir
foreach ($p in $packages) {
	$dll = Get-ChildItem -Path "$($p.FullName)\lib\$($target)\*.dll"
	if (!($null -eq $dll)) {
		$d = $dll[0]
		if (!(Test-Path "$($pluginDir)\$($d.Name)")) {
			Move-Item -Path $d.FullName -Destination $pluginDir
		}
	}
}
