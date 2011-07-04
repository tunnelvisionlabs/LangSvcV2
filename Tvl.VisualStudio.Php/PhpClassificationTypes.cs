#pragma warning disable 169 // The field 'fieldname' is never used

namespace Tvl.VisualStudio.Language.Php
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Language.StandardClassification;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Utilities;
    using Colors = System.Windows.Media.Colors;

    public static class PhpClassificationTypes
    {
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
