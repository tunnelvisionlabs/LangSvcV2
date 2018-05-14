namespace Tvl.VisualStudio.Language.Antlr3
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using JetBrains.Annotations;
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
    [ContentType(AntlrConstants.AntlrContentType)]
    [Order]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    [Name("AntlrIntellisenseController")]
    internal class AntlrIntellisenseControllerProvider : IntellisenseControllerProvider
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

        protected override IntellisenseController TryCreateIntellisenseController([NotNull] ITextView textView, [NotNull] IList<ITextBuffer> subjectBuffers)
        {
            Requires.NotNull(textView, nameof(textView));
            Requires.NotNull(subjectBuffers, nameof(subjectBuffers));

            AntlrIntellisenseController controller = new AntlrIntellisenseController(textView, this, (AntlrBackgroundParser)BackgroundParserFactoryService.GetBackgroundParser(textView.TextBuffer));
            textView.Properties[typeof(AntlrIntellisenseController)] = controller;
            return controller;
        }
    }
}
