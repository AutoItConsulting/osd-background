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
            this.buttonUserToolRun = new System.Windows.Forms.Button();
            this.listBoxUserTools = new System.Windows.Forms.ListBox();
            this.tabInformation = new System.Windows.Forms.TabPage();
            this.tabVariables = new System.Windows.Forms.TabPage();
            this.dgvTaskSequenceVariables = new System.Windows.Forms.DataGridView();
            this.buttonCloseTools = new System.Windows.Forms.Button();
            this.buttonCloseApp = new System.Windows.Forms.Button();
            this.VariableName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VariableValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buttonVariablesRefresh = new System.Windows.Forms.Button();
            this.tabControl.SuspendLayout();
            this.tabUserTools.SuspendLayout();
            this.tabVariables.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTaskSequenceVariables)).BeginInit();
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
            this.tabControl.Size = new System.Drawing.Size(599, 379);
            this.tabControl.TabIndex = 0;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            this.tabControl.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControl_Selecting);
            // 
            // tabUserTools
            // 
            this.tabUserTools.BackColor = System.Drawing.SystemColors.Control;
            this.tabUserTools.Controls.Add(this.buttonUserToolRun);
            this.tabUserTools.Controls.Add(this.listBoxUserTools);
            this.tabUserTools.Location = new System.Drawing.Point(4, 22);
            this.tabUserTools.Name = "tabUserTools";
            this.tabUserTools.Padding = new System.Windows.Forms.Padding(3);
            this.tabUserTools.Size = new System.Drawing.Size(591, 353);
            this.tabUserTools.TabIndex = 0;
            this.tabUserTools.Text = "User Tools";
            // 
            // buttonUserToolRun
            // 
            this.buttonUserToolRun.Location = new System.Drawing.Point(258, 319);
            this.buttonUserToolRun.Name = "buttonUserToolRun";
            this.buttonUserToolRun.Size = new System.Drawing.Size(75, 23);
            this.buttonUserToolRun.TabIndex = 2;
            this.buttonUserToolRun.Text = "Run Tool";
            this.buttonUserToolRun.UseVisualStyleBackColor = true;
            this.buttonUserToolRun.Click += new System.EventHandler(this.buttonUserToolRun_Click);
            // 
            // listBoxUserTools
            // 
            this.listBoxUserTools.FormattingEnabled = true;
            this.listBoxUserTools.Location = new System.Drawing.Point(6, 7);
            this.listBoxUserTools.Name = "listBoxUserTools";
            this.listBoxUserTools.ScrollAlwaysVisible = true;
            this.listBoxUserTools.Size = new System.Drawing.Size(579, 303);
            this.listBoxUserTools.TabIndex = 1;
            this.listBoxUserTools.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBoxUserTools_MouseDoubleClick);
            // 
            // tabInformation
            // 
            this.tabInformation.Location = new System.Drawing.Point(4, 22);
            this.tabInformation.Name = "tabInformation";
            this.tabInformation.Padding = new System.Windows.Forms.Padding(3);
            this.tabInformation.Size = new System.Drawing.Size(591, 353);
            this.tabInformation.TabIndex = 1;
            this.tabInformation.Text = "Information";
            // 
            // tabVariables
            // 
            this.tabVariables.BackColor = System.Drawing.SystemColors.Control;
            this.tabVariables.Controls.Add(this.buttonVariablesRefresh);
            this.tabVariables.Controls.Add(this.dgvTaskSequenceVariables);
            this.tabVariables.Location = new System.Drawing.Point(4, 22);
            this.tabVariables.Name = "tabVariables";
            this.tabVariables.Size = new System.Drawing.Size(591, 353);
            this.tabVariables.TabIndex = 2;
            this.tabVariables.Text = "Task Sequence Variables";
            // 
            // dgvTaskSequenceVariables
            // 
            this.dgvTaskSequenceVariables.AllowUserToDeleteRows = false;
            this.dgvTaskSequenceVariables.AllowUserToResizeRows = false;
            this.dgvTaskSequenceVariables.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvTaskSequenceVariables.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.VariableName,
            this.VariableValue});
            this.dgvTaskSequenceVariables.Location = new System.Drawing.Point(6, 7);
            this.dgvTaskSequenceVariables.Name = "dgvTaskSequenceVariables";
            this.dgvTaskSequenceVariables.RowHeadersVisible = false;
            this.dgvTaskSequenceVariables.RowHeadersWidth = 120;
            this.dgvTaskSequenceVariables.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvTaskSequenceVariables.Size = new System.Drawing.Size(582, 303);
            this.dgvTaskSequenceVariables.TabIndex = 0;
            this.dgvTaskSequenceVariables.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgvTaskSequenceVariables_CellBeginEdit);
            this.dgvTaskSequenceVariables.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTaskSequenceVariables_CellEndEdit);
            // 
            // buttonCloseTools
            // 
            this.buttonCloseTools.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCloseTools.Location = new System.Drawing.Point(23, 406);
            this.buttonCloseTools.Name = "buttonCloseTools";
            this.buttonCloseTools.Size = new System.Drawing.Size(164, 23);
            this.buttonCloseTools.TabIndex = 1;
            this.buttonCloseTools.Text = "Close Tools Menu";
            this.buttonCloseTools.UseVisualStyleBackColor = true;
            // 
            // buttonCloseApp
            // 
            this.buttonCloseApp.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCloseApp.Location = new System.Drawing.Point(441, 406);
            this.buttonCloseApp.Name = "buttonCloseApp";
            this.buttonCloseApp.Size = new System.Drawing.Size(164, 23);
            this.buttonCloseApp.TabIndex = 2;
            this.buttonCloseApp.Text = "Quit and Unload";
            this.buttonCloseApp.UseVisualStyleBackColor = true;
            this.buttonCloseApp.Click += new System.EventHandler(this.buttonCloseApp_Click);
            // 
            // VariableName
            // 
            this.VariableName.DividerWidth = 3;
            this.VariableName.Frozen = true;
            this.VariableName.HeaderText = "Name";
            this.VariableName.MinimumWidth = 50;
            this.VariableName.Name = "VariableName";
            this.VariableName.Width = 200;
            // 
            // VariableValue
            // 
            this.VariableValue.DividerWidth = 3;
            this.VariableValue.HeaderText = "Value";
            this.VariableValue.MinimumWidth = 50;
            this.VariableValue.Name = "VariableValue";
            this.VariableValue.Width = 1000;
            // 
            // buttonVariablesRefresh
            // 
            this.buttonVariablesRefresh.Location = new System.Drawing.Point(258, 319);
            this.buttonVariablesRefresh.Name = "buttonVariablesRefresh";
            this.buttonVariablesRefresh.Size = new System.Drawing.Size(75, 23);
            this.buttonVariablesRefresh.TabIndex = 3;
            this.buttonVariablesRefresh.Text = "Refresh";
            this.buttonVariablesRefresh.UseVisualStyleBackColor = true;
            this.buttonVariablesRefresh.Click += new System.EventHandler(this.buttonVariablesRefresh_Click);
            // 
            // FormTools
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCloseTools;
            this.ClientSize = new System.Drawing.Size(624, 441);
            this.ControlBox = false;
            this.Controls.Add(this.buttonCloseApp);
            this.Controls.Add(this.buttonCloseTools);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormTools";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormMain";
            this.Load += new System.EventHandler(this.FormTools_Load);
            this.tabControl.ResumeLayout(false);
            this.tabUserTools.ResumeLayout(false);
            this.tabVariables.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTaskSequenceVariables)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabUserTools;
        private System.Windows.Forms.TabPage tabInformation;
        private System.Windows.Forms.Button buttonCloseTools;
        private System.Windows.Forms.TabPage tabVariables;
        private System.Windows.Forms.ListBox listBoxUserTools;
        private System.Windows.Forms.Button buttonUserToolRun;
        private System.Windows.Forms.DataGridView dgvTaskSequenceVariables;
        private System.Windows.Forms.Button buttonCloseApp;
        private System.Windows.Forms.Button buttonVariablesRefresh;
        private System.Windows.Forms.DataGridViewTextBoxColumn VariableName;
        private System.Windows.Forms.DataGridViewTextBoxColumn VariableValue;
    }
}