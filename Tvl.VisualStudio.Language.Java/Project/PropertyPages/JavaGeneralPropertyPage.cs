namespace Tvl.VisualStudio.Language.Java.Project.PropertyPages
{
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    [Guid(JavaProjectConstants.JavaGeneralPropertyPageGuidString)]
    public class JavaGeneralPropertyPage : JavaPropertyPage
    {
        public JavaGeneralPropertyPage()
        {
            PageName = JavaConfigConstants.PageNameGeneral;
        }

        public new JavaGeneralPropertyPagePanel PropertyPagePanel
        {
            get
            {
                return (JavaGeneralPropertyPagePanel)base.PropertyPagePanel;
            }
        }

        protected override void BindProperties()
        {
            if (ProjectManager != null)
                ProjectManager.SharedBuildOptions.General = PropertyPagePanel;

            PropertyPagePanel.ProjectFolder = ProjectManager.ProjectFolder;
            PropertyPagePanel.JavacPath = GetConfigProperty(JavaConfigConstants.JavacPath, ProjectPropertyStorage.ProjectFile);
        }

        protected override bool ApplyChanges()
        {
            SetProperty(JavaConfigConstants.JavacPath, PropertyPagePanel.JavacPath, ProjectPropertyStorage.ProjectFile);
            return true;
        }

        protected override JavaPropertyPagePanel CreatePropertyPagePanel()
        {
            return new JavaGeneralPropertyPagePanel(this);
        }
    }
}
