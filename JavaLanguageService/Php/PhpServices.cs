namespace JavaLanguageService.Php
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Utilities;

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
    }
}
