namespace Tvl.VisualStudio.Tools.AstExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Tvl.VisualStudio.Language.Parsing4;
    using Tvl.VisualStudio.Text.Tagging;
    using ParseResultEventArgs = Tvl.VisualStudio.Language.Parsing.ParseResultEventArgs;

    partial class AstExplorerControl
    {
        private IList<IToken> Tokens4
        {
            get;
            set;
        }

        private void HandleTreeViewSelectedItemChangedV4(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                if (Tagger == null)
                    return;

                if (Tokens3 != null || e.NewValue is Antlr.Runtime.Tree.ITree)
                    return;

                using (Tagger.Update())
                {
                    Tagger.RemoveTagSpans(tagSpan => true);

                    if (Snapshot == null)
                        return;

                    IParseTree selected = e.NewValue as IParseTree;

                    IList<IToken> tokens = Tokens4;
                    if (tokens != null && selected != null)
                    {
                        if (selected.SourceInterval.a >= 0 && selected.SourceInterval.b >= 0)
                        {
                            IToken startToken = tokens[selected.SourceInterval.a];
                            IToken stopToken = tokens[selected.SourceInterval.b];
                            Span span = new Span(startToken.StartIndex, stopToken.StopIndex - startToken.StartIndex + 1);
                            Span sourceSpan = new Span(0, Snapshot.Length);
                            if (sourceSpan.Contains(span))
                            {
                                ITrackingSpan trackingSpan = Snapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgeExclusive);
                                Tagger.CreateTagSpan(trackingSpan, PredefinedTextMarkerTags.Vivid);
                                var activeView = ActiveViewTrackerService.ActiveView;
                                if (activeView != null && activeView.TextBuffer == Snapshot.TextBuffer)
                                    activeView.ViewScroller.EnsureSpanVisible(new SnapshotSpan(Snapshot, span), EnsureSpanVisibleOptions.ShowStart);
                            }
                        }
                    }
                    else if (selected is ITerminalNode)
                    {
                        IToken token = ((ITerminalNode)selected).Symbol;
                        if (token.StartIndex >= 0 && token.StopIndex >= 0)
                        {
                            Span span = new Span(token.StartIndex, token.StopIndex - token.StartIndex + 1);
                            Span sourceSpan = new Span(0, Snapshot.Length);
                            if (sourceSpan.Contains(span))
                            {
                                ITrackingSpan trackingSpan = Snapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgeExclusive);
                                Tagger.CreateTagSpan(trackingSpan, PredefinedTextMarkerTags.Vivid);
                                var activeView = ActiveViewTrackerService.ActiveView;
                                if (activeView != null && activeView.TextBuffer == Snapshot.TextBuffer)
                                    activeView.ViewScroller.EnsureSpanVisible(new SnapshotSpan(Snapshot, span), EnsureSpanVisibleOptions.ShowStart);
                            }
                        }
                    }
                    else if (selected is IRuleNode)
                    {
                        //if (selected.Token.StartIndex >= 0 && selected.Token.StopIndex >= 0)
                        //{
                        //    IToken token = selected.Token;
                        //    Span span = new Span(token.StartIndex, token.StopIndex - token.StartIndex + 1);
                        //    Span sourceSpan = new Span(0, Snapshot.Length);
                        //    if (sourceSpan.Contains(span))
                        //    {
                        //        ITrackingSpan trackingSpan = Snapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgeExclusive);
                        //        Tagger.CreateTagSpan(trackingSpan, PredefinedTextMarkerTags.Vivid);
                        //        var activeView = ActiveViewTrackerService.ActiveView;
                        //        if (activeView != null && activeView.TextBuffer == Snapshot.TextBuffer)
                        //            activeView.ViewScroller.EnsureSpanVisible(new SnapshotSpan(Snapshot, span), EnsureSpanVisibleOptions.ShowStart);
                        //    }
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                if (ErrorHandler.IsCriticalException(ex))
                    throw;
            }
        }

        private bool TryHandleParseCompleteV4(object sender, ParseResultEventArgs e)
        {
            if (!object.ReferenceEquals(sender, BackgroundParser))
                return false;

            AntlrParseResultEventArgs antlrArgs = e as AntlrParseResultEventArgs;
            if (antlrArgs == null)
            {
                this.Tokens4 = null;
                return false;
            }

            var result = antlrArgs.Result;
            this.Snapshot = e.Snapshot;
            this.Tokens4 = antlrArgs.Tokens;

            Tree.Dispatcher.Invoke(
                (Action)(() =>
                {
                    try
                    {
                        this.Tree.Items.Clear();
                        this.Tree.Items.Add(new ParseTreeWrapper(result));
                    }
                    catch (Exception ex)
                    {
                        if (ErrorHandler.IsCriticalException(ex))
                            throw;
                    }
                }));

            return true;
        }

        public class ParseTreeWrapper
        {
            private readonly IParseTree _tree;

            public ParseTreeWrapper(IParseTree tree)
            {
                _tree = tree;
            }

            public string Text
            {
                get
                {
                    return _tree.ToString();
                }
            }

            public ParseTreeWrapper[] Children
            {
                get
                {
                    List<ParseTreeWrapper> children = new List<ParseTreeWrapper>();
                    for (int i = 0; i < _tree.ChildCount; i++)
                        children.Add(new ParseTreeWrapper(_tree.GetChild(i)));

                    return children.ToArray();
                }
            }
        }
    }
}
