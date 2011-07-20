namespace Tvl.VisualStudio.Language.Alloy.Experimental
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Antlr.Runtime;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Text.Tagging;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Shell.OutputWindow;

    internal sealed class AlloySymbolTagger : BackgroundParser, ITagger<IClassificationTag>
    {
        private readonly IClassificationTypeRegistryService _classificationTypeRegistryService;

        private List<ITagSpan<IClassificationTag>> _tags = new List<ITagSpan<IClassificationTag>>();

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public AlloySymbolTagger(ITextBuffer textBuffer, IClassificationTypeRegistryService classificationTypeRegistryService, TaskScheduler taskScheduler, ITextDocumentFactoryService textDocumentFactoryService, IOutputWindowService outputWindowService)
            : base(textBuffer, taskScheduler, textDocumentFactoryService, outputWindowService)
        {
            _classificationTypeRegistryService = classificationTypeRegistryService;
        }

        public IEnumerable<ITagSpan<IClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            return _tags;
        }

        private void OnTagsChanged(SnapshotSpanEventArgs e)
        {
            var t = TagsChanged;
            if (t != null)
                t(this, e);
        }

        protected override void ReParseImpl()
        {
            // lex the entire document to get the set of identifiers we'll need to classify
            ITextSnapshot snapshot = TextBuffer.CurrentSnapshot;
            var input = new SnapshotCharStream(snapshot, new Span(0, snapshot.Length));
            var lexer = new AlloyLexer(input);
            var tokens = new CommonTokenStream(lexer);
            tokens.Fill();

            /* easy to handle the following definitions:
             *  - module (name)
             *  - open (external symbol reference) ... as (name)
             *  - fact (name)?
             *  - assert (name)?
             *  - fun (ref.name | name)
             *  - pred (ref.name | name)
             *  - (name): run|check
             *  - sig (namelist)
             *  - enum (name)
             * moderate to handle the following definitions:
             *  - decl name(s)
             * harder to handle the following definitions:
             */

            /* A single name follows the following keywords:
             *  - KW_MODULE
             *  - KW_OPEN
             *  - KW_AS
             *  - KW_ENUM
             *  - KW_FACT (name is optional)
             *  - KW_ASSERT (name is optional)
             */
            List<IToken> singleNameKeywords = new List<IToken>();
            List<IToken> qualifiedNameKeywords = new List<IToken>();
            List<IToken> declColons = new List<IToken>();

            List<IToken> identifiers = new List<IToken>();
            while (tokens.LA(1) != CharStreamConstants.EndOfFile)
            {
                switch (tokens.LA(1))
                {
                case AlloyLexer.IDENTIFIER:
                    identifiers.Add(tokens.LT(1));
                    break;

                case AlloyLexer.KW_MODULE:
                case AlloyLexer.KW_OPEN:
                case AlloyLexer.KW_AS:
                case AlloyLexer.KW_ENUM:
                case AlloyLexer.KW_FACT:
                case AlloyLexer.KW_ASSERT:
                case AlloyLexer.KW_RUN:
                case AlloyLexer.KW_CHECK:
                    singleNameKeywords.Add(tokens.LT(1));
                    break;

                case AlloyLexer.KW_FUN:
                case AlloyLexer.KW_PRED:
                    qualifiedNameKeywords.Add(tokens.LT(1));
                    break;

                case AlloyLexer.COLON:
                    declColons.Add(tokens.LT(1));
                    break;

                case CharStreamConstants.EndOfFile:
                    goto doneLexing;

                default:
                    break;
                }

                tokens.Consume();
            }

        doneLexing:

            HashSet<IToken> definitions = new HashSet<IToken>(TokenIndexEqualityComparer.Default);
            HashSet<IToken> references = new HashSet<IToken>(TokenIndexEqualityComparer.Default);

            foreach (var token in singleNameKeywords)
            {
                tokens.Seek(token.TokenIndex);
                tokens.Consume();
                List<IToken> nameTokens = ReadName(tokens);

                switch (token.Type)
                {
                case AlloyLexer.KW_RUN:
                case AlloyLexer.KW_CHECK:
                    references.UnionWith(nameTokens);
                    break;

                default:
                    definitions.UnionWith(nameTokens);
                    break;
                }
            }

            foreach (var token in qualifiedNameKeywords)
            {
                tokens.Seek(token.TokenIndex);
                tokens.Consume();
                List<IToken> refTokens = null;
                List<IToken> nameTokens = ReadName(tokens);

                if (tokens.LA(1) == AlloyLexer.DOT)
                {
                    refTokens = nameTokens;
                    tokens.Consume();
                    nameTokens = ReadName(tokens);
                }

                if (refTokens != null)
                    references.UnionWith(refTokens);

                definitions.UnionWith(nameTokens);
            }

            foreach (var token in declColons)
            {
                // have to read names in reverse
                tokens.Seek(token.TokenIndex);

                while (true)
                {
                    tokens.Seek(tokens.LT(1).TokenIndex);
                    List<IToken> nameTokens = ReadNameReverse(tokens);
                    definitions.UnionWith(nameTokens);
                    switch (tokens.LA(-1))
                    {
                    case AlloyLexer.COMMA:
                    //case AlloyLexer.IDENTIFIER:
                    //case AlloyLexer.KW_THIS:
                        tokens.Seek(tokens.LT(-1).TokenIndex);
                        continue;

                    default:
                        break;
                    }

                    break;
                }
            }

            HashSet<IToken> unknownIdentifiers = new HashSet<IToken>(identifiers, TokenIndexEqualityComparer.Default);
            unknownIdentifiers.ExceptWith(definitions);
            unknownIdentifiers.ExceptWith(references);

            List<ITagSpan<IClassificationTag>> tags = new List<ITagSpan<IClassificationTag>>();

            IClassificationType definitionClassificationType = _classificationTypeRegistryService.GetClassificationType(AlloySymbolTaggerClassificationTypeNames.Definition);
            tags.AddRange(ClassifyTokens(snapshot, definitions, new ClassificationTag(definitionClassificationType)));

            IClassificationType referenceClassificationType = _classificationTypeRegistryService.GetClassificationType(AlloySymbolTaggerClassificationTypeNames.Reference);
            tags.AddRange(ClassifyTokens(snapshot, references, new ClassificationTag(referenceClassificationType)));

            IClassificationType unknownClassificationType = _classificationTypeRegistryService.GetClassificationType(AlloySymbolTaggerClassificationTypeNames.UnknownIdentifier);
            tags.AddRange(ClassifyTokens(snapshot, unknownIdentifiers, new ClassificationTag(unknownClassificationType)));

            _tags = tags;

            OnTagsChanged(new SnapshotSpanEventArgs(new SnapshotSpan(snapshot, new Span(0, snapshot.Length))));
        }

        private static IEnumerable<ITagSpan<IClassificationTag>> ClassifyTokens(ITextSnapshot snapshot, IEnumerable<IToken> tokens, IClassificationTag classificationTag)
        {
            foreach (var token in tokens)
            {
                SnapshotSpan span = new SnapshotSpan(snapshot, Span.FromBounds(token.StartIndex, token.StopIndex + 1));
                yield return new TagSpan<IClassificationTag>(span, classificationTag);
            }
        }

        private List<IToken> ReadName(ITokenStream tokenStream)
        {
            List<IToken> tokens = new List<IToken>();

            // 'this' | IDENTIFIER
            switch (tokenStream.LA(1))
            {
            case AlloyLexer.IDENTIFIER:
            case AlloyLexer.KW_THIS:
                tokens.Add(tokenStream.LT(1));
                tokenStream.Consume();
                break;

            default:
                return tokens;
            }

            while (true)
            {
                if (tokenStream.LA(1) != AlloyLexer.SLASH || tokenStream.LA(2) != AlloyLexer.IDENTIFIER)
                    return tokens;

                tokens.Add(tokenStream.LT(1));
                tokenStream.Consume();
                tokens.Add(tokenStream.LT(1));
                tokenStream.Consume();
            }
        }

        private List<IToken> ReadNameReverse(ITokenStream tokenStream)
        {
            List<IToken> tokens = new List<IToken>();
            int offset = -1;

            // ('this' | IDENTIFIER) ('/' IDENTIFIER)*
            while (true)
            {
                switch (tokenStream.LA(offset))
                {
                case AlloyLexer.IDENTIFIER:
                    tokens.Add(tokenStream.LT(offset));
                    if (tokenStream.LA(offset - 1) == AlloyLexer.SLASH)
                    {
                        tokens.Add(tokenStream.LT(offset - 1));
                        offset -= 2;
                        continue;
                    }

                    break;

                case AlloyLexer.KW_THIS:
                    tokens.Add(tokenStream.LT(offset));
                    break;

                default:
                    break;
                }

                break;
            }

            if (tokens.Count > 0)
                tokenStream.Seek(tokens[tokens.Count - 1].TokenIndex);

            return tokens;
        }

        private class TokenIndexEqualityComparer : IEqualityComparer<IToken>
        {
            private static readonly TokenIndexEqualityComparer _default = new TokenIndexEqualityComparer();

            public static TokenIndexEqualityComparer Default
            {
                get
                {
                    return _default;
                }
            }

            public bool Equals(IToken x, IToken y)
            {
                if (x == null)
                    return y == null;

                if (y == null)
                    return false;

                return x.TokenIndex == y.TokenIndex;
            }

            public int GetHashCode(IToken obj)
            {
                return obj.TokenIndex.GetHashCode();
            }
        }
    }
}
