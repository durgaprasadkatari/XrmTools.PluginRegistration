@echo off
set package_root=..\..\
REM Find the spkl in the package folder (irrespective of version)
For /R %package_root% %%G IN (PluginRegistration.TasRunner.exe) do (
	IF EXIST "%%G" (set taskrunner_path=%%G
	goto :continue)
	)

:continue
@echo Using '%spkl_path%'
REM spkl plugins [path] [connection-string] [/p:release]
"%taskrunner_path%" "Url=https://crm-dev.crm11.dynamics.com;thumbprint=11111111;ClientId=11110-11132;AuthType=Certificate;" %*

if errorlevel 1 (
echo Error Code=%errorlevel%
exit /b %errorlevel%
)

pause