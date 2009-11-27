namespace JavaLanguageService.AntlrLanguage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Language.StandardClassification;
    using Microsoft.VisualStudio.Utilities;
    using Microsoft.VisualStudio.Text.Classification;
    using System.Windows.Media;

    public static class AntlrServices
    {
        [Export]
        [Name(AntlrConstants.AntlrContentType)]
        [BaseDefinition("code")]
        internal static readonly ContentTypeDefinition AntlrContentTypeDefinition;

        [Export]
        [FileExtension(AntlrConstants.AntlrFileExtension)]
        [ContentType(AntlrConstants.AntlrContentType)]
        internal static FileExtensionToContentTypeDefinition AntlrFileExtensionToContentTypeDefinition;

        [Export]
        [FileExtension(AntlrConstants.AntlrFileExtension2)]
        [ContentType(AntlrConstants.AntlrContentType)]
        internal static FileExtensionToContentTypeDefinition AntlrFileExtensionToContentTypeDefinition2;

        [Export]
        [BaseDefinition(PredefinedClassificationTypeNames.Identifier)]
        [Name(AntlrClassificationTypeNames.ParserRule)]
        internal static ClassificationTypeDefinition ParserRuleClassificationType;

        [Export]
        [BaseDefinition(PredefinedClassificationTypeNames.Identifier)]
        [Name(AntlrClassificationTypeNames.LexerRule)]
        internal static ClassificationTypeDefinition LexerRuleClassificationType;

        [Export]
        [BaseDefinition(PredefinedClassificationTypeNames.Literal)]
        [Name(AntlrClassificationTypeNames.ActionLiteral)]
        internal static ClassificationTypeDefinition ActionLiteralClassificationType;

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = AntlrClassificationTypeNames.ParserRule)]
        [Name(AntlrClassificationTypeNames.ParserRule)]
        public sealed class ParserRuleFormatDefinition : ClassificationFormatDefinition
        {
            public ParserRuleFormatDefinition()
            {
                this.ForegroundColor = Colors.SlateBlue;
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = AntlrClassificationTypeNames.LexerRule)]
        [Name(AntlrClassificationTypeNames.LexerRule)]
        public sealed class LexerRuleFormatDefinition : ClassificationFormatDefinition
        {
            public LexerRuleFormatDefinition()
            {
                this.ForegroundColor = Colors.DarkBlue;
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = AntlrClassificationTypeNames.ActionLiteral)]
        [Name(AntlrClassificationTypeNames.ActionLiteral)]
        public sealed class ActionLiteralFormatDefinition : ClassificationFormatDefinition
        {
            public ActionLiteralFormatDefinition()
            {
                this.ForegroundColor = Colors.Black;
                this.BackgroundColor = Colors.LightGray;
            }
        }
    }
}
