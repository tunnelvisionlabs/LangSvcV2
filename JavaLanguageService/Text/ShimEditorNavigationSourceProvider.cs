namespace Tvl.VisualStudio.Text.Navigation.Implementation
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
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
