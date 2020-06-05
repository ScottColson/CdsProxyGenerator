@echo off
set package_root=..\..\
REM Find the proxygenerator in the package folder (irrespective of version)
For /R %package_root% %%G IN (proxygenerator.exe) do (
	IF EXIST "%%G" (set codegen_path=%%G
	goto :continue)
	)

:continue
@echo Using '%codegen_path%' 
REM proxygenerator [path] [connection-string]
"%codegen_path%" "%cd%\.."

if errorlevel 1 (
echo Error Code=%errorlevel%
exit /b %errorlevel%
)

pause