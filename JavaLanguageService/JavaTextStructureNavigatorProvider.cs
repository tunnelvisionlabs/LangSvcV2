namespace JavaLanguageService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Text.Operations;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Utilities;
    using System.ComponentModel.Composition;

    //[Export(typeof(ITextStructureNavigatorProvider))]
    [ContentType(Constants.JavaContentType)]
    public sealed class JavaTextStructureNavigatorProvider : ITextStructureNavigatorProvider
    {
        [Import]
        public IContentTypeRegistryService contentTypeRegistry;

        public ITextStructureNavigator CreateTextStructureNavigator(ITextBuffer textBuffer)
        {
            Func<JavaTextStructureNavigator> factory = () => new JavaTextStructureNavigator(textBuffer, contentTypeRegistry);
            return textBuffer.Properties.GetOrCreateSingletonProperty<JavaTextStructureNavigator>(factory);
        }
    }
}
