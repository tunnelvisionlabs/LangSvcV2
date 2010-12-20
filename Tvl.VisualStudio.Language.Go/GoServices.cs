#pragma warning disable 169 // The field 'fieldname' is never used

namespace Tvl.VisualStudio.Language.Go
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Utilities;

    public static class GoServices
    {
        [Export]
        [Name(GoConstants.GoContentType)]
        [BaseDefinition("code")]
        private static readonly ContentTypeDefinition GoContentTypeDefinition;

        [Export]
        [FileExtension(GoConstants.GoFileExtension)]
        [ContentType(GoConstants.GoContentType)]
        private static readonly FileExtensionToContentTypeDefinition GoFileExtensionToContentTypeDefinition;
    }
}
