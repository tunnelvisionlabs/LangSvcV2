namespace Tvl.VisualStudio.Language.Java.Project.PropertyPages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Runtime.InteropServices;
    using Tvl.Collections;
    using Microsoft.VisualStudio.Project;

    [ComVisible(true)]
    [Guid(JavaProjectConstants.JavaApplicationPropertyPageGuidString)]
    public class JavaApplicationPropertyPage : JavaPropertyPage
    {
        private static readonly string NotSetStartupObject = string.Empty;

        private static readonly ImmutableList<string> _defaultAvailableTargetVirtualMachines =
            new ImmutableList<string>(new string[]
            {
                JavaProjectFileConstants.HotspotTargetVirtualMachine,
                JavaProjectFileConstants.JRockitTargetVirtualMachine,
            });
        private static readonly ImmutableList<string> _defaultAvailableOutputTypes =
            new ImmutableList<string>(new string[]
            {
                JavaProjectFileConstants.JavaArchiveOutputType,
            });
        private static readonly ImmutableList<string> _defaultAvailableStartupObjects =
            new ImmutableList<string>(new string[]
            {
                NotSetStartupObject,
            });

        public JavaApplicationPropertyPage()
        {
            PageName = JavaConfigConstants.PageNameApplication;
        }

        public new JavaApplicationPropertyPagePanel PropertyPagePanel
        {
            get
            {
                return (JavaApplicationPropertyPagePanel)base.PropertyPagePanel;
            }
        }

        protected override JavaPropertyPagePanel CreatePropertyPagePanel()
        {
            return new JavaApplicationPropertyPagePanel(this);
        }

        protected override void BindProperties()
        {
            // package name
            PropertyPagePanel.PackageName = GetConfigProperty(ProjectFileConstants.AssemblyName);

            // available items
            PropertyPagePanel.AvailableTargetVirtualMachines = _defaultAvailableTargetVirtualMachines;
            PropertyPagePanel.AvailableOutputTypes = _defaultAvailableOutputTypes;
            PropertyPagePanel.AvailableStartupObjects = _defaultAvailableStartupObjects;

            // selected items
            PropertyPagePanel.TargetVirtualMachine = GetConfigProperty(JavaConfigConstants.TargetVM);
            PropertyPagePanel.OutputType = GetConfigProperty(JavaConfigConstants.OutputType);
            PropertyPagePanel.StartupObject = GetConfigProperty(JavaConfigConstants.StartupObject);
        }

        protected override bool ApplyChanges()
        {
            SetConfigProperty(ProjectFileConstants.AssemblyName, PropertyPagePanel.PackageName);
            SetConfigProperty(JavaConfigConstants.TargetVM, PropertyPagePanel.TargetVirtualMachine);
            SetConfigProperty(JavaConfigConstants.OutputType, PropertyPagePanel.OutputType);
            SetConfigProperty(JavaConfigConstants.StartupObject, PropertyPagePanel.StartupObject);
            return true;
        }
    }
}
