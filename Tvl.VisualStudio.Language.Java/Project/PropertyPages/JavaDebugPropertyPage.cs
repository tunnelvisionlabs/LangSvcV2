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

            StartAction startAction;
            if (!Enum.TryParse(GetConfigProperty(JavaConfigConstants.DebugStartAction, ProjectPropertyStorage.UserFile), out startAction))
                startAction = StartAction.Class;

            PropertyPagePanel.StartAction = startAction;
            PropertyPagePanel.StartClass = GetConfigProperty(JavaConfigConstants.DebugStartClass, ProjectPropertyStorage.UserFile);
            PropertyPagePanel.StartProgram = GetConfigProperty(JavaConfigConstants.DebugStartProgram, ProjectPropertyStorage.UserFile);
            PropertyPagePanel.StartBrowserUrl = GetConfigProperty(JavaConfigConstants.DebugStartBrowserUrl, ProjectPropertyStorage.UserFile);

            PropertyPagePanel.ExtraArguments = GetConfigProperty(JavaConfigConstants.DebugExtraArgs, ProjectPropertyStorage.UserFile);
            PropertyPagePanel.WorkingDirectory = GetConfigProperty(JavaConfigConstants.DebugWorkingDirectory, ProjectPropertyStorage.UserFile);
            PropertyPagePanel.UseRemoteMachine = GetConfigPropertyBoolean(JavaConfigConstants.DebugUseRemoteMachine, ProjectPropertyStorage.UserFile);
            PropertyPagePanel.RemoteMachineName = GetConfigProperty(JavaConfigConstants.DebugRemoteMachineName, ProjectPropertyStorage.UserFile);

            PropertyPagePanel.VirtualMachineArguments = GetConfigProperty(JavaConfigConstants.DebugJvmArguments, ProjectPropertyStorage.UserFile);
        }

        protected override bool ApplyChanges()
        {
            SetConfigProperty(JavaConfigConstants.DebugStartAction, PropertyPagePanel.StartAction.ToString(), ProjectPropertyStorage.UserFile);
            SetConfigProperty(JavaConfigConstants.DebugStartClass, PropertyPagePanel.StartClass, ProjectPropertyStorage.UserFile);
            SetConfigProperty(JavaConfigConstants.DebugStartProgram, PropertyPagePanel.StartProgram, ProjectPropertyStorage.UserFile);
            SetConfigProperty(JavaConfigConstants.DebugStartBrowserUrl, PropertyPagePanel.StartBrowserUrl, ProjectPropertyStorage.UserFile);

            SetConfigProperty(JavaConfigConstants.DebugExtraArgs, PropertyPagePanel.ExtraArguments, ProjectPropertyStorage.UserFile);
            SetConfigProperty(JavaConfigConstants.DebugWorkingDirectory, PropertyPagePanel.WorkingDirectory, ProjectPropertyStorage.UserFile);
            SetConfigProperty(JavaConfigConstants.DebugUseRemoteMachine, PropertyPagePanel.UseRemoteMachine, ProjectPropertyStorage.UserFile);
            SetConfigProperty(JavaConfigConstants.DebugRemoteMachineName, PropertyPagePanel.RemoteMachineName, ProjectPropertyStorage.UserFile);

            SetConfigProperty(JavaConfigConstants.DebugJvmArguments, PropertyPagePanel.VirtualMachineArguments, ProjectPropertyStorage.UserFile);

            return true;
        }
    }
}
