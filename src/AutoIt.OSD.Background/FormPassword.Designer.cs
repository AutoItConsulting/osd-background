namespace AutoIt.OSD.Background
{
    partial class FormPassword
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonPasswordOK = new System.Windows.Forms.Button();
            this.buttonPasswordCancel = new System.Windows.Forms.Button();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.labelPassword = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonPasswordOK
            // 
            this.buttonPasswordOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonPasswordOK.Location = new System.Drawing.Point(298, 13);
            this.buttonPasswordOK.Name = "buttonPasswordOK";
            this.buttonPasswordOK.Size = new System.Drawing.Size(75, 23);
            this.buttonPasswordOK.TabIndex = 2;
            this.buttonPasswordOK.Text = "OK";
            this.buttonPasswordOK.UseVisualStyleBackColor = true;
            this.buttonPasswordOK.Click += new System.EventHandler(this.buttonPasswordOK_Click);
            // 
            // buttonPasswordCancel
            // 
            this.buttonPasswordCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonPasswordCancel.Location = new System.Drawing.Point(297, 43);
            this.buttonPasswordCancel.Name = "buttonPasswordCancel";
            this.buttonPasswordCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonPasswordCancel.TabIndex = 3;
            this.buttonPasswordCancel.Text = "Cancel";
            this.buttonPasswordCancel.UseVisualStyleBackColor = true;
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(13, 29);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.PasswordChar = '*';
            this.textBoxPassword.Size = new System.Drawing.Size(279, 20);
            this.textBoxPassword.TabIndex = 1;
            // 
            // labelPassword
            // 
            this.labelPassword.AutoSize = true;
            this.labelPassword.Location = new System.Drawing.Point(13, 10);
            this.labelPassword.Name = "labelPassword";
            this.labelPassword.Size = new System.Drawing.Size(56, 13);
            this.labelPassword.TabIndex = 0;
            this.labelPassword.Text = "Password:";
            // 
            // FormPassword
            // 
            this.AcceptButton = this.buttonPasswordOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonPasswordCancel;
            this.ClientSize = new System.Drawing.Size(385, 82);
            this.ControlBox = false;
            this.Controls.Add(this.labelPassword);
            this.Controls.Add(this.textBoxPassword);
            this.Controls.Add(this.buttonPasswordCancel);
            this.Controls.Add(this.buttonPasswordOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormPassword";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormPassword";
            this.Load += new System.EventHandler(this.FormPassword_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonPasswordOK;
        private System.Windows.Forms.Button buttonPasswordCancel;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Label labelPassword;
    }
}