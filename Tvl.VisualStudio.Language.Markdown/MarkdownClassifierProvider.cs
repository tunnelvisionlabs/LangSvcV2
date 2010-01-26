namespace Tvl.VisualStudio.Language.Markdown
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Utilities;
    using Microsoft.VisualStudio.Text;

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
