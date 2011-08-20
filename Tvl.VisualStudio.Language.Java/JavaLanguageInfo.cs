namespace Tvl.VisualStudio.Language.Java
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Tvl.VisualStudio.Text;

    [Guid(Constants.JavaLanguageGuidString)]
    internal class JavaLanguageInfo : LanguageInfo
    {
        public JavaLanguageInfo(SVsServiceProvider serviceProvider)
            : base(serviceProvider, typeof(JavaLanguageInfo).GUID)
        {
        }

        public override string LanguageName
        {
            get
            {
                return Constants.JavaLanguageName;
            }
        }

        public override IEnumerable<string> FileExtensions
        {
            get
            {
                yield return Constants.JavaFileExtension;
            }
        }

        public override int ValidateBreakpointLocation(IVsTextBuffer buffer, int line, int col, TextSpan[] pCodeSpan)
        {
            // allow any single line to have a breakpoint (no parser-based location validation)
            pCodeSpan[0].iStartLine = line;
            pCodeSpan[0].iStartIndex = 0;
            pCodeSpan[0].iEndLine = line;
            ErrorHandler.ThrowOnFailure(buffer.GetLengthOfLine(line, out pCodeSpan[0].iEndIndex));

            return VSConstants.S_OK;
        }
    }
}
