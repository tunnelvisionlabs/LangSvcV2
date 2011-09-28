namespace Tvl.VisualStudio.Language.Antlr3
{
    using System.Collections.Generic;
    using Antlr.Runtime;
    using Microsoft.VisualStudio.Language.StandardClassification;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Tvl.VisualStudio.Language.Parsing;

    internal class AntlrClassifier : AntlrClassifierBase<AntlrClassifierLexerState>
    {
        private static readonly HashSet<string> keywords =
            new HashSet<string>
            {
                "lexer",
                "parser",
                "catch",
                "finally",
                "grammar",
                "private",
                "protected",
                "public",
                "returns",
                "throws",
                "tree",
                "scope",
                "import",
                "fragment",
                "tokens",
                "options",
            };

        private readonly IStandardClassificationService _standardClassificationService;
        private readonly IClassificationTypeRegistryService _classificationTypeRegistryService;

        private readonly IClassificationType _parserRule;
        private readonly IClassificationType _lexerRule;

        private readonly IClassificationType _validOption;
        private readonly IClassificationType _invalidOption;

        private readonly IClassificationType _astOperator;

        private readonly IClassificationType _actionLiteral;
        private readonly IClassificationType _actionComment;
        private readonly IClassificationType _actionStringLiteral;
        private readonly IClassificationType _actionSymbolReference;

        public AntlrClassifier(ITextBuffer textBuffer, IStandardClassificationService standardClassificationService, IClassificationTypeRegistryService classificationTypeRegistryService)
            : base(textBuffer)
        {
            this._standardClassificationService = standardClassificationService;
            this._classificationTypeRegistryService = classificationTypeRegistryService;

            this._parserRule = classificationTypeRegistryService.GetClassificationType(AntlrClassificationTypeNames.ParserRule);
            this._lexerRule = classificationTypeRegistryService.GetClassificationType(AntlrClassificationTypeNames.LexerRule);

            this._validOption = classificationTypeRegistryService.GetClassificationType(AntlrClassificationTypeNames.ValidOption);
            this._invalidOption = classificationTypeRegistryService.GetClassificationType(AntlrClassificationTypeNames.InvalidOption);

            this._astOperator = classificationTypeRegistryService.GetClassificationType(AntlrClassificationTypeNames.AstOperator);

            this._actionLiteral = classificationTypeRegistryService.GetClassificationType(AntlrClassificationTypeNames.ActionLiteral);
            this._actionComment = classificationTypeRegistryService.GetClassificationType(AntlrClassificationTypeNames.ActionComment);
            this._actionStringLiteral = classificationTypeRegistryService.GetClassificationType(AntlrClassificationTypeNames.ActionStringLiteral);
            this._actionSymbolReference = classificationTypeRegistryService.GetClassificationType(AntlrClassificationTypeNames.ActionSymbolReference);
        }

        public static HashSet<string> Keywords
        {
            get
            {
                return keywords;
            }
        }

        protected override AntlrClassifierLexerState GetStartState()
        {
            return AntlrClassifierLexerState.Initial;
        }

        protected override ITokenSourceWithState<AntlrClassifierLexerState> CreateLexer(ICharStream input, AntlrClassifierLexerState state)
        {
            return new AntlrClassifierLexer(input, state);
        }

        protected override IClassificationType ClassifyToken(IToken token)
        {
            switch (token.Type)
            {
            case AntlrGrammarClassifierLexer.IDENTIFIER:
                string text = token.Text;
                if (keywords.Contains(text))
                    return _standardClassificationService.Keyword;

                if (char.IsLower(text, 0))
                    return this._parserRule;
                else
                    return this._lexerRule;

            case AntlrGrammarClassifierLexer.ValidGrammarOption:
                return _validOption;

            case AntlrGrammarClassifierLexer.InvalidGrammarOption:
                return _invalidOption;

            case AntlrGrammarClassifierLexer.OptionValue:
                return _standardClassificationService.Identifier;

            case AntlrGrammarClassifierLexer.LABEL:
                return _standardClassificationService.SymbolDefinition;

            case AntlrGrammarClassifierLexer.COMMENT:
            case AntlrGrammarClassifierLexer.ML_COMMENT:
                return _standardClassificationService.Comment;

            case AntlrGrammarClassifierLexer.CHAR_LITERAL:
            case AntlrGrammarClassifierLexer.STRING_LITERAL:
            case AntlrGrammarClassifierLexer.DOUBLE_ANGLE_STRING_LITERAL:
                return _standardClassificationService.StringLiteral;

            case AntlrActionClassifierLexer.ACTION_COMMENT:
            case AntlrActionClassifierLexer.ACTION_ML_COMMENT:
                return _actionComment;

            case AntlrGrammarClassifierLexer.ACTION:
            case AntlrGrammarClassifierLexer.ARG_ACTION:
            case AntlrActionClassifierLexer.ACTION_TEXT:
            case AntlrGrammarClassifierLexer.LBRACK:
            case AntlrGrammarClassifierLexer.RBRACK:
                return _actionLiteral;

            case AntlrActionClassifierLexer.ACTION_CHAR_LITERAL:
            case AntlrActionClassifierLexer.ACTION_STRING_LITERAL:
                return _actionStringLiteral;

            case AntlrActionClassifierLexer.ACTION_REFERENCE:
                return _actionSymbolReference;

            case AntlrGrammarClassifierLexer.DIRECTIVE:
                return _standardClassificationService.PreprocessorKeyword;

            case AntlrGrammarClassifierLexer.REFERENCE:
                return _standardClassificationService.SymbolReference;

            case AntlrGrammarClassifierLexer.AMPERSAND:
            //case AntlrGrammarClassifierLexer.COMMA:
            case AntlrGrammarClassifierLexer.QUESTION:
            //case AntlrGrammarClassifierLexer.COLON:
            //case AntlrGrammarClassifierLexer.STAR:
            case AntlrGrammarClassifierLexer.PLUS:
            case AntlrGrammarClassifierLexer.ASSIGN:
            case AntlrGrammarClassifierLexer.PLUS_ASSIGN:
            case AntlrGrammarClassifierLexer.IMPLIES:
            //case AntlrGrammarClassifierLexer.SEMI:
            //case AntlrGrammarClassifierLexer.OR:
            //case AntlrGrammarClassifierLexer.WILDCARD:
            case AntlrGrammarClassifierLexer.ETC:
            case AntlrGrammarClassifierLexer.RANGE:
            case AntlrGrammarClassifierLexer.NOT:
            case AntlrGrammarClassifierLexer.DOLLAR:
                //return _standardClassificationService.Operator;
                goto default;

            case AntlrGrammarClassifierLexer.BANG:
            case AntlrGrammarClassifierLexer.REWRITE:
            case AntlrGrammarClassifierLexer.ROOT:
                return _astOperator;

            case AntlrGrammarClassifierLexer.WS:
                return null;
                //return _standardClassificationService.WhiteSpace;

            case AntlrGrammarClassifierLexer.INT:
                return _standardClassificationService.NumberLiteral;

            default:
                return base.ClassifyToken(token);
            }
        }
    }
}
