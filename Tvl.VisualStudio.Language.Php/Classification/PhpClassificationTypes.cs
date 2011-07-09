#pragma warning disable 169 // The field 'fieldname' is never used

namespace Tvl.VisualStudio.Language.Php.Classification
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Language.StandardClassification;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Utilities;
    using Colors = System.Windows.Media.Colors;

    public static class PhpClassificationTypes
    {
        [Export]
        [BaseDefinition(PredefinedClassificationTypeNames.Identifier)]
        [Name(PhpClassificationTypeNames.GlobalFunction)]
        private static readonly ClassificationTypeDefinition PredefinedGlobalFunctionClassificationType;

        [Export]
        [BaseDefinition(PredefinedClassificationTypeNames.Identifier)]
        [Name(PhpClassificationTypeNames.GlobalObject)]
        private static readonly ClassificationTypeDefinition PredefinedGlobalObjectClassificationType;

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = PhpClassificationTypeNames.GlobalFunction)]
        [Name(PhpClassificationTypeNames.GlobalFunction)]
        public sealed class PredefinedGlobalFunctionFormatDefinition : ClassificationFormatDefinition
        {
            public PredefinedGlobalFunctionFormatDefinition()
            {
                this.ForegroundColor = Colors.Gray;
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = PhpClassificationTypeNames.GlobalObject)]
        [Name(PhpClassificationTypeNames.GlobalObject)]
        public sealed class PredefinedGlobalObjectFormatDefinition : ClassificationFormatDefinition
        {
            public PredefinedGlobalObjectFormatDefinition()
            {
                this.ForegroundColor = Colors.DarkGoldenrod;
            }
        }

        [Export]
        [BaseDefinition(PredefinedClassificationTypeNames.Comment)]
        [Name(PhpClassificationTypeNames.DocCommentText)]
        private static readonly ClassificationTypeDefinition DocCommentText;

        [Export]
        [BaseDefinition(PhpClassificationTypeNames.DocCommentText)]
        [Name(PhpClassificationTypeNames.DocCommentTag)]
        private static readonly ClassificationTypeDefinition DocCommentTag;

        [Export]
        [BaseDefinition(PhpClassificationTypeNames.DocCommentTag)]
        [Name(PhpClassificationTypeNames.DocCommentInvalidTag)]
        private static readonly ClassificationTypeDefinition DocCommentInvalidTag;

        [Export]
        [BaseDefinition("HTML ATTRIBUTE NAME")]
        [Name(PhpClassificationTypeNames.HtmlAttributeName)]
        private static readonly ClassificationTypeDefinition HtmlAttributeName;

        [Export]
        [BaseDefinition("HTML ATTRIBUTE VALUE")]
        [Name(PhpClassificationTypeNames.HtmlAttributeValue)]
        private static readonly ClassificationTypeDefinition HtmlAttributeValue;

        [Export]
        [BaseDefinition("HTML COMMENT")]
        [Name(PhpClassificationTypeNames.HtmlComment)]
        private static readonly ClassificationTypeDefinition HtmlComment;

        [Export]
        [BaseDefinition("HTML ELEMENT NAME")]
        [Name(PhpClassificationTypeNames.HtmlElementName)]
        private static readonly ClassificationTypeDefinition HtmlElementName;

        [Export]
        [BaseDefinition("HTML ENTITY")]
        [Name(PhpClassificationTypeNames.HtmlEntity)]
        private static readonly ClassificationTypeDefinition HtmlEntity;

        [Export]
        [BaseDefinition("HTML OPERATOR")]
        [Name(PhpClassificationTypeNames.HtmlOperator)]
        private static readonly ClassificationTypeDefinition HtmlOperator;

        [Export]
        [BaseDefinition("HTML SERVER-SIDE SCRIPT")]
        [Name(PhpClassificationTypeNames.HtmlServerSideScript)]
        private static readonly ClassificationTypeDefinition HtmlServerSideScript;

        [Export]
        [BaseDefinition("HTML TAG DELIMITER")]
        [Name(PhpClassificationTypeNames.HtmlTagDelimiter)]
        private static readonly ClassificationTypeDefinition HtmlTagDelimiter;

        [Export(typeof(EditorFormatDefinition))]
        [Name(PhpClassificationTypeNames.DocCommentText + ".format")]
        [DisplayName("PHP Doc Comment (text)")]
        [UserVisible(false)]
        [ClassificationType(ClassificationTypeNames = PhpClassificationTypeNames.DocCommentText)]
        [Order]
        internal class DocCommentTextFormatDefinition : ClassificationFormatDefinition
        {
            public DocCommentTextFormatDefinition()
            {
                this.ForegroundColor = Colors.Teal;
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name(PhpClassificationTypeNames.DocCommentTag + ".format")]
        [DisplayName("PHP Doc Comment (tag)")]
        [UserVisible(false)]
        [ClassificationType(ClassificationTypeNames = PhpClassificationTypeNames.DocCommentTag)]
        [Order]
        internal class DocCommentTagFormatDefinition : ClassificationFormatDefinition
        {
            public DocCommentTagFormatDefinition()
            {
                this.ForegroundColor = Colors.DarkGray;
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name(PhpClassificationTypeNames.DocCommentInvalidTag + ".format")]
        [DisplayName("PHP Doc Comment (invalid tag)")]
        [UserVisible(false)]
        [ClassificationType(ClassificationTypeNames = PhpClassificationTypeNames.DocCommentInvalidTag)]
        [Order]
        internal class DocCommentInvalidTagFormatDefinition : ClassificationFormatDefinition
        {
            public DocCommentInvalidTagFormatDefinition()
            {
                this.ForegroundColor = Colors.Maroon;
                this.IsBold = true;
            }
        }

    }
}
