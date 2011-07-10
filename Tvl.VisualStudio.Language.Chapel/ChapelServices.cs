#pragma warning disable 169 // The field 'fieldname' is never used

namespace Tvl.VisualStudio.Language.Chapel
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Shell.OutputWindow;

    public static class ChapelServices
    {
        [Export]
        [Name(ChapelConstants.ChapelContentType)]
        [BaseDefinition("code")]
        private static readonly ContentTypeDefinition ChapelContentTypeDefinition;

        [Export]
        [FileExtension(ChapelConstants.ChapelFileExtension)]
        [ContentType(ChapelConstants.ChapelContentType)]
        private static readonly FileExtensionToContentTypeDefinition ChapelFileExtensionToContentTypeDefinition;
    }
}
