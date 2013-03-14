param($installPath, $toolsPath, $package, $project)
$source = "$installPath\..\Microsoft.CompilerServices.AsyncTargetingPack.1.0.0\lib\net40\Microsoft.CompilerServices.AsyncTargetingPack.Net4.dll"
$target = "$installPath\lib\net40\Microsoft.CompilerServices.AsyncTargetingPack.Net4.dll"
copy $source $target
