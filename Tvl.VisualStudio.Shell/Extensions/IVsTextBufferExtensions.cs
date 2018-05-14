namespace Tvl.VisualStudio.Shell
{
    using System;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio;
    using IVsTextBuffer = Microsoft.VisualStudio.TextManager.Interop.IVsTextBuffer;

    public static class IVsTextBufferExtensions
    {
        public static Guid? GetLanguageServiceID([NotNull] this IVsTextBuffer textBuffer)
        {
            Requires.NotNull(textBuffer, nameof(textBuffer));

            Guid id;
            int hr = textBuffer.GetLanguageServiceID(out id);
            if (hr != VSConstants.S_OK)
                return null;

            return id;
        }
    }
}
