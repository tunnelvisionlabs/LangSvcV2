#pragma warning disable 169 // The field 'fieldname' is never used

namespace Tvl.VisualStudio.Language.AntlrV4
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Utilities;

    public static class Antlr4Services
    {
        [Export]
        [Name(Antlr4Constants.AntlrContentType)]
        [BaseDefinition("code")]
        private static readonly ContentTypeDefinition AntlrContentTypeDefinition;

        /* The FileExtensionToContentTypeDefinition export is not necessary when the package provides an
         * IVsLanguageInfo implementation.
         */
        //[Export]
        //[FileExtension(Antlr4Constants.AntlrFileExtension)]
        //[ContentType(Antlr4Constants.AntlrContentType)]
        //private static readonly FileExtensionToContentTypeDefinition AntlrFileExtensionToContentTypeDefinition;
    }
}
