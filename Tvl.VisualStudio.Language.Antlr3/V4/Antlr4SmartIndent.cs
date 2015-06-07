namespace Tvl.VisualStudio.Language.AntlrV4
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Atn;
    using Antlr4.Runtime.Misc;
    using Antlr4.Runtime.Tree;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Tvl.VisualStudio.OutputWindow.Interfaces;
    using Tvl.VisualStudio.Shell;
    using AntlrLanguagePackage = Tvl.VisualStudio.Language.Antlr3.AntlrLanguagePackage;

    internal sealed class Antlr4SmartIndent : SmartIndent
    {
        private readonly Antlr4SmartIndentProvider _provider;
        private readonly Antlr4LanguageInfo _languageInfo;

        public Antlr4SmartIndent(Antlr4SmartIndentProvider provider, IOutputWindowPane diagnosticsPane)
            : base(diagnosticsPane)
        {
            _provider = provider;

            var shell = provider.ServiceProvider.GetShell();
            var package = shell.LoadPackage<AntlrLanguagePackage>();
            _languageInfo = package.GetService<Antlr4LanguageInfo>();
        }

        protected override vsIndentStyle IndentStyle
        {
            get
            {
                return _languageInfo.LanguagePreferences.RawPreferences.IndentStyle;
            }
        }

        protected override int TabSize
        {
            get
            {
                return (int)_languageInfo.LanguagePreferences.RawPreferences.uTabSize;
            }
        }

        private int IndentSize
        {
            get
            {
                return (int)_languageInfo.LanguagePreferences.RawPreferences.uIndentSize;
            }
        }

        protected override ITokenSource GetTokenSource(SnapshotSpan snapshotSpan)
        {
            GrammarLexer lexer = new GrammarLexer(new AntlrInputStream(snapshotSpan.Snapshot.GetText()));
            lexer.RemoveErrorListeners();
            ITextSnapshotLine line = snapshotSpan.Snapshot.GetLineFromPosition(snapshotSpan.Start);
            lexer.InputStream.Seek(snapshotSpan.Start);
            lexer.Line = line.LineNumber + 1;
            lexer.Column = snapshotSpan.Start - line.Start;
            return lexer;
        }

        [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_ruleSpec, 0, Dependents.Self)]
        protected override IDictionary<RuleContext, CaretReachedException> GetParseTrees(ITokenStream tokens, IReferenceAnchors referenceAnchors)
        {
            Antlr4CodeCompletionParser parser = new Antlr4CodeCompletionParser(tokens);
            parser.RemoveErrorListeners();
            parser.BuildParseTree = true;
            parser.ErrorHandler = new CodeCompletionErrorStrategy();

            Antlr4ForestParser forestParser;
            if (referenceAnchors.Previous != null)
            {
                switch (referenceAnchors.Previous.RuleIndex)
                {
                case GrammarParser.RULE_ruleSpec:
                    forestParser = Antlr4ForestParser.Rules;
                    break;

                default:
                    forestParser = null;
                    break;
                }
            }
            else
            {
                forestParser = Antlr4ForestParser.GrammarSpec;
            }

            if (forestParser == null)
                return null;

            var originalInterpreter = parser.Interpreter;
            try
            {
                IntervalSet wordlikeTokenTypes = IntervalSet.Of(0, GrammarParser._ATN.maxTokenType);
                parser.Interpreter = new FixedCompletionParserATNSimulator(parser, GrammarParser._ATN, wordlikeTokenTypes);
                return forestParser.GetParseTrees(parser);
            }
            finally
            {
                parser.Interpreter = originalInterpreter;
            }
        }

        protected override IList<IAnchor> GetDynamicAnchorPoints(ITextSnapshot snapshot)
        {
            return _provider.DynamicAnchorPointsProvider.GetDynamicAnchorPoints(snapshot.TextBuffer).GetValue(snapshot, ParserDataOptions.None);
        }

        [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_parserRuleSpec, 0, Dependents.Self | Dependents.Parents)]
        [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_ruleAltList, 0, Dependents.Self | Dependents.Parents)]
        [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_lexerRule, 0, Dependents.Self | Dependents.Parents)]
        [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_lexerAltList, 1, Dependents.Self | Dependents.Parents)]
        [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_altList, 0, Dependents.Self | Dependents.Parents)]
        [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_blockSet, 0, Dependents.Self | Dependents.Parents)]
        [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_lexerBlock, 1, Dependents.Self | Dependents.Parents)]
        [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_block, 0, Dependents.Self | Dependents.Parents)]
        [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_optionsSpec, 6, Dependents.Self | Dependents.Parents)]
        [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_tokensSpec, 6, Dependents.Self | Dependents.Parents)]
        [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_channelsSpec, 6, Dependents.Self | Dependents.Parents)]
        [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_modeSpec, 3, Dependents.Self | Dependents.Parents)]
        [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_delegateGrammars, 6, Dependents.Self | Dependents.Parents)]
        [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_actionBlock, 5, Dependents.Self | Dependents.Parents)]
        [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_elements, 5, Dependents.Self | Dependents.Parents)]
        [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_lexerElements, 3, Dependents.Self | Dependents.Parents)]
        [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_delegateGrammar, 0, Dependents.Self | Dependents.Parents)]
        [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_lexerAlt, 3, Dependents.Self | Dependents.Parents)]
        [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_labeledAlt, 1, Dependents.Self | Dependents.Parents)]
        [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_rules, 0, Dependents.Self | Dependents.Parents)]
        [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_lexerCommands, 3, Dependents.Self | Dependents.Descendants)]
        protected override AlignmentRequirements GetAlignmentRequirement(KeyValuePair<RuleContext, CaretReachedException> parseTree, IParseTree targetElement, IParseTree ancestor)
        {
            if (ancestor == targetElement && !(targetElement is ITerminalNode))
            {
                // special handling for predicted tokens that don't actually exist yet
                CaretReachedException ex = parseTree.Value;
                if (ex != null && ex.Transitions != null)
                {
                    bool validTransition = false;
                    bool selfTransition = false;

                    // examine transitions for predictions that don't actually exist yet
                    foreach (KeyValuePair<ATNConfig, IList<Transition>> entry in ex.Transitions)
                    {
                        if (entry.Value == null)
                            continue;

                        foreach (Transition transition in entry.Value)
                        {
                            IntervalSet label = transition.Label;
                            if (label == null)
                                continue;

                            bool containsInvalid =
                                label.Contains(GrammarParser.OR)
                                || label.Contains(GrammarParser.RPAREN)
                                || label.Contains(GrammarParser.RBRACE)
                                || label.Contains(GrammarParser.END_ACTION)
                                || label.Contains(GrammarParser.END_ARG_ACTION)
                                || label.Contains(GrammarParser.OPTIONS)
                                || label.Contains(GrammarParser.AT)
                                || label.Contains(GrammarParser.ASSIGN)
                                || label.Contains(GrammarParser.SEMI)
                                || label.Contains(GrammarParser.COMMA)
                                || label.Contains(GrammarParser.MODE)
                                || label.Contains(GrammarParser.RARROW)
                                || label.Contains(GrammarParser.POUND);
                            bool containsInvalidSelf =
                                label.Contains(GrammarParser.LPAREN)
                                || label.Contains(GrammarParser.BEGIN_ACTION)
                                || label.Contains(GrammarParser.BEGIN_ARG_ACTION);

                            if (transition is NotSetTransition)
                            {
                                containsInvalid = !containsInvalid;
                                containsInvalidSelf = !containsInvalidSelf;
                            }

                            validTransition |= !containsInvalid;
                            selfTransition |= !containsInvalidSelf;
                        }
                    }

                    if (!validTransition)
                        return AlignmentRequirements.IgnoreTree;
                    else if (!selfTransition)
                        return AlignmentRequirements.UseAncestor;
                }
            }

            IRuleNode ruleNode = ancestor as IRuleNode;
            if (ruleNode == null)
                return AlignmentRequirements.UseAncestor;

            RuleContext ruleContext = ruleNode.RuleContext;
            switch (ruleContext.RuleIndex)
            {
            case GrammarParser.RULE_parserRuleSpec:
                if (((GrammarParser.ParserRuleSpecContext)ruleContext).RULE_REF() == null)
                    return AlignmentRequirements.UseAncestor;

                return AlignmentRequirements.PriorSibling;

            case GrammarParser.RULE_lexerRule:
                if (((GrammarParser.LexerRuleContext)ruleContext).TOKEN_REF() == null)
                    return AlignmentRequirements.UseAncestor;

                return AlignmentRequirements.PriorSibling;

            case GrammarParser.RULE_ruleAltList:
            case GrammarParser.RULE_lexerAltList:
            case GrammarParser.RULE_altList:
            case GrammarParser.RULE_blockSet:
            case GrammarParser.RULE_lexerBlock:
            case GrammarParser.RULE_block:
            case GrammarParser.RULE_optionsSpec:
            case GrammarParser.RULE_tokensSpec:
            case GrammarParser.RULE_channelsSpec:
            case GrammarParser.RULE_modeSpec:
            case GrammarParser.RULE_delegateGrammars:
            case GrammarParser.RULE_actionBlock:
            case GrammarParser.RULE_elements:
            case GrammarParser.RULE_lexerElements:
            case GrammarParser.RULE_rules:
                //case GrammarParser.RULE_lexerCommands:
                return AlignmentRequirements.PriorSibling;

            case GrammarParser.RULE_lexerAlt:
                GrammarParser.LexerAltContext lexerAltContext = ParseTrees.GetTypedRuleContext<GrammarParser.LexerAltContext>(ancestor);
                if (lexerAltContext != null && lexerAltContext.lexerCommands() != null && ParseTrees.IsAncestorOf(lexerAltContext.lexerCommands(), targetElement))
                    return AlignmentRequirements.PriorSibling;
                else
                    return AlignmentRequirements.UseAncestor;

            case GrammarParser.RULE_labeledAlt:
                if (ParseTrees.GetTerminalNodeType(targetElement) == GrammarParser.POUND)
                    return AlignmentRequirements.PriorSibling;
                else
                    return AlignmentRequirements.UseAncestor;

            case GrammarParser.RULE_delegateGrammar:
                return AlignmentRequirements.None;

            default:
                return AlignmentRequirements.UseAncestor;
            }
        }

        protected override Tuple<IParseTree, int> GetAlignmentElement(KeyValuePair<RuleContext, CaretReachedException> parseTree, IParseTree targetElement, IParseTree container, IList<IParseTree> priorSiblings)
        {
            AlignmentElementVisitor visitor = new AlignmentElementVisitor(parseTree, targetElement, priorSiblings, IndentSize);
            return visitor.Visit(container);
        }

        private class FixedCompletionParserATNSimulator : CompletionParserATNSimulator
        {
            private readonly IntervalSet _wordlikeTokenTypes;

            public FixedCompletionParserATNSimulator(Parser parser, ATN atn, IntervalSet wordlikeTokenTypes)
                : base(parser, atn)
            {
                _wordlikeTokenTypes = wordlikeTokenTypes;
            }

            protected override IntervalSet GetWordlikeTokenTypes()
            {
                return _wordlikeTokenTypes;
            }
        }

        private class AlignmentElementVisitor : GrammarParserBaseVisitor<Tuple<IParseTree, int>>
        {
            private readonly KeyValuePair<RuleContext, CaretReachedException> parseTree;
            private readonly IParseTree targetElement;
            private readonly IList<IParseTree> priorSiblings;
            private readonly int _indentSize;

            public AlignmentElementVisitor(KeyValuePair<RuleContext, CaretReachedException> parseTree, IParseTree targetElement, IList<IParseTree> priorSiblings, int indentSize)
            {
                this.parseTree = parseTree;
                this.targetElement = targetElement;
                this.priorSiblings = priorSiblings;
                _indentSize = indentSize;
            }

            public override Tuple<IParseTree, int> VisitChildren(IRuleNode node)
            {
                throw new NotSupportedException("This visitor is designed for top-level nodes only.");
            }

            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_elements, 5, Dependents.Self | Dependents.Parents)]
            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_lexerElements, 3, Dependents.Self | Dependents.Parents)]
            private Tuple<IParseTree, int> VisitElements()
            {
                // the non-terminals under these rules are straightforward
                int firstElementIndex = -1;
                for (int i = 0; i < priorSiblings.Count; i++)
                {
                    IParseTree sibling = priorSiblings[i];
                    if (sibling is IRuleNode)
                    {
                        firstElementIndex = i;
                        break;
                    }
                }

                for (int i = priorSiblings.Count - 2; i >= 0; i--)
                {
                    IParseTree sibling = priorSiblings[i];
                    if (!(sibling is IRuleNode))
                        continue;

                    if (i == firstElementIndex || ParseTrees.ElementStartsLine(sibling))
                        return Tuple.Create(sibling, 0);
                }

                // handle at the parent
                return null;
            }

            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_ruleAltList, 0, Dependents.Self | Dependents.Parents)]
            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_lexerAltList, 1, Dependents.Self | Dependents.Parents)]
            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_altList, 0, Dependents.Self | Dependents.Parents)]
            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_blockSet, 0, Dependents.Self | Dependents.Parents)]
            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_lexerBlock, 1, Dependents.Self | Dependents.Parents)]
            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_block, 0, Dependents.Self | Dependents.Parents)]
            private Tuple<IParseTree, int> VisitGenericBlock(ParserRuleContext container)
            {
                if (targetElement == ParseTrees.GetStartNode(container))
                {
                    return null;
                }

                if (ParseTrees.GetTerminalNodeType(targetElement) == GrammarParser.RPAREN)
                {
                    return Tuple.Create<IParseTree, int>(container, 0);
                }

                // OR lines up with previous OR
                bool orNode = ParseTrees.GetTerminalNodeType(targetElement) == GrammarParser.OR;
                if (orNode)
                {
                    for (int i = priorSiblings.Count - 2; i >= 0; i--)
                    {
                        ITerminalNode sibling = priorSiblings[i] as ITerminalNode;
                        if (sibling == null)
                            continue;

                        if (i == 0 || ParseTrees.ElementStartsLine(sibling))
                            return Tuple.Create<IParseTree, int>(sibling, 0);
                    }

                    if (ParseTrees.GetTerminalNodeType(ParseTrees.GetStartNode(container)) != GrammarParser.LPAREN)
                    {
                        // handle at the parent so it aligns at the (
                        return null;
                    }

                    return Tuple.Create<IParseTree, int>(container, 0);
                }

                // the non-terminals under these rules are straightforward
                int firstRuleIndex = -1;
                for (int i = 0; i < priorSiblings.Count; i++)
                {
                    IParseTree sibling = priorSiblings[i];
                    if (sibling is IRuleNode)
                    {
                        firstRuleIndex = i;
                        break;
                    }
                }

                for (int i = priorSiblings.Count - 2; i >= 0; i--)
                {
                    IRuleNode sibling = priorSiblings[i] as IRuleNode;
                    if (sibling == null)
                        continue;

                    if (i == firstRuleIndex || ParseTrees.ElementStartsLine(sibling))
                        return Tuple.Create<IParseTree, int>(sibling, 0);
                }

                // handle at the parent
                return null;
            }

            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_elements, 5, Dependents.Self | Dependents.Parents)]
            public override Tuple<IParseTree, int> VisitElements([NotNull]AbstractGrammarParser.ElementsContext context)
            {
                return VisitElements();
            }

            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_lexerElements, 3, Dependents.Self | Dependents.Parents)]
            public override Tuple<IParseTree, int> VisitLexerElements([NotNull]AbstractGrammarParser.LexerElementsContext context)
            {
                return VisitElements();
            }

            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_ruleAltList, 0, Dependents.Self | Dependents.Parents)]
            public override Tuple<IParseTree, int> VisitRuleAltList([NotNull]AbstractGrammarParser.RuleAltListContext context)
            {
                return VisitGenericBlock(context);
            }

            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_lexerAltList, 1, Dependents.Self | Dependents.Parents)]
            public override Tuple<IParseTree, int> VisitLexerAltList([NotNull]AbstractGrammarParser.LexerAltListContext context)
            {
                return VisitGenericBlock(context);
            }

            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_altList, 0, Dependents.Self | Dependents.Parents)]
            public override Tuple<IParseTree, int> VisitAltList([NotNull]AbstractGrammarParser.AltListContext context)
            {
                return VisitGenericBlock(context);
            }

            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_blockSet, 0, Dependents.Self | Dependents.Parents)]
            public override Tuple<IParseTree, int> VisitBlockSet([NotNull]AbstractGrammarParser.BlockSetContext context)
            {
                return VisitGenericBlock(context);
            }

            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_lexerBlock, 1, Dependents.Self | Dependents.Parents)]
            public override Tuple<IParseTree, int> VisitLexerBlock([NotNull]AbstractGrammarParser.LexerBlockContext context)
            {
                return VisitGenericBlock(context);
            }

            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_block, 0, Dependents.Self | Dependents.Parents)]
            public override Tuple<IParseTree, int> VisitBlock([NotNull]AbstractGrammarParser.BlockContext context)
            {
                return VisitGenericBlock(context);
            }

            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_parserRuleSpec, 0, Dependents.Self | Dependents.Parents)]
            public override Tuple<IParseTree, int> VisitParserRuleSpec([NotNull]AbstractGrammarParser.ParserRuleSpecContext context)
            {
                if (ParseTrees.GetTerminalNodeType(targetElement) == GrammarParser.AT)
                    return Tuple.Create<IParseTree, int>(context, 0);

                if (context.COLON() != null)
                {
                    if (ParseTrees.StartsBeforeStartOf(context.COLON(), targetElement))
                    {
                        switch (ParseTrees.GetTerminalNodeType(targetElement))
                        {
                        case GrammarParser.SEMI:
                        case GrammarParser.OR:
                            return Tuple.Create<IParseTree, int>(context.COLON(), 0);

                        default:
                            return Tuple.Create<IParseTree, int>(context.COLON(), _indentSize);
                        }
                    }
                }

                return Tuple.Create<IParseTree, int>(context, _indentSize);
            }

            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_lexerRule, 0, Dependents.Self | Dependents.Parents)]
            public override Tuple<IParseTree, int> VisitLexerRule([NotNull]AbstractGrammarParser.LexerRuleContext context)
            {
                if (context.name == null)
                    return null;

                if (ParseTrees.GetTerminalNodeType(targetElement) == GrammarParser.AT)
                    return Tuple.Create<IParseTree, int>(context, 0);

                if (context.COLON() != null)
                {
                    if (ParseTrees.StartsBeforeStartOf(context.COLON(), targetElement))
                    {
                        switch (ParseTrees.GetTerminalNodeType(targetElement))
                        {
                        case GrammarParser.SEMI:
                        case GrammarParser.OR:
                            return Tuple.Create<IParseTree, int>(context.COLON(), 0);

                        default:
                            return Tuple.Create<IParseTree, int>(context.COLON(), _indentSize);
                        }
                    }
                }

                return Tuple.Create<IParseTree, int>(context, _indentSize);
            }

            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_lexerAlt, 3, Dependents.Self | Dependents.Parents)]
            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_lexerElements, 0, Dependents.Self)]
            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_lexerCommands, 3, Dependents.Self | Dependents.Descendants)]
            public override Tuple<IParseTree, int> VisitLexerAlt([NotNull]AbstractGrammarParser.LexerAltContext context)
            {
                Debug.Assert(context.lexerCommands() != null && ParseTrees.IsAncestorOf(context.lexerCommands(), targetElement));
                if (context.lexerElements() == null)
                    return null;

                return Tuple.Create<IParseTree, int>(context.lexerElements(), 0);
            }

            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_labeledAlt, 1, Dependents.Self | Dependents.Parents)]
            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_alternative, 5, Dependents.Self)]
            public override Tuple<IParseTree, int> VisitLabeledAlt([NotNull]AbstractGrammarParser.LabeledAltContext context)
            {
                Debug.Assert(ParseTrees.GetTerminalNodeType(targetElement) == GrammarParser.POUND);
                if (context.alternative() == null)
                    return null;

                return Tuple.Create<IParseTree, int>(context.alternative(), 0);
            }

            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_rules, 0, Dependents.Self | Dependents.Parents)]
            public override Tuple<IParseTree, int> VisitRules([NotNull]AbstractGrammarParser.RulesContext context)
            {
                for (int i = priorSiblings.Count - 2; i >= 0; i--)
                {
                    IParseTree sibling = priorSiblings[i];
                    if (i == 0 || ParseTrees.ElementStartsLine(sibling))
                        return Tuple.Create(sibling, 0);
                }

                return null;
            }

            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_optionsSpec, 6, Dependents.Self | Dependents.Parents)]
            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_option, 3, Dependents.Self | Dependents.Parents)]
            public override Tuple<IParseTree, int> VisitOptionsSpec([NotNull]AbstractGrammarParser.OptionsSpecContext context)
            {
                // use previous option if any, otherwise use the block.
                // special handling for closing }
                if (targetElement == context.RBRACE())
                    return Tuple.Create<IParseTree, int>(context, 0);

                int firstOptionIndex = -1;
                for (int i = 0; i < priorSiblings.Count; i++)
                {
                    IRuleNode sibling = priorSiblings[i] as IRuleNode;
                    if (sibling == null)
                        continue;

                    if (sibling.RuleContext.RuleIndex == GrammarParser.RULE_option)
                    {
                        firstOptionIndex = i;
                        break;
                    }
                }

                bool semi = ParseTrees.GetTerminalNodeType(targetElement) == GrammarParser.SEMI;
                for (int i = priorSiblings.Count - 2; i >= 0; i--)
                {
                    IRuleNode sibling = priorSiblings[i] as IRuleNode;
                    if (sibling == null)
                        continue;

                    RuleContext ruleContext = sibling.RuleContext;
                    if (ruleContext.RuleIndex == GrammarParser.RULE_option)
                    {
                        if (i == firstOptionIndex || ParseTrees.ElementStartsLine(sibling))
                        {
                            return Tuple.Create<IParseTree, int>(sibling, semi ? _indentSize : 0);
                        }
                    }
                }

                return Tuple.Create<IParseTree, int>(context, _indentSize);
            }

            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_tokensSpec, 6, Dependents.Self | Dependents.Parents)]
            public override Tuple<IParseTree, int> VisitTokensSpec([NotNull]AbstractGrammarParser.TokensSpecContext context)
            {
                if (context.ChildCount == 0 || targetElement == context.GetChild(0))
                    return null;

                if (ParseTrees.GetTerminalNodeType(targetElement) == GrammarParser.RBRACE)
                    return Tuple.Create<IParseTree, int>(context, 0);

                // align to the previous element
                for (int i = priorSiblings.Count - 2; i >= 0; i--)
                {
                    IParseTree sibling = priorSiblings[i];
                    // stop at the first id rule, index 0 is the TOKENS terminal itself
                    if (i == 1 || ParseTrees.ElementStartsLine(sibling))
                        return Tuple.Create(sibling, 0);
                }

                return Tuple.Create<IParseTree, int>(context, _indentSize);
            }

            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_channelsSpec, 6, Dependents.Self | Dependents.Parents)]
            public override Tuple<IParseTree, int> VisitChannelsSpec([NotNull]AbstractGrammarParser.ChannelsSpecContext context)
            {
                if (context.ChildCount == 0 || targetElement == context.GetChild(0))
                    return null;

                if (ParseTrees.GetTerminalNodeType(targetElement) == GrammarParser.RBRACE)
                    return Tuple.Create<IParseTree, int>(context, 0);

                // align to the previous element
                for (int i = priorSiblings.Count - 2; i >= 0; i--)
                {
                    IParseTree sibling = priorSiblings[i];
                    // stop at the first id rule, index 0 is the CHANNELS terminal itself
                    if (i == 1 || ParseTrees.ElementStartsLine(sibling))
                        return Tuple.Create(sibling, 0);
                }

                return Tuple.Create<IParseTree, int>(context, _indentSize);
            }

            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_actionBlock, 5, Dependents.Self | Dependents.Parents)]
            public override Tuple<IParseTree, int> VisitActionBlock([NotNull]AbstractGrammarParser.ActionBlockContext context)
            {
                if (context.ChildCount == 0 || targetElement == context.GetChild(0))
                    return null;

                if (ParseTrees.GetTerminalNodeType(targetElement) == GrammarParser.RBRACE)
                    return Tuple.Create<IParseTree, int>(context, 0);

                // align to the previous element
                for (int i = priorSiblings.Count - 2; i >= 0; i--)
                {
                    IParseTree sibling = priorSiblings[i];
                    // stop at the first id rule, index 0 is the BEGIN_ACTION terminal itself
                    if (i == 1 || ParseTrees.ElementStartsLine(sibling))
                        return Tuple.Create(sibling, 0);
                }

                return Tuple.Create<IParseTree, int>(context, _indentSize);
            }

            public override Tuple<IParseTree, int> VisitDelegateGrammar([NotNull]AbstractGrammarParser.DelegateGrammarContext context)
            {
                throw new NotImplementedException();
            }

            public override Tuple<IParseTree, int> VisitDelegateGrammars([NotNull]AbstractGrammarParser.DelegateGrammarsContext context)
            {
                throw new NotImplementedException();
            }

            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_modeSpec, 3, Dependents.Self | Dependents.Parents)]
            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_ruleSpec, 3, Dependents.Self | Dependents.Parents)]
            public override Tuple<IParseTree, int> VisitModeSpec([NotNull]AbstractGrammarParser.ModeSpecContext context)
            {
                // use the preceding rule (if any), otherwise relative to mode
                for (int i = priorSiblings.Count - 2; i >= 0; i--)
                {
                    IRuleNode sibling = priorSiblings[i] as IRuleNode;
                    if (sibling == null)
                        continue;

                    RuleContext ruleContext = sibling.RuleContext;
                    if (context.RuleIndex == GrammarParser.RULE_ruleSpec)
                        return Tuple.Create<IParseTree, int>(ruleContext, 0);
                }

                return Tuple.Create<IParseTree, int>(context, _indentSize);
            }
        }
    }
}
