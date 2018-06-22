//  
// Copyright (c) AutoIt Consulting Ltd. All rights reserved.  
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.  
// using System;
//

using System;
using System.Windows.Forms;
using AutoIt.OSD.Background.Properties;

namespace AutoIt.OSD.Background
{
    public partial class FormPassword : Form
    {
        private Options _xmlOptions;

        public FormPassword(Options xmlOptions)
        {
            _xmlOptions = xmlOptions;

            InitializeComponent();
        }

        private void buttonPasswordOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_xmlOptions.Password) || _xmlOptions.Password == textBoxPassword.Text)
            {
                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Invalid password.", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
                textBoxPassword.Clear();
            }
        }

        private void FormPassword_Load(object sender, EventArgs e)
        {
            // Set title
            Text = _xmlOptions.Title;

            // Set main icon
            Icon = Resources.main;
        }

        private void FormPassword_Shown(object sender, EventArgs e)
        {
            textBoxPassword.Select();

            // Get tools window close to the top so that user can see it
            BringToFront();
        }
    }
}