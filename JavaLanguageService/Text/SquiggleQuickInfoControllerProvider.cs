namespace JavaLanguageService.Text
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
    public sealed class SquiggleQuickInfoControllerProvider : IIntellisenseControllerProvider
    {
        [Import]
        public IQuickInfoBroker QuickInfoBroker;

        [Import]
        public IViewTagAggregatorFactoryService TagAggregatorFactoryService;

        public IIntellisenseController TryCreateIntellisenseController(ITextView textView, IList<ITextBuffer> subjectBuffers)
        {
            Func<TagQuickInfoController<SquiggleTag>> creator =
                () =>
                {
                    var tagAggregator = TagAggregatorFactoryService.CreateTagAggregator<SquiggleTag>(textView);
                    var controller = new TagQuickInfoController<SquiggleTag>(QuickInfoBroker, textView, tagAggregator);
                    return controller;
                };

            return textView.Properties.GetOrCreateSingletonProperty(creator);
        }
    }
}
