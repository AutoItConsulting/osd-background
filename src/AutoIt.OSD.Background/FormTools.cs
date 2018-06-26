//  
// Copyright (c) AutoIt Consulting Ltd. All rights reserved.  
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.  
// using System;
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
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
        private readonly DataGridViewCellStyle _rowStyleReadOnly;
        private readonly Options _xmlOptions;
        private Dictionary<string, string> _taskSequenceDictionary = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        private bool _taskSequenceVariablesEnabled;
        private bool _taskSequenceVariablesReadOnly;
        private bool _userToolsEnabled;
        private PasswordMode _passwordMode;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="xmlOptions"></param>
        public FormTools(Options xmlOptions, PasswordMode passwordMode)
        {
            _xmlOptions = xmlOptions;
            _passwordMode = passwordMode;

            InitializeComponent();

            // Our readonly row style for the grid view
            _rowStyleReadOnly = new DataGridViewCellStyle
            {
                Font = new Font(dgvTaskSequenceVariables.Font, FontStyle.Italic)
            };
        }

        /// <summary>
        ///     Called when the app unload and close button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCloseApp_Click(object sender, EventArgs e)
        {
            // Use the Abort result to signify that we want to unload the entire app
            DialogResult = DialogResult.Abort;
        }

        /// <summary>
        ///     Called when the run tool button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        ///     Called when the Refresh task sequence variables button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonVariablesRefresh_Click(object sender, EventArgs e)
        {
            _taskSequenceDictionary = TaskSequence.GetAllVariables();
            VariablesDictionaryViewUpdate();
        }

        /// <summary>
        ///     Called before a cell is edited. Cancelable.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        ///     Called after a cell has been edited.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                        MessageBox.Show(Resources.UnableToCreateVariabledWithUnderscore, Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        row.Cells[0].Value = null;
                        row.Cells[1].Value = null;
                        return;
                    }

                    if (_taskSequenceDictionary.ContainsKey(varName))
                    {
                        MessageBox.Show(Resources.VariableNameAlreadyExists, Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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

        /// <summary>
        ///     Called when the form is first loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormTools_Load(object sender, EventArgs e)
        {
            // Set title
            Text = _xmlOptions.Title;

            // Set main icon
            Icon = Resources.main;

            _userToolsEnabled = (_xmlOptions.UserTools.EnabledAdmin && _passwordMode == PasswordMode.Admin) ||
                                (_xmlOptions.UserTools.EnabledUser && _passwordMode == PasswordMode.User);
            _taskSequenceVariablesEnabled = _xmlOptions.TaskSequenceVariables.Enabled;
            _taskSequenceVariablesReadOnly = _xmlOptions.TaskSequenceVariables.ReadOnly;

            // Showing the tools tab?
            if (_userToolsEnabled)
            {
                // Populate filtered tools list depending on access level
                var filteredTools = new List<UserTool>();

                foreach (UserTool tool in _xmlOptions.UserTools.UserToolList)
                {
                    if (tool.AdminOnly)
                    {
                        if (_passwordMode == PasswordMode.Admin)
                        {
                            filteredTools.Add(tool);
                        }
                    }
                    else
                    {
                        filteredTools.Add(tool);
                    }
                }

                listBoxUserTools.DataSource = filteredTools;
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

            // Showing the task sequence?
            if (_taskSequenceVariablesEnabled == false)
            {
                tabControl.TabPages.Remove(tabVariables);
            }

            // No info yet
            tabControl.TabPages.Remove(tabInformation);

            // Get tools window close to the top so that user can see it
            BringToFront();

            _taskSequenceDictionary = TaskSequence.GetAllVariables();
            VariablesDictionaryViewUpdate();
        }

        /// <summary>
        ///     Called when double clicking on the tools listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBoxUserTools_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = listBoxUserTools.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                // Clicked on an item, use the button click handler
                buttonUserToolRun_Click(sender, e);
            }
        }

        /// <summary>
        ///     Called when tab page index changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        ///     Called when selecing a tab page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl_Selecting(object sender, TabControlCancelEventArgs e)
        {
            // Don't allow selection if the tab is disabled
            e.Cancel = !e.TabPage.Enabled;
        }

        /// <summary>
        ///     Gets the currently set task sequence variables and populates the user interface.
        /// </summary>
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

                        // Make readonly variables readonly rows
                        if (_taskSequenceVariablesReadOnly || kvp.Key.StartsWith("_"))
                        {
                            row.ReadOnly = true;
                            row.DefaultCellStyle = _rowStyleReadOnly;
                        }

                        return row;
                    }).ToArray());

            // Scroll to the first non-readonly variable
            foreach (DataGridViewRow row in dgvTaskSequenceVariables.Rows)
            {
                if (row.Cells[0].Value != null && row.Cells[0].Value.ToString().StartsWith("_") == false)
                {
                    int rowIndex = row.Index;
                    dgvTaskSequenceVariables.CurrentCell = dgvTaskSequenceVariables.Rows[rowIndex].Cells[0];
                    break;
                }
            }
        }
    }
}