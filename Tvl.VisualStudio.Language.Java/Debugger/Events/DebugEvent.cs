namespace Tvl.VisualStudio.Language.Java.Debugger.Events
{
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Debugger.Interop;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public abstract class DebugEvent : IDebugEvent2
    {
        private readonly enum_EVENTATTRIBUTES _attributes;

        protected DebugEvent(enum_EVENTATTRIBUTES attributes)
        {
            _attributes = attributes;
        }

        public int GetAttributes(out uint pdwAttrib)
        {
            pdwAttrib = (uint)_attributes;
            return VSConstants.S_OK;

        }
    }
}
