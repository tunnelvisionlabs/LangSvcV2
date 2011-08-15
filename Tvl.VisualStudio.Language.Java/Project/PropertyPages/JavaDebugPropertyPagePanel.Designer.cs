namespace Tvl.VisualStudio.Language.Java.Project.PropertyPages
{
    partial class JavaDebugPropertyPagePanel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label label2;
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtExtraOptions = new System.Windows.Forms.TextBox();
            this.txtCommandLine = new System.Windows.Forms.TextBox();
            this.txtStartupExe = new Tvl.VisualStudio.Language.Java.Project.Controls.FileBrowserTextBox();
            this.label1 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtExtraOptions);
            this.groupBox1.Controls.Add(this.txtCommandLine);
            this.groupBox1.Controls.Add(label3);
            this.groupBox1.Controls.Add(label2);
            this.groupBox1.Controls.Add(this.txtStartupExe);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(553, 315);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Start Action";
            // 
            // txtExtraOptions
            // 
            this.txtExtraOptions.Location = new System.Drawing.Point(42, 214);
            this.txtExtraOptions.Multiline = true;
            this.txtExtraOptions.Name = "txtExtraOptions";
            this.txtExtraOptions.Size = new System.Drawing.Size(355, 80);
            this.txtExtraOptions.TabIndex = 13;
            this.txtExtraOptions.TextChanged += new System.EventHandler(this.HandleExtraOptionsTextChanged);
            // 
            // txtCommandLine
            // 
            this.txtCommandLine.Location = new System.Drawing.Point(42, 87);
            this.txtCommandLine.Multiline = true;
            this.txtCommandLine.Name = "txtCommandLine";
            this.txtCommandLine.ReadOnly = true;
            this.txtCommandLine.Size = new System.Drawing.Size(355, 98);
            this.txtCommandLine.TabIndex = 12;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(39, 71);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(77, 13);
            label3.TabIndex = 10;
            label3.Text = "Command Line";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(39, 198);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(95, 13);
            label2.TabIndex = 11;
            label2.Text = "Additional Options:";
            // 
            // txtStartupExe
            // 
            this.txtStartupExe.Location = new System.Drawing.Point(219, 25);
            this.txtStartupExe.MakeRelative = true;
            this.txtStartupExe.Margin = new System.Windows.Forms.Padding(0);
            this.txtStartupExe.MinimumSize = new System.Drawing.Size(64, 23);
            this.txtStartupExe.Name = "txtStartupExe";
            this.txtStartupExe.RootFolder = null;
            this.txtStartupExe.Size = new System.Drawing.Size(317, 22);
            this.txtStartupExe.TabIndex = 3;
            this.txtStartupExe.TextChanged += new System.EventHandler(this.HandleStartupExeTextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(39, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Start Executable:";
            // 
            // JavaDebugPropertyPagePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.groupBox1);
            this.MinimumSize = new System.Drawing.Size(563, 533);
            this.Name = "JavaDebugPropertyPagePanel";
            this.Size = new System.Drawing.Size(563, 533);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private Tvl.VisualStudio.Language.Java.Project.Controls.FileBrowserTextBox txtStartupExe;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtExtraOptions;
        private System.Windows.Forms.TextBox txtCommandLine;
    }
}
