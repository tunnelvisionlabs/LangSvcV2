namespace Tvl.VisualStudio.Language.AntlrV4
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Operations;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Language.Intellisense;
    using Tvl.VisualStudio.Language.Parsing;
    using SVsServiceProvider = Microsoft.VisualStudio.Shell.SVsServiceProvider;

    [Export(typeof(IIntellisenseControllerProvider))]
    [ContentType(Antlr4Constants.AntlrContentType)]
    [Order]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    [Name("Antlr4IntellisenseController")]
    internal class Antlr4IntellisenseControllerProvider : IntellisenseControllerProvider
    {
        [Import]
        public IDispatcherGlyphService GlyphService
        {
            get;
            private set;
        }

        [Import]
        internal IClassifierAggregatorService ClassifierAggregatorService
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

        [Import]
        internal ITextStructureNavigatorSelectorService TextStructureNavigatorSelectorService
        {
            get;
            private set;
        }

        [Import]
        private IBackgroundParserFactoryService BackgroundParserFactoryService
        {
            get;
            set;
        }

        [Import]
        public SVsServiceProvider GlobalServiceProvider
        {
            get;
            private set;
        }

        protected override IntellisenseController TryCreateIntellisenseController(ITextView textView, IList<ITextBuffer> subjectBuffers)
        {
            Antlr4IntellisenseController controller = new Antlr4IntellisenseController(textView, this, (Antlr4BackgroundParser)BackgroundParserFactoryService.GetBackgroundParser(textView.TextBuffer));
            textView.Properties[typeof(Antlr4IntellisenseController)] = controller;
            return controller;
        }
    }
}
