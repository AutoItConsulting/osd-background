//  
// Copyright (c) AutoIt Consulting Ltd. All rights reserved.  
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.  
// using System;
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using AutoIt.OSD.Background.Properties;

namespace AutoIt.OSD.Background
{
    public partial class FormTools : Form
    {
        private readonly string _appPath = Directory.GetParent(Assembly.GetExecutingAssembly().Location).ToString();
        private Dictionary<string, string> _taskSequenceDictionary = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
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

        private void buttonCloseApp_Click(object sender, EventArgs e)
        {
            // Use the Abort result to signify that we want to unload the entire app
            DialogResult = DialogResult.Abort;
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

        private void buttonVariablesRefresh_Click(object sender, EventArgs e)
        {
            _taskSequenceDictionary = TaskSequence.GetAllVariables();
            VariablesDictionaryViewUpdate();
        }

        private void dgvTaskSequenceVariables_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            // If we editing values allow anything as long as the name is there
            if (e.ColumnIndex == 1)
            {
                if (dgvTaskSequenceVariables.Rows[e.RowIndex].Cells[0].Value == null)
                {
                    e.Cancel = true;
                }

                return;
            }

            // Column 0

            // If previous entry has a blank name don't allow
            if (e.RowIndex > 0 && dgvTaskSequenceVariables.Rows[e.RowIndex - 1].Cells[e.ColumnIndex].Value == null)
            {
                // Deny edit
                e.Cancel = true;
                return;
            }

            // Only allow editing names that are blank (i.e. new rows)
            if (dgvTaskSequenceVariables.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
            {
                // Deny edit
                e.Cancel = true;
            }
        }

        private void dgvTaskSequenceVariables_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dgvTaskSequenceVariables.Rows[e.RowIndex];

            var varName = (string)row.Cells[0].Value;
            var varValue = (string)row.Cells[1].Value;

            if (e.ColumnIndex == 0)
            {
                if (!string.IsNullOrEmpty(varName))
                {
                    // Can't set variables that start with _
                    if (varName.StartsWith("_"))
                    {
                        MessageBox.Show("Unable to create variables that being with an underscore.", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        row.Cells[0].Value = null;
                        row.Cells[1].Value = null;
                        return;
                    }

                    if (_taskSequenceDictionary.ContainsKey(varName))
                    {
                        MessageBox.Show("Variable name already exists.", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        row.Cells[0].Value = null;
                        row.Cells[1].Value = null;
                        return;
                    }
                }
            }

            // Only update if the name is valid
            if (!string.IsNullOrEmpty(varName))
            {
                if (!string.IsNullOrEmpty(varValue))
                {
                    _taskSequenceDictionary[varName] = varValue;
                    TaskSequence.SetVariable(varName, varValue);
                }
                else
                {
                    _taskSequenceDictionary[varName] = "";
                    TaskSequence.SetVariable(varName, "");
                }

                //VariablesDictionaryViewUpdate();
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

            _taskSequenceDictionary = TaskSequence.GetAllVariables();
            VariablesDictionaryViewUpdate();
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

        private void VariablesDictionaryViewUpdate()
        {
            //_UpdateDictionaryListView();
            dgvTaskSequenceVariables.Rows.Clear();
            dgvTaskSequenceVariables.Rows.AddRange(
                _taskSequenceDictionary.OrderBy(kvp => kvp.Key).Select(
                    kvp =>
                    {
                        var row = new DataGridViewRow();

                        if (string.IsNullOrEmpty(kvp.Key) || !TaskSequence.IsPasswordVariable(kvp.Key))
                        {
                            row.CreateCells(dgvTaskSequenceVariables, kvp.Key, kvp.Value);
                        }
                        else
                        {
                            row.CreateCells(dgvTaskSequenceVariables, kvp.Key, @"******** (Password removed)");
                        }

                        return row;
                    }).ToArray());
        }
    }
}