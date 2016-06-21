#pragma warning disable 169 // The field 'fieldname' is never used

namespace Tvl.VisualStudio.Language.Alloy
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Utilities;

    public static class AlloyServices
    {
        [Export]
        [Name(AlloyConstants.AlloyContentType)]
        [BaseDefinition("code")]
        private static readonly ContentTypeDefinition AlloyContentTypeDefinition;

        /* The FileExtensionToContentTypeDefinition export is not necessary when the package provides an
         * IVsLanguageInfo implementation.
         */
        //[Export]
        //[FileExtension(AlloyConstants.AlloyFileExtension)]
        //[ContentType(AlloyConstants.AlloyContentType)]
        //private static readonly FileExtensionToContentTypeDefinition AlloyFileExtensionToContentTypeDefinition;
    }
}
