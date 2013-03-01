namespace Tvl.VisualStudio.Language.AntlrV4
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Antlr4.Runtime;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Operations;
    using Tvl.VisualStudio.Language.Antlr3.OptionsPages;
    using Tvl.VisualStudio.Language.Intellisense;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Shell;

    using AntlrLanguagePackage = Tvl.VisualStudio.Language.Antlr3.AntlrLanguagePackage;
    using ImageSource = System.Windows.Media.ImageSource;
    using Regex = System.Text.RegularExpressions.Regex;
    using RegexOptions = System.Text.RegularExpressions.RegexOptions;
    using Stopwatch = System.Diagnostics.Stopwatch;

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

    internal partial class Antlr4CompletionSource : CompletionSource
    {
        private static readonly Regex IdentifierRegex = new Regex(@"^(?:(?:\$?[A-Za-z][A-Za-z0-9_]*)|\$)$", RegexOptions.Compiled);

        private readonly AntlrIntellisenseOptions _intellisenseOptions;

        public Antlr4CompletionSource(ITextBuffer textBuffer, Antlr4CompletionSourceProvider provider, Antlr4BackgroundParser backgroundParser)
            : base(textBuffer, provider, Antlr4Constants.Antlr4LanguageGuid)
        {
            BackgroundParser = backgroundParser;
            var shell = Provider.GlobalServiceProvider.GetShell();
            var package = shell.LoadPackage<AntlrLanguagePackage>();
            _intellisenseOptions = package.IntellisenseOptions;
        }

        public new Antlr4CompletionSourceProvider Provider
        {
            get
            {
                return (Antlr4CompletionSourceProvider)base.Provider;
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
                return Antlr4Classifier.Keywords;
            }
        }

        private Antlr4BackgroundParser BackgroundParser
        {
            get;
            set;
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
                IntellisenseController controller = GetControllerForView(session.TextView);
                CompletionInfo completionInfo = controller.CompletionInfo;
                ITextSnapshot snapshot = triggerPoint.TextBuffer.CurrentSnapshot;
                SnapshotPoint point = triggerPoint.GetPoint(snapshot);
                bool extendLeft = false;
                bool extend = true;

                // labels includes both implicit and explicit labels
                var labels = FindLabelsInScope(point);

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

                if (invocationType == IntellisenseInvocationType.BackspaceDeleteOrBackTab && extentOfWord.Span.Length > 0)
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

                if (completionInfo.InfoType == CompletionInfoType.AutoListMemberInfo && extentOfWord.Span.GetText().StartsWith("$") && labels.Count == 0)
                {
                    session.Dismiss();
                    return;
                }

                ITrackingSpan applicableTo = snapshot.CreateTrackingSpan(extentOfWord.Span, SpanTrackingMode.EdgeInclusive, TrackingFidelityMode.Forward);
                if (extendLeft)
                {
                    SnapshotSpan textSoFarSpan = new SnapshotSpan(snapshot, extentOfWord.Span.Start, triggerPoint.GetPoint(snapshot));
                    string textSoFar = textSoFarSpan.GetText();
                    applicableTo = snapshot.CreateTrackingSpan(point.Position - textSoFar.Length, textSoFar.Length, SpanTrackingMode.EdgeInclusive, TrackingFidelityMode.Forward);
                }

                IEnumerable<Completion> context = GetContextCompletions(triggerPoint.GetPoint(snapshot), (Antlr4IntellisenseController)controller, session);
                IEnumerable<Completion> keywords = GetKeywordCompletions();
                IEnumerable<Completion> snippets = GetSnippetCompletions();
                IEnumerable<Completion> labelCompletions = GetLabelCompletions(labels);
                //SnapshotSpan? Provider.IntellisenseCache.GetExpressionSpan(triggerPoint.GetPoint(snapshot));

                IEnumerable<Completion> completions = context.Concat(keywords).Concat(snippets).Concat(labelCompletions);
                IEnumerable<Completion> orderedCompletions = completions.Distinct(CompletionDisplayNameComparer.CurrentCulture).OrderBy(i => i.DisplayText, StringComparer.CurrentCultureIgnoreCase);

                CompletionSet completionSet = new CompletionSet("AntlrCompletions", "Antlr Completions", applicableTo, orderedCompletions, EmptyCompletions);
                completionSets.Add(completionSet);
            }
        }

        protected override IEnumerable<Completion> GetKeywordCompletions()
        {
            if (!_intellisenseOptions.KeywordsInCompletionLists)
                return Enumerable.Empty<Completion>();

            return base.GetKeywordCompletions();
        }

        protected override IEnumerable<Completion> GetSnippetCompletions()
        {
            if (!_intellisenseOptions.CodeSnippetsInCompletionLists)
                return Enumerable.Empty<Completion>();

            return base.GetSnippetCompletions();
        }

        private IEnumerable<Completion> GetLabelCompletions(List<LabelInfo> labels)
        {
            return labels.Select(CreateLabelCompletion);
        }

        private Completion CreateLabelCompletion(LabelInfo label)
        {
            string displayText = "$" + label.Name;
            string insertionText = "$" + label.Name;
            string description = label.Description;
            string iconAutomationText = string.Empty;
            ImageSource iconSource = Provider.GlyphService.GetGlyph(label.Glyph, StandardGlyphItem.GlyphItemPublic);
            return new Completion(displayText, insertionText, description, iconSource, iconAutomationText);
        }

