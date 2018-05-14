namespace Tvl.VisualStudio.Language.Intellisense
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Tvl.VisualStudio.Shell;

    using ImageSource = System.Windows.Media.ImageSource;

    public abstract class CompletionSource : ICompletionSource
    {
        protected static readonly string[] EmptyKeywords = new string[0];
        protected static readonly Completion[] EmptyCompletions = new Completion[0];

        private readonly ITextBuffer _textBuffer;
        private readonly CompletionSourceProvider _provider;
        private readonly Guid _languageGuid;
        private readonly List<Completion> _keywordCompletions = new List<Completion>();

        public CompletionSource([NotNull] ITextBuffer textBuffer, [NotNull] CompletionSourceProvider provider, Guid languageGuid)
        {
            Requires.NotNull(textBuffer, nameof(textBuffer));
            Requires.NotNull(provider, nameof(provider));

            _textBuffer = textBuffer;
            _provider = provider;
            _languageGuid = languageGuid;
        }

        [NotNull]
        public ITextBuffer TextBuffer
        {
            get
            {
                return _textBuffer;
            }
        }

        public Guid LanguageGuid
        {
            get
            {
                return _languageGuid;
            }
        }

        [NotNull]
        public virtual IEnumerable<string> Keywords
        {
            get
            {
                return EmptyKeywords;
            }
        }

        [NotNull]
        protected CompletionSourceProvider Provider
        {
            get
            {
                return _provider;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public abstract void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets);

        protected virtual void Dispose(bool disposing)
        {
        }

        protected virtual IEnumerable<Completion> GetKeywordCompletions()
        {
            if (_keywordCompletions.Count == 0)
            {
                _keywordCompletions.AddRange(Keywords.Select(CreateKeywordCompletion));
            }

            return _keywordCompletions;
        }

        [NotNull]
        protected virtual Completion CreateKeywordCompletion([NotNull] string keyword)
        {
            Requires.NotNullOrEmpty(keyword, nameof(keyword));

            string displayText = keyword;
            string insertionText = keyword;
            string description = null;
            ImageSource iconSource = Provider.GlyphService.GetGlyph(StandardGlyphGroup.GlyphKeyword, StandardGlyphItem.GlyphItemPublic);
            string iconAutomationText = new IconDescription(StandardGlyphGroup.GlyphKeyword, StandardGlyphItem.GlyphItemPublic).ToString();
            return new Completion(displayText, insertionText, description, iconSource, iconAutomationText);
        }

        [NotNull]
        protected virtual IEnumerable<Completion> GetSnippetCompletions()
        {
            if (LanguageGuid == Guid.Empty)
                return EmptyCompletions;

            List<Completion> snippetCompletions = new List<Completion>();
            Guid languageGuid = LanguageGuid;
            VsExpansion[] expansions = Provider.ExpansionManager.EnumerateExpansions(languageGuid, new string[] { "Expansion" }, false);
            ImageSource defaultExpansionGlyph = Provider.GlyphService.GetGlyph(StandardGlyphGroup.GlyphCSharpExpansion, StandardGlyphItem.GlyphItemPublic);
            IconDescription defaultIconDescription = new IconDescription(StandardGlyphGroup.GlyphCSharpExpansion, StandardGlyphItem.GlyphItemPublic);
            foreach (var expansion in expansions)
            {
                Completion completion = CreateSnippetCompletion(expansion, defaultExpansionGlyph, defaultIconDescription);
                if (completion == null)
                    continue;

                snippetCompletions.Add(completion);
            }

            return snippetCompletions;
        }

        protected virtual Completion CreateSnippetCompletion(VsExpansion expansion, ImageSource defaultExpansionGlyph, IconDescription defaultIconDescription)
        {
            if (string.IsNullOrEmpty(expansion.shortcut))
                return null;

            string displayText = expansion.shortcut;
            string insertionText = expansion.shortcut;
            string description = expansion.description;
            return new Completion(displayText, insertionText, description, defaultExpansionGlyph, defaultIconDescription.ToString());
        }
    }
}
