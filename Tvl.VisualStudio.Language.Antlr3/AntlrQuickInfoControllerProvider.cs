namespace Tvl.VisualStudio.Language.Antlr3
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Text;

    [Export(typeof(IIntellisenseControllerProvider))]
    [ContentType(AntlrConstants.AntlrContentType)]
    public sealed class AntlrQuickInfoControllerProvider : IIntellisenseControllerProvider
    {
        [Import]
        private IQuickInfoBroker QuickInfoBroker
        {
            get;
            set;
        }

        [Import]
        private IViewTagAggregatorFactoryService TagAggregatorFactoryService
        {
            get;
            set;
        }

        public IIntellisenseController TryCreateIntellisenseController(ITextView textView, IList<ITextBuffer> subjectBuffers)
        {
            Func<TagQuickInfoController<ClassificationTag>> creator =
                () =>
                {
                    var tagAggregator = TagAggregatorFactoryService.CreateTagAggregator<ClassificationTag>(textView);
                    var controller = new TagQuickInfoController<ClassificationTag>(QuickInfoBroker, textView, tagAggregator);
                    return controller;
                };

            return textView.Properties.GetOrCreateSingletonProperty(typeof(AntlrQuickInfoControllerProvider), creator);
        }
    }
}
