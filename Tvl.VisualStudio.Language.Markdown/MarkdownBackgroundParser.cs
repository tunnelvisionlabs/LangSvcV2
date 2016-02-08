namespace Tvl.VisualStudio.Language.Markdown
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Shell.OutputWindow;

    public class MarkdownBackgroundParser : BackgroundParser
    {
        private MarkdownSharp.Markdown _markdownTransform = new MarkdownSharp.Markdown();

        public MarkdownBackgroundParser(ITextBuffer textBuffer, TaskScheduler taskScheduler, ITextDocumentFactoryService textDocumentFactoryService, IOutputWindowService outputWindowService)
            : base(textBuffer, taskScheduler, textDocumentFactoryService, outputWindowService)
        {
            Contract.Requires(textBuffer != null);
            Contract.Requires(taskScheduler != null);
            Contract.Requires(textDocumentFactoryService != null);
            Contract.Requires(outputWindowService != null);
        }

        protected override void ReParseImpl()
        {
            var outputWindow = OutputWindowService.TryGetPane(PredefinedOutputWindowPanes.TvlIntellisense);
            try
            {
                var snapshot = TextBuffer.CurrentSnapshot;
                string content = GetHTMLText(snapshot.GetText(), true);
                OnParseComplete(new MarkdownParseResultEventArgs(snapshot, content));
            }
            catch (Exception e)
            {
                if (ErrorHandler.IsCriticalException(e))
                    throw;

                try
                {
                    if (outputWindow != null)
                        outputWindow.WriteLine(e.Message);
                }
                catch (Exception e2)
                {
                    if (ErrorHandler.IsCriticalException(e2))
                        throw;
                }
            }
        }

        private string GetHTMLText(string text, bool extraSpace)
        {
            StringBuilder html = new StringBuilder(_markdownTransform.Transform(text));
            if (extraSpace)
            {
                for (int i = 0; i < 20; i++)
                    html.Append("<br />");
            }
            return html.ToString();
        }
    }
}
