namespace Tvl.VisualStudio.Language.StringTemplate4
{
    using System.Collections.Generic;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio.Language.StandardClassification;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Text.Editor;
    using Tvl.VisualStudio.Text.Tagging;

    public class StringTemplateBraceMatchingTagger : BraceMatchingTagger
    {
        private static readonly KeyValuePair<char, char>[] _matchingCharacters =
            new KeyValuePair<char, char>[]
            {
                new KeyValuePair<char, char>('(', ')'),
                new KeyValuePair<char, char>('{', '}'),
                new KeyValuePair<char, char>('[', ']'),
                new KeyValuePair<char, char>('<', '>'),
            };

        public StringTemplateBraceMatchingTagger([NotNull] ITextView textView, [NotNull] ITextBuffer sourceBuffer, [NotNull] IClassifier aggregator)
            : base(textView, sourceBuffer, aggregator, _matchingCharacters)
        {
            Requires.NotNull(textView, nameof(textView));
            Requires.NotNull(sourceBuffer, nameof(sourceBuffer));
            Requires.NotNull(aggregator, nameof(aggregator));
        }

        protected override bool IsClassificationTypeIgnored(IClassificationType classificationType)
        {
            return classificationType.IsOfType(PredefinedClassificationTypeNames.Comment);
        }
    }
}
