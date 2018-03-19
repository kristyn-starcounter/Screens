@ECHO OFF

IF "%CONFIGURATION%"=="" SET CONFIGURATION=Debug

star %* --resourcedir="%~dp0src\Screens\wwwroot" "%~dp0src/Screens/bin/%CONFIGURATION%/Screens.exe"