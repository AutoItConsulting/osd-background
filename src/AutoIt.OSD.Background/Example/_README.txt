1. Please download Sysinternals BGInfo from:

https://docs.microsoft.com/en-us/sysinternals/downloads/bginfo

Open the archive and copy BGInfo.exe and BGInfo64.exe into this folder.


2. Modify Options.xml as required.

3. Open BGInfo.exe and open Default.bgi and Error.bgi to customize your BGInfo text.

4. Modify Default.jpg and Error.jpg as required.

5. Run the script inside your task sequence like so:

	cscript.exe //NoLogo OSD_SetPhase.vbs "This is some phase text"

