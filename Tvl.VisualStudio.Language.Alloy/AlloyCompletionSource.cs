namespace Tvl.VisualStudio.Language.Alloy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Operations;
    using Tvl.VisualStudio.Language.Alloy.IntellisenseModel;
    using Tvl.VisualStudio.Language.Intellisense;
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

    internal partial class AlloyCompletionSource : CompletionSource
    {
        private static readonly Regex IdentifierRegex = new Regex("^[A-Za-z_][A-Za-z_']*(/[A-Za-z_][A-Za-z_']*)*$", RegexOptions.Compiled);

        public AlloyCompletionSource(ITextBuffer textBuffer, AlloyCompletionSourceProvider provider)
            : base(textBuffer, provider, AlloyConstants.AlloyLanguageGuid)
        {
        }

        public new AlloyCompletionSourceProvider Provider
        {
            get
            {
                return (AlloyCompletionSourceProvider)base.Provider;
            }
        }

        public ITextStructureNavigatorSelectorService TextStructureNavigatorSelectorService
        {
            get
            {
                return Provider.TextStructureNavigatorSelectorService;
            }
        }

        public override IEnumerable<string> Keywords
        {
            get
            {
                return AlloyClassifier.Keywords;
            }
        }

        public override void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
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

                IEnumerable<Completion> context = GetContextCompletions(triggerPoint.GetPoint(snapshot), (AlloyIntellisenseController)controller, session);
                IEnumerable<Completion> keywords = GetKeywordCompletions();
                IEnumerable<Completion> snippets = GetSnippetCompletions();
                //List<Completion> signatures = GetSignatureCompletions();
                //List<Completion> relations = GetRelationCompletions();
                //List<Completion> predicates = GetPredicateCompletions();
                //List<Completion> functions = GetFunctionCompletions();
                //SnapshotSpan? Provider.IntellisenseCache.GetExpressionSpan(triggerPoint.GetPoint(snapshot));

                IEnumerable<Completion> completions = context.Concat(keywords).Concat(snippets);
                IEnumerable<Completion> orderedCompletions = completions.Distinct(CompletionDisplayNameComparer.CurrentCulture).OrderBy(i => i.DisplayText, StringComparer.CurrentCultureIgnoreCase);

                CompletionSet completionSet = new CompletionSet("AlloyCompletions", "Alloy Completions", applicableTo, orderedCompletions, EmptyCompletions);
                completionSets.Add(completionSet);
            }
        }

        private List<Completion> GetContextCompletions(SnapshotPoint triggerPoint, AlloyIntellisenseController controller, ICompletionSession session)
        {
            // Alloy has the strange property that almost any globally visible token can be used throughout an expression (work out the subtle details later).
            Element element;
            if (!Provider.IntellisenseCache.TryResolveContext(AlloyIntellisenseCache.AlloyPositionReference.FromSnapshotPoint(triggerPoint), out element))
                return new List<Completion>();

            List<Element> scopedDeclarations = element.GetScopedDeclarations().ToList();
            List<Completion> completions = new List<Completion>();
            foreach (var decl in scopedDeclarations)
            {
                if (decl == null)
                    continue;

                Completion completion = decl.CreateCompletion(controller, session);
                if (completion != null)
                    completions.Add(completion);
            }

            return completions;
        }

        private static bool IsCompletionPrefix(TextExtent extent)
        {
            string text = extent.Span.GetText();
            if (string.IsNullOrEmpty(text))
                return false;

            return IdentifierRegex.IsMatch(text);
        }

        private static IntellisenseController GetControllerForView(ITextView view)
        {
            object controllerList;
            if (!view.Properties.TryGetProperty(typeof(ITvlIntellisenseController), out controllerList))
                return null;

            IEnumerable<ITvlIntellisenseController> controllers = controllerList as IEnumerable<ITvlIntellisenseController>;
            if (controllers == null)
                return null;

            return controllers.OfType<AlloyIntellisenseController>().SingleOrDefault();
        }
    }
}
