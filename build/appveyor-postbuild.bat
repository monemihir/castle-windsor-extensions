REM ~ cleanup
rmdir /S /Q dist

REM ~ copy binaries
xcopy /I /Y "src\Castle.Windsor.Extensions\bin\Castle.Windsor.Extensions*.*" "dist\lib\net40"
REM ~ del "dist\lib\net40\*.pdb"

REM ~ create archive
7z a artifacts\Castle.Windsor.Extensions.zip dist

REM ~ build nuget packages
cd build
nuget pack Castle.Windsor.Extensions.nuspec -OutputDirectory ..\artifacts
cd ..
