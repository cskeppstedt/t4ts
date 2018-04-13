
for /R %%f in (*.nuspec) do (
   nuget pack "%%~f"
  )