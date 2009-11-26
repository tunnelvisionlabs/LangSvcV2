namespace JavaLanguageService
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Utilities;
    using JavaLanguageService.Panes;

    public static class Services
    {
        [Export]
        [Name(Constants.JavaContentType)]
        [BaseDefinition("code")]
        internal static readonly ContentTypeDefinition JavaContentTypeDefinition;

        [Export]
        [FileExtension(Constants.JavaFileExtension)]
        [ContentType(Constants.JavaContentType)]
        internal static FileExtensionToContentTypeDefinition JavaFileExtensionToContentTypeDefinition;

        [Export]
        [Name(Constants.AntlrIntellisenseOutputWindow)]
        internal static OutputWindowDefinition AntlrIntellisenseOutputWindowDefinition;
    }
}
