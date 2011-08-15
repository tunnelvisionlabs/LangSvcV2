namespace Tvl.VisualStudio.Language.Java.Project.PropertyPages
{
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    [Guid(JavaProjectConstants.JavaDebugPropertyPageGuidString)]
    public class JavaDebugPropertyPage : JavaPropertyPage
    {
        public JavaDebugPropertyPage()
        {
            PageName = JavaConfigConstants.PageNameDebug;
        }

        public new JavaDebugPropertyPagePanel PropertyPagePanel
        {
            get
            {
                return (JavaDebugPropertyPagePanel)base.PropertyPagePanel;
            }
        }

        protected override JavaPropertyPagePanel CreatePropertyPagePanel()
        {
            return new JavaDebugPropertyPagePanel(this);
        }

        protected override void BindProperties()
        {
            if (ProjectManager != null)
                ProjectManager.SharedBuildOptions.Debug = PropertyPagePanel;

            PropertyPagePanel.ProjectFolder = ProjectManager.ProjectFolder;

            PropertyPagePanel.StartExecutable = GetConfigProperty(JavaConfigConstants.DebugExe, ProjectPropertyStorage.UserFile);
            PropertyPagePanel.ExtraArguments = GetConfigProperty(JavaConfigConstants.DebugExtraArgs, ProjectPropertyStorage.UserFile);
        }

        protected override bool ApplyChanges()
        {
            SetConfigProperty(JavaConfigConstants.DebugExe, PropertyPagePanel.StartExecutable, ProjectPropertyStorage.UserFile);
            SetConfigProperty(JavaConfigConstants.DebugExtraArgs, PropertyPagePanel.ExtraArguments, ProjectPropertyStorage.UserFile);

            return true;
        }
    }
}
