namespace Tvl.VisualStudio.Language.Alloy
{
    using System.Collections.Generic;
    using Antlr.Runtime;
    using Microsoft.VisualStudio.Language.StandardClassification;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Tvl.VisualStudio.Language.Parsing;

    internal sealed class AlloyClassifier : AntlrClassifierBase
    {
        private static readonly HashSet<string> Keywords =
            new HashSet<string>()
            {
                "let",
                "all",
                "no",
                "some",
                "one",
                "lone",
                "set",
                "seq",
                "sum",
                "else",
                "in",
                "module",
                "exactly",
                "private",
                "open",
                "as",
                "fact",
                "assert",
                "fun",
                "pred",
                "run",
                "check",
                "for",
                "but",
                "expect",
                "int",
                "sig",
                "enum",
                "abstract",
                "extends",
                "not",
                "implies",
                "none",
                "iden",
                "univ",
                "Int",
                "seq/Int",
                "disj",
                "or",
                "and",
                "iff",
                "this",
            };

        private readonly ITextBuffer _textBuffer;
        private readonly IStandardClassificationService _standardClassificationService;
        private readonly IClassificationTypeRegistryService _classificationTypeRegistryService;

        public AlloyClassifier(ITextBuffer textBuffer, IStandardClassificationService standardClassificationService, IClassificationTypeRegistryService classificationTypeRegistryService)
        {
            this._textBuffer = textBuffer;
            this._standardClassificationService = standardClassificationService;
            this._classificationTypeRegistryService = classificationTypeRegistryService;
        }

        protected override Lexer CreateLexer(ICharStream input)
        {
            return new AlloyColorizerLexer(input);
        }

        protected override IClassificationType ClassifyToken(IToken token)
        {
            switch (token.Type)
            {
            case AlloyColorizerLexer.EQ:
            case AlloyColorizerLexer.NEQ:
            //case AlloyColorizerLexer.EQEQ:
            case AlloyColorizerLexer.PLUS:
            //case AlloyColorizerLexer.PLUSEQ:
            case AlloyColorizerLexer.MINUS:
            //case AlloyColorizerLexer.MINUSEQ:
            //case AlloyColorizerLexer.TIMES:
            //case AlloyColorizerLexer.TIMESEQ:
            //case AlloyColorizerLexer.DIV:
            //case AlloyColorizerLexer.DIVEQ:
            case AlloyColorizerLexer.LT:
            case AlloyColorizerLexer.GT:
            case AlloyColorizerLexer.LE:
            case AlloyColorizerLexer.GE:
            case AlloyColorizerLexer.NOT:
            case AlloyColorizerLexer.AND:
            case AlloyColorizerLexer.BITAND:
            //case AlloyColorizerLexer.ANDEQ:
            case AlloyColorizerLexer.OR:
            //case AlloyColorizerLexer.BITOR:
            //case AlloyColorizerLexer.OREQ:
            //case AlloyColorizerLexer.COLON:
            case AlloyColorizerLexer.INC:
            //case AlloyColorizerLexer.DEC:
            //case AlloyColorizerLexer.XOR:
            //case AlloyColorizerLexer.XOREQ:
            //case AlloyColorizerLexer.MOD:
            //case AlloyColorizerLexer.MODEQ:
            case AlloyColorizerLexer.LSHIFT:
            case AlloyColorizerLexer.RSHIFT:
            //case AlloyColorizerLexer.LSHIFTEQ:
            //case AlloyColorizerLexer.RSHIFTEQ:
                return _standardClassificationService.Operator;

            //case AlloyColorizerLexer.CHAR_LITERAL:
            //    return _standardClassificationService.CharacterLiteral;

            //case AlloyColorizerLexer.STRING_LITERAL:
            //case AlloyColorizerLexer.RAW_STRING_LITERAL:
            //    return _standardClassificationService.StringLiteral;

            case AlloyColorizerLexer.INTEGER:
                return _standardClassificationService.NumberLiteral;

            case AlloyColorizerLexer.IDENTIFIER:
                if (Keywords.Contains(token.Text))
                    return _standardClassificationService.Keyword;

                return _standardClassificationService.Identifier;

            case AlloyColorizerLexer.COMMENT:
            case AlloyColorizerLexer.ML_COMMENT:
                return _standardClassificationService.Comment;

            default:
                return null;
            }
        }
    }
}
