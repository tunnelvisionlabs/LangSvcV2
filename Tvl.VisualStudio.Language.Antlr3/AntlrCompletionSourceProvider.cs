namespace Tvl.VisualStudio.Language.Antlr3
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Operations;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Language.Intellisense;
    using Tvl.VisualStudio.Language.Parsing;
    using BitmapImage = System.Windows.Media.Imaging.BitmapImage;
    using System;
    using ImageSource = System.Windows.Media.ImageSource;

    [Name("AntlrCompletionSourceProvider")]
    [ContentType(AntlrConstants.AntlrContentType)]
    [Order(Before = "default")]
    [Export(typeof(ICompletionSourceProvider))]
    internal class AntlrCompletionSourceProvider : CompletionSourceProvider
    {
        private readonly BitmapImage _lexerRuleGlyph;
        private readonly BitmapImage _parserRuleGlyph;

        public AntlrCompletionSourceProvider()
        {
            string assemblyName = typeof(AntlrCompletionSourceProvider).Assembly.GetName().Name;
            this._lexerRuleGlyph = new BitmapImage(new Uri(string.Format("pack://application:,,,/{0};component/Resources/lexericon.png", assemblyName)));
            this._parserRuleGlyph = new BitmapImage(new Uri(string.Format("pack://application:,,,/{0};component/Resources/parsericon.png", assemblyName)));
        }

        [Import]
        public ITextStructureNavigatorSelectorService TextStructureNavigatorSelectorService
        {
            get;
            private set;
        }

        public ImageSource LexerRuleGlyph
        {
            get
            {
                return _lexerRuleGlyph;
            }
        }

        public ImageSource ParserRuleGlyph
        {
            get
            {
                return _parserRuleGlyph;
            }
        }

        [Import]
        private IBackgroundParserFactoryService BackgroundParserFactoryService
        {
            get;
            set;
        }

        public override ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
        {
            return new AntlrCompletionSource(textBuffer, this, (AntlrBackgroundParser)BackgroundParserFactoryService.GetBackgroundParser(textBuffer));
        }
    }
}
