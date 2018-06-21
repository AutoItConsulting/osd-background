//  
// Copyright (c) AutoIt Consulting Ltd. All rights reserved.  
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.  
// using System;
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using AutoIt.OSD.Background.Properties;

namespace AutoIt.OSD.Background
{
    public sealed partial class FormTools : Form
    {
        private List<string>_userTools = new List<string>();
        private Options _xmlOptions;
        private readonly string _appPath = Directory.GetParent(Assembly.GetExecutingAssembly().Location).ToString();

        public FormTools(Options xmlOptions)
        {
            _xmlOptions = xmlOptions;

            InitializeComponent();

            // Set title
            Text = xmlOptions.Title;

            // Set main icon
            Icon = Resources.main;

            listBoxUserTools.DataSource = _xmlOptions.UserTools;
            listBoxUserTools.DisplayMember = "Name";
        }

        private void tabControl_Selecting(object sender, TabControlCancelEventArgs e)
        {
            // Don't allow selection if the tab is disabled
            e.Cancel = !e.TabPage.Enabled;
        }

        private void FormTools_Load(object sender, EventArgs e)
        {
            listBoxUserTools.Select();

            // Select first tab
            tabControl.SelectTab("tabUserTools");
            AcceptButton = buttonUserToolRun;
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
                FileName = program,
                Arguments = arguments,
                WorkingDirectory = workingDirectory,
                
            };


            using (Process.Start(startInfo))
            {
            }

            //MessageBox.Show("Running: " + ((UserTool)listBoxUserTools.SelectedItem).Name);
            //MessageBox.Show("Running: " + program);
            //MessageBox.Show("Running: " + workingDirectory);
        }

        private void listBoxUserTools_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = this.listBoxUserTools.IndexFromPoint(e.Location);
            if (index != System.Windows.Forms.ListBox.NoMatches)
            {
                // Clicked on an item, use the button click handler
                buttonUserToolRun_Click(sender, e);
            }
        }
    }
}
