//  
// Copyright (c) AutoIt Consulting Ltd. All rights reserved.  
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.  
// using System;
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Serialization;
using AutoIt.OSD.Background.Properties;
using AutoIt.Windows;
using Microsoft.Win32;

namespace AutoIt.OSD.Background
{
    public partial class FormBackground : Form
    {
        private const int RefreshInervalSecs = 1;
        private readonly string _appPath = Directory.GetParent(Assembly.GetExecutingAssembly().Location).ToString();
        private Color _progressBarBackColor;

        private DockStyle _progressBarDock;
        private bool _progressBarEnabled;
        private Color _progressBarForeColor;
        private int _progressBarHeight;
        private int _progressBarOffset;

        private DateTime _wallpaperLastModified;

        private string _wallpaperPath = string.Empty;
        private Options _xmlOptions;

        /// <inheritdoc />
        public FormBackground()
        {
            InitializeComponent();

            // Set main icon
            Icon = Resources.main;

            // Set these here rather than designer as it make it easier to work with designer
            pictureBoxBackground.Dock = DockStyle.Fill;
            pictureBoxBackground.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        /// <summary>
        ///     We don't want this window to activate when it is shown
        /// </summary>
        protected override bool ShowWithoutActivation
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        ///     Check if this is Windows 8 or later. Requires an OS manifest to work correctly.
        /// </summary>
        /// <returns></returns>
        public static bool IsWin8OrLater()
        {
            //    Windows 10	10.0*
            //    Windows Server 2016	10.0*
            //    Windows 8.1	6.3*
            //    Windows Server 2012 R2	6.3*
            //    Windows 8	6.2
            //    Windows Server 2012	6.2
            //    Windows 7	6.1

            if (Environment.OSVersion.Version.Major < 6)
            {
                return false;
            }

            if (Environment.OSVersion.Version.Major > 6)
            {
                return true;
            }

            // Major = 6
            return Environment.OSVersion.Version.Minor >= 2;
        }

        /// <summary>
        ///     Look for an existing process with this name and terminate it.
        /// </summary>
        private static void KillPreviousInstance()
        {
            Process[] pname = Process.GetProcessesByName(AppDomain.CurrentDomain.FriendlyName.Remove(AppDomain.CurrentDomain.FriendlyName.Length - 4));
            if (pname.Length > 1)
            {
                pname.First(p => p.Id != Process.GetCurrentProcess().Id).Kill();
            }
        }

        private void BringToFrontOfWindowsSetupProgress()
        {
            // Find the window(s) (should only be one) with the FirstUXWndClass, same on Win7/Win10
            List<IntPtr> progressWindows = Management.FindWindowsWithClass("FirstUXWndClass").ToList();

            const uint flag = NativeMethods.SWP_NOACTIVATE | NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE;

            // If there are no progress windows then just move to bottom most
            if (progressWindows.Count == 0)
            {
                NativeMethods.SetWindowPos(Handle, NativeMethods.HWND_BOTTOM, 0, 0, 0, 0, flag);
                return;
            }

            // After various tests with modifying Z-order without ever obscuring the foreground what
            // seems to work best is to move our window right to the bottom, then move the Win7/10 progress
            // window to the bottom
            // We'll also try and hide the Windows screen for good measure
            NativeMethods.SetWindowPos(Handle, NativeMethods.HWND_BOTTOM, 0, 0, 0, 0, flag);
            NativeMethods.SetWindowPos(progressWindows[0], NativeMethods.HWND_BOTTOM, 0, 0, 0, 0, flag);
            NativeMethods.ShowWindowAsync(progressWindows[0], NativeMethods.SW_HIDE);
        }

        /// <summary>
        ///     Converts TRUE/FALSE/1/0/ON/OFF strings to a bool
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">String is not a valid bool.</exception>
        private static bool ConvertStringToBool(string input)
        {
            if (input.ToUpper() == "TRUE" || input.ToUpper() == "ON" || input == "1")
            {
                return true;
            }

            if (input.ToUpper() == "FALSE" || input.ToUpper() == "OFF" || input == "0")
            {
                return false;
            }

            throw new ArgumentException();
        }

        /// <summary>
        ///     Runs when form is starting to close.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormBackground_FormClosing(object sender, FormClosingEventArgs e)
        {
            timerRefresh.Stop();

            // Remove static events
            SystemEvents.SessionEnding -= SystemEvents_SessionEnding;
        }

        /// <summary>
        ///     Load event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormBackground_Load(object sender, EventArgs e)
        {
            Text = AppDomain.CurrentDomain.FriendlyName;

            // Read in options file if specified
            if (!GetOptions())
            {
                MessageBox.Show(@"Unable to read or parse Options.xml file.", AppDomain.CurrentDomain.FriendlyName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }

            // Make sure picture box is bottom most control on our form
            pictureBoxBackground.SendToBack();

            // First update of background image
            if (RefreshBackgroundImage(true) == false)
            {
                MessageBox.Show(@"Unable to load user wallpaper.", AppDomain.CurrentDomain.FriendlyName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }

            // First update of progress bar
            RefreshProgressBar();

            // Push the Win7/Win10 progress screen to the bottom, then put our screen on top
            BringToFrontOfWindowsSetupProgress();

            // Trap shutdown
            SystemEvents.SessionEnding += SystemEvents_SessionEnding;

            // Trap display change
            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
        }

        /// <summary>
        ///     Shown event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormBackground_Shown(object sender, EventArgs e)
        {
            // Let form finish showing
            Refresh();

            // We don't want any other versions running - kill it after we have completely shown our new screen to reduce flicker
            KillPreviousInstance();

            // Start the refresh timer
            timerRefresh.Interval = (int)TimeSpan.FromSeconds(RefreshInervalSecs).TotalMilliseconds;
            timerRefresh.Start();
        }

        /// <summary>
        ///     Returns path of the current user wallpaper.
        /// </summary>
        /// <returns></returns>
        private static string GetCurrentUserWallpaperPath()
        {
            string wallpaperPath = string.Empty;

            try
            {
                RegistryKey userWallpaper = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", false);
                if (userWallpaper != null)
                {
                    wallpaperPath = userWallpaper.GetValue("Wallpaper").ToString();
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return wallpaperPath;
        }

        private bool GetOptions()
        {
            string[] arguments = Environment.GetCommandLineArgs();

            // If no file specifed use defaults
            if (arguments.Length == 1)
            {
                _progressBarDock = DockStyle.Bottom;
                _progressBarEnabled = true;
                _progressBarOffset = 0;
                _progressBarHeight = 5;
                _progressBarForeColor = progressBar.ForeColor;
                _progressBarBackColor = progressBar.BackColor;
                return true;
            }

            try
            {
                var deSerializer = new XmlSerializer(typeof(Options));
                TextReader reader = new StreamReader(_appPath + @"\" + arguments[1]);
                _xmlOptions = (Options)deSerializer.Deserialize(reader);

                _progressBarEnabled = ConvertStringToBool(_xmlOptions.ProgressBarEnabled);
                _progressBarHeight = _xmlOptions.ProgressBarHeight;
                _progressBarOffset = _xmlOptions.ProgressBarOffset;
                _progressBarDock = (DockStyle)Enum.Parse(typeof(DockStyle), _xmlOptions.ProgressBarDock, true);
                _progressBarForeColor = ColorTranslator.FromHtml(_xmlOptions.ProgressBarForeColor);
                _progressBarBackColor = ColorTranslator.FromHtml(_xmlOptions.ProgressBarBackColor);

                if (_progressBarDock != DockStyle.Bottom && _progressBarDock != DockStyle.Top)
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Refresh the picture box with the current user wallpaper
        /// </summary>
        /// <param name="forceUpdate"></param>
        /// <returns></returns>
        private bool RefreshBackgroundImage(bool forceUpdate = false)
        {
            // Get the current user's wallpaper
            string wallpaperPath = GetCurrentUserWallpaperPath();

            // If wallpaper is blank or doesn't exist then quit as we can't do anything useful
            if (string.IsNullOrEmpty(wallpaperPath) || !File.Exists(wallpaperPath))
            {
                return false;
            }

            // Get Current filename and filetime and check if we need to update the image
            DateTime modifiedTime = File.GetLastWriteTime(wallpaperPath);
            if (!forceUpdate && _wallpaperPath == wallpaperPath && _wallpaperLastModified.CompareTo(modifiedTime) == 0)
            {
                return false;
            }

            // Save new values
            _wallpaperPath = wallpaperPath;
            _wallpaperLastModified = modifiedTime;

            // Set the form bitmap and force display to primary monitor
            StartPosition = FormStartPosition.Manual;
            Location = Screen.PrimaryScreen.Bounds.Location;
            WindowState = FormWindowState.Maximized;

            try
            {
                using (var fileStream = new FileStream(wallpaperPath, FileMode.Open, FileAccess.Read))
                {
                    pictureBoxBackground.BackgroundImage = Image.FromStream(fileStream);
                    pictureBoxBackground.Invalidate();
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private void RefreshProgressBar()
        {
            if (!_progressBarEnabled)
            {
                progressBar.Visible = false;
                return;
            }

            try
            {
#if DEBUG
                var currentInstruction = 50;
                var lastInstruction = 100;
#else
                int currentInstruction = int.Parse(TaskSequence.GetVariable("_SMSTSNextInstructionPointer")) + 1;
                int lastInstruction = int.Parse(TaskSequence.GetVariable("_SMSTSInstructionTableSize")) + 1;
#endif

                if (currentInstruction > lastInstruction)
                {
                    currentInstruction = lastInstruction;
                }

                // Format to client size
                progressBar.Left = ClientRectangle.Left;
                progressBar.Width = ClientSize.Width;
                progressBar.Height = _progressBarHeight;

                if (_progressBarDock == DockStyle.Bottom)
                {
                    progressBar.Top = ClientSize.Height - _progressBarHeight - _progressBarOffset;
                }
                else if (_progressBarDock == DockStyle.Top)
                {
                    progressBar.Top = _progressBarOffset;
                }

                if (progressBar.Top < ClientRectangle.Top)
                {
                    progressBar.Top = ClientRectangle.Top;
                }
                else if (progressBar.Top > ClientRectangle.Bottom)
                {
                    progressBar.Top = ClientRectangle.Bottom;
                }

                // Set percentage and make visible
                progressBar.ForeColor = _progressBarForeColor;
                progressBar.BackColor = _progressBarBackColor;
                progressBar.Value = 100 * currentInstruction / lastInstruction;
                progressBar.Visible = true;

                // Ensure in front of picture box
                progressBar.BringToFront();
            }
            catch (Exception)
            {
                progressBar.Visible = false;
            }
        }

        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            // Force update of background image in case of resolution change
            RefreshBackgroundImage(true);

            // Push the Win7/Win10 progress screen to the bottom, then put our screen on top
            BringToFrontOfWindowsSetupProgress();
        }

        /// <summary>
        ///     Closes form when detects logoff/shutdown.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
        {
            Close();
        }

        /// <summary>
        ///     Called on internal to update bitmap and variables.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            // Stop time while we process
            timerRefresh.Stop();

            // Update the background image if it's changed
            RefreshBackgroundImage();

            // Update overall progress bar
            RefreshProgressBar();

            // Restart timer
            timerRefresh.Start();
        }
    }
}