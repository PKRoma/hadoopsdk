param($installPath, $toolsPath, $package, $project)
$source = "$installPath\..\Microsoft.CompilerServices.AsyncTargetingPack.1.0.1\lib\net40\Microsoft.CompilerServices.AsyncTargetingPack.Net4.dll"
$target = "$installPath\lib\net40\Microsoft.CompilerServices.AsyncTargetingPack.Net4.dll"
copy $source $target

try {
  Write-Host Setting MRLib items CopyToOutputDirectory=true
  $project.ProjectItems.Item("MRLib").ProjectItems.Item("Microsoft.Hadoop.CombineDriver.exe").Properties.Item("CopyToOutputDirectory").Value = 1
  $project.ProjectItems.Item("MRLib").ProjectItems.Item("Microsoft.Hadoop.MapDriver.exe").Properties.Item("CopyToOutputDirectory").Value = 1
  $project.ProjectItems.Item("MRLib").ProjectItems.Item("Microsoft.Hadoop.ReduceDriver.exe").Properties.Item("CopyToOutputDirectory").Value = 1
  $project.ProjectItems.Item("MRLib").ProjectItems.Item("Microsoft.Hadoop.MapReduce.dll").Properties.Item("CopyToOutputDirectory").Value = 1
  $project.ProjectItems.Item("MRLib").ProjectItems.Item("MRRunner.exe").Properties.Item("CopyToOutputDirectory").Value = 1
} catch [Exception] {
}
