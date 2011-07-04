namespace Tvl.VisualStudio.Language.Php
{
    using System.Collections.Generic;
    using Antlr.Runtime;
    using Microsoft.VisualStudio.Language.StandardClassification;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Tvl.VisualStudio.Language.Parsing;

    internal sealed class PhpClassifier : AntlrClassifierBase<PhpClassifierLexerState>
    {
        private static readonly HashSet<string> Keywords =
            new HashSet<string>()
            {
                "and",
                "or",
                "xor",
                "__FILE__",
                "exception",
                "__LINE__",
                "as",
                "break",
                "case",
                "class",
                "const",
                "continue",
                "declare",
                "default",
                "do",
                "else",
                "elseif",
                "enddeclare",
                "endfor",
                "endforeach",
                "endif",
                "endswitch",
                "endwhile",
                "extends",
                "for",
                "foreach",
                "function",
                "global",
                "if",
                "new",
                "return",
                "static",
                "switch",
                "use",
                "var",
                "while",
                "__FUNCTION__",
                "__CLASS__",
                "__METHOD__",
                "final",
                "php_user_filter",
                "interface",
                "implements",
                "instanceof",
                "public",
                "private",
                "protected",
                "abstract",
                "clone",
                "try",
                "catch",
                "throw",
                "this",
                "final",
                "__NAMESPACE__",
                "namespace",
                "__DIR__",
            };

        private static readonly HashSet<string> BuiltinFunctions =
            new HashSet<string>()
            {
                "array",
                "die",
                "echo",
                "empty",
                "eval",
                "exit",
                "include",
                "include_once",
                "isset",
                "list",
                "print",
                "require",
                "require_once",
                "unset",
            };

        private static readonly HashSet<string> BuiltinObjects =
            new HashSet<string>()
            {
                "$GLOBALS",
                "$_SERVER",
                "$_GET",
                "$_POST",
                "$_FILES",
                "$_REQUEST",
                "$_SESSION",
                "$_ENV",
                "$_COOKIE",
                "$php_errormsg",
                "$HTTP_RAW_POST_DATA",
                "$http_response_header",
                "$argc",
                "$argv",
            };

        private readonly ITextBuffer _textBuffer;
        private readonly IStandardClassificationService _standardClassificationService;
        private readonly IClassificationTypeRegistryService _classificationTypeRegistryService;

        private readonly IClassificationType _globalFunction;
        private readonly IClassificationType _globalObject;

        private readonly IClassificationType _docCommentText;
        private readonly IClassificationType _docCommentTag;
        private readonly IClassificationType _docCommentInvalidTag;

        public PhpClassifier(ITextBuffer textBuffer, IStandardClassificationService standardClassificationService, IClassificationTypeRegistryService classificationTypeRegistryService)
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
        }

        protected override PhpClassifierLexerState GetStartState()
        {
            return PhpClassifierLexerState.Initial;
        }

        protected override ITokenSourceWithState<PhpClassifierLexerState> CreateLexer(ICharStream input, PhpClassifierLexerState state)
        {
            return new PhpClassifierLexer(input, state);
        }

        protected override IClassificationType ClassifyToken(IToken token)
        {
            switch (token.Type)
            {
            case PhpHtmlTagClassifierLexer.SINGLE_QUOTE_STRING:
            case PhpHtmlTagClassifierLexer.DOUBLE_QUOTE_STRING:
            case PhpCodeClassifierLexer.PHP_SINGLE_STRING_LITERAL:
            case PhpCodeClassifierLexer.PHP_DOUBLE_STRING_LITERAL:
            case PhpCodeClassifierLexer.PHP_HEREDOC_TEXT:
                return _standardClassificationService.StringLiteral;

            case PhpCodeClassifierLexer.PHP_NUMBER:
                return _standardClassificationService.NumberLiteral;

            case PhpCodeClassifierLexer.PHP_IDENTIFIER:
                if (Keywords.Contains(token.Text))
                    return _standardClassificationService.Keyword;
                else if (BuiltinFunctions.Contains(token.Text))
                    return _globalFunction;
                else if (BuiltinObjects.Contains(token.Text))
                    return _globalObject;
                else if (token.Text[0] == '$')
                    return _standardClassificationService.SymbolDefinition;

                return _standardClassificationService.Identifier;

            case PhpHtmlTextClassifierLexer.HTML_START_CODE:
            case PhpCodeClassifierLexer.CLOSE_PHP_TAG:
                return _standardClassificationService.Keyword;

            case PhpHtmlTextClassifierLexer.HTML_COMMENT:
            case PhpCodeClassifierLexer.PHP_COMMENT:
            case PhpCodeClassifierLexer.PHP_ML_COMMENT:
                return _standardClassificationService.Comment;

            case PhpDocCommentClassifierLexer.DOC_COMMENT_TEXT:
                return _docCommentText;

            case PhpDocCommentClassifierLexer.DOC_COMMENT_TAG:
                return _docCommentTag;

            case PhpDocCommentClassifierLexer.DOC_COMMENT_INVALID_TAG:
                return _docCommentInvalidTag;

            case PhpHtmlTextClassifierLexer.HTML_CDATA:
                return _standardClassificationService.ExcludedCode;

            default:
                return null;
            }
        }
    }
}
