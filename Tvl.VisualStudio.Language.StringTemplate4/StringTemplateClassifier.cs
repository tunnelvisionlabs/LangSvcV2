namespace Tvl.VisualStudio.Language.StringTemplate4
{
    using System;
    using System.Collections.Generic;
    using Antlr.Runtime;
    using Microsoft.VisualStudio.Language.StandardClassification;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Tvl.VisualStudio.Language.Parsing;

    internal sealed class StringTemplateClassifier : AntlrClassifierBase<ClassifierLexer.LexerState>
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
            : base(textBuffer)
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

        protected override ICharStream CreateInputStream(SnapshotSpan span)
        {
            ICharStream input = new StringTemplateEscapedCharStream(span.Snapshot);
            input.Seek(span.Start.Position);
            return input;
        }

        protected override ITokenSourceWithState<ClassifierLexer.LexerState> CreateLexer(ICharStream input, ClassifierLexer.LexerState state)
        {
            return new ClassifierLexer(input, state);
        }

        protected override ClassifierLexer.LexerState GetStartState()
        {
            return ClassifierLexer.LexerState.Initial;
        }

        //protected override ICharStream CreateInputStream(SnapshotSpan span)
        //{
        //    ClassifierLexer.Stream stream = new ClassifierLexer.Stream(span);
        //    return stream;
        //}

        protected override IClassificationType ClassifyToken(IToken token)
        {
            switch (token.Type)
            {
            case GroupClassifierLexer.ID:
                if (Array.IndexOf(_keywords, token.Text) >= 0)
                    return _standardClassificationService.Keyword;

                return _standardClassificationService.Identifier;

            case GroupClassifierLexer.PARAMETER_DEFINITION:
                return _standardClassificationService.Identifier;

            case InsideClassifierLexer.EXPR_IDENTIFIER:
                if (Array.IndexOf(_expressionKeywords, token.Text) >= 0)
                    return _standardClassificationService.Keyword;

                return _standardClassificationService.Identifier;

            case GroupClassifierLexer.BEGIN_BIGSTRING:
            case GroupClassifierLexer.END_BIGSTRING:
            case GroupClassifierLexer.BEGIN_BIGSTRINGLINE:
            case GroupClassifierLexer.END_BIGSTRINGLINE:
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

            case InsideClassifierLexer.REGION_REF:
                return _standardClassificationService.SymbolReference;

            case OutsideClassifierLexer.ESCAPE_CHAR:
                return _escapeCharacterClassificationType;

            case OutsideClassifierLexer.ESCAPE_TAG:
                return _escapeTagClassificationType;

            default:
                return null;
            }
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

        internal class StringTemplateEscapedCharStream : SnapshotCharStream
        {
            public StringTemplateEscapedCharStream(ITextSnapshot snapshot)
                : base(snapshot)
            {
            }

            public ClassifierLexer Lexer
            {
                get;
                set;
            }

            public override void Consume()
            {
                bool consumeEscape = ShouldConsumeEscape(0);

                if (consumeEscape)
                    base.Consume();

                base.Consume();
            }

            public override int LA(int i)
            {
                int escapeCount = 0;
                if (i >= 1)
                {
                    for (int j = 0; j < i; j++)
                    {
                        if (ShouldConsumeEscape(j + escapeCount))
                            escapeCount++;
                    }
                }

                return base.LA(i + escapeCount);
            }

            private bool ShouldConsumeEscape(int offset)
            {
                if (Lexer == null)
                    return false;

                if (base.LA(offset + 1) != '\\')
                    return false;

                bool inString = Lexer.Outermost == ClassifierLexer.OutermostTemplate.String;
                if (inString)
                    return base.LA(offset + 2) == '"';

                bool inBigString = Lexer.Outermost == ClassifierLexer.OutermostTemplate.BigString
                    || Lexer.Outermost == ClassifierLexer.OutermostTemplate.BigStringLine;
                if (inBigString)
                    return base.LA(offset + 2) == '>';

                return false;
            }
        }
    }
}
