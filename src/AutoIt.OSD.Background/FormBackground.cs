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
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using AutoIt.OSD.Background.Properties;
using AutoIt.Windows;
using Microsoft.Win32;

namespace AutoIt.OSD.Background
{
    public partial class FormBackground : Form
    {
        private const string MutexName = "MUTEX_AUTOIT_OSD_BACKGROUND";
        private const string PipeName = "PIPE_AUTOIT_OSD_BACKGROUND";

        private const int TimerIntervalSecs = 1;

        /// <summary>
        ///     Directory containing this instance of the exe
        /// </summary>
        private readonly string _appPath = Directory.GetParent(Assembly.GetExecutingAssembly().Location).ToString();

        /// <summary>
        ///     Signal to update the options recieved from another instance.
        /// </summary>
        private readonly ManualResetEvent _eventNewOptionsAvailable = new ManualResetEvent(false);

        /// <summary>
        ///     Signal for the application to close.
        /// </summary>
        private readonly ManualResetEvent _eventShutdownRequested = new ManualResetEvent(false);

        private readonly KeyboardHook _keyboardHook = new KeyboardHook();
        private readonly object _namedPiperServerThreadLock = new object();
        private bool _customBackgroundEnabled;

        private bool _firstApplicationInstance;

        private FormTools _formTools;

        private Mutex _mutexApplication;

        private IAsyncResult _namedPipeAsyncResult;
        private NamedPipeServerStream _namedPipeServerStream;
        private Thread _namedPipeServerThread;
        private NamedPipeXmlPayload _namedPipeXmlPayload;

        private Options _options;

        private string _osdBackgroundDir;

        private Color _progressBarBackColor;
        private DockStyle _progressBarDock;
        private bool _progressBarEnabled;
        private Color _progressBarForeColor;
        private int _progressBarHeight;
        private int _progressBarOffset;

        private bool _startedInTaskSequence;
        private bool _taskSequenceVariablesEnabled;
        private bool _userToolsEnabled;

        private DateTime _wallpaperLastModified;
        private string _wallpaperPath = string.Empty;

        /// <inheritdoc />
        public FormBackground()
        {
            InitializeComponent();

            // Set main icon
            Icon = Resources.main;

            // Set these here rather than designer as it makes it easier to work with designer
            pictureBoxBackground.Dock = DockStyle.Fill;
            pictureBoxBackground.SizeMode = PictureBoxSizeMode.StretchImage;

            // Store the exe dir and working dir so we can pass this info to another instance
            _osdBackgroundDir = Directory.GetParent(Assembly.GetExecutingAssembly().Location).ToString();
        }

        /// <inheritdoc />
        protected override bool ShowWithoutActivation
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        ///     Gets a value indicating if the tools menu is currently showing.
        /// </summary>
        private bool ShowingPasswordOrTools { get; set; }

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
            Process[] processes = Process.GetProcessesByName(AppDomain.CurrentDomain.FriendlyName.Remove(AppDomain.CurrentDomain.FriendlyName.Length - 4));
            if (processes.Length <= 1)
            {
                return;
            }

            foreach (Process process in processes)
            {
                if (process.Id != Process.GetCurrentProcess().Id)
                {
                    process.Kill();
                    process.WaitForExit(5000);
                    break;
                }
            }
        }

        /// <summary>
        ///     Sends the Windows Setup window to the bottom, and then puts our form in front of that.
        /// </summary>
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

        /// <summary>
        ///     Runs when form is starting to close.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormBackground_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Stop timer
            timerRefresh.Stop();

            // Remove static events
            SystemEvents.SessionEnding -= SystemEvents_SessionEnding;

            // Dispose keyboard hook
            _keyboardHook.Dispose();

            // Flag quit signal in case any thread is mid execution. Timer is stopped so it won't trigger another Close() call.
            _eventShutdownRequested.Set();

            // Give the server thread time to stop before foricbly terminating it
            if (_namedPipeServerThread != null && !_namedPipeServerThread.Join(5000))
            {
                _namedPipeServerThread.Abort();
            }

            // Close our mutex
            if (_mutexApplication != null)
            {
                _mutexApplication.Dispose();
                _mutexApplication = null;
            }
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
            if (!OptionsReadCommandLineAndOptionsFile())
            {
                Close();
                return;
            }

