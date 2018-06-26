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
            this.buttonPasswordOK.Location = new System.Drawing.Point(348, 16);
            this.buttonPasswordOK.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonPasswordOK.Name = "buttonPasswordOK";
            this.buttonPasswordOK.Size = new System.Drawing.Size(87, 28);
            this.buttonPasswordOK.TabIndex = 2;
            this.buttonPasswordOK.Text = "OK";
            this.buttonPasswordOK.UseVisualStyleBackColor = true;
            this.buttonPasswordOK.Click += new System.EventHandler(this.buttonPasswordOK_Click);
            // 
            // buttonPasswordCancel
            // 
            this.buttonPasswordCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonPasswordCancel.Location = new System.Drawing.Point(348, 52);
            this.buttonPasswordCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonPasswordCancel.Name = "buttonPasswordCancel";
            this.buttonPasswordCancel.Size = new System.Drawing.Size(87, 28);
            this.buttonPasswordCancel.TabIndex = 3;
            this.buttonPasswordCancel.Text = "Cancel";
            this.buttonPasswordCancel.UseVisualStyleBackColor = true;
            this.buttonPasswordCancel.Click += new System.EventHandler(this.buttonPasswordCancel_Click);
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(15, 36);
            this.textBoxPassword.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.PasswordChar = '*';
            this.textBoxPassword.Size = new System.Drawing.Size(325, 23);
            this.textBoxPassword.TabIndex = 1;
            // 
            // labelPassword
            // 
            this.labelPassword.AutoSize = true;
            this.labelPassword.Location = new System.Drawing.Point(12, 16);
            this.labelPassword.Name = "labelPassword";
            this.labelPassword.Size = new System.Drawing.Size(68, 16);
            this.labelPassword.TabIndex = 0;
            this.labelPassword.Text = "Password:";
            // 
            // FormPassword
            // 
            this.AcceptButton = this.buttonPasswordOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonPasswordCancel;
            this.ClientSize = new System.Drawing.Size(449, 96);
            this.ControlBox = false;
            this.Controls.Add(this.labelPassword);
            this.Controls.Add(this.textBoxPassword);
            this.Controls.Add(this.buttonPasswordCancel);
            this.Controls.Add(this.buttonPasswordOK);
            this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FormPassword";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormPassword";
            this.Load += new System.EventHandler(this.FormPassword_Load);
            this.Shown += new System.EventHandler(this.FormPassword_Shown);
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