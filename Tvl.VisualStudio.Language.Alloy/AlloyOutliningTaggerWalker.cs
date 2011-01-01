namespace Tvl.VisualStudio.Language.Alloy
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Antlr.Runtime;
    using Antlr.Runtime.Tree;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;

    internal class AlloyOutliningTaggerWalker : AlloyBaseWalker
    {
        private readonly List<ITagSpan<IOutliningRegionTag>> _outliningRegions = new List<ITagSpan<IOutliningRegionTag>>();
        private readonly ReadOnlyCollection<IToken> _tokens;
        private readonly AlloyOutliningTaggerProvider _provider;
        private readonly ITextSnapshot _snapshot;

        private AlloyOutliningTaggerWalker(ITreeNodeStream input, ReadOnlyCollection<IToken> tokens, AlloyOutliningTaggerProvider provider, ITextSnapshot snapshot)
            : base(input, snapshot, provider.OutputWindowService)
        {
            _tokens = tokens;
            _provider = provider;
            _snapshot = snapshot;
        }

        public static List<ITagSpan<IOutliningRegionTag>> ExtractOutliningRegions(AlloyParser.compilationUnit_return parseResult, ReadOnlyCollection<IToken> tokens, AlloyOutliningTaggerProvider provider, ITextSnapshot snapshot)
        {
            BufferedTreeNodeStream input = new BufferedTreeNodeStream(parseResult.Tree);
            AlloyOutliningTaggerWalker walker = new AlloyOutliningTaggerWalker(input, tokens, provider, snapshot);
            walker.compilationUnit();

            return walker._outliningRegions;
        }

        protected override void HandleAssert(CommonTree assert, CommonTree name, CommonTree body)
        {
            OutlineBlock(body);
        }

        protected override void HandleEnum(CommonTree @enum, CommonTree name, CommonTree body)
        {
            OutlineBlock(body);
        }

        protected override void HandleFact(CommonTree fact, CommonTree name, CommonTree body)
        {
            OutlineBlock(body);
        }

        protected override void HandleFunction(CommonTree function, CommonTree name, bool isPrivate, IList<CommonTree> parameters, CommonTree returnSpec, CommonTree body)
        {
            OutlineBlock(body);
        }

        protected override void HandlePredicate(CommonTree function, CommonTree name, bool isPrivate, IList<CommonTree> parameters, CommonTree body)
        {
            OutlineBlock(body);
        }

        protected override void HandleSignature(CommonTree signature, IList<IToken> qualifiers, IList<CommonTree> names, CommonTree extendsSpec, CommonTree body, CommonTree block)
        {
            OutlineBlock(body);
            OutlineBlock(block);
        }

        private void OutlineBlock(CommonTree subchild)
        {
            if (subchild == null || subchild.Type != LBRACE)
                return;

            var startToken = _tokens[subchild.TokenStartIndex];
            var stopToken = _tokens[subchild.TokenStopIndex];
            Span span = new Span(startToken.StartIndex, stopToken.StopIndex - startToken.StartIndex + 1);
            SnapshotSpan snapshotSpan = new SnapshotSpan(_snapshot, span);
            IOutliningRegionTag tag = new OutliningRegionTag();
            TagSpan<IOutliningRegionTag> tagSpan = new TagSpan<IOutliningRegionTag>(snapshotSpan, tag);
            _outliningRegions.Add(tagSpan);
        }
    }
}
