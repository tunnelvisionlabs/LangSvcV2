namespace JavaLanguageService.Text
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Text;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Text;

    [Export(typeof(IEditorNavigationSourceProvider))]
    [ContentType("text")]
    public sealed class ShimEditorNavigationSourceProvider : IEditorNavigationSourceProvider
    {
        public IEditorNavigationSource TryCreateEditorNavigationSource(ITextBuffer textBuffer)
        {
            throw new NotImplementedException();
            //return textBuffer.Properties.GetOrCreateSingletonProperty(() => new ShimEditorNavigationSource(textBuffer));
        }
    }
}
