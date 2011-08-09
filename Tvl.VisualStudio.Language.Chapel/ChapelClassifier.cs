namespace Tvl.VisualStudio.Language.Chapel
{
    using System.Collections.Generic;
    using Antlr.Runtime;
    using Microsoft.VisualStudio.Language.StandardClassification;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Tvl.VisualStudio.Language.Parsing;

    internal sealed class ChapelClassifier : AntlrClassifierBase<ChapelClassifierLexerState>
    {
        private static readonly HashSet<string> Keywords =
            new HashSet<string>()
            {
                "atomic",
                "begin",
                "break",
                "by",
                "class",
                "cobegin",
                "coforall",
                "config",
                "const",
                "continue",
                "delete",
                "dmapped",
                "do",
                "domain",
                "else",
                "enum",
                "false",
                "for",
                "forall",
                "if",
                "in",
                "index",
                "inout",
                "iter",
                "label",
                "let",
                "local",
                "locale",
                "module",
                "new",
                "nil",
                "on",
                "opaque",
                "otherwise",
                "out",
                "param",
                "proc",
                "range",
                "record",
                "reduce",
                "return",
                "scan",
                "select",
                "serial",
                "single",
                "sparse",
                "subdomain",
                "sync",
                "then",
                "true",
                "type",
                "union",
                "use",
                "var",
                "when",
                "where",
                "while",
                "yield",
            };

        private readonly ITextBuffer _textBuffer;
        private readonly IStandardClassificationService _standardClassificationService;
        private readonly IClassificationTypeRegistryService _classificationTypeRegistryService;

        private readonly IClassificationType _docCommentText;
        private readonly IClassificationType _docCommentTag;
        private readonly IClassificationType _docCommentInvalidTag;

        public ChapelClassifier(ITextBuffer textBuffer, IStandardClassificationService standardClassificationService, IClassificationTypeRegistryService classificationTypeRegistryService)
            : base(textBuffer)
        {
            this._textBuffer = textBuffer;
            this._standardClassificationService = standardClassificationService;
            this._classificationTypeRegistryService = classificationTypeRegistryService;

            this._docCommentText = this._classificationTypeRegistryService.GetClassificationType(ChapelClassificationTypeNames.DocCommentText);
            this._docCommentTag = this._classificationTypeRegistryService.GetClassificationType(ChapelClassificationTypeNames.DocCommentTag);
            this._docCommentInvalidTag = this._classificationTypeRegistryService.GetClassificationType(ChapelClassificationTypeNames.DocCommentInvalidTag);
        }

        protected override ChapelClassifierLexerState GetStartState()
        {
            return ChapelClassifierLexerState.Initial;
        }

        protected override ITokenSourceWithState<ChapelClassifierLexerState> CreateLexer(ICharStream input, ChapelClassifierLexerState state)
        {
            return new ChapelClassifierLexer(input, state);
        }

        protected override bool IsMultilineToken(ITextSnapshot snapshot, ITokenSource lexer, IToken token)
        {
            ChapelClassifierLexer chapelLexer = lexer as ChapelClassifierLexer;
            if (chapelLexer != null && chapelLexer.CharStream.Line >= token.Line)
                return false;

            int startLine = snapshot.GetLineNumberFromPosition(token.StartIndex);
            int stopLine = snapshot.GetLineNumberFromPosition(token.StopIndex + 1);
            return startLine != stopLine;
        }

        protected override IClassificationType ClassifyToken(IToken token)
        {
            switch (token.Type)
            {
#if false // operator coloring was just annoying
            case ChapelCodeClassifierLexer.EQ:
            case ChapelCodeClassifierLexer.NEQ:
            case ChapelCodeClassifierLexer.EQEQ:
            case ChapelCodeClassifierLexer.PLUS:
            case ChapelCodeClassifierLexer.PLUSEQ:
            case ChapelCodeClassifierLexer.MINUS:
            case ChapelCodeClassifierLexer.MINUSEQ:
            case ChapelCodeClassifierLexer.TIMES:
            case ChapelCodeClassifierLexer.TIMESEQ:
            case ChapelCodeClassifierLexer.DIV:
            case ChapelCodeClassifierLexer.DIVEQ:
            case ChapelCodeClassifierLexer.LT:
            case ChapelCodeClassifierLexer.GT:
            case ChapelCodeClassifierLexer.LE:
            case ChapelCodeClassifierLexer.GE:
            case ChapelCodeClassifierLexer.NOT:
            case ChapelCodeClassifierLexer.BITNOT:
            case ChapelCodeClassifierLexer.AND:
            case ChapelCodeClassifierLexer.BITAND:
            case ChapelCodeClassifierLexer.ANDEQ:
            case ChapelCodeClassifierLexer.QUES:
            case ChapelCodeClassifierLexer.OR:
            case ChapelCodeClassifierLexer.BITOR:
            case ChapelCodeClassifierLexer.OREQ:
            case ChapelCodeClassifierLexer.COLON:
            case ChapelCodeClassifierLexer.INC:
            case ChapelCodeClassifierLexer.DEC:
            case ChapelCodeClassifierLexer.XOR:
            case ChapelCodeClassifierLexer.XOREQ:
            case ChapelCodeClassifierLexer.MOD:
            case ChapelCodeClassifierLexer.MODEQ:
            case ChapelCodeClassifierLexer.LSHIFT:
            case ChapelCodeClassifierLexer.RSHIFT:
            case ChapelCodeClassifierLexer.LSHIFTEQ:
            case ChapelCodeClassifierLexer.RSHIFTEQ:
            case ChapelCodeClassifierLexer.ROR:
            case ChapelCodeClassifierLexer.ROREQ:
                return _standardClassificationService.Operator;
#endif

            //case ChapelCodeClassifierLexer.CHAR_LITERAL:
            //    //return _standardClassificationService.CharacterLiteral;
            //    return _standardClassificationService.StringLiteral;

            case ChapelCodeClassifierLexer.STRING:
                return _standardClassificationService.StringLiteral;

            case ChapelCodeClassifierLexer.NUMBER:
                return _standardClassificationService.NumberLiteral;

            case ChapelCodeClassifierLexer.IDENTIFIER:
                if (Keywords.Contains(token.Text))
                    return _standardClassificationService.Keyword;

                return _standardClassificationService.Identifier;

            case ChapelCodeClassifierLexer.COMMENT:
            case ChapelCodeClassifierLexer.ML_COMMENT:
                return _standardClassificationService.Comment;

            //case ChapelDocCommentClassifierLexer.DOC_COMMENT_TEXT:
            //    return _docCommentText;

            //case ChapelDocCommentClassifierLexer.DOC_COMMENT_TAG:
            //    return _docCommentTag;

            //case ChapelDocCommentClassifierLexer.DOC_COMMENT_INVALID_TAG:
            //    return _docCommentInvalidTag;

            default:
                return null;
            }
        }
    }
}
