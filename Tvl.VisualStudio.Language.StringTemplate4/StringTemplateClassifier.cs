namespace Tvl.VisualStudio.Language.StringTemplate4
{
    using System.Collections.Generic;
    using Antlr.Runtime;
    using Microsoft.VisualStudio.Language.StandardClassification;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Tvl.VisualStudio.Language.Parsing;
    using System;

    internal sealed class StringTemplateClassifier : AntlrClassifierBase
    {
        private static readonly string[] _keywords = { "default", "import" };

        private readonly ITextBuffer _textBuffer;
        private readonly IStandardClassificationService _standardClassificationService;
        private readonly IClassificationTypeRegistryService _classificationTypeRegistryService;

        public StringTemplateClassifier(ITextBuffer textBuffer, IStandardClassificationService standardClassificationService, IClassificationTypeRegistryService classificationTypeRegistryService)
        {
            this._textBuffer = textBuffer;
            this._standardClassificationService = standardClassificationService;
            this._classificationTypeRegistryService = classificationTypeRegistryService;
        }

        protected override Lexer CreateLexer(ICharStream input)
        {
            return new StringTemplateColorizerLexer(input);
        }

        protected override IClassificationType ClassifyToken(IToken token)
        {
            switch (token.Type)
            {
            //case StringTemplateColorizerLexer.ST:
            //    return _standardClassificationService.Operator;

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
        }
    }
}
