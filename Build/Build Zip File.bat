@call "%VS100COMNTOOLS%vsvars32.bat"

@msbuild "CreateZipFile.proj" /t:CreateZipFile


pause