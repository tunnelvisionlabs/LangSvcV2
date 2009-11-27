namespace JavaLanguageService.AntlrLanguage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Antlr.Runtime;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Language.StandardClassification;
    using Microsoft.VisualStudio.Text.Classification;

    internal class AntlrClassifier : AntlrClassifierBase
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

        private readonly ITextBuffer _textBuffer;
        private readonly IStandardClassificationService _standardClassificationService;
        private readonly IClassificationTypeRegistryService _classificationTypeRegistryService;

        private readonly IClassificationType _parserRule;
        private readonly IClassificationType _lexerRule;
        private readonly IClassificationType _actionLiteral;

        private AntlrColorableLexer _lexer;

        public AntlrClassifier(ITextBuffer textBuffer, IStandardClassificationService standardClassificationService, IClassificationTypeRegistryService classificationTypeRegistryService)
        {
            this._textBuffer = textBuffer;
            this._standardClassificationService = standardClassificationService;
            this._classificationTypeRegistryService = classificationTypeRegistryService;

            this._parserRule = classificationTypeRegistryService.GetClassificationType(AntlrClassificationTypeNames.ParserRule);
            this._lexerRule = classificationTypeRegistryService.GetClassificationType(AntlrClassificationTypeNames.LexerRule);
            this._actionLiteral = classificationTypeRegistryService.GetClassificationType(AntlrClassificationTypeNames.ActionLiteral);
        }

        protected override Lexer CreateLexer(ICharStream input)
        {
            if (_lexer != null)
                _lexer.CharStream = input;
            else
                _lexer = new AntlrColorableLexer(input);

            return _lexer;
        }

        protected override IClassificationType ClassifyToken(IToken token)
        {
            switch (token.Type)
            {
            case AntlrColorableLexer.IDENTIFIER:
                string text = token.Text;
                if (keywords.Contains(text))
                    return _standardClassificationService.Keyword;

                if (char.IsLower(text, 0))
                    return this._parserRule;
                else
                    return this._lexerRule;

            case AntlrColorableLexer.COMMENT:
            case AntlrColorableLexer.ML_COMMENT:
                return _standardClassificationService.Comment;

            case AntlrColorableLexer.ACTION:
            case AntlrColorableLexer.ARG_ACTION:
                return this._actionLiteral;

            case AntlrColorableLexer.CHAR_LITERAL:
                return _standardClassificationService.CharacterLiteral;

            case AntlrColorableLexer.STRING_LITERAL:
                return _standardClassificationService.StringLiteral;

            case AntlrColorableLexer.DOUBLE_ANGLE_STRING_LITERAL:
                return _standardClassificationService.StringLiteral;

            case AntlrColorableLexer.DIRECTIVE:
                return _standardClassificationService.PreprocessorKeyword;

            case AntlrColorableLexer.REFERENCE:
                return _standardClassificationService.SymbolReference;

            case AntlrColorableLexer.AMPERSAND:
            //case AntlrColorableLexer.COMMA:
            case AntlrColorableLexer.QUESTION:
            //case AntlrColorableLexer.COLON:
            //case AntlrColorableLexer.STAR:
            case AntlrColorableLexer.PLUS:
            case AntlrColorableLexer.ASSIGN:
            case AntlrColorableLexer.PLUS_ASSIGN:
            case AntlrColorableLexer.IMPLIES:
            case AntlrColorableLexer.REWRITE:
            //case AntlrColorableLexer.SEMI:
            case AntlrColorableLexer.ROOT:
            case AntlrColorableLexer.BANG:
            //case AntlrColorableLexer.OR:
            //case AntlrColorableLexer.WILDCARD:
            case AntlrColorableLexer.ETC:
            case AntlrColorableLexer.RANGE:
            case AntlrColorableLexer.NOT:
            case AntlrColorableLexer.DOLLAR:
                return _standardClassificationService.Operator;

            default:
                return base.ClassifyToken(token);
            }
        }
    }
}
