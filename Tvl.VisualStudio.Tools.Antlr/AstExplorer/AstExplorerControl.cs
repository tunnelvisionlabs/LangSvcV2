namespace Tvl.VisualStudio.Tools.AstExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using Antlr.Runtime;
    using Antlr.Runtime.Tree;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.ComponentModelHost;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Tagging;
    using Tvl.Events;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Shell;
    using Tvl.VisualStudio.Shell.Extensions;
    using Tvl.VisualStudio.Text.Tagging;

    internal class AstExplorerControl : ContentControl
    {
        public AstExplorerControl(IServiceProvider serviceProvider)
        {
            IComponentModel componentModel = (IComponentModel)serviceProvider.GetService<SComponentModel>();
            this.ActiveViewTrackerService = componentModel.GetService<IActiveViewTrackerService>();
            this.BackgroundParserFactoryService = componentModel.GetService<IBackgroundParserFactoryService>();
            this.AstReferenceTaggerProvider = componentModel.GetService<IAstReferenceTaggerProvider>();

            this.Tree = new AstExplorerTreeControl();
            this.Content = this.Tree;

            this.ActiveViewTrackerService.ViewChanged += WeakEvents.AsWeak<ViewChangedEventArgs>(HandleViewChanged, eh => this.ActiveViewTrackerService.ViewChanged -= eh);
            this.Tree.SelectedItemChanged += HandleTreeViewSelectedItemChanged;
        }

        private IActiveViewTrackerService ActiveViewTrackerService
        {
            get;
            set;
        }

        private IBackgroundParserFactoryService BackgroundParserFactoryService
        {
            get;
            set;
        }

        private IAstReferenceTaggerProvider AstReferenceTaggerProvider
        {
            get;
            set;
        }

        private IBackgroundParser BackgroundParser
        {
            get;
            set;
        }

        private SimpleTagger<TextMarkerTag> Tagger
        {
            get;
            set;
        }

        private ITextSnapshot Snapshot
        {
            get;
            set;
        }

        private IList<IToken> Tokens
        {
            get;
            set;
        }

        private AstExplorerTreeControl Tree
        {
            get;
            set;
        }

        private void HandleTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                if (Tagger == null)
                    return;

                using (Tagger.Update())
                {
                    Tagger.RemoveTagSpans(tagSpan => true);

                    if (Snapshot == null)
                        return;

                    CommonTree selected = e.NewValue as CommonTree;

                    IList<IToken> tokens = Tokens;
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

        private void HandleViewChanged(object sender, ViewChangedEventArgs e)
        {
            BackgroundParser = null;
            Tagger = null;
            Tokens = null;
            Snapshot = null;
            Tree.Dispatcher.Invoke(
                (Action)(() =>
                {
                    try
                    {
                        Tree.Items.Clear();
                    }
                    catch (Exception ex)
                    {
                        if (ErrorHandler.IsCriticalException(ex))
                            throw;
                    }
                }));

            if (e.NewView != null)
            {
                var backgroundParser = BackgroundParserFactoryService.GetBackgroundParser(e.NewView.TextBuffer);
                BackgroundParser = backgroundParser;
                if (backgroundParser != null)
                {
                    Tagger = AstReferenceTaggerProvider.GetAstReferenceTagger(e.NewView.TextBuffer);
                    backgroundParser.ParseComplete += WeakEvents.AsWeak<ParseResultEventArgs>(HandleParseComplete, eh => backgroundParser.ParseComplete -= eh);
                    backgroundParser.RequestParse();
                }
            }
        }

        private void HandleParseComplete(object sender, ParseResultEventArgs e)
        {
            if (!object.ReferenceEquals(sender, BackgroundParser))
                return;

            AntlrParseResultEventArgs antlrArgs = e as AntlrParseResultEventArgs;
            if (antlrArgs == null)
                return;

            var result = antlrArgs.Result;
            this.Snapshot = e.Snapshot;
            this.Tokens = antlrArgs.Tokens;

            Tree.Dispatcher.Invoke(
                (Action)(() =>
                {
                    try
                    {
                        dynamic resultArgs = result;
                        this.Tree.Items.Clear();
                        ITree tree = resultArgs.Tree;
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
        }
    }
}
