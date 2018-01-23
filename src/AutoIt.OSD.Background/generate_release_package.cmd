REM Get current script folder with no trailing slash
SET ScriptDir=%~dp0
SET ScriptDir=%ScriptDir:~0,-1%

SET RootOutDir=%ScriptDir%\..\ReleasePackage
SET ThisScript=%~nx0

REM Clean up
ECHO %RootOutDir%
rmdir /s /q %RootOutDir%

robocopy "%ScriptDir%" "%RootOutDir%" /mir /xf "%ThisScript%" /xf *.pdb
copy "%ScriptDir%\AutoIt.OSD.Background.exe" "%RootOutDir%\Example" /y 
exit /b 0