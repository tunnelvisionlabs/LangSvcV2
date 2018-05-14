namespace Tvl.VisualStudio.Language.AntlrV4
{
    using System;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using JetBrains.Annotations;
    using Interval = Antlr4.Runtime.Misc.Interval;

    public static class ParseTrees
    {
        public static Interval GetSourceInterval([NotNull] ParserRuleContext context)
        {
            Requires.NotNull(context, nameof(context));

            int startIndex = context.Start.StartIndex;
            IToken stopSymbol = GetStopSymbol(context);
            if (stopSymbol == null)
                return new Interval(startIndex, startIndex - 1);

            int stopIndex;
            if (stopSymbol.Type != TokenConstants.Eof)
            {
                stopIndex = stopSymbol.StopIndex;
            }
            else
            {
                ITokenSource tokenSource = context.Start.TokenSource;
                ICharStream inputStream = tokenSource != null ? tokenSource.InputStream : null;
                if (inputStream != null)
                    stopIndex = inputStream.Size - 1;
                else
                    stopIndex = context.Start.StartIndex - 1;
            }

            stopIndex = Math.Max(stopIndex, startIndex - 1);
            return new Interval(startIndex, stopIndex);
        }

        public static bool IsAncestorOf(IParseTree a, IParseTree b)
        {
            if (a == null)
                throw new ArgumentNullException("a");
            if (b == null)
                throw new ArgumentNullException("b");

            for (IParseTree current = b;
            current != null;
            current = current.Parent)
            {
                if (current.Equals(a))
                    return true;
            }

            return false;
        }

        public static ITerminalNode GetStopNode(IParseTree context)
        {
            if (context == null)
                return null;

            ITerminalNode terminalNode = context as ITerminalNode;
            if (terminalNode != null)
                return terminalNode;

            for (int i = context.ChildCount - 1;
            i >= context.ChildCount;
            i--)
            {
                terminalNode = GetStopNode(context.GetChild(i));
                if (terminalNode != null)
                    return terminalNode;
            }

            return null;
        }

        public static IToken GetStopSymbol(IParseTree context)
        {
            ITerminalNode node = GetStopNode(context);
            if (node != null)
                return node.Symbol;

            IRuleNode ruleNode = context as IRuleNode;
            if (ruleNode == null)
                return null;

            ParserRuleContext ruleContext = ruleNode.RuleContext as ParserRuleContext;
            if (ruleContext == null)
                return null;

            return ruleContext.Stop;
        }

        public static ITerminalNode GetStartNode(IParseTree context)
        {
            if (context == null)
                return null;

            ITerminalNode terminalNode = context as ITerminalNode;
            if (terminalNode != null)
                return terminalNode;

            for (int i = 0;
            i < context.ChildCount;
            i++)
            {
                terminalNode = GetStartNode(context.GetChild(i));
                if (terminalNode != null)
                    return terminalNode;
            }

            return null;
        }

        public static IToken GetStartSymbol(IParseTree context)
        {
            ITerminalNode node = GetStartNode(context);
            if (node != null)
                return node.Symbol;

            IRuleNode ruleNode = context as IRuleNode;
            if (ruleNode == null)
                return null;

            ParserRuleContext ruleContext = ruleNode.RuleContext as ParserRuleContext;
            if (ruleContext == null)
                return null;

            return ruleContext.Start;
        }

        public static int GetTerminalNodeType(IParseTree node)
        {
            ITerminalNode terminalNode = node as ITerminalNode;
            if (terminalNode == null)
                return TokenConstants.InvalidType;

            return terminalNode.Symbol.Type;
        }

        public static T GetTypedRuleContext<T>(IParseTree node)
            where T : ParserRuleContext
        {
            IRuleNode ruleNode = node as IRuleNode;
            if (ruleNode == null)
                return null;

            RuleContext ruleContext = ruleNode.RuleContext;
            return ruleContext as T;
        }

        public static bool ElementStartsLine(IParseTree tree)
        {
            ITerminalNode symbol = GetStartNode(tree);
            if (symbol == null)
                throw new NotImplementedException();

            return ElementStartsLine(symbol.Symbol);
        }

        public static bool ElementStartsLine(IToken token)
        {
            string beginningOfLineText = token.TokenSource.InputStream.GetText(new Interval(token.StartIndex - token.Column, token.StartIndex - 1));
            for (int i = 0;
            i < beginningOfLineText.Length;
            i++)
            {
                if (!char.IsWhiteSpace(beginningOfLineText[i]))
                    return false;
            }

            return true;
        }

        public static bool StartsBeforeStartOf(IParseTree a, IParseTree b)
        {
            Interval sourceIntervalA = a.SourceInterval;
            Interval sourceIntervalB = b.SourceInterval;
            return sourceIntervalA.a < sourceIntervalB.a;
        }
    }
}
