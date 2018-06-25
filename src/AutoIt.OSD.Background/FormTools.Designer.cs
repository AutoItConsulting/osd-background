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
            this.buttonVariablesRefresh = new System.Windows.Forms.Button();
            this.dgvTaskSequenceVariables = new System.Windows.Forms.DataGridView();
            this.VariableName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VariableValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buttonCloseTools = new System.Windows.Forms.Button();
            this.buttonCloseApp = new System.Windows.Forms.Button();
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
            this.tabControl.Location = new System.Drawing.Point(15, 16);
            this.tabControl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(677, 438);
            this.tabControl.TabIndex = 0;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            this.tabControl.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControl_Selecting);
            // 
            // tabUserTools
            // 
            this.tabUserTools.BackColor = System.Drawing.SystemColors.Control;
            this.tabUserTools.Controls.Add(this.buttonUserToolRun);
            this.tabUserTools.Controls.Add(this.listBoxUserTools);
            this.tabUserTools.Location = new System.Drawing.Point(4, 25);
            this.tabUserTools.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabUserTools.Name = "tabUserTools";
            this.tabUserTools.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabUserTools.Size = new System.Drawing.Size(669, 409);
            this.tabUserTools.TabIndex = 0;
            this.tabUserTools.Text = "User Tools";
            // 
            // buttonUserToolRun
            // 
            this.buttonUserToolRun.Location = new System.Drawing.Point(291, 373);
            this.buttonUserToolRun.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonUserToolRun.Name = "buttonUserToolRun";
            this.buttonUserToolRun.Size = new System.Drawing.Size(87, 28);
            this.buttonUserToolRun.TabIndex = 2;
            this.buttonUserToolRun.Text = "Run Tool";
            this.buttonUserToolRun.UseVisualStyleBackColor = true;
            this.buttonUserToolRun.Click += new System.EventHandler(this.buttonUserToolRun_Click);
            // 
            // listBoxUserTools
            // 
            this.listBoxUserTools.FormattingEnabled = true;
            this.listBoxUserTools.ItemHeight = 16;
            this.listBoxUserTools.Location = new System.Drawing.Point(6, 9);
            this.listBoxUserTools.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.listBoxUserTools.Name = "listBoxUserTools";
            this.listBoxUserTools.ScrollAlwaysVisible = true;
            this.listBoxUserTools.Size = new System.Drawing.Size(657, 356);
            this.listBoxUserTools.TabIndex = 1;
            this.listBoxUserTools.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBoxUserTools_MouseDoubleClick);
            // 
            // tabInformation
            // 
            this.tabInformation.Location = new System.Drawing.Point(4, 25);
            this.tabInformation.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabInformation.Name = "tabInformation";
            this.tabInformation.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabInformation.Size = new System.Drawing.Size(669, 409);
            this.tabInformation.TabIndex = 1;
            this.tabInformation.Text = "Information";
            // 
            // tabVariables
            // 
            this.tabVariables.BackColor = System.Drawing.SystemColors.Control;
            this.tabVariables.Controls.Add(this.buttonVariablesRefresh);
            this.tabVariables.Controls.Add(this.dgvTaskSequenceVariables);
            this.tabVariables.Location = new System.Drawing.Point(4, 25);
            this.tabVariables.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabVariables.Name = "tabVariables";
            this.tabVariables.Size = new System.Drawing.Size(669, 409);
            this.tabVariables.TabIndex = 2;
            this.tabVariables.Text = "Task Sequence Variables";
            // 
            // buttonVariablesRefresh
            // 
            this.buttonVariablesRefresh.Location = new System.Drawing.Point(291, 373);
            this.buttonVariablesRefresh.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonVariablesRefresh.Name = "buttonVariablesRefresh";
            this.buttonVariablesRefresh.Size = new System.Drawing.Size(87, 28);
            this.buttonVariablesRefresh.TabIndex = 3;
            this.buttonVariablesRefresh.Text = "Refresh";
            this.buttonVariablesRefresh.UseVisualStyleBackColor = true;
            this.buttonVariablesRefresh.Click += new System.EventHandler(this.buttonVariablesRefresh_Click);
            // 
            // dgvTaskSequenceVariables
            // 
            this.dgvTaskSequenceVariables.AllowUserToDeleteRows = false;
            this.dgvTaskSequenceVariables.AllowUserToResizeRows = false;
            this.dgvTaskSequenceVariables.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvTaskSequenceVariables.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.VariableName,
            this.VariableValue});
            this.dgvTaskSequenceVariables.Location = new System.Drawing.Point(3, 4);
            this.dgvTaskSequenceVariables.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dgvTaskSequenceVariables.Name = "dgvTaskSequenceVariables";
            this.dgvTaskSequenceVariables.RowHeadersVisible = false;
            this.dgvTaskSequenceVariables.RowHeadersWidth = 120;
            this.dgvTaskSequenceVariables.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvTaskSequenceVariables.Size = new System.Drawing.Size(663, 354);
            this.dgvTaskSequenceVariables.TabIndex = 0;
            this.dgvTaskSequenceVariables.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgvTaskSequenceVariables_CellBeginEdit);
            this.dgvTaskSequenceVariables.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTaskSequenceVariables_CellEndEdit);
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
            // buttonCloseTools
            // 
            this.buttonCloseTools.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCloseTools.Location = new System.Drawing.Point(15, 462);
            this.buttonCloseTools.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonCloseTools.Name = "buttonCloseTools";
            this.buttonCloseTools.Size = new System.Drawing.Size(191, 28);
            this.buttonCloseTools.TabIndex = 1;
            this.buttonCloseTools.Text = "Close and Stay Running";
            this.buttonCloseTools.UseVisualStyleBackColor = true;
            // 
            // buttonCloseApp
            // 
            this.buttonCloseApp.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCloseApp.Location = new System.Drawing.Point(497, 462);
            this.buttonCloseApp.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonCloseApp.Name = "buttonCloseApp";
            this.buttonCloseApp.Size = new System.Drawing.Size(191, 28);
            this.buttonCloseApp.TabIndex = 2;
            this.buttonCloseApp.Text = "Close and Exit";
            this.buttonCloseApp.UseVisualStyleBackColor = true;
            this.buttonCloseApp.Click += new System.EventHandler(this.buttonCloseApp_Click);
            // 
            // FormTools
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCloseTools;
            this.ClientSize = new System.Drawing.Size(704, 501);
            this.ControlBox = false;
            this.Controls.Add(this.buttonCloseApp);
            this.Controls.Add(this.buttonCloseTools);
            this.Controls.Add(this.tabControl);
            this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
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