@call "%VS140COMNTOOLS%vsvars32.bat"

@msbuild "BuildAll.proj" /t:BuildAll /p:Configuration=Debug;TargetVersion=4.0

pause