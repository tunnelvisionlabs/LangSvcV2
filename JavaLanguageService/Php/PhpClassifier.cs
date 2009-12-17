namespace JavaLanguageService.Php
{
    using System.Collections.Generic;
    using Antlr.Runtime;
    using Microsoft.VisualStudio.Language.StandardClassification;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Tvl.VisualStudio.Language.Parsing;

    internal sealed class PhpClassifier : AntlrClassifierBase
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
                //"goto",
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

        public PhpClassifier(ITextBuffer textBuffer, IStandardClassificationService standardClassificationService)
        {
            this._textBuffer = textBuffer;
            this._standardClassificationService = standardClassificationService;
        }

        protected override Lexer CreateLexer(ICharStream input)
        {
            return new PhpColorizerLexer(input);
        }

        protected override IClassificationType ClassifyToken(IToken token)
        {
            switch (token.Type)
            {
            case PhpColorizerLexer.CHAR_LITERAL:
                return _standardClassificationService.CharacterLiteral;

            case PhpColorizerLexer.STRING_LITERAL:
                return _standardClassificationService.StringLiteral;

            case PhpColorizerLexer.NUMBER:
                return _standardClassificationService.NumberLiteral;

            case PhpColorizerLexer.OPEN_PHP_TAG:
            case PhpColorizerLexer.CLOSE_PHP_TAG:
                return _standardClassificationService.Keyword;

            case PhpColorizerLexer.IDENTIFIER:
                if (Keywords.Contains(token.Text))
                    return _standardClassificationService.Keyword;
                else if (BuiltinFunctions.Contains(token.Text))
                    return _standardClassificationService.PreprocessorKeyword;
                else if (BuiltinObjects.Contains(token.Text))
                    return _standardClassificationService.SymbolDefinition;

                return _standardClassificationService.Identifier;

            case PhpColorizerLexer.COMMENT:
            case PhpColorizerLexer.ML_COMMENT:
                return _standardClassificationService.Comment;

            default:
                return null;
            }
        }
    }
}
