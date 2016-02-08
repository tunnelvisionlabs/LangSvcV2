namespace Tvl.VisualStudio.Language.StringTemplate4
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
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

        public StringTemplateBraceMatchingTagger(ITextView textView, ITextBuffer sourceBuffer, IClassifier aggregator)
            : base(textView, sourceBuffer, aggregator, _matchingCharacters)
        {
            Contract.Requires<ArgumentNullException>(textView != null, "textView");
            Contract.Requires<ArgumentNullException>(sourceBuffer != null, "sourceBuffer");
            Contract.Requires<ArgumentNullException>(aggregator != null, "aggregator");
        }

        protected override bool IsClassificationTypeIgnored(IClassificationType classificationType)
        {
            return classificationType.IsOfType(PredefinedClassificationTypeNames.Comment);
        }
    }
}
