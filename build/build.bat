ECHO ON
setlocal
cd ..\CrittercismWP8SDK
rem FIXME jbley this is the default location, might want to pursue something smarter than this
call "C:\Program Files (x86)\Microsoft Visual Studio 11.0\VC\WPSDK\WP80\vcvarsphoneall.bat"
ECHO ON

msbuild /p:Configuration=Release /target:Clean
msbuild /p:Configuration=Release
if %errorlevel% neq 0 exit /b %errorlevel%

cd ..\build

rd /s /q tmp
mkdir tmp\
mkdir tmp\lib
mkdir tmp\lib\WindowsPhone8

copy ..\CrittercismWP8SDK\CrittercismWP8SDK\Bin\Release\CrittercismWP8SDK.dll tmp\lib\WindowsPhone8
copy CrittercismWP8SDK.nuspec tmp\CrittercismWP8SDK.nuspec
rem this file will show up in Visual Studio when you install the package.
copy README_PUBLIC.txt tmp\README.txt
.\NuGet.exe pack tmp\CrittercismWP8SDK.nuspec
if %errorlevel% neq 0 exit /b %errorlevel%






