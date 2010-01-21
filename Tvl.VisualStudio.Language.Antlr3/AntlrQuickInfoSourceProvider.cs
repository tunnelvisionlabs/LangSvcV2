namespace Tvl.VisualStudio.Language.Antlr3
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Utilities;
    using Microsoft.VisualStudio.Text.Tagging;

    [Export(typeof(IQuickInfoSourceProvider))]
    [Order]
    [ContentType(AntlrConstants.AntlrContentType)]
    [Name("AntlrQuickInfoSource")]
    public sealed class AntlrQuickInfoSourceProvider : IQuickInfoSourceProvider
    {
        [Import]
        internal IBufferTagAggregatorFactoryService AggregatorFactory
        {
            get;
            private set;
        }

        public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer)
        {
            return new AntlrQuickInfoSource(textBuffer, AggregatorFactory.CreateTagAggregator<ClassificationTag>(textBuffer));
        }
    }
}
