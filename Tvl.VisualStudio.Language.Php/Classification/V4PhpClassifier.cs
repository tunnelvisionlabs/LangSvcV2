namespace Tvl.VisualStudio.Language.Php.Classification
{
    using Antlr4.Runtime;
    using Microsoft.VisualStudio.Language.StandardClassification;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Tvl.VisualStudio.Language.Parsing4;

    internal sealed partial class V4PhpClassifier : AntlrClassifierBase<V4PhpClassifierLexerState>
    {
        private readonly ITextBuffer _textBuffer;
        private readonly IStandardClassificationService _standardClassificationService;
        private readonly IClassificationTypeRegistryService _classificationTypeRegistryService;

        private readonly IClassificationType _globalFunction;
        private readonly IClassificationType _globalObject;

        private readonly IClassificationType _docCommentText;
        private readonly IClassificationType _docCommentTag;
        private readonly IClassificationType _docCommentInvalidTag;

        private readonly IClassificationType _htmlServerSideScript;

        public V4PhpClassifier(ITextBuffer textBuffer, IStandardClassificationService standardClassificationService, IClassificationTypeRegistryService classificationTypeRegistryService)
            : base(textBuffer)
        {
            this._textBuffer = textBuffer;
            this._standardClassificationService = standardClassificationService;
            this._classificationTypeRegistryService = classificationTypeRegistryService;

            this._globalFunction = this._classificationTypeRegistryService.GetClassificationType(PhpClassificationTypeNames.GlobalFunction);
            this._globalObject = this._classificationTypeRegistryService.GetClassificationType(PhpClassificationTypeNames.GlobalObject);

            this._docCommentText = this._classificationTypeRegistryService.GetClassificationType(PhpClassificationTypeNames.DocCommentText);
            this._docCommentTag = this._classificationTypeRegistryService.GetClassificationType(PhpClassificationTypeNames.DocCommentTag);
            this._docCommentInvalidTag = this._classificationTypeRegistryService.GetClassificationType(PhpClassificationTypeNames.DocCommentInvalidTag);

            this._htmlServerSideScript = this._classificationTypeRegistryService.GetClassificationType(PhpClassificationTypeNames.HtmlServerSideScript);
        }

        protected override V4PhpClassifierLexerState GetStartState()
        {
            return V4PhpClassifierLexerState.Initial;
        }

        protected override ITokenSourceWithState<V4PhpClassifierLexerState> CreateLexer(ICharStream input, int startLine, V4PhpClassifierLexerState state)
        {
            V4PhpClassifierLexer lexer = new V4PhpClassifierLexer(input);
            lexer.Line = startLine;
            lexer.Column = 0;
            state.Apply(lexer);
            return lexer;
        }

        protected override IClassificationType ClassifyToken(IToken token)
        {
            switch (token.Type)
            {
            case V4PhpClassifierLexer.PHP_SINGLE_STRING_LITERAL:
            case V4PhpClassifierLexer.PHP_DOUBLE_STRING_LITERAL:
            case V4PhpClassifierLexer.PHP_HEREDOC_START:
            case V4PhpClassifierLexer.PHP_HEREDOC_TEXT:
            case V4PhpClassifierLexer.PHP_HEREDOC_END:
            case V4PhpClassifierLexer.PHP_NOWDOC_START:
            case V4PhpClassifierLexer.PHP_NOWDOC_TEXT:
            case V4PhpClassifierLexer.PHP_NOWDOC_END:
                return _standardClassificationService.StringLiteral;

            case V4PhpClassifierLexer.PhpHereDoc_DOUBLE_STRING_ESCAPE:
            case V4PhpClassifierLexer.DOUBLE_STRING_ESCAPE:
                return _standardClassificationService.StringLiteral;

            case V4PhpClassifierLexer.PHP_NUMBER:
                return _standardClassificationService.NumberLiteral;

            case V4PhpClassifierLexer.KW___CLASS__:
            case V4PhpClassifierLexer.KW___DIR__:
            case V4PhpClassifierLexer.KW___FILE__:
            case V4PhpClassifierLexer.KW___FUNCTION__:
            case V4PhpClassifierLexer.KW___LINE__:
            case V4PhpClassifierLexer.KW___METHOD__:
            case V4PhpClassifierLexer.KW___NAMESPACE__:
            case V4PhpClassifierLexer.KW_ABSTRACT:
            case V4PhpClassifierLexer.KW_AND:
            case V4PhpClassifierLexer.KW_AS:
            case V4PhpClassifierLexer.KW_BREAK:
            case V4PhpClassifierLexer.KW_CASE:
            case V4PhpClassifierLexer.KW_CATCH:
            case V4PhpClassifierLexer.KW_CLASS:
            case V4PhpClassifierLexer.KW_CLONE:
            case V4PhpClassifierLexer.KW_CONST:
            case V4PhpClassifierLexer.KW_CONTINUE:
            case V4PhpClassifierLexer.KW_DECLARE:
            case V4PhpClassifierLexer.KW_DEFAULT:
            case V4PhpClassifierLexer.KW_DO:
            case V4PhpClassifierLexer.KW_ELSE:
            case V4PhpClassifierLexer.KW_ELSEIF:
            case V4PhpClassifierLexer.KW_ENDDECLARE:
            case V4PhpClassifierLexer.KW_ENDFOR:
            case V4PhpClassifierLexer.KW_ENDFOREACH:
            case V4PhpClassifierLexer.KW_ENDIF:
            case V4PhpClassifierLexer.KW_ENDSWITCH:
            case V4PhpClassifierLexer.KW_ENDWHILE:
            case V4PhpClassifierLexer.KW_EXCEPTION:
            case V4PhpClassifierLexer.KW_EXTENDS:
            case V4PhpClassifierLexer.KW_FINAL:
            case V4PhpClassifierLexer.KW_FOR:
            case V4PhpClassifierLexer.KW_FOREACH:
            case V4PhpClassifierLexer.KW_FUNCTION:
            case V4PhpClassifierLexer.KW_GLOBAL:
            case V4PhpClassifierLexer.KW_IF:
            case V4PhpClassifierLexer.KW_IMPLEMENTS:
            case V4PhpClassifierLexer.KW_INSTANCEOF:
            case V4PhpClassifierLexer.KW_INTERFACE:
            case V4PhpClassifierLexer.KW_NAMESPACE:
            case V4PhpClassifierLexer.KW_NEW:
            case V4PhpClassifierLexer.KW_OR:
            case V4PhpClassifierLexer.KW_PHP_USER_FILTER:
            case V4PhpClassifierLexer.KW_PRIVATE:
            case V4PhpClassifierLexer.KW_PROTECTED:
            case V4PhpClassifierLexer.KW_PUBLIC:
            case V4PhpClassifierLexer.KW_RETURN:
            case V4PhpClassifierLexer.KW_STATIC:
            case V4PhpClassifierLexer.KW_SWITCH:
            case V4PhpClassifierLexer.KW_THIS:
            case V4PhpClassifierLexer.KW_THROW:
            case V4PhpClassifierLexer.KW_TRY:
            case V4PhpClassifierLexer.KW_USE:
            case V4PhpClassifierLexer.KW_VAR:
            case V4PhpClassifierLexer.KW_WHILE:
            case V4PhpClassifierLexer.KW_XOR:
                return _standardClassificationService.Keyword;

            case V4PhpClassifierLexer.PHP_IDENTIFIER:
                if (PhpClassifierConstants.BuiltinFunctions.Contains(token.Text))
                    return _globalFunction;
                else if (PhpClassifierConstants.BuiltinObjects.Contains(token.Text) || PhpClassifierConstants.PredefinedConstants.Contains(token.Text))
                    return _globalObject;
                else if (token.Text[0] == '$')
                    return _standardClassificationService.SymbolDefinition;

                return _standardClassificationService.Identifier;

            // HTML Server-Side Script
            case V4PhpClassifierLexer.HTML_START_CODE:
            case V4PhpClassifierLexer.CLOSE_PHP_TAG:
                return _htmlServerSideScript;

            case V4PhpClassifierLexer.PHP_COMMENT:
            case V4PhpClassifierLexer.PHP_ML_COMMENT:
                return _standardClassificationService.Comment;

            case V4PhpClassifierLexer.DOC_COMMENT_TEXT:
                return _docCommentText;

            case V4PhpClassifierLexer.DOC_COMMENT_TAG:
                return _docCommentTag;

            case V4PhpClassifierLexer.DOC_COMMENT_INVALID_TAG:
                return _docCommentInvalidTag;

            default:
                return null;
            }
        }
    }
}
