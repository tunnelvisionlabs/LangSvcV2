namespace Tvl.VisualStudio.Text.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(IIntellisenseControllerProvider))]
    [ContentType("text")]
    public sealed class UrlQuickInfoControllerProvider : IIntellisenseControllerProvider
    {
        [Import]
        public IQuickInfoBroker QuickInfoBroker;

        [Import]
        public IViewTagAggregatorFactoryService TagAggregatorFactoryService;

        public IIntellisenseController TryCreateIntellisenseController(ITextView textView, IList<ITextBuffer> subjectBuffers)
        {
            Func<TagQuickInfoController<IUrlTag>> creator =
                () =>
                {
                    var tagAggregator = TagAggregatorFactoryService.CreateTagAggregator<IUrlTag>(textView);
                    var controller = new TagQuickInfoController<IUrlTag>(QuickInfoBroker, textView, tagAggregator);
                    return controller;
                };

            return textView.Properties.GetOrCreateSingletonProperty(creator);
        }
    }
}
