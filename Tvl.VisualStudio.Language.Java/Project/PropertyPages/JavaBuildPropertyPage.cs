namespace Tvl.VisualStudio.Language.Java.Project.PropertyPages
{
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    [Guid(JavaProjectConstants.JavaBuildPropertyPageGuidString)]
    public class JavaBuildPropertyPage : JavaPropertyPage
    {
        public JavaBuildPropertyPage()
        {
            PageName = JavaConfigConstants.PageNameBuild;
        }

        public new JavaBuildPropertyPagePanel PropertyPagePanel
        {
            get
            {
                return (JavaBuildPropertyPagePanel)base.PropertyPagePanel;
            }
        }

        protected override void BindProperties()
        {
            if (ProjectManager != null)
                ProjectManager.SharedBuildOptions.Build = PropertyPagePanel;

            PropertyPagePanel.DebugMode = GetConfigPropertyBoolean(JavaConfigConstants.DebugSymbols);
            PropertyPagePanel.ExtraArguments = GetConfigProperty(JavaConfigConstants.BuildArgs);
        }

        protected override bool ApplyChanges()
        {
            SetConfigProperty(JavaConfigConstants.DebugSymbols, PropertyPagePanel.DebugMode);
            SetConfigProperty(JavaConfigConstants.BuildArgs, PropertyPagePanel.ExtraArguments);
            return true;
        }

        protected override JavaPropertyPagePanel CreatePropertyPagePanel()
        {
            return new JavaBuildPropertyPagePanel(this);
        }
    }
}
