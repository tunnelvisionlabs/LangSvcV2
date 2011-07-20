namespace Tvl.VisualStudio.Language.Alloy.Experimental
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Xml.Linq;
    using Tvl.VisualStudio.Language.Parsing.Experimental.Atn;
    using StringBuilder = System.Text.StringBuilder;

    internal class AlloySimplifiedAtnBuilder : NetworkBuilder<AlloySimplifiedAtnBuilder>
    {
        private static Network _network;

        private readonly RuleBindings _ruleBindings;
        private readonly IList<RuleBinding> _rules;

        public AlloySimplifiedAtnBuilder()
        {
            _ruleBindings = new RuleBindings();

            _rules =
                new List<RuleBinding>()
                {
                    _ruleBindings.CompilationUnit,
                    _ruleBindings.Specification,
                    _ruleBindings.Module,
                    _ruleBindings.Open,
                    _ruleBindings.Paragraph,
                    _ruleBindings.FactDecl,
                    _ruleBindings.AssertDecl,
                    _ruleBindings.FunDecl,
                    _ruleBindings.FunctionName,
                    _ruleBindings.FunctionReturn,
                    _ruleBindings.FunctionParameters,
                    _ruleBindings.DeclList,
                    _ruleBindings.CmdDecl,
                    _ruleBindings.CmdScope,
                    _ruleBindings.CmdScopeFor,
                    _ruleBindings.CmdScopeExpect,
                    _ruleBindings.TypescopeDeclList,
                    _ruleBindings.Typescope,
                    _ruleBindings.SigDecl,
                    _ruleBindings.NameList,
                    _ruleBindings.NameListName,
                    _ruleBindings.NameDeclList,
                    _ruleBindings.SigBody,
                    _ruleBindings.EnumDecl,
                    _ruleBindings.EnumBody,
                    _ruleBindings.SigQual,
                    _ruleBindings.SigExt,
                    _ruleBindings.Expr,
                    _ruleBindings.UnOpExpr1,
                    _ruleBindings.LetDecls,
                    _ruleBindings.QuantDecls,
                    _ruleBindings.BinaryExpression,
                    _ruleBindings.UnaryExpression,
                    _ruleBindings.ElseClause,
                    _ruleBindings.BinOpExpr18,
                    _ruleBindings.CallArguments,
                    _ruleBindings.UnOpExpr19,
                    _ruleBindings.PrimaryExpr,
                    _ruleBindings.Decl,
                    _ruleBindings.LetDecl,
                    _ruleBindings.Quant,
                    _ruleBindings.ArrowMultiplicity,
                    _ruleBindings.Block,
                    _ruleBindings.Name,
                    _ruleBindings.Number,
                    _ruleBindings.Ref,
                };
        }

        protected override IList<RuleBinding> Rules
        {
            get
            {
                return _rules;
            }
        }

        private RuleBindings Bindings
        {
            get
            {
                return _ruleBindings;
            }
        }

        protected override void BindRules()
        {
            Nfa.BindRule(Bindings.CompilationUnit, BuildCompilationUnitRule());
            Nfa.BindRule(Bindings.Specification, BuildSpecificationRule());
            Nfa.BindRule(Bindings.Module, BuildModuleRule());
            Nfa.BindRule(Bindings.Open, BuildOpenRule());
            Nfa.BindRule(Bindings.Paragraph, BuildParagraphRule());
            Nfa.BindRule(Bindings.FactDecl, BuildFactDeclRule());
            Nfa.BindRule(Bindings.AssertDecl, BuildAssertDeclRule());
            Nfa.BindRule(Bindings.FunDecl, BuildFunDeclRule());
            Nfa.BindRule(Bindings.FunctionName, BuildFunctionNameRule());
            Nfa.BindRule(Bindings.FunctionReturn, BuildFunctionReturnRule());
            Nfa.BindRule(Bindings.FunctionParameters, BuildFunctionParametersRule());
            Nfa.BindRule(Bindings.DeclList, BuildDeclListRule());
            Nfa.BindRule(Bindings.CmdDecl, BuildCmdDeclRule());
            Nfa.BindRule(Bindings.CmdScope, BuildCmdScopeRule());
            Nfa.BindRule(Bindings.CmdScopeFor, BuildCmdScopeForRule());
            Nfa.BindRule(Bindings.CmdScopeExpect, BuildCmdScopeExpectRule());
            Nfa.BindRule(Bindings.TypescopeDeclList, BuildTypescopeDeclListRule());
            Nfa.BindRule(Bindings.Typescope, BuildTypescopeRule());
            Nfa.BindRule(Bindings.SigDecl, BuildSigDeclRule());
            Nfa.BindRule(Bindings.NameList, BuildNameListRule());
            Nfa.BindRule(Bindings.NameListName, BuildNameListNameRule());
            Nfa.BindRule(Bindings.NameDeclList, BuildNameDeclListRule());
            Nfa.BindRule(Bindings.SigBody, BuildSigBodyRule());
            Nfa.BindRule(Bindings.EnumDecl, BuildEnumDeclRule());
            Nfa.BindRule(Bindings.EnumBody, BuildEnumBodyRule());
            Nfa.BindRule(Bindings.SigQual, BuildSigQualRule());
            Nfa.BindRule(Bindings.SigExt, BuildSigExtRule());
            Nfa.BindRule(Bindings.Expr, BuildExprRule());
            Nfa.BindRule(Bindings.UnOpExpr1, BuildUnOpExpr1Rule());
            Nfa.BindRule(Bindings.LetDecls, BuildLetDeclsRule());
            Nfa.BindRule(Bindings.QuantDecls, BuildQuantDeclsRule());
            Nfa.BindRule(Bindings.BinaryExpression, BuildBinaryExpression());
            Nfa.BindRule(Bindings.UnaryExpression, BuildUnaryExpression());
            Nfa.BindRule(Bindings.ElseClause, BuildElseClauseRule());
            Nfa.BindRule(Bindings.BinOpExpr18, BuildBinOpExpr18Rule());
            Nfa.BindRule(Bindings.CallArguments, BuildCallArguments());
            Nfa.BindRule(Bindings.UnOpExpr19, BuildUnOpExpr19Rule());
            Nfa.BindRule(Bindings.PrimaryExpr, BuildPrimaryExprRule());
            Nfa.BindRule(Bindings.Decl, BuildDeclRule());
            Nfa.BindRule(Bindings.LetDecl, BuildLetDeclRule());
            Nfa.BindRule(Bindings.Quant, BuildQuantRule());
            Nfa.BindRule(Bindings.ArrowMultiplicity, BuildArrowMultiplicityRule());
            Nfa.BindRule(Bindings.Block, BuildBlockRule());
            Nfa.BindRule(Bindings.Name, BuildNameRule());
            Nfa.BindRule(Bindings.Number, BuildNumberRule());
            Nfa.BindRule(Bindings.Ref, BuildRefRule());
        }

        private Nfa BuildCompilationUnitRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.Specification),
                Nfa.Match(AlloyLexer.EOF));
        }

        private Nfa BuildSpecificationRule()
        {
            return Nfa.Sequence(
                Nfa.Optional(Nfa.Rule(Bindings.Module)),
                Nfa.Closure(Nfa.Rule(Bindings.Open)),
                Nfa.Closure(Nfa.Rule(Bindings.Paragraph)));
        }

        private Nfa BuildModuleRule()
        {
            return Nfa.Sequence(
                Nfa.Match(AlloyLexer.KW_MODULE),
                Nfa.Rule(Bindings.Name),
                Nfa.Optional(
                    Nfa.Sequence(
                        Nfa.Match(AlloyLexer.LBRACK),
                        Nfa.Optional(Nfa.Match(AlloyLexer.KW_EXACTLY)),
                        Nfa.Rule(Bindings.Name),
                        Nfa.Closure(
                            Nfa.Sequence(
                                Nfa.Match(AlloyLexer.COMMA),
                                Nfa.Optional(Nfa.Match(AlloyLexer.KW_EXACTLY)),
                                Nfa.Rule(Bindings.Number))),
                        Nfa.Match(AlloyLexer.RBRACK))));
        }

        private Nfa BuildOpenRule()
        {
            return Nfa.Sequence(
                Nfa.Optional(Nfa.Match(AlloyLexer.KW_PRIVATE)),
                Nfa.Match(AlloyLexer.KW_OPEN),
                Nfa.Rule(Bindings.Name),
                Nfa.Optional(
                    Nfa.Sequence(
                        Nfa.Match(AlloyLexer.LBRACK),
                        Nfa.Rule(Bindings.Ref),
                        Nfa.Closure(
                            Nfa.Sequence(
                                Nfa.Match(AlloyLexer.COMMA),
                                Nfa.Rule(Bindings.Ref))),
                        Nfa.Optional(Nfa.Match(AlloyLexer.COMMA)),
                        Nfa.Match(AlloyLexer.RBRACK))),
                Nfa.Optional(
                    Nfa.Sequence(
                        Nfa.Match(AlloyLexer.KW_AS),
                        Nfa.Rule(Bindings.Name))));
        }

        private Nfa BuildParagraphRule()
        {
            return Nfa.Choice(
                Nfa.Rule(Bindings.FactDecl),
                Nfa.Rule(Bindings.AssertDecl),
                Nfa.Rule(Bindings.FunDecl),
                Nfa.Rule(Bindings.CmdDecl),
                Nfa.Rule(Bindings.EnumDecl),
                Nfa.Rule(Bindings.SigDecl));
        }

        private Nfa BuildFactDeclRule()
        {
            return Nfa.Sequence(
                Nfa.Match(AlloyLexer.KW_FACT),
                Nfa.Optional(Nfa.Rule(Bindings.Name)),
                Nfa.Rule(Bindings.Block));
        }

        private Nfa BuildAssertDeclRule()
        {
            return Nfa.Sequence(
                Nfa.Match(AlloyLexer.KW_ASSERT),
                Nfa.Optional(Nfa.Rule(Bindings.Name)),
                Nfa.Rule(Bindings.Block));
        }

        private Nfa BuildFunDeclRule()
        {
            return Nfa.Sequence(
                Nfa.Optional(Nfa.Match(AlloyLexer.KW_PRIVATE)),
                Nfa.Choice(
                    Nfa.Sequence(
                        Nfa.Match(AlloyLexer.KW_FUN),
                        Nfa.Rule(Bindings.FunctionName),
                        Nfa.Optional(Nfa.Rule(Bindings.FunctionParameters)),
                        Nfa.Rule(Bindings.FunctionReturn),
                        Nfa.Rule(Bindings.Block)),
                    Nfa.Sequence(
                        Nfa.Match(AlloyLexer.KW_PRED),
                        Nfa.Rule(Bindings.FunctionName),
                        Nfa.Optional(Nfa.Rule(Bindings.FunctionParameters)),
                        Nfa.Rule(Bindings.Block))));
        }

        private Nfa BuildFunctionNameRule()
        {
            return Nfa.Sequence(
                Nfa.Optional(
                    Nfa.Sequence(
                        Nfa.Rule(Bindings.Ref),
                        Nfa.Match(AlloyLexer.DOT))),
                Nfa.Rule(Bindings.Name));
        }

        private Nfa BuildFunctionReturnRule()
        {
            return Nfa.Sequence(
                Nfa.Match(AlloyLexer.COLON),
                Nfa.Rule(Bindings.Expr));
        }

        private Nfa BuildFunctionParametersRule()
        {
            return Nfa.Choice(
                Nfa.Sequence(
                    Nfa.Match(AlloyLexer.LPAREN),
                    Nfa.Optional(Nfa.Rule(Bindings.DeclList)),
                    Nfa.Match(AlloyLexer.RPAREN)),
                Nfa.Sequence(
                    Nfa.Match(AlloyLexer.LBRACK),
                    Nfa.Optional(Nfa.Rule(Bindings.DeclList)),
                    Nfa.Match(AlloyLexer.RBRACK)));
        }

        private Nfa BuildDeclListRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.Decl),
                Nfa.Closure(
                    Nfa.Sequence(
                        Nfa.Match(AlloyLexer.COMMA),
                        Nfa.Rule(Bindings.Decl))),
                Nfa.Optional(Nfa.Match(AlloyLexer.COMMA)));
        }

        private Nfa BuildCmdDeclRule()
        {
            return Nfa.Sequence(
                Nfa.Optional(
                    Nfa.Sequence(
                        Nfa.Rule(Bindings.Name),
                        Nfa.Match(AlloyLexer.COLON))),
                Nfa.MatchAny(AlloyLexer.KW_RUN, AlloyLexer.KW_CHECK),
                Nfa.Optional(
                    Nfa.Choice(
                        Nfa.Rule(Bindings.Name),
                        Nfa.Rule(Bindings.Block))),
                Nfa.Rule(Bindings.CmdScope));
        }

        private Nfa BuildCmdScopeRule()
        {
            return Nfa.Sequence(
                Nfa.Optional(Nfa.Rule(Bindings.CmdScopeFor)),
                Nfa.Optional(Nfa.Rule(Bindings.CmdScopeExpect)));
        }

        private Nfa BuildCmdScopeForRule()
        {
            return Nfa.Sequence(
                Nfa.Match(AlloyLexer.KW_FOR),
                Nfa.Choice(
                    Nfa.Sequence(
                        Nfa.Rule(Bindings.Number),
                        Nfa.Match(AlloyLexer.KW_BUT),
                        Nfa.Rule(Bindings.TypescopeDeclList)),
                    Nfa.Rule(Bindings.TypescopeDeclList)));
        }

        private Nfa BuildCmdScopeExpectRule()
        {
            return Nfa.Sequence(
                Nfa.Match(AlloyLexer.KW_EXPECT),
                Nfa.Optional(Nfa.Rule(Bindings.Number)));
        }

        private Nfa BuildTypescopeDeclListRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.Typescope),
                Nfa.Closure(
                    Nfa.Sequence(
                        Nfa.Match(AlloyLexer.COMMA),
                        Nfa.Rule(Bindings.Typescope))),
                Nfa.Optional(Nfa.Match(AlloyLexer.COMMA)));
        }

        private Nfa BuildTypescopeRule()
        {
            return Nfa.Sequence(
                Nfa.Optional(Nfa.Match(AlloyLexer.KW_EXACTLY)),
                Nfa.Rule(Bindings.Number),
                Nfa.Optional(
                    Nfa.Choice(
                        Nfa.Rule(Bindings.Name),
                        Nfa.MatchAny(AlloyLexer.KW_INT, AlloyLexer.KW_SEQ))));
        }

        private Nfa BuildSigDeclRule()
        {
            return Nfa.Sequence(
                Nfa.Closure(Nfa.Rule(Bindings.SigQual)),
                Nfa.Match(AlloyLexer.KW_SIG),
                Nfa.Rule(Bindings.NameDeclList),
                Nfa.Optional(Nfa.Rule(Bindings.SigExt)),
                Nfa.Rule(Bindings.SigBody),
                Nfa.Optional(Nfa.Rule(Bindings.Block)));
        }

        private Nfa BuildNameListRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.NameListName),
                Nfa.Optional(
                    Nfa.Sequence(
                        Nfa.Match(AlloyLexer.COMMA),
                        Nfa.Rule(Bindings.NameListName),
                        Nfa.Closure(
                            Nfa.Sequence(
                                Nfa.Match(AlloyLexer.COMMA),
                                Nfa.Rule(Bindings.NameListName))))));
        }

        private Nfa BuildNameListNameRule()
        {
            return Nfa.Rule(Bindings.Name);
        }

        private Nfa BuildNameDeclListRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.NameList),
                Nfa.Optional(Nfa.Match(AlloyLexer.COMMA)));
        }

        private Nfa BuildSigBodyRule()
        {
            return Nfa.Sequence(
                Nfa.Match(AlloyLexer.LBRACE),
                Nfa.Optional(
                    Nfa.Sequence(
                        Nfa.Rule(Bindings.Decl),
                        Nfa.Closure(
                            Nfa.Sequence(
                                Nfa.Match(AlloyLexer.COMMA),
                                Nfa.Rule(Bindings.Decl))),
                        Nfa.Optional(Nfa.Match(AlloyLexer.COMMA)))),
                Nfa.Match(AlloyLexer.RBRACE));
        }

        private Nfa BuildEnumDeclRule()
        {
            return Nfa.Sequence(
                Nfa.Match(AlloyLexer.KW_ENUM),
                Nfa.Rule(Bindings.Name),
                Nfa.Rule(Bindings.EnumBody));
        }

        private Nfa BuildEnumBodyRule()
        {
            return Nfa.Sequence(
                Nfa.Match(AlloyLexer.LBRACE),
                Nfa.Rule(Bindings.NameList),
                Nfa.Match(AlloyLexer.RBRACE));
        }

        private Nfa BuildSigQualRule()
        {
            return Nfa.MatchAny(AlloyLexer.KW_ABSTRACT, AlloyLexer.KW_LONE, AlloyLexer.KW_ONE, AlloyLexer.KW_SOME, AlloyLexer.KW_PRIVATE);
        }

        private Nfa BuildSigExtRule()
        {
            return Nfa.Choice(
                Nfa.Sequence(
                    Nfa.Match(AlloyLexer.KW_EXTENDS),
                    Nfa.Rule(Bindings.Ref)),
                Nfa.Sequence(
                    Nfa.Match(AlloyLexer.KW_IN),
                    Nfa.Rule(Bindings.Ref),
                    Nfa.Closure(
                        Nfa.Sequence(
                            Nfa.Match(AlloyLexer.PLUS),
                            Nfa.Rule(Bindings.Ref)))));
        }

        private Nfa BuildExprRule()
        {
            return Nfa.Rule(Bindings.UnOpExpr1);
        }

        private Nfa BuildUnOpExpr1Rule()
        {
            return Nfa.Choice(
                Nfa.Sequence(
                    Nfa.Choice(
                        Nfa.Rule(Bindings.LetDecls),
                        Nfa.Rule(Bindings.QuantDecls)),
                    Nfa.Choice(
                        Nfa.Rule(Bindings.Block),
                        Nfa.Sequence(
                            Nfa.Match(AlloyLexer.BAR),
                            Nfa.Rule(Bindings.Expr)))),
                Nfa.Rule(Bindings.BinaryExpression));
        }

        private Nfa BuildLetDeclsRule()
        {
            return Nfa.Sequence(
                Nfa.Match(AlloyLexer.KW_LET),
                Nfa.Rule(Bindings.LetDecl),
                Nfa.Closure(
                    Nfa.Sequence(
                        Nfa.Match(AlloyLexer.COMMA),
                        Nfa.Rule(Bindings.LetDecl))),
                Nfa.Optional(Nfa.Match(AlloyLexer.COMMA)));
        }

        private Nfa BuildQuantDeclsRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.Quant),
                Nfa.Rule(Bindings.Decl),
                Nfa.Closure(
                    Nfa.Sequence(
                        Nfa.Match(AlloyLexer.COMMA),
                        Nfa.Rule(Bindings.Decl))),
                Nfa.Optional(Nfa.Match(AlloyLexer.COMMA)));
        }

        private Nfa BuildBinaryExpression()
        {
            int[] binaryOperators =
                {
                    AlloyLexer.OR, AlloyLexer.KW_OR,
                    AlloyLexer.IFF, AlloyLexer.KW_IFF,
                    AlloyLexer.AND, AlloyLexer.KW_AND,
                    AlloyLexer.LSHIFT, AlloyLexer.RSHIFT, AlloyLexer.URSHIFT,
                    AlloyLexer.PLUS, AlloyLexer.MINUS,
                    AlloyLexer.OVERRIDE,
                    AlloyLexer.BITAND,
                    AlloyLexer.DOMAIN_RES,
                    AlloyLexer.RANGE_RES,
                };
            int[] binaryComparisonOperators =
                {
                    AlloyLexer.LT, AlloyLexer.GT,
                    AlloyLexer.LE, AlloyLexer.GE,
                    AlloyLexer.EQ,
                    AlloyLexer.KW_IN
                };

            return Nfa.Sequence(
                Nfa.Rule(Bindings.UnaryExpression),
                Nfa.Closure(
                    Nfa.Choice(
                        Nfa.Sequence(
                            Nfa.MatchAny(binaryOperators),
                            Nfa.Rule(Bindings.UnaryExpression)),
                        Nfa.Sequence(
                            Nfa.Optional(Nfa.Rule(Bindings.ArrowMultiplicity)),
                            Nfa.Match(AlloyLexer.ARROW),
                            Nfa.Optional(Nfa.Rule(Bindings.ArrowMultiplicity)),
                            Nfa.Rule(Bindings.UnaryExpression)),
                        Nfa.Sequence(
                            Nfa.Optional(Nfa.MatchAny(AlloyLexer.NOT, AlloyLexer.KW_NOT)),
                            Nfa.MatchAny(binaryComparisonOperators),
                            Nfa.Rule(Bindings.UnaryExpression)),
                        Nfa.Sequence(
                            Nfa.MatchAny(AlloyLexer.IMPLIES, AlloyLexer.KW_IMPLIES),
                            Nfa.Rule(Bindings.BinaryExpression),
                            Nfa.Optional(Nfa.Rule(Bindings.ElseClause))))));
        }

        private Nfa BuildUnaryExpression()
        {
            int[] unaryPrefixOperators =
                {
                    AlloyLexer.NOT, AlloyLexer.KW_NOT,
                    AlloyLexer.KW_NO, AlloyLexer.KW_SOME, AlloyLexer.KW_LONE, AlloyLexer.KW_ONE, AlloyLexer.KW_SET, AlloyLexer.KW_SEQ,
                    AlloyLexer.COUNT
                };
            int[] unarySuffixOperators = { };

            return Nfa.Sequence(
                Nfa.Closure(Nfa.MatchAny(unaryPrefixOperators)),
                Nfa.Rule(Bindings.BinOpExpr18));
        }

        private Nfa BuildElseClauseRule()
        {
            return Nfa.Sequence(
                Nfa.MatchAny(AlloyLexer.KW_ELSE/*, AlloyLexer.COMMA*/),
                Nfa.Rule(Bindings.UnaryExpression));
        }

        private Nfa BuildBinOpExpr18Rule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.UnOpExpr19),
                Nfa.Closure(
                    Nfa.Choice(
                        Nfa.Sequence(
                            Nfa.Match(AlloyLexer.DOT),
                            Nfa.Rule(Bindings.UnOpExpr19)),
                        Nfa.Sequence(
                            Nfa.Match(AlloyLexer.LBRACK),
                            Nfa.Rule(Bindings.CallArguments),
                            Nfa.Match(AlloyLexer.RBRACK)))));
        }

        private Nfa BuildCallArguments()
        {
#if false
            return Nfa.Closure(
                Nfa.Choice(
                    Nfa.Sequence(
                        Nfa.Match(AlloyLexer.LBRACK),
                        Nfa.Rule(Rules.CallArguments),
                        Nfa.Match(AlloyLexer.RBRACK)),
                    Nfa.MatchComplement(Interval.FromBounds(AlloyLexer.LBRACK, AlloyLexer.LBRACK), Interval.FromBounds(AlloyLexer.RBRACK, AlloyLexer.RBRACK))));
#else
            return Nfa.Optional(
                Nfa.Sequence(
                    Nfa.Rule(Bindings.Expr),
                    Nfa.Closure(
                        Nfa.Sequence(
                            Nfa.Match(AlloyLexer.COMMA),
                            Nfa.Rule(Bindings.Expr)))));
#endif
        }

        private Nfa BuildUnOpExpr19Rule()
        {
            return Nfa.Choice(
                Nfa.Sequence(
                    Nfa.MatchAny(AlloyLexer.TILDE, AlloyLexer.STAR, AlloyLexer.CARET),
                    Nfa.Rule(Bindings.UnOpExpr19)),
                Nfa.Rule(Bindings.PrimaryExpr));
        }

        private Nfa BuildPrimaryExprRule()
        {
            return Nfa.Choice(
                Nfa.MatchAny(AlloyLexer.KW_NONE, AlloyLexer.KW_IDEN, AlloyLexer.KW_UNIV, AlloyLexer.KW_INT2, AlloyLexer.KW_SEQINT),
                Nfa.Sequence(
                    Nfa.Match(AlloyLexer.LPAREN),
                    Nfa.Rule(Bindings.Expr),
                    Nfa.Match(AlloyLexer.RPAREN)),
                Nfa.Sequence(
                    Nfa.Optional(Nfa.Match(AlloyLexer.AT)),
                    Nfa.Rule(Bindings.Name)),
                Nfa.Rule(Bindings.Number),
                Nfa.Rule(Bindings.Block));
        }

        private Nfa BuildDeclRule()
        {
            return Nfa.Sequence(
                Nfa.Optional(Nfa.Match(AlloyLexer.KW_PRIVATE)),
                Nfa.Optional(Nfa.Match(AlloyLexer.KW_DISJ)),
                Nfa.Rule(Bindings.NameList),
                Nfa.Match(AlloyLexer.COLON),
                Nfa.Optional(Nfa.Match(AlloyLexer.KW_DISJ)),
                Nfa.Rule(Bindings.Expr));
        }

        private Nfa BuildLetDeclRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.Name),
                Nfa.Match(AlloyLexer.EQ),
                Nfa.Rule(Bindings.Expr));
        }

        private Nfa BuildQuantRule()
        {
            return Nfa.MatchAny(AlloyLexer.KW_ALL, AlloyLexer.KW_NO, AlloyLexer.KW_SOME, AlloyLexer.KW_LONE, AlloyLexer.KW_ONE, AlloyLexer.KW_SUM);
        }

        private Nfa BuildArrowMultiplicityRule()
        {
            return Nfa.MatchAny(AlloyLexer.KW_SOME, AlloyLexer.KW_ONE, AlloyLexer.KW_LONE, AlloyLexer.KW_SET);
        }

        private Nfa BuildBlockRule()
        {
            return Nfa.Sequence(
                Nfa.Match(AlloyLexer.LBRACE),
                Nfa.Closure(Nfa.Rule(Bindings.Expr)),
                Nfa.Match(AlloyLexer.RBRACE));
        }

        private Nfa BuildNameRule()
        {
            return Nfa.Sequence(
                Nfa.MatchAny(AlloyLexer.KW_THIS, AlloyLexer.IDENTIFIER),
                Nfa.Closure(
                    Nfa.Sequence(
                        Nfa.Match(AlloyLexer.SLASH),
                        Nfa.Match(AlloyLexer.IDENTIFIER))));
        }

        private Nfa BuildNumberRule()
        {
            return Nfa.Match(AlloyLexer.INTEGER);
        }

        private Nfa BuildRefRule()
        {
            return Nfa.Choice(
                Nfa.Rule(Bindings.Name),
                Nfa.MatchAny(AlloyLexer.KW_UNIV, AlloyLexer.KW_INT2, AlloyLexer.KW_SEQINT));
        }

        public static class RuleNames
        {
            public static readonly string CompilationUnit = "CompilationUnit";
            public static readonly string Specification = "Specification";
            public static readonly string Module = "Module";
            public static readonly string Open = "Open";
            public static readonly string Paragraph = "Paragraph";
            public static readonly string FactDecl = "FactDecl";
            public static readonly string AssertDecl = "AssertDecl";
            public static readonly string FunDecl = "FunDecl";
            public static readonly string FunctionName = "FunctionName";
            public static readonly string FunctionReturn = "FunctionReturn";
            public static readonly string FunctionParameters = "FunctionParameters";
            public static readonly string DeclList = "DeclList";
            public static readonly string CmdDecl = "CmdDecl";
            public static readonly string CmdScope = "CmdScope";
            public static readonly string CmdScopeFor = "CmdScopeFor";
            public static readonly string CmdScopeExpect = "CmdScopeExpect";
            public static readonly string TypescopeDeclList = "TypescopeDeclList";
            public static readonly string Typescope = "Typescope";
            public static readonly string SigDecl = "SigDecl";
            public static readonly string NameList = "NameList";
            public static readonly string NameListName = "NameListName";
            public static readonly string NameDeclList = "NameDeclList";
            public static readonly string SigBody = "SigBody";
            public static readonly string EnumDecl = "EnumDecl";
            public static readonly string EnumBody = "EnumBody";
            public static readonly string SigQual = "SigQual";
            public static readonly string SigExt = "SigExt";
            public static readonly string Expr = "Expr";
            public static readonly string UnOpExpr1 = "UnOpExpr1";
            public static readonly string LetDecls = "LetDecls";
            public static readonly string QuantDecls = "QuantDecls";
            public static readonly string BinaryExpression = "BinaryExpression";
            public static readonly string UnaryExpression = "UnaryExpression";
            public static readonly string ElseClause = "ElseClause";
            public static readonly string BinOpExpr18 = "BinOpExpr18";
            public static readonly string CallArguments = "CallArguments";
            public static readonly string UnOpExpr19 = "UnOpExpr19";
            public static readonly string PrimaryExpr = "PrimaryExpr";
            public static readonly string Decl = "Decl";
            public static readonly string LetDecl = "LetDecl";
            public static readonly string Quant = "Quant";
            public static readonly string ArrowMultiplicity = "ArrowMultiplicity";
            public static readonly string Block = "Block";
            public static readonly string Name = "Name";
            public static readonly string Number = "Number";
            public static readonly string Ref = "Ref";
        }

        private class RuleBindings
        {
            public readonly RuleBinding CompilationUnit = new RuleBinding(RuleNames.CompilationUnit);
            public readonly RuleBinding Specification = new RuleBinding(RuleNames.Specification);
            public readonly RuleBinding Module = new RuleBinding(RuleNames.Module);
            public readonly RuleBinding Open = new RuleBinding(RuleNames.Open);
            public readonly RuleBinding Paragraph = new RuleBinding(RuleNames.Paragraph);
            public readonly RuleBinding FactDecl = new RuleBinding(RuleNames.FactDecl);
            public readonly RuleBinding AssertDecl = new RuleBinding(RuleNames.AssertDecl);
            public readonly RuleBinding FunDecl = new RuleBinding(RuleNames.FunDecl);
            public readonly RuleBinding FunctionName = new RuleBinding(RuleNames.FunctionName);
            public readonly RuleBinding FunctionReturn = new RuleBinding(RuleNames.FunctionReturn);
            public readonly RuleBinding FunctionParameters = new RuleBinding(RuleNames.FunctionParameters);
            public readonly RuleBinding DeclList = new RuleBinding(RuleNames.DeclList);
            public readonly RuleBinding CmdDecl = new RuleBinding(RuleNames.CmdDecl);
            public readonly RuleBinding CmdScope = new RuleBinding(RuleNames.CmdScope);
            public readonly RuleBinding CmdScopeFor = new RuleBinding(RuleNames.CmdScopeFor);
            public readonly RuleBinding CmdScopeExpect = new RuleBinding(RuleNames.CmdScopeExpect);
            public readonly RuleBinding TypescopeDeclList = new RuleBinding(RuleNames.TypescopeDeclList);
            public readonly RuleBinding Typescope = new RuleBinding(RuleNames.Typescope);
            public readonly RuleBinding SigDecl = new RuleBinding(RuleNames.SigDecl);
            public readonly RuleBinding NameList = new RuleBinding(RuleNames.NameList);
            public readonly RuleBinding NameListName = new RuleBinding(RuleNames.NameListName);
            public readonly RuleBinding NameDeclList = new RuleBinding(RuleNames.NameDeclList);
            public readonly RuleBinding SigBody = new RuleBinding(RuleNames.SigBody);
            public readonly RuleBinding EnumDecl = new RuleBinding(RuleNames.EnumDecl);
            public readonly RuleBinding EnumBody = new RuleBinding(RuleNames.EnumBody);
            public readonly RuleBinding SigQual = new RuleBinding(RuleNames.SigQual);
            public readonly RuleBinding SigExt = new RuleBinding(RuleNames.SigExt);
            public readonly RuleBinding Expr = new RuleBinding(RuleNames.Expr);
            public readonly RuleBinding UnOpExpr1 = new RuleBinding(RuleNames.UnOpExpr1);
            public readonly RuleBinding LetDecls = new RuleBinding(RuleNames.LetDecls);
            public readonly RuleBinding QuantDecls = new RuleBinding(RuleNames.QuantDecls);
            public readonly RuleBinding BinaryExpression = new RuleBinding(RuleNames.BinaryExpression);
            public readonly RuleBinding UnaryExpression = new RuleBinding(RuleNames.UnaryExpression);
            public readonly RuleBinding ElseClause = new RuleBinding(RuleNames.ElseClause);
            public readonly RuleBinding BinOpExpr18 = new RuleBinding(RuleNames.BinOpExpr18);
            public readonly RuleBinding CallArguments = new RuleBinding(RuleNames.CallArguments);
            public readonly RuleBinding UnOpExpr19 = new RuleBinding(RuleNames.UnOpExpr19);
            public readonly RuleBinding PrimaryExpr = new RuleBinding(RuleNames.PrimaryExpr);
            public readonly RuleBinding Decl = new RuleBinding(RuleNames.Decl);
            public readonly RuleBinding LetDecl = new RuleBinding(RuleNames.LetDecl);
            public readonly RuleBinding Quant = new RuleBinding(RuleNames.Quant);
            public readonly RuleBinding ArrowMultiplicity = new RuleBinding(RuleNames.ArrowMultiplicity);
            public readonly RuleBinding Block = new RuleBinding(RuleNames.Block);
            public readonly RuleBinding Name = new RuleBinding(RuleNames.Name);
            public readonly RuleBinding Number = new RuleBinding(RuleNames.Number);
            public readonly RuleBinding Ref = new RuleBinding(RuleNames.Ref);
        }
    }
}
