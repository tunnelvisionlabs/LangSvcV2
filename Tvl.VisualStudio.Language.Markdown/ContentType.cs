namespace Tvl.VisualStudio.Language.Markdown
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Utilities;

    internal static class ContentType
    {
        [Export]
        [Name(MarkdownConstants.MarkdownContentType)]
        [DisplayName("Markdown")]
        [BaseDefinition("plaintext")]
        [BaseDefinition("HTML")]
        public static ContentTypeDefinition MarkdownModeContentType = null;

        [Export]
        [ContentType(MarkdownConstants.MarkdownContentType)]
        [FileExtension(MarkdownConstants.MarkdownFileExtension)]
        public static FileExtensionToContentTypeDefinition MkdFileExtension = null;

        [Export]
        [ContentType(MarkdownConstants.MarkdownContentType)]
        [FileExtension(MarkdownConstants.MarkdownFileExtension2)]
        public static FileExtensionToContentTypeDefinition MarkdownFileExtension = null;
    }
}
