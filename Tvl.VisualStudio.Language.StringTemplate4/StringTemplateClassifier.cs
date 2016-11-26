namespace Tvl.VisualStudio.Language.StringTemplate4
{
    using Antlr4.Runtime;
    using Microsoft.VisualStudio.Language.StandardClassification;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Tvl.VisualStudio.Language.Parsing4;

    internal sealed class StringTemplateClassifier : AntlrClassifierBase<ClassifierLexerState>
    {
        private readonly IStandardClassificationService _standardClassificationService;
        private readonly IClassificationTypeRegistryService _classificationTypeRegistryService;

        private readonly IClassificationType _anonymousTemplateDelimiterClassificationType;
        private readonly IClassificationType _bigStringDelimiterClassificationType;
        private readonly IClassificationType _expressionDelimiterClassificationType;
        private readonly IClassificationType _escapeCharacterClassificationType;
        private readonly IClassificationType _escapeTagClassificationType;

        public StringTemplateClassifier(ITextBuffer textBuffer, IStandardClassificationService standardClassificationService, IClassificationTypeRegistryService classificationTypeRegistryService)
            : base(textBuffer)
        {
            this._standardClassificationService = standardClassificationService;
            this._classificationTypeRegistryService = classificationTypeRegistryService;

            this._anonymousTemplateDelimiterClassificationType = classificationTypeRegistryService.GetClassificationType(StringTemplateClassificationTypeNames.AnonymousTemplateDelimiter);
            this._bigStringDelimiterClassificationType = classificationTypeRegistryService.GetClassificationType(StringTemplateClassificationTypeNames.BigStringDelimiter);
            this._expressionDelimiterClassificationType = classificationTypeRegistryService.GetClassificationType(StringTemplateClassificationTypeNames.ExpressionDelimiter);
            this._escapeCharacterClassificationType = classificationTypeRegistryService.GetClassificationType(StringTemplateClassificationTypeNames.EscapeCharacter);
            this._escapeTagClassificationType = classificationTypeRegistryService.GetClassificationType(StringTemplateClassificationTypeNames.EscapeTag);
        }

        protected override ITokenSourceWithState<ClassifierLexerState> CreateLexer(ICharStream input, int startLine, ClassifierLexerState startState)
        {
            var lexer = new ClassifierLexer(input);
            lexer.Line = startLine;
            lexer.Column = 0;
            startState.Apply(lexer);
            return lexer;
        }

        protected override ClassifierLexerState GetStartState()
        {
            return ClassifierLexerState.Initial;
        }

        protected override IClassificationType ClassifyToken(IToken token)
        {
            switch (token.Type)
            {
            case ClassifierLexer.DEFAULT:
            case ClassifierLexer.IMPORT:
            case ClassifierLexer.GROUP:
            case ClassifierLexer.TRUE:
            case ClassifierLexer.FALSE:
            case ClassifierLexer.DELIMITERS:
            case ClassifierLexer.IF:
            case ClassifierLexer.ELSEIF:
            case ClassifierLexer.ELSE:
            case ClassifierLexer.ENDIF:
            case ClassifierLexer.END:

            case ClassifierLexer.SUPER:
            case ClassifierLexer.FIRST:
            case ClassifierLexer.LAST:
            case ClassifierLexer.REST:
            case ClassifierLexer.TRUNC:
            case ClassifierLexer.STRIP:
            case ClassifierLexer.TRIM:
            case ClassifierLexer.LENGTH:
            case ClassifierLexer.STRLEN:
            case ClassifierLexer.REVERSE:
                return _standardClassificationService.Keyword;

            case ClassifierLexer.ID:
                return _standardClassificationService.Identifier;

            case ClassifierLexer.LBRACE:
            case ClassifierLexer.RBRACE:
                return _anonymousTemplateDelimiterClassificationType;

            case ClassifierLexer.BIGSTRING:
            case ClassifierLexer.BIGSTRINGLINE:
            case ClassifierLexer.BigStringTemplate_END:
            case ClassifierLexer.BigStringLineTemplate_END:
                return _bigStringDelimiterClassificationType;

            case ClassifierLexer.STRING:
                return _standardClassificationService.StringLiteral;

            case ClassifierLexer.LINE_COMMENT:
            case ClassifierLexer.COMMENT:
                return _standardClassificationService.Comment;

            //case GroupClassifierLexer4.WS:
            //    return whitespaceAttributes;

            case ClassifierLexer.OPEN_DELIMITER:
            case ClassifierLexer.CLOSE_DELIMITER:
                return _expressionDelimiterClassificationType;

            case ClassifierLexer.ESCAPE:
                return _escapeTagClassificationType;

            case ClassifierLexer.AnonymousTemplate_ID:
            case ClassifierLexer.AnonymousTemplate_COMMA:
            case ClassifierLexer.TEXT:
                return _standardClassificationService.StringLiteral;

            case ClassifierLexer.REGION_ID:
                //return _regionUseClassificationType;
                return _standardClassificationService.Identifier;

            default:
                return null;
            }
        }
    }
}
