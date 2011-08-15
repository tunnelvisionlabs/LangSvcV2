namespace Tvl.VisualStudio.Language.Java.Project.PropertyPages
{
    using System;
    using Path = System.IO.Path;

    public partial class JavaBuildPropertyPagePanel : JavaPropertyPagePanel
    {
        public JavaBuildPropertyPagePanel()
            : this( null )
        {
        }

        public JavaBuildPropertyPagePanel( JavaPropertyPage parentPropertyPage )
            : base( parentPropertyPage )
        {
            InitializeComponent();
            RefreshCommandLine();
        }

        public new JavaBuildPropertyPage ParentPropertyPage
        {
            get
            {
                return base.ParentPropertyPage as JavaBuildPropertyPage;
            }
        }

        public bool DebugMode
        {
            get
            {
                return chkDebugBuild.Checked;
            }

            set
            {
                chkDebugBuild.Checked = value;
            }
        }

        public string ExtraArguments
        {
            get
            {
                return txtBuildExtraOptions.Text;
            }

            set
            {
                txtBuildExtraOptions.Text = value;
            }
        }

        internal void RefreshCommandLine()
        {
            string line = string.Empty;
            string ucc = null;
            if (ParentPropertyPage.ProjectManager != null && ParentPropertyPage.ProjectManager.SharedBuildOptions.General != null)
                ucc = ParentPropertyPage.ProjectManager.SharedBuildOptions.General.JavacPath;
            if (ucc == null)
                ucc = ParentPropertyPage.GetConfigProperty(JavaConfigConstants.JavacPath, ProjectPropertyStorage.ProjectFile);

            string fullucc = ucc;
            try
            {
                if (!Path.IsPathRooted(fullucc) && ParentPropertyPage.ProjectManager != null)
                {
                    fullucc = Path.Combine(ParentPropertyPage.ProjectManager.ProjectFolder, ucc);
                }
            }
            catch (ArgumentException)
            {
                fullucc = ucc;
            }

            line = "\"" + ucc + "\" make";
            if (DebugMode)
                line += " -debug";
            if (!string.IsNullOrEmpty(ExtraArguments))
                line += " " + ExtraArguments;

            txtBuildCommandLine.Text = line;
        }

        // Build debug scripts
        private void checkBox1_CheckedChanged( object sender, EventArgs e )
        {
            ParentPropertyPage.IsDirty = true;
            RefreshCommandLine();
        }

        // Include unpublished
        private void checkBox2_CheckedChanged( object sender, EventArgs e )
        {
            ParentPropertyPage.IsDirty = true;
            RefreshCommandLine();
        }

        // Additional options
        private void textBox2_TextChanged( object sender, EventArgs e )
        {
            ParentPropertyPage.IsDirty = true;
            RefreshCommandLine();
        }
    }
}