            // First update of background image
            if (RefreshBackgroundImage(true) == false)
            {
                MessageBox.Show(Resources.UnableToLoadUserWallpaper, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }

            // Make sure picture box is bottom most control on our form
            pictureBoxBackground.SendToBack();

            // First update of progress bar
            ProgressBarRedraw();
            ProgressBarRefresh();

            // Push the Win7/Win10 progress screen to the bottom, then put our form on top of that
            BringToFrontOfWindowsSetupProgress();

            // Trap shutdown
            SystemEvents.SessionEnding += SystemEvents_SessionEnding;

            // Trap display change
            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;

            // By default assume not in a task sequence
            _startedInTaskSequence = false;

            // Create a new background thread and start it
            _namedPipeServerThread = new Thread(NamedPipeServerThreadFunc)
            {
                IsBackground = true
            };
            _namedPipeServerThread.Start();
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
                timerRefresh.Interval = (int)TimeSpan.FromSeconds(TimerIntervalSecs).TotalMilliseconds;
                timerRefresh.Start();
            }
            else
            {
                Hide();
            }

            // Register a global keyboard hook to bring up the tools menu - can only do this after killing previous instances
            try
            {
                _keyboardHook.KeyPressed += KeyboardHook_OnPressed;
                _keyboardHook.RegisterHotKey(Background.ModifierKeys.Control | Background.ModifierKeys.Alt, Keys.F12);
            }
            catch (Exception)
            {
                MessageBox.Show(Resources.UnableToRegisterHotkey, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        /// <summary>
        ///     Checks if this is the first instance of this application. Can be run multiple times.
        /// </summary>
        /// <returns></returns>
        private bool IsApplicationFirstInstance()
        {
            // Allow for multiple runs but only try and get the mutex once
            if (_mutexApplication == null)
            {
                _mutexApplication = new Mutex(true, MutexName, out _firstApplicationInstance);
            }

            return _firstApplicationInstance;
        }

        /// <summary>
        ///     Runs when our global keyboard hook is triggered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeyboardHook_OnPressed(object sender, KeyPressedEventArgs e)
        {
            // Ignore if quit in progress
            if (_eventShutdownRequested.WaitOne(0))
            {
                return;
            }

            // Ignore if already showing the tools form
            if (ShowingPasswordOrTools)
            {
                return;
            }

            // Ignore if there are no possible tabs enabled
            if (_userToolsEnabled == false && _taskSequenceVariablesEnabled == false)
            {
                return;
            }

            ShowingPasswordOrTools = true;

            // Hide the background window because it causes issues when the user clicks on it
            if (_customBackgroundEnabled)
            {
                Activate();
                BringToFront();
                Hide();
            }

            // Hide TS dialog
            TaskSequence.CloseProgressDialog();

            // Ask for password if needed
            PasswordMode passwordMode;

            using (var formPassword = new FormPassword(_options))
            {
                formPassword.ShowDialog();
                passwordMode = formPassword.PasswordMode;
            }

            // If password is ok, launch the tools
            if (passwordMode != PasswordMode.None)
            {
                _formTools = new FormTools(_options, passwordMode, _osdBackgroundDir);
                DialogResult result = _formTools.ShowDialog(this);
                _formTools.Dispose();
                _formTools = null;

                // Check if closed via the "Close App" button 
                if (result == DialogResult.Abort)
                {
                    // Queue the quit signal
                    _eventShutdownRequested.Set();
                }
            }

            // Reshow the background and push it to the back again
            if (_customBackgroundEnabled)
            {
                Show();
            }

            // Push the Win7/Win10 progress screen to the bottom, then put our form on top of that
            BringToFrontOfWindowsSetupProgress();

            // Reshow TS progress
            TaskSequence.ShowTsProgress();

            ShowingPasswordOrTools = false;
        }

        /// <summary>
        ///     Uses a named pipe to send the currently parsed options to an already running instance.
        /// </summary>
        /// <param name="namedPipePayload"></param>
        private void NamedPipeClientSendOptions(NamedPipeXmlPayload namedPipePayload)
        {
            //List<string> arguments = Environment.GetCommandLineArgs().ToList();

            try
            {
                using (var namedPipeClientStream = new NamedPipeClientStream(".", PipeName, PipeDirection.Out))
                {
                    namedPipeClientStream.Connect(3000); // Maximum wait 3 seconds

                    var xmlSerializer = new XmlSerializer(typeof(NamedPipeXmlPayload));
                    xmlSerializer.Serialize(namedPipeClientStream, namedPipePayload);
                }
            }
            catch (Exception)
            {
                // Error connecting or sending
            }
        }

        /// <summary>
        ///     The function called when a client connects to the named pipe.
        /// </summary>
        /// <param name="iAsyncResult"></param>
        private void NamedPipeServerConnectionCallback(IAsyncResult iAsyncResult)
        {
            try
            {
                // End waiting for the connection
                _namedPipeServerStream.EndWaitForConnection(_namedPipeAsyncResult);

                // Read data and prevent access to _namedPipeXmlPayload during threaded operations
                lock (_namedPiperServerThreadLock)
                {
                    // Read data from client
                    var xmlSerializer = new XmlSerializer(typeof(NamedPipeXmlPayload));
                    _namedPipeXmlPayload = (NamedPipeXmlPayload)xmlSerializer.Deserialize(_namedPipeServerStream);

                    // Need to signal quit?
                    if (_namedPipeXmlPayload.SignalQuit)
                    {
                        _eventShutdownRequested.Set();
                        return;
                    }

                    // Flag that there is new options data available. It will be applied on timer event
                    // when the tools menus are NOT open
                    _eventNewOptionsAvailable.Set();
                }
            }
            catch (ObjectDisposedException)
            {
                // EndWaitForConnection will exception when someone calls .Close() before a connection received
                // In that case we dont create any more pipes and just return
                // This will happen when app is closing and our pipe is closed
                return;
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                // Close the original server (we have to create a new one each time)
                if (_namedPipeServerStream != null)
                {
                    _namedPipeServerStream.Dispose();
                    _namedPipeServerStream = null;
                }

                _namedPipeAsyncResult = null;
            }

            // Create a new pipe for next connection
            NamedPipeServerCreateServer();
        }

        /// <summary>
        ///     Starts a new pipe server if one isn't already active.
        /// </summary>
        private void NamedPipeServerCreateServer()
        {
            if (_namedPipeServerStream != null)
            {
                // Already a pipe setup, just return
                return;
            }

            // Create a new pipe accessible by local authenticated users, disallow network
            //var sidAuthUsers = new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null);
            var sidNetworkService = new SecurityIdentifier(WellKnownSidType.NetworkServiceSid, null);
            var sidWorld = new SecurityIdentifier(WellKnownSidType.WorldSid, null);

            var pipeSecurity = new PipeSecurity();

            // Deny network access to the pipe
            var accessRule = new PipeAccessRule(sidNetworkService, PipeAccessRights.ReadWrite, AccessControlType.Deny);
            pipeSecurity.AddAccessRule(accessRule);

            // Alow Everyone to read/write
            accessRule = new PipeAccessRule(sidWorld, PipeAccessRights.ReadWrite, AccessControlType.Allow);
            pipeSecurity.AddAccessRule(accessRule);

            // This user is the owner (can create pipes)
            SecurityIdentifier sidOwner = WindowsIdentity.GetCurrent().Owner;
            if (sidOwner != null)
            {
                accessRule = new PipeAccessRule(sidOwner, PipeAccessRights.FullControl, AccessControlType.Allow);
                pipeSecurity.AddAccessRule(accessRule);
            }

            // Create pipe and start the async connection wait
            _namedPipeServerStream = new NamedPipeServerStream(
                PipeName,
                PipeDirection.In,
                1,
                PipeTransmissionMode.Byte,
                PipeOptions.Asynchronous,
                0,
                0,
                pipeSecurity);

            _namedPipeAsyncResult = _namedPipeServerStream.BeginWaitForConnection(NamedPipeServerConnectionCallback, _namedPipeServerStream);
        }

        /// <summary>
        ///     The main worker thread. This runs until the service stops.
        /// </summary>
        private void NamedPipeServerThreadFunc()
        {
            // Create a new pipe 
            NamedPipeServerCreateServer();

            // Wait until we get asked to shutdown
            _eventShutdownRequested.WaitOne();

            // Close pipe if it's currently waiting
            if (_namedPipeServerStream != null)
            {
                _namedPipeServerStream.Dispose();
            }
        }

        /// <summary>
        ///     Processes command line options and options.xml
        /// </summary>
        /// <returns></returns>
        private bool OptionsReadCommandLineAndOptionsFile()
        {
            string[] arguments = Environment.GetCommandLineArgs();
            string optionsFilename = string.Empty;

            if (arguments.Length >= 2)
            {
                string arg = arguments[1].ToUpper();

                if (arg == "/?" || arg == "?")
                {
                    var usage = @"AutoIt.OSD.Background [/Close] | [Options.xml]";
                    MessageBox.Show(usage, Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                if (arg == "/CLOSE" || arg == "CLOSE")
                {
                    // If we are not the first instance, send a quit message along the pipe
                    if (!IsApplicationFirstInstance())
                    {
                        //KillPreviousInstance();
                        var namedPipeXmlPayload = new NamedPipeXmlPayload
                        {
                            SignalQuit = true
                        };

                        NamedPipeClientSendOptions(namedPipeXmlPayload);
                    }

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
                _options = (Options)deSerializer.Deserialize(reader);
                _options.SanityCheck();
            }
            catch (Exception e)
            {
                string message = Resources.UnableToParseXml;

                if (e.InnerException != null)
                {
                    message += "\n\n" + e.InnerException.Message;
                }

                MessageBox.Show(message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Additional parsing of some string values in useful fields
            if (!OptionsXmlOptionsObjectToFields(_options))
            {
                return false;
            }

            // If are not the first instance then send a message to the running app with the new options and then quit
            if (!IsApplicationFirstInstance())
            {
                var namedPipeXmlPayload = new NamedPipeXmlPayload
                {
                    SignalQuit = false,
                    OsdBackgroundDir = _osdBackgroundDir,
                    OsdBackgroundWorkingDir = Directory.GetCurrentDirectory(),
                    Options = _options
                };

                NamedPipeClientSendOptions(namedPipeXmlPayload);
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Some of the strings in the xml object need parsing, this populates fields for those values
        /// </summary>
        /// <returns>Returns true if all fields successfully parsed and valid.</returns>
        private bool OptionsXmlOptionsObjectToFields(Options xmlOptions)
        {
            // TODO: Move this logic into Options.cs

            try
            {
                _customBackgroundEnabled = xmlOptions.CustomBackground.Enabled;
                _userToolsEnabled = xmlOptions.UserTools.EnabledAdmin | xmlOptions.UserTools.EnabledUser;
                _taskSequenceVariablesEnabled = xmlOptions.TaskSequenceVariables.EnabledAdmin || xmlOptions.TaskSequenceVariables.EnabledUser;

                _progressBarEnabled = xmlOptions.CustomBackground.ProgressBar.Enabled;
                _progressBarHeight = xmlOptions.CustomBackground.ProgressBar.Height;
                _progressBarOffset = xmlOptions.CustomBackground.ProgressBar.Offset;
                _progressBarDock = (DockStyle)Enum.Parse(typeof(DockStyle), xmlOptions.CustomBackground.ProgressBar.Dock, true);
                _progressBarForeColor = ColorTranslator.FromHtml(xmlOptions.CustomBackground.ProgressBar.ForeColor);
                _progressBarBackColor = ColorTranslator.FromHtml(xmlOptions.CustomBackground.ProgressBar.BackColor);

                if (_progressBarDock != DockStyle.Bottom && _progressBarDock != DockStyle.Top)
                {
                    _progressBarDock = DockStyle.Bottom;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Fully redraws the progress bar, also handles size changes based on the screen size.
        /// </summary>
        private void ProgressBarRedraw()
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
                    _eventShutdownRequested.Set();
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

            // Don't use Maximized as it goes over the task bar which can be ugly
            //Size = new Size(Screen.GetWorkingArea(this).Width, Screen.GetWorkingArea(this).Height);

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
            ProgressBarRedraw();
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
        ///     This will perform the neccessary changes based on an options change request. This is handled during a timer request
        ///     and the timer will be paused.
        /// </summary>
        private void TimerHandleOptionsChangeEvent()
        {
            // Lock so no chance of receiving another new options file during processing
            lock (_namedPiperServerThreadLock)
            {
                if (_eventNewOptionsAvailable.WaitOne(0))
                {
                    _eventNewOptionsAvailable.Reset();

                    // Save data
                    _osdBackgroundDir = _namedPipeXmlPayload.OsdBackgroundDir;
                    _options = _namedPipeXmlPayload.Options;

                    // Change fields based on updated options data
                    OptionsXmlOptionsObjectToFields(_options);

                    // Change the working dir to match the last AutoIt.OSDBackground.exe that was run
                    Directory.SetCurrentDirectory(_namedPipeXmlPayload.OsdBackgroundWorkingDir);

                    // Redraw the background and progress bar (size and/or color might have new options)
                    RefreshBackgroundImage(true);
                    ProgressBarRedraw();
                }
            }
        }

        /// <summary>
        ///     Called on interval to update bitmap and variables and check for quit signals
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerHandleTickEvent(object sender, EventArgs e)
        {
            // If the tools menu is showing then don't do anything (ignores quit signals, new options until tools closed)
            if (ShowingPasswordOrTools)
            {
                return;
            }

            // Stop time while we process
            timerRefresh.Stop();

            // Has a new options file been received?  If so handle it.
            TimerHandleOptionsChangeEvent();

            // Update the background image if it's changed
            RefreshBackgroundImage();

            // Update overall progress bar percentage
            ProgressBarRefresh();

            // Is quit signalled? We only check it when tools not showing to prevent another instance loading
            // and closing our app while using the tools menu
            if (_eventShutdownRequested.WaitOne(0))
            {
                // Close form, and don't restart timer
                Close();
            }

            // Restart timer
            timerRefresh.Start();
        }
    }
}