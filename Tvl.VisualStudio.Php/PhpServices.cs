#pragma warning disable 169 // The field 'fieldname' is never used

namespace Tvl.VisualStudio.Language.Php
{
    using System.ComponentModel.Composition;
    using System.Windows.Media;
    using Microsoft.VisualStudio.Language.StandardClassification;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Utilities;

    public static class PhpServices
    {
        [Export]
        [Name(PhpConstants.PhpContentType)]
        [BaseDefinition("code")]
        private static readonly ContentTypeDefinition PhpContentTypeDefinition;

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
    }
}
