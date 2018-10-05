@call "%VS110COMNTOOLS%vsvars32.bat"

@msbuild "RemoveFolders.proj" /t:RemoveFolders

@msbuild "BuildAll.proj" /t:BuildAll /p:Configuration=Release;TargetVersion=4.0
@msbuild "CreateZipFile.proj" /t:CreateZipFile
@msbuild "BuildNuGetPackage.proj" /t:BuildNuGetPackage

REM @call "Build (4.0 Release).bat"
REM @call "Build Zip File.bat"
REM @call "Build NuGet Package.bat"

pause