#if false
        private List<LabelInfo> FindLabelsInScope(SnapshotPoint triggerPoint)
        {
            List<LabelInfo> labels = new List<LabelInfo>();

            /* use the experimental model to locate and process the expression */
            Stopwatch stopwatch = Stopwatch.StartNew();

            // lex the entire document
            var currentSnapshot = triggerPoint.Snapshot;
            var input = new SnapshotCharStream(currentSnapshot, new Span(0, currentSnapshot.Length));
            var lexer = new ANTLRLexer(input);
            var tokens = new CommonTokenStream(lexer);
            tokens.Fill();

            // locate the last token before the trigger point
            while (true)
            {
                IToken nextToken = tokens.Lt(1);
                if (nextToken.Type == CharStreamConstants.EndOfFile)
                    break;

                if (nextToken.StartIndex > triggerPoint.Position)
                    break;

                tokens.Consume();
            }

            bool inAction = false;
            IToken triggerToken = tokens.LT(-1);
            switch (triggerToken.Type)
            {
            case ANTLRLexer.RULE_REF:
            case ANTLRLexer.TOKEN_REF:
            case ANTLRLexer.DOLLAR:
                break;

            case ANTLRLexer.ACTION:
            case ANTLRLexer.FORCED_ACTION:
            case ANTLRLexer.SEMPRED:
            case ANTLRLexer.ARG_ACTION:
                inAction = true;
                break;

            default:
                return labels;
            }

            NetworkInterpreter interpreter = CreateNetworkInterpreter(tokens);
            while (interpreter.TryStepBackward())
            {
                if (interpreter.Contexts.Count == 0 || interpreter.Contexts.Count > 4000)
                    break;

                if (interpreter.Contexts.All(context => context.BoundedStart))
                    break;
            }

            if (interpreter.Failed)
                interpreter.Contexts.Clear();

            interpreter.CombineBoundedStartContexts();

            HashSet<IToken> labelTokens = new HashSet<IToken>(TokenIndexEqualityComparer.Default);
            foreach (var context in interpreter.Contexts)
            {
                var tokenTransitions = context.Transitions.Where(i => i.TokenIndex != null).ToList();
                for (int i = 1; i < tokenTransitions.Count - 1; i++)
                {
                    if (tokenTransitions[i].Symbol != ANTLRLexer.TOKEN_REF && tokenTransitions[i].Symbol != ANTLRLexer.RULE_REF)
                        continue;

                    // we add explicit labels, plus implicit labels if we're in an action
                    if (tokenTransitions[i + 1].Symbol == ANTLRLexer.ASSIGN || tokenTransitions[i + 1].Symbol == ANTLRLexer.PLUS_ASSIGN)
                    {
                        RuleBinding rule = interpreter.Network.StateRules[tokenTransitions[i + 1].Transition.SourceState.Id];
                        if (rule.Name == AntlrAtnBuilder.RuleNames.TreeRoot || rule.Name == AntlrAtnBuilder.RuleNames.ElementNoOptionSpec)
                            labelTokens.Add(tokenTransitions[i].Token);
                    }
                    else if (inAction && tokenTransitions[i - 1].Symbol != ANTLRLexer.ASSIGN && tokenTransitions[i - 1].Symbol != ANTLRLexer.PLUS_ASSIGN)
                    {
                        RuleBinding rule = interpreter.Network.StateRules[tokenTransitions[i].Transition.SourceState.Id];
                        if (rule.Name == AntlrAtnBuilder.RuleNames.Terminal || rule.Name == AntlrAtnBuilder.RuleNames.NotTerminal || rule.Name == AntlrAtnBuilder.RuleNames.RuleRef)
                            labelTokens.Add(tokenTransitions[i].Token);
                    }
                }
            }

            foreach (var token in labelTokens)
            {
                labels.Add(new LabelInfo(token.Text, "(label) " + token.Text, new SnapshotSpan(triggerPoint.Snapshot, Span.FromBounds(token.StartIndex, token.StopIndex + 1)), StandardGlyphGroup.GlyphGroupField, Enumerable.Empty<LabelInfo>()));
            }

            /* add scopes */
            if (inAction)
            {
                /* add global scopes */
                IList<IToken> tokensList = tokens.GetTokens();
                for (int i = 0; i < tokensList.Count - 1; i++)
                {
                    var token = tokensList[i];
                    /* all global scopes appear before the first rule. before the first rule, the only place a ':' can appear is
                     * in the form '::' for things like @lexer::namespace{}
                     */
                    if (token.Type == ANTLRLexer.COLON && tokensList[i + 1].Type == ANTLRLexer.COLON)
                        break;

                    if (token.Type == ANTLRLexer.SCOPE)
                    {
                        var nextToken = tokensList.Skip(i + 1).FirstOrDefault(t => t.Channel == TokenChannels.Default);
                        if (nextToken != null && (nextToken.Type == ANTLRLexer.RULE_REF || nextToken.Type == ANTLRLexer.TOKEN_REF))
                        {
                            // TODO: parse scope members
                            IToken actionToken = tokensList.Skip(nextToken.TokenIndex + 1).FirstOrDefault(t => t.Channel == TokenChannels.Default);
                            IEnumerable<LabelInfo> members = Enumerable.Empty<LabelInfo>();

                            if (actionToken != null && actionToken.Type == ANTLRLexer.ACTION)
                            {
                                IEnumerable<IToken> scopeMembers = ExtractScopeAttributes(nextToken);
                                members = scopeMembers.Select(member =>
                                    {
                                        string name = member.Text;
                                        SnapshotSpan definition = new SnapshotSpan(triggerPoint.Snapshot, Span.FromBounds(member.StartIndex, member.StopIndex + 1));
                                        StandardGlyphGroup glyph = StandardGlyphGroup.GlyphGroupField;
                                        IEnumerable<LabelInfo> nestedMembers = Enumerable.Empty<LabelInfo>();
                                        return new LabelInfo(name, string.Empty, definition, glyph, nestedMembers);
                                    });
                            }

                            labels.Add(new LabelInfo(nextToken.Text, "(global scope) " + nextToken.Text, new SnapshotSpan(triggerPoint.Snapshot, Span.FromBounds(nextToken.StartIndex, nextToken.StopIndex + 1)), StandardGlyphGroup.GlyphGroupNamespace, members));
                        }
                    }
                }

                /* add rule scopes */
                // todo
            }

            /* add arguments and return values */
            if (inAction)
            {
                HashSet<IToken> argumentTokens = new HashSet<IToken>(TokenIndexEqualityComparer.Default);
                foreach (var context in interpreter.Contexts)
                {
                    var tokenTransitions = context.Transitions.Where(i => i.TokenIndex != null).ToList();
                    for (int i = 1; i < tokenTransitions.Count; i++)
                    {
                        if (tokenTransitions[i].Symbol == ANTLRLexer.RETURNS || tokenTransitions[i].Symbol == ANTLRLexer.COLON)
                            break;

                        if (tokenTransitions[i].Symbol == ANTLRLexer.ARG_ACTION)
                            argumentTokens.Add(tokenTransitions[i].Token);
                    }
                }

                foreach (var token in argumentTokens)
                {
                    IEnumerable<IToken> arguments = ExtractArguments(token);
                    foreach (var argument in arguments)
                    {
                        labels.Add(new LabelInfo(argument.Text, "(parameter) " + argument.Text, new SnapshotSpan(triggerPoint.Snapshot, Span.FromBounds(argument.StartIndex, argument.StopIndex + 1)), StandardGlyphGroup.GlyphGroupVariable, Enumerable.Empty<LabelInfo>()));
                    }
                }
            }

            /* add return values */
            if (inAction)
            {
                HashSet<IToken> returnTokens = new HashSet<IToken>(TokenIndexEqualityComparer.Default);
                foreach (var context in interpreter.Contexts)
                {
                    var tokenTransitions = context.Transitions.Where(i => i.TokenIndex != null).ToList();
                    for (int i = 1; i < tokenTransitions.Count - 1; i++)
                    {
                        if (tokenTransitions[i].Symbol == ANTLRLexer.COLON)
                            break;

                        if (tokenTransitions[i].Symbol == ANTLRLexer.RETURNS)
                        {
                            if (tokenTransitions[i + 1].Symbol == ANTLRLexer.ARG_ACTION)
                                returnTokens.Add(tokenTransitions[i + 1].Token);

                            break;
                        }
                    }
                }

                foreach (var token in returnTokens)
                {
                    IEnumerable<IToken> returnValues = ExtractArguments(token);
                    foreach (var returnValue in returnValues)
                    {
                        labels.Add(new LabelInfo(returnValue.Text, "(return value) "  + returnValue.Text, new SnapshotSpan(triggerPoint.Snapshot, Span.FromBounds(returnValue.StartIndex, returnValue.StopIndex + 1)), StandardGlyphGroup.GlyphGroupVariable, Enumerable.Empty<LabelInfo>()));
                    }
                }
            }

            /* add intrinsic labels ($start, $type, $text, $enclosingRuleName) */
            IToken ruleNameToken = null;
            HashSet<IToken> enclosingRuleNameTokens = new HashSet<IToken>(TokenIndexEqualityComparer.Default);
            foreach (var context in interpreter.Contexts)
            {
                var tokenTransitions = context.Transitions.Where(i => i.Symbol == ANTLRLexer.RULE_REF || i.Symbol == ANTLRLexer.TOKEN_REF).ToList();
                if (!tokenTransitions.Any())
                    continue;

                ruleNameToken = tokenTransitions.First().Token;
                if (ruleNameToken != null)
                    enclosingRuleNameTokens.Add(ruleNameToken);
            }

            foreach (var token in enclosingRuleNameTokens)
            {
                // TODO: add members
                labels.Add(new LabelInfo(token.Text, "(enclosing rule) " + token.Text, new SnapshotSpan(triggerPoint.Snapshot, Span.FromBounds(token.StartIndex, token.StopIndex + 1)), StandardGlyphGroup.GlyphGroupIntrinsic, Enumerable.Empty<LabelInfo>()));
            }

            GrammarType grammarType = GrammarType.None;

            int mark = tokens.Mark();
            try
            {
                tokens.Seek(0);
                bool hasGrammarType = false;
                while (!hasGrammarType)
                {
                    int la1 = tokens.LA(1);
                    switch (la1)
                    {
                    case ANTLRLexer.GRAMMAR:
                        IToken previous = tokens.LT(-1);
                        if (previous == null)
                            grammarType = GrammarType.Combined;
                        else if (previous.Type == ANTLRLexer.LEXER)
                            grammarType = GrammarType.Lexer;
                        else if (previous.Type == ANTLRLexer.PARSER)
                            grammarType = GrammarType.Parser;
                        else if (previous.Type == ANTLRLexer.TREE)
                            grammarType = GrammarType.TreeParser;
                        else
                            grammarType = GrammarType.None;

                        hasGrammarType = true;
                        break;

                    case CharStreamConstants.EndOfFile:
                        hasGrammarType = true;
                        break;

                    default:
                        break;
                    }

                    tokens.Consume();
                }
            }
            finally
            {
                tokens.Rewind(mark);
            }

            if (inAction)
            {
                switch (grammarType)
                {
                case GrammarType.Combined:
                    if (ruleNameToken == null)
                        goto default;
                    if (ruleNameToken.Type == ANTLRLexer.RULE_REF)
                        goto case GrammarType.Parser;
                    else
                        goto case GrammarType.Lexer;

                case GrammarType.Lexer:
                    labels.Add(new LabelInfo("text", string.Empty, new SnapshotSpan(), StandardGlyphGroup.GlyphGroupIntrinsic, Enumerable.Empty<LabelInfo>()));
                    labels.Add(new LabelInfo("type", string.Empty, new SnapshotSpan(), StandardGlyphGroup.GlyphGroupIntrinsic, Enumerable.Empty<LabelInfo>()));
                    labels.Add(new LabelInfo("line", string.Empty, new SnapshotSpan(), StandardGlyphGroup.GlyphGroupIntrinsic, Enumerable.Empty<LabelInfo>()));
                    labels.Add(new LabelInfo("index", string.Empty, new SnapshotSpan(), StandardGlyphGroup.GlyphGroupIntrinsic, Enumerable.Empty<LabelInfo>()));
                    labels.Add(new LabelInfo("pos", string.Empty, new SnapshotSpan(), StandardGlyphGroup.GlyphGroupIntrinsic, Enumerable.Empty<LabelInfo>()));
                    labels.Add(new LabelInfo("channel", string.Empty, new SnapshotSpan(), StandardGlyphGroup.GlyphGroupIntrinsic, Enumerable.Empty<LabelInfo>()));
                    labels.Add(new LabelInfo("start", string.Empty, new SnapshotSpan(), StandardGlyphGroup.GlyphGroupIntrinsic, Enumerable.Empty<LabelInfo>()));
                    labels.Add(new LabelInfo("stop", string.Empty, new SnapshotSpan(), StandardGlyphGroup.GlyphGroupIntrinsic, Enumerable.Empty<LabelInfo>()));
                    labels.Add(new LabelInfo("int", string.Empty, new SnapshotSpan(), StandardGlyphGroup.GlyphGroupIntrinsic, Enumerable.Empty<LabelInfo>()));
                    break;

                case GrammarType.Parser:
                    labels.Add(new LabelInfo("text", string.Empty, new SnapshotSpan(), StandardGlyphGroup.GlyphGroupIntrinsic, Enumerable.Empty<LabelInfo>()));
                    labels.Add(new LabelInfo("start", string.Empty, new SnapshotSpan(), StandardGlyphGroup.GlyphGroupIntrinsic, Enumerable.Empty<LabelInfo>()));
                    labels.Add(new LabelInfo("stop", string.Empty, new SnapshotSpan(), StandardGlyphGroup.GlyphGroupIntrinsic, Enumerable.Empty<LabelInfo>()));
                    labels.Add(new LabelInfo("tree", string.Empty, new SnapshotSpan(), StandardGlyphGroup.GlyphGroupIntrinsic, Enumerable.Empty<LabelInfo>()));
                    labels.Add(new LabelInfo("st", string.Empty, new SnapshotSpan(), StandardGlyphGroup.GlyphGroupIntrinsic, Enumerable.Empty<LabelInfo>()));
                    break;

                case GrammarType.TreeParser:
                    labels.Add(new LabelInfo("text", string.Empty, new SnapshotSpan(), StandardGlyphGroup.GlyphGroupIntrinsic, Enumerable.Empty<LabelInfo>()));
                    labels.Add(new LabelInfo("start", string.Empty, new SnapshotSpan(), StandardGlyphGroup.GlyphGroupIntrinsic, Enumerable.Empty<LabelInfo>()));
                    labels.Add(new LabelInfo("tree", string.Empty, new SnapshotSpan(), StandardGlyphGroup.GlyphGroupIntrinsic, Enumerable.Empty<LabelInfo>()));
                    labels.Add(new LabelInfo("st", string.Empty, new SnapshotSpan(), StandardGlyphGroup.GlyphGroupIntrinsic, Enumerable.Empty<LabelInfo>()));
                    break;

                default:
                    // if we're unsure about the grammar type, include all the possible options to make sure we're covered
                    labels.Add(new LabelInfo("text", string.Empty, new SnapshotSpan(), StandardGlyphGroup.GlyphGroupIntrinsic, Enumerable.Empty<LabelInfo>()));
                    labels.Add(new LabelInfo("type", string.Empty, new SnapshotSpan(), StandardGlyphGroup.GlyphGroupIntrinsic, Enumerable.Empty<LabelInfo>()));
                    labels.Add(new LabelInfo("line", string.Empty, new SnapshotSpan(), StandardGlyphGroup.GlyphGroupIntrinsic, Enumerable.Empty<LabelInfo>()));
                    labels.Add(new LabelInfo("index", string.Empty, new SnapshotSpan(), StandardGlyphGroup.GlyphGroupIntrinsic, Enumerable.Empty<LabelInfo>()));
                    labels.Add(new LabelInfo("pos", string.Empty, new SnapshotSpan(), StandardGlyphGroup.GlyphGroupIntrinsic, Enumerable.Empty<LabelInfo>()));
                    labels.Add(new LabelInfo("channel", string.Empty, new SnapshotSpan(), StandardGlyphGroup.GlyphGroupIntrinsic, Enumerable.Empty<LabelInfo>()));
                    labels.Add(new LabelInfo("start", string.Empty, new SnapshotSpan(), StandardGlyphGroup.GlyphGroupIntrinsic, Enumerable.Empty<LabelInfo>()));
                    labels.Add(new LabelInfo("stop", string.Empty, new SnapshotSpan(), StandardGlyphGroup.GlyphGroupIntrinsic, Enumerable.Empty<LabelInfo>()));
                    labels.Add(new LabelInfo("int", string.Empty, new SnapshotSpan(), StandardGlyphGroup.GlyphGroupIntrinsic, Enumerable.Empty<LabelInfo>()));
                    labels.Add(new LabelInfo("tree", string.Empty, new SnapshotSpan(), StandardGlyphGroup.GlyphGroupIntrinsic, Enumerable.Empty<LabelInfo>()));
                    labels.Add(new LabelInfo("st", string.Empty, new SnapshotSpan(), StandardGlyphGroup.GlyphGroupIntrinsic, Enumerable.Empty<LabelInfo>()));
                    break;
                }
            }

            return labels;
        }
