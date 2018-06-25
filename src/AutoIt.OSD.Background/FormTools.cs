//  
// Copyright (c) AutoIt Consulting Ltd. All rights reserved.  
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.  
// using System;
//

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using AutoIt.OSD.Background.Properties;

namespace AutoIt.OSD.Background
{
    public partial class FormTools : Form
    {
        private readonly string _appPath = Directory.GetParent(Assembly.GetExecutingAssembly().Location).ToString();
        private bool _userToolsEnabled;
        private Options _xmlOptions;

        public FormTools(Options xmlOptions)
        {
            _xmlOptions = xmlOptions;

            InitializeComponent();
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

        private void buttonUserToolRun_Click(object sender, EventArgs e)
        {
            var userTool = (UserTool)listBoxUserTools.SelectedItem;

            // Set environment variable so user tools can reference this path
            Environment.SetEnvironmentVariable("OSDBackgoundExeDir", _appPath);

            // Expand environment variables
            string program = Environment.ExpandEnvironmentVariables(userTool.Program);
            string arguments = Environment.ExpandEnvironmentVariables(userTool.Arguments);
            string workingDirectory = Environment.ExpandEnvironmentVariables(userTool.WorkingDirectory);

            // Run the tool
            var startInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                FileName = program
            };

            // Arguments and WorkingDirectory may be blank, so set as required
            if (!string.IsNullOrEmpty(arguments))
            {
                startInfo.Arguments = arguments;
            }

            if (!string.IsNullOrEmpty(workingDirectory))
            {
                startInfo.WorkingDirectory = workingDirectory;
            }

            try
            {
                // Start process and dipose
                using (Process.Start(startInfo))
                {
                }

                // Finished 
                return;
            }
            catch (Win32Exception exception)
            {
                // Check for elevation error
                if ((uint)exception.ErrorCode != 0x80004005)
                {
                    MessageBox.Show(Resources.ErrorLaunchingTool + exception.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(Resources.ErrorLaunchingTool + exception.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Retry and elevate (working directory won't be valid now though)
            try
            {
                startInfo.UseShellExecute = true;
                startInfo.Verb = "runas";

                // Start process and dipose
                using (Process.Start(startInfo))
                {
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(Resources.ErrorLaunchingTool + exception.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormTools_Load(object sender, EventArgs e)
        {
            // Set title
            Text = _xmlOptions.Title;

            // Set main icon
            Icon = Resources.main;

            _userToolsEnabled = ConvertStringToBool(_xmlOptions.UserTools.UserToolsEnabled);

            // Showing the tools tab?
            if (_userToolsEnabled)
            {
                listBoxUserTools.DataSource = _xmlOptions.UserTools.UserToolList;
                listBoxUserTools.DisplayMember = "Name";

                listBoxUserTools.Select();

                // Select first tab
                tabControl.SelectTab(tabUserTools);
                AcceptButton = buttonUserToolRun;
            }
            else
            {
                tabControl.TabPages.Remove(tabUserTools);
            }

            // Get tools window close to the top so that user can see it
            BringToFront();
        }

        private void listBoxUserTools_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = listBoxUserTools.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                // Clicked on an item, use the button click handler
                buttonUserToolRun_Click(sender, e);
            }
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab == tabControl.TabPages["tabUserTools"])
            {
                AcceptButton = buttonUserToolRun;
            }
            else
            {
                AcceptButton = null;
            }
        }

        private void tabControl_Selecting(object sender, TabControlCancelEventArgs e)
        {
            // Don't allow selection if the tab is disabled
            e.Cancel = !e.TabPage.Enabled;
        }
    }
}