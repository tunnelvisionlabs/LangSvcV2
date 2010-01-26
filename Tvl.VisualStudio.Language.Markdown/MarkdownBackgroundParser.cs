namespace Tvl.VisualStudio.Language.Markdown
{
    using System;
    using System.Collections.Generic;
    using Antlr.Runtime;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Shell.OutputWindow;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio;
    using System.Text;
    using System.Windows.Controls;

    public class MarkdownBackgroundParser : BackgroundParser
    {
        private MarkdownSharp.Markdown _markdownTransform = new MarkdownSharp.Markdown();

        public MarkdownBackgroundParser(ITextBuffer textBuffer, IOutputWindowService outputWindowService)
            : base(textBuffer)
        {
            this.OutputWindowService = outputWindowService;
        }

        public IOutputWindowService OutputWindowService
        {
            get;
            private set;
        }

        protected override void ReParseImpl()
        {
            var outputWindow = OutputWindowService.TryGetPane(PredefinedOutputWindowPanes.General);
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