#else
        private List<LabelInfo> FindLabelsInScope(SnapshotPoint triggerPoint)
        {
            return new List<LabelInfo>();
        }
#endif

        private IEnumerable<IToken> ExtractScopeAttributes(IToken token)
        {
            Contract.Requires(token != null);
            return ExtractArguments(token, ';');
        }

        private IEnumerable<IToken> ExtractArguments(IToken token)
        {
            Contract.Requires(token != null);
            return ExtractArguments(token, ',');
        }

        private IEnumerable<IToken> ExtractArguments(IToken token, char separator)
        {
            Contract.Requires<ArgumentNullException>(token != null, "token");

            string actionText = token.Text;
            List<IToken> attributeTokens = new List<IToken>();
            List<IToken> attributes = new List<IToken>();
#if false
            CodeGenerator.GetListOfArgumentsFromAction(token.Text, 0, -1, separator, attributes);
            foreach (IToken attributeToken in attributes)
            {
                string attributeText = attributeToken.Text;
                Attribute attribute = new Attribute(attributeText);
                int attributeStartIndex = actionText.IndexOf(attribute.Decl);
                int attributeNameStartIndex = attribute.Decl.IndexOf(attribute.Name);
                int start = token.StartIndex + attributeStartIndex + attributeNameStartIndex + 1;
                int stop = start + attribute.Name.Length - 1;

                // explicitly set the text in case escaped \] characters mess up the indexing
                attributeTokens.Add(new CommonToken(token.InputStream, ANTLRLexer.ID, TokenChannels.Default, start, stop)
                    {
                        Text = attribute.Name
                    });
            }
#endif

            return attributeTokens;
        }

