$thisDir = (Split-Path $MyInvocation.MyCommand.Path)
Set-Alias nuget "$thisDir/.nuget/Nuget.exe"
nuget update -Self
nuget pack T4TS.nuspec -Prop Configuration=Release -IncludeReferencedProjects