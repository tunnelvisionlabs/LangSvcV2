namespace Tvl.VisualStudio.Tools.AstExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using Antlr.Runtime;
    using Antlr.Runtime.Tree;
    using Microsoft.VisualStudio.ComponentModelHost;
    using Microsoft.VisualStudio.Text;
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

            this.ActiveViewTrackerService.ViewChanged += WeakEvents.AsWeak<ViewChangedEventArgs>(OnViewChanged, eh => this.ActiveViewTrackerService.ViewChanged -= eh);
            this.Tree.SelectedItemChanged += OnTreeViewSelectedItemChanged;
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

        private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
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
                            ITrackingSpan trackingSpan = Snapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgeExclusive);
                            Tagger.CreateTagSpan(trackingSpan, PredefinedTextMarkerTags.Vivid);
                        }
                    }
                    else if (selected != null && selected.Token != null)
                    {
                        if (selected.Token.StartIndex >= 0 && selected.Token.StopIndex >= 0)
                        {
                            IToken token = selected.Token;
                            Span span = new Span(token.StartIndex, token.StopIndex - token.StartIndex + 1);
                            ITrackingSpan trackingSpan = Snapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgeExclusive);
                            Tagger.CreateTagSpan(trackingSpan, PredefinedTextMarkerTags.Vivid);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void OnViewChanged(object sender, ViewChangedEventArgs e)
        {
            BackgroundParser = null;
            Tagger = null;
            Tokens = null;
            Snapshot = null;

            if (e.NewView != null)
            {
                var backgroundParser = BackgroundParserFactoryService.GetBackgroundParser(e.NewView.TextBuffer);
                BackgroundParser = backgroundParser;
                if (backgroundParser != null)
                {
                    Tagger = AstReferenceTaggerProvider.GetAstReferenceTagger(e.NewView.TextBuffer);
                    backgroundParser.ParseComplete += WeakEvents.AsWeak<ParseResultEventArgs>(OnParseComplete, eh => backgroundParser.ParseComplete -= eh);
                }
            }
        }

        private void OnParseComplete(object sender, ParseResultEventArgs e)
        {
            if (!object.ReferenceEquals(sender, BackgroundParser))
                return;

            var result = ((AntlrParseResultEventArgs)e).Result;
            this.Snapshot = e.Snapshot;
            this.Tokens = ((AntlrParseResultEventArgs)e).Tokens;

            Tree.Dispatcher.Invoke(
                (Action)(() =>
                {
                    try
                    {
                        this.Tree.Items.Clear();
                        this.Tree.Items.Add(result.Tree);
                    }
                    catch
                    {
                    }
                }));
        }
    }
}
