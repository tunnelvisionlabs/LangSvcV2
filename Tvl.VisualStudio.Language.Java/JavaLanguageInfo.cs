namespace Tvl.VisualStudio.Language.Java
{
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Shell.Extensions;
    using Tvl.VisualStudio.Text;

    using IVsEditorAdaptersFactoryService = Microsoft.VisualStudio.Editor.IVsEditorAdaptersFactoryService;
    using IVsTextBuffer = Microsoft.VisualStudio.TextManager.Interop.IVsTextBuffer;
    using SVsServiceProvider = Microsoft.VisualStudio.Shell.SVsServiceProvider;
    using TextSpan = Microsoft.VisualStudio.TextManager.Interop.TextSpan;
    using VSConstants = Microsoft.VisualStudio.VSConstants;

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
            var componentModel = ServiceProvider.GetComponentModel();
            var adapterFactoryService = componentModel.DefaultExportProvider.GetExport<IVsEditorAdaptersFactoryService>();
            ITextBuffer textBuffer = adapterFactoryService.Value.GetDataBuffer(buffer);

            ITextSnapshot snapshot = textBuffer.CurrentSnapshot;
            ITextSnapshotLine snapshotLine = snapshot.GetLineFromLineNumber(line);
            string lineText = snapshotLine.GetText();

            // allow any single line to have a breakpoint (no parser-based location validation)
            pCodeSpan[0].iStartLine = line;
            pCodeSpan[0].iStartIndex = snapshotLine.Length - lineText.TrimStart().Length;
            pCodeSpan[0].iEndLine = line;
            pCodeSpan[0].iEndIndex = snapshotLine.Length;
            return VSConstants.S_OK;
        }
    }
}
