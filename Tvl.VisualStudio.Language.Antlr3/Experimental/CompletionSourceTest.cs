namespace Tvl.VisualStudio.Language.Antlr3.Experimental
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Tvl.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Tvl.VisualStudio.Language.Antlr3.OptionsPages;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Shell.Extensions;
    using Microsoft.VisualStudio.Text.Operations;
    using GrammarType = global::Antlr3.Tool.GrammarType;
    using System.Text.RegularExpressions;

    public class CompletionSourceTest : CompletionSource, ICompletionSource
    {
        private readonly AntlrIntellisenseOptions _intellisenseOptions;

        public CompletionSourceTest(ITextBuffer textBuffer, CompletionSourceProvider provider)
            : base(textBuffer, provider, AntlrConstants.AntlrLanguageGuid)
        {
            var shell = Provider.GlobalServiceProvider.GetShell();
            var package = shell.LoadPackage<AntlrLanguagePackage>();
            _intellisenseOptions = package.IntellisenseOptions;
        }

        public ITextStructureNavigatorSelectorService TextStructureNavigatorSelectorService
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        private string CommitCharacters
        {
            get
            {
                return _intellisenseOptions.CommitCharacters ?? AntlrIntellisenseOptions.DefaultCommitCharacters;
            }
        }

        public override void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            if (session == null || completionSets == null)
                return;

            ITrackingPoint triggerPoint = session.GetTriggerPoint(TextBuffer);
            if (triggerPoint != null)
            {
                IntellisenseController controller = null;
                CompletionInfo completionInfo = controller.CompletionInfo;
                ITextSnapshot snapshot = triggerPoint.TextBuffer.CurrentSnapshot;
                SnapshotPoint point = triggerPoint.GetPoint(snapshot);
                bool extendLeft = false;
                bool extend = true;

                switch (completionInfo.InvocationType)
                {
                case IntellisenseInvocationType.Default:
                    extend = completionInfo.InfoType == CompletionInfoType.GlobalInfo;
                    break;

                case IntellisenseInvocationType.BackspaceDeleteOrBackTab:
                case IntellisenseInvocationType.IdentifierChar:
                case IntellisenseInvocationType.Sharp:
                case IntellisenseInvocationType.Space:
                case IntellisenseInvocationType.ShowMemberList:
                    break;

                default:
                    extendLeft = true;
                    break;
                }

                TextExtent extentOfWord = default(TextExtent);
                if (extend)
                {
                    ITextBuffer textBuffer = TextBuffer;
                    ITextStructureNavigator navigator = TextStructureNavigatorSelectorService.CreateTextStructureNavigator(textBuffer, textBuffer.ContentType);
                    SnapshotPoint currentPosition = new SnapshotPoint(snapshot, triggerPoint.GetPosition(snapshot));
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

                if (completionInfo.InvocationType == IntellisenseInvocationType.BackspaceDeleteOrBackTab && extentOfWord.Span.Length > 0)
                {
                    string str3 = snapshot.GetText(extentOfWord.Span);
                    if (!string.IsNullOrWhiteSpace(str3))
                    {
                        while (CommitCharacters.IndexOf(str3[0]) > 0)
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
                if (extendLeft)
                {
                    SnapshotSpan textSoFarSpan = new SnapshotSpan(snapshot, extentOfWord.Span.Start, triggerPoint.GetPoint(snapshot));
                    string textSoFar = textSoFarSpan.GetText();
                    applicableTo = snapshot.CreateTrackingSpan(point.Position - textSoFar.Length, textSoFar.Length, SpanTrackingMode.EdgeInclusive, TrackingFidelityMode.Forward);
                }

                /* Context Tree
                 * 
                 *  - Global
                 *    - Options
                 *    - Tokens
                 *    - AttributeScope
                 *      - ACTION
                 *    - Named Action
                 *      - ACTION
                 *    - Rule
                 *      - Arguments
                 *        - ARG_ACTION
                 *      - AttributeScope
                 *        - ACTION
                 *      - Named Action
                 *        - ACTION
                 *      - Options
                 *      - Alternative
                 *        - Alternative*
                 *        - Rewrite
                 *        - ACTION
                 */

                List<Completion> completions = new List<Completion>();
                List<Completion> completionBuilders = new List<Completion>();

                List<IntellisenseContext> intellisenseContexts = GetIntellisenseContexts(triggerPoint);
                foreach (var context in intellisenseContexts)
                {
                    context.AugmentCompletionSession(session, completions, completionBuilders);
                }

                string moniker = "AntlrCompletions";
                string displayName = "ANTLR Completions";
                CompletionSet completionSet = new CompletionSet(moniker, displayName, applicableTo, completions, completionBuilders);
                completionSets.Add(completionSet);
            }
        }

        /// <summary>
        /// Returns a list of all possible IntelliSense contexts at the trigger point.
        /// </summary>
        private List<IntellisenseContext> GetIntellisenseContexts(ITrackingPoint triggerPoint)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a list of IntelliSense contexts for all attribute scopes defined in the current grammar
        /// or any of its imports.
        /// </summary>
        private List<IntellisenseContext> GetAttributeScopeContexts()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a list of IntelliSense contexts for all rules defined in the current grammar
        /// or any of its imports.
        /// </summary>
        private List<IntellisenseContext> GetRuleContexts()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a GrammarOptions object for the current grammar.
        /// </summary>
        private GrammarOptions GetGrammarOptions()
        {
            throw new NotImplementedException();
        }

        private static readonly Regex IdentifierRegex = new Regex(@"^(?:(?:\$?[A-Za-z][A-Za-z0-9_]*)|\$)$", RegexOptions.Compiled);

        private bool IsCompletionPrefix(TextExtent extent)
        {
            string text = extent.Span.GetText();
            if (string.IsNullOrEmpty(text))
                return false;

            if (text == "$")
                return true;

            return IdentifierRegex.IsMatch(text);
        }

        public class GrammarOptions
        {
            /// <summary>
            /// Returns the target language for the grammar.
            /// </summary>
            public string TargetLanguage
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public GrammarType GrammarType
            {
                get
                {
                    throw new NotImplementedException();
                }
            }
        }

        public class IntellisenseContext
        {
            public static IntellisenseContext GlobalContext = new IntellisenseContext(IntellisenseContextType.Global, null);

            public readonly IntellisenseContextType ContextType;
            public readonly IntellisenseContext EnclosingContext;

            public IntellisenseContext(IntellisenseContextType contextType, IntellisenseContext enclosingContext)
            {
                ContextType = contextType;
                EnclosingContext = enclosingContext;
            }

            public bool InMemberExpression
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public void AugmentCompletionSession(ICompletionSession session, List<Completion> completions, List<Completion> completionBuilders)
            {
                throw new NotImplementedException();
            }
        }

        public enum IntellisenseContextType
        {
            Global,
            Rule,
            RuleRewrite,
            Options,
            Tokens,
            AttributeScope,
            NamedAction,
            Arguments,
            Action,
        }
    }
}
