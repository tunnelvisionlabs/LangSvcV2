namespace Tvl.VisualStudio.Language.Alloy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Operations;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Tvl.VisualStudio.Language.Intellisense;
    using Tvl.VisualStudio.Shell.Extensions;
    using ImageSource = System.Windows.Media.ImageSource;
    using Regex = System.Text.RegularExpressions.Regex;
    using RegexOptions = System.Text.RegularExpressions.RegexOptions;

    /*
     * Establishing identifier visibility scopes.
     *
     *  Need a way to express
     *      "A parameter is visible until the following occurs in order:
     *          1. The parentheses level (PL) drops from the level at the declaration point by 1.
     *          2. Exactly one of the following occurs:
     *              a. The brace level increases by one, then returns to the current level.
     *              b. The brace level decreases by one.
     *              c. The end of the file is reached.
     *              d. (Not relevant to alloy, but is for other languages) A semicolon is reached.
     *      "A local variable is visible from the declaration point until:
     *          1. The brace level decreases by one.
     */

    internal partial class AlloyCompletionSource : ICompletionSource
    {
        private static readonly Regex IdentifierRegex = new Regex("^[A-Za-z_][A-Za-z_']*(/[A-Za-z_][A-Za-z_']*)*$", RegexOptions.Compiled);
        private static readonly List<Completion> _keywordCompletions = new List<Completion>();
        private static readonly Completion[] EmptyCompletions = new Completion[0];

        private readonly ITextBuffer _textBuffer;
        private readonly AlloyCompletionSourceProvider _provider;

        public AlloyCompletionSource(ITextBuffer textBuffer, AlloyCompletionSourceProvider provider)
        {
            this._textBuffer = textBuffer;
            this._provider = provider;
        }

        public ITextBuffer TextBuffer
        {
            get
            {
                return _textBuffer;
            }
        }

        public AlloyCompletionSourceProvider Provider
        {
            get
            {
                return _provider;
            }
        }

        public ITextStructureNavigatorSelectorService TextStructureNavigatorSelectorService
        {
            get
            {
                return Provider.TextStructureNavigatorSelectorService;
            }
        }

        public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            if (session == null || completionSets == null)
                return;

            ITrackingPoint triggerPoint = session.GetTriggerPoint(TextBuffer);
            if (triggerPoint != null)
            {
                IntellisenseController controller = GetControllerForView(session.TextView);
                CompletionInfo completionInfo = controller.CompletionInfo;
                ITextSnapshot snapshot = triggerPoint.TextBuffer.CurrentSnapshot;
                SnapshotPoint point = triggerPoint.GetPoint(snapshot);
                ITrackingPoint point2 = triggerPoint;
                bool flag = false;
                bool extend = true;

                IntellisenseInvocationType invocationType = completionInfo.InvocationType;
                CompletionInfoType infoType = completionInfo.InfoType;

                switch (invocationType)
                {
                case IntellisenseInvocationType.Default:
                    extend = infoType == CompletionInfoType.GlobalInfo;
                    break;

                case IntellisenseInvocationType.BackspaceDeleteOrBackTab:
                case IntellisenseInvocationType.IdentifierChar:
                case IntellisenseInvocationType.Sharp:
                case IntellisenseInvocationType.Space:
                case IntellisenseInvocationType.ShowMemberList:
                    break;

                default:
                    flag = true;
                    break;
                }

                TextExtent extentOfWord = default(TextExtent);
                if (extend)
                {
                    ITextBuffer textBuffer = TextBuffer;
                    ITextStructureNavigator navigator = TextStructureNavigatorSelectorService.CreateTextStructureNavigator(textBuffer, textBuffer.ContentType);
                    SnapshotPoint currentPosition = new SnapshotPoint(snapshot, point2.GetPosition(snapshot));
                    extentOfWord = navigator.GetExtentOfWord(currentPosition);
                    if (extentOfWord.Span.Start == point)
                    {
                        TextExtent extentOfPreviousWord = navigator.GetExtentOfWord(currentPosition - 1);
                        if (extentOfPreviousWord.IsSignificant && extentOfPreviousWord.Span.End == point && IsCompletionPrefix(extentOfPreviousWord))
                            extentOfWord = extentOfPreviousWord;
                        else
                            extend = false;
                    }
                }

                if (!extend || !extentOfWord.IsSignificant)
                {
                    SnapshotSpan span = new SnapshotSpan(point, 0);
                    extentOfWord = new TextExtent(span, false);
                }

                if (invocationType == IntellisenseInvocationType.BackspaceDeleteOrBackTab && extentOfWord.Span.Length > 0)
                {
                    string str3 = snapshot.GetText(extentOfWord.Span);
                    if (!string.IsNullOrWhiteSpace(str3))
                    {
                        while ("!^()=<>\\:;.,+-*/{}\" '&%@?".IndexOf(str3[0]) > 0)
                        {
                            SnapshotSpan span2 = extentOfWord.Span;
                            SnapshotSpan span3 = new SnapshotSpan(snapshot, span2.Start + 1, span2.Length - 1);
                            extentOfWord = new TextExtent(span3, false);
                            str3 = snapshot.GetText(extentOfWord.Span);
                            if (string.IsNullOrEmpty(str3))
                                break;
                        }
                    }
                    else
                    {
                        SnapshotSpan span4 = new SnapshotSpan(snapshot, extentOfWord.Span.End, 0);
                        extentOfWord = new TextExtent(span4, false);
                        completionInfo.InvocationType = IntellisenseInvocationType.Default;
                    }
                }

                ITrackingSpan applicableTo = snapshot.CreateTrackingSpan(extentOfWord.Span, SpanTrackingMode.EdgeInclusive, TrackingFidelityMode.Forward);
                if (flag)
                {
                    SnapshotSpan textSoFarSpan = new SnapshotSpan(snapshot, extentOfWord.Span.Start, triggerPoint.GetPoint(snapshot));
                    string textSoFar = textSoFarSpan.GetText();
                    applicableTo = snapshot.CreateTrackingSpan(point.Position - textSoFar.Length, textSoFar.Length, SpanTrackingMode.EdgeInclusive, TrackingFidelityMode.Forward);
                }

                List<Completion> keywords = GetOrCreateKeywordCompletions(Provider.GlyphService);
                List<Completion> snippets = GetSnippetCompletions(Provider.ExpansionManager, Provider.GlyphService);

                IEnumerable<Completion> completions = keywords.Concat(snippets);
                IEnumerable<Completion> orderedCompletions = completions.Distinct(CompletionDisplayNameComparer.CurrentCulture).OrderBy(i => i.DisplayText, StringComparer.CurrentCultureIgnoreCase);

                CompletionSet completionSet = new CompletionSet("AlloyCompletions", "Alloy Completions", applicableTo, orderedCompletions, EmptyCompletions);
                completionSets.Add(completionSet);
            }
        }

        private static bool IsCompletionPrefix(TextExtent extent)
        {
            string text = extent.Span.GetText();
            if (string.IsNullOrEmpty(text))
                return false;

            return IdentifierRegex.IsMatch(text);
        }

        public void Dispose()
        {
        }

        private static IntellisenseController GetControllerForView(ITextView view)
        {
            object controllerList;
            if (!view.Properties.TryGetProperty(typeof(IntellisenseController), out controllerList))
                return null;

            IEnumerable<IntellisenseController> controllers = controllerList as IEnumerable<IntellisenseController>;
            if (controllers == null)
                return null;

            return controllers.OfType<AlloyIntellisenseController>().SingleOrDefault();
        }

        private static List<Completion> GetOrCreateKeywordCompletions(IGlyphService glyphService)
        {
            if (_keywordCompletions.Count == 0)
            {
                _keywordCompletions.AddRange(AlloyClassifier.Keywords.Select(i => CreateKeywordCompletion(i, glyphService)));
            }

            return _keywordCompletions;
        }

        private static List<Completion> GetSnippetCompletions(IVsExpansionManager expansionManager, IGlyphService glyphService)
        {
            List<Completion> snippetCompletions = new List<Completion>();
            Guid languageGuid = AlloyConstants.AlloyLanguageGuid;
            VsExpansion[] expansions = expansionManager.EnumerateExpansions(languageGuid, new string[] { "Expansion" }, false);
            ImageSource iconSource = glyphService.GetGlyph(StandardGlyphGroup.GlyphCSharpExpansion, StandardGlyphItem.GlyphItemPublic);
            string iconAutomationText = new IconDescription(StandardGlyphGroup.GlyphCSharpExpansion, StandardGlyphItem.GlyphItemPublic).ToString();
            foreach (var expansion in expansions)
            {
                if (string.IsNullOrEmpty(expansion.shortcut))
                    continue;

                string displayText = expansion.shortcut;
                string insertionText = expansion.shortcut;
                string description = expansion.description;
                snippetCompletions.Add(new Completion(displayText, insertionText, description, iconSource, iconAutomationText));
            }

            return snippetCompletions;
        }

        private static Completion CreateKeywordCompletion(string keyword, IGlyphService glyphService)
        {
            string displayText = keyword;
            string insertionText = keyword;
            string description = null;
            ImageSource iconSource = glyphService.GetGlyph(StandardGlyphGroup.GlyphKeyword, StandardGlyphItem.GlyphItemPublic);
            string iconAutomationText = new IconDescription(StandardGlyphGroup.GlyphKeyword, StandardGlyphItem.GlyphItemPublic).ToString();
            return new Completion(displayText, insertionText, description, iconSource, iconAutomationText);
        }
    }
}
