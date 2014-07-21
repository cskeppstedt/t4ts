T4TS.Build.Builder
==================

This project contains the `BuildTemplateTask` which is used to
generate the `T4TS.tt` template file. 

Building with MSBuild
---------------------

The build is fully automated through MSBuild. You can run this command
in a console to build T4TS.tt, assuming that you are standing in
`.\T4TS.Build.Builder\`:

`MSBuild.exe BuildTemplate.csproj`

If you don't have MSBuild in your path, you will have to specify the path:

`c:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe BuildTemplate.csproj`

How T4TS.tt is built
--------------------

T4TS.tt is built by parsing all C# classes found in `..\T4TS\T4TS.csproj`,
which are merged into a single string. This is then combined with two 
strings found in `Properties\Resources.resx`:

 * `TemplatePrefix`
 * `TemplateSuffix`

`T4TS.tt` is written to the directory `OutputDir` which is passed as an argument
to the `BuildTemplateTask` (see `BuildTemplate.csproj`).

How T4TS.tt.settings.t4 is built
--------------------------------

`T4TS.tt.settings.t4` is also written to the `OutputDir` directory. Its
content is simply the `TemplateSettings` string found in `Properties\Resources.resx`.

