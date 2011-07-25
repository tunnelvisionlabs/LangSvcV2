namespace Tvl.VisualStudio.Language.Go.Experimental
{
    using System.Collections.Generic;
    using Tvl.VisualStudio.Language.Parsing.Collections;
    using Tvl.VisualStudio.Language.Parsing.Experimental.Atn;
    using Contract = System.Diagnostics.Contracts.Contract;

    internal class GoSimplifiedAtnBuilder : NetworkBuilder
    {
        private readonly RuleBindings _ruleBindings;
        private readonly List<RuleBinding> _rules;

        public GoSimplifiedAtnBuilder()
        {
            _ruleBindings = new RuleBindings();
            _rules =
                new List<RuleBinding>()
                {
                    Bindings.Type,
                    Bindings.TypeNoParens,
                    Bindings.TypeName,
                    Bindings.TypeLit,
                    Bindings.ArrayType,
                    Bindings.ArrayLength,
                    Bindings.ElementType,
                    Bindings.SliceType,
                    Bindings.StructType,
                    Bindings.FieldDecl,
                    Bindings.FieldIdentifierList,
                    Bindings.AnonymousField,
                    Bindings.Tag,
                    Bindings.PointerType,
                    Bindings.BaseType,
                    Bindings.FunctionType,
                    Bindings.Signature,
                    Bindings.Parameters,
                    Bindings.ParameterList,
                    Bindings.ParameterDecl,
                    Bindings.ParameterType,
                    Bindings.InterfaceType,
                    Bindings.MethodSpec,
                    Bindings.MethodName,
                    Bindings.InterfaceTypeName,
                    Bindings.MapType,
                    Bindings.KeyType,
                    Bindings.ChannelType,
                    Bindings.Channel,
                    Bindings.SendChannel,
                    Bindings.RecvChannel,
                    Bindings.Block,
                    Bindings.Declaration,
                    Bindings.TopLevelDecl,
                    Bindings.ConstDecl,
                    Bindings.ConstSpec,
                    Bindings.IdentifierList,
                    Bindings.ExpressionList,
                    Bindings.TypeDecl,
                    Bindings.TypeSpec,
                    Bindings.VarDecl,
                    Bindings.VarSpec,
                    Bindings.ShortVarDecl,
                    Bindings.FunctionDecl,
                    Bindings.FunctionDeclHeader,
                    Bindings.Body,
                    Bindings.MethodDecl,
                    Bindings.MethodDeclHeader,
                    Bindings.Receiver,
                    Bindings.BaseTypeName,
                    Bindings.Operand,
                    Bindings.Literal,
                    Bindings.BasicLit,
                    Bindings.QualifiedIdent,
                    Bindings.CompositeLit,
                    Bindings.LiteralType,
                    Bindings.LiteralValue,
                    Bindings.ElementList,
                    Bindings.Element,
                    Bindings.Key,
                    Bindings.FieldName,
                    Bindings.ElementIndex,
                    Bindings.Value,
                    Bindings.FunctionLit,
                    Bindings.PrimaryExpr,
                    Bindings.Selector,
                    Bindings.IndexOrSlice,
                    Bindings.TypeAssertion,
                    Bindings.Call,
                    Bindings.Expression,
                    Bindings.UnaryExpr,
                    Bindings.BinaryOp,
                    Bindings.LogOp,
                    Bindings.ComOp,
                    Bindings.RelOp,
                    Bindings.AddOp,
                    Bindings.MulOp,
                    Bindings.UnaryOp,
                    Bindings.MethodExpr,
                    Bindings.ReceiverType,
                    Bindings.Conversion,
                    Bindings.Statement,
                    Bindings.SimpleStmt,
                    Bindings.EmptyStmt,
                    Bindings.LabeledStmt,
                    Bindings.Label,
                    Bindings.ExpressionStmt,
                    Bindings.IncDecStmt,
                    Bindings.Assignment,
                    Bindings.AssignOp,
                    Bindings.IfStmt,
                    Bindings.SwitchStmt,
                    Bindings.ExprSwitchStmt,
                    Bindings.ExprCaseClause,
                    Bindings.ExprSwitchCase,
                    Bindings.TypeSwitchStmt,
                    Bindings.TypeSwitchGuard,
                    Bindings.TypeCaseClause,
                    Bindings.TypeSwitchCase,
                    Bindings.TypeList,
                    Bindings.ForStmt,
                    Bindings.Condition,
                    Bindings.ForClause,
                    Bindings.InitStmt,
                    Bindings.PostStmt,
                    Bindings.RangeClause,
                    Bindings.GoStmt,
                    Bindings.SelectStmt,
                    Bindings.CommClause,
                    Bindings.CommCase,
                    Bindings.ReturnStmt,
                    Bindings.BreakStmt,
                    Bindings.ContinueStmt,
                    Bindings.GotoStmt,
                    Bindings.FallthroughStmt,
                    Bindings.DeferStmt,
                    Bindings.CompilationUnit,
                    Bindings.PackageClause,
                    Bindings.PackageName,
                    Bindings.ImportDecl,
                    Bindings.ImportSpec,
                    Bindings.SymbolDefinitionIdentifier,
                    Bindings.SymbolReferenceIdentifier,
                };
        }

        protected RuleBindings Bindings
        {
            get
            {
                return _ruleBindings;
            }
        }

        protected override IList<RuleBinding> Rules
        {
            get
            {
                return _rules;
            }
        }

        protected static void TryBindRule(RuleBinding ruleBinding, Nfa nfa)
        {
            Contract.Requires(ruleBinding != null);
            if (nfa == null)
                return;

            Nfa.BindRule(ruleBinding, nfa);
        }

        protected sealed override void BindRules()
        {
            BindRulesImpl();
            _rules.RemoveAll(i => i.StartState.OutgoingTransitions.Count == 0);
        }

        protected virtual void BindRulesImpl()
        {
            TryBindRule(Bindings.Type, this.BuildTypeRule());
            TryBindRule(Bindings.TypeNoParens, this.BuildTypeNoParensRule());
            TryBindRule(Bindings.TypeName, this.BuildTypeNameRule());
            TryBindRule(Bindings.TypeLit, this.BuildTypeLitRule());
            TryBindRule(Bindings.ArrayType, this.BuildArrayTypeRule());
            TryBindRule(Bindings.ArrayLength, this.BuildArrayLengthRule());
            TryBindRule(Bindings.ElementType, this.BuildElementTypeRule());
            TryBindRule(Bindings.SliceType, this.BuildSliceTypeRule());
            TryBindRule(Bindings.StructType, this.BuildStructTypeRule());
            TryBindRule(Bindings.FieldDecl, this.BuildFieldDeclRule());
            TryBindRule(Bindings.FieldIdentifierList, this.BuildFieldIdentifierListRule());
            TryBindRule(Bindings.AnonymousField, this.BuildAnonymousFieldRule());
            TryBindRule(Bindings.Tag, this.BuildTagRule());
            TryBindRule(Bindings.PointerType, this.BuildPointerTypeRule());
            TryBindRule(Bindings.BaseType, this.BuildBaseTypeRule());
            TryBindRule(Bindings.FunctionType, this.BuildFunctionTypeRule());
            TryBindRule(Bindings.Signature, this.BuildSignatureRule());
            TryBindRule(Bindings.Parameters, this.BuildParametersRule());
            TryBindRule(Bindings.ParameterList, this.BuildParameterListRule());
            TryBindRule(Bindings.ParameterDecl, this.BuildParameterDeclRule());
            TryBindRule(Bindings.ParameterType, this.BuildParameterTypeRule());
            TryBindRule(Bindings.InterfaceType, this.BuildInterfaceTypeRule());
            TryBindRule(Bindings.MethodSpec, this.BuildMethodSpecRule());
            TryBindRule(Bindings.MethodName, this.BuildMethodNameRule());
            TryBindRule(Bindings.InterfaceTypeName, this.BuildInterfaceTypeNameRule());
            TryBindRule(Bindings.MapType, this.BuildMapTypeRule());
            TryBindRule(Bindings.KeyType, this.BuildKeyTypeRule());
            TryBindRule(Bindings.ChannelType, this.BuildChannelTypeRule());
            TryBindRule(Bindings.Channel, this.BuildChannelRule());
            TryBindRule(Bindings.SendChannel, this.BuildSendChannelRule());
            TryBindRule(Bindings.RecvChannel, this.BuildRecvChannelRule());
            TryBindRule(Bindings.Block, this.BuildBlockRule());
            TryBindRule(Bindings.Declaration, this.BuildDeclarationRule());
            TryBindRule(Bindings.TopLevelDecl, this.BuildTopLevelDeclRule());
            TryBindRule(Bindings.ConstDecl, this.BuildConstDeclRule());
            TryBindRule(Bindings.ConstSpec, this.BuildConstSpecRule());
            TryBindRule(Bindings.IdentifierList, this.BuildIdentifierListRule());
            TryBindRule(Bindings.ExpressionList, this.BuildExpressionListRule());
            TryBindRule(Bindings.TypeDecl, this.BuildTypeDeclRule());
            TryBindRule(Bindings.TypeSpec, this.BuildTypeSpecRule());
            TryBindRule(Bindings.VarDecl, this.BuildVarDeclRule());
            TryBindRule(Bindings.VarSpec, this.BuildVarSpecRule());
            TryBindRule(Bindings.ShortVarDecl, this.BuildShortVarDeclRule());
            TryBindRule(Bindings.FunctionDecl, this.BuildFunctionDeclRule());
            TryBindRule(Bindings.FunctionDeclHeader, this.BuildFunctionDeclHeaderRule());
            TryBindRule(Bindings.Body, this.BuildBodyRule());
            TryBindRule(Bindings.MethodDecl, this.BuildMethodDeclRule());
            TryBindRule(Bindings.MethodDeclHeader, this.BuildMethodDeclHeaderRule());
            TryBindRule(Bindings.Receiver, this.BuildReceiverRule());
            TryBindRule(Bindings.BaseTypeName, this.BuildBaseTypeNameRule());
            TryBindRule(Bindings.Operand, this.BuildOperandRule());
            TryBindRule(Bindings.Literal, this.BuildLiteralRule());
            TryBindRule(Bindings.BasicLit, this.BuildBasicLitRule());
            TryBindRule(Bindings.QualifiedIdent, this.BuildQualifiedIdentRule());
            TryBindRule(Bindings.CompositeLit, this.BuildCompositeLitRule());
            TryBindRule(Bindings.LiteralType, this.BuildLiteralTypeRule());
            TryBindRule(Bindings.LiteralValue, this.BuildLiteralValueRule());
            TryBindRule(Bindings.ElementList, this.BuildElementListRule());
            TryBindRule(Bindings.Element, this.BuildElementRule());
            TryBindRule(Bindings.Key, this.BuildKeyRule());
            TryBindRule(Bindings.FieldName, this.BuildFieldNameRule());
            TryBindRule(Bindings.ElementIndex, this.BuildElementIndexRule());
            TryBindRule(Bindings.Value, this.BuildValueRule());
            TryBindRule(Bindings.FunctionLit, this.BuildFunctionLitRule());
            TryBindRule(Bindings.PrimaryExpr, this.BuildPrimaryExprRule());
            TryBindRule(Bindings.Selector, this.BuildSelectorRule());
            TryBindRule(Bindings.IndexOrSlice, this.BuildIndexOrSliceRule());
            TryBindRule(Bindings.TypeAssertion, this.BuildTypeAssertionRule());
            TryBindRule(Bindings.Call, this.BuildCallRule());
            TryBindRule(Bindings.Expression, this.BuildExpressionRule());
            TryBindRule(Bindings.UnaryExpr, this.BuildUnaryExprRule());
            TryBindRule(Bindings.BinaryOp, this.BuildBinaryOpRule());
            TryBindRule(Bindings.LogOp, this.BuildLogOpRule());
            TryBindRule(Bindings.ComOp, this.BuildComOpRule());
            TryBindRule(Bindings.RelOp, this.BuildRelOpRule());
            TryBindRule(Bindings.AddOp, this.BuildAddOpRule());
            TryBindRule(Bindings.MulOp, this.BuildMulOpRule());
            TryBindRule(Bindings.UnaryOp, this.BuildUnaryOpRule());
            TryBindRule(Bindings.MethodExpr, this.BuildMethodExprRule());
            TryBindRule(Bindings.ReceiverType, this.BuildReceiverTypeRule());
            TryBindRule(Bindings.Conversion, this.BuildConversionRule());
            TryBindRule(Bindings.Statement, this.BuildStatementRule());
            TryBindRule(Bindings.SimpleStmt, this.BuildSimpleStmtRule());
            TryBindRule(Bindings.EmptyStmt, this.BuildEmptyStmtRule());
            TryBindRule(Bindings.LabeledStmt, this.BuildLabeledStmtRule());
            TryBindRule(Bindings.Label, this.BuildLabelRule());
            TryBindRule(Bindings.ExpressionStmt, this.BuildExpressionStmtRule());
            TryBindRule(Bindings.IncDecStmt, this.BuildIncDecStmtRule());
            TryBindRule(Bindings.Assignment, this.BuildAssignmentRule());
            TryBindRule(Bindings.AssignOp, this.BuildAssignOpRule());
            TryBindRule(Bindings.IfStmt, this.BuildIfStmtRule());
            TryBindRule(Bindings.SwitchStmt, this.BuildSwitchStmtRule());
            TryBindRule(Bindings.ExprSwitchStmt, this.BuildExprSwitchStmtRule());
            TryBindRule(Bindings.ExprCaseClause, this.BuildExprCaseClauseRule());
            TryBindRule(Bindings.ExprSwitchCase, this.BuildExprSwitchCaseRule());
            TryBindRule(Bindings.TypeSwitchStmt, this.BuildTypeSwitchStmtRule());
            TryBindRule(Bindings.TypeSwitchGuard, this.BuildTypeSwitchGuardRule());
            TryBindRule(Bindings.TypeCaseClause, this.BuildTypeCaseClauseRule());
            TryBindRule(Bindings.TypeSwitchCase, this.BuildTypeSwitchCaseRule());
            TryBindRule(Bindings.TypeList, this.BuildTypeListRule());
            TryBindRule(Bindings.ForStmt, this.BuildForStmtRule());
            TryBindRule(Bindings.Condition, this.BuildConditionRule());
            TryBindRule(Bindings.ForClause, this.BuildForClauseRule());
            TryBindRule(Bindings.InitStmt, this.BuildInitStmtRule());
            TryBindRule(Bindings.PostStmt, this.BuildPostStmtRule());
            TryBindRule(Bindings.RangeClause, this.BuildRangeClauseRule());
            TryBindRule(Bindings.GoStmt, this.BuildGoStmtRule());
            TryBindRule(Bindings.SelectStmt, this.BuildSelectStmtRule());
            TryBindRule(Bindings.CommClause, this.BuildCommClauseRule());
            TryBindRule(Bindings.CommCase, this.BuildCommCaseRule());
            TryBindRule(Bindings.ReturnStmt, this.BuildReturnStmtRule());
            TryBindRule(Bindings.BreakStmt, this.BuildBreakStmtRule());
            TryBindRule(Bindings.ContinueStmt, this.BuildContinueStmtRule());
            TryBindRule(Bindings.GotoStmt, this.BuildGotoStmtRule());
            TryBindRule(Bindings.FallthroughStmt, this.BuildFallthroughStmtRule());
            TryBindRule(Bindings.DeferStmt, this.BuildDeferStmtRule());
            TryBindRule(Bindings.CompilationUnit, this.BuildCompilationUnitRule());
            TryBindRule(Bindings.PackageClause, this.BuildPackageClauseRule());
            TryBindRule(Bindings.PackageName, this.BuildPackageNameRule());
            TryBindRule(Bindings.ImportDecl, this.BuildImportDeclRule());
            TryBindRule(Bindings.ImportSpec, this.BuildImportSpecRule());
            TryBindRule(Bindings.SymbolDefinitionIdentifier, this.BuildSymbolDefinitionIdentifierRule());
            TryBindRule(Bindings.SymbolReferenceIdentifier, this.BuildSymbolReferenceIdentifierRule());

            Bindings.CompilationUnit.IsStartRule = true;
        }

        protected virtual Nfa BuildTypeRule()
        {
            return Nfa.Choice(
                Nfa.Rule(Bindings.TypeName),
                Nfa.Rule(Bindings.TypeLit),
                Nfa.Sequence(
                    Nfa.Match(GoLexer.LPAREN),
                    Nfa.Rule(Bindings.Type),
                    Nfa.Match(GoLexer.RPAREN)));
        }

        protected virtual Nfa BuildTypeNoParensRule()
        {
            return Nfa.Choice(
                Nfa.Rule(Bindings.TypeName),
                Nfa.Rule(Bindings.TypeLit));
        }

        protected virtual Nfa BuildTypeNameRule()
        {
            return Nfa.Rule(Bindings.QualifiedIdent);
        }

        protected virtual Nfa BuildTypeLitRule()
        {
            return Nfa.Choice(
                Nfa.Rule(Bindings.ArrayType),
                Nfa.Rule(Bindings.StructType),
                Nfa.Rule(Bindings.PointerType),
                Nfa.Rule(Bindings.FunctionType),
                Nfa.Rule(Bindings.InterfaceType),
                Nfa.Rule(Bindings.SliceType),
                Nfa.Rule(Bindings.MapType),
                Nfa.Rule(Bindings.ChannelType));
        }

        protected virtual Nfa BuildArrayTypeRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.LBRACK),
                Nfa.Rule(Bindings.ArrayLength),
                Nfa.Match(GoLexer.RBRACK),
                Nfa.Rule(Bindings.ElementType));
        }

        protected virtual Nfa BuildArrayLengthRule()
        {
            return Nfa.Rule(Bindings.Expression);
        }

        protected virtual Nfa BuildElementTypeRule()
        {
            return Nfa.Rule(Bindings.Type);
        }

        protected virtual Nfa BuildSliceTypeRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.LBRACK),
                Nfa.Match(GoLexer.RBRACK),
                Nfa.Rule(Bindings.ElementType));
        }

        protected virtual Nfa BuildStructTypeRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.KW_STRUCT),
                Nfa.Match(GoLexer.LBRACE),
                Nfa.Choice(
                    Nfa.Match(GoLexer.RBRACE),
                    Nfa.Sequence(
                        Nfa.Rule(Bindings.FieldDecl),
                        Nfa.Closure(
                            Nfa.Sequence(
                                Nfa.Match(GoLexer.SEMI),
                                Nfa.Rule(Bindings.FieldDecl))),
                        Nfa.Optional(Nfa.Match(GoLexer.SEMI)),
                        Nfa.Match(GoLexer.RBRACE))));
        }

        protected virtual Nfa BuildFieldDeclRule()
        {
            return Nfa.Choice(
                Nfa.Sequence(
                    Nfa.Rule(Bindings.FieldIdentifierList),
                    Nfa.Rule(Bindings.Type),
                    Nfa.Optional(Nfa.Rule(Bindings.Tag))),
                Nfa.Sequence(
                    Nfa.Rule(Bindings.AnonymousField),
                    Nfa.Optional(Nfa.Rule(Bindings.Tag))));
        }

        protected virtual Nfa BuildFieldIdentifierListRule()
        {
            return Nfa.Rule(Bindings.IdentifierList);
            //return Nfa.Sequence(
            //    Nfa.Match(GoLexer.IDENTIFIER),
            //    Nfa.Closure(
            //        Nfa.Sequence(
            //            Nfa.Match(GoLexer.COMMA),
            //            Nfa.Match(GoLexer.IDENTIFIER))));
        }

        protected virtual Nfa BuildAnonymousFieldRule()
        {
            return Nfa.Sequence(
                Nfa.Optional(Nfa.Match(GoLexer.TIMES)),
                Nfa.Rule(Bindings.TypeName));
        }

        protected virtual Nfa BuildTagRule()
        {
            return Nfa.Match(GoLexer.STRING_LITERAL);
        }

        protected virtual Nfa BuildPointerTypeRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.TIMES),
                Nfa.Rule(Bindings.BaseType));
        }

        protected virtual Nfa BuildBaseTypeRule()
        {
            return Nfa.Rule(Bindings.Type);
        }

        protected virtual Nfa BuildFunctionTypeRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.KW_FUNC),
                Nfa.Rule(Bindings.Signature));
        }

        protected virtual Nfa BuildSignatureRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.Parameters),
                Nfa.Choice(
                    Nfa.Rule(Bindings.Parameters),
                    Nfa.Optional(Nfa.Rule(Bindings.TypeNoParens))));
        }

        protected virtual Nfa BuildParametersRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.LPAREN),
                Nfa.Optional(
                    Nfa.Sequence(
                        Nfa.Rule(Bindings.ParameterList),
                        Nfa.Optional(Nfa.Match(GoLexer.COMMA)))),
                Nfa.Match(GoLexer.RPAREN));
        }

        protected virtual Nfa BuildParameterListRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.ParameterDecl),
                Nfa.Closure(
                    Nfa.Sequence(
                        Nfa.Match(GoLexer.COMMA),
                        Nfa.Rule(Bindings.ParameterDecl))));
        }

        protected virtual Nfa BuildParameterDeclRule()
        {
            return Nfa.Choice(
                Nfa.Sequence(
                    Nfa.Rule(Bindings.IdentifierList),
                    Nfa.Rule(Bindings.ParameterType)),
                Nfa.Rule(Bindings.ParameterType));
        }

        protected virtual Nfa BuildParameterTypeRule()
        {
            return Nfa.Sequence(
                Nfa.Optional(Nfa.Match(GoLexer.ELLIP)),
                Nfa.Rule(Bindings.Type));
        }

        protected virtual Nfa BuildInterfaceTypeRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.KW_INTERFACE),
                Nfa.Match(GoLexer.LBRACE),
                Nfa.Choice(
                    Nfa.Match(GoLexer.RBRACE),
                    Nfa.Sequence(
                        Nfa.Rule(Bindings.MethodSpec),
                        Nfa.Closure(
                            Nfa.Sequence(
                                Nfa.Match(GoLexer.SEMI),
                                Nfa.Rule(Bindings.MethodSpec))),
                        Nfa.Optional(Nfa.Match(GoLexer.SEMI)),
                        Nfa.Match(GoLexer.RBRACE))));
        }

        protected virtual Nfa BuildMethodSpecRule()
        {
            return Nfa.Choice(
                Nfa.Sequence(
                    Nfa.Rule(Bindings.MethodName),
                    Nfa.Rule(Bindings.Signature)),
                Nfa.Rule(Bindings.InterfaceTypeName));
        }

        protected virtual Nfa BuildMethodNameRule()
        {
            return Nfa.Rule(Bindings.SymbolDefinitionIdentifier);
        }

        protected virtual Nfa BuildInterfaceTypeNameRule()
        {
            return Nfa.Rule(Bindings.TypeName);
        }

        protected virtual Nfa BuildMapTypeRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.KW_MAP),
                Nfa.Match(GoLexer.LBRACK),
                Nfa.Rule(Bindings.KeyType),
                Nfa.Match(GoLexer.RBRACK),
                Nfa.Rule(Bindings.ElementType));
        }

        protected virtual Nfa BuildKeyTypeRule()
        {
            return Nfa.Rule(Bindings.Type);
        }

        protected virtual Nfa BuildChannelTypeRule()
        {
            return Nfa.Choice(
                Nfa.Rule(Bindings.RecvChannel),
                Nfa.Rule(Bindings.SendChannel),
                Nfa.Rule(Bindings.Channel));
        }

        protected virtual Nfa BuildChannelRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.KW_CHAN),
                Nfa.Rule(Bindings.ElementType));
        }

        protected virtual Nfa BuildSendChannelRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.KW_CHAN),
                Nfa.Match(GoLexer.CHAN),
                Nfa.Rule(Bindings.ElementType));
        }

        protected virtual Nfa BuildRecvChannelRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.CHAN),
                Nfa.Match(GoLexer.KW_CHAN),
                Nfa.Rule(Bindings.ElementType));
        }

        protected virtual Nfa BuildBlockRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.LBRACE),
                Nfa.Rule(Bindings.Statement),
                Nfa.Closure(
                    Nfa.Sequence(
                        Nfa.Match(GoLexer.SEMI),
                        Nfa.Rule(Bindings.Statement))),
                Nfa.Match(GoLexer.RBRACE));
        }

        protected virtual Nfa BuildDeclarationRule()
        {
            return Nfa.Choice(
                Nfa.Rule(Bindings.ConstDecl),
                Nfa.Rule(Bindings.TypeDecl),
                Nfa.Rule(Bindings.VarDecl));
        }

        protected virtual Nfa BuildTopLevelDeclRule()
        {
            return Nfa.Choice(
                Nfa.Rule(Bindings.Declaration),
                Nfa.Rule(Bindings.FunctionDecl),
                Nfa.Rule(Bindings.MethodDecl));
        }

        protected virtual Nfa BuildConstDeclRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.KW_CONST),
                Nfa.Choice(
                    Nfa.Rule(Bindings.ConstSpec),
                    Nfa.Sequence(
                        Nfa.Match(GoLexer.LPAREN),
                        Nfa.Closure(
                            Nfa.Sequence(
                                Nfa.Rule(Bindings.ConstSpec),
                                Nfa.Match(GoLexer.SEMI))),
                        Nfa.Match(GoLexer.RPAREN))));
        }

        protected virtual Nfa BuildConstSpecRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.IdentifierList),
                Nfa.Optional(
                    Nfa.Sequence(
                        Nfa.Optional(Nfa.Rule(Bindings.Type)),
                        Nfa.Match(GoLexer.EQ),
                        Nfa.Rule(Bindings.ExpressionList))));
        }

        protected virtual Nfa BuildIdentifierListRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.SymbolDefinitionIdentifier),
                Nfa.Closure(
                    Nfa.Sequence(
                        Nfa.Match(GoLexer.COMMA),
                        Nfa.Rule(Bindings.SymbolDefinitionIdentifier))));
        }

        protected virtual Nfa BuildExpressionListRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.Expression),
                Nfa.Closure(
                    Nfa.Sequence(
                        Nfa.Match(GoLexer.COMMA),
                        Nfa.Rule(Bindings.Expression))));
        }

        protected virtual Nfa BuildTypeDeclRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.KW_TYPE),
                Nfa.Choice(
                    Nfa.Rule(Bindings.TypeSpec),
                    Nfa.Sequence(
                        Nfa.Match(GoLexer.LPAREN),
                        Nfa.Closure(
                            Nfa.Sequence(
                                Nfa.Rule(Bindings.TypeSpec),
                                Nfa.Match(GoLexer.SEMI))),
                        Nfa.Match(GoLexer.RPAREN))));
        }

        protected virtual Nfa BuildTypeSpecRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.SymbolDefinitionIdentifier),
                Nfa.Rule(Bindings.Type));
        }

        protected virtual Nfa BuildVarDeclRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.KW_VAR),
                Nfa.Choice(
                    Nfa.Rule(Bindings.VarSpec),
                    Nfa.Sequence(
                        Nfa.Match(GoLexer.LPAREN),
                        Nfa.Closure(
                            Nfa.Sequence(
                                Nfa.Rule(Bindings.VarSpec),
                                Nfa.Match(GoLexer.SEMI))),
                        Nfa.Match(GoLexer.RPAREN))));
        }

        protected virtual Nfa BuildVarSpecRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.IdentifierList),
                Nfa.Choice(
                    Nfa.Sequence(
                        Nfa.Rule(Bindings.Type),
                        Nfa.Optional(
                            Nfa.Sequence(
                                Nfa.Match(GoLexer.EQ),
                                Nfa.Rule(Bindings.ExpressionList)))),
                    Nfa.Sequence(
                        Nfa.Match(GoLexer.EQ),
                        Nfa.Rule(Bindings.ExpressionList))));
        }

        protected virtual Nfa BuildShortVarDeclRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.IdentifierList),
                Nfa.Match(GoLexer.DEFEQ),
                Nfa.Rule(Bindings.ExpressionList));
        }

        protected virtual Nfa BuildFunctionDeclRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.FunctionDeclHeader),
                Nfa.Optional(Nfa.Rule(Bindings.Body)));
        }

        protected virtual Nfa BuildFunctionDeclHeaderRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.KW_FUNC),
                Nfa.Rule(Bindings.SymbolDefinitionIdentifier),
                Nfa.Rule(Bindings.Signature));
        }

        protected virtual Nfa BuildBodyRule()
        {
            return Nfa.Rule(Bindings.Block);
        }

        protected virtual Nfa BuildMethodDeclRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.MethodDeclHeader),
                Nfa.Optional(Nfa.Rule(Bindings.Body)));
        }

        protected virtual Nfa BuildMethodDeclHeaderRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.KW_FUNC),
                Nfa.Rule(Bindings.Receiver),
                Nfa.Rule(Bindings.MethodName),
                Nfa.Rule(Bindings.Signature));
        }

        protected virtual Nfa BuildReceiverRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.LPAREN),
                Nfa.Optional(Nfa.Rule(Bindings.SymbolDefinitionIdentifier)),
                Nfa.Optional(Nfa.Match(GoLexer.TIMES)),
                Nfa.Rule(Bindings.BaseTypeName),
                Nfa.Match(GoLexer.RPAREN));
        }

        protected virtual Nfa BuildBaseTypeNameRule()
        {
            return Nfa.Rule(Bindings.SymbolReferenceIdentifier);
        }

        protected virtual Nfa BuildOperandRule()
        {
            return Nfa.Choice(
                Nfa.Rule(Bindings.Literal),
                Nfa.Rule(Bindings.MethodExpr),
                Nfa.Rule(Bindings.QualifiedIdent),
                Nfa.Sequence(
                    Nfa.Match(GoLexer.LPAREN),
                    Nfa.Rule(Bindings.Expression),
                    Nfa.Match(GoLexer.RPAREN)));
        }

        protected virtual Nfa BuildLiteralRule()
        {
            return Nfa.Choice(
                Nfa.Rule(Bindings.BasicLit),
                Nfa.Rule(Bindings.CompositeLit),
                Nfa.Rule(Bindings.FunctionLit));
        }

        protected virtual Nfa BuildBasicLitRule()
        {
            return Nfa.MatchAny(GoLexer.NUMBER, GoLexer.CHAR_LITERAL, GoLexer.STRING_LITERAL);
        }

        protected virtual Nfa BuildQualifiedIdentRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.SymbolReferenceIdentifier),
                Nfa.Optional(
                    Nfa.Sequence(
                        Nfa.Match(GoLexer.DOT),
                        Nfa.Rule(Bindings.SymbolReferenceIdentifier))));
        }

        protected virtual Nfa BuildCompositeLitRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.LiteralType),
                Nfa.Rule(Bindings.LiteralValue));
        }

        protected virtual Nfa BuildLiteralTypeRule()
        {
            return Nfa.Choice(
                Nfa.Rule(Bindings.StructType),
                Nfa.Rule(Bindings.ArrayType),
                Nfa.Sequence(
                    Nfa.Match(GoLexer.LBRACK),
                    Nfa.Match(GoLexer.ELLIP),
                    Nfa.Match(GoLexer.RBRACK),
                    Nfa.Rule(Bindings.ElementType)),
                Nfa.Rule(Bindings.SliceType),
                Nfa.Rule(Bindings.MapType),
                Nfa.Rule(Bindings.TypeName),
                Nfa.Sequence(
                    Nfa.Match(GoLexer.LPAREN),
                    Nfa.Rule(Bindings.LiteralType),
                    Nfa.Match(GoLexer.RPAREN)));
        }

        protected virtual Nfa BuildLiteralValueRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.LBRACE),
                Nfa.Optional(
                    Nfa.Sequence(
                        Nfa.Rule(Bindings.ElementList),
                        Nfa.Optional(Nfa.Match(GoLexer.COMMA)))),
                Nfa.Match(GoLexer.RBRACE));
        }

        protected virtual Nfa BuildElementListRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.Element),
                Nfa.Closure(
                    Nfa.Sequence(
                        Nfa.Match(GoLexer.COMMA),
                        Nfa.Rule(Bindings.Element))));
        }

        protected virtual Nfa BuildElementRule()
        {
            return Nfa.Sequence(
                Nfa.Optional(
                    Nfa.Sequence(
                        Nfa.Rule(Bindings.Key),
                        Nfa.Match(GoLexer.COLON))),
                Nfa.Rule(Bindings.Value));
        }

        protected virtual Nfa BuildKeyRule()
        {
            return Nfa.Rule(Bindings.ElementIndex);
        }

        protected virtual Nfa BuildFieldNameRule()
        {
            return Nfa.Rule(Bindings.SymbolReferenceIdentifier);
        }

        protected virtual Nfa BuildElementIndexRule()
        {
            return Nfa.Rule(Bindings.Expression);
        }

        protected virtual Nfa BuildValueRule()
        {
            return Nfa.Choice(
                Nfa.Rule(Bindings.Expression),
                Nfa.Rule(Bindings.LiteralValue));
        }

        protected virtual Nfa BuildFunctionLitRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.FunctionType),
                Nfa.Rule(Bindings.Body));
        }

        protected virtual Nfa BuildPrimaryExprRule()
        {
            return Nfa.Sequence(
                Nfa.Choice(
                    Nfa.Rule(Bindings.Operand),
                    Nfa.Rule(Bindings.Conversion)),
                Nfa.Closure(
                    Nfa.Choice(
                        Nfa.Rule(Bindings.Selector),
                        Nfa.Rule(Bindings.IndexOrSlice),
                        Nfa.Rule(Bindings.TypeAssertion),
                        Nfa.Rule(Bindings.Call))));
        }

        protected virtual Nfa BuildSelectorRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.DOT),
                Nfa.Rule(Bindings.SymbolReferenceIdentifier));
        }

        protected virtual Nfa BuildIndexOrSliceRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.LBRACK),
                Nfa.Rule(Bindings.Expression),
                Nfa.Choice(
                    Nfa.Sequence(
                        Nfa.Match(GoLexer.COLON),
                        Nfa.Optional(Nfa.Rule(Bindings.Expression)),
                        Nfa.Match(GoLexer.RBRACK)),
                    Nfa.Match(GoLexer.RBRACK)));
        }

        protected virtual Nfa BuildTypeAssertionRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.DOT),
                Nfa.Match(GoLexer.LPAREN),
                Nfa.Rule(Bindings.Type),
                Nfa.Match(GoLexer.RPAREN));
        }

        protected virtual Nfa BuildCallRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.LPAREN),
                Nfa.Optional(
                    Nfa.Choice(
                        Nfa.Sequence(
                            Nfa.Rule(Bindings.ExpressionList),
                            Nfa.Optional(Nfa.Match(GoLexer.COMMA))),
                        Nfa.Sequence(
                            Nfa.Rule(Bindings.Type),
                            Nfa.Optional(
                                Nfa.Sequence(
                                    Nfa.Match(GoLexer.COMMA),
                                    Nfa.Rule(Bindings.ExpressionList))),
                            Nfa.Optional(Nfa.Match(GoLexer.COMMA))))),
                Nfa.Match(GoLexer.RPAREN));
        }

        protected virtual Nfa BuildExpressionRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.UnaryExpr),
                Nfa.Closure(
                    Nfa.Sequence(
                        Nfa.Rule(Bindings.BinaryOp),
                        Nfa.Rule(Bindings.UnaryExpr))));
        }

        protected virtual Nfa BuildUnaryExprRule()
        {
            return Nfa.Choice(
                Nfa.Sequence(
                    Nfa.Rule(Bindings.UnaryOp),
                    Nfa.Rule(Bindings.UnaryExpr)),
                Nfa.Rule(Bindings.PrimaryExpr));
        }

        protected virtual Nfa BuildBinaryOpRule()
        {
            return Nfa.Choice(
                Nfa.Rule(Bindings.LogOp),
                Nfa.Rule(Bindings.ComOp),
                Nfa.Rule(Bindings.RelOp),
                Nfa.Rule(Bindings.AddOp),
                Nfa.Rule(Bindings.MulOp));
        }

        protected virtual Nfa BuildLogOpRule()
        {
            return Nfa.MatchAny(GoLexer.OR, GoLexer.AND);
        }

        protected virtual Nfa BuildComOpRule()
        {
            return Nfa.Match(GoLexer.CHAN);
        }

        protected virtual Nfa BuildRelOpRule()
        {
            return Nfa.MatchAny(GoLexer.EQEQ, GoLexer.NEQ, GoLexer.LT, GoLexer.LE, GoLexer.GT, GoLexer.GE);
        }

        protected virtual Nfa BuildAddOpRule()
        {
            return Nfa.MatchAny(GoLexer.PLUS, GoLexer.MINUS, GoLexer.BITOR, GoLexer.XOR);
        }

        protected virtual Nfa BuildMulOpRule()
        {
            return Nfa.MatchAny(GoLexer.TIMES, GoLexer.DIV, GoLexer.MOD, GoLexer.LSHIFT, GoLexer.RSHIFT, GoLexer.BITAND, GoLexer.BITCLR);
        }

        protected virtual Nfa BuildUnaryOpRule()
        {
            return Nfa.MatchAny(GoLexer.PLUS, GoLexer.MINUS, GoLexer.NOT, GoLexer.XOR, GoLexer.TIMES, GoLexer.BITAND, GoLexer.CHAN);
        }

        protected virtual Nfa BuildMethodExprRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.ReceiverType),
                Nfa.Match(GoLexer.DOT),
                Nfa.Rule(Bindings.SymbolReferenceIdentifier));
                //Nfa.Rule(Bindings.MethodName));
        }

        protected virtual Nfa BuildReceiverTypeRule()
        {
            return Nfa.Choice(
                Nfa.Rule(Bindings.TypeName),
                Nfa.Sequence(
                    Nfa.Match(GoLexer.LPAREN),
                    Nfa.Match(GoLexer.TIMES),
                    Nfa.Rule(Bindings.TypeName),
                    Nfa.Match(GoLexer.RPAREN)));
        }

        protected virtual Nfa BuildConversionRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.LiteralType),
                Nfa.Match(GoLexer.LPAREN),
                Nfa.Rule(Bindings.Expression),
                Nfa.Match(GoLexer.RPAREN));
        }

        protected virtual Nfa BuildStatementRule()
        {
            return Nfa.Choice(
                Nfa.Rule(Bindings.Declaration),
                Nfa.Rule(Bindings.LabeledStmt),
                Nfa.Rule(Bindings.SimpleStmt),
                Nfa.Rule(Bindings.GoStmt),
                Nfa.Rule(Bindings.ReturnStmt),
                Nfa.Rule(Bindings.BreakStmt),
                Nfa.Rule(Bindings.ContinueStmt),
                Nfa.Rule(Bindings.GotoStmt),
                Nfa.Rule(Bindings.FallthroughStmt),
                Nfa.Rule(Bindings.Block),
                Nfa.Rule(Bindings.IfStmt),
                Nfa.Rule(Bindings.SwitchStmt),
                Nfa.Rule(Bindings.SelectStmt),
                Nfa.Rule(Bindings.ForStmt),
                Nfa.Rule(Bindings.DeferStmt));
        }

        protected virtual Nfa BuildSimpleStmtRule()
        {
            //return Nfa.Choice(
            //    Nfa.Sequence(
            //        Nfa.Rule(Bindings.Expression),
            //        Nfa.Optional(
            //            Nfa.Choice(
            //                Nfa.Sequence(
            //                    Nfa.Optional(
            //                        Nfa.Sequence(
            //                            Nfa.Match(GoLexer.COMMA),
            //                            Nfa.Rule(Bindings.ExpressionList))),
            //                    Nfa.Choice(
            //                        Nfa.Sequence(
            //                            Nfa.Match(GoLexer.DEFEQ),
            //                            Nfa.Rule(Bindings.ExpressionList)),
            //                        Nfa.Sequence(
            //                            Nfa.Rule(Bindings.AssignOp),
            //                            Nfa.Rule(Bindings.ExpressionList)))),
            //                Nfa.MatchAny(GoLexer.INC, GoLexer.DEC)))),
            //    Nfa.Rule(Bindings.EmptyStmt));
            return Nfa.Choice(
                Nfa.Rule(Bindings.ShortVarDecl),
                Nfa.Rule(Bindings.Assignment),
                Nfa.Rule(Bindings.IncDecStmt),
                Nfa.Rule(Bindings.ExpressionStmt),
                Nfa.Rule(Bindings.EmptyStmt));
        }

        protected virtual Nfa BuildEmptyStmtRule()
        {
            return Nfa.Epsilon();
        }

        protected virtual Nfa BuildLabeledStmtRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.Label),
                Nfa.Match(GoLexer.COLON),
                Nfa.Rule(Bindings.Statement));
        }

        protected virtual Nfa BuildLabelRule()
        {
            return Nfa.Rule(Bindings.SymbolDefinitionIdentifier);
        }

        protected virtual Nfa BuildExpressionStmtRule()
        {
            return Nfa.Rule(Bindings.Expression);
        }

        protected virtual Nfa BuildIncDecStmtRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.Expression),
                Nfa.MatchAny(GoLexer.INC, GoLexer.DEC));
        }

        protected virtual Nfa BuildAssignmentRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.ExpressionList),
                Nfa.Rule(Bindings.AssignOp),
                Nfa.Rule(Bindings.ExpressionList));
        }

        protected virtual Nfa BuildAssignOpRule()
        {
            return Nfa.MatchAny(
                GoLexer.EQ,
                GoLexer.PLUSEQ, GoLexer.MINUSEQ, GoLexer.OREQ, GoLexer.XOREQ,
                GoLexer.TIMESEQ, GoLexer.DIVEQ, GoLexer.MODEQ, GoLexer.LSHIFTEQ, GoLexer.RSHIFTEQ, GoLexer.ANDEQ, GoLexer.BITCLREQ);
        }

        protected virtual Nfa BuildIfStmtRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.KW_IF),
                Nfa.Optional(
                    Nfa.Sequence(
                        Nfa.Rule(Bindings.SimpleStmt),
                        Nfa.Match(GoLexer.SEMI))),
                Nfa.Optional(Nfa.Rule(Bindings.Expression)),
                Nfa.Rule(Bindings.Block),
                Nfa.Optional(
                    Nfa.Sequence(
                        Nfa.Match(GoLexer.KW_ELSE),
                        Nfa.Rule(Bindings.Statement))));
        }

        protected virtual Nfa BuildSwitchStmtRule()
        {
            return Nfa.Choice(
                Nfa.Rule(Bindings.ExprSwitchStmt),
                Nfa.Rule(Bindings.TypeSwitchStmt));
        }

        protected virtual Nfa BuildExprSwitchStmtRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.KW_SWITCH),
                Nfa.Optional(
                    Nfa.Sequence(
                        Nfa.Rule(Bindings.SimpleStmt),
                        Nfa.Match(GoLexer.SEMI))),
                Nfa.Optional(Nfa.Rule(Bindings.Expression)),
                Nfa.Match(GoLexer.LBRACE),
                Nfa.Closure(Nfa.Rule(Bindings.ExprCaseClause)),
                Nfa.Match(GoLexer.RBRACE));
        }

        protected virtual Nfa BuildExprCaseClauseRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.ExprSwitchCase),
                Nfa.Match(GoLexer.COLON),
                Nfa.Closure(
                    Nfa.Sequence(
                        Nfa.Rule(Bindings.Statement),
                        Nfa.Match(GoLexer.SEMI))));
        }

        protected virtual Nfa BuildExprSwitchCaseRule()
        {
            return Nfa.Choice(
                Nfa.Sequence(
                    Nfa.Match(GoLexer.KW_CASE),
                    Nfa.Rule(Bindings.ExpressionList)),
                Nfa.Match(GoLexer.KW_DEFAULT));
        }

        protected virtual Nfa BuildTypeSwitchStmtRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.KW_SWITCH),
                Nfa.Optional(
                    Nfa.Sequence(
                        Nfa.Rule(Bindings.SimpleStmt),
                        Nfa.Match(GoLexer.SEMI))),
                Nfa.Rule(Bindings.TypeSwitchGuard),
                Nfa.Match(GoLexer.LBRACE),
                Nfa.Closure(Nfa.Rule(Bindings.TypeCaseClause)),
                Nfa.Match(GoLexer.RBRACE));
        }

        protected virtual Nfa BuildTypeSwitchGuardRule()
        {
            return Nfa.Sequence(
                Nfa.Optional(
                    Nfa.Sequence(
                        Nfa.Rule(Bindings.SymbolDefinitionIdentifier),
                        Nfa.Match(GoLexer.DEFEQ))),
                Nfa.Rule(Bindings.PrimaryExpr),
                Nfa.Match(GoLexer.DOT),
                Nfa.Match(GoLexer.LPAREN),
                Nfa.Match(GoLexer.KW_TYPE),
                Nfa.Match(GoLexer.RPAREN));
        }

        protected virtual Nfa BuildTypeCaseClauseRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.TypeSwitchCase),
                Nfa.Match(GoLexer.COLON),
                Nfa.Closure(
                    Nfa.Sequence(
                        Nfa.Rule(Bindings.Statement),
                        Nfa.Match(GoLexer.SEMI))));
        }

        protected virtual Nfa BuildTypeSwitchCaseRule()
        {
            return Nfa.Choice(
                Nfa.Sequence(
                    Nfa.Match(GoLexer.KW_CASE),
                    Nfa.Rule(Bindings.TypeList)),
                Nfa.Match(GoLexer.KW_DEFAULT));
        }

        protected virtual Nfa BuildTypeListRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.Type),
                Nfa.Closure(
                    Nfa.Sequence(
                        Nfa.Match(GoLexer.COMMA),
                        Nfa.Rule(Bindings.Type))));
        }

        protected virtual Nfa BuildForStmtRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.KW_FOR),
                Nfa.Optional(
                    Nfa.Choice(
                        Nfa.Rule(Bindings.RangeClause),
                        Nfa.Rule(Bindings.ForClause),
                        Nfa.Rule(Bindings.Condition))),
                Nfa.Rule(Bindings.Block));
        }

        protected virtual Nfa BuildConditionRule()
        {
            return Nfa.Rule(Bindings.Expression);
        }

        protected virtual Nfa BuildForClauseRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.InitStmt),
                Nfa.Match(GoLexer.SEMI),
                Nfa.Optional(Nfa.Rule(Bindings.Condition)),
                Nfa.Match(GoLexer.SEMI),
                Nfa.Rule(Bindings.PostStmt));
        }

        protected virtual Nfa BuildInitStmtRule()
        {
            return Nfa.Rule(Bindings.SimpleStmt);
        }

        protected virtual Nfa BuildPostStmtRule()
        {
            return Nfa.Rule(Bindings.SimpleStmt);
        }

        protected virtual Nfa BuildRangeClauseRule()
        {
            return Nfa.Sequence(
                Nfa.Choice(
                    Nfa.Sequence(
                        Nfa.Rule(Bindings.ExpressionList),
                        Nfa.Match(GoLexer.EQ)),
                    Nfa.Sequence(
                        Nfa.Rule(Bindings.IdentifierList),
                        Nfa.Match(GoLexer.DEFEQ))),
                Nfa.Match(GoLexer.KW_RANGE),
                Nfa.Rule(Bindings.Expression));
        }

        protected virtual Nfa BuildGoStmtRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.KW_GO),
                Nfa.Rule(Bindings.Expression));
        }

        protected virtual Nfa BuildSelectStmtRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.KW_SELECT),
                Nfa.Match(GoLexer.LBRACE),
                Nfa.Closure(Nfa.Rule(Bindings.CommClause)),
                Nfa.Match(GoLexer.RBRACE));
        }

        protected virtual Nfa BuildCommClauseRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.CommCase),
                Nfa.Match(GoLexer.COLON),
                Nfa.Closure(
                    Nfa.Sequence(
                        Nfa.Rule(Bindings.Statement),
                        Nfa.Match(GoLexer.SEMI))));
        }

        protected virtual Nfa BuildCommCaseRule()
        {
            return Nfa.Choice(
                Nfa.Sequence(
                    Nfa.Match(GoLexer.KW_CASE),
                    Nfa.Choice(
                        Nfa.Sequence(
                            Nfa.Rule(Bindings.Expression),
                            Nfa.Choice(
                                Nfa.Sequence(
                                    Nfa.MatchAny(GoLexer.EQ, GoLexer.DEFEQ),
                                    Nfa.Match(GoLexer.CHAN),
                                    Nfa.Rule(Bindings.Expression)),
                                Nfa.Sequence(
                                    Nfa.Match(GoLexer.CHAN),
                                    Nfa.Rule(Bindings.Expression)))),
                        Nfa.Sequence(
                            Nfa.Match(GoLexer.CHAN),
                            Nfa.Rule(Bindings.Expression)))),
                Nfa.Match(GoLexer.KW_DEFAULT));
        }

        protected virtual Nfa BuildReturnStmtRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.KW_RETURN),
                Nfa.Optional(Nfa.Rule(Bindings.ExpressionList)));
        }

        protected virtual Nfa BuildBreakStmtRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.KW_BREAK),
                Nfa.Optional(Nfa.Rule(Bindings.Label)));
        }

        protected virtual Nfa BuildContinueStmtRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.KW_CONTINUE),
                Nfa.Optional(Nfa.Rule(Bindings.Label)));
        }

        protected virtual Nfa BuildGotoStmtRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.KW_GOTO),
                Nfa.Rule(Bindings.Label));
        }

        protected virtual Nfa BuildFallthroughStmtRule()
        {
            return Nfa.Match(GoLexer.KW_FALLTHROUGH);
        }

        protected virtual Nfa BuildDeferStmtRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.KW_DEFER),
                Nfa.Rule(Bindings.Expression));
        }

        protected virtual Nfa BuildCompilationUnitRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.PackageClause),
                Nfa.Match(GoLexer.SEMI),
                Nfa.Closure(
                    Nfa.Sequence(
                        Nfa.Rule(Bindings.ImportDecl),
                        Nfa.Match(GoLexer.SEMI))),
                Nfa.Closure(
                    Nfa.Sequence(
                        Nfa.Rule(Bindings.TopLevelDecl),
                        Nfa.Match(GoLexer.SEMI))));
        }

        protected virtual Nfa BuildPackageClauseRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.KW_PACKAGE),
                Nfa.Rule(Bindings.PackageName));
        }

        protected virtual Nfa BuildPackageNameRule()
        {
            return Nfa.Rule(Bindings.SymbolDefinitionIdentifier);
        }

        protected virtual Nfa BuildImportDeclRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.KW_IMPORT),
                Nfa.Choice(
                    Nfa.Rule(Bindings.ImportSpec),
                    Nfa.Sequence(
                        Nfa.Match(GoLexer.LPAREN),
                        Nfa.Closure(
                            Nfa.Sequence(
                                Nfa.Rule(Bindings.ImportSpec),
                                Nfa.Match(GoLexer.SEMI))),
                        Nfa.Match(GoLexer.RPAREN))));
        }

        protected virtual Nfa BuildImportSpecRule()
        {
            return Nfa.Sequence(
                Nfa.Optional(
                    Nfa.Choice(
                        Nfa.Match(GoLexer.DOT),
                        Nfa.Rule(Bindings.SymbolReferenceIdentifier))),
                Nfa.Match(GoLexer.STRING_LITERAL));
        }

        protected virtual Nfa BuildSymbolDefinitionIdentifierRule()
        {
            return Nfa.Match(GoLexer.IDENTIFIER);
        }

        protected virtual Nfa BuildSymbolReferenceIdentifierRule()
        {
            return Nfa.Match(GoLexer.IDENTIFIER);
        }

        public static class RuleNames
        {
            public const string Type = "Type";
            public const string TypeNoParens = "TypeNoParens";
            public const string TypeName = "TypeName";
            public const string TypeLit = "TypeLit";
            public const string ArrayType = "ArrayType";
            public const string ArrayLength = "ArrayLength";
            public const string ElementType = "ElementType";
            public const string SliceType = "SliceType";
            public const string StructType = "StructType";
            public const string FieldDecl = "FieldDecl";
            public const string FieldIdentifierList = "FieldIdentifierList";
            public const string AnonymousField = "AnonymousField";
            public const string Tag = "Tag";
            public const string PointerType = "PointerType";
            public const string BaseType = "BaseType";
            public const string FunctionType = "FunctionType";
            public const string Signature = "Signature";
            public const string Parameters = "Parameters";
            public const string ParameterList = "ParameterList";
            public const string ParameterDecl = "ParameterDecl";
            public const string ParameterType = "ParameterType";
            public const string InterfaceType = "InterfaceType";
            public const string MethodSpec = "MethodSpec";
            public const string MethodName = "MethodName";
            public const string InterfaceTypeName = "InterfaceTypeName";
            public const string MapType = "MapType";
            public const string KeyType = "KeyType";
            public const string ChannelType = "ChannelType";
            public const string Channel = "Channel";
            public const string SendChannel = "SendChannel";
            public const string RecvChannel = "RecvChannel";
            public const string Block = "Block";
            public const string Declaration = "Declaration";
            public const string TopLevelDecl = "TopLevelDecl";
            public const string ConstDecl = "ConstDecl";
            public const string ConstSpec = "ConstSpec";
            public const string IdentifierList = "IdentifierList";
            public const string ExpressionList = "ExpressionList";
            public const string TypeDecl = "TypeDecl";
            public const string TypeSpec = "TypeSpec";
            public const string VarDecl = "VarDecl";
            public const string VarSpec = "VarSpec";
            public const string ShortVarDecl = "ShortVarDecl";
            public const string FunctionDecl = "FunctionDecl";
            public const string FunctionDeclHeader = "FunctionDeclHeader";
            public const string Body = "Body";
            public const string MethodDecl = "MethodDecl";
            public const string MethodDeclHeader = "MethodDeclHeader";
            public const string Receiver = "Receiver";
            public const string BaseTypeName = "BaseTypeName";
            public const string Operand = "Operand";
            public const string Literal = "Literal";
            public const string BasicLit = "BasicLit";
            public const string QualifiedIdent = "QualifiedIdent";
            public const string CompositeLit = "CompositeLit";
            public const string LiteralType = "LiteralType";
            public const string LiteralValue = "LiteralValue";
            public const string ElementList = "ElementList";
            public const string Element = "Element";
            public const string Key = "Key";
            public const string FieldName = "FieldName";
            public const string ElementIndex = "ElementIndex";
            public const string Value = "Value";
            public const string FunctionLit = "FunctionLit";
            public const string PrimaryExpr = "PrimaryExpr";
            public const string Selector = "Selector";
            public const string IndexOrSlice = "IndexOrSlice";
            public const string TypeAssertion = "TypeAssertion";
            public const string Call = "Call";
            public const string Expression = "Expression";
            public const string UnaryExpr = "UnaryExpr";
            public const string BinaryOp = "BinaryOp";
            public const string LogOp = "LogOp";
            public const string ComOp = "ComOp";
            public const string RelOp = "RelOp";
            public const string AddOp = "AddOp";
            public const string MulOp = "MulOp";
            public const string UnaryOp = "UnaryOp";
            public const string MethodExpr = "MethodExpr";
            public const string ReceiverType = "ReceiverType";
            public const string Conversion = "Conversion";
            public const string Statement = "Statement";
            public const string SimpleStmt = "SimpleStmt";
            public const string EmptyStmt = "EmptyStmt";
            public const string LabeledStmt = "LabeledStmt";
            public const string Label = "Label";
            public const string ExpressionStmt = "ExpressionStmt";
            public const string IncDecStmt = "IncDecStmt";
            public const string Assignment = "Assignment";
            public const string AssignOp = "AssignOp";
            public const string IfStmt = "IfStmt";
            public const string SwitchStmt = "SwitchStmt";
            public const string ExprSwitchStmt = "ExprSwitchStmt";
            public const string ExprCaseClause = "ExprCaseClause";
            public const string ExprSwitchCase = "ExprSwitchCase";
            public const string TypeSwitchStmt = "TypeSwitchStmt";
            public const string TypeSwitchGuard = "TypeSwitchGuard";
            public const string TypeCaseClause = "TypeCaseClause";
            public const string TypeSwitchCase = "TypeSwitchCase";
            public const string TypeList = "TypeList";
            public const string ForStmt = "ForStmt";
            public const string Condition = "Condition";
            public const string ForClause = "ForClause";
            public const string InitStmt = "InitStmt";
            public const string PostStmt = "PostStmt";
            public const string RangeClause = "RangeClause";
            public const string GoStmt = "GoStmt";
            public const string SelectStmt = "SelectStmt";
            public const string CommClause = "CommClause";
            public const string CommCase = "CommCase";
            public const string ReturnStmt = "ReturnStmt";
            public const string BreakStmt = "BreakStmt";
            public const string ContinueStmt = "ContinueStmt";
            public const string GotoStmt = "GotoStmt";
            public const string FallthroughStmt = "FallthroughStmt";
            public const string DeferStmt = "DeferStmt";
            public const string CompilationUnit = "CompilationUnit";
            public const string PackageClause = "PackageClause";
            public const string PackageName = "PackageName";
            public const string ImportDecl = "ImportDecl";
            public const string ImportSpec = "ImportSpec";
            public const string SymbolDefinitionIdentifier = "SymbolDefinitionIdentifier";
            public const string SymbolReferenceIdentifier = "SymbolReferenceIdentifier";
        }

        protected class RuleBindings
        {
            public readonly RuleBinding Type = new RuleBinding(RuleNames.Type);
            public readonly RuleBinding TypeNoParens = new RuleBinding(RuleNames.TypeNoParens);
            public readonly RuleBinding TypeName = new RuleBinding(RuleNames.TypeName);
            public readonly RuleBinding TypeLit = new RuleBinding(RuleNames.TypeLit);
            public readonly RuleBinding ArrayType = new RuleBinding(RuleNames.ArrayType);
            public readonly RuleBinding ArrayLength = new RuleBinding(RuleNames.ArrayLength);
            public readonly RuleBinding ElementType = new RuleBinding(RuleNames.ElementType);
            public readonly RuleBinding SliceType = new RuleBinding(RuleNames.SliceType);
            public readonly RuleBinding StructType = new RuleBinding(RuleNames.StructType);
            public readonly RuleBinding FieldDecl = new RuleBinding(RuleNames.FieldDecl);
            public readonly RuleBinding FieldIdentifierList = new RuleBinding(RuleNames.FieldIdentifierList);
            public readonly RuleBinding AnonymousField = new RuleBinding(RuleNames.AnonymousField);
            public readonly RuleBinding Tag = new RuleBinding(RuleNames.Tag);
            public readonly RuleBinding PointerType = new RuleBinding(RuleNames.PointerType);
            public readonly RuleBinding BaseType = new RuleBinding(RuleNames.BaseType);
            public readonly RuleBinding FunctionType = new RuleBinding(RuleNames.FunctionType);
            public readonly RuleBinding Signature = new RuleBinding(RuleNames.Signature);
            public readonly RuleBinding Parameters = new RuleBinding(RuleNames.Parameters);
            public readonly RuleBinding ParameterList = new RuleBinding(RuleNames.ParameterList);
            public readonly RuleBinding ParameterDecl = new RuleBinding(RuleNames.ParameterDecl);
            public readonly RuleBinding ParameterType = new RuleBinding(RuleNames.ParameterType);
            public readonly RuleBinding InterfaceType = new RuleBinding(RuleNames.InterfaceType);
            public readonly RuleBinding MethodSpec = new RuleBinding(RuleNames.MethodSpec);
            public readonly RuleBinding MethodName = new RuleBinding(RuleNames.MethodName);
            public readonly RuleBinding InterfaceTypeName = new RuleBinding(RuleNames.InterfaceTypeName);
            public readonly RuleBinding MapType = new RuleBinding(RuleNames.MapType);
            public readonly RuleBinding KeyType = new RuleBinding(RuleNames.KeyType);
            public readonly RuleBinding ChannelType = new RuleBinding(RuleNames.ChannelType);
            public readonly RuleBinding Channel = new RuleBinding(RuleNames.Channel);
            public readonly RuleBinding SendChannel = new RuleBinding(RuleNames.SendChannel);
            public readonly RuleBinding RecvChannel = new RuleBinding(RuleNames.RecvChannel);
            public readonly RuleBinding Block = new RuleBinding(RuleNames.Block);
            public readonly RuleBinding Declaration = new RuleBinding(RuleNames.Declaration);
            public readonly RuleBinding TopLevelDecl = new RuleBinding(RuleNames.TopLevelDecl);
            public readonly RuleBinding ConstDecl = new RuleBinding(RuleNames.ConstDecl);
            public readonly RuleBinding ConstSpec = new RuleBinding(RuleNames.ConstSpec);
            public readonly RuleBinding IdentifierList = new RuleBinding(RuleNames.IdentifierList);
            public readonly RuleBinding ExpressionList = new RuleBinding(RuleNames.ExpressionList);
            public readonly RuleBinding TypeDecl = new RuleBinding(RuleNames.TypeDecl);
            public readonly RuleBinding TypeSpec = new RuleBinding(RuleNames.TypeSpec);
            public readonly RuleBinding VarDecl = new RuleBinding(RuleNames.VarDecl);
            public readonly RuleBinding VarSpec = new RuleBinding(RuleNames.VarSpec);
            public readonly RuleBinding ShortVarDecl = new RuleBinding(RuleNames.ShortVarDecl);
            public readonly RuleBinding FunctionDecl = new RuleBinding(RuleNames.FunctionDecl);
            public readonly RuleBinding FunctionDeclHeader = new RuleBinding(RuleNames.FunctionDeclHeader);
            public readonly RuleBinding Body = new RuleBinding(RuleNames.Body);
            public readonly RuleBinding MethodDecl = new RuleBinding(RuleNames.MethodDecl);
            public readonly RuleBinding MethodDeclHeader = new RuleBinding(RuleNames.MethodDeclHeader);
            public readonly RuleBinding Receiver = new RuleBinding(RuleNames.Receiver);
            public readonly RuleBinding BaseTypeName = new RuleBinding(RuleNames.BaseTypeName);
            public readonly RuleBinding Operand = new RuleBinding(RuleNames.Operand);
            public readonly RuleBinding Literal = new RuleBinding(RuleNames.Literal);
            public readonly RuleBinding BasicLit = new RuleBinding(RuleNames.BasicLit);
            public readonly RuleBinding QualifiedIdent = new RuleBinding(RuleNames.QualifiedIdent);
            public readonly RuleBinding CompositeLit = new RuleBinding(RuleNames.CompositeLit);
            public readonly RuleBinding LiteralType = new RuleBinding(RuleNames.LiteralType);
            public readonly RuleBinding LiteralValue = new RuleBinding(RuleNames.LiteralValue);
            public readonly RuleBinding ElementList = new RuleBinding(RuleNames.ElementList);
            public readonly RuleBinding Element = new RuleBinding(RuleNames.Element);
            public readonly RuleBinding Key = new RuleBinding(RuleNames.Key);
            public readonly RuleBinding FieldName = new RuleBinding(RuleNames.FieldName);
            public readonly RuleBinding ElementIndex = new RuleBinding(RuleNames.ElementIndex);
            public readonly RuleBinding Value = new RuleBinding(RuleNames.Value);
            public readonly RuleBinding FunctionLit = new RuleBinding(RuleNames.FunctionLit);
            public readonly RuleBinding PrimaryExpr = new RuleBinding(RuleNames.PrimaryExpr);
            public readonly RuleBinding Selector = new RuleBinding(RuleNames.Selector);
            public readonly RuleBinding IndexOrSlice = new RuleBinding(RuleNames.IndexOrSlice);
            public readonly RuleBinding TypeAssertion = new RuleBinding(RuleNames.TypeAssertion);
            public readonly RuleBinding Call = new RuleBinding(RuleNames.Call);
            public readonly RuleBinding Expression = new RuleBinding(RuleNames.Expression);
            public readonly RuleBinding UnaryExpr = new RuleBinding(RuleNames.UnaryExpr);
            public readonly RuleBinding BinaryOp = new RuleBinding(RuleNames.BinaryOp);
            public readonly RuleBinding LogOp = new RuleBinding(RuleNames.LogOp);
            public readonly RuleBinding ComOp = new RuleBinding(RuleNames.ComOp);
            public readonly RuleBinding RelOp = new RuleBinding(RuleNames.RelOp);
            public readonly RuleBinding AddOp = new RuleBinding(RuleNames.AddOp);
            public readonly RuleBinding MulOp = new RuleBinding(RuleNames.MulOp);
            public readonly RuleBinding UnaryOp = new RuleBinding(RuleNames.UnaryOp);
            public readonly RuleBinding MethodExpr = new RuleBinding(RuleNames.MethodExpr);
            public readonly RuleBinding ReceiverType = new RuleBinding(RuleNames.ReceiverType);
            public readonly RuleBinding Conversion = new RuleBinding(RuleNames.Conversion);
            public readonly RuleBinding Statement = new RuleBinding(RuleNames.Statement);
            public readonly RuleBinding SimpleStmt = new RuleBinding(RuleNames.SimpleStmt);
            public readonly RuleBinding EmptyStmt = new RuleBinding(RuleNames.EmptyStmt);
            public readonly RuleBinding LabeledStmt = new RuleBinding(RuleNames.LabeledStmt);
            public readonly RuleBinding Label = new RuleBinding(RuleNames.Label);
            public readonly RuleBinding ExpressionStmt = new RuleBinding(RuleNames.ExpressionStmt);
            public readonly RuleBinding IncDecStmt = new RuleBinding(RuleNames.IncDecStmt);
            public readonly RuleBinding Assignment = new RuleBinding(RuleNames.Assignment);
            public readonly RuleBinding AssignOp = new RuleBinding(RuleNames.AssignOp);
            public readonly RuleBinding IfStmt = new RuleBinding(RuleNames.IfStmt);
            public readonly RuleBinding SwitchStmt = new RuleBinding(RuleNames.SwitchStmt);
            public readonly RuleBinding ExprSwitchStmt = new RuleBinding(RuleNames.ExprSwitchStmt);
            public readonly RuleBinding ExprCaseClause = new RuleBinding(RuleNames.ExprCaseClause);
            public readonly RuleBinding ExprSwitchCase = new RuleBinding(RuleNames.ExprSwitchCase);
            public readonly RuleBinding TypeSwitchStmt = new RuleBinding(RuleNames.TypeSwitchStmt);
            public readonly RuleBinding TypeSwitchGuard = new RuleBinding(RuleNames.TypeSwitchGuard);
            public readonly RuleBinding TypeCaseClause = new RuleBinding(RuleNames.TypeCaseClause);
            public readonly RuleBinding TypeSwitchCase = new RuleBinding(RuleNames.TypeSwitchCase);
            public readonly RuleBinding TypeList = new RuleBinding(RuleNames.TypeList);
            public readonly RuleBinding ForStmt = new RuleBinding(RuleNames.ForStmt);
            public readonly RuleBinding Condition = new RuleBinding(RuleNames.Condition);
            public readonly RuleBinding ForClause = new RuleBinding(RuleNames.ForClause);
            public readonly RuleBinding InitStmt = new RuleBinding(RuleNames.InitStmt);
            public readonly RuleBinding PostStmt = new RuleBinding(RuleNames.PostStmt);
            public readonly RuleBinding RangeClause = new RuleBinding(RuleNames.RangeClause);
            public readonly RuleBinding GoStmt = new RuleBinding(RuleNames.GoStmt);
            public readonly RuleBinding SelectStmt = new RuleBinding(RuleNames.SelectStmt);
            public readonly RuleBinding CommClause = new RuleBinding(RuleNames.CommClause);
            public readonly RuleBinding CommCase = new RuleBinding(RuleNames.CommCase);
            public readonly RuleBinding ReturnStmt = new RuleBinding(RuleNames.ReturnStmt);
            public readonly RuleBinding BreakStmt = new RuleBinding(RuleNames.BreakStmt);
            public readonly RuleBinding ContinueStmt = new RuleBinding(RuleNames.ContinueStmt);
            public readonly RuleBinding GotoStmt = new RuleBinding(RuleNames.GotoStmt);
            public readonly RuleBinding FallthroughStmt = new RuleBinding(RuleNames.FallthroughStmt);
            public readonly RuleBinding DeferStmt = new RuleBinding(RuleNames.DeferStmt);
            public readonly RuleBinding CompilationUnit = new RuleBinding(RuleNames.CompilationUnit);
            public readonly RuleBinding PackageClause = new RuleBinding(RuleNames.PackageClause);
            public readonly RuleBinding PackageName = new RuleBinding(RuleNames.PackageName);
            public readonly RuleBinding ImportDecl = new RuleBinding(RuleNames.ImportDecl);
            public readonly RuleBinding ImportSpec = new RuleBinding(RuleNames.ImportSpec);
            public readonly RuleBinding SymbolDefinitionIdentifier = new RuleBinding(RuleNames.SymbolDefinitionIdentifier);
            public readonly RuleBinding SymbolReferenceIdentifier = new RuleBinding(RuleNames.SymbolReferenceIdentifier);
        }
    }
}
