namespace JavaLanguageService.AntlrLanguage
{
    using System.ComponentModel.Composition;
    using System.Windows.Media;
    using Microsoft.VisualStudio.Language.StandardClassification;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Utilities;
using Tvl.VisualStudio.Text.Navigation;

    public static class AntlrServices
    {
        [Export]
        [Name(AntlrConstants.AntlrContentType)]
        [BaseDefinition("code")]
        private static readonly ContentTypeDefinition AntlrContentTypeDefinition;

        [Export]
        [FileExtension(AntlrConstants.AntlrFileExtension)]
        [ContentType(AntlrConstants.AntlrContentType)]
        private static readonly FileExtensionToContentTypeDefinition AntlrFileExtensionToContentTypeDefinition;

        [Export]
        [FileExtension(AntlrConstants.AntlrFileExtension2)]
        [ContentType(AntlrConstants.AntlrContentType)]
        private static readonly FileExtensionToContentTypeDefinition AntlrFileExtensionToContentTypeDefinition2;

        [Export]
        [Name(AntlrEditorNavigationTypeNames.ParserRule)]
        [Order(Before = AntlrEditorNavigationTypeNames.LexerRule)]
        [DisplayName("Parser Rules")]
        private static readonly EditorNavigationTypeDefinition ParserRuleNavigationType;

        [Export]
        [Name(AntlrEditorNavigationTypeNames.LexerRule)]
        [DisplayName("Lexer Rules")]
        private static readonly EditorNavigationTypeDefinition LexerRuleNavigationType;

        [Export]
        [BaseDefinition(PredefinedClassificationTypeNames.Identifier)]
        [Name(AntlrClassificationTypeNames.ParserRule)]
        private static readonly ClassificationTypeDefinition ParserRuleClassificationType;

        [Export]
        [BaseDefinition(PredefinedClassificationTypeNames.Identifier)]
        [Name(AntlrClassificationTypeNames.LexerRule)]
        private static readonly ClassificationTypeDefinition LexerRuleClassificationType;

        [Export]
        [BaseDefinition(PredefinedClassificationTypeNames.Literal)]
        [Name(AntlrClassificationTypeNames.ActionLiteral)]
        private static readonly ClassificationTypeDefinition ActionLiteralClassificationType;

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
