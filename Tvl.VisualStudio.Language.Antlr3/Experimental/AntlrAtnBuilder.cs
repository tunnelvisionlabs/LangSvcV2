namespace Tvl.VisualStudio.Language.Antlr3.Experimental
{
    using System.Collections.Generic;
    using Tvl.VisualStudio.Language.Parsing.Experimental.Atn;
    using ANTLRParser = global::Antlr3.Grammars.ANTLRParser;

    internal class AntlrAtnBuilder : NetworkBuilder
    {
        private readonly RuleBindings _ruleEndpoints;
        private readonly List<RuleBinding> _rules;

        public AntlrAtnBuilder()
        {
            _ruleEndpoints = new RuleBindings();

            _rules = new List<RuleBinding>()
                {
                    _ruleEndpoints.Grammar,
                    _ruleEndpoints.GrammarType,
                    _ruleEndpoints.Actions,
                    _ruleEndpoints.Action,
                    _ruleEndpoints.ActionScopeName,
                    _ruleEndpoints.OptionsSpec,
                    _ruleEndpoints.Option,
                    _ruleEndpoints.OptionValue,
                    _ruleEndpoints.DelegateGrammars,
                    _ruleEndpoints.DelegateGrammar,
                    _ruleEndpoints.TokensSpec,
                    _ruleEndpoints.TokenSpec,
                    _ruleEndpoints.AttrScopes,
                    _ruleEndpoints.AttrScope,
                    _ruleEndpoints.RulesRule,
                    _ruleEndpoints.Rule,
                    _ruleEndpoints.RuleActions,
                    _ruleEndpoints.RuleAction,
                    _ruleEndpoints.ThrowsSpec,
                    _ruleEndpoints.RuleScopeSpec,
                    _ruleEndpoints.RuleAltList,
                    _ruleEndpoints.Block,
                    _ruleEndpoints.Alternative,
                    _ruleEndpoints.ExceptionGroup,
                    _ruleEndpoints.ExceptionHandler,
                    _ruleEndpoints.FinallyClause,
                    _ruleEndpoints.Element,
                    _ruleEndpoints.ElementNoOptionSpec,
                    _ruleEndpoints.Atom,
                    _ruleEndpoints.RuleRef,
                    _ruleEndpoints.NotSet,
                    _ruleEndpoints.TreeRoot,
                    _ruleEndpoints.Tree,
                    _ruleEndpoints.Ebnf,
                    _ruleEndpoints.Range,
                    _ruleEndpoints.Terminal,
                    _ruleEndpoints.ElementOptions,
                    _ruleEndpoints.DefaultNodeOption,
                    _ruleEndpoints.ElementOption,
                    _ruleEndpoints.ElementOptionId,
                    _ruleEndpoints.EbnfSuffix,
                    _ruleEndpoints.NotTerminal,
                    _ruleEndpoints.IdList,
                    _ruleEndpoints.Id,

                    _ruleEndpoints.Rewrite,
                    _ruleEndpoints.RewriteWithSempred,
                    _ruleEndpoints.RewriteBlock,
                    _ruleEndpoints.RewriteAlternative,
                    _ruleEndpoints.RewriteElement,
                    _ruleEndpoints.RewriteAtom,
                    _ruleEndpoints.Label,
                    _ruleEndpoints.RewriteEbnf,
                    _ruleEndpoints.RewriteTree,
                    _ruleEndpoints.RewriteTemplate,
                    _ruleEndpoints.RewriteTemplateHead,
                    _ruleEndpoints.RewriteIndirectTemplateHead,
                    _ruleEndpoints.RewriteTemplateArgs,
                    _ruleEndpoints.RewriteTemplateArg,
                };
        }

        protected RuleBindings Bindings
        {
            get
            {
                return _ruleEndpoints;
            }
        }

        protected sealed override IList<RuleBinding> Rules
        {
            get
            {
                return _rules;
            }
        }

        protected sealed override void BindRules()
        {
            BindRulesImpl();
            // derived classes might remove some rules originally declared in this class
            _rules.RemoveAll(i => i.StartState.OutgoingTransitions.Count == 0);
        }

        protected virtual void BindRulesImpl()
        {
            TryBindRule(Bindings.Grammar, this.Grammar());
            TryBindRule(Bindings.GrammarType, this.GrammarType());
            TryBindRule(Bindings.Actions, this.Actions());
            TryBindRule(Bindings.Action, this.Action());
            TryBindRule(Bindings.ActionScopeName, this.ActionScopeName());
            TryBindRule(Bindings.OptionsSpec, this.OptionsSpec());
            TryBindRule(Bindings.Option, this.Option());
            TryBindRule(Bindings.OptionValue, this.OptionValue());
            TryBindRule(Bindings.DelegateGrammars, this.DelegateGrammars());
            TryBindRule(Bindings.DelegateGrammar, this.DelegateGrammar());
            TryBindRule(Bindings.TokensSpec, this.TokensSpec());
            TryBindRule(Bindings.TokenSpec, this.TokenSpec());
            TryBindRule(Bindings.AttrScopes, this.AttrScopes());
            TryBindRule(Bindings.AttrScope, this.AttrScope());
            TryBindRule(Bindings.RulesRule, this.RulesRule());
            TryBindRule(Bindings.Rule, this.Rule());
            TryBindRule(Bindings.RuleActions, this.RuleActions());
            TryBindRule(Bindings.RuleAction, this.RuleAction());
            TryBindRule(Bindings.ThrowsSpec, this.ThrowsSpec());
            TryBindRule(Bindings.RuleScopeSpec, this.RuleScopeSpec());
            TryBindRule(Bindings.RuleAltList, this.RuleAltList());
            TryBindRule(Bindings.Block, this.Block());
            TryBindRule(Bindings.Alternative, this.Alternative());
            TryBindRule(Bindings.ExceptionGroup, this.ExceptionGroup());
            TryBindRule(Bindings.ExceptionHandler, this.ExceptionHandler());
            TryBindRule(Bindings.FinallyClause, this.FinallyClause());
            TryBindRule(Bindings.Element, this.Element());
            TryBindRule(Bindings.ElementNoOptionSpec, this.ElementNoOptionSpec());
            TryBindRule(Bindings.Atom, this.Atom());
            TryBindRule(Bindings.RuleRef, this.RuleRef());
            TryBindRule(Bindings.NotSet, this.NotSet());
            TryBindRule(Bindings.TreeRoot, this.TreeRoot());
            TryBindRule(Bindings.Tree, this.Tree());
            TryBindRule(Bindings.Ebnf, this.Ebnf());
            TryBindRule(Bindings.Range, this.Range());
            TryBindRule(Bindings.Terminal, this.Terminal());
            TryBindRule(Bindings.ElementOptions, this.ElementOptions());
            TryBindRule(Bindings.DefaultNodeOption, this.DefaultNodeOption());
            TryBindRule(Bindings.ElementOption, this.ElementOption());
            TryBindRule(Bindings.ElementOptionId, this.ElementOptionId());
            TryBindRule(Bindings.EbnfSuffix, this.EbnfSuffix());
            TryBindRule(Bindings.NotTerminal, this.NotTerminal());
            TryBindRule(Bindings.IdList, this.IdList());
            TryBindRule(Bindings.Id, this.Id());

            TryBindRule(Bindings.Rewrite, this.Rewrite());
            TryBindRule(Bindings.RewriteWithSempred, this.RewriteWithSempred());
            TryBindRule(Bindings.RewriteBlock, this.RewriteBlock());
            TryBindRule(Bindings.RewriteAlternative, this.RewriteAlternative());
            TryBindRule(Bindings.RewriteElement, this.RewriteElement());
            TryBindRule(Bindings.RewriteAtom, this.RewriteAtom());
            TryBindRule(Bindings.Label, this.Label());
            TryBindRule(Bindings.RewriteEbnf, this.RewriteEbnf());
            TryBindRule(Bindings.RewriteTree, this.RewriteTree());
            TryBindRule(Bindings.RewriteTemplate, this.RewriteTemplate());
            TryBindRule(Bindings.RewriteTemplateHead, this.RewriteTemplateHead());
            TryBindRule(Bindings.RewriteIndirectTemplateHead, this.RewriteIndirectTemplateHead());
            TryBindRule(Bindings.RewriteTemplateArgs, this.RewriteTemplateArgs());
            TryBindRule(Bindings.RewriteTemplateArg, this.RewriteTemplateArg());

            Bindings.Grammar.IsStartRule = true;
        }

        protected virtual Nfa Grammar()
        {
            Nfa optionalAction = Nfa.Optional(Nfa.Match(ANTLRParser.ACTION));
            Nfa optionalDocComment = Nfa.Optional(Nfa.Match(ANTLRParser.DOC_COMMENT));

            Nfa grammarType = Nfa.Rule(Bindings.GrammarType);
            Nfa id = Nfa.Rule(Bindings.Id);
            Nfa semi = Nfa.Match(ANTLRParser.SEMI);

            Nfa optionalOptionsSpec = Nfa.Optional(Nfa.Rule(Bindings.OptionsSpec));
            Nfa optionalDelegateGrammars = Nfa.Optional(Nfa.Rule(Bindings.DelegateGrammars));

            Nfa optionalTokensSpec = Nfa.Optional(Nfa.Rule(Bindings.TokensSpec));
            Nfa attrScopes = Nfa.Rule(Bindings.AttrScopes);
            Nfa optionalActions = Nfa.Optional(Nfa.Rule(Bindings.Actions));
            Nfa rules = Nfa.Rule(Bindings.RulesRule);
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

        protected virtual Nfa GrammarType()
        {
            Nfa type = Nfa.MatchAny(ANTLRParser.LEXER, ANTLRParser.PARSER, ANTLRParser.TREE);
            return Nfa.Sequence(Nfa.Optional(type), Nfa.Match(ANTLRParser.GRAMMAR));
        }

        protected virtual Nfa Actions()
        {
            return Nfa.Sequence(Nfa.Rule(Bindings.Action), Nfa.Closure(Nfa.Rule(Bindings.Action)));
        }

        protected virtual Nfa Action()
        {
            return Nfa.Sequence(
                Nfa.Match(ANTLRParser.AMPERSAND),
                Nfa.Optional(
                    Nfa.Sequence(
                        Nfa.Rule(Bindings.ActionScopeName),
                        Nfa.Match(ANTLRParser.COLON),
                        Nfa.Match(ANTLRParser.COLON))),
                Nfa.Rule(Bindings.Id),
                Nfa.Match(ANTLRParser.ACTION));
        }

        protected virtual Nfa ActionScopeName()
        {
            return Nfa.Choice(
                Nfa.Rule(Bindings.Id),
                Nfa.MatchAny(ANTLRParser.LEXER, ANTLRParser.PARSER));
        }

        protected virtual Nfa OptionsSpec()
        {
            return Nfa.Sequence(
                Nfa.Match(ANTLRParser.OPTIONS),
                Nfa.Closure(
                    Nfa.Sequence(
                        Nfa.Rule(Bindings.Option),
                        Nfa.Match(ANTLRParser.SEMI))),
                Nfa.Match(ANTLRParser.RCURLY));
        }

        protected virtual Nfa Option()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.Id),
                Nfa.Match(ANTLRParser.ASSIGN),
                Nfa.Rule(Bindings.OptionValue));
        }

        protected virtual Nfa OptionValue()
        {
            return Nfa.Choice(
                Nfa.Rule(Bindings.Id),
                Nfa.MatchAny(ANTLRParser.STRING_LITERAL, ANTLRParser.CHAR_LITERAL, ANTLRParser.INT, ANTLRParser.STAR));
        }

        protected virtual Nfa DelegateGrammars()
        {
            return Nfa.Sequence(
                Nfa.Match(ANTLRParser.IMPORT),
                Nfa.Rule(Bindings.DelegateGrammar),
                Nfa.Closure(
                    Nfa.Sequence(
                        Nfa.Match(ANTLRParser.COMMA),
                        Nfa.Rule(Bindings.DelegateGrammar))),
                Nfa.Match(ANTLRParser.SEMI));
        }

        protected virtual Nfa DelegateGrammar()
        {
            return Nfa.Sequence(
                Nfa.Optional(
                    Nfa.Sequence(
                        Nfa.Rule(Bindings.Id),
                        Nfa.Match(ANTLRParser.ASSIGN))),
                Nfa.Rule(Bindings.Id));
        }

        protected virtual Nfa TokensSpec()
        {
            return Nfa.Sequence(
                Nfa.Match(ANTLRParser.TOKENS),
                Nfa.Closure(Nfa.Rule(Bindings.TokenSpec)),
                Nfa.Match(ANTLRParser.RCURLY));
        }

        protected virtual Nfa TokenSpec()
        {
            return Nfa.Sequence(
                Nfa.Match(ANTLRParser.TOKEN_REF),
                Nfa.Optional(
                    Nfa.Sequence(
                        Nfa.Match(ANTLRParser.ASSIGN),
                        Nfa.MatchAny(ANTLRParser.STRING_LITERAL, ANTLRParser.CHAR_LITERAL))),
                Nfa.Match(ANTLRParser.SEMI));
        }

        protected virtual Nfa AttrScopes()
        {
            return Nfa.Closure(Nfa.Rule(Bindings.AttrScope));
        }

        protected virtual Nfa AttrScope()
        {
            return Nfa.Sequence(
                Nfa.Match(ANTLRParser.SCOPE),
                Nfa.Rule(Bindings.Id),
                Nfa.Optional(Nfa.Rule(Bindings.RuleActions)),
                Nfa.Match(ANTLRParser.ACTION));
        }

        protected virtual Nfa RulesRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.Rule),
                Nfa.Closure(Nfa.Rule(Bindings.Rule)));
        }

        protected virtual Nfa Rule()
        {
            return Nfa.Sequence(
                Nfa.Optional(Nfa.Match(ANTLRParser.DOC_COMMENT)),
                Nfa.Optional(Nfa.MatchAny(ANTLRParser.PROTECTED, ANTLRParser.PUBLIC, ANTLRParser.PRIVATE, ANTLRParser.FRAGMENT)),
                Nfa.Rule(Bindings.Id),
                Nfa.Optional(Nfa.Match(ANTLRParser.BANG)),
                Nfa.Optional(Nfa.Match(ANTLRParser.ARG_ACTION)),
                Nfa.Optional(
                    Nfa.Sequence(
                        Nfa.Match(ANTLRParser.RETURNS),
                        Nfa.Match(ANTLRParser.ARG_ACTION))),
                Nfa.Optional(Nfa.Rule(Bindings.ThrowsSpec)),
                Nfa.Optional(Nfa.Rule(Bindings.OptionsSpec)),
                Nfa.Rule(Bindings.RuleScopeSpec),
                Nfa.Optional(Nfa.Rule(Bindings.RuleActions)),
                Nfa.Match(ANTLRParser.COLON),
                Nfa.Rule(Bindings.RuleAltList),
                Nfa.Match(ANTLRParser.SEMI),
                Nfa.Optional(Nfa.Rule(Bindings.ExceptionGroup)));
        }

        protected virtual Nfa RuleActions()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.RuleAction),
                Nfa.Closure(Nfa.Rule(Bindings.RuleAction)));
        }

        protected virtual Nfa RuleAction()
        {
            return Nfa.Sequence(
                Nfa.Match(ANTLRParser.AMPERSAND),
                Nfa.Rule(Bindings.Id),
                Nfa.Match(ANTLRParser.ACTION));
        }

        protected virtual Nfa ThrowsSpec()
        {
            return Nfa.Sequence(
                Nfa.Match(ANTLRParser.THROWS),
                Nfa.Rule(Bindings.Id),
                Nfa.Closure(
                    Nfa.Sequence(
                        Nfa.Match(ANTLRParser.COMMA),
                        Nfa.Rule(Bindings.Id))));
        }

        protected virtual Nfa RuleScopeSpec()
        {
            return Nfa.Sequence(
                Nfa.Optional(
                    Nfa.Sequence(
                        Nfa.Match(ANTLRParser.SCOPE),
                        Nfa.Optional(Nfa.Rule(Bindings.RuleActions)),
                        Nfa.Match(ANTLRParser.ACTION))),
                Nfa.Closure(
                    Nfa.Sequence(
                        Nfa.Match(ANTLRParser.SCOPE),
                        Nfa.Rule(Bindings.IdList),
                        Nfa.Match(ANTLRParser.SEMI))));
        }

        protected virtual Nfa RuleAltList()
        {
            return Nfa.Sequence(
                Nfa.Sequence(
                    Nfa.Rule(Bindings.Alternative),
                    Nfa.Rule(Bindings.Rewrite)),
                Nfa.Closure(
                    Nfa.Sequence(
                        Nfa.Match(ANTLRParser.OR),
                        Nfa.Rule(Bindings.Alternative),
                        Nfa.Rule(Bindings.Rewrite))));
        }

        protected virtual Nfa Block()
        {
            return Nfa.Sequence(
                Nfa.Match(ANTLRParser.LPAREN),
                Nfa.Optional(
                    Nfa.Choice(
                        Nfa.Sequence(
                            Nfa.Optional(Nfa.Rule(Bindings.OptionsSpec)),
                            Nfa.Optional(Nfa.Rule(Bindings.RuleActions)),
                            Nfa.Match(ANTLRParser.COLON)),
                        Nfa.Sequence(
                            Nfa.Match(ANTLRParser.ACTION),
                            Nfa.Match(ANTLRParser.COLON)))),
                Nfa.Rule(Bindings.Alternative),
                Nfa.Rule(Bindings.Rewrite),
                Nfa.Closure(
                    Nfa.Sequence(
                        Nfa.Match(ANTLRParser.OR),
                        Nfa.Rule(Bindings.Alternative),
                        Nfa.Rule(Bindings.Rewrite))),
                Nfa.Match(ANTLRParser.RPAREN));
        }

        protected virtual Nfa Alternative()
        {
            return Nfa.Closure(Nfa.Rule(Bindings.Element));
        }

        protected virtual Nfa ExceptionGroup()
        {
            return Nfa.Choice(
                Nfa.Sequence(
                    Nfa.Rule(Bindings.ExceptionHandler),
                    Nfa.Closure(Nfa.Rule(Bindings.ExceptionHandler)),
                    Nfa.Optional(Nfa.Rule(Bindings.FinallyClause))),
                Nfa.Rule(Bindings.FinallyClause));
        }

        protected virtual Nfa ExceptionHandler()
        {
            return Nfa.Sequence(
                Nfa.Match(ANTLRParser.CATCH),
                Nfa.Match(ANTLRParser.ARG_ACTION),
                Nfa.Match(ANTLRParser.ACTION));
        }

        protected virtual Nfa FinallyClause()
        {
            return Nfa.Sequence(
                Nfa.Match(ANTLRParser.FINALLY),
                Nfa.Match(ANTLRParser.ACTION));
        }

        protected virtual Nfa Element()
        {
            return Nfa.Rule(Bindings.ElementNoOptionSpec);
        }

        protected virtual Nfa ElementNoOptionSpec()
        {
            return Nfa.Choice(
                Nfa.Sequence(
                    Nfa.Rule(Bindings.Id),
                    Nfa.MatchAny(ANTLRParser.ASSIGN, ANTLRParser.PLUS_ASSIGN),
                    Nfa.Choice(
                        Nfa.Rule(Bindings.Atom),
                        Nfa.Rule(Bindings.Block)),
                    Nfa.Optional(Nfa.Rule(Bindings.EbnfSuffix))),
                Nfa.Sequence(
                    Nfa.Rule(Bindings.Atom),
                    Nfa.Optional(Nfa.Rule(Bindings.EbnfSuffix))),
                Nfa.Rule(Bindings.Ebnf),
                Nfa.Match(ANTLRParser.FORCED_ACTION),
                Nfa.Match(ANTLRParser.ACTION),
                Nfa.Sequence(
                    Nfa.Match(ANTLRParser.SEMPRED),
                    Nfa.Optional(Nfa.Match(ANTLRParser.IMPLIES))),
                Nfa.Rule(Bindings.Tree));
        }

        protected virtual Nfa Atom()
        {
            return Nfa.Choice(
                Nfa.Sequence(
                    Nfa.Rule(Bindings.Range),
                    Nfa.MatchAny(ANTLRParser.ROOT, ANTLRParser.BANG)),
                Nfa.Choice(
                    Nfa.Rule(Bindings.Terminal),
                    Nfa.Rule(Bindings.RuleRef)),
                Nfa.Sequence(
                    Nfa.Rule(Bindings.NotSet),
                    Nfa.Optional(Nfa.MatchAny(ANTLRParser.BANG, ANTLRParser.BANG))));
        }

        protected virtual Nfa RuleRef()
        {
            return Nfa.Sequence(
                Nfa.Match(ANTLRParser.RULE_REF),
                Nfa.Optional(Nfa.Match(ANTLRParser.ARG_ACTION)),
                Nfa.Optional(Nfa.MatchAny(ANTLRParser.ROOT, ANTLRParser.BANG)));
        }

        protected virtual Nfa NotSet()
        {
            return Nfa.Sequence(
                Nfa.Match(ANTLRParser.NOT),
                Nfa.Choice(
                    Nfa.Rule(Bindings.NotTerminal),
                    Nfa.Rule(Bindings.Block)));
        }

        protected virtual Nfa TreeRoot()
        {
            return Nfa.Choice(
                Nfa.Sequence(
                    Nfa.Rule(Bindings.Id),
                    Nfa.MatchAny(ANTLRParser.ASSIGN, ANTLRParser.PLUS_ASSIGN),
                    Nfa.Choice(
                        Nfa.Rule(Bindings.Atom),
                        Nfa.Rule(Bindings.Block))),
                Nfa.Rule(Bindings.Atom),
                Nfa.Rule(Bindings.Block));
        }

        protected virtual Nfa Tree()
        {
            return Nfa.Sequence(
                Nfa.Match(ANTLRParser.TREE_BEGIN),
                Nfa.Rule(Bindings.TreeRoot),
                Nfa.Rule(Bindings.Element),
                Nfa.Closure(Nfa.Rule(Bindings.Element)),
                Nfa.Match(ANTLRParser.RPAREN));
        }

        protected virtual Nfa Ebnf()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.Block),
                Nfa.Optional(
                    Nfa.MatchAny(
                        ANTLRParser.QUESTION,
                        ANTLRParser.STAR,
                        ANTLRParser.PLUS,
                        ANTLRParser.IMPLIES,
                        ANTLRParser.ROOT,
                        ANTLRParser.BANG)));
        }

        protected virtual Nfa Range()
        {
            return Nfa.Sequence(
                Nfa.Match(ANTLRParser.CHAR_LITERAL),
                Nfa.Match(ANTLRParser.RANGE),
                Nfa.Match(ANTLRParser.CHAR_LITERAL));
        }

        protected virtual Nfa Terminal()
        {
            return Nfa.Choice(
                Nfa.Sequence(
                    Nfa.Match(ANTLRParser.CHAR_LITERAL),
                    Nfa.Optional(Nfa.Rule(Bindings.ElementOptions)),
                    Nfa.Optional(Nfa.MatchAny(ANTLRParser.ROOT, ANTLRParser.BANG))),
                Nfa.Sequence(
                    Nfa.MatchAny(ANTLRParser.TOKEN_REF),
                    Nfa.Optional(Nfa.Rule(Bindings.ElementOptions)),
                    Nfa.Optional(Nfa.Match(ANTLRParser.ARG_ACTION)),
                    Nfa.Optional(Nfa.MatchAny(ANTLRParser.ROOT, ANTLRParser.BANG))),
                Nfa.Sequence(
                    Nfa.Match(ANTLRParser.STRING_LITERAL),
                    Nfa.Optional(Nfa.Rule(Bindings.ElementOptions)),
                    Nfa.Optional(Nfa.MatchAny(ANTLRParser.ROOT, ANTLRParser.BANG))),
                Nfa.Sequence(
                    Nfa.Match(ANTLRParser.WILDCARD),
                    Nfa.Optional(Nfa.MatchAny(ANTLRParser.ROOT, ANTLRParser.BANG))));
        }

        protected virtual Nfa ElementOptions()
        {
            return Nfa.Choice(
                Nfa.Sequence(
                    Nfa.Match(ANTLRParser.OPEN_ELEMENT_OPTION),
                    Nfa.Rule(Bindings.DefaultNodeOption),
                    Nfa.Match(ANTLRParser.CLOSE_ELEMENT_OPTION)),
                Nfa.Sequence(
                    Nfa.Match(ANTLRParser.OPEN_ELEMENT_OPTION),
                    Nfa.Rule(Bindings.ElementOption),
                    Nfa.Closure(
                        Nfa.Sequence(
                            Nfa.Match(ANTLRParser.SEMI),
                            Nfa.Rule(Bindings.ElementOption))),
                    Nfa.Match(ANTLRParser.CLOSE_ELEMENT_OPTION)));
        }

        protected virtual Nfa DefaultNodeOption()
        {
            return Nfa.Rule(Bindings.ElementOptionId);
        }

        protected virtual Nfa ElementOption()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.Id),
                Nfa.Match(ANTLRParser.ASSIGN),
                Nfa.Choice(
                    Nfa.Rule(Bindings.ElementOptionId),
                    Nfa.MatchAny(ANTLRParser.STRING_LITERAL, ANTLRParser.DOUBLE_QUOTE_STRING_LITERAL, ANTLRParser.DOUBLE_ANGLE_STRING_LITERAL)));
        }

        protected virtual Nfa ElementOptionId()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.Id),
                Nfa.Closure(
                    Nfa.Sequence(
                        Nfa.Match(ANTLRParser.DOT),
                        Nfa.Rule(Bindings.Id))));
        }

        protected virtual Nfa EbnfSuffix()
        {
            return Nfa.MatchAny(ANTLRParser.QUESTION, ANTLRParser.STAR, ANTLRParser.PLUS);
        }

        protected virtual Nfa NotTerminal()
        {
            return Nfa.MatchAny(ANTLRParser.CHAR_LITERAL, ANTLRParser.TOKEN_REF, ANTLRParser.STRING_LITERAL);
        }

        protected virtual Nfa IdList()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.Id),
                Nfa.Closure(
                    Nfa.Sequence(
                        Nfa.Match(ANTLRParser.COMMA),
                        Nfa.Rule(Bindings.Id))));
        }

        protected virtual Nfa Id()
        {
            return Nfa.MatchAny(ANTLRParser.TOKEN_REF, ANTLRParser.RULE_REF);
        }

        // rewrite syntax

        protected virtual Nfa Rewrite()
        {
            return Nfa.Optional(
                Nfa.Sequence(
                    Nfa.Closure(Nfa.Rule(Bindings.RewriteWithSempred)),
                    Nfa.Match(ANTLRParser.REWRITE),
                    Nfa.Rule(Bindings.RewriteAlternative)));
        }

        protected virtual Nfa RewriteWithSempred()
        {
            return Nfa.Sequence(
                Nfa.Match(ANTLRParser.REWRITE),
                Nfa.Match(ANTLRParser.SEMPRED),
                Nfa.Rule(Bindings.RewriteAlternative));
        }

        protected virtual Nfa RewriteBlock()
        {
            return Nfa.Sequence(
                Nfa.Match(ANTLRParser.LPAREN),
                Nfa.Rule(Bindings.RewriteAlternative),
                Nfa.Match(ANTLRParser.RPAREN));
        }

        protected virtual Nfa RewriteAlternative()
        {
            // can't handle semantic predicates now so assume BuildAST is true.
            return Nfa.Optional(
                Nfa.Choice(
                    Nfa.Sequence(
                        Nfa.Rule(Bindings.RewriteElement),
                        Nfa.Closure(Nfa.Rule(Bindings.RewriteElement))),
                    Nfa.Match(ANTLRParser.ETC)));
        }

        protected virtual Nfa RewriteElement()
        {
            return Nfa.Choice(
                Nfa.Sequence(
                    Nfa.Rule(Bindings.RewriteAtom),
                    Nfa.Optional(Nfa.Rule(Bindings.EbnfSuffix))),
                Nfa.Rule(Bindings.RewriteEbnf),
                Nfa.Sequence(
                    Nfa.Rule(Bindings.RewriteTree),
                    Nfa.Optional(Nfa.Rule(Bindings.EbnfSuffix))));
        }

        protected virtual Nfa RewriteAtom()
        {
            return Nfa.Choice(
                Nfa.Sequence(
                    Nfa.Match(ANTLRParser.TOKEN_REF),
                    Nfa.Optional(Nfa.Rule(Bindings.ElementOptions)),
                    Nfa.Optional(Nfa.Match(ANTLRParser.ARG_ACTION))),
                Nfa.Match(ANTLRParser.RULE_REF),
                Nfa.Sequence(
                    Nfa.Match(ANTLRParser.CHAR_LITERAL),
                    Nfa.Optional(Nfa.Rule(Bindings.ElementOptions))),
                Nfa.Sequence(
                    Nfa.Match(ANTLRParser.STRING_LITERAL),
                    Nfa.Optional(Nfa.Rule(Bindings.ElementOptions))),
                Nfa.Sequence(
                    Nfa.Match(ANTLRParser.DOLLAR),
                    Nfa.Rule(Bindings.Label)),
                Nfa.Match(ANTLRParser.ACTION));
        }

        protected virtual Nfa Label()
        {
            return Nfa.MatchAny(ANTLRParser.TOKEN_REF, ANTLRParser.RULE_REF);
        }

        protected virtual Nfa RewriteEbnf()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.RewriteBlock),
                Nfa.MatchAny(ANTLRParser.QUESTION, ANTLRParser.STAR, ANTLRParser.PLUS));
        }

        protected virtual Nfa RewriteTree()
        {
            return Nfa.Sequence(
                Nfa.Match(ANTLRParser.TREE_BEGIN),
                Nfa.Rule(Bindings.RewriteAtom),
                Nfa.Closure(Nfa.Rule(Bindings.RewriteElement)),
                Nfa.Match(ANTLRParser.RPAREN));
        }

        protected virtual Nfa RewriteTemplate()
        {
            // doesn't handle the "template" predicate yet
            return Nfa.Choice(
                Nfa.Rule(Bindings.RewriteTemplateHead),
                Nfa.Rule(Bindings.RewriteIndirectTemplateHead),
                Nfa.Match(ANTLRParser.ACTION));
        }

        protected virtual Nfa RewriteTemplateHead()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.Id),
                Nfa.Match(ANTLRParser.LPAREN),
                Nfa.Rule(Bindings.RewriteTemplateArgs),
                Nfa.Match(ANTLRParser.RPAREN));
        }

        protected virtual Nfa RewriteIndirectTemplateHead()
        {
            return Nfa.Sequence(
                Nfa.Match(ANTLRParser.LPAREN),
                Nfa.Match(ANTLRParser.ACTION),
                Nfa.Match(ANTLRParser.RPAREN),
                Nfa.Match(ANTLRParser.LPAREN),
                Nfa.Rule(Bindings.RewriteTemplateArgs),
                Nfa.Match(ANTLRParser.RPAREN));
        }

        protected virtual Nfa RewriteTemplateArgs()
        {
            return Nfa.Optional(
                Nfa.Sequence(
                    Nfa.Rule(Bindings.RewriteTemplateArg),
                    Nfa.Closure(
                        Nfa.Sequence(
                            Nfa.Match(ANTLRParser.COMMA),
                            Nfa.Rule(Bindings.RewriteTemplateArg)))));
        }

        protected virtual Nfa RewriteTemplateArg()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.Id),
                Nfa.Match(ANTLRParser.ASSIGN),
                Nfa.Match(ANTLRParser.ACTION));
        }

        public static class RuleNames
        {
            public static readonly string Grammar = "Grammar";
            public static readonly string GrammarType = "GrammarType";
            public static readonly string Actions = "Actions";
            public static readonly string Action = "Action";
            public static readonly string ActionScopeName = "ActionScopeName";
            public static readonly string OptionsSpec = "OptionsSpec";
            public static readonly string Option = "Option";
            public static readonly string OptionValue = "OptionValue";
            public static readonly string DelegateGrammars = "DelegateGrammars";
            public static readonly string DelegateGrammar = "DelegateGrammar";
            public static readonly string TokensSpec = "TokensSpec";
            public static readonly string TokenSpec = "TokenSpec";
            public static readonly string AttrScopes = "AttrScopes";
            public static readonly string AttrScope = "AttrScope";
            public static readonly string RulesRule = "Rules";
            public static readonly string Rule = "Rule";
            public static readonly string RuleActions = "RuleActions";
            public static readonly string RuleAction = "RuleAction";
            public static readonly string ThrowsSpec = "ThrowsSpec";
            public static readonly string RuleScopeSpec = "RuleScopeSpec";
            public static readonly string RuleAltList = "RuleAltList";
            public static readonly string Block = "Block";
            public static readonly string Alternative = "Alternative";
            public static readonly string ExceptionGroup = "ExceptionGroup";
            public static readonly string ExceptionHandler = "ExceptionHandler";
            public static readonly string FinallyClause = "FinallyClause";
            public static readonly string Element = "Element";
            public static readonly string ElementNoOptionSpec = "ElementNoOptionSpec";
            public static readonly string Atom = "Atom";
            public static readonly string RuleRef = "RuleRef";
            public static readonly string NotSet = "NotSet";
            public static readonly string TreeRoot = "TreeRoot";
            public static readonly string Tree = "Tree";
            public static readonly string Ebnf = "Ebnf";
            public static readonly string Range = "Range";
            public static readonly string Terminal = "Terminal";
            public static readonly string ElementOptions = "ElementOptions";
            public static readonly string DefaultNodeOption = "DefaultNodeOption";
            public static readonly string ElementOption = "ElementOption";
            public static readonly string ElementOptionId = "ElementOptionId";
            public static readonly string EbnfSuffix = "EbnfSuffix";
            public static readonly string NotTerminal = "NotTerminal";
            public static readonly string IdList = "IdList";
            public static readonly string Id = "Id";

            // rewrite syntax

            public static readonly string Rewrite = "Rewrite";
            public static readonly string RewriteWithSempred = "RewriteWithSempred";
            public static readonly string RewriteBlock = "RewriteBlock";
            public static readonly string RewriteAlternative = "RewriteAlternative";
            public static readonly string RewriteElement = "RewriteElement";
            public static readonly string RewriteAtom = "RewriteAtom";
            public static readonly string Label = "Label";
            public static readonly string RewriteEbnf = "RewriteEbnf";
            public static readonly string RewriteTree = "RewriteTree";
            public static readonly string RewriteTemplate = "RewriteTemplate";
            public static readonly string RewriteTemplateHead = "RewriteTemplateHead";
            public static readonly string RewriteIndirectTemplateHead = "RewriteIndirectTemplateHead";
            public static readonly string RewriteTemplateArgs = "RewriteTemplateArgs";
            public static readonly string RewriteTemplateArg = "RewriteTemplateArg";
        }

        protected class RuleBindings
        {
            public readonly RuleBinding Grammar = new RuleBinding(RuleNames.Grammar);
            public readonly RuleBinding GrammarType = new RuleBinding(RuleNames.GrammarType);
            public readonly RuleBinding Actions = new RuleBinding(RuleNames.Actions);
            public readonly RuleBinding Action = new RuleBinding(RuleNames.Action);
            public readonly RuleBinding ActionScopeName = new RuleBinding(RuleNames.ActionScopeName);
            public readonly RuleBinding OptionsSpec = new RuleBinding(RuleNames.OptionsSpec);
            public readonly RuleBinding Option = new RuleBinding(RuleNames.Option);
            public readonly RuleBinding OptionValue = new RuleBinding(RuleNames.OptionValue);
            public readonly RuleBinding DelegateGrammars = new RuleBinding(RuleNames.DelegateGrammars);
            public readonly RuleBinding DelegateGrammar = new RuleBinding(RuleNames.DelegateGrammar);
            public readonly RuleBinding TokensSpec = new RuleBinding(RuleNames.TokensSpec);
            public readonly RuleBinding TokenSpec = new RuleBinding(RuleNames.TokenSpec);
            public readonly RuleBinding AttrScopes = new RuleBinding(RuleNames.AttrScopes);
            public readonly RuleBinding AttrScope = new RuleBinding(RuleNames.AttrScope);
            public readonly RuleBinding RulesRule = new RuleBinding(RuleNames.RulesRule);
            public readonly RuleBinding Rule = new RuleBinding(RuleNames.Rule);
            public readonly RuleBinding RuleActions = new RuleBinding(RuleNames.RuleActions);
            public readonly RuleBinding RuleAction = new RuleBinding(RuleNames.RuleAction);
            public readonly RuleBinding ThrowsSpec = new RuleBinding(RuleNames.ThrowsSpec);
            public readonly RuleBinding RuleScopeSpec = new RuleBinding(RuleNames.RuleScopeSpec);
            public readonly RuleBinding RuleAltList = new RuleBinding(RuleNames.RuleAltList);
            public readonly RuleBinding Block = new RuleBinding(RuleNames.Block);
            public readonly RuleBinding Alternative = new RuleBinding(RuleNames.Alternative);
            public readonly RuleBinding ExceptionGroup = new RuleBinding(RuleNames.ExceptionGroup);
            public readonly RuleBinding ExceptionHandler = new RuleBinding(RuleNames.ExceptionHandler);
            public readonly RuleBinding FinallyClause = new RuleBinding(RuleNames.FinallyClause);
            public readonly RuleBinding Element = new RuleBinding(RuleNames.Element);
            public readonly RuleBinding ElementNoOptionSpec = new RuleBinding(RuleNames.ElementNoOptionSpec);
            public readonly RuleBinding Atom = new RuleBinding(RuleNames.Atom);
            public readonly RuleBinding RuleRef = new RuleBinding(RuleNames.RuleRef);
            public readonly RuleBinding NotSet = new RuleBinding(RuleNames.NotSet);
            public readonly RuleBinding TreeRoot = new RuleBinding(RuleNames.TreeRoot);
            public readonly RuleBinding Tree = new RuleBinding(RuleNames.Tree);
            public readonly RuleBinding Ebnf = new RuleBinding(RuleNames.Ebnf);
            public readonly RuleBinding Range = new RuleBinding(RuleNames.Range);
            public readonly RuleBinding Terminal = new RuleBinding(RuleNames.Terminal);
            public readonly RuleBinding ElementOptions = new RuleBinding(RuleNames.ElementOptions);
            public readonly RuleBinding DefaultNodeOption = new RuleBinding(RuleNames.DefaultNodeOption);
            public readonly RuleBinding ElementOption = new RuleBinding(RuleNames.ElementOption);
            public readonly RuleBinding ElementOptionId = new RuleBinding(RuleNames.ElementOptionId);
            public readonly RuleBinding EbnfSuffix = new RuleBinding(RuleNames.EbnfSuffix);
            public readonly RuleBinding NotTerminal = new RuleBinding(RuleNames.NotTerminal);
            public readonly RuleBinding IdList = new RuleBinding(RuleNames.IdList);
            public readonly RuleBinding Id = new RuleBinding(RuleNames.Id);

            // rewrite syntax

            public readonly RuleBinding Rewrite = new RuleBinding(RuleNames.Rewrite);
            public readonly RuleBinding RewriteWithSempred = new RuleBinding(RuleNames.RewriteWithSempred);
            public readonly RuleBinding RewriteBlock = new RuleBinding(RuleNames.RewriteBlock);
            public readonly RuleBinding RewriteAlternative = new RuleBinding(RuleNames.RewriteAlternative);
            public readonly RuleBinding RewriteElement = new RuleBinding(RuleNames.RewriteElement);
            public readonly RuleBinding RewriteAtom = new RuleBinding(RuleNames.RewriteAtom);
            public readonly RuleBinding Label = new RuleBinding(RuleNames.Label);
            public readonly RuleBinding RewriteEbnf = new RuleBinding(RuleNames.RewriteEbnf);
            public readonly RuleBinding RewriteTree = new RuleBinding(RuleNames.RewriteTree);
            public readonly RuleBinding RewriteTemplate = new RuleBinding(RuleNames.RewriteTemplate);
            public readonly RuleBinding RewriteTemplateHead = new RuleBinding(RuleNames.RewriteTemplateHead);
            public readonly RuleBinding RewriteIndirectTemplateHead = new RuleBinding(RuleNames.RewriteIndirectTemplateHead);
            public readonly RuleBinding RewriteTemplateArgs = new RuleBinding(RuleNames.RewriteTemplateArgs);
            public readonly RuleBinding RewriteTemplateArg = new RuleBinding(RuleNames.RewriteTemplateArg);
        }
    }
}
