namespace Tvl.VisualStudio.InheritanceMargin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using System.Diagnostics.Contracts;
    using Tvl.VisualStudio.Language.Parsing;

    public class CSharpInheritanceTagger : ITagger<InheritanceTag>
    {
        internal static readonly ITagSpan<InheritanceTag>[] NoTags = new ITagSpan<InheritanceTag>[0];

        private readonly CSharpInheritanceTaggerProvider _provider;
        private readonly ITextView _textView;
        private readonly ITextBuffer _buffer;
        private readonly CSharpInheritanceAnalyzer _analyzer;

        private ITagSpan<InheritanceTag>[] _tags = NoTags;

        public CSharpInheritanceTagger(CSharpInheritanceTaggerProvider provider, ITextView textView, ITextBuffer buffer)
        {
            Contract.Requires<ArgumentNullException>(provider != null, "provider");
            Contract.Requires<ArgumentNullException>(textView != null, "textView");
            Contract.Requires<ArgumentNullException>(buffer != null, "buffer");

            this._provider = provider;
            this._textView = textView;
            this._buffer = buffer;
            this._analyzer = new CSharpInheritanceAnalyzer(buffer, provider.TaskScheduler, provider.TextDocumentFactoryService, provider.OutputWindowService, provider.GlobalServiceProvider);
            this._analyzer.ParseComplete += HandleParseComplete;
            this._analyzer.RequestParse(false);
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        internal static CSharpInheritanceTagger CreateInstance(CSharpInheritanceTaggerProvider provider, ITextView textView, ITextBuffer buffer)
        {
            Contract.Requires<ArgumentNullException>(provider != null, "provider");
            Contract.Requires<ArgumentNullException>(textView != null, "textView");
            Contract.Requires<ArgumentNullException>(buffer != null, "buffer");

            return textView.Properties.GetOrCreateSingletonProperty(() => new CSharpInheritanceTagger(provider, textView, buffer));
        }

        public IEnumerable<ITagSpan<InheritanceTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            return _tags;
        }

        protected virtual void HandleParseComplete(object sender, ParseResultEventArgs e)
        {
            IEnumerable<ITagSpan<InheritanceTag>> tags = NoTags;

            InheritanceParseResultEventArgs ie = e as InheritanceParseResultEventArgs;
            if (ie != null)
            {
                tags = ie.Tags;
            }

            _tags = tags.ToArray();
            OnTagsChanged(new SnapshotSpanEventArgs(new SnapshotSpan(e.Snapshot, new Span(0, e.Snapshot.Length))));
        }

        protected virtual void OnTagsChanged(SnapshotSpanEventArgs e)
        {
            var t = TagsChanged;
            if (t != null)
                t(this, e);
        }
    }
}
