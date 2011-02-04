#pragma warning disable 169 // The field 'fieldname' is never used

namespace Tvl.VisualStudio.Language.Antlr3
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

        [Export(typeof(EditorNavigationTypeDefinition))]
        [Name(AntlrEditorNavigationTypeNames.ParserRule)]
        [Order(Before = AntlrEditorNavigationTypeNames.LexerRule)]
        internal sealed class ParserRuleNavigationType : EditorNavigationTypeDefinition
        {
            public ParserRuleNavigationType()
            {
                this.DisplayName = "Parser Rules";
            }
        }

        [Export(typeof(EditorNavigationTypeDefinition))]
        [Name(AntlrEditorNavigationTypeNames.LexerRule)]
        internal sealed class LexerRuleNavigationType : EditorNavigationTypeDefinition
        {
            public LexerRuleNavigationType()
            {
                this.DisplayName = "Parser Rules";
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = AntlrClassificationTypeNames.ParserRule)]
        [Name(AntlrClassificationTypeNames.ParserRule)]
        [UserVisible(true)]
        public sealed class ParserRuleFormatDefinition : ClassificationFormatDefinition
        {
            public ParserRuleFormatDefinition()
            {
                this.ForegroundColor = Colors.SlateBlue;
                this.DisplayName = "ANTLR Parser Rule";
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = AntlrClassificationTypeNames.LexerRule)]
        [Name(AntlrClassificationTypeNames.LexerRule)]
        [UserVisible(true)]
        public sealed class LexerRuleFormatDefinition : ClassificationFormatDefinition
        {
            public LexerRuleFormatDefinition()
            {
                this.ForegroundColor = Colors.DarkBlue;
                this.DisplayName = "ANTLR Lexer Rule";
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = AntlrClassificationTypeNames.ActionLiteral)]
        [Name(AntlrClassificationTypeNames.ActionLiteral)]
        [UserVisible(true)]
        public sealed class ActionLiteralFormatDefinition : ClassificationFormatDefinition
        {
            public ActionLiteralFormatDefinition()
            {
                this.ForegroundColor = null;
                this.BackgroundColor = Colors.LavenderBlush;
                this.DisplayName = "ANTLR Action Literal";
            }
        }
    }
}
