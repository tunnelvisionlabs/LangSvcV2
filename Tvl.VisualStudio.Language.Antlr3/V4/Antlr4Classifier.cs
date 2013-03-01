namespace Tvl.VisualStudio.Language.AntlrV4
{
    using System;
    using System.Collections.Generic;
    using Antlr4.Runtime;
    using Microsoft.VisualStudio.Language.StandardClassification;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Tvl.VisualStudio.Language.Parsing4;
    using AntlrClassificationTypeNames = Tvl.VisualStudio.Language.Antlr3.AntlrClassificationTypeNames;

    internal class Antlr4Classifier : AntlrClassifierBase<SimpleLexerState>
    {
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

        public Antlr4Classifier(ITextBuffer textBuffer, IStandardClassificationService standardClassificationService, IClassificationTypeRegistryService classificationTypeRegistryService)
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

        protected override SimpleLexerState GetStartState()
        {
            return SimpleLexerState.Initial;
        }

        protected override ITokenSourceWithState<SimpleLexerState> CreateLexer(ICharStream input, SimpleLexerState state)
        {
            var lexer = new GrammarHighlighterLexer(input);
            state.Apply(lexer);
            return lexer;
        }

        protected override IClassificationType ClassifyToken(IToken token)
        {
            switch (token.Type)
            {
            case GrammarHighlighterLexer.LEXER:
            case GrammarHighlighterLexer.PARSER:
            case GrammarHighlighterLexer.CATCH:
            case GrammarHighlighterLexer.FINALLY:
            case GrammarHighlighterLexer.GRAMMAR:
            case GrammarHighlighterLexer.PRIVATE:
            case GrammarHighlighterLexer.PROTECTED:
            case GrammarHighlighterLexer.PUBLIC:
            case GrammarHighlighterLexer.RETURNS:
            case GrammarHighlighterLexer.THROWS:
            case GrammarHighlighterLexer.IMPORT:
            case GrammarHighlighterLexer.FRAGMENT:
            case GrammarHighlighterLexer.TOKENS:
            case GrammarHighlighterLexer.OPTIONS:
            case GrammarHighlighterLexer.MODE:
            case GrammarHighlighterLexer.LOCALS:
                return _standardClassificationService.Keyword;

            case GrammarHighlighterLexer.IDENTIFIER:
                if (char.IsUpper(token.Text, 0))
                    return _lexerRule;
                else
                    return _parserRule;

            case GrammarHighlighterLexer.ValidGrammarOption:
                return _validOption;

            case GrammarHighlighterLexer.InvalidGrammarOption:
                return _invalidOption;

            case GrammarHighlighterLexer.LABEL:
                return _standardClassificationService.SymbolDefinition;

            case GrammarHighlighterLexer.COMMENT:
            case GrammarHighlighterLexer.ML_COMMENT:
                return _standardClassificationService.Comment;

            case GrammarHighlighterLexer.CHAR_LITERAL:
            case GrammarHighlighterLexer.STRING_LITERAL:
                return _standardClassificationService.StringLiteral;

            case GrammarHighlighterLexer.Action_COMMENT:
            case GrammarHighlighterLexer.Action_ML_COMMENT:
                return _actionComment;

            case GrammarHighlighterLexer.ArgAction_TEXT:
            case GrammarHighlighterLexer.ArgAction_ESCAPE:
            case GrammarHighlighterLexer.Action_TEXT:
            case GrammarHighlighterLexer.Action_ESCAPE:
                return _actionLiteral;

            case GrammarHighlighterLexer.ArgAction_REFERENCE:
                return _actionSymbolReference;

            case GrammarHighlighterLexer.ArgAction_CHAR_LITERAL:
            case GrammarHighlighterLexer.ArgAction_STRING_LITERAL:
            case GrammarHighlighterLexer.Action_CHAR_LITERAL:
            case GrammarHighlighterLexer.Action_STRING_LITERAL:
                return _actionStringLiteral;

            case GrammarHighlighterLexer.LexerCharSet_ESCAPE:
            case GrammarHighlighterLexer.LexerCharSet_INVALID_ESCAPE:
            case GrammarHighlighterLexer.LexerCharSet_TEXT:
                return _standardClassificationService.StringLiteral;

            case GrammarHighlighterLexer.REWRITE:
                return _astOperator;

            case GrammarHighlighterLexer.WS:
                return _standardClassificationService.WhiteSpace;

            case GrammarHighlighterLexer.INT:
                return _standardClassificationService.NumberLiteral;

            default:
                return base.ClassifyToken(token);
            }
        }
    }
}
