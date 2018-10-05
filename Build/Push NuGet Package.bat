@call "%VS110COMNTOOLS%vsvars32.bat"

@msbuild "BuildNuGetPackage.proj" /t:PushNuGetPackage


pause