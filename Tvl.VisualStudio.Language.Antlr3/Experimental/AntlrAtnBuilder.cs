namespace Tvl.VisualStudio.Language.Antlr3.Experimental
{
    using System;
    using System.Collections.Generic;
    using Tvl.VisualStudio.Language.Parsing.Experimental.Atn;
    using ANTLRParser = global::Antlr3.Grammars.ANTLRParser;

    internal class AntlrAtnBuilder
    {
        private static Network _network;
        private readonly RuleBindings _ruleEndpoints = new RuleBindings();

        private AntlrAtnBuilder()
        {
        }

        private RuleBindings Rules
        {
            get
            {
                return _ruleEndpoints;
            }
        }

        public static Network BuildNetwork()
        {
            if (_network != null)
                return _network;

            AntlrAtnBuilder builder = new AntlrAtnBuilder();
            var rules = builder.Rules;

            Nfa.BindRule(rules.Grammar, builder.BuildGrammarRule());
            Nfa.BindRule(rules.GrammarType, builder.BuildGrammarTypeRule());
            Nfa.BindRule(rules.Actions, builder.BuildActionsRule());
            Nfa.BindRule(rules.Action, builder.BuildActionRule());
            Nfa.BindRule(rules.ActionScopeName, builder.BuildActionScopeNameRule());
            Nfa.BindRule(rules.OptionsSpec, builder.BuildOptionsSpecRule());
            Nfa.BindRule(rules.Option, builder.BuildOptionRule());
            Nfa.BindRule(rules.OptionValue, builder.BuildOptionValueRule());
            Nfa.BindRule(rules.DelegateGrammars, builder.BuildDelegateGrammarsRule());
            Nfa.BindRule(rules.DelegateGrammar, builder.BuildDelegateGrammarRule());
            Nfa.BindRule(rules.TokensSpec, builder.BuildTokensSpecRule());
            Nfa.BindRule(rules.TokenSpec, builder.BuildTokenSpecRule());
            Nfa.BindRule(rules.AttrScopes, builder.BuildAttrScopesRule());
            Nfa.BindRule(rules.AttrScope, builder.BuildAttrScopeRule());
            Nfa.BindRule(rules.Rules, builder.BuildRulesRule());
            Nfa.BindRule(rules.Rule, builder.BuildRuleRule());
            Nfa.BindRule(rules.RuleActions, builder.BuildRuleActionsRule());
            Nfa.BindRule(rules.RuleAction, builder.BuildRuleActionRule());
            Nfa.BindRule(rules.ThrowsSpec, builder.BuildThrowsSpecRule());
            Nfa.BindRule(rules.RuleScopeSpec, builder.BuildRuleScopeSpecRule());
            Nfa.BindRule(rules.RuleAltList, builder.BuildRuleAltListRule());
            Nfa.BindRule(rules.Block, builder.BuildBlockRule());
            Nfa.BindRule(rules.Alternative, builder.BuildAlternativeRule());
            Nfa.BindRule(rules.ExceptionGroup, builder.BuildExceptionGroupRule());
            Nfa.BindRule(rules.ExceptionHandler, builder.BuildExceptionHandlerRule());
            Nfa.BindRule(rules.FinallyClause, builder.BuildFinallyClauseRule());
            Nfa.BindRule(rules.Element, builder.BuildElementRule());
            Nfa.BindRule(rules.ElementNoOptionSpec, builder.BuildElementNoOptionSpecRule());
            Nfa.BindRule(rules.Atom, builder.BuildAtomRule());
            Nfa.BindRule(rules.RuleRef, builder.BuildRuleRefRule());
            Nfa.BindRule(rules.NotSet, builder.BuildNotSetRule());
            Nfa.BindRule(rules.TreeRoot, builder.BuildTreeRootRule());
            Nfa.BindRule(rules.Tree, builder.BuildTreeRule());
            Nfa.BindRule(rules.Ebnf, builder.BuildEbnfRule());
            Nfa.BindRule(rules.Range, builder.BuildRangeRule());
            Nfa.BindRule(rules.Terminal, builder.BuildTerminalRule());
            Nfa.BindRule(rules.ElementOptions, builder.BuildElementOptionsRule());
            Nfa.BindRule(rules.DefaultNodeOption, builder.BuildDefaultNodeOptionRule());
            Nfa.BindRule(rules.ElementOption, builder.BuildElementOptionRule());
            Nfa.BindRule(rules.ElementOptionId, builder.BuildElementOptionIdRule());
            Nfa.BindRule(rules.EbnfSuffix, builder.BuildEbnfSuffixRule());
            Nfa.BindRule(rules.NotTerminal, builder.BuildNotTerminalRule());
            Nfa.BindRule(rules.IdList, builder.BuildIdListRule());
            Nfa.BindRule(rules.Id, builder.BuildIdRule());

            Nfa.BindRule(rules.Rewrite, builder.BuildRewriteRule());
            Nfa.BindRule(rules.RewriteWithSempred, builder.BuildRewriteWithSempredRule());
            Nfa.BindRule(rules.RewriteBlock, builder.BuildRewriteBlockRule());
            Nfa.BindRule(rules.RewriteAlternative, builder.BuildRewriteAlternativeRule());
            Nfa.BindRule(rules.RewriteElement, builder.BuildRewriteElementRule());
            Nfa.BindRule(rules.RewriteAtom, builder.BuildRewriteAtomRule());
            Nfa.BindRule(rules.Label, builder.BuildLabelRule());
            Nfa.BindRule(rules.RewriteEbnf, builder.BuildRewriteEbnfRule());
            Nfa.BindRule(rules.RewriteTree, builder.BuildRewriteTreeRule());
            Nfa.BindRule(rules.RewriteTemplate, builder.BuildRewriteTemplateRule());
            Nfa.BindRule(rules.RewriteTemplateHead, builder.BuildRewriteTemplateHeadRule());
            Nfa.BindRule(rules.RewriteIndirectTemplateHead, builder.BuildRewriteIndirectTemplateHeadRule());
            Nfa.BindRule(rules.RewriteTemplateArgs, builder.BuildRewriteTemplateArgsRule());
            Nfa.BindRule(rules.RewriteTemplateArg, builder.BuildRewriteTemplateArgRule());

            List<RuleBinding> ruleBindings = new List<RuleBinding>();

            ruleBindings.Add(rules.Grammar);
            ruleBindings.Add(rules.GrammarType);
            ruleBindings.Add(rules.Actions);
            ruleBindings.Add(rules.Action);
            ruleBindings.Add(rules.ActionScopeName);
            ruleBindings.Add(rules.OptionsSpec);
            ruleBindings.Add(rules.Option);
            ruleBindings.Add(rules.OptionValue);
            ruleBindings.Add(rules.DelegateGrammars);
            ruleBindings.Add(rules.DelegateGrammar);
            ruleBindings.Add(rules.TokensSpec);
            ruleBindings.Add(rules.TokenSpec);
            ruleBindings.Add(rules.AttrScopes);
            ruleBindings.Add(rules.AttrScope);
            ruleBindings.Add(rules.Rules);
            ruleBindings.Add(rules.Rule);
            ruleBindings.Add(rules.RuleActions);
            ruleBindings.Add(rules.RuleAction);
            ruleBindings.Add(rules.ThrowsSpec);
            ruleBindings.Add(rules.RuleScopeSpec);
            ruleBindings.Add(rules.RuleAltList);
            ruleBindings.Add(rules.Block);
            ruleBindings.Add(rules.Alternative);
            ruleBindings.Add(rules.ExceptionGroup);
            ruleBindings.Add(rules.ExceptionHandler);
            ruleBindings.Add(rules.FinallyClause);
            ruleBindings.Add(rules.Element);
            ruleBindings.Add(rules.ElementNoOptionSpec);
            ruleBindings.Add(rules.Atom);
            ruleBindings.Add(rules.RuleRef);
            ruleBindings.Add(rules.NotSet);
            ruleBindings.Add(rules.TreeRoot);
            ruleBindings.Add(rules.Tree);
            ruleBindings.Add(rules.Ebnf);
            ruleBindings.Add(rules.Range);
            ruleBindings.Add(rules.Terminal);
            ruleBindings.Add(rules.ElementOptions);
            ruleBindings.Add(rules.DefaultNodeOption);
            ruleBindings.Add(rules.ElementOption);
            ruleBindings.Add(rules.ElementOptionId);
            ruleBindings.Add(rules.EbnfSuffix);
            ruleBindings.Add(rules.NotTerminal);
            ruleBindings.Add(rules.IdList);
            ruleBindings.Add(rules.Id);

            ruleBindings.Add(rules.Rewrite);
            ruleBindings.Add(rules.RewriteWithSempred);
            ruleBindings.Add(rules.RewriteBlock);
            ruleBindings.Add(rules.RewriteAlternative);
            ruleBindings.Add(rules.RewriteElement);
            ruleBindings.Add(rules.RewriteAtom);
            ruleBindings.Add(rules.Label);
            ruleBindings.Add(rules.RewriteEbnf);
            ruleBindings.Add(rules.RewriteTree);
            ruleBindings.Add(rules.RewriteTemplate);
            ruleBindings.Add(rules.RewriteTemplateHead);
            ruleBindings.Add(rules.RewriteIndirectTemplateHead);
            ruleBindings.Add(rules.RewriteTemplateArgs);
            ruleBindings.Add(rules.RewriteTemplateArg);

            throw new NotImplementedException();
            _network = new Network(ruleBindings, null);
            return _network;
        }

        private Nfa BuildGrammarRule()
        {
            Nfa optionalAction = Nfa.Optional(Nfa.Match(ANTLRParser.ACTION));
            Nfa optionalDocComment = Nfa.Optional(Nfa.Match(ANTLRParser.DOC_COMMENT));

            Nfa grammarType = Nfa.Rule(Rules.GrammarType);
            Nfa id = Nfa.Rule(Rules.Id);
            Nfa semi = Nfa.Match(ANTLRParser.SEMI);

            Nfa optionalOptionsSpec = Nfa.Optional(Nfa.Rule(Rules.OptionsSpec));
            Nfa optionalDelegateGrammars = Nfa.Optional(Nfa.Rule(Rules.DelegateGrammars));

            Nfa optionalTokensSpec = Nfa.Optional(Nfa.Rule(Rules.TokensSpec));
            Nfa attrScopes = Nfa.Rule(Rules.AttrScopes);
            Nfa optionalActions = Nfa.Optional(Nfa.Rule(Rules.Actions));
            Nfa rules = Nfa.Rule(Rules.Rules);
            Nfa eof = Nfa.Match(ANTLRParser.EOF);

            Nfa body = Nfa.Sequence(
                optionalAction,
                optionalDocComment,
                grammarType,
                id,
                semi,
                optionalOptionsSpec,
                optionalDelegateGrammars,
                optionalTokensSpec,
                attrScopes,
                optionalActions,
                rules,
                eof);

            return body;
        }

        private Nfa BuildGrammarTypeRule()
        {
            Nfa type = Nfa.MatchAny(ANTLRParser.LEXER, ANTLRParser.PARSER, ANTLRParser.TREE);
            return Nfa.Sequence(Nfa.Optional(type), Nfa.Match(ANTLRParser.GRAMMAR));
        }

        private Nfa BuildActionsRule()
        {
            return Nfa.Sequence(Nfa.Rule(Rules.Action), Nfa.Closure(Nfa.Rule(Rules.Action)));
        }

        private Nfa BuildActionRule()
        {
            return Nfa.Sequence(
                Nfa.Match(ANTLRParser.AMPERSAND),
                Nfa.Optional(
                    Nfa.Sequence(
                        Nfa.Rule(Rules.ActionScopeName),
                        Nfa.Match(ANTLRParser.COLON),
                        Nfa.Match(ANTLRParser.COLON))),
                Nfa.Rule(Rules.Id),
                Nfa.Match(ANTLRParser.ACTION));
        }

        private Nfa BuildActionScopeNameRule()
        {
            return Nfa.Choice(
                Nfa.Rule(Rules.Id),
                Nfa.MatchAny(ANTLRParser.LEXER, ANTLRParser.PARSER));
        }

        private Nfa BuildOptionsSpecRule()
        {
            return Nfa.Sequence(
                Nfa.Match(ANTLRParser.OPTIONS),
                Nfa.Closure(
                    Nfa.Sequence(
                        Nfa.Rule(Rules.Option),
                        Nfa.Match(ANTLRParser.SEMI))),
                Nfa.Match(ANTLRParser.RCURLY));
        }

        private Nfa BuildOptionRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Rules.Id),
                Nfa.Match(ANTLRParser.ASSIGN),
                Nfa.Rule(Rules.OptionValue));
        }

        private Nfa BuildOptionValueRule()
        {
            return Nfa.Choice(
                Nfa.Rule(Rules.Id),
                Nfa.MatchAny(ANTLRParser.STRING_LITERAL, ANTLRParser.CHAR_LITERAL, ANTLRParser.INT, ANTLRParser.STAR));
        }

        private Nfa BuildDelegateGrammarsRule()
        {
            return Nfa.Sequence(
                Nfa.Match(ANTLRParser.IMPORT),
                Nfa.Rule(Rules.DelegateGrammar),
                Nfa.Closure(
                    Nfa.Sequence(
                        Nfa.Match(ANTLRParser.COMMA),
                        Nfa.Rule(Rules.DelegateGrammar))),
                Nfa.Match(ANTLRParser.SEMI));
        }

        private Nfa BuildDelegateGrammarRule()
        {
            return Nfa.Sequence(
                Nfa.Optional(
                    Nfa.Sequence(
                        Nfa.Rule(Rules.Id),
                        Nfa.Match(ANTLRParser.ASSIGN))),
                Nfa.Rule(Rules.Id));
        }

        private Nfa BuildTokensSpecRule()
        {
            return Nfa.Sequence(
                Nfa.Match(ANTLRParser.TOKENS),
                Nfa.Closure(Nfa.Rule(Rules.TokenSpec)),
                Nfa.Match(ANTLRParser.RCURLY));
        }

        private Nfa BuildTokenSpecRule()
        {
            return Nfa.Sequence(
                Nfa.Match(ANTLRParser.TOKEN_REF),
                Nfa.Optional(
                    Nfa.Sequence(
                        Nfa.Match(ANTLRParser.ASSIGN),
                        Nfa.MatchAny(ANTLRParser.STRING_LITERAL, ANTLRParser.CHAR_LITERAL))),
                Nfa.Match(ANTLRParser.SEMI));
        }

        private Nfa BuildAttrScopesRule()
        {
            return Nfa.Closure(Nfa.Rule(Rules.AttrScope));
        }

        private Nfa BuildAttrScopeRule()
        {
            return Nfa.Sequence(
                Nfa.Match(ANTLRParser.SCOPE),
                Nfa.Rule(Rules.Id),
                Nfa.Optional(Nfa.Rule(Rules.RuleActions)),
                Nfa.Match(ANTLRParser.ACTION));
        }

        private Nfa BuildRulesRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Rules.Rule),
                Nfa.Closure(Nfa.Rule(Rules.Rule)));
        }

        private Nfa BuildRuleRule()
        {
            return Nfa.Sequence(
                Nfa.Optional(Nfa.Match(ANTLRParser.DOC_COMMENT)),
                Nfa.Optional(Nfa.MatchAny(ANTLRParser.PROTECTED, ANTLRParser.PUBLIC, ANTLRParser.PRIVATE, ANTLRParser.FRAGMENT)),
                Nfa.Rule(Rules.Id),
                Nfa.Optional(Nfa.Match(ANTLRParser.BANG)),
                Nfa.Optional(Nfa.Match(ANTLRParser.ARG_ACTION)),
                Nfa.Optional(
                    Nfa.Sequence(
                        Nfa.Match(ANTLRParser.RETURNS),
                        Nfa.Match(ANTLRParser.ARG_ACTION))),
                Nfa.Optional(Nfa.Rule(Rules.ThrowsSpec)),
                Nfa.Optional(Nfa.Rule(Rules.OptionsSpec)),
                Nfa.Rule(Rules.RuleScopeSpec),
                Nfa.Optional(Nfa.Rule(Rules.RuleActions)),
                Nfa.Match(ANTLRParser.COLON),
                Nfa.Rule(Rules.RuleAltList),
                Nfa.Match(ANTLRParser.SEMI),
                Nfa.Optional(Nfa.Rule(Rules.ExceptionGroup)));
        }

        private Nfa BuildRuleActionsRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Rules.RuleAction),
                Nfa.Closure(Nfa.Rule(Rules.RuleAction)));
        }

        private Nfa BuildRuleActionRule()
        {
            return Nfa.Sequence(
                Nfa.Match(ANTLRParser.AMPERSAND),
                Nfa.Rule(Rules.Id),
                Nfa.Match(ANTLRParser.ACTION));
        }

        private Nfa BuildThrowsSpecRule()
        {
            return Nfa.Sequence(
                Nfa.Match(ANTLRParser.THROWS),
                Nfa.Rule(Rules.Id),
                Nfa.Closure(
                    Nfa.Sequence(
                        Nfa.Match(ANTLRParser.COMMA),
                        Nfa.Rule(Rules.Id))));
        }

        private Nfa BuildRuleScopeSpecRule()
        {
            return Nfa.Sequence(
                Nfa.Optional(
                    Nfa.Sequence(
                        Nfa.Match(ANTLRParser.SCOPE),
                        Nfa.Optional(Nfa.Rule(Rules.RuleActions)),
                        Nfa.Match(ANTLRParser.ACTION))),
                Nfa.Closure(
                    Nfa.Sequence(
                        Nfa.Match(ANTLRParser.SCOPE),
                        Nfa.Rule(Rules.IdList),
                        Nfa.Match(ANTLRParser.SEMI))));
        }

        private Nfa BuildRuleAltListRule()
        {
            return Nfa.Sequence(
                Nfa.Sequence(
                    Nfa.Rule(Rules.Alternative),
                    Nfa.Rule(Rules.Rewrite)),
                Nfa.Closure(
                    Nfa.Sequence(
                        Nfa.Match(ANTLRParser.OR),
                        Nfa.Rule(Rules.Alternative),
                        Nfa.Rule(Rules.Rewrite))));
        }

        private Nfa BuildBlockRule()
        {
            return Nfa.Sequence(
                Nfa.Match(ANTLRParser.LPAREN),
                Nfa.Optional(
                    Nfa.Choice(
                        Nfa.Sequence(
                            Nfa.Optional(Nfa.Rule(Rules.OptionsSpec)),
                            Nfa.Optional(Nfa.Rule(Rules.RuleActions)),
                            Nfa.Match(ANTLRParser.COLON)),
                        Nfa.Sequence(
                            Nfa.Match(ANTLRParser.ACTION),
                            Nfa.Match(ANTLRParser.COLON)))),
                Nfa.Rule(Rules.Alternative),
                Nfa.Rule(Rules.Rewrite),
                Nfa.Closure(
                    Nfa.Sequence(
                        Nfa.Match(ANTLRParser.OR),
                        Nfa.Rule(Rules.Alternative),
                        Nfa.Rule(Rules.Rewrite))),
                Nfa.Match(ANTLRParser.RPAREN));
        }

        private Nfa BuildAlternativeRule()
        {
            return Nfa.Closure(Nfa.Rule(Rules.Element));
        }

        private Nfa BuildExceptionGroupRule()
        {
            return Nfa.Choice(
                Nfa.Sequence(
                    Nfa.Rule(Rules.ExceptionHandler),
                    Nfa.Closure(Nfa.Rule(Rules.ExceptionHandler)),
                    Nfa.Optional(Nfa.Rule(Rules.FinallyClause))),
                Nfa.Rule(Rules.FinallyClause));
        }

        private Nfa BuildExceptionHandlerRule()
        {
            return Nfa.Sequence(
                Nfa.Match(ANTLRParser.CATCH),
                Nfa.Match(ANTLRParser.ARG_ACTION),
                Nfa.Match(ANTLRParser.ACTION));
        }

        private Nfa BuildFinallyClauseRule()
        {
            return Nfa.Sequence(
                Nfa.Match(ANTLRParser.FINALLY),
                Nfa.Match(ANTLRParser.ACTION));
        }

        private Nfa BuildElementRule()
        {
            return Nfa.Rule(Rules.ElementNoOptionSpec);
        }

        private Nfa BuildElementNoOptionSpecRule()
        {
            return Nfa.Choice(
                Nfa.Sequence(
                    Nfa.Rule(Rules.Id),
                    Nfa.MatchAny(ANTLRParser.ASSIGN, ANTLRParser.PLUS_ASSIGN),
                    Nfa.Choice(
                        Nfa.Rule(Rules.Atom),
                        Nfa.Rule(Rules.Block)),
                    Nfa.Optional(Nfa.Rule(Rules.EbnfSuffix))),
                Nfa.Sequence(
                    Nfa.Rule(Rules.Atom),
                    Nfa.Optional(Nfa.Rule(Rules.EbnfSuffix))),
                Nfa.Rule(Rules.Ebnf),
                Nfa.Match(ANTLRParser.FORCED_ACTION),
                Nfa.Match(ANTLRParser.ACTION),
                Nfa.Sequence(
                    Nfa.Match(ANTLRParser.SEMPRED),
                    Nfa.Optional(Nfa.Match(ANTLRParser.IMPLIES))),
                Nfa.Rule(Rules.Tree));
        }

        private Nfa BuildAtomRule()
        {
            return Nfa.Choice(
                Nfa.Sequence(
                    Nfa.Rule(Rules.Range),
                    Nfa.MatchAny(ANTLRParser.ROOT, ANTLRParser.BANG)),
                Nfa.Choice(
                    Nfa.Rule(Rules.Terminal),
                    Nfa.Rule(Rules.RuleRef)),
                Nfa.Sequence(
                    Nfa.Rule(Rules.NotSet),
                    Nfa.Optional(Nfa.MatchAny(ANTLRParser.BANG, ANTLRParser.BANG))));
        }

        private Nfa BuildRuleRefRule()
        {
            return Nfa.Sequence(
                Nfa.Match(ANTLRParser.RULE_REF),
                Nfa.Optional(Nfa.Match(ANTLRParser.ARG_ACTION)),
                Nfa.Optional(Nfa.MatchAny(ANTLRParser.ROOT, ANTLRParser.BANG)));
        }

        private Nfa BuildNotSetRule()
        {
            return Nfa.Sequence(
                Nfa.Match(ANTLRParser.NOT),
                Nfa.Choice(
                    Nfa.Rule(Rules.NotTerminal),
                    Nfa.Rule(Rules.Block)));
        }

        private Nfa BuildTreeRootRule()
        {
            return Nfa.Choice(
                Nfa.Sequence(
                    Nfa.Rule(Rules.Id),
                    Nfa.MatchAny(ANTLRParser.ASSIGN, ANTLRParser.PLUS_ASSIGN),
                    Nfa.Choice(
                        Nfa.Rule(Rules.Atom),
                        Nfa.Rule(Rules.Block))),
                Nfa.Rule(Rules.Atom),
                Nfa.Rule(Rules.Block));
        }

        private Nfa BuildTreeRule()
        {
            return Nfa.Sequence(
                Nfa.Match(ANTLRParser.TREE_BEGIN),
                Nfa.Rule(Rules.TreeRoot),
                Nfa.Rule(Rules.Element),
                Nfa.Closure(Nfa.Rule(Rules.Element)),
                Nfa.Match(ANTLRParser.RPAREN));
        }

        private Nfa BuildEbnfRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Rules.Block),
                Nfa.Optional(
                    Nfa.MatchAny(
                        ANTLRParser.QUESTION,
                        ANTLRParser.STAR,
                        ANTLRParser.PLUS,
                        ANTLRParser.IMPLIES,
                        ANTLRParser.ROOT,
                        ANTLRParser.BANG)));
        }

        private Nfa BuildRangeRule()
        {
            return Nfa.Sequence(
                Nfa.Match(ANTLRParser.CHAR_LITERAL),
                Nfa.Match(ANTLRParser.RANGE),
                Nfa.Match(ANTLRParser.CHAR_LITERAL));
        }

        private Nfa BuildTerminalRule()
        {
            return Nfa.Choice(
                Nfa.Sequence(
                    Nfa.Match(ANTLRParser.CHAR_LITERAL),
                    Nfa.Optional(Nfa.Rule(Rules.ElementOptions)),
                    Nfa.Optional(Nfa.MatchAny(ANTLRParser.ROOT, ANTLRParser.BANG))),
                Nfa.Sequence(
                    Nfa.MatchAny(ANTLRParser.TOKEN_REF),
                    Nfa.Optional(Nfa.Rule(Rules.ElementOptions)),
                    Nfa.Optional(Nfa.Match(ANTLRParser.ARG_ACTION)),
                    Nfa.Optional(Nfa.MatchAny(ANTLRParser.ROOT, ANTLRParser.BANG))),
                Nfa.Sequence(
                    Nfa.Match(ANTLRParser.STRING_LITERAL),
                    Nfa.Optional(Nfa.Rule(Rules.ElementOptions)),
                    Nfa.Optional(Nfa.MatchAny(ANTLRParser.ROOT, ANTLRParser.BANG))),
                Nfa.Sequence(
                    Nfa.Match(ANTLRParser.WILDCARD),
                    Nfa.Optional(Nfa.MatchAny(ANTLRParser.ROOT, ANTLRParser.BANG))));
        }

        private Nfa BuildElementOptionsRule()
        {
            return Nfa.Choice(
                Nfa.Sequence(
                    Nfa.Match(ANTLRParser.OPEN_ELEMENT_OPTION),
                    Nfa.Rule(Rules.DefaultNodeOption),
                    Nfa.Match(ANTLRParser.CLOSE_ELEMENT_OPTION)),
                Nfa.Sequence(
                    Nfa.Match(ANTLRParser.OPEN_ELEMENT_OPTION),
                    Nfa.Rule(Rules.ElementOption),
                    Nfa.Closure(
                        Nfa.Sequence(
                            Nfa.Match(ANTLRParser.SEMI),
                            Nfa.Rule(Rules.ElementOption))),
                    Nfa.Match(ANTLRParser.CLOSE_ELEMENT_OPTION)));
        }

        private Nfa BuildDefaultNodeOptionRule()
        {
            return Nfa.Rule(Rules.ElementOptionId);
        }

        private Nfa BuildElementOptionRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Rules.Id),
                Nfa.Match(ANTLRParser.ASSIGN),
                Nfa.Choice(
                    Nfa.Rule(Rules.ElementOptionId),
                    Nfa.MatchAny(ANTLRParser.STRING_LITERAL, ANTLRParser.DOUBLE_QUOTE_STRING_LITERAL, ANTLRParser.DOUBLE_ANGLE_STRING_LITERAL)));
        }

        private Nfa BuildElementOptionIdRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Rules.Id),
                Nfa.Closure(
                    Nfa.Sequence(
                        Nfa.Match(ANTLRParser.DOT),
                        Nfa.Rule(Rules.Id))));
        }

        private Nfa BuildEbnfSuffixRule()
        {
            return Nfa.MatchAny(ANTLRParser.QUESTION, ANTLRParser.STAR, ANTLRParser.PLUS);
        }

        private Nfa BuildNotTerminalRule()
        {
            return Nfa.MatchAny(ANTLRParser.CHAR_LITERAL, ANTLRParser.TOKEN_REF, ANTLRParser.STRING_LITERAL);
        }

        private Nfa BuildIdListRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Rules.Id),
                Nfa.Closure(
                    Nfa.Sequence(
                        Nfa.Match(ANTLRParser.COMMA),
                        Nfa.Rule(Rules.Id))));
        }

        private Nfa BuildIdRule()
        {
            return Nfa.MatchAny(ANTLRParser.TOKEN_REF, ANTLRParser.RULE_REF);
        }

        // rewrite syntax

        private Nfa BuildRewriteRule()
        {
            return Nfa.Optional(
                Nfa.Sequence(
                    Nfa.Closure(Nfa.Rule(Rules.RewriteWithSempred)),
                    Nfa.Match(ANTLRParser.REWRITE),
                    Nfa.Rule(Rules.RewriteAlternative)));
        }

        private Nfa BuildRewriteWithSempredRule()
        {
            return Nfa.Sequence(
                Nfa.Match(ANTLRParser.REWRITE),
                Nfa.Match(ANTLRParser.SEMPRED),
                Nfa.Rule(Rules.RewriteAlternative));
        }

        private Nfa BuildRewriteBlockRule()
        {
            return Nfa.Sequence(
                Nfa.Match(ANTLRParser.LPAREN),
                Nfa.Rule(Rules.RewriteAlternative),
                Nfa.Match(ANTLRParser.RPAREN));
        }

        private Nfa BuildRewriteAlternativeRule()
        {
            // can't handle semantic predicates now so assume BuildAST is true.
            return Nfa.Optional(
                Nfa.Choice(
                    Nfa.Sequence(
                        Nfa.Rule(Rules.RewriteElement),
                        Nfa.Closure(Nfa.Rule(Rules.RewriteElement))),
                    Nfa.Match(ANTLRParser.ETC)));
        }

        private Nfa BuildRewriteElementRule()
        {
            return Nfa.Choice(
                Nfa.Sequence(
                    Nfa.Rule(Rules.RewriteAtom),
                    Nfa.Optional(Nfa.Rule(Rules.EbnfSuffix))),
                Nfa.Rule(Rules.RewriteEbnf),
                Nfa.Sequence(
                    Nfa.Rule(Rules.RewriteTree),
                    Nfa.Optional(Nfa.Rule(Rules.EbnfSuffix))));
        }

        private Nfa BuildRewriteAtomRule()
        {
            return Nfa.Choice(
                Nfa.Sequence(
                    Nfa.Match(ANTLRParser.TOKEN_REF),
                    Nfa.Optional(Nfa.Rule(Rules.ElementOptions)),
                    Nfa.Optional(Nfa.Match(ANTLRParser.ARG_ACTION))),
                Nfa.Match(ANTLRParser.RULE_REF),
                Nfa.Sequence(
                    Nfa.Match(ANTLRParser.CHAR_LITERAL),
                    Nfa.Optional(Nfa.Rule(Rules.ElementOptions))),
                Nfa.Sequence(
                    Nfa.Match(ANTLRParser.STRING_LITERAL),
                    Nfa.Optional(Nfa.Rule(Rules.ElementOptions))),
                Nfa.Sequence(
                    Nfa.Match(ANTLRParser.DOLLAR),
                    Nfa.Rule(Rules.Label)),
                Nfa.Match(ANTLRParser.ACTION));
        }

        private Nfa BuildLabelRule()
        {
            return Nfa.MatchAny(ANTLRParser.TOKEN_REF, ANTLRParser.RULE_REF);
        }

        private Nfa BuildRewriteEbnfRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Rules.RewriteBlock),
                Nfa.MatchAny(ANTLRParser.QUESTION, ANTLRParser.STAR, ANTLRParser.PLUS));
        }

        private Nfa BuildRewriteTreeRule()
        {
            return Nfa.Sequence(
                Nfa.Match(ANTLRParser.TREE_BEGIN),
                Nfa.Rule(Rules.RewriteAtom),
                Nfa.Closure(Nfa.Rule(Rules.RewriteElement)),
                Nfa.Match(ANTLRParser.RPAREN));
        }

        private Nfa BuildRewriteTemplateRule()
        {
            // doesn't handle the "template" predicate yet
            return Nfa.Choice(
                Nfa.Rule(Rules.RewriteTemplateHead),
                Nfa.Rule(Rules.RewriteIndirectTemplateHead),
                Nfa.Match(ANTLRParser.ACTION));
        }

        private Nfa BuildRewriteTemplateHeadRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Rules.Id),
                Nfa.Match(ANTLRParser.LPAREN),
                Nfa.Rule(Rules.RewriteTemplateArgs),
                Nfa.Match(ANTLRParser.RPAREN));
        }

        private Nfa BuildRewriteIndirectTemplateHeadRule()
        {
            return Nfa.Sequence(
                Nfa.Match(ANTLRParser.LPAREN),
                Nfa.Match(ANTLRParser.ACTION),
                Nfa.Match(ANTLRParser.RPAREN),
                Nfa.Match(ANTLRParser.LPAREN),
                Nfa.Rule(Rules.RewriteTemplateArgs),
                Nfa.Match(ANTLRParser.RPAREN));
        }

        private Nfa BuildRewriteTemplateArgsRule()
        {
            return Nfa.Optional(
                Nfa.Sequence(
                    Nfa.Rule(Rules.RewriteTemplateArg),
                    Nfa.Closure(
                        Nfa.Sequence(
                            Nfa.Match(ANTLRParser.COMMA),
                            Nfa.Rule(Rules.RewriteTemplateArg)))));
        }

        private Nfa BuildRewriteTemplateArgRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Rules.Id),
                Nfa.Match(ANTLRParser.ASSIGN),
                Nfa.Match(ANTLRParser.ACTION));
        }

        private class RuleBindings
        {
            public readonly RuleBinding Grammar = new RuleBinding("Grammar");
            public readonly RuleBinding GrammarType = new RuleBinding("GrammarType");
            public readonly RuleBinding Actions = new RuleBinding("Actions");
            public readonly RuleBinding Action = new RuleBinding("Action");
            public readonly RuleBinding ActionScopeName = new RuleBinding("ActionScopeName");
            public readonly RuleBinding OptionsSpec = new RuleBinding("OptionsSpec");
            public readonly RuleBinding Option = new RuleBinding("Option");
            public readonly RuleBinding OptionValue = new RuleBinding("OptionValue");
            public readonly RuleBinding DelegateGrammars = new RuleBinding("DelegateGrammars");
            public readonly RuleBinding DelegateGrammar = new RuleBinding("DelegateGrammar");
            public readonly RuleBinding TokensSpec = new RuleBinding("TokensSpec");
            public readonly RuleBinding TokenSpec = new RuleBinding("TokenSpec");
            public readonly RuleBinding AttrScopes = new RuleBinding("AttrScopes");
            public readonly RuleBinding AttrScope = new RuleBinding("AttrScope");
            public readonly RuleBinding Rules = new RuleBinding("Rules");
            public readonly RuleBinding Rule = new RuleBinding("Rule");
            public readonly RuleBinding RuleActions = new RuleBinding("RuleActions");
            public readonly RuleBinding RuleAction = new RuleBinding("RuleAction");
            public readonly RuleBinding ThrowsSpec = new RuleBinding("ThrowsSpec");
            public readonly RuleBinding RuleScopeSpec = new RuleBinding("RuleScopeSpec");
            public readonly RuleBinding RuleAltList = new RuleBinding("RuleAltList");
            public readonly RuleBinding Block = new RuleBinding("Block");
            public readonly RuleBinding Alternative = new RuleBinding("Alternative");
            public readonly RuleBinding ExceptionGroup = new RuleBinding("ExceptionGroup");
            public readonly RuleBinding ExceptionHandler = new RuleBinding("ExceptionHandler");
            public readonly RuleBinding FinallyClause = new RuleBinding("FinallyClause");
            public readonly RuleBinding Element = new RuleBinding("Element");
            public readonly RuleBinding ElementNoOptionSpec = new RuleBinding("ElementNoOptionSpec");
            public readonly RuleBinding Atom = new RuleBinding("Atom");
            public readonly RuleBinding RuleRef = new RuleBinding("RuleRef");
            public readonly RuleBinding NotSet = new RuleBinding("NotSet");
            public readonly RuleBinding TreeRoot = new RuleBinding("TreeRoot");
            public readonly RuleBinding Tree = new RuleBinding("Tree");
            public readonly RuleBinding Ebnf = new RuleBinding("Ebnf");
            public readonly RuleBinding Range = new RuleBinding("Range");
            public readonly RuleBinding Terminal = new RuleBinding("Terminal");
            public readonly RuleBinding ElementOptions = new RuleBinding("ElementOptions");
            public readonly RuleBinding DefaultNodeOption = new RuleBinding("DefaultNodeOption");
            public readonly RuleBinding ElementOption = new RuleBinding("ElementOption");
            public readonly RuleBinding ElementOptionId = new RuleBinding("ElementOptionId");
            public readonly RuleBinding EbnfSuffix = new RuleBinding("EbnfSuffix");
            public readonly RuleBinding NotTerminal = new RuleBinding("NotTerminal");
            public readonly RuleBinding IdList = new RuleBinding("IdList");
            public readonly RuleBinding Id = new RuleBinding("Id");

            // rewrite syntax

            public readonly RuleBinding Rewrite = new RuleBinding("Rewrite");
            public readonly RuleBinding RewriteWithSempred = new RuleBinding("RewriteWithSempred");
            public readonly RuleBinding RewriteBlock = new RuleBinding("RewriteBlock");
            public readonly RuleBinding RewriteAlternative = new RuleBinding("RewriteAlternative");
            public readonly RuleBinding RewriteElement = new RuleBinding("RewriteElement");
            public readonly RuleBinding RewriteAtom = new RuleBinding("RewriteAtom");
            public readonly RuleBinding Label = new RuleBinding("Label");
            public readonly RuleBinding RewriteEbnf = new RuleBinding("RewriteEbnf");
            public readonly RuleBinding RewriteTree = new RuleBinding("RewriteTree");
            public readonly RuleBinding RewriteTemplate = new RuleBinding("RewriteTemplate");
            public readonly RuleBinding RewriteTemplateHead = new RuleBinding("RewriteTemplateHead");
            public readonly RuleBinding RewriteIndirectTemplateHead = new RuleBinding("RewriteIndirectTemplateHead");
            public readonly RuleBinding RewriteTemplateArgs = new RuleBinding("RewriteTemplateArgs");
            public readonly RuleBinding RewriteTemplateArg = new RuleBinding("RewriteTemplateArg");
        }
    }
}
