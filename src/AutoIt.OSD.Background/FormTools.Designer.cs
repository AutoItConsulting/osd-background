namespace AutoIt.OSD.Background
{
    partial class FormTools
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
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabUserTools = new System.Windows.Forms.TabPage();
            this.listBoxUserTools = new System.Windows.Forms.ListBox();
            this.tabInformation = new System.Windows.Forms.TabPage();
            this.tabVariables = new System.Windows.Forms.TabPage();
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonUserToolRun = new System.Windows.Forms.Button();
            this.tabControl.SuspendLayout();
            this.tabUserTools.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabUserTools);
            this.tabControl.Controls.Add(this.tabInformation);
            this.tabControl.Controls.Add(this.tabVariables);
            this.tabControl.Location = new System.Drawing.Point(13, 13);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(439, 379);
            this.tabControl.TabIndex = 0;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            this.tabControl.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControl_Selecting);
            // 
            // tabUserTools
            // 
            this.tabUserTools.BackColor = System.Drawing.Color.Transparent;
            this.tabUserTools.Controls.Add(this.buttonUserToolRun);
            this.tabUserTools.Controls.Add(this.listBoxUserTools);
            this.tabUserTools.Location = new System.Drawing.Point(4, 22);
            this.tabUserTools.Name = "tabUserTools";
            this.tabUserTools.Padding = new System.Windows.Forms.Padding(3);
            this.tabUserTools.Size = new System.Drawing.Size(431, 353);
            this.tabUserTools.TabIndex = 0;
            this.tabUserTools.Text = "User Tools";
            // 
            // listBoxUserTools
            // 
            this.listBoxUserTools.FormattingEnabled = true;
            this.listBoxUserTools.Location = new System.Drawing.Point(6, 7);
            this.listBoxUserTools.Name = "listBoxUserTools";
            this.listBoxUserTools.ScrollAlwaysVisible = true;
            this.listBoxUserTools.Size = new System.Drawing.Size(419, 303);
            this.listBoxUserTools.TabIndex = 1;
            this.listBoxUserTools.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBoxUserTools_MouseDoubleClick);
            // 
            // tabInformation
            // 
            this.tabInformation.Location = new System.Drawing.Point(4, 22);
            this.tabInformation.Name = "tabInformation";
            this.tabInformation.Padding = new System.Windows.Forms.Padding(3);
            this.tabInformation.Size = new System.Drawing.Size(431, 353);
            this.tabInformation.TabIndex = 1;
            this.tabInformation.Text = "Information";
            // 
            // tabVariables
            // 
            this.tabVariables.Location = new System.Drawing.Point(4, 22);
            this.tabVariables.Name = "tabVariables";
            this.tabVariables.Size = new System.Drawing.Size(431, 353);
            this.tabVariables.TabIndex = 2;
            this.tabVariables.Text = "Task Sequence Variables";
            this.tabVariables.UseVisualStyleBackColor = true;
            // 
            // buttonClose
            // 
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Location = new System.Drawing.Point(195, 406);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 1;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            // 
            // buttonUserToolRun
            // 
            this.buttonUserToolRun.Location = new System.Drawing.Point(178, 324);
            this.buttonUserToolRun.Name = "buttonUserToolRun";
            this.buttonUserToolRun.Size = new System.Drawing.Size(75, 23);
            this.buttonUserToolRun.TabIndex = 2;
            this.buttonUserToolRun.Text = "Run Tool";
            this.buttonUserToolRun.UseVisualStyleBackColor = true;
            this.buttonUserToolRun.Click += new System.EventHandler(this.buttonUserToolRun_Click);
            // 
            // FormTools
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(464, 441);
            this.ControlBox = false;
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormTools";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormMain";
            this.Load += new System.EventHandler(this.FormTools_Load);
            this.tabControl.ResumeLayout(false);
            this.tabUserTools.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabUserTools;
        private System.Windows.Forms.TabPage tabInformation;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.TabPage tabVariables;
        private System.Windows.Forms.ListBox listBoxUserTools;
        private System.Windows.Forms.Button buttonUserToolRun;
    }
}