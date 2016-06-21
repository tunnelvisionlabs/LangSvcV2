#pragma warning disable 169 // The field 'fieldname' is never used

namespace Tvl.VisualStudio.Language.Alloy.Experimental
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Utilities;
    using Colors = System.Windows.Media.Colors;

    public static class AlloySymbolTaggerClassificationTypeNames
    {
        public const string Definition = "alloy.symboldefinition";
        public const string Reference = "alloy.symbolreference";
        public const string UnknownIdentifier = "alloy.unknownidentifier";

        [Export]
        [Name(AlloySymbolTaggerClassificationTypeNames.Definition)]
        private static readonly ClassificationTypeDefinition DefinitionClassification;

        [Export]
        [Name(AlloySymbolTaggerClassificationTypeNames.Reference)]
        private static readonly ClassificationTypeDefinition ReferenceClassification;

        [Export]
        [Name(AlloySymbolTaggerClassificationTypeNames.UnknownIdentifier)]
        private static readonly ClassificationTypeDefinition UnknownIdentifierClassification;

        [Export(typeof(EditorFormatDefinition))]
        [Name(AlloySymbolTaggerClassificationTypeNames.Definition + ".format")]
        [DisplayName("Alloy Symbol Tagger (definition)")]
        [UserVisible(false)]
        [ClassificationType(ClassificationTypeNames = AlloySymbolTaggerClassificationTypeNames.Definition)]
        [Order]
        internal class DefinitionTagFormatDefinition : ClassificationFormatDefinition
        {
            public DefinitionTagFormatDefinition()
            {
                this.BackgroundColor = Colors.LightBlue;
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name(AlloySymbolTaggerClassificationTypeNames.Reference + ".format")]
        [DisplayName("Alloy Symbol Tagger (reference)")]
        [UserVisible(false)]
        [ClassificationType(ClassificationTypeNames = AlloySymbolTaggerClassificationTypeNames.Reference)]
        [Order]
        internal class ReferenceTagFormatDefinition : ClassificationFormatDefinition
        {
            public ReferenceTagFormatDefinition()
            {
                this.BackgroundColor = Colors.LightGreen;
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name(AlloySymbolTaggerClassificationTypeNames.UnknownIdentifier + ".format")]
        [DisplayName("Alloy Symbol Tagger (unknown)")]
        [UserVisible(false)]
        [ClassificationType(ClassificationTypeNames = AlloySymbolTaggerClassificationTypeNames.UnknownIdentifier)]
        [Order]
        internal class UnknownIdentifierTagFormatDefinition : ClassificationFormatDefinition
        {
            public UnknownIdentifierTagFormatDefinition()
            {
                this.BackgroundColor = Colors.LightGray;
            }
        }
    }
}
