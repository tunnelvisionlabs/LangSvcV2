namespace Tvl.VisualStudio.Language.Go.Experimental
{
    using Tvl.VisualStudio.Language.Parsing.Collections;
    using Tvl.VisualStudio.Language.Parsing.Experimental.Atn;

    internal class GoReducedAtnBuilder : GoSimplifiedAtnBuilder
    {
        protected override Nfa BuildArrayTypeRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.LBRACK),
                Nfa.Rule(Bindings.Expression),
                Nfa.Match(GoLexer.RBRACK),
                Nfa.Rule(Bindings.Type));
        }

        protected override Nfa BuildSliceTypeRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.LBRACK),
                Nfa.Match(GoLexer.RBRACK),
                Nfa.Rule(Bindings.Type));
        }

        protected override Nfa BuildMapTypeRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.KW_MAP),
                Nfa.Match(GoLexer.LBRACK),
                Nfa.Rule(Bindings.Type),
                Nfa.Match(GoLexer.RBRACK),
                Nfa.Rule(Bindings.Type));
        }

        protected override Nfa BuildChannelTypeRule()
        {
            return Nfa.Choice(
                Nfa.Sequence(
                    Nfa.Match(GoLexer.KW_CHAN),
                    Nfa.Optional(Nfa.Match(GoLexer.CHAN)),
                    Nfa.Rule(Bindings.Type)),
                Nfa.Sequence(
                    Nfa.Match(GoLexer.CHAN),
                    Nfa.Match(GoLexer.KW_CHAN),
                    Nfa.Rule(Bindings.Type)));
        }

        protected override Nfa BuildLiteralTypeRule()
        {
            return Nfa.Choice(
                Nfa.Rule(Bindings.StructType),
                Nfa.Rule(Bindings.ArrayType),
                Nfa.Sequence(
                    Nfa.Match(GoLexer.LBRACK),
                    Nfa.Match(GoLexer.ELLIP),
                    Nfa.Match(GoLexer.RBRACK),
                    Nfa.Rule(Bindings.Type)),
                Nfa.Rule(Bindings.SliceType),
                Nfa.Rule(Bindings.MapType),
                Nfa.Rule(Bindings.QualifiedIdent),
                Nfa.Sequence(
                    Nfa.Match(GoLexer.LPAREN),
                    Nfa.Rule(Bindings.LiteralType),
                    Nfa.Match(GoLexer.RPAREN)));
        }

        protected override Nfa BuildReceiverTypeRule()
        {
            return Nfa.Choice(
                Nfa.Rule(Bindings.QualifiedIdent),
                Nfa.Sequence(
                    Nfa.Match(GoLexer.LPAREN),
                    Nfa.Match(GoLexer.TIMES),
                    Nfa.Rule(Bindings.QualifiedIdent),
                    Nfa.Match(GoLexer.RPAREN)));
        }

        protected override Nfa BuildTypeNoParensRule()
        {
            return Nfa.Choice(
                Nfa.Rule(Bindings.QualifiedIdent),
                Nfa.Rule(Bindings.TypeLit));
        }

        protected override Nfa BuildForStmtRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.KW_FOR),
                Nfa.Optional(
                    Nfa.Choice(
                        Nfa.Rule(Bindings.RangeClause),
                        Nfa.Rule(Bindings.ForClause),
                        Nfa.Rule(Bindings.Expression))),
                Nfa.Rule(Bindings.Block));
        }

        protected override Nfa BuildForClauseRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.SimpleStmt),
                Nfa.Match(GoLexer.SEMI),
                Nfa.Optional(Nfa.Rule(Bindings.Expression)),
                Nfa.Match(GoLexer.SEMI),
                Nfa.Rule(Bindings.SimpleStmt));
        }

        protected override Nfa BuildMethodSpecRule()
        {
            return Nfa.Choice(
                Nfa.Sequence(
                    Nfa.Rule(Bindings.MethodName),
                    Nfa.Rule(Bindings.Signature)),
                Nfa.Rule(Bindings.QualifiedIdent));
        }

        protected override Nfa BuildFunctionDeclRule()
        {
            // inline the body rule
            return Nfa.Sequence(
                Nfa.Rule(Bindings.FunctionDeclHeader),
                Nfa.Optional(Nfa.Rule(Bindings.Block)));
        }

        protected override Nfa BuildFunctionLitRule()
        {
            // inline the body rule
            return Nfa.Sequence(
                Nfa.Rule(Bindings.FunctionType),
                Nfa.Rule(Bindings.Block));
        }

        protected override Nfa BuildAnonymousFieldRule()
        {
            return Nfa.Sequence(
                Nfa.Optional(Nfa.Match(GoLexer.TIMES)),
                Nfa.Rule(Bindings.QualifiedIdent));
        }

        protected override Nfa BuildElementRule()
        {
            return Nfa.Sequence(
                Nfa.Optional(
                    Nfa.Sequence(
                        Nfa.Rule(Bindings.Expression),
                        Nfa.Match(GoLexer.COLON))),
                Nfa.Rule(Bindings.Value));
        }

        protected override Nfa BuildMethodDeclRule()
        {
            // inline the body rule
            return Nfa.Sequence(
                Nfa.Rule(Bindings.MethodDeclHeader),
                Nfa.Optional(Nfa.Rule(Bindings.Block)));
        }

        protected override Nfa BuildPointerTypeRule()
        {
            // inline the baseType rule
            return Nfa.Sequence(
                Nfa.Match(GoLexer.TIMES),
                Nfa.Rule(Bindings.Type));
        }

        protected override Nfa BuildReceiverRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.LPAREN),
                Nfa.Optional(Nfa.Rule(Bindings.SymbolDefinitionIdentifier)),
                Nfa.Optional(Nfa.Match(GoLexer.TIMES)),
                // inline the baseTypeName rule
                Nfa.Rule(Bindings.SymbolReferenceIdentifier),
                Nfa.Match(GoLexer.RPAREN));
        }

        protected override Nfa BuildTypeRule()
        {
            // factor out the typeNoParens rule
            return Nfa.Choice(
                Nfa.Rule(Bindings.TypeNoParens),
                Nfa.Sequence(
                    Nfa.Match(GoLexer.LPAREN),
                    Nfa.Rule(Bindings.Type),
                    Nfa.Match(GoLexer.RPAREN)));
        }

        protected override Nfa BuildLabeledStmtRule()
        {
            return Nfa.Sequence(
                Nfa.Rule(Bindings.SymbolDefinitionIdentifier),
                Nfa.Match(GoLexer.COLON),
                Nfa.Rule(Bindings.Statement));
        }

        protected override Nfa BuildBreakStmtRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.KW_BREAK),
                Nfa.Optional(Nfa.Rule(Bindings.SymbolReferenceIdentifier)));
        }

        protected override Nfa BuildContinueStmtRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.KW_CONTINUE),
                Nfa.Optional(Nfa.Rule(Bindings.SymbolReferenceIdentifier)));
        }

        protected override Nfa BuildGotoStmtRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.KW_GOTO),
                Nfa.Rule(Bindings.SymbolReferenceIdentifier));
        }

        protected override Nfa BuildPackageClauseRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.KW_PACKAGE),
                Nfa.Rule(Bindings.SymbolDefinitionIdentifier));
        }

        protected override Nfa BuildSimpleStmtRule()
        {
            return Nfa.Choice(
                Nfa.Rule(Bindings.ShortVarDecl),
                Nfa.Rule(Bindings.Assignment),
                Nfa.Rule(Bindings.IncDecStmt),
                Nfa.Rule(Bindings.Expression),
                Nfa.Rule(Bindings.EmptyStmt));
        }

        protected override Nfa BuildElementIndexRule()
        {
            return null;
        }

        protected override Nfa BuildBodyRule()
        {
            return null;
        }

        protected override Nfa BuildBaseTypeRule()
        {
            return null;
        }

        protected override Nfa BuildBaseTypeNameRule()
        {
            return null;
        }

        protected override Nfa BuildTypeNameRule()
        {
            return null;
        }

        protected override Nfa BuildArrayLengthRule()
        {
            return null;
        }

        protected override Nfa BuildElementTypeRule()
        {
            return null;
        }

        protected override Nfa BuildInterfaceTypeNameRule()
        {
            return null;
        }

        protected override Nfa BuildKeyTypeRule()
        {
            return null;
        }

        protected override Nfa BuildChannelRule()
        {
            return null;
        }

        protected override Nfa BuildSendChannelRule()
        {
            return null;
        }

        protected override Nfa BuildRecvChannelRule()
        {
            return null;
        }

        protected override Nfa BuildFieldNameRule()
        {
            return null;
        }

        protected override Nfa BuildKeyRule()
        {
            return null;
        }

        protected override Nfa BuildLabelRule()
        {
            return null;
        }

        protected override Nfa BuildExpressionStmtRule()
        {
            return null;
        }

        protected override Nfa BuildConditionRule()
        {
            return null;
        }

        protected override Nfa BuildInitStmtRule()
        {
            return null;
        }

        protected override Nfa BuildPostStmtRule()
        {
            return null;
        }

        protected override Nfa BuildPackageNameRule()
        {
            return null;
        }
    }
}
