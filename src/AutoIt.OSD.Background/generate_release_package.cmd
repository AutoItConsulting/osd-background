REM Get current script folder with no trailing slash
SET ScriptDir=%~dp0
SET ScriptDir=%ScriptDir:~0,-1%

SET RootOutDir=%ScriptDir%\..\ReleasePackage
SET ThisScript=%~nx0

REM Sign if possible
REM If SIGNTOOL environment variable is not set then try setting it to a known location
if "%SIGNTOOL%"=="" set SIGNTOOL=%ProgramFiles(x86)%\Windows Kits\8.1\bin\x86\signtool.exe
REM Check to see if the signtool utility is missing
if exist "%SIGNTOOL%" goto OK1
    REM Give error that SIGNTOOL environment variable needs to be set
    echo "Must set environment variable SIGNTOOL to full path for signtool.exe code signing utility"
    echo Location is of the form "C:\Program Files (x86)\Windows Kits\8.1\x86\bin\signtool.exe"
:OK1

"%SIGNTOOL%" sign /tr http://rfc3161timestamp.globalsign.com/advanced /td SHA256 /fd SHA256 /n "AutoIt Consulting Ltd" /q "%ScriptDir%\AutoIt.OSD.Background.exe"


REM Clean up
ECHO %RootOutDir%
rmdir /s /q %RootOutDir%

robocopy "%ScriptDir%" "%RootOutDir%" /mir /xf "%ThisScript%" /xf *.pdb
copy "%ScriptDir%\AutoIt.OSD.Background.exe" "%RootOutDir%\Example" /y 
exit /b 0