namespace Tvl.VisualStudio.Language.Java.Project.PropertyPages
{
    using System;
    using Path = System.IO.Path;

    public partial class JavaDebugPropertyPagePanel : JavaPropertyPagePanel
    {
        public JavaDebugPropertyPagePanel()
            : this(null)
        {
        }

        public JavaDebugPropertyPagePanel(JavaDebugPropertyPage parentPropertyPage)
            : base(parentPropertyPage)
        {
            InitializeComponent();
            RefreshCommandLine();
        }

        public new JavaDebugPropertyPage ParentPropertyPage
        {
            get
            {
                return (JavaDebugPropertyPage)base.ParentPropertyPage;
            }
        }

        public string ProjectFolder
        {
            get
            {
                return txtStartupExe.RootFolder;
            }

            set
            {
                txtStartupExe.RootFolder = value;
            }
        }

        public string StartExecutable
        {
            get
            {
                return txtStartupExe.Text;
            }

            set
            {
                txtStartupExe.Text = value;
            }
        }

        public string ExtraArguments
        {
            get
            {
                return txtExtraOptions.Text;
            }

            set
            {
                txtExtraOptions.Text = value;
            }
        }

        public string FullExecutablePath
        {
            get
            {
                string ucc = StartExecutable;
                if (string.IsNullOrEmpty(ucc))
                    return string.Empty;

                try
                {
                    string fullucc = ucc;
                    if (!Path.IsPathRooted(fullucc) && ParentPropertyPage.ProjectManager != null)
                        fullucc = Path.Combine(ParentPropertyPage.ProjectManager.ProjectFolder, ucc);

                    return Path.GetFullPath(fullucc);
                }
                catch (ArgumentException)
                {
                    return string.Empty;
                }
            }
        }

        public string CommandLine
        {
            get
            {
                string line = string.Empty;
                string fullpath = FullExecutablePath;
                if (string.IsNullOrEmpty(fullpath))
                    fullpath = StartExecutable;

                line = "\"" + fullpath + "\"";

                if (!string.IsNullOrEmpty(ExtraArguments))
                    line += " " + ExtraArguments;

                return line;
            }
        }

        private void RefreshCommandLine()
        {
            txtCommandLine.Text = CommandLine;
        }

        private void HandleStartupExeTextChanged(object sender, EventArgs e)
        {
            ParentPropertyPage.IsDirty = true;
            RefreshCommandLine();
        }

        private void HandleExtraOptionsTextChanged(object sender, EventArgs e)
        {
            ParentPropertyPage.IsDirty = true;
            RefreshCommandLine();
        }
    }
}
