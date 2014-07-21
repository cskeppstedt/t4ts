T4TS.Build
==========

This project shows how a merged, and ready-for-deploy, T4TS.tt file works in a project.

T4TS.Build.csproj has a `<Target Name="BeforeBuild">` which triggers 
`T4TS.Build.Builder\BuildTemplate.csproj` to build. See `T4TS.Build.Builder\README.md` 
for details. This will result in 3 files in the `build\` directory:

 * T4TS.Attributes.dll
 * T4TS.tt
 * T4TS.tt.settings.t4
 
As you might've noticed, these files are linked to this project.