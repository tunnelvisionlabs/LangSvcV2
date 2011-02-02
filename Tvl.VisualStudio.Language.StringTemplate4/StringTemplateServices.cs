#pragma warning disable 169 // The field 'fieldname' is never used

namespace Tvl.VisualStudio.Language.StringTemplate4
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Utilities;

    public static class StringTemplateServices
    {
        [Export]
        [Name(StringTemplateConstants.StringTemplateContentType)]
        [BaseDefinition("code")]
        private static readonly ContentTypeDefinition StringTemplateContentTypeDefinition;

        /* The FileExtensionToContentTypeDefinition export is not necessary when the package provides an
         * IVsLanguageInfo implementation.
         */
        [Export]
        [FileExtension(StringTemplateConstants.StringTemplateFileExtension)]
        [ContentType(StringTemplateConstants.StringTemplateContentType)]
        private static readonly FileExtensionToContentTypeDefinition StringTemplateFileExtensionToContentTypeDefinition;
    }
}
