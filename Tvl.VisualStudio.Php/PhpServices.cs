namespace Tvl.VisualStudio.Language.Php
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Utilities;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Language.StandardClassification;
    using System.Windows.Media;

    public static class PhpServices
    {
        [Export]
        [Name(PhpConstants.PhpContentType)]
        [BaseDefinition("code")]
        private static readonly ContentTypeDefinition PhpContentTypeDefinition;

        [Export]
        [FileExtension(PhpConstants.PhpFileExtension)]
        [ContentType(PhpConstants.PhpContentType)]
        private static readonly FileExtensionToContentTypeDefinition PhpFileExtensionToContentTypeDefinition;

        [Export]
        [FileExtension(PhpConstants.PhpFileExtension2)]
        [ContentType(PhpConstants.PhpContentType)]
        private static readonly FileExtensionToContentTypeDefinition PhpFileExtensionToContentTypeDefinition2;

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
