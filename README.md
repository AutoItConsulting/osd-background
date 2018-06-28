# OSD Background

![alt text](https://www.autoitconsulting.com/site/wp-content/uploads/2018/01/OSDBackgroundExample_annotated_1080x675.png "OSD Background")


## Overview

When doing an SCCM or MDT Operating System Deployment a common task is to customize the background that is shown during the various stages of the build in order to brand the build or to just indicate the main build tasks.

During Windows 7 setup there is a window that shows the message "Setup is preparing your computer for first use". On Windows 8 and Windows 10 that message is "Getting Ready" or the circling dots animation. These setup windows obscure the background thus hiding our custom background.

The only solution is to create a new custom window and place it above this progress window and fill it with our wallpaper bitmap thus creating a "fake" background. Additionally, the tool also registers a hotkey combination to bring up a debug menu for running user configurable tools and task sequence variables.

## Main Web Site and Binary Downloads

Please see the product page for more details and binary downloads. [https://www.autoitconsulting.com/site/software/osd-background/](https://www.autoitconsulting.com/site/software/osd-background/).

## Features
Here are some of the key features of OSD Background:
* Allows background bitmap to be seen on Windows 10 during deployment.
* Background can be configured to show an automatic progress bar that is based on the position in the current task sequence
* Hotkey combination (Ctrl+Alt+F12) will bring up a debug menu.
* Debug menu can be protected with two different passwords for Admin and User levels access.
* User-configurable tools menu. Command prompts, registry editing, system information, anything! Individual tools can be marked as 'Admin Only' and are only shown when the Admin password is used to open the menu.
* Editable list of task sequence variables.
