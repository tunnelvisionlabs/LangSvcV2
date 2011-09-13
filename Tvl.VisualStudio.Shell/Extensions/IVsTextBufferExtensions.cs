namespace Tvl.VisualStudio.Shell
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using IVsTextBuffer = Microsoft.VisualStudio.TextManager.Interop.IVsTextBuffer;
    using System.Diagnostics.Contracts;
    using Microsoft.VisualStudio;

    public static class IVsTextBufferExtensions
    {
        public static Guid? GetLanguageServiceID(this IVsTextBuffer textBuffer)
        {
            Contract.Requires<ArgumentNullException>(textBuffer != null, "textBuffer");

            Guid id;
            int hr = textBuffer.GetLanguageServiceID(out id);
            if (hr != VSConstants.S_OK)
                return null;

            return id;
        }
    }
}
