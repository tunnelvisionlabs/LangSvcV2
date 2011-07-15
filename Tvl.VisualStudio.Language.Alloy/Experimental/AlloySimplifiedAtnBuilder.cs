namespace Tvl.VisualStudio.Language.Alloy.Experimental
{
    using System.Collections.Generic;
    using Tvl.VisualStudio.Language.Parsing.Collections;
    using Tvl.VisualStudio.Language.Parsing.Experimental.Atn;

    internal class AlloySimplifiedAtnBuilder
    {
        private static Network _network;
        private readonly RuleBindings _ruleBindings = new RuleBindings();

        private RuleBindings Rules
        {
            get
            {
                return _ruleBindings;
            }
        }

        public static Network BuildNetwork()
        {
            if (_network != null)
                return _network;

            AlloySimplifiedAtnBuilder builder = new AlloySimplifiedAtnBuilder();
            var rules = builder.Rules;

            Nfa.BindRule(rules.CompilationUnit, builder.BuildCompilationUnitRule());
            Nfa.BindRule(rules.Specification, builder.BuildSpecificationRule());
            Nfa.BindRule(rules.Module, builder.BuildModuleRule());
            Nfa.BindRule(rules.Open, builder.BuildOpenRule());
            Nfa.BindRule(rules.Paragraph, builder.BuildParagraphRule());
            Nfa.BindRule(rules.FactDecl, builder.BuildFactDeclRule());
            Nfa.BindRule(rules.AssertDecl, builder.BuildAssertDeclRule());
            Nfa.BindRule(rules.FunDecl, builder.BuildFunDeclRule());
            Nfa.BindRule(rules.FunctionName, builder.BuildFunctionNameRule());
            Nfa.BindRule(rules.FunctionReturn, builder.BuildFunctionReturnRule());
            Nfa.BindRule(rules.FunctionParameters, builder.BuildFunctionParametersRule());
            Nfa.BindRule(rules.DeclList, builder.BuildDeclListRule());
            Nfa.BindRule(rules.CmdDecl, builder.BuildCmdDeclRule());
            Nfa.BindRule(rules.CmdScope, builder.BuildCmdScopeRule());
            Nfa.BindRule(rules.CmdScopeFor, builder.BuildCmdScopeForRule());
            Nfa.BindRule(rules.CmdScopeExpect, builder.BuildCmdScopeExpectRule());
            Nfa.BindRule(rules.TypescopeDeclList, builder.BuildTypescopeDeclListRule());
            Nfa.BindRule(rules.Typescope, builder.BuildTypescopeRule());
            Nfa.BindRule(rules.SigDecl, builder.BuildSigDeclRule());
            Nfa.BindRule(rules.NameList, builder.BuildNameListRule());
            Nfa.BindRule(rules.NameDeclList, builder.BuildNameDeclListRule());
            Nfa.BindRule(rules.SigBody, builder.BuildSigBodyRule());
            Nfa.BindRule(rules.EnumDecl, builder.BuildEnumDeclRule());
            Nfa.BindRule(rules.EnumBody, builder.BuildEnumBodyRule());
            Nfa.BindRule(rules.SigQual, builder.BuildSigQualRule());
            Nfa.BindRule(rules.SigExt, builder.BuildSigExtRule());
            Nfa.BindRule(rules.Expr, builder.BuildExprRule());
            Nfa.BindRule(rules.UnOpExpr1, builder.BuildUnOpExpr1Rule());
            Nfa.BindRule(rules.LetDecls, builder.BuildLetDeclsRule());
            Nfa.BindRule(rules.QuantDecls, builder.BuildQuantDeclsRule());
            Nfa.BindRule(rules.BinaryExpression, builder.BuildBinaryExpression());
            Nfa.BindRule(rules.UnaryExpression, builder.BuildUnaryExpression());
            Nfa.BindRule(rules.ElseClause, builder.BuildElseClauseRule());
            Nfa.BindRule(rules.BinOpExpr18, builder.BuildBinOpExpr18Rule());
            Nfa.BindRule(rules.CallArguments, builder.BuildCallArguments());
            Nfa.BindRule(rules.UnOpExpr19, builder.BuildUnOpExpr19Rule());
            Nfa.BindRule(rules.PrimaryExpr, builder.BuildPrimaryExprRule());
            Nfa.BindRule(rules.Decl, builder.BuildDeclRule());
            Nfa.BindRule(rules.LetDecl, builder.BuildLetDeclRule());
            Nfa.BindRule(rules.Quant, builder.BuildQuantRule());
            Nfa.BindRule(rules.ArrowMultiplicity, builder.BuildArrowMultiplicityRule());
            Nfa.BindRule(rules.Block, builder.BuildBlockRule());
            Nfa.BindRule(rules.Name, builder.BuildNameRule());
            Nfa.BindRule(rules.Number, builder.BuildNumberRule());
            Nfa.BindRule(rules.Ref, builder.BuildRefRule());

            List<RuleBinding> ruleBindings =
                new List<RuleBinding>()
                {
                    rules.CompilationUnit,
                    rules.Specification,
                    rules.Module,
                    rules.Open,
                    rules.Paragraph,
                    rules.FactDecl,
                    rules.AssertDecl,
                    rules.FunDecl,
                    rules.FunctionName,
                    rules.FunctionReturn,
                    rules.FunctionParameters,
                    rules.DeclList,
                    rules.CmdDecl,
                    rules.CmdScope,
                    rules.CmdScopeFor,
                    rules.CmdScopeExpect,
                    rules.TypescopeDeclList,
                    rules.Typescope,
                    rules.SigDecl,
                    rules.NameList,
                    rules.NameDeclList,
                    rules.SigBody,
                    rules.EnumDecl,
                    rules.EnumBody,
                    rules.SigQual,
                    rules.SigExt,
                    rules.Expr,
                    rules.UnOpExpr1,
                    rules.LetDecls,
                    rules.QuantDecls,
                    rules.BinaryExpression,
                    rules.UnaryExpression,
                    rules.ElseClause,
                    rules.BinOpExpr18,
                    rules.CallArguments,
                    rules.UnOpExpr19,
                    rules.PrimaryExpr,
                    rules.Decl,
                    rules.LetDecl,
                    rules.Quant,
                    rules.ArrowMultiplicity,
                    rules.Block,
                    rules.Name,
                    rules.Number,
                    rules.Ref,
                };

            _network = new Network(ruleBindings);
            return _network;
        }

        private Nfa BuildCompilationUnitRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Rules.Specification),
                Nfa.Match(AlloyLexer.EOF));
        }

        private Nfa BuildSpecificationRule()
        {
            return Nfa.Sequence(
                Nfa.Optional(Nfa.Rule(Rules.Module)),
                Nfa.Closure(Nfa.Rule(Rules.Open)),
                Nfa.Closure(Nfa.Rule(Rules.Paragraph)));
        }

        private Nfa BuildModuleRule()
        {
            return Nfa.Sequence(
                Nfa.Match(AlloyLexer.KW_MODULE),
                Nfa.Rule(Rules.Name),
                Nfa.Optional(
                    Nfa.Sequence(
                        Nfa.Match(AlloyLexer.LBRACK),
                        Nfa.Optional(Nfa.Match(AlloyLexer.KW_EXACTLY)),
                        Nfa.Rule(Rules.Name),
                        Nfa.Closure(
                            Nfa.Sequence(
                                Nfa.Match(AlloyLexer.COMMA),
                                Nfa.Optional(Nfa.Match(AlloyLexer.KW_EXACTLY)),
                                Nfa.Rule(Rules.Number))),
                        Nfa.Match(AlloyLexer.RBRACK))));
        }

        private Nfa BuildOpenRule()
        {
            return Nfa.Sequence(
                Nfa.Optional(Nfa.Match(AlloyLexer.KW_PRIVATE)),
                Nfa.Match(AlloyLexer.KW_OPEN),
                Nfa.Rule(Rules.Name),
                Nfa.Optional(
                    Nfa.Sequence(
                        Nfa.Match(AlloyLexer.LBRACK),
                        Nfa.Rule(Rules.Ref),
                        Nfa.Closure(
                            Nfa.Sequence(
                                Nfa.Match(AlloyLexer.COMMA),
                                Nfa.Rule(Rules.Ref))),
                        Nfa.Optional(Nfa.Match(AlloyLexer.COMMA)),
                        Nfa.Match(AlloyLexer.RBRACK))),
                Nfa.Optional(
                    Nfa.Sequence(
                        Nfa.Match(AlloyLexer.KW_AS),
                        Nfa.Rule(Rules.Name))));
        }

        private Nfa BuildParagraphRule()
        {
            return Nfa.Choice(
                Nfa.Rule(Rules.FactDecl),
                Nfa.Rule(Rules.AssertDecl),
                Nfa.Rule(Rules.FunDecl),
                Nfa.Rule(Rules.CmdDecl),
                Nfa.Rule(Rules.EnumDecl),
                Nfa.Rule(Rules.SigDecl));
        }

        private Nfa BuildFactDeclRule()
        {
            return Nfa.Sequence(
                Nfa.Match(AlloyLexer.KW_FACT),
                Nfa.Optional(Nfa.Rule(Rules.Name)),
                Nfa.Rule(Rules.Block));
        }

        private Nfa BuildAssertDeclRule()
        {
            return Nfa.Sequence(
                Nfa.Match(AlloyLexer.KW_ASSERT),
                Nfa.Optional(Nfa.Rule(Rules.Name)),
                Nfa.Rule(Rules.Block));
        }

        private Nfa BuildFunDeclRule()
        {
            return Nfa.Sequence(
                Nfa.Optional(Nfa.Match(AlloyLexer.KW_PRIVATE)),
                Nfa.Choice(
                    Nfa.Sequence(
                        Nfa.Match(AlloyLexer.KW_FUN),
                        Nfa.Rule(Rules.FunctionName),
                        Nfa.Optional(Nfa.Rule(Rules.FunctionParameters)),
                        Nfa.Rule(Rules.FunctionReturn),
                        Nfa.Rule(Rules.Block)),
                    Nfa.Sequence(
                        Nfa.Match(AlloyLexer.KW_PRED),
                        Nfa.Rule(Rules.FunctionName),
                        Nfa.Optional(Nfa.Rule(Rules.FunctionParameters)),
                        Nfa.Rule(Rules.Block))));
        }

        private Nfa BuildFunctionNameRule()
        {
            return Nfa.Sequence(
                Nfa.Optional(
                    Nfa.Sequence(
                        Nfa.Rule(Rules.Ref),
                        Nfa.Match(AlloyLexer.DOT))),
                Nfa.Rule(Rules.Name));
        }

        private Nfa BuildFunctionReturnRule()
        {
            return Nfa.Sequence(
                Nfa.Match(AlloyLexer.COLON),
                Nfa.Rule(Rules.Expr));
        }

        private Nfa BuildFunctionParametersRule()
        {
            return Nfa.Choice(
                Nfa.Sequence(
                    Nfa.Match(AlloyLexer.LPAREN),
                    Nfa.Optional(Nfa.Rule(Rules.DeclList)),
                    Nfa.Match(AlloyLexer.RPAREN)),
                Nfa.Sequence(
                    Nfa.Match(AlloyLexer.LBRACK),
                    Nfa.Optional(Nfa.Rule(Rules.DeclList)),
                    Nfa.Match(AlloyLexer.RBRACK)));
        }

        private Nfa BuildDeclListRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Rules.Decl),
                Nfa.Closure(
                    Nfa.Sequence(
                        Nfa.Match(AlloyLexer.COMMA),
                        Nfa.Rule(Rules.Decl))),
                Nfa.Optional(Nfa.Match(AlloyLexer.COMMA)));
        }

        private Nfa BuildCmdDeclRule()
        {
            return Nfa.Sequence(
                Nfa.Optional(
                    Nfa.Sequence(
                        Nfa.Rule(Rules.Name),
                        Nfa.Match(AlloyLexer.COLON))),
                Nfa.MatchAny(AlloyLexer.KW_RUN, AlloyLexer.KW_CHECK),
                Nfa.Optional(
                    Nfa.Choice(
                        Nfa.Rule(Rules.Name),
                        Nfa.Rule(Rules.Block))),
                Nfa.Rule(Rules.CmdScope));
        }

        private Nfa BuildCmdScopeRule()
        {
            return Nfa.Sequence(
                Nfa.Optional(Nfa.Rule(Rules.CmdScopeFor)),
                Nfa.Optional(Nfa.Rule(Rules.CmdScopeExpect)));
        }

        private Nfa BuildCmdScopeForRule()
        {
            return Nfa.Sequence(
                Nfa.Match(AlloyLexer.KW_FOR),
                Nfa.Choice(
                    Nfa.Sequence(
                        Nfa.Rule(Rules.Number),
                        Nfa.Match(AlloyLexer.KW_BUT),
                        Nfa.Rule(Rules.TypescopeDeclList)),
                    Nfa.Rule(Rules.TypescopeDeclList)));
        }

        private Nfa BuildCmdScopeExpectRule()
        {
            return Nfa.Sequence(
                Nfa.Match(AlloyLexer.KW_EXPECT),
                Nfa.Optional(Nfa.Rule(Rules.Number)));
        }

        private Nfa BuildTypescopeDeclListRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Rules.Typescope),
                Nfa.Closure(
                    Nfa.Sequence(
                        Nfa.Match(AlloyLexer.COMMA),
                        Nfa.Rule(Rules.Typescope))),
                Nfa.Optional(Nfa.Match(AlloyLexer.COMMA)));
        }

        private Nfa BuildTypescopeRule()
        {
            return Nfa.Sequence(
                Nfa.Optional(Nfa.Match(AlloyLexer.KW_EXACTLY)),
                Nfa.Rule(Rules.Number),
                Nfa.Optional(
                    Nfa.Choice(
                        Nfa.Rule(Rules.Name),
                        Nfa.MatchAny(AlloyLexer.KW_INT, AlloyLexer.KW_SEQ))));
        }

        private Nfa BuildSigDeclRule()
        {
            return Nfa.Sequence(
                Nfa.Closure(Nfa.Rule(Rules.SigQual)),
                Nfa.Match(AlloyLexer.KW_SIG),
                Nfa.Rule(Rules.NameDeclList),
                Nfa.Optional(Nfa.Rule(Rules.SigExt)),
                Nfa.Rule(Rules.SigBody),
                Nfa.Optional(Nfa.Rule(Rules.Block)));
        }

        private Nfa BuildNameListRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Rules.Name),
                Nfa.Optional(
                    Nfa.Sequence(
                        Nfa.Match(AlloyLexer.COMMA),
                        Nfa.Rule(Rules.Name),
                        Nfa.Closure(
                            Nfa.Sequence(
                                Nfa.Match(AlloyLexer.COMMA),
                                Nfa.Rule(Rules.Name))))));
        }

        private Nfa BuildNameDeclListRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Rules.NameList),
                Nfa.Optional(Nfa.Match(AlloyLexer.COMMA)));
        }

        private Nfa BuildSigBodyRule()
        {
            return Nfa.Sequence(
                Nfa.Match(AlloyLexer.LBRACE),
                Nfa.Optional(
                    Nfa.Sequence(
                        Nfa.Rule(Rules.Decl),
                        Nfa.Closure(
                            Nfa.Sequence(
                                Nfa.Match(AlloyLexer.COMMA),
                                Nfa.Rule(Rules.Decl))),
                        Nfa.Optional(Nfa.Match(AlloyLexer.COMMA)))),
                Nfa.Match(AlloyLexer.RBRACE));
        }

        private Nfa BuildEnumDeclRule()
        {
            return Nfa.Sequence(
                Nfa.Match(AlloyLexer.KW_ENUM),
                Nfa.Rule(Rules.Name),
                Nfa.Rule(Rules.EnumBody));
        }

        private Nfa BuildEnumBodyRule()
        {
            return Nfa.Sequence(
                Nfa.Match(AlloyLexer.LBRACE),
                Nfa.Rule(Rules.NameList),
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
                    Nfa.Rule(Rules.Ref)),
                Nfa.Sequence(
                    Nfa.Match(AlloyLexer.KW_IN),
                    Nfa.Rule(Rules.Ref),
                    Nfa.Closure(
                        Nfa.Sequence(
                            Nfa.Match(AlloyLexer.PLUS),
                            Nfa.Rule(Rules.Ref)))));
        }

        private Nfa BuildExprRule()
        {
            return Nfa.Rule(Rules.UnOpExpr1);
        }

        private Nfa BuildUnOpExpr1Rule()
        {
            return Nfa.Choice(
                Nfa.Sequence(
                    Nfa.Choice(
                        Nfa.Rule(Rules.LetDecls),
                        Nfa.Rule(Rules.QuantDecls)),
                    Nfa.Choice(
                        Nfa.Rule(Rules.Block),
                        Nfa.Sequence(
                            Nfa.Match(AlloyLexer.BAR),
                            Nfa.Rule(Rules.Expr)))),
                Nfa.Rule(Rules.BinaryExpression));
        }

        private Nfa BuildLetDeclsRule()
        {
            return Nfa.Sequence(
                Nfa.Match(AlloyLexer.KW_LET),
                Nfa.Rule(Rules.LetDecl),
                Nfa.Closure(
                    Nfa.Sequence(
                        Nfa.Match(AlloyLexer.COMMA),
                        Nfa.Rule(Rules.LetDecl))),
                Nfa.Optional(Nfa.Match(AlloyLexer.COMMA)));
        }

        private Nfa BuildQuantDeclsRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Rules.Quant),
                Nfa.Rule(Rules.Decl),
                Nfa.Closure(
                    Nfa.Sequence(
                        Nfa.Match(AlloyLexer.COMMA),
                        Nfa.Rule(Rules.Decl))),
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
                Nfa.Rule(Rules.UnaryExpression),
                Nfa.Closure(
                    Nfa.Choice(
                        Nfa.Sequence(
                            Nfa.MatchAny(binaryOperators),
                            Nfa.Rule(Rules.UnaryExpression)),
                        Nfa.Sequence(
                            Nfa.Optional(Nfa.Rule(Rules.ArrowMultiplicity)),
                            Nfa.Match(AlloyLexer.ARROW),
                            Nfa.Optional(Nfa.Rule(Rules.ArrowMultiplicity)),
                            Nfa.Rule(Rules.UnaryExpression)),
                        Nfa.Sequence(
                            Nfa.Optional(Nfa.MatchAny(AlloyLexer.NOT, AlloyLexer.KW_NOT)),
                            Nfa.MatchAny(binaryComparisonOperators),
                            Nfa.Rule(Rules.UnaryExpression)),
                        Nfa.Sequence(
                            Nfa.MatchAny(AlloyLexer.IMPLIES, AlloyLexer.KW_IMPLIES),
                            Nfa.Rule(Rules.BinaryExpression),
                            Nfa.Optional(Nfa.Rule(Rules.ElseClause))))));
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
                Nfa.Rule(Rules.BinOpExpr18));
        }

        private Nfa BuildElseClauseRule()
        {
            return Nfa.Sequence(
                Nfa.MatchAny(AlloyLexer.KW_ELSE, AlloyLexer.COMMA),
                Nfa.Rule(Rules.UnaryExpression));
        }

        private Nfa BuildBinOpExpr18Rule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Rules.UnOpExpr19),
                Nfa.Closure(
                    Nfa.Choice(
                        Nfa.Sequence(
                            Nfa.Match(AlloyLexer.DOT),
                            Nfa.Rule(Rules.UnOpExpr19)),
                        Nfa.Sequence(
                            Nfa.Match(AlloyLexer.LBRACK),
                            Nfa.Rule(Rules.CallArguments),
                            //Nfa.Optional(
                            //    Nfa.Sequence(
                            //        Nfa.Rule(Rules.Expr),
                            //        Nfa.Closure(
                            //            Nfa.Sequence(
                            //                Nfa.Match(AlloyLexer.COMMA),
                            //                Nfa.Rule(Rules.Expr))))),
                            Nfa.Match(AlloyLexer.RBRACK)))));
        }

        private Nfa BuildCallArguments()
        {
            return Nfa.Closure(
                Nfa.Choice(
                    Nfa.Sequence(
                        Nfa.Match(AlloyLexer.LBRACK),
                        Nfa.Rule(Rules.CallArguments),
                        Nfa.Match(AlloyLexer.RBRACK)),
                    Nfa.MatchComplement(Interval.FromBounds(AlloyLexer.LBRACK, AlloyLexer.LBRACK), Interval.FromBounds(AlloyLexer.RBRACK, AlloyLexer.RBRACK))));
        }

        private Nfa BuildUnOpExpr19Rule()
        {
            return Nfa.Choice(
                Nfa.Sequence(
                    Nfa.MatchAny(AlloyLexer.TILDE, AlloyLexer.STAR, AlloyLexer.CARET),
                    Nfa.Rule(Rules.UnOpExpr19)),
                Nfa.Rule(Rules.PrimaryExpr));
        }

        private Nfa BuildPrimaryExprRule()
        {
            return Nfa.Choice(
                Nfa.MatchAny(AlloyLexer.KW_NONE, AlloyLexer.KW_IDEN, AlloyLexer.KW_UNIV, AlloyLexer.KW_INT2, AlloyLexer.KW_SEQINT),
                Nfa.Sequence(
                    Nfa.Match(AlloyLexer.LPAREN),
                    Nfa.Rule(Rules.Expr),
                    Nfa.Match(AlloyLexer.RPAREN)),
                Nfa.Sequence(
                    Nfa.Optional(Nfa.Match(AlloyLexer.AT)),
                    Nfa.Rule(Rules.Name)),
                Nfa.Rule(Rules.Number),
                Nfa.Rule(Rules.Block));
        }

        private Nfa BuildDeclRule()
        {
            return Nfa.Sequence(
                Nfa.Optional(Nfa.Match(AlloyLexer.KW_PRIVATE)),
                Nfa.Optional(Nfa.Match(AlloyLexer.KW_DISJ)),
                Nfa.Rule(Rules.NameList),
                Nfa.Match(AlloyLexer.COLON),
                Nfa.Optional(Nfa.Match(AlloyLexer.KW_DISJ)),
                Nfa.Rule(Rules.Expr));
        }

        private Nfa BuildLetDeclRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Rules.Name),
                Nfa.Match(AlloyLexer.EQ),
                Nfa.Rule(Rules.Expr));
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
                Nfa.Closure(Nfa.Rule(Rules.Expr)),
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
                Nfa.Rule(Rules.Name),
                Nfa.MatchAny(AlloyLexer.KW_UNIV, AlloyLexer.KW_INT2, AlloyLexer.KW_SEQINT));
        }

        private class RuleBindings
        {
            public readonly RuleBinding CompilationUnit = new RuleBinding("CompilationUnit");
            public readonly RuleBinding Specification = new RuleBinding("Specification");
            public readonly RuleBinding Module = new RuleBinding("Module");
            public readonly RuleBinding Open = new RuleBinding("Open");
            public readonly RuleBinding Paragraph = new RuleBinding("Paragraph");
            public readonly RuleBinding FactDecl = new RuleBinding("FactDecl");
            public readonly RuleBinding AssertDecl = new RuleBinding("AssertDecl");
            public readonly RuleBinding FunDecl = new RuleBinding("FunDecl");
            public readonly RuleBinding FunctionName = new RuleBinding("FunctionName");
            public readonly RuleBinding FunctionReturn = new RuleBinding("FunctionReturn");
            public readonly RuleBinding FunctionParameters = new RuleBinding("FunctionParameters");
            public readonly RuleBinding DeclList = new RuleBinding("DeclList");
            public readonly RuleBinding CmdDecl = new RuleBinding("CmdDecl");
            public readonly RuleBinding CmdScope = new RuleBinding("CmdScope");
            public readonly RuleBinding CmdScopeFor = new RuleBinding("CmdScopeFor");
            public readonly RuleBinding CmdScopeExpect = new RuleBinding("CmdScopeExpect");
            public readonly RuleBinding TypescopeDeclList = new RuleBinding("TypescopeDeclList");
            public readonly RuleBinding Typescope = new RuleBinding("Typescope");
            public readonly RuleBinding SigDecl = new RuleBinding("SigDecl");
            public readonly RuleBinding NameList = new RuleBinding("NameList");
            public readonly RuleBinding NameDeclList = new RuleBinding("NameDeclList");
            public readonly RuleBinding SigBody = new RuleBinding("SigBody");
            public readonly RuleBinding EnumDecl = new RuleBinding("EnumDecl");
            public readonly RuleBinding EnumBody = new RuleBinding("EnumBody");
            public readonly RuleBinding SigQual = new RuleBinding("SigQual");
            public readonly RuleBinding SigExt = new RuleBinding("SigExt");
            public readonly RuleBinding Expr = new RuleBinding("Expr");
            public readonly RuleBinding UnOpExpr1 = new RuleBinding("UnOpExpr1");
            public readonly RuleBinding LetDecls = new RuleBinding("LetDecls");
            public readonly RuleBinding QuantDecls = new RuleBinding("QuantDecls");
            public readonly RuleBinding BinaryExpression = new RuleBinding("BinaryExpression");
            public readonly RuleBinding UnaryExpression = new RuleBinding("UnaryExpression");
            public readonly RuleBinding ElseClause = new RuleBinding("ElseClause");
            public readonly RuleBinding BinOpExpr18 = new RuleBinding("BinOpExpr18");
            public readonly RuleBinding CallArguments = new RuleBinding("CallArguments");
            public readonly RuleBinding UnOpExpr19 = new RuleBinding("UnOpExpr19");
            public readonly RuleBinding PrimaryExpr = new RuleBinding("PrimaryExpr");
            public readonly RuleBinding Decl = new RuleBinding("Decl");
            public readonly RuleBinding LetDecl = new RuleBinding("LetDecl");
            public readonly RuleBinding Quant = new RuleBinding("Quant");
            public readonly RuleBinding ArrowMultiplicity = new RuleBinding("ArrowMultiplicity");
            public readonly RuleBinding Block = new RuleBinding("Block");
            public readonly RuleBinding Name = new RuleBinding("Name");
            public readonly RuleBinding Number = new RuleBinding("Number");
            public readonly RuleBinding Ref = new RuleBinding("Ref");
        }
    }
}
