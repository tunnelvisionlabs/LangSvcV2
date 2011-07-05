#pragma warning disable 169 // The field 'fieldname' is never used

namespace Tvl.VisualStudio.Language.Php
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Utilities;

    public static class PhpServices
    {
        [Export]
        [Name(PhpConstants.PhpContentType)]
        [BaseDefinition("code")]
        private static readonly ContentTypeDefinition PhpContentTypeDefinition;
    }
}
