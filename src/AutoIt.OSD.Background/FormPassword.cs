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
    public enum PasswordMode
    {
        None,
        Admin,
        User
    }

    public partial class FormPassword : Form
    {
        private readonly Options _xmlOptions;

        public FormPassword(Options xmlOptions)
        {
            _xmlOptions = xmlOptions;

            InitializeComponent();

            // Default mode is None/bad password
            PasswordMode = PasswordMode.None;
        }

        public PasswordMode PasswordMode { get; private set; }

        private void buttonPasswordOK_Click(object sender, EventArgs e)
        {
            // If admin password blank then return in admin mode
            if (string.IsNullOrEmpty(_xmlOptions.PasswordAdmin))
            {
                DialogResult = DialogResult.OK;
                PasswordMode = PasswordMode.Admin;
                return;
            }

            // Check for admin password
            if (_xmlOptions.PasswordAdmin == textBoxPassword.Text)
            {
                DialogResult = DialogResult.OK;
                PasswordMode = PasswordMode.Admin;
                return;
            }

            // If user password blank then return in user mode
            if (string.IsNullOrEmpty(_xmlOptions.PasswordUser))
            {
                DialogResult = DialogResult.OK;
                PasswordMode = PasswordMode.User;
                return;
            }

            // Check for user password
            if (_xmlOptions.PasswordUser == textBoxPassword.Text)
            {
                DialogResult = DialogResult.OK;
                PasswordMode = PasswordMode.User;
                return;
            }

            // Error
            PasswordMode = PasswordMode.None;
            DialogResult = DialogResult.None;

            MessageBox.Show(Resources.InvalidPassword, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            textBoxPassword.Clear();
        }

        private void FormPassword_Load(object sender, EventArgs e)
        {
            // Set title
            Text = _xmlOptions.Title;

            // Set main icon
            Icon = Resources.main;

            // If both passwords are blank then abort showing the form
            if (string.IsNullOrEmpty(_xmlOptions.PasswordAdmin) && string.IsNullOrEmpty(_xmlOptions.PasswordUser))
            {
                DialogResult = DialogResult.Cancel;
            }
        }

        private void FormPassword_Shown(object sender, EventArgs e)
        {
            textBoxPassword.Select();

            // Get tools window close to the top so that user can see it
            BringToFront();
        }

        private void buttonPasswordCancel_Click(object sender, EventArgs e)
        {
            PasswordMode = PasswordMode.None;
            DialogResult = DialogResult.Cancel;
        }
    }
}