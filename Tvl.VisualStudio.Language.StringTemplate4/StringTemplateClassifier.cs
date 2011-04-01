#define NEW_CLASSIFIER

namespace Tvl.VisualStudio.Language.StringTemplate4
{
    using System.Collections.Generic;
    using Antlr.Runtime;
    using Microsoft.VisualStudio.Language.StandardClassification;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Tvl.VisualStudio.Language.Parsing;
    using System;
    using System.Linq;

    internal sealed class StringTemplateClassifier : AntlrClassifierBase
    {
        private static readonly string[] _keywords = { "group", "default", "import", "true", "false" };
        private static readonly string[] _expressionKeywords = { "if", "elseif", "endif", "else", "end" };

        private readonly ITextBuffer _textBuffer;
        private readonly IStandardClassificationService _standardClassificationService;
        private readonly IClassificationTypeRegistryService _classificationTypeRegistryService;

        private readonly IClassificationType _anonymousTemplateDelimiterClassificationType;
        private readonly IClassificationType _bigStringDelimiterClassificationType;
        private readonly IClassificationType _expressionDelimiterClassificationType;
        private readonly IClassificationType _escapeCharacterClassificationType;
        private readonly IClassificationType _escapeTagClassificationType;

        public StringTemplateClassifier(ITextBuffer textBuffer, IStandardClassificationService standardClassificationService, IClassificationTypeRegistryService classificationTypeRegistryService)
        {
            this._textBuffer = textBuffer;
            this._standardClassificationService = standardClassificationService;
            this._classificationTypeRegistryService = classificationTypeRegistryService;

            this._anonymousTemplateDelimiterClassificationType = classificationTypeRegistryService.GetClassificationType(StringTemplateClassificationTypeNames.AnonymousTemplateDelimiter);
            this._bigStringDelimiterClassificationType = classificationTypeRegistryService.GetClassificationType(StringTemplateClassificationTypeNames.BigStringDelimiter);
            this._expressionDelimiterClassificationType = classificationTypeRegistryService.GetClassificationType(StringTemplateClassificationTypeNames.ExpressionDelimiter);
            this._escapeCharacterClassificationType = classificationTypeRegistryService.GetClassificationType(StringTemplateClassificationTypeNames.EscapeCharacter);
            this._escapeTagClassificationType = classificationTypeRegistryService.GetClassificationType(StringTemplateClassificationTypeNames.EscapeTag);
        }

        protected override ITokenSource CreateLexer(ICharStream input)
        {
#if NEW_CLASSIFIER
            return new ClassifierLexer((ClassifierLexer.Stream)input);
#else
            return new StringTemplateColorizerLexer(input);
#endif
        }

#if NEW_CLASSIFIER
        protected override ICharStream CreateInputStream(SnapshotSpan span)
        {
            ClassifierLexer.Stream stream = new ClassifierLexer.Stream(span);
            return stream;
        }
#endif

        protected override IClassificationType ClassifyToken(IToken token)
        {
#if NEW_CLASSIFIER
            switch (token.Type)
            {
            case GroupClassifierLexer.ID:
                if (Array.IndexOf(_keywords, token.Text) >= 0)
                    return _standardClassificationService.Keyword;

                return _standardClassificationService.Identifier;

            case InsideClassifierLexer.EXPR_IDENTIFIER:
                if (Array.IndexOf(_expressionKeywords, token.Text) >= 0)
                    return _standardClassificationService.Keyword;

                return _standardClassificationService.Identifier;

            case GroupClassifierLexer.BEGIN_BIGSTRING:
            case GroupClassifierLexer.END_BIGSTRING:
                return _bigStringDelimiterClassificationType;

            case OutsideClassifierLexer.TEXT:
            case InsideClassifierLexer.STRING:
            case OutsideClassifierLexer.QUOTE:
                return _standardClassificationService.StringLiteral;

            case GroupClassifierLexer.COMMENT:
            case GroupClassifierLexer.LINE_COMMENT:
                return _standardClassificationService.Comment;

            case GroupClassifierLexer.WS:
                return _standardClassificationService.WhiteSpace;

            case OutsideClassifierLexer.LANGLE:
            case InsideClassifierLexer.RANGLE:
                return _expressionDelimiterClassificationType;

            case GroupClassifierLexer.LBRACE:
            case GroupClassifierLexer.RBRACE:
                return _anonymousTemplateDelimiterClassificationType;

            case GroupClassifierLexer.DEFINED:
            case GroupClassifierLexer.EQUALS:
            case InsideClassifierLexer.ELLIPSIS:
            case GroupClassifierLexer.AT:
                return _standardClassificationService.Operator;

            case OutsideClassifierLexer.ESCAPE_CHAR:
                return _escapeCharacterClassificationType;

            case OutsideClassifierLexer.ESCAPE_TAG:
                return _escapeTagClassificationType;

            default:
                return null;
            }
#else
            switch (token.Type)
            {
            case StringTemplateColorizerLexer.ID:
                if (Array.IndexOf(_keywords, token.Text) >= 0)
                    return _standardClassificationService.Keyword;

                return _standardClassificationService.Identifier;

            case StringTemplateColorizerLexer.STRING:
            case StringTemplateColorizerLexer.BIGSTRING:
            case StringTemplateColorizerLexer.ANONYMOUS_TEMPLATE:
                return _standardClassificationService.StringLiteral;

            case StringTemplateColorizerLexer.LINE_COMMENT:
            case StringTemplateColorizerLexer.COMMENT:
                return _standardClassificationService.Comment;

            case StringTemplateColorizerLexer.WS:
                return _standardClassificationService.WhiteSpace;

            default:
                return null;
            }
#endif
        }

        protected override IEnumerable<ClassificationSpan> GetClassificationSpansForToken(IToken token, ITextSnapshot snapshot)
        {
            List<ClassificationSpan> spans = new List<ClassificationSpan>();
            spans.AddRange(base.GetClassificationSpansForToken(token, snapshot));
            if (spans.Count > 0)
            {
                var classification = spans[0];
                if (classification.ClassificationType.Classification == StringTemplateClassificationTypeNames.TemplateLiteral0)
                {
                }
            }

            return spans;
        }
    }
}