#if false
        private NetworkInterpreter CreateNetworkInterpreter(CommonTokenStream tokens)
        {
            Network network = NetworkBuilder<AntlrAtnBuilder>.GetOrBuildNetwork();
            NetworkInterpreter interpreter = new NetworkInterpreter(network, tokens);

            IToken previousToken = tokens.LT(-1);
            if (previousToken == null)
                return new NetworkInterpreter(network, new CommonTokenStream());

            switch (previousToken.Type)
            {
            case ANTLRLexer.RULE_REF:
            case ANTLRLexer.TOKEN_REF:
            case ANTLRLexer.DOLLAR:
            case ANTLRLexer.ACTION:
            case ANTLRLexer.FORCED_ACTION:
            case ANTLRLexer.SEMPRED:
            case ANTLRLexer.ARG_ACTION:
                interpreter.BoundaryRules.Add(network.GetRule(AntlrAtnBuilder.RuleNames.Grammar));
                interpreter.BoundaryRules.Add(network.GetRule(AntlrAtnBuilder.RuleNames.Option));
                interpreter.BoundaryRules.Add(network.GetRule(AntlrAtnBuilder.RuleNames.DelegateGrammar));
                interpreter.BoundaryRules.Add(network.GetRule(AntlrAtnBuilder.RuleNames.TokenSpec));
                interpreter.BoundaryRules.Add(network.GetRule(AntlrAtnBuilder.RuleNames.AttrScope));
                interpreter.BoundaryRules.Add(network.GetRule(AntlrAtnBuilder.RuleNames.Action));
                interpreter.BoundaryRules.Add(network.GetRule(AntlrAtnBuilder.RuleNames.Rule));
                break;

            default:
                return new NetworkInterpreter(network, new CommonTokenStream());
            }

            return interpreter;
        }
