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

            // general
            PropertyPagePanel.SourceRelease = GetConfigProperty(JavaConfigConstants.SourceRelease);
            PropertyPagePanel.TargetRelease = GetConfigProperty(JavaConfigConstants.TargetRelease);
            PropertyPagePanel.Encoding = GetConfigProperty(JavaConfigConstants.SourceEncoding);

            // debugging
            DebuggingInformation info;
            if (!Enum.TryParse(GetConfigProperty(JavaConfigConstants.DebugSymbols), out info))
                info = DebuggingInformation.Default;

            PropertyPagePanel.DebuggingInformation = info;
            PropertyPagePanel.SpecificDebuggingInformation = GetConfigProperty(JavaConfigConstants.SpecificDebugSymbols);

            // warnings
            PropertyPagePanel.ShowWarnings = GetConfigPropertyBoolean(JavaConfigConstants.ShowWarnings);
            PropertyPagePanel.ShowAllWarnings = GetConfigPropertyBoolean(JavaConfigConstants.ShowAllWarnings);

            // warnings as errors
            WarningsAsErrors warnAsError;
            if (!Enum.TryParse(GetConfigProperty(JavaConfigConstants.TreatWarningsAsErrors), out warnAsError))
                warnAsError = WarningsAsErrors.None;

            PropertyPagePanel.WarningsAsErrors = warnAsError;
            PropertyPagePanel.SpecificWarningsAsErrors = GetConfigProperty(JavaConfigConstants.WarningsAsErrors);

            // output
            PropertyPagePanel.OutputPath = GetConfigProperty(JavaConfigConstants.OutputPath);

            // extra arguments
            PropertyPagePanel.ExtraArguments = GetConfigProperty(JavaConfigConstants.BuildArgs);
        }

        protected override bool ApplyChanges()
        {
            // general
            SetConfigProperty(JavaConfigConstants.SourceRelease, PropertyPagePanel.SourceRelease);
            SetConfigProperty(JavaConfigConstants.TargetRelease, PropertyPagePanel.TargetRelease);
            SetConfigProperty(JavaConfigConstants.SourceEncoding, PropertyPagePanel.Encoding);

            // debugging
            SetConfigProperty(JavaConfigConstants.DebugSymbols, PropertyPagePanel.DebuggingInformation.ToString());
            SetConfigProperty(JavaConfigConstants.SpecificDebugSymbols, PropertyPagePanel.SpecificDebuggingInformation);

            // warnings
            SetConfigProperty(JavaConfigConstants.ShowWarnings, PropertyPagePanel.ShowWarnings);
            SetConfigProperty(JavaConfigConstants.ShowAllWarnings, PropertyPagePanel.ShowAllWarnings);

            // warnings as errors
            SetConfigProperty(JavaConfigConstants.TreatWarningsAsErrors, PropertyPagePanel.WarningsAsErrors.ToString());
            SetConfigProperty(JavaConfigConstants.WarningsAsErrors, PropertyPagePanel.SpecificWarningsAsErrors);

            // output
            SetConfigProperty(JavaConfigConstants.OutputPath, PropertyPagePanel.OutputPath);

            // extra arguments
            SetConfigProperty(JavaConfigConstants.BuildArgs, PropertyPagePanel.ExtraArguments);

            return true;
        }

        protected override JavaPropertyPagePanel CreatePropertyPagePanel()
        {
            return new JavaBuildPropertyPagePanel(this);
        }
    }
}
