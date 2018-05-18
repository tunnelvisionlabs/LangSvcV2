namespace Tvl.VisualStudio.Language.AntlrV4
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Text.Navigation;

    [Export(typeof(IQuickInfoSourceProvider))]
    [Order]
    [ContentType(Antlr4Constants.AntlrContentType)]
    [Name("Antlr4QuickInfoSource")]
    public sealed class Antlr4QuickInfoSourceProvider : IQuickInfoSourceProvider
    {
        [Import]
        internal IEditorNavigationSourceAggregatorFactoryService EditorNavigationSourceAggregatorFactoryService
        {
            get;
            private set;
        }

        [Import]
        internal IBufferTagAggregatorFactoryService AggregatorFactory
        {
            get;
            private set;
        }

        public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer)
        {
            Func<Antlr4QuickInfoSource> factory =
                () =>
                {
                    return new Antlr4QuickInfoSource(
                        textBuffer,
                        EditorNavigationSourceAggregatorFactoryService.CreateEditorNavigationSourceAggregator(textBuffer),
                        AggregatorFactory.CreateTagAggregator<ClassificationTag>(textBuffer));
                };

            return textBuffer.Properties.GetOrCreateSingletonProperty(factory);
        }
    }
}
