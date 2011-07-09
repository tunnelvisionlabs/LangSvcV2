namespace Tvl.VisualStudio.Language.Php.Outlining
{
    using Antlr.Runtime;
    using Tvl.VisualStudio.Language.Php.Classification;

    internal class PhpOutliningLexer : PhpClassifierLexer
    {
        public PhpOutliningLexer(ICharStream input)
            : base(input)
        {
        }

        public override IToken NextToken()
        {
            IToken token = base.NextToken();

            switch (token.Type)
            {
            case PhpCodeClassifierLexer.WS:
            case PhpCodeClassifierLexer.PHP_COMMENT:
            case PhpCodeClassifierLexer.PHP_ML_COMMENT:
            case PhpDocCommentClassifierLexer.EMPTY_BLOCK_COMMENT:
            case PhpDocCommentClassifierLexer.DOC_COMMENT_INVALID_TAG:
            case PhpDocCommentClassifierLexer.DOC_COMMENT_START:
            case PhpDocCommentClassifierLexer.DOC_COMMENT_TAG:
            case PhpDocCommentClassifierLexer.DOC_COMMENT_TEXT:
            case PhpDocCommentClassifierLexer.NEWLINE:
                token.Channel = TokenChannels.Hidden;
                break;

            case PhpCodeClassifierLexer.PHP_IDENTIFIER:
                int keywordType;
                if (PhpClassifier.KeywordTokenTypes.TryGetValue(token.Text, out keywordType))
                    token.Type = keywordType;

                break;

            default:
                break;
            }

            return token;
        }
    }
}
