namespace Tvl.VisualStudio.Language.AntlrV4
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Misc;
    using Antlr4.Runtime.Tree;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Tvl.VisualStudio.OutputWindow.Interfaces;

    internal abstract class SmartIndent : ISmartIndent
    {
        private readonly IOutputWindowPane _diagnosticsPane;

        protected SmartIndent()
            : this(null)
        {
        }

        protected SmartIndent(IOutputWindowPane diagnosticsPane)
        {
            _diagnosticsPane = diagnosticsPane;
        }

        protected abstract vsIndentStyle IndentStyle
        {
            get;
        }

        protected abstract int TabSize
        {
            get;
        }

        protected virtual IOutputWindowPane DiagnosticsPane
        {
            get
            {
                return _diagnosticsPane;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        public virtual int? GetDesiredIndentation(ITextSnapshotLine line)
        {
            try
            {
                vsIndentStyle indentStyle = IndentStyle;
                if (indentStyle == vsIndentStyle.vsIndentStyleNone)
                    return 0;

                int? result = null;

                if (indentStyle == vsIndentStyle.vsIndentStyleSmart)
                    result = GetSmartIndentation(line);

                if (result == null)
                    result = GetFallbackIndentation(line);

                return result;
            }
            catch (Exception ex) when (!ErrorHandler.IsCriticalException(ex))
            {
                // Throwing an exception from here will crash the IDE.
                return null;
            }
        }

        protected virtual int? GetSmartIndentation(ITextSnapshotLine line)
        {
            ITextSnapshot snapshot = line.Snapshot;
            SnapshotPoint contextEndPosition = line.Start;
            SnapshotPoint endPosition = line.EndIncludingLineBreak;
            SnapshotPoint endPositionOnLine = line.End;

            IReferenceAnchors anchors = FindNearestAnchors(contextEndPosition);
            IAnchor previous = anchors.Previous;

            int spanEnd = Math.Min(line.Snapshot.Length, endPosition.Position + 1);
            Span span;
            IAnchor enclosing = anchors.Enclosing;
            if (enclosing != null)
            {
                span = Span.FromBounds(enclosing.TrackingSpan.GetStartPoint(snapshot).Position, spanEnd);
            }
            else if (previous != null)
            {
                // at least for now, include the previous span due to the way error handling places bounds on an anchor
                span = Span.FromBounds(previous.TrackingSpan.GetStartPoint(snapshot).Position, spanEnd);
            }
            else
            {
                span = Span.FromBounds(0, spanEnd);
            }

            var diagnosticsPane = DiagnosticsPane;
            if (diagnosticsPane != null)
                diagnosticsPane.WriteLine(string.Format("Smart indent from anchor span: {0}", span));

            ITokenSource bufferTokenSource = GetTokenSource(new SnapshotSpan(snapshot, span));
            ITokenSource tokenSource = new CodeCompletionTokenSource(bufferTokenSource, endPosition);
            ITokenStream tokenStream = new CommonTokenStream(tokenSource);

            IDictionary<RuleContext, CaretReachedException> parseTrees = GetParseTrees(tokenStream, anchors);
            if (parseTrees == null)
                return null;

            var indentLevels = new SortedDictionary<int, IList<KeyValuePair<RuleContext, CaretReachedException>>>();
            foreach (var parseTree in parseTrees)
            {
                if (parseTree.Value == null)
                    continue;

                IParseTree firstNodeOnLine = FindFirstNodeAfterOffset(parseTree.Key, line.Start.Position);
                if (firstNodeOnLine == null)
                    firstNodeOnLine = parseTree.Value.FinalContext;

                if (firstNodeOnLine == null)
                    continue;

                int? indentationLevel = GetIndent(parseTree, firstNodeOnLine, line.Start);
                if (indentationLevel == null)
                    continue;

                IList<KeyValuePair<RuleContext, CaretReachedException>> indentList;
                if (!indentLevels.TryGetValue(indentationLevel.Value, out indentList))
                {
                    indentList = new List<KeyValuePair<RuleContext, CaretReachedException>>();
                    indentLevels[indentationLevel.Value] = indentList;
                }

                indentList.Add(parseTree);
            }

            if (indentLevels.Count == 0)
                return null;

            int indentLevel = indentLevels.First().Key;
            if (indentLevels.Count > 1)
            {
                // TODO: resolve multiple possibilities
            }

            return indentLevel;
        }

        protected virtual IReferenceAnchors FindNearestAnchors(SnapshotPoint point)
        {
            ITextSnapshot snapshot = point.Snapshot;
            IList<IAnchor> anchors = GetDynamicAnchorPoints(snapshot);

            // the innermost anchor enclosing the caret
            IAnchor enclosing = null;
            // the last anchor starting before the caret
            IAnchor previous = null;

            if (anchors != null)
            {
                IAnchor next = null;

                // parse the current rule
                foreach (IAnchor anchor in anchors)
                {
                    if (anchor.TrackingSpan.GetStartPoint(snapshot) <= point)
                    {
                        previous = anchor;
                        if (anchor.TrackingSpan.GetEndPoint(snapshot) > point)
                            enclosing = anchor;
                    }
                    else
                    {
                        next = anchor;
                        break;
                    }
                }
            }

            return new ReferenceAnchors(previous, enclosing);
        }

        protected virtual ITerminalNode FindFirstNodeAfterOffset(IParseTree parseTree, int offset)
        {
            ITerminalNode lastNode = ParseTrees.GetStopNode(parseTree);
            if (lastNode == null)
                return null;

            ICaretToken caretToken = lastNode.Symbol as ICaretToken;
            if (caretToken != null)
                throw new NotImplementedException();
            else if (lastNode.Symbol.StartIndex < offset)
                return null;

            lastNode = parseTree as ITerminalNode;
            if (lastNode != null)
                return lastNode;

            for (int i = 0; i < parseTree.ChildCount; i++)
            {
                ITerminalNode node = FindFirstNodeAfterOffset(parseTree.GetChild(i), offset);
                if (node != null)
                    return node;
            }

            return null;
        }

        protected abstract ITokenSource GetTokenSource(SnapshotSpan snapshotSpan);

        protected abstract IDictionary<RuleContext, CaretReachedException> GetParseTrees(ITokenStream tokens, IReferenceAnchors referenceAnchors);

        protected abstract IList<IAnchor> GetDynamicAnchorPoints(ITextSnapshot snapshot);

        protected abstract AlignmentRequirements GetAlignmentRequirement(KeyValuePair<RuleContext, CaretReachedException> parseTree, IParseTree targetElement, IParseTree ancestor);

        protected abstract Tuple<IParseTree, int> GetAlignmentElement(KeyValuePair<RuleContext, CaretReachedException> parseTree, IParseTree targetElement, IParseTree container, IList<IParseTree> priorSiblings);

        protected virtual int? GetIndent(KeyValuePair<RuleContext, CaretReachedException> parseTree, IParseTree firstNodeOnLine, SnapshotPoint lineStart)
        {
            for (IParseTree ancestor = firstNodeOnLine; ancestor != null; ancestor = ancestor.Parent)
            {
                AlignmentRequirements requirements = GetAlignmentRequirement(parseTree, firstNodeOnLine, ancestor);
                if ((requirements & AlignmentRequirements.UseAncestor) != 0)
                    continue;

                if ((requirements & AlignmentRequirements.IgnoreTree) != 0)
                    return null;

                IList<IParseTree> siblings = null;
                if ((requirements & AlignmentRequirements.PriorSibling) != 0)
                {
                    int childCount = ancestor.ChildCount;
                    siblings = new List<IParseTree>(childCount);
                    for (int i = 0; i < childCount; i++)
                    {
                        IParseTree child = ancestor.GetChild(i);
                        siblings.Add(child);
                        if (ParseTrees.IsAncestorOf(child, firstNodeOnLine))
                            break;
                    }
                }

                Tuple<IParseTree, int> alignmentElement = GetAlignmentElement(parseTree, firstNodeOnLine, ancestor, siblings);
                if (alignmentElement == null)
                    continue;

                IToken startToken = ParseTrees.GetStartSymbol(alignmentElement.Item1);
                string beginningOfLineText = startToken.TokenSource.InputStream.GetText(new Interval(startToken.StartIndex - startToken.Column, startToken.StartIndex - 1));
                int elementIndent = 0;
                for (int i = 0; i < beginningOfLineText.Length; i++)
                {
                    if (beginningOfLineText[i] == '\t')
                    {
                        elementIndent += TabSize;
                        elementIndent -= (elementIndent % TabSize);
                    }
                    else
                    {
                        elementIndent++;
                    }
                }

                var diagnosticsPane = DiagnosticsPane;
                if (diagnosticsPane != null)
                {
                    if (ParseTrees.GetStartSymbol(firstNodeOnLine) == startToken)
                        diagnosticsPane.WriteLine("Attempting to indent a line relative to an element on that line.");

                    diagnosticsPane.WriteLine(string.Format("Indent {0} relative to {1} (offset {2}) => {3}", firstNodeOnLine, alignmentElement.Item1, alignmentElement.Item2, elementIndent + alignmentElement.Item2));
                }

                return elementIndent + alignmentElement.Item2;
            }

            return null;
        }

        protected virtual int? GetFallbackIndentation(ITextSnapshotLine line)
        {
            ITextSnapshot snapshot = line.Snapshot;
            int tabSize = TabSize;

            // indent to the same level as the preceding non-empty line
            for (int lineNumber = line.LineNumber - 1; lineNumber >= 0; lineNumber--)
            {
                ITextSnapshotLine priorLine = snapshot.GetLineFromLineNumber(lineNumber);
                string text = priorLine.GetText();
                if (string.IsNullOrWhiteSpace(text))
                    continue;

                int indent = 0;
                foreach (char c in text)
                {
                    if (c == ' ')
                    {
                        indent++;
                    }
                    else if (c == '\t')
                    {
                        indent += tabSize - (indent % tabSize);
                    }
                    else
                    {
                        break;
                    }
                }

                return indent;
            }

            return null;
        }
    }
}
