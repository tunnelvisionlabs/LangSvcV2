namespace Tvl.VisualStudio.Language.Alloy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;

    internal class AlloyIntellisenseTagger : ITagger<AlloyIntellisenseTag>
    {
        private readonly ITextBuffer _textBuffer;
        private readonly AlloyIntellisenseTaggerProvider _provider;
        private readonly ITagAggregator<ClassificationTag> _classificationTagAggregator;

        private readonly List<SnapshotSpan> _openBraces = new List<SnapshotSpan>();
        private readonly List<SnapshotSpan> _closeBraces = new List<SnapshotSpan>();
        private bool _initialized;

        public AlloyIntellisenseTagger(ITextBuffer textBuffer, AlloyIntellisenseTaggerProvider provider)
        {
            if (textBuffer == null)
                throw new ArgumentNullException("textBuffer");
            if (provider == null)
                throw new ArgumentNullException("provider");

            _textBuffer = textBuffer;
            _provider = provider;
            _classificationTagAggregator = _provider.BufferTagAggregatorFactoryService.CreateTagAggregator<ClassificationTag>(textBuffer);
            _classificationTagAggregator.BatchedTagsChanged += HandleClassificationTagsChanged;
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public ITextBuffer TextBuffer
        {
            get
            {
                return _textBuffer;
            }
        }

        public AlloyIntellisenseTaggerProvider Provider
        {
            get
            {
                return _provider;
            }
        }

        public ITagAggregator<ClassificationTag> ClassificationTagAggregator
        {
            get
            {
                return _classificationTagAggregator;
            }
        }

        /*
         * To delineate expressions, the following tags are relevant
         *  1. open/close brace (block expression)
         *  2. 'let' and the quantifiers (followed by blockOrBar which, in particular,
         *     means the following block is contained in the current expression).
         *  3. leading expression tokens
         *      'let'
         *      'all'
         *      'no'
         *      'some'
         *      'lone'
         *      'one'
         *      'sum'
         *      'set'
         *      'seq'
         *      '!', 'not'
         *      '+', '-'
         *      '#'
         *      '~', '*', '^'
         *      'none'
         *      'iden'
         *      'univ'
         *      'Int'
         *      'seq/Int'
         *      '('
         *      '@'
         *      'this'
         *      IDENTIFIER
         *      INTEGER
         *      '{'
         *  4. trailing expression tokens
         *      'none'
         *      'iden'
         *      'univ'
         *      'Int'
         *      'seq/Int'
         *      ']'
         *      ')'
         *      'this'
         *      IDENTIFIER
         *      INTEGER
         *      '}'
         *  5. inner expression tokens (which cannot lead or trail)
         *      '/'
         *      '|'
         *      '||', 'or'
         *      '<=>', 'iff'
         *      '=>', 'implies'
         *      'else', ','
         *      '&&', 'and'
         *      '<', '>', '=<', '>=', '=', 'in'
         *      '<<', '>>', '>>>'
         *      '++'
         *      '&'
         *      '->'
         *      '<:'
         *      ':>'
         *      '['
         *      '.'
         *      'disj', ':'
         */

        public IEnumerable<ITagSpan<AlloyIntellisenseTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (!_initialized)
            {
                ITextSnapshot snapshot = TextBuffer.CurrentSnapshot;
                SnapshotSpan documentSpan = new SnapshotSpan(snapshot, 0, snapshot.Length);
                var tags = ClassificationTagAggregator.GetTags(documentSpan);
                //var openBraces = tags.Where(i => i.Tag.ClassificationType.c
                _initialized = true;
            }

            //SnapshotSpan span;span.TranslateTo(null, SpanTrackingMode.
            return Enumerable.Empty<ITagSpan<AlloyIntellisenseTag>>();
        }

        private void HandleClassificationTagsChanged(object sender, BatchedTagsChangedEventArgs e)
        {
            if (!_initialized)
            {
                var snapshot = TextBuffer.CurrentSnapshot;
                OnTagsChanged(new SnapshotSpanEventArgs(new SnapshotSpan(snapshot, 0, snapshot.Length)));
            }
            else
            {
            }
        }

        private void OnTagsChanged(SnapshotSpanEventArgs e)
        {
            var t = TagsChanged;
            if (t != null)
                t(this, e);
        }
    }
}
