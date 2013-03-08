namespace Tvl.VisualStudio.Language.Php.Projection
{
    using Antlr4.Runtime;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Language.Parsing4;

    internal class ContentTypeTagger : AntlrTaggerBase<PhpContentTypeLexerState, ContentTypeTag>
    {
        private readonly IContentType _phpContentType;

        public ContentTypeTagger(ITextBuffer textBuffer, IContentTypeRegistryService contentTypeRegistryService)
            : base(textBuffer)
        {
            _phpContentType = contentTypeRegistryService.GetContentType(PhpConstants.PhpContentType);
        }

        protected override PhpContentTypeLexerState GetStartState()
        {
            return PhpContentTypeLexerState.Initial;
        }

        protected override ITokenSourceWithState<PhpContentTypeLexerState> CreateLexer(ICharStream input, PhpContentTypeLexerState startState)
        {
            PhpContentTypeLexer lexer = new PhpContentTypeLexer(input);
            lexer.TokenFactory = new SnapshotTokenFactory(lexer);
            startState.Apply(lexer);
            return lexer;
        }

        protected override bool TryTagToken(IToken token, out ContentTypeTag tag)
        {
            switch (token.Type)
            {
            case PhpContentTypeLexer.HTML_START_CODE:
                tag = new ContentTypeTag(_phpContentType, RegionType.Begin);
                return true;

            case PhpContentTypeLexer.CLOSE_PHP_TAG:
                tag = new ContentTypeTag(_phpContentType, RegionType.End);
                return true;

            default:
                tag = null;
                return false;
            }
        }
    }
}
