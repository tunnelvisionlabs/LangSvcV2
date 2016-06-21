namespace Tvl.VisualStudio.Language.Markdown
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows.Controls;
    using Tvl.VisualStudio.Shell;

    [Guid("DB319129-4783-4BC4-920A-F3E99BAA97A8")]
    internal class MarkdownPreviewToolWindowPane : WpfToolWindowPane
    {
        public MarkdownPreviewToolWindowPane()
        {
            this.Caption = "Markdown Preview";
        }

        protected override Control CreateToolWindowControl()
        {
            return new MarkdownPreviewControl(this, GlobalServiceProvider);
        }
    }
}
