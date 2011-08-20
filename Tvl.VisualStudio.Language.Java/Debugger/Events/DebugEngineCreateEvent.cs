namespace Tvl.VisualStudio.Language.Java.Debugger.Events
{
    using System;
    using System.Diagnostics.Contracts;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Debugger.Interop;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public class DebugEngineCreateEvent : DebugEvent, IDebugEngineCreateEvent2
    {
        private readonly IDebugEngine2 _engine;

        public DebugEngineCreateEvent(enum_EVENTATTRIBUTES attributes, IDebugEngine2 engine)
            : base(attributes)
        {
            Contract.Requires<ArgumentNullException>(engine != null, "engine");
            _engine = engine;
        }

        public int GetEngine(out IDebugEngine2 pEngine)
        {
            pEngine = _engine;
            return VSConstants.S_OK;
        }
    }
}
