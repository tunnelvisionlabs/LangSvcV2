namespace Tvl.VisualStudio.Language.Java.Project.PropertyPages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Runtime.InteropServices;
    using Tvl.Collections;

    [ComVisible(true)]
    [Guid(JavaProjectConstants.JavaApplicationPropertyPageGuidString)]
    public class JavaApplicationPropertyPage : JavaPropertyPage
    {
        private static readonly string HotspotTargetVirtualMachine = "Hotspot";
        private static readonly string JRockitTargetVirtualMachine = "JRockit";
        private static readonly string JavaArchiveOutputType = "jar";
        private static readonly string NotSetStartupObject = string.Empty;

        private static readonly ImmutableList<string> _defaultAvailableTargetVirtualMachines =
            new ImmutableList<string>(new string[]
            {
                HotspotTargetVirtualMachine,
                JRockitTargetVirtualMachine,
            });
        private static readonly ImmutableList<string> _defaultAvailableOutputTypes =
            new ImmutableList<string>(new string[]
            {
                JavaArchiveOutputType,
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
            PropertyPagePanel.PackageName = GetConfigProperty(JavaConfigConstants.PackageName);

            // available items
            PropertyPagePanel.AvailableTargetVirtualMachines = _defaultAvailableTargetVirtualMachines;
            PropertyPagePanel.AvailableOutputTypes = _defaultAvailableOutputTypes;
            PropertyPagePanel.AvailableStartupObjects = _defaultAvailableStartupObjects;

            // selected items
            PropertyPagePanel.TargetVirtualMachine = HotspotTargetVirtualMachine;
            PropertyPagePanel.OutputType = JavaArchiveOutputType;
            PropertyPagePanel.StartupObject = NotSetStartupObject;
        }

        protected override bool ApplyChanges()
        {
            SetConfigProperty(JavaConfigConstants.PackageName, PropertyPagePanel.PackageName);
            SetConfigProperty(JavaConfigConstants.TargetVM, PropertyPagePanel.TargetVirtualMachine);
            SetConfigProperty(JavaConfigConstants.OutputType, PropertyPagePanel.OutputType);
            SetConfigProperty(JavaConfigConstants.StartupObject, PropertyPagePanel.StartupObject);
            return true;
        }
    }
}
