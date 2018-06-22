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

        private Form _formTools;

        private KeyboardHook _keyboardHook = new KeyboardHook();

        private bool _customBackgroundEnabled;

        private bool _progressBarEnabled;
        private DockStyle _progressBarDock;
        private Color _progressBarBackColor;
        private Color _progressBarForeColor;
        private int _progressBarHeight;
        private int _progressBarOffset;

        private bool _startedInTaskSequence;

        private DateTime _wallpaperLastModified;

        private string _wallpaperPath = string.Empty;
        private Options _xmlOptions;

        /// <inheritdoc />
        public FormBackground()
        {
            InitializeComponent();

            // Set main icon
            Icon = Resources.main;

            // Set these here rather than designer as it makes it easier to work with designer
            pictureBoxBackground.Dock = DockStyle.Fill;
            pictureBoxBackground.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        /// <summary>
        ///     We don't want this window to activate when it is shown
        /// </summary>
        protected override bool ShowWithoutActivation => true;

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

            const uint flag = Windows.NativeMethods.SWP_NOACTIVATE | Windows.NativeMethods.SWP_NOMOVE | Windows.NativeMethods.SWP_NOSIZE;

            // If there are no progress windows then just move to bottom most
            if (progressWindows.Count == 0)
            {
                Windows.NativeMethods.SetWindowPos(Handle, Windows.NativeMethods.HWND_BOTTOM, 0, 0, 0, 0, flag);
                return;
            }

            // After various tests with modifying Z-order without ever obscuring the foreground what
            // seems to work best is to move our window right to the bottom, then move the Win7/10 progress
            // window to the bottom
            // We'll also try and hide the Windows screen for good measure
            Windows.NativeMethods.SetWindowPos(Handle, Windows.NativeMethods.HWND_BOTTOM, 0, 0, 0, 0, flag);
            Windows.NativeMethods.SetWindowPos(progressWindows[0], Windows.NativeMethods.HWND_BOTTOM, 0, 0, 0, 0, flag);
            Windows.NativeMethods.ShowWindowAsync(progressWindows[0], Windows.NativeMethods.SW_HIDE);
        }

        private void FormBackground_Activated(object sender, EventArgs e)
        {
            //MessageBox.Show("Activated");
            //BringToFrontOfWindowsSetupProgress();
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

            // Dispose keyboard hook
            _keyboardHook.Dispose();
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
            ProgressBarResize();
            ProgressBarRefresh();

            // Push the Win7/Win10 progress screen to the bottom, then put our form on top of that
            BringToFrontOfWindowsSetupProgress();

            // Trap shutdown
            SystemEvents.SessionEnding += SystemEvents_SessionEnding;

            // Trap display change
            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;

            // Register a global keyboard hook to bring up the tools menu
            _keyboardHook.KeyPressed += KeyboardHook_OnPressed;
            _keyboardHook.RegisterHotKey(Background.ModifierKeys.Control | Background.ModifierKeys.Alt, Keys.F12);

            // By default assume not in a task sequence
            _startedInTaskSequence = false;
        }

        /// <summary>
        ///     Shown event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormBackground_Shown(object sender, EventArgs e)
        {
            if (_customBackgroundEnabled)
            {
                // Let the form finish showing (so if we kill a previous running version no flicker)
                Refresh();

                // Start the refresh timer
                timerRefresh.Interval = (int)TimeSpan.FromSeconds(RefreshInervalSecs).TotalMilliseconds;
                timerRefresh.Start();
            }
            else
            {
                Hide();
            }

            // We don't want any other versions running - kill it after we have completely shown our new screen to reduce flicker
            KillPreviousInstance();
        }

        /// <summary>
        ///     Processes command line options and options.xml
        /// </summary>
        /// <returns></returns>
        private bool GetOptions()
        {
            string[] arguments = Environment.GetCommandLineArgs();
            string optionsFilename = string.Empty;

            if (arguments.Length >= 2)
            {
                string arg = arguments[1].ToUpper();

                if (arg == "/?" || arg == "?")
                {
                    var usage = @"AutoIt.OSD.Background [/Close] | [Options.xml]";
                    MessageBox.Show(usage, AppDomain.CurrentDomain.FriendlyName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                if (arg == "/CLOSE" || arg == "CLOSE")
                {
                    KillPreviousInstance();
                    return false;
                }

                // Get the options filename
                optionsFilename = arguments[1];
            }

            // If no filename specified, use options.xml in current folder
            if (arguments.Length == 1)
            {
                optionsFilename = _appPath + @"\Options.xml";
            }

            try
            {
                var deSerializer = new XmlSerializer(typeof(Options));
                TextReader reader = new StreamReader(optionsFilename);
                _xmlOptions = (Options)deSerializer.Deserialize(reader);

                _customBackgroundEnabled = ConvertStringToBool(_xmlOptions.CustomBackgroundEnabled);

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

                // Can't have progress if no background
                if (!_customBackgroundEnabled)
                {
                    _progressBarEnabled = false;
                }
            }
            catch (Exception)
            {
                MessageBox.Show(@"Unable to read or parse Options.xml file.", AppDomain.CurrentDomain.FriendlyName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Runs when our global keyboard hook is triggered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeyboardHook_OnPressed(object sender, KeyPressedEventArgs e)
        {
            // Ignore if already showing the tools form
            if (_formTools != null)
            {
                return;
            }

            // Hide the background window because it causes issues when the user clicks on it
            if (_customBackgroundEnabled)
            {
                Hide();
            }

            // Ask for password if needed
            var result = DialogResult.OK;
            if (!string.IsNullOrEmpty(_xmlOptions.Password))
            {
                using (Form formPassword = new FormPassword(_xmlOptions))
                {
                    result = formPassword.ShowDialog();
                }
            }

            // If password is ok, launch the tools
            if (result == DialogResult.OK)
            {
                

                using (_formTools = new FormTools(_xmlOptions))
                {
                    _formTools.ShowDialog();
                }

                _formTools = null;
            }

            // Reshow the background and push it to the back again
            if (_customBackgroundEnabled)
            {
                Show();
            }

            // Push the Win7/Win10 progress screen to the bottom, then put our form on top of that
            BringToFrontOfWindowsSetupProgress();
        }

        /// <summary>
        ///     Updates the progress bar based on the position in the task sequence.
        /// </summary>
        private void ProgressBarRefresh()
        {
            // Get position in task sequence if there is one
            int currentInstruction;
            int lastInstruction;

            try
            {
#if DEBUG
                currentInstruction = 50;
                lastInstruction = 100;
#else
                // Get the current position in the task sequence - will get blanks and exceptions if not in a TS
                currentInstruction = int.Parse(TaskSequence.GetVariable("_SMSTSNextInstructionPointer")) + 1;
                lastInstruction = int.Parse(TaskSequence.GetVariable("_SMSTSInstructionTableSize")) + 1;
#endif

                if (currentInstruction > lastInstruction)
                {
                    currentInstruction = lastInstruction;
                }
            }
            catch (Exception)
            {
                // Error reading task sequence variables, remove progress bar
                progressBar.Visible = false;

                // Have we been running in a task sequence before? If so, assume that the task sequence has ended
                // and close down - this prevents situations where the caller forgets to close us at the end of a TS
                if (_startedInTaskSequence)
                {
                    Close();
                }

                return;
            }

            // If we reached here, we are in a task sequence, update flag
            _startedInTaskSequence = true;

            // If bar is not enabled then nothing else to do
            if (!_progressBarEnabled)
            {
                progressBar.Visible = false;
                return;
            }

            // Set percentage and make visible
            progressBar.Value = 100 * currentInstruction / lastInstruction;
            progressBar.Visible = true;
        }

        /// <summary>
        ///     Updates the progress bar based on the screen size.
        /// </summary>
        private void ProgressBarResize()
        {
            if (!_progressBarEnabled)
            {
                return;
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

            progressBar.ForeColor = _progressBarForeColor;
            progressBar.BackColor = _progressBarBackColor;

            // Ensure in front of picture box
            progressBar.BringToFront();
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
            
            // Don't use Maximized as it goes over the task bar which can be ugly
            //WindowState = FormWindowState.Maximized;
            Size = new Size(Screen.GetWorkingArea(this).Width, Screen.GetWorkingArea(this).Height);

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

        /// <summary>
        ///     Event to handle display settings changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            // Force update of background image in case of resolution change
            RefreshBackgroundImage(true);

            // Push the Win7/Win10 progress screen to the bottom, then put our screen on top
            BringToFrontOfWindowsSetupProgress();

            // Resize progress bar
            ProgressBarResize();
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
        ///     Called on interval to update bitmap and variables.
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
            ProgressBarRefresh();

            // Restart timer
            timerRefresh.Start();
        }
    }
}