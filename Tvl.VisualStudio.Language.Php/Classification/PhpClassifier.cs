namespace Tvl.VisualStudio.Language.Php.Classification
{
    using System.Collections.Generic;
    using Antlr.Runtime;
    using Microsoft.VisualStudio.Language.StandardClassification;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Tvl.VisualStudio.Language.Parsing;

    using StringComparer = System.StringComparer;

    internal sealed partial class PhpClassifier : AntlrClassifierBase<PhpClassifierLexerState>
    {
        private readonly ITextBuffer _textBuffer;
        private readonly IStandardClassificationService _standardClassificationService;
        private readonly IClassificationTypeRegistryService _classificationTypeRegistryService;

        private readonly IClassificationType _globalFunction;
        private readonly IClassificationType _globalObject;

        private readonly IClassificationType _docCommentText;
        private readonly IClassificationType _docCommentTag;
        private readonly IClassificationType _docCommentInvalidTag;

        private readonly IClassificationType _htmlAttributeName;
        private readonly IClassificationType _htmlAttributeValue;
        private readonly IClassificationType _htmlComment;
        private readonly IClassificationType _htmlElementName;
        private readonly IClassificationType _htmlEntity;
        private readonly IClassificationType _htmlOperator;
        private readonly IClassificationType _htmlServerSideScript;
        private readonly IClassificationType _htmlTagDelimiter;

        public PhpClassifier(ITextBuffer textBuffer, IStandardClassificationService standardClassificationService, IClassificationTypeRegistryService classificationTypeRegistryService)
            : base(textBuffer)
        {
            this._textBuffer = textBuffer;
            this._standardClassificationService = standardClassificationService;
            this._classificationTypeRegistryService = classificationTypeRegistryService;

            this._globalFunction = this._classificationTypeRegistryService.GetClassificationType(PhpClassificationTypeNames.GlobalFunction);
            this._globalObject = this._classificationTypeRegistryService.GetClassificationType(PhpClassificationTypeNames.GlobalObject);

            this._docCommentText = this._classificationTypeRegistryService.GetClassificationType(PhpClassificationTypeNames.DocCommentText);
            this._docCommentTag = this._classificationTypeRegistryService.GetClassificationType(PhpClassificationTypeNames.DocCommentTag);
            this._docCommentInvalidTag = this._classificationTypeRegistryService.GetClassificationType(PhpClassificationTypeNames.DocCommentInvalidTag);

            this._htmlAttributeName = this._classificationTypeRegistryService.GetClassificationType(PhpClassificationTypeNames.HtmlAttributeName);
            this._htmlAttributeValue = this._classificationTypeRegistryService.GetClassificationType(PhpClassificationTypeNames.HtmlAttributeValue);
            this._htmlComment = this._classificationTypeRegistryService.GetClassificationType(PhpClassificationTypeNames.HtmlComment);
            this._htmlElementName = this._classificationTypeRegistryService.GetClassificationType(PhpClassificationTypeNames.HtmlElementName);
            this._htmlEntity = this._classificationTypeRegistryService.GetClassificationType(PhpClassificationTypeNames.HtmlEntity);
            this._htmlOperator = this._classificationTypeRegistryService.GetClassificationType(PhpClassificationTypeNames.HtmlOperator);
            this._htmlServerSideScript = this._classificationTypeRegistryService.GetClassificationType(PhpClassificationTypeNames.HtmlServerSideScript);
            this._htmlTagDelimiter = this._classificationTypeRegistryService.GetClassificationType(PhpClassificationTypeNames.HtmlTagDelimiter);
        }

        protected override PhpClassifierLexerState GetStartState()
        {
            return PhpClassifierLexerState.Initial;
        }

        protected override ITokenSourceWithState<PhpClassifierLexerState> CreateLexer(ICharStream input, PhpClassifierLexerState state)
        {
            return new PhpClassifierLexer(input, state);
        }

        protected override IClassificationType ClassifyToken(IToken token)
        {
            switch (token.Type)
            {
            case PhpCodeClassifierLexer.PHP_SINGLE_STRING_LITERAL:
            case PhpCodeClassifierLexer.PHP_DOUBLE_STRING_LITERAL:
            case PhpCodeClassifierLexer.PHP_HEREDOC_TEXT:
                return _standardClassificationService.StringLiteral;

            case PhpCodeClassifierLexer.PHP_NUMBER:
                return _standardClassificationService.NumberLiteral;

            case PhpCodeClassifierLexer.PHP_IDENTIFIER:
                if (Keywords.Contains(token.Text))
                    return _standardClassificationService.Keyword;
                else if (BuiltinFunctions.Contains(token.Text))
                    return _globalFunction;
                else if (BuiltinObjects.Contains(token.Text) || PredefinedConstants.Contains(token.Text))
                    return _globalObject;
                else if (token.Text[0] == '$')
                    return _standardClassificationService.SymbolDefinition;

                return _standardClassificationService.Identifier;

            // HTML Server-Side Script
            case PhpHtmlTextClassifierLexer.HTML_START_CODE:
            case PhpCodeClassifierLexer.CLOSE_PHP_TAG:
                return _htmlServerSideScript;

            // HTML Entity??

            // HTML Attribute Name
            case PhpHtmlTagClassifierLexer.HTML_ATTRIBUTE_NAME:
                return _htmlAttributeName;

            // HTML Attribute Value
            case PhpHtmlTagClassifierLexer.HTML_ATTRIBUTE_VALUE:
                return _htmlAttributeValue;

            // HTML Entity Name
            case PhpHtmlTagClassifierLexer.HTML_ELEMENT_NAME:
                return _htmlElementName;

            // HTML Operator
            case PhpHtmlTagClassifierLexer.HTML_OPERATOR:
                return _htmlOperator;

            // HTML Comment
            case PhpHtmlTextClassifierLexer.HTML_COMMENT:
                return _htmlComment;

            // HTML Tag Delimiter
            case PhpHtmlTextClassifierLexer.HTML_START_TAG:
            case PhpHtmlTagClassifierLexer.HTML_CLOSE_TAG:
                return _htmlTagDelimiter;

            case PhpCodeClassifierLexer.PHP_COMMENT:
            case PhpCodeClassifierLexer.PHP_ML_COMMENT:
                return _standardClassificationService.Comment;

            case PhpDocCommentClassifierLexer.DOC_COMMENT_TEXT:
                return _docCommentText;

            case PhpDocCommentClassifierLexer.DOC_COMMENT_TAG:
                return _docCommentTag;

            case PhpDocCommentClassifierLexer.DOC_COMMENT_INVALID_TAG:
                return _docCommentInvalidTag;

            case PhpHtmlTextClassifierLexer.HTML_CDATA:
                return _standardClassificationService.ExcludedCode;

            default:
                return null;
            }
        }
    }
}
