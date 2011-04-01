namespace Tvl.VisualStudio.Language.Go
{
    using System.Collections.Generic;
    using Antlr.Runtime;
    using Microsoft.VisualStudio.Language.StandardClassification;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Tvl.VisualStudio.Language.Parsing;

    internal sealed class GoClassifier : AntlrClassifierBase
    {
        private static readonly HashSet<string> Keywords =
            new HashSet<string>()
            {
                "break",
                "case",
                "chan",
                "const",
                "continue",
                "default",
                "defer",
                "else",
                "fallthrough",
                "for",
                "func",
                "go",
                "goto",
                "if",
                "import",
                "interface",
                "map",
                "package",
                "range",
                "return",
                "select",
                "struct",
                "switch",
                "type",
                "var",
            };

        private readonly ITextBuffer _textBuffer;
        private readonly IStandardClassificationService _standardClassificationService;
        private readonly IClassificationTypeRegistryService _classificationTypeRegistryService;

        public GoClassifier(ITextBuffer textBuffer, IStandardClassificationService standardClassificationService, IClassificationTypeRegistryService classificationTypeRegistryService)
        {
            this._textBuffer = textBuffer;
            this._standardClassificationService = standardClassificationService;
            this._classificationTypeRegistryService = classificationTypeRegistryService;
        }

        protected override ITokenSource CreateLexer(ICharStream input)
        {
            return new GoColorizerLexer(input);
        }

        protected override IClassificationType ClassifyToken(IToken token)
        {
            switch (token.Type)
            {
            case GoColorizerLexer.EQ:
            case GoColorizerLexer.NEQ:
            case GoColorizerLexer.EQEQ:
            case GoColorizerLexer.PLUS:
            case GoColorizerLexer.PLUSEQ:
            case GoColorizerLexer.MINUS:
            case GoColorizerLexer.MINUSEQ:
            case GoColorizerLexer.TIMES:
            case GoColorizerLexer.TIMESEQ:
            case GoColorizerLexer.DIV:
            case GoColorizerLexer.DIVEQ:
            case GoColorizerLexer.LT:
            case GoColorizerLexer.GT:
            case GoColorizerLexer.LE:
            case GoColorizerLexer.GE:
            case GoColorizerLexer.NOT:
            case GoColorizerLexer.AND:
            case GoColorizerLexer.BITAND:
            case GoColorizerLexer.ANDEQ:
            case GoColorizerLexer.OR:
            case GoColorizerLexer.BITOR:
            case GoColorizerLexer.OREQ:
            case GoColorizerLexer.COLON:
            case GoColorizerLexer.INC:
            case GoColorizerLexer.DEC:
            case GoColorizerLexer.XOR:
            case GoColorizerLexer.XOREQ:
            case GoColorizerLexer.MOD:
            case GoColorizerLexer.MODEQ:
            case GoColorizerLexer.LSHIFT:
            case GoColorizerLexer.RSHIFT:
            case GoColorizerLexer.LSHIFTEQ:
            case GoColorizerLexer.RSHIFTEQ:
                return _standardClassificationService.Operator;

            case GoColorizerLexer.CHAR_LITERAL:
                return _standardClassificationService.CharacterLiteral;

            case GoColorizerLexer.STRING_LITERAL:
            case GoColorizerLexer.RAW_STRING_LITERAL:
                return _standardClassificationService.StringLiteral;

            case GoColorizerLexer.NUMBER:
                return _standardClassificationService.NumberLiteral;

            case GoColorizerLexer.IDENTIFIER:
                if (Keywords.Contains(token.Text))
                    return _standardClassificationService.Keyword;

                return _standardClassificationService.Identifier;

            case GoColorizerLexer.COMMENT:
            case GoColorizerLexer.ML_COMMENT:
                return _standardClassificationService.Comment;

            default:
                return null;
            }
        }
    }
}
