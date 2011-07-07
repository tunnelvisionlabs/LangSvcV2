namespace Tvl.VisualStudio.Language.Alloy
{
    using System.Collections.Generic;
    using Antlr.Runtime;
    using Microsoft.VisualStudio.Language.StandardClassification;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Tvl.VisualStudio.Language.Parsing;

    internal sealed class AlloyClassifier : AntlrClassifierBase<AlloyClassifierLexerState>
    {
        internal static readonly HashSet<string> Keywords =
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
            : base(textBuffer)
        {
            this._textBuffer = textBuffer;
            this._standardClassificationService = standardClassificationService;
            this._classificationTypeRegistryService = classificationTypeRegistryService;
        }

        protected override AlloyClassifierLexerState GetStartState()
        {
            return AlloyClassifierLexerState.Initial;
        }

        protected override ITokenSourceWithState<AlloyClassifierLexerState> CreateLexer(ICharStream input, AlloyClassifierLexerState state)
        {
            return new AlloyClassifierLexer(input, state);
        }

        protected override IClassificationType ClassifyToken(IToken token)
        {
            switch (token.Type)
            {
            case AlloyColorizerLexer.LPAREN:
            case AlloyColorizerLexer.RPAREN:
            case AlloyColorizerLexer.LBRACE:
            case AlloyColorizerLexer.RBRACE:
            case AlloyColorizerLexer.LBRACK:
            case AlloyColorizerLexer.RBRACK:
                return _standardClassificationService.Operator;

            case AlloyColorizerLexer.LT:
            case AlloyColorizerLexer.GT:
            case AlloyColorizerLexer.LE:
            case AlloyColorizerLexer.GE:
            case AlloyColorizerLexer.COLON:
            case AlloyColorizerLexer.COMMA:
            case AlloyColorizerLexer.NOT:
            case AlloyColorizerLexer.COUNT:
            case AlloyColorizerLexer.AND:
            case AlloyColorizerLexer.BITAND:
            case AlloyColorizerLexer.STAR:
            case AlloyColorizerLexer.PLUS:
            case AlloyColorizerLexer.MINUS:
            case AlloyColorizerLexer.OVERRIDE:
            case AlloyColorizerLexer.ARROW:
            case AlloyColorizerLexer.DOT:
            case AlloyColorizerLexer.LSHIFT:
            case AlloyColorizerLexer.RSHIFT:
            case AlloyColorizerLexer.RROTATE:
            case AlloyColorizerLexer.IFF:
            case AlloyColorizerLexer.DOMAIN_RES:
            case AlloyColorizerLexer.RANGE_RES:
            case AlloyColorizerLexer.EQ:
            case AlloyColorizerLexer.IMPLIES:
            case AlloyColorizerLexer.AT:
            case AlloyColorizerLexer.CARET:
            case AlloyColorizerLexer.BAR:
            case AlloyColorizerLexer.OR:
            case AlloyColorizerLexer.TILDE:
                return _standardClassificationService.Operator;

            case AlloyColorizerLexer.INTEGER:
                return _standardClassificationService.NumberLiteral;

            case AlloyColorizerLexer.SLASH:
            case AlloyColorizerLexer.IDENTIFIER:
                if (Keywords.Contains(token.Text))
                    return _standardClassificationService.Keyword;

                return _standardClassificationService.Identifier;

            case AlloyColorizerLexer.COMMENT:
            case AlloyColorizerLexer.ML_COMMENT:
                return _standardClassificationService.Comment;

            case AlloyColorizerLexer.WS:
                return _standardClassificationService.WhiteSpace;

            default:
                return null;
            }
        }
    }
}
