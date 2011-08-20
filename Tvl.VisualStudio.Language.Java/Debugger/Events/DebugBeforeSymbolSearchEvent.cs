namespace Tvl.VisualStudio.Language.Java.Debugger.Events
{
    using System;
    using System.Diagnostics.Contracts;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Debugger.Interop;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public class DebugBeforeSymbolSearchEvent : DebugEvent, IDebugBeforeSymbolSearchEvent2
    {
        private readonly string _moduleName;

        public DebugBeforeSymbolSearchEvent(enum_EVENTATTRIBUTES attributes, string moduleName)
            : base(attributes)
        {
            Contract.Requires<ArgumentNullException>(moduleName != null, "moduleName");
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(moduleName));

            _moduleName = moduleName;
        }

        public int GetModuleName(out string pbstrModuleName)
        {
            pbstrModuleName = _moduleName;
            return VSConstants.S_OK;
        }
    }
}
