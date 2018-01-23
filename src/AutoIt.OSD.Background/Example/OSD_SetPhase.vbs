	
'-----------------------------------------------------------------------------
' ScriptName:	OSD_SetPhase.vbs
' Date:			23/01/18
' Author:		Jonathan Bennett <jon@autoitconsulting.com>
' Purpose:		Launches BGinfo to customize the desktop and then ensure it is visible on Windows 7 / 8 / 10.
' Usage:			cscript.exe OSD_SetPhase.vbs "Installing Something"  (Use the phrase "ERROR: xxxx" to show the Error.jpg bitmap")
'-----------------------------------------------------------------------------
Option Explicit

' Set this to True if your WinPE boot image has .NET support and AutoIt.OSD.Background.exe works correctly there
Const DotNetInWinPE = False

Const WinStyleMinimizedInactive = 7
Const WinStyleNormal = 1
Const WinStyleHidden = 0

Dim g_oFSO : Set g_oFSO = CreateObject("Scripting.FileSystemObject")
Dim g_oShell : Set g_oShell = WScript.CreateObject("WScript.Shell")

Dim g_sScriptDir : g_sScriptDir = g_oFSO.GetParentFolderName(WScript.ScriptFullName)

Dim g_bTaskSequenceRunning : g_bTaskSequenceRunning = False
On Error Resume Next
Dim g_oTSEnv : Set g_oTSEnv = CreateObject("Microsoft.SMS.TSEnvironment")
If Err.Number = 0 Then 
	g_bTaskSequenceRunning = True
End If
On Error Goto 0

Dim sBGInfoText

' Get arguments
Dim oArgs : Set oArgs = WScript.Arguments
If oArgs.Count >= 1 Then
	sBGInfoText = oArgs(0)
End If

Dim sBgi : sBgi = "Default.bgi"

If InStr(UCase(sBGInfoText), "ERROR:") > 0 Then
	sBgi = "Error.bgi"
End If

' All our BGInfo.bgi files reference .\Default.bmp or .\Error.bmp so we need to make sure the current working directory is set
g_oShell.CurrentDirectory = g_sScriptDir

' Set the BGINFO temporary environment variables
Dim oEnv : Set oEnv = g_oShell.Environment("Process")
oEnv("BGINFO_Text") = sBGInfoText
If sBGInfoText <> "" Then 
	oEnv("BGINFO_Title") = GetTSVariable("_SMSTSPackageName")
Else
	oEnv("BGINFO_Title") = ""
End If

' Change bitmap
On Error Resume Next
If oEnv("PROCESSOR_ARCHITECTURE") = "AMD64" Then
	g_oShell.Run "bginfo64.exe " & sBgi & " /nolicprompt /silent /timer:0",,True
Else
	g_oShell.Run "bginfo.exe " & sBgi & " /nolicprompt /silent /timer:0",,True
End If
On Error Goto 0

' We run differently depending on if we are in WinPE as we can't really assume that .NET is present.
Dim bInWinPE
If UCase(GetTSVariable("_SMSTSInWinPE")) = "TRUE" Then
	bInWinPE = True
Else	
	bInWinPE = False
End If

' Run our custom OSD background exe?
If bInWinPE = False Or (bInWinPE = True And DotNetInWinPE = True) Then
	On Error Resume Next			
	g_oShell.Run "AutoIt.OSD.Background.exe Options.xml", WinStyleNormal, False
	On Error Goto 0
End If

' Finish 	
WScript.Quit 0

'----------------------------------------------------

Function GetTSVariable(ByVal sVar)
	If g_bTaskSequenceRunning = True Then
		GetTSVariable = g_oTSEnv(sVar)
	End If
End Function

'----------------------------------------------------