#endif

        private IEnumerable<Completion> GetContextCompletions(SnapshotPoint triggerPoint, Antlr4IntellisenseController controller, ICompletionSession session)
        {
            var ruleSpans = BackgroundParser.RuleSpans;
            if (ruleSpans == null || ruleSpans.Count == 0)
                return EmptyCompletions;

            var ruleNames = ruleSpans.Keys.ToArray();
            List<Completion> completions = ruleNames.Select(CreateRuleReferenceCompletion).ToList();
            return completions;
            //// Antlr has the strange property that almost any globally visible token can be used throughout an expression (work out the subtle details later).
            //Element element;
            //if (!Provider.IntellisenseCache.TryResolveContext(AntlrIntellisenseCache.AntlrPositionReference.FromSnapshotPoint(triggerPoint), out element))
            //    return new List<Completion>();

            //List<Element> scopedDeclarations = element.GetScopedDeclarations().ToList();
            //List<Completion> completions = new List<Completion>();
            //foreach (var decl in scopedDeclarations)
            //{
            //    if (decl == null)
            //        continue;

            //    Completion completion = decl.CreateCompletion(controller, session);
            //    if (completion != null)
            //        completions.Add(completion);
            //}

            //return completions;
        }

        private Completion CreateRuleReferenceCompletion(string ruleName)
        {
            string displayText = ruleName;
            string insertionText = ruleName;
            string description = string.Empty;
            string iconAutomationText = string.Empty;
            ImageSource iconSource = char.IsLower(ruleName[0]) ? Provider.ParserRuleGlyph : Provider.LexerRuleGlyph;
            return new Completion(displayText, insertionText, description, iconSource, iconAutomationText);
        }

        private static bool IsCompletionPrefix(TextExtent extent)
        {
            string text = extent.Span.GetText();
            if (string.IsNullOrEmpty(text))
                return false;

            if (text == "$")
                return true;

            return IdentifierRegex.IsMatch(text);
        }

        private static IntellisenseController GetControllerForView(ITextView view)
        {
            object controllerList;
            if (!view.Properties.TryGetProperty(typeof(IntellisenseController), out controllerList))
                return null;

            IEnumerable<IntellisenseController> controllers = controllerList as IEnumerable<IntellisenseController>;
            if (controllers == null)
                return null;

            return controllers.OfType<Antlr4IntellisenseController>().SingleOrDefault();
        }

        private class LabelInfo
        {
            public readonly string Name;
            public readonly string Description = string.Empty;
            public readonly SnapshotSpan Definition;
            public readonly List<LabelInfo> Members;
            public readonly StandardGlyphGroup Glyph;

            public LabelInfo(string name, string description, SnapshotSpan definition, StandardGlyphGroup glyph, IEnumerable<LabelInfo> members)
            {
                Name = name;
                Description = description;
                Definition = definition;
                Glyph = glyph;
                Members = new List<LabelInfo>(members);
            }
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
