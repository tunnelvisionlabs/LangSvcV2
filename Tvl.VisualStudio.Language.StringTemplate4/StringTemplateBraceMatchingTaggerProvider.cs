namespace Tvl.VisualStudio.Language.StringTemplate4
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(IViewTaggerProvider))]
    [ContentType(StringTemplateConstants.StringTemplateContentType)]
    [TagType(typeof(TextMarkerTag))]
    public sealed class StringTemplateBraceMatchingTaggerProvider : IViewTaggerProvider
    {
        [Import]
        private IClassifierAggregatorService AggregatorService
        {
            get;
            set;
        }

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            if (textView == null)
                return null;

            var aggregator = AggregatorService.GetClassifier(buffer);
            return new StringTemplateBraceMatchingTagger(textView, buffer, aggregator) as ITagger<T>;
        }
    }
}
