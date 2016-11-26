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

        [Export]
        [BaseDefinition(StringTemplateClassificationTypeNames.TemplateReference)]
        [Name(StringTemplateClassificationTypeNames.TemplateDefinition)]
        private static readonly ClassificationTypeDefinition TemplateDeclaration;

        [Export]
        [BaseDefinition(PredefinedClassificationTypeNames.Identifier)]
        [Name(StringTemplateClassificationTypeNames.TemplateReference)]
        private static readonly ClassificationTypeDefinition TemplateUse;

        [Export]
        [BaseDefinition(StringTemplateClassificationTypeNames.RegionReference)]
        [Name(StringTemplateClassificationTypeNames.RegionDefinition)]
        private static readonly ClassificationTypeDefinition RegionDeclaration;

        [Export]
        [BaseDefinition(StringTemplateClassificationTypeNames.TemplateReference)]
        [Name(StringTemplateClassificationTypeNames.RegionReference)]
        private static readonly ClassificationTypeDefinition RegionUse;

        [Export]
        [BaseDefinition(StringTemplateClassificationTypeNames.DictionaryReference)]
        [Name(StringTemplateClassificationTypeNames.DictionaryDefinition)]
        private static readonly ClassificationTypeDefinition DictionaryDeclaration;

        [Export]
        [BaseDefinition(StringTemplateClassificationTypeNames.TemplateReference)]
        [Name(StringTemplateClassificationTypeNames.DictionaryReference)]
        private static readonly ClassificationTypeDefinition DictionaryUse;

        [Export]
        [BaseDefinition(StringTemplateClassificationTypeNames.ParameterReference)]
        [Name(StringTemplateClassificationTypeNames.ParameterDefinition)]
        private static readonly ClassificationTypeDefinition ParameterDeclaration;

        [Export]
        [BaseDefinition(PredefinedClassificationTypeNames.Identifier)]
        [Name(StringTemplateClassificationTypeNames.ParameterReference)]
        private static readonly ClassificationTypeDefinition ParameterUse;

        [Export]
        [BaseDefinition(StringTemplateClassificationTypeNames.ParameterReference)]
        [Name(StringTemplateClassificationTypeNames.AttributeReference)]
        private static readonly ClassificationTypeDefinition AttributeUse;

        [Export]
        [BaseDefinition(PredefinedClassificationTypeNames.Identifier)]
        [Name(StringTemplateClassificationTypeNames.ExpressionOption)]
        private static readonly ClassificationTypeDefinition ExpressionOption;

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

        [Export(typeof(EditorFormatDefinition))]
        [Name("stringtemplate.templatedefinition.format")]
        [DisplayName("StringTemplate Template Definition")]
        [UserVisible(true)]
        [ClassificationType(ClassificationTypeNames = StringTemplateClassificationTypeNames.TemplateDefinition)]
        [Order]
        internal class TemplateDefinitionFormatDefinition : ClassificationFormatDefinition
        {
            public TemplateDefinitionFormatDefinition()
            {
                DisplayName = "StringTemplate Template Definition";
                IsBold = true;
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name("stringtemplate.templatereference.format")]
        [DisplayName("StringTemplate Template Reference")]
        [UserVisible(true)]
        [ClassificationType(ClassificationTypeNames = StringTemplateClassificationTypeNames.TemplateReference)]
        [Order]
        internal class TemplateReferenceFormatDefinition : ClassificationFormatDefinition
        {
            public TemplateReferenceFormatDefinition()
            {
                DisplayName = "StringTemplate Template Reference";
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name("stringtemplate.regiondefinition.format")]
        [DisplayName("StringTemplate Region Definition")]
        [UserVisible(true)]
        [ClassificationType(ClassificationTypeNames = StringTemplateClassificationTypeNames.RegionDefinition)]
        [Order]
        internal class RegionDefinitionFormatDefinition : ClassificationFormatDefinition
        {
            public RegionDefinitionFormatDefinition()
            {
                DisplayName = "StringTemplate Region Definition";
                IsBold = true;
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name("stringtemplate.regionreference.format")]
        [DisplayName("StringTemplate Region Reference")]
        [UserVisible(true)]
        [ClassificationType(ClassificationTypeNames = StringTemplateClassificationTypeNames.RegionReference)]
        [Order]
        internal class RegionReferenceFormatDefinition : ClassificationFormatDefinition
        {
            public RegionReferenceFormatDefinition()
            {
                DisplayName = "StringTemplate Region Reference";
                IsItalic = true;
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name("stringtemplate.dictionarydefinition.format")]
        [DisplayName("StringTemplate Dictionary Definition")]
        [UserVisible(true)]
        [ClassificationType(ClassificationTypeNames = StringTemplateClassificationTypeNames.DictionaryDefinition)]
        [Order]
        internal class DictionaryDefinitionFormatDefinition : ClassificationFormatDefinition
        {
            public DictionaryDefinitionFormatDefinition()
            {
                DisplayName = "StringTemplate Dictionary Definition";
                IsBold = true;
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name("stringtemplate.dictionaryreference.format")]
        [DisplayName("StringTemplate Dictionary Reference")]
        [UserVisible(true)]
        [ClassificationType(ClassificationTypeNames = StringTemplateClassificationTypeNames.DictionaryReference)]
        [Order]
        internal class DictionaryReferenceFormatDefinition : ClassificationFormatDefinition
        {
            public DictionaryReferenceFormatDefinition()
            {
                DisplayName = "StringTemplate Dictionary Reference";
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name("stringtemplate.parameterdefinition.format")]
        [DisplayName("StringTemplate Parameter Definition")]
        [UserVisible(true)]
        [ClassificationType(ClassificationTypeNames = StringTemplateClassificationTypeNames.ParameterDefinition)]
        [Order]
        internal class ParameterDefinitionFormatDefinition : ClassificationFormatDefinition
        {
            public ParameterDefinitionFormatDefinition()
            {
                DisplayName = "StringTemplate Parameter Definition";
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name("stringtemplate.parameterreference.format")]
        [DisplayName("StringTemplate Parameter Reference")]
        [UserVisible(true)]
        [ClassificationType(ClassificationTypeNames = StringTemplateClassificationTypeNames.ParameterReference)]
        [Order]
        internal class ParameterReferenceFormatDefinition : ClassificationFormatDefinition
        {
            public ParameterReferenceFormatDefinition()
            {
                DisplayName = "StringTemplate Parameter Reference";
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name("stringtemplate.attributereference.format")]
        [DisplayName("StringTemplate Attribute Reference")]
        [UserVisible(true)]
        [ClassificationType(ClassificationTypeNames = StringTemplateClassificationTypeNames.AttributeReference)]
        [Order]
        internal class AttributeReferenceFormatDefinition : ClassificationFormatDefinition
        {
            public AttributeReferenceFormatDefinition()
            {
                DisplayName = "StringTemplate Attribute Reference";
                IsItalic = true;
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name("stringtemplate.expressionoption.format")]
        [DisplayName("StringTemplate Expression Option")]
        [UserVisible(true)]
        [ClassificationType(ClassificationTypeNames = StringTemplateClassificationTypeNames.ExpressionOption)]
        [Order]
        internal class ExpressionOptionFormatDefinition : ClassificationFormatDefinition
        {
            public ExpressionOptionFormatDefinition()
            {
                DisplayName = "StringTemplate Expression Option";
                IsItalic = true;
            }
        }
    }
}
