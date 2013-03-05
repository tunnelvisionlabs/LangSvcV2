namespace Tvl.VisualStudio.Tools.AstExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using Antlr.Runtime;
    using Antlr.Runtime.Tree;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Text.Tagging;

    partial class AstExplorerControl
    {
        private IList<IToken> Tokens3
        {
            get;
            set;
        }

        private void HandleTreeViewSelectedItemChangedV3(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                if (Tagger == null)
                    return;

                if (Tokens4 != null || e.NewValue is Antlr4.Runtime.Tree.IParseTree)
                    return;

                using (Tagger.Update())
                {
                    Tagger.RemoveTagSpans(tagSpan => true);

                    if (Snapshot == null)
                        return;

                    CommonTree selected = e.NewValue as CommonTree;

                    IList<IToken> tokens = Tokens3;
                    if (tokens != null && selected != null)
                    {
                        if (selected.TokenStartIndex >= 0 && selected.TokenStopIndex >= 0)
                        {
                            IToken startToken = tokens[selected.TokenStartIndex];
                            IToken stopToken = tokens[selected.TokenStopIndex];
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
                    else if (selected != null && selected.Token != null)
                    {
                        if (selected.Token.StartIndex >= 0 && selected.Token.StopIndex >= 0)
                        {
                            IToken token = selected.Token;
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
                }
            }
            catch (Exception ex)
            {
                if (ErrorHandler.IsCriticalException(ex))
                    throw;
            }
        }

        private bool TryHandleParseCompleteV3(object sender, ParseResultEventArgs e)
        {
            if (!object.ReferenceEquals(sender, BackgroundParser))
                return false;

            AntlrParseResultEventArgs antlrArgs = e as AntlrParseResultEventArgs;
            if (antlrArgs == null)
            {
                this.Tokens3 = null;
                return false;
            }

            var result = antlrArgs.Result;
            this.Snapshot = e.Snapshot;
            this.Tokens3 = antlrArgs.Tokens;

            Tree.Dispatcher.Invoke(
                (Action)(() =>
                {
                    try
                    {
                        this.Tree.Items.Clear();
                        IAstRuleReturnScope resultArgs = result as IAstRuleReturnScope;
                        ITree tree = resultArgs != null ? resultArgs.Tree as ITree : null;
                        if (tree != null)
                        {
                            if (!tree.IsNil)
                            {
                                this.Tree.Items.Add(resultArgs.Tree);
                            }
                            else if (tree.ChildCount > 0)
                            {
                                for (int i = 0; i < tree.ChildCount; i++)
                                    this.Tree.Items.Add(tree.GetChild(i));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ErrorHandler.IsCriticalException(ex))
                            throw;
                    }
                }));

            return true;
        }
    }
}
