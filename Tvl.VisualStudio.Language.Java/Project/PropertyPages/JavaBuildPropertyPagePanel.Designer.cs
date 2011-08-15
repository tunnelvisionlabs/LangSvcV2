namespace Tvl.VisualStudio.Language.Java.Project.PropertyPages
{
    partial class JavaBuildPropertyPagePanel
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
            System.Windows.Forms.Label label1;
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtBuildExtraOptions = new System.Windows.Forms.TextBox();
            this.txtBuildCommandLine = new System.Windows.Forms.TextBox();
            this.chkDebugBuild = new System.Windows.Forms.CheckBox();
            label3 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.txtBuildExtraOptions);
            this.groupBox1.Controls.Add(this.txtBuildCommandLine);
            this.groupBox1.Controls.Add(label3);
            this.groupBox1.Controls.Add(label1);
            this.groupBox1.Controls.Add(this.chkDebugBuild);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(441, 319);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Compiler Options";
            // 
            // txtBuildExtraOptions
            // 
            this.txtBuildExtraOptions.Location = new System.Drawing.Point(43, 222);
            this.txtBuildExtraOptions.Multiline = true;
            this.txtBuildExtraOptions.Name = "txtBuildExtraOptions";
            this.txtBuildExtraOptions.Size = new System.Drawing.Size(355, 80);
            this.txtBuildExtraOptions.TabIndex = 9;
            this.txtBuildExtraOptions.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // txtBuildCommandLine
            // 
            this.txtBuildCommandLine.Location = new System.Drawing.Point(43, 95);
            this.txtBuildCommandLine.Multiline = true;
            this.txtBuildCommandLine.Name = "txtBuildCommandLine";
            this.txtBuildCommandLine.ReadOnly = true;
            this.txtBuildCommandLine.Size = new System.Drawing.Size(355, 98);
            this.txtBuildCommandLine.TabIndex = 8;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(40, 79);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(77, 13);
            label3.TabIndex = 6;
            label3.Text = "Command Line";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(40, 206);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(95, 13);
            label1.TabIndex = 6;
            label1.Text = "Additional Options:";
            // 
            // chkDebugBuild
            // 
            this.chkDebugBuild.AutoSize = true;
            this.chkDebugBuild.Location = new System.Drawing.Point(43, 29);
            this.chkDebugBuild.Name = "chkDebugBuild";
            this.chkDebugBuild.Size = new System.Drawing.Size(82, 17);
            this.chkDebugBuild.TabIndex = 5;
            this.chkDebugBuild.Text = "Build debug";
            this.chkDebugBuild.UseVisualStyleBackColor = true;
            this.chkDebugBuild.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // JavaBuildPropertyPagePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.MinimumSize = new System.Drawing.Size(447, 326);
            this.Name = "JavaBuildPropertyPagePanel";
            this.Size = new System.Drawing.Size(447, 326);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkDebugBuild;
        private System.Windows.Forms.TextBox txtBuildExtraOptions;
        private System.Windows.Forms.TextBox txtBuildCommandLine;

    }
}
