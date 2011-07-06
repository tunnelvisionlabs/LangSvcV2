namespace Tvl.VisualStudio.Language.Markdown
{
    using System;
    using System.Text;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Shell.OutputWindow;

    public class MarkdownBackgroundParser : BackgroundParser
    {
        private MarkdownSharp.Markdown _markdownTransform = new MarkdownSharp.Markdown();

        public MarkdownBackgroundParser(ITextBuffer textBuffer, ITextDocumentFactoryService textDocumentFactoryService, IOutputWindowService outputWindowService)
            : base(textBuffer, textDocumentFactoryService, outputWindowService)
        {
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
