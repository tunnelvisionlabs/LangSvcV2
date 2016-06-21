#pragma warning disable 169 // The field 'fieldname' is never used

namespace Tvl.VisualStudio.Language.Go.Experimental
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Utilities;
    using Colors = System.Windows.Media.Colors;

    public static class GoSymbolTaggerClassificationTypeNames
    {
        public const string Definition = "go.symboldefinition";
        public const string Reference = "go.symbolreference";
        public const string UnknownIdentifier = "go.unknownidentifier";

        [Export]
        [Name(GoSymbolTaggerClassificationTypeNames.Definition)]
        private static readonly ClassificationTypeDefinition DefinitionClassification;

        [Export]
        [Name(GoSymbolTaggerClassificationTypeNames.Reference)]
        private static readonly ClassificationTypeDefinition ReferenceClassification;

        [Export]
        [Name(GoSymbolTaggerClassificationTypeNames.UnknownIdentifier)]
        private static readonly ClassificationTypeDefinition UnknownIdentifierClassification;

        [Export(typeof(EditorFormatDefinition))]
        [Name(GoSymbolTaggerClassificationTypeNames.Definition + ".format")]
        [DisplayName("Go Symbol Tagger (definition)")]
        [UserVisible(false)]
        [ClassificationType(ClassificationTypeNames = GoSymbolTaggerClassificationTypeNames.Definition)]
        [Order]
        internal class DefinitionTagFormatDefinition : ClassificationFormatDefinition
        {
            public DefinitionTagFormatDefinition()
            {
                this.BackgroundColor = Colors.LightBlue;
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name(GoSymbolTaggerClassificationTypeNames.Reference + ".format")]
        [DisplayName("Go Symbol Tagger (reference)")]
        [UserVisible(false)]
        [ClassificationType(ClassificationTypeNames = GoSymbolTaggerClassificationTypeNames.Reference)]
        [Order]
        internal class ReferenceTagFormatDefinition : ClassificationFormatDefinition
        {
            public ReferenceTagFormatDefinition()
            {
                this.BackgroundColor = Colors.LightGreen;
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name(GoSymbolTaggerClassificationTypeNames.UnknownIdentifier + ".format")]
        [DisplayName("Go Symbol Tagger (unknown)")]
        [UserVisible(false)]
        [ClassificationType(ClassificationTypeNames = GoSymbolTaggerClassificationTypeNames.UnknownIdentifier)]
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
