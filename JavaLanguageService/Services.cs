namespace JavaLanguageService
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Shell.OutputWindow;

    public static class Services
    {
        [Export]
        [Name(Constants.JavaContentType)]
        [BaseDefinition("code")]
        private static readonly ContentTypeDefinition JavaContentTypeDefinition;

        [Export]
        [FileExtension(Constants.JavaFileExtension)]
        [ContentType(Constants.JavaContentType)]
        private static readonly FileExtensionToContentTypeDefinition JavaFileExtensionToContentTypeDefinition;

        [Export]
        [Name(Constants.AntlrIntellisenseOutputWindow)]
        private static readonly OutputWindowDefinition AntlrIntellisenseOutputWindowDefinition;
    }
}
