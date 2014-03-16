namespace Tvl.VisualStudio.InheritanceMargin
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Windows.Input;
    using Microsoft.VisualStudio.Shell;
    using CommandId = Tvl.VisualStudio.InheritanceMargin.CommandTranslation.CommandId;
    using IOleCommandTarget = Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget;
    using OLECMD = Microsoft.VisualStudio.OLE.Interop.OLECMD;
    using OleConstants = Microsoft.VisualStudio.OLE.Interop.Constants;
    using UICONTEXT = Microsoft.VisualStudio.VSConstants.UICONTEXT;

    [Guid(InheritanceMarginConstants.guidInheritanceMarginPackageString)]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [ProvideMenuResource(1000, 1)]
    public class InheritanceMarginPackage : Package, IOleCommandTarget
    {
        private static InheritanceMarginPackage _instance;

        private readonly Dictionary<CommandId, RoutedCommand> _definedCommandTable =
            new Dictionary<CommandId, RoutedCommand>(CommandId.DictionaryEqualityComparer);

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

        public static RoutedCommand InheritanceTargetsList
        {
            get;
            private set;
        }

        public SVsServiceProvider ServiceProvider
        {
            get
            {
                return new VsServiceProviderWrapper(this);
            }
        }

        public RoutedCommand FindCommand(Guid commandGroup, uint id)
        {
            return FindCommand(new CommandId(commandGroup, (int)id));
        }

        private RoutedCommand FindCommand(CommandId commandId)
        {
            RoutedCommand result;
            if (_definedCommandTable.TryGetValue(commandId, out result))
                return result;

            return null;
        }

        protected override void Initialize()
        {
            base.Initialize();
            DefineRoutableCommands();
        }

        private void DefineRoutableCommands()
        {
            DefineRoutableCommand("InheritanceTargetsList", InheritanceMarginConstants.guidInheritanceMarginCommandSet, InheritanceMarginConstants.cmdidInheritanceTargetsList, InheritanceMarginConstants.cmdidInheritanceTargetsListEnd);
        }

        private void DefineRoutableCommand(string propertyName, Guid guid, int startId, int endId)
        {
            DefineRoutableCommand(typeof(InheritanceMarginPackage), propertyName, guid, startId, endId);
        }

        private void DefineRoutableCommand(Type owningType, string propertyName, Guid guid, int startId, int endId)
        {
            PropertyInfo property = owningType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Static);
            DefineRoutableCommand(property, guid, startId, endId);
        }

        private void DefineRoutableCommand(PropertyInfo property, Guid guid, int startId, int endId)
        {
            RoutedCommand command = new RoutedCommand(property.Name, property.DeclaringType);
            property.SetValue(this, command, new object[0]);
            DefineRoutableCommand(command, guid, startId, endId);
        }

        private void DefineRoutableCommand(RoutedCommand command, Guid guid, int startId, int endId)
        {
            _definedCommandTable.Add(new CommandId(guid, startId, endId), command);
        }

        int IOleCommandTarget.Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            IOleCommandTarget service = (IOleCommandTarget)base.GetService(typeof(IOleCommandTarget));
            if (service != null)
                return service.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);

            return (int)OleConstants.MSOCMDERR_E_NOTSUPPORTED;
        }

        int IOleCommandTarget.QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            IOleCommandTarget service = (IOleCommandTarget)base.GetService(typeof(IOleCommandTarget));
            if (service != null)
                return service.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);

            return (int)OleConstants.MSOCMDERR_E_NOTSUPPORTED;
        }
    }
}
