#pragma warning disable 169 // The field 'fieldname' is never used

namespace Tvl.VisualStudio.Language.StringTemplate4
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Language.StandardClassification;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Utilities;
    using Colors = System.Windows.Media.Colors;

    public static class StringTemplateClassificationTypes
    {
        [Export]
        [BaseDefinition(PredefinedClassificationTypeNames.String)]
        [Name(StringTemplateClassificationTypeNames.TemplateLiteral0)]
        private static readonly ClassificationTypeDefinition TemplateLiteral0;

        [Export]
        [BaseDefinition(PredefinedClassificationTypeNames.FormalLanguage)]
        [Name(StringTemplateClassificationTypeNames.AnonymousTemplateDelimiter)]
        private static readonly ClassificationTypeDefinition AnonymousTemplateDelimiter;

        [Export]
        [BaseDefinition(PredefinedClassificationTypeNames.String)]
        [Name(StringTemplateClassificationTypeNames.BigStringDelimiter)]
        private static readonly ClassificationTypeDefinition BigStringDelimiter;

        [Export]
        [BaseDefinition(PredefinedClassificationTypeNames.FormalLanguage)]
        [Name(StringTemplateClassificationTypeNames.ExpressionDelimiter)]
        private static readonly ClassificationTypeDefinition ExpressionDelimiter;

        [Export]
        [BaseDefinition(PredefinedClassificationTypeNames.String)]
        [Name(StringTemplateClassificationTypeNames.EscapeCharacter)]
        private static readonly ClassificationTypeDefinition EscapeCharacter;

        [Export]
        [BaseDefinition(PredefinedClassificationTypeNames.String)]
        [Name(StringTemplateClassificationTypeNames.EscapeTag)]
        private static readonly ClassificationTypeDefinition EscapeTag;

        [Export(typeof(EditorFormatDefinition))]
        [Name("stringtemplate.anonymoustemplatetag.format")]
        [DisplayName("StringTemplate Anonymous Template Tag")]
        [UserVisible(false)]
        [ClassificationType(ClassificationTypeNames = StringTemplateClassificationTypeNames.AnonymousTemplateDelimiter)]
        [Order]
        internal class AnonymousTemplateDelimiterFormatDefinition : ClassificationFormatDefinition
        {
            public AnonymousTemplateDelimiterFormatDefinition()
            {
                IsBold = true;
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name("stringtemplate.bigstringdelimiter.format")]
        [DisplayName("StringTemplate Big String Delimiter")]
        [UserVisible(false)]
        [ClassificationType(ClassificationTypeNames = StringTemplateClassificationTypeNames.BigStringDelimiter)]
        [Order]
        internal class BigStringDelimiterFormatDefinition : ClassificationFormatDefinition
        {
            public BigStringDelimiterFormatDefinition()
            {
                BackgroundColor = Colors.Yellow;
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name("stringtemplate.expressiontag.format")]
        [DisplayName("StringTemplate Expression Tag")]
        [UserVisible(false)]
        [ClassificationType(ClassificationTypeNames = StringTemplateClassificationTypeNames.ExpressionDelimiter)]
        [Order]
        internal class ExpressionDelimiterFormatDefinition : ClassificationFormatDefinition
        {
            public ExpressionDelimiterFormatDefinition()
            {
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name("stringtemplate.escapecharacter.format")]
        [DisplayName("StringTemplate Escape Character")]
        [UserVisible(false)]
        [ClassificationType(ClassificationTypeNames = StringTemplateClassificationTypeNames.EscapeCharacter)]
        [Order]
        internal class EscapeCharacterFormatDefinition : ClassificationFormatDefinition
        {
            public EscapeCharacterFormatDefinition()
            {
                ForegroundColor = Colors.DarkBlue;
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name("stringtemplate.escapetag.format")]
        [DisplayName("StringTemplate Escape Tag")]
        [UserVisible(false)]
        [ClassificationType(ClassificationTypeNames = StringTemplateClassificationTypeNames.EscapeTag)]
        [Order]
        internal class EscapeTagFormatDefinition : ClassificationFormatDefinition
        {
            public EscapeTagFormatDefinition()
            {
                ForegroundColor = Colors.DarkBlue;
            }
        }
    }
}
