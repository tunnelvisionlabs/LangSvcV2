#pragma warning disable 169 // The field 'fieldname' is never used

namespace Tvl.VisualStudio.Language.Chapel
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Language.StandardClassification;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Utilities;
    using Colors = System.Windows.Media.Colors;

    public static class ChapelClassificationTypes
    {
        [Export]
        [BaseDefinition(PredefinedClassificationTypeNames.Comment)]
        [Name(ChapelClassificationTypeNames.DocCommentText)]
        private static readonly ClassificationTypeDefinition DocCommentText;

        [Export]
        [BaseDefinition(ChapelClassificationTypeNames.DocCommentText)]
        [Name(ChapelClassificationTypeNames.DocCommentTag)]
        private static readonly ClassificationTypeDefinition DocCommentTag;

        [Export]
        [BaseDefinition(ChapelClassificationTypeNames.DocCommentTag)]
        [Name(ChapelClassificationTypeNames.DocCommentInvalidTag)]
        private static readonly ClassificationTypeDefinition DocCommentInvalidTag;

        [Export(typeof(EditorFormatDefinition))]
        [Name(ChapelClassificationTypeNames.DocCommentText + ".format")]
        [DisplayName("Chapel Doc Comment (text)")]
        [UserVisible(false)]
        [ClassificationType(ClassificationTypeNames = ChapelClassificationTypeNames.DocCommentText)]
        [Order]
        internal class DocCommentTextFormatDefinition : ClassificationFormatDefinition
        {
            public DocCommentTextFormatDefinition()
            {
                this.ForegroundColor = Colors.Teal;
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name(ChapelClassificationTypeNames.DocCommentTag + ".format")]
        [DisplayName("Chapel Doc Comment (tag)")]
        [UserVisible(false)]
        [ClassificationType(ClassificationTypeNames = ChapelClassificationTypeNames.DocCommentTag)]
        [Order]
        internal class DocCommentTagFormatDefinition : ClassificationFormatDefinition
        {
            public DocCommentTagFormatDefinition()
            {
                this.ForegroundColor = Colors.DarkGray;
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name(ChapelClassificationTypeNames.DocCommentInvalidTag + ".format")]
        [DisplayName("Chapel Doc Comment (invalid tag)")]
        [UserVisible(false)]
        [ClassificationType(ClassificationTypeNames = ChapelClassificationTypeNames.DocCommentInvalidTag)]
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
