namespace Tvl.VisualStudio.Language.Php.Classification
{
    using System.Collections.Generic;
    using Antlr4.Runtime;
    using Microsoft.VisualStudio.Language.StandardClassification;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Tvl.VisualStudio.Language.Parsing4;

    using StringComparer = System.StringComparer;

    internal sealed partial class V4PhpClassifier : AntlrClassifierBase<V4PhpClassifierLexerState>
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

        public V4PhpClassifier(ITextBuffer textBuffer, IStandardClassificationService standardClassificationService, IClassificationTypeRegistryService classificationTypeRegistryService)
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

        protected override V4PhpClassifierLexerState GetStartState()
        {
            return V4PhpClassifierLexerState.Initial;
        }

        protected override ITokenSourceWithState<V4PhpClassifierLexerState> CreateLexer(ICharStream input, V4PhpClassifierLexerState state)
        {
            V4PhpClassifierLexer lexer = new V4PhpClassifierLexer(input);
            state.Apply(lexer);
            return lexer;
        }

        protected override IClassificationType ClassifyToken(IToken token)
        {
            switch (token.Type)
            {
            case V4PhpClassifierLexer.PHP_SINGLE_STRING_LITERAL:
            case V4PhpClassifierLexer.PHP_DOUBLE_STRING_LITERAL:
            case V4PhpClassifierLexer.PHP_HEREDOC_TEXT:
                return _standardClassificationService.StringLiteral;

            case V4PhpClassifierLexer.PHP_NUMBER:
                return _standardClassificationService.NumberLiteral;

            case V4PhpClassifierLexer.KW___CLASS__:
            case V4PhpClassifierLexer.KW___DIR__:
            case V4PhpClassifierLexer.KW___FILE__:
            case V4PhpClassifierLexer.KW___FUNCTION__:
            case V4PhpClassifierLexer.KW___LINE__:
            case V4PhpClassifierLexer.KW___METHOD__:
            case V4PhpClassifierLexer.KW___NAMESPACE__:
            case V4PhpClassifierLexer.KW_ABSTRACT:
            case V4PhpClassifierLexer.KW_AND:
            case V4PhpClassifierLexer.KW_AS:
            case V4PhpClassifierLexer.KW_BREAK:
            case V4PhpClassifierLexer.KW_CASE:
            case V4PhpClassifierLexer.KW_CATCH:
            case V4PhpClassifierLexer.KW_CLASS:
            case V4PhpClassifierLexer.KW_CLONE:
            case V4PhpClassifierLexer.KW_CONST:
            case V4PhpClassifierLexer.KW_CONTINUE:
            case V4PhpClassifierLexer.KW_DECLARE:
            case V4PhpClassifierLexer.KW_DEFAULT:
            case V4PhpClassifierLexer.KW_DO:
            case V4PhpClassifierLexer.KW_ELSE:
            case V4PhpClassifierLexer.KW_ELSEIF:
            case V4PhpClassifierLexer.KW_ENDDECLARE:
            case V4PhpClassifierLexer.KW_ENDFOR:
            case V4PhpClassifierLexer.KW_ENDFOREACH:
            case V4PhpClassifierLexer.KW_ENDIF:
            case V4PhpClassifierLexer.KW_ENDSWITCH:
            case V4PhpClassifierLexer.KW_ENDWHILE:
            case V4PhpClassifierLexer.KW_EXCEPTION:
            case V4PhpClassifierLexer.KW_EXTENDS:
            case V4PhpClassifierLexer.KW_FINAL:
            case V4PhpClassifierLexer.KW_FOR:
            case V4PhpClassifierLexer.KW_FOREACH:
            case V4PhpClassifierLexer.KW_FUNCTION:
            case V4PhpClassifierLexer.KW_GLOBAL:
            case V4PhpClassifierLexer.KW_IF:
            case V4PhpClassifierLexer.KW_IMPLEMENTS:
            case V4PhpClassifierLexer.KW_INSTANCEOF:
            case V4PhpClassifierLexer.KW_INTERFACE:
            case V4PhpClassifierLexer.KW_NAMESPACE:
            case V4PhpClassifierLexer.KW_NEW:
            case V4PhpClassifierLexer.KW_OR:
            case V4PhpClassifierLexer.KW_PHP_USER_FILTER:
            case V4PhpClassifierLexer.KW_PRIVATE:
            case V4PhpClassifierLexer.KW_PROTECTED:
            case V4PhpClassifierLexer.KW_PUBLIC:
            case V4PhpClassifierLexer.KW_RETURN:
            case V4PhpClassifierLexer.KW_STATIC:
            case V4PhpClassifierLexer.KW_SWITCH:
            case V4PhpClassifierLexer.KW_THIS:
            case V4PhpClassifierLexer.KW_THROW:
            case V4PhpClassifierLexer.KW_TRY:
            case V4PhpClassifierLexer.KW_USE:
            case V4PhpClassifierLexer.KW_VAR:
            case V4PhpClassifierLexer.KW_WHILE:
            case V4PhpClassifierLexer.KW_XOR:
                return _standardClassificationService.Keyword;

            case V4PhpClassifierLexer.PHP_IDENTIFIER:
                if (PhpClassifier.Keywords.Contains(token.Text))
                    return _standardClassificationService.Keyword;
                else if (PhpClassifier.BuiltinFunctions.Contains(token.Text))
                    return _globalFunction;
                else if (PhpClassifier.BuiltinObjects.Contains(token.Text) || PhpClassifier.PredefinedConstants.Contains(token.Text))
                    return _globalObject;
                else if (token.Text[0] == '$')
                    return _standardClassificationService.SymbolDefinition;

                return _standardClassificationService.Identifier;

            // HTML Server-Side Script
            case V4PhpClassifierLexer.HTML_START_CODE:
            case V4PhpClassifierLexer.CLOSE_PHP_TAG:
                return _htmlServerSideScript;

            // HTML Attribute Name
            case V4PhpClassifierLexer.HTML_ATTRIBUTE_NAME:
                return _htmlAttributeName;

            // HTML Attribute Value
            case V4PhpClassifierLexer.HTML_ATTRIBUTE_VALUE:
                return _htmlAttributeValue;

            // HTML Entity Name
            case V4PhpClassifierLexer.HTML_ELEMENT_NAME:
                return _htmlElementName;

            // HTML Operator
            case V4PhpClassifierLexer.HTML_OPERATOR:
                return _htmlOperator;

            // HTML Comment
            case V4PhpClassifierLexer.HTML_COMMENT:
                return _htmlComment;

            // HTML Tag Delimiter
            case V4PhpClassifierLexer.HTML_START_TAG:
            case V4PhpClassifierLexer.HTML_CLOSE_TAG:
                return _htmlTagDelimiter;

            case V4PhpClassifierLexer.PHP_COMMENT:
            case V4PhpClassifierLexer.PHP_ML_COMMENT:
                return _standardClassificationService.Comment;

            case V4PhpClassifierLexer.DOC_COMMENT_TEXT:
                return _docCommentText;

            case V4PhpClassifierLexer.DOC_COMMENT_TAG:
                return _docCommentTag;

            case V4PhpClassifierLexer.DOC_COMMENT_INVALID_TAG:
                return _docCommentInvalidTag;

            case V4PhpClassifierLexer.HTML_CDATA:
                return _standardClassificationService.ExcludedCode;

            default:
                return null;
            }
        }
    }
}
