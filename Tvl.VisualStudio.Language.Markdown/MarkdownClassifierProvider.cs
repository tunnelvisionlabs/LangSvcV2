namespace Tvl.VisualStudio.Language.Markdown
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(IClassifierProvider))]
    [ContentType(MarkdownConstants.MarkdownContentType)]
    internal class MarkdownClassifierProvider : IClassifierProvider
    {
        [Import]
        private IClassificationTypeRegistryService ClassificationRegistry
        {
            get;
            set;
        }

        public IClassifier GetClassifier(ITextBuffer textBuffer)
        {
            Func<MarkdownClassifier> creator = () => new MarkdownClassifier(textBuffer, ClassificationRegistry);
            return textBuffer.Properties.GetOrCreateSingletonProperty(creator);
        }
    }
}
