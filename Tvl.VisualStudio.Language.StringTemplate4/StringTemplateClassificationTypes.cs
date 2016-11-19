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
        [UserVisible(true)]
        [ClassificationType(ClassificationTypeNames = StringTemplateClassificationTypeNames.AnonymousTemplateDelimiter)]
        [Order]
        internal class AnonymousTemplateDelimiterFormatDefinition : ClassificationFormatDefinition
        {
            public AnonymousTemplateDelimiterFormatDefinition()
            {
                DisplayName = "StringTemplate Anonymous Template Tag";
                IsBold = true;
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name("stringtemplate.bigstringdelimiter.format")]
        [DisplayName("StringTemplate Big String Delimiter")]
        [UserVisible(true)]
        [ClassificationType(ClassificationTypeNames = StringTemplateClassificationTypeNames.BigStringDelimiter)]
        [Order]
        internal class BigStringDelimiterFormatDefinition : ClassificationFormatDefinition
        {
            public BigStringDelimiterFormatDefinition()
            {
                DisplayName = "StringTemplate Big String Delimiter";
                BackgroundColor = Colors.Yellow;
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name("stringtemplate.expressiontag.format")]
        [DisplayName("StringTemplate Expression Tag")]
        [UserVisible(true)]
        [ClassificationType(ClassificationTypeNames = StringTemplateClassificationTypeNames.ExpressionDelimiter)]
        [Order]
        internal class ExpressionDelimiterFormatDefinition : ClassificationFormatDefinition
        {
            public ExpressionDelimiterFormatDefinition()
            {
                DisplayName = "StringTemplate Expression Tag";
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name("stringtemplate.escapecharacter.format")]
        [DisplayName("StringTemplate Escape Character")]
        [UserVisible(true)]
        [ClassificationType(ClassificationTypeNames = StringTemplateClassificationTypeNames.EscapeCharacter)]
        [Order]
        internal class EscapeCharacterFormatDefinition : ClassificationFormatDefinition
        {
            public EscapeCharacterFormatDefinition()
            {
                DisplayName = "StringTemplate Escape Character";
                ForegroundColor = Colors.DarkBlue;
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name("stringtemplate.escapetag.format")]
        [DisplayName("StringTemplate Escape Tag")]
        [UserVisible(true)]
        [ClassificationType(ClassificationTypeNames = StringTemplateClassificationTypeNames.EscapeTag)]
        [Order]
        internal class EscapeTagFormatDefinition : ClassificationFormatDefinition
        {
            public EscapeTagFormatDefinition()
            {
                DisplayName = "StringTemplate Escape Tag";
                ForegroundColor = Colors.DarkBlue;
            }
        }
    }
}
