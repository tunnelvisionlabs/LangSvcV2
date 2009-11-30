namespace JavaLanguageService
{
    using System.Collections.Generic;
    using Antlr.Runtime;
    using Microsoft.VisualStudio.Language.StandardClassification;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using System;
    using System.Reflection;
    using Tvl.VisualStudio.Language.Parsing;

    internal sealed class JavaClassifier : AntlrClassifierBase
    {
        private static readonly HashSet<string> Keywords =
            new HashSet<string>()
            {
                "abstract",
                "assert",
                "boolean",
                "break",
                "byte",
                "case",
                "catch",
                "char",
                "class",
                "const",
                "continue",
                "default",
                "do",
                "double",
                "else",
                "enum",
                "extends",
                "final",
                "finally",
                "float",
                "for",
                "if",
                "goto",
                "implements",
                "import",
                "instanceof",
                "int",
                "interface",
                "long",
                "native",
                "new",
                "package",
                "private",
                "protected",
                "public",
                "return",
                "short",
                "static",
                "strictfp",
                "super",
                "switch",
                "synchronized",
                "this",
                "throw",
                "throws",
                "transient",
                "try",
                "void",
                "volatile",
                "while",
                "true",
                "false",
                "null"
            };

        private readonly ITextBuffer _textBuffer;
        private readonly IStandardClassificationService _standardClassificationService;

        public JavaClassifier(ITextBuffer textBuffer, IStandardClassificationService standardClassificationService)
        {
            this._textBuffer = textBuffer;
            this._standardClassificationService = standardClassificationService;
        }

        protected override Lexer CreateLexer(ICharStream input)
        {
            return new JavaColorizerLexer(input);
        }

        protected override IClassificationType ClassifyToken(IToken token)
        {
            switch (token.Type)
            {
            case JavaColorizerLexer.EQ:
            case JavaColorizerLexer.NEQ:
            case JavaColorizerLexer.EQEQ:
            case JavaColorizerLexer.PLUS:
            case JavaColorizerLexer.PLUSEQ:
            case JavaColorizerLexer.MINUS:
            case JavaColorizerLexer.MINUSEQ:
            case JavaColorizerLexer.TIMES:
            case JavaColorizerLexer.TIMESEQ:
            case JavaColorizerLexer.DIV:
            case JavaColorizerLexer.DIVEQ:
            case JavaColorizerLexer.LT:
            case JavaColorizerLexer.GT:
            case JavaColorizerLexer.LE:
            case JavaColorizerLexer.GE:
            case JavaColorizerLexer.NOT:
            case JavaColorizerLexer.BITNOT:
            case JavaColorizerLexer.AND:
            case JavaColorizerLexer.BITAND:
            case JavaColorizerLexer.ANDEQ:
            case JavaColorizerLexer.QUES:
            case JavaColorizerLexer.OR:
            case JavaColorizerLexer.BITOR:
            case JavaColorizerLexer.OREQ:
            case JavaColorizerLexer.COLON:
            case JavaColorizerLexer.INC:
            case JavaColorizerLexer.DEC:
            case JavaColorizerLexer.XOR:
            case JavaColorizerLexer.XOREQ:
            case JavaColorizerLexer.MOD:
            case JavaColorizerLexer.MODEQ:
            case JavaColorizerLexer.LSHIFT:
            case JavaColorizerLexer.RSHIFT:
            case JavaColorizerLexer.LSHIFTEQ:
            case JavaColorizerLexer.RSHIFTEQ:
            case JavaColorizerLexer.ROR:
            case JavaColorizerLexer.ROREQ:
                return _standardClassificationService.Operator;

            case JavaColorizerLexer.CHAR_LITERAL:
                return _standardClassificationService.CharacterLiteral;

            case JavaColorizerLexer.STRING_LITERAL:
                return _standardClassificationService.StringLiteral;

            case JavaColorizerLexer.NUMBER:
                return _standardClassificationService.NumberLiteral;

            case JavaColorizerLexer.IDENTIFIER:
                if (Keywords.Contains(token.Text))
                    return _standardClassificationService.Keyword;

                return _standardClassificationService.Identifier;

            case JavaColorizerLexer.COMMENT:
            case JavaColorizerLexer.ML_COMMENT:
                return _standardClassificationService.Comment;

            default:
                return null;
            }
        }
    }
}
