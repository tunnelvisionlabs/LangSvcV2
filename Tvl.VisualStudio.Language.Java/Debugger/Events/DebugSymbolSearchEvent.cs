namespace Tvl.VisualStudio.Language.Java.Debugger.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Debugger.Interop;
    using System.Diagnostics.Contracts;
    using Microsoft.VisualStudio;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public class DebugSymbolSearchEvent : DebugEvent, IDebugSymbolSearchEvent2
    {
        private readonly IDebugModule3 _module;
        private readonly string _debugMessage;
        private readonly enum_MODULE_INFO_FLAGS _moduleInfoFlags;

        public DebugSymbolSearchEvent(enum_EVENTATTRIBUTES attributes, IDebugModule3 module, string debugMessage, enum_MODULE_INFO_FLAGS moduleInfoFlags)
            : base(attributes)
        {
            Contract.Requires<ArgumentNullException>(module != null, "module");
            Contract.Requires<ArgumentNullException>(debugMessage != null, "debugMessage");
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(debugMessage));

            _module = module;
            _debugMessage = debugMessage;
            _moduleInfoFlags = moduleInfoFlags;
        }

        public int GetSymbolSearchInfo(out IDebugModule3 pModule, ref string pbstrDebugMessage, enum_MODULE_INFO_FLAGS[] pdwModuleInfoFlags)
        {
            if (pdwModuleInfoFlags == null)
                throw new ArgumentNullException("pdwModuleInfoFlags");
            if (pdwModuleInfoFlags.Length < 1)
                throw new ArgumentException();

            pModule = _module;
            pbstrDebugMessage = _debugMessage;
            pdwModuleInfoFlags[0] = _moduleInfoFlags;
            return VSConstants.S_OK;
        }
    }
}
