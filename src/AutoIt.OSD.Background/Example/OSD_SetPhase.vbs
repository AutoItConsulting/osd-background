	
'-----------------------------------------------------------------------------
' ScriptName:	OSD_SetPhase.vbs
' Date:			26/06/18
' Author:		Jonathan Bennett <jon@autoitconsulting.com>
' Purpose:		Launches BGinfo to customize the desktop and then ensure it is visible on Windows 7 / 8 / 10.
' Usage:		cscript.exe OSD_SetPhase.vbs "Installing Something"  (Use the phrase "ERROR: xxxx" to show the Error.jpg bitmap")
'-----------------------------------------------------------------------------
Option Explicit

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
	' May succeed because of a failed Task Sequence or bad clean-up of COM objects.
	' Double check by reading the Task Sequence type, if its blank - not in a Task Sequence
	If g_oTSEnv("_SMSTSType") <> "" Then 
		g_bTaskSequenceRunning = True
	End If	
End If
On Error Goto 0

' Get arguments
Dim sBGInfoText
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

' Run our custom OSD background exe? Needs .NET 4 or later
If IsDotNet4Or45Installed() Then
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
	
Function IsDotNet4Or45Installed()
	
	If ReadFromRegistry("HKLM\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4.0\Full\Version", "") <> "" Then
		IsDotNet4Or45Installed = True
	ElseIf ReadFromRegistry("HKLM\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4.0\Client\Version", "") <> "" Then
		IsDotNet4Or45Installed = True
	ElseIf ReadFromRegistry("HKLM\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\Version", "") <> "" Then
		IsDotNet4Or45Installed = True
	Else
		IsDotNet4Or45Installed = False
	End If
	
End Function
	
'----------------------------------------------------

Function ReadFromRegistry (ByVal strRegistryKey, ByVal strDefault )
	On Error Resume Next
	
	Dim val : val = g_oShell.RegRead( strRegistryKey )
	if err.number <> 0 then
	
		ReadFromRegistry = strDefault
	else
		ReadFromRegistry = val
	end if
End Function

'----------------------------------------------------
