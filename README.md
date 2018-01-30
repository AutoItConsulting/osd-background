# OSD Background

![alt text](https://www.autoitconsulting.com/site/wp-content/uploads/2018/01/OSDBackgroundExample_annotated_1080x675.png "OSD Background")


## Overview

Please see the product page for more details and binary downloads. [https://www.autoitconsulting.com/software/osd-background/](https://www.autoitconsulting.com/software/osd-background/).

When doing an SCCM or MDT Operating System Deployment a common task is to customize the background that is shown during the various stages of the build in order to brand the build or to just indicate the main build tasks.

## Issues

During Windows 7 setup there is a window that shows the message "Setup is preparing your computer for first use". On Windows 8 and Windows 10 that message is "Getting Ready" or the circling dots animation. These setup windows obscure the background thus hiding our custom background.

The only solution is to create a new custom window and place it above this progress window and fill it with our wallpaper bitmap thus creating a "fake" background.

## Solution Features

Here are some solution highlights:

* Works on Windows 7, Windows 8 and Windows 10
* Works on WinPE (requires Windows 8+ versions that can have .NET components added)
* Automatically hides the progress window. No need to run WindowHide.exe first
* Creates a custom screen behind other windows to simulate desktop wallpaper
* Automatically updates a customisable progress bar based on the current position within a task sequence. Run it once each reboot and forget it if you like!
* If you run the tool multiple times it will close down previously executed versions. No need to *taskkill* each time

## Requirements

The solution requires Microsoft .NET 4.0. In most deployments the SCCM client will install this version so it seemed a good version to choose.

## Usage

The basic usage is:

* Edit *Options.xml* to customise as required

````
<Options>

  <!-- Show progress bar for current task sequence progress -->
  <ProgressBarEnabled>true</ProgressBarEnabled>

  <!-- The color of the progress bar in HTML/RGB format: #RRGGBB -->
  <ProgressBarForeColor>#5D83AC</ProgressBarForeColor>
  <ProgressBarBackColor>#FFFFFF</ProgressBarBackColor>

  <!-- Location of the progress bar. Top or Bottom -->
  <ProgressBarDock>Bottom</ProgressBarDock>
  
  <!-- Offset in pixels from the Top or Bottom of the screen -->
  <ProgressBarOffset>4</ProgressBarOffset>
  
  <!-- Height of the progress bar -->
  <ProgressBarHeight>4</ProgressBarHeight>

</Options>
````

* Run *BGInfo.exe* to customise the desktop wallpaper (or manually set the wallpaper if preferred) at various places in the OSD build process
* Run this command to ensure that the wallpaper can be seen in the background:

````
    AutoIt.OSD.Background Options.xml
````

* Remember to rerun the command after each Restart Computer step in the task sequence to reshow the background.

## Windows 10 Additional Requirements

On the latest versions of Windows 10 you must also use a custom unattend.xml file with the **SkipMachineOOBE** and **SkipUserOOBE** options set to **true** otherwise there seems to be a block on any windows being shown at all - even the SCCM progress bars are invisible. The example below is for the x64 version of Windows 10.

````
<?xml version="1.0" encoding="utf-8"?>
<unattend xmlns="urn:schemas-microsoft-com:unattend">
    <settings pass="oobeSystem">
        <component name="Microsoft-Windows-Shell-Setup" processorArchitecture="amd64" publicKeyToken="31bf3856ad364e35" language="neutral" versionScope="nonSxS" xmlns:wcm="http://schemas.microsoft.com/WMIConfig/2002/State" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
            <OOBE>
                <HideEULAPage>true</HideEULAPage>
                <HideLocalAccountScreen>true</HideLocalAccountScreen>
                <HideOEMRegistrationScreen>true</HideOEMRegistrationScreen>
                <HideOnlineAccountScreens>true</HideOnlineAccountScreens>
                <HideWirelessSetupInOOBE>true</HideWirelessSetupInOOBE>
                <ProtectYourPC>1</ProtectYourPC>
                <SkipMachineOOBE>true</SkipMachineOOBE>
                <SkipUserOOBE>true</SkipUserOOBE>
            </OOBE>
        </component>
    </settings>
</unattend>

```` 