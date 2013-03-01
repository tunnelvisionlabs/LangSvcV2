namespace Tvl.VisualStudio.Language.AntlrV4
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

    [Name("Antlr4CompletionSourceProvider")]
    [ContentType(Antlr4Constants.AntlrContentType)]
    [Order(Before = "default")]
    [Export(typeof(ICompletionSourceProvider))]
    internal class Antlr4CompletionSourceProvider : CompletionSourceProvider
    {
        private readonly BitmapImage _lexerRuleGlyph;
        private readonly BitmapImage _parserRuleGlyph;

        public Antlr4CompletionSourceProvider()
        {
            string assemblyName = typeof(Antlr4CompletionSourceProvider).Assembly.GetName().Name;
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
            return new Antlr4CompletionSource(textBuffer, this, (Antlr4BackgroundParser)BackgroundParserFactoryService.GetBackgroundParser(textBuffer));
        }
    }
}
