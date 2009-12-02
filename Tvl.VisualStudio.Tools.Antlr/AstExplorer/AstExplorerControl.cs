namespace Tvl.VisualStudio.Tools.AstExplorer
{
    using System.Windows;
    using System.Windows.Controls;
    using Antlr.Runtime;
    using Antlr.Runtime.Tree;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Tvl.Events;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Shell;
    using Tvl.VisualStudio.Text.Tagging;
    using System;
    using System.Collections.Generic;

    internal class AstExplorerControl : ContentControl
    {
        private readonly AstExplorerProvider _provider;
        private readonly IActiveViewTrackerService _activeViewTrackerService;
        private readonly IBackgroundParserFactoryService _backgroundParserFactoryService;
        private IBackgroundParser _backgroundParser;
        private SimpleTagger<TextMarkerTag> _tagger;

        private ITextSnapshot _snapshot;
        private IList<IToken> _tokens;
        private AstExplorerTreeControl _tree;

        public AstExplorerControl(AstExplorerProvider provider)
        {
            this._provider = provider;
            this._activeViewTrackerService = provider.ActiveViewTrackerService;
            this._backgroundParserFactoryService = provider.BackgroundParserFactoryService;

            this._tree = new AstExplorerTreeControl();
            this.Content = this._tree;

            this._activeViewTrackerService.ViewChanged += WeakEvents.AsWeak<ViewChangedEventArgs>(OnViewChanged, eh => this._activeViewTrackerService.ViewChanged -= eh);
            this._tree.SelectedItemChanged += OnTreeViewSelectedItemChanged;
        }

        void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                if (_tagger == null)
                    return;

                using (_tagger.Update())
                {
                    _tagger.RemoveTagSpans(tagSpan => true);

                    if (_snapshot == null)
                        return;

                    CommonTree selected = e.NewValue as CommonTree;

                    IList<IToken> tokens = _tokens;
                    if (tokens != null && selected != null)
                    {
                        if (selected.TokenStartIndex >= 0 && selected.TokenStopIndex >= 0)
                        {
                            IToken startToken = tokens[selected.TokenStartIndex];
                            IToken stopToken = tokens[selected.TokenStopIndex];
                            Span span = new Span(startToken.StartIndex, stopToken.StopIndex - startToken.StartIndex + 1);
                            ITrackingSpan trackingSpan = _snapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgeExclusive);
                            _tagger.CreateTagSpan(trackingSpan, PredefinedTextMarkerTags.Vivid);
                        }
                    }
                    else if (selected != null && selected.Token != null)
                    {
                        if (selected.Token.StartIndex >= 0 && selected.Token.StopIndex >= 0)
                        {
                            IToken token = selected.Token;
                            Span span = new Span(token.StartIndex, token.StopIndex - token.StartIndex + 1);
                            ITrackingSpan trackingSpan = _snapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgeExclusive);
                            _tagger.CreateTagSpan(trackingSpan, PredefinedTextMarkerTags.Vivid);
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
            _backgroundParser = null;
            _tagger = null;
            _tokens = null;
            _snapshot = null;

            if (e.NewView != null)
            {
                var backgroundParser = _backgroundParserFactoryService.GetBackgroundParser(e.NewView.TextBuffer);
                _backgroundParser = backgroundParser;
                if (backgroundParser != null)
                {
                    _tagger = _provider.AstReferenceTaggerProvider.GetAstReferenceTagger(e.NewView.TextBuffer);
                    backgroundParser.ParseComplete += WeakEvents.AsWeak<ParseResultEventArgs>(OnParseComplete, eh => backgroundParser.ParseComplete -= eh);
                }
            }
        }

        private void OnParseComplete(object sender, ParseResultEventArgs e)
        {
            if (!object.ReferenceEquals(sender, _backgroundParser))
                return;

            var result = ((AntlrParseResultEventArgs)e).Result;
            _snapshot = e.Snapshot;
            _tokens = ((AntlrParseResultEventArgs)e).Tokens;

            _tree.Dispatcher.Invoke(
                (Action)(() =>
                {
                    this._tree.Items.Clear();
                    this._tree.Items.Add(result.Tree);
                }));
        }
    }
}
