namespace Tvl.VisualStudio.InheritanceMargin
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;

    [Guid(InheritanceMarginConstants.guidInheritanceMarginPackageString)]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    public class InheritanceMarginPackage : Package
    {
        private static InheritanceMarginPackage _instance;

        public InheritanceMarginPackage()
        {
            _instance = this;
        }

        public static InheritanceMarginPackage Instance
        {
            get
            {
                return _instance;
            }
        }

        protected override void Initialize()
        {
            base.Initialize();
        }
    }
}
