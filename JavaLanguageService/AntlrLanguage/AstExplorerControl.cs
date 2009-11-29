namespace JavaLanguageService.AntlrLanguage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Controls;
    using JavaLanguageService.Language.Parsing;
    using JavaLanguageService.ShellServices;
    using System.Windows;
    using Antlr.Runtime;
    using Antlr.Runtime.Tree;
    using Microsoft.VisualStudio.Text.Editor;

    public class AstExplorerControl : Panel
    {
        private readonly IActiveViewTrackerService _activeViewTrackerService;
        private readonly IBackgroundParserFactoryService _backgroundParserFactoryService;
        private IBackgroundParser _backgroundParser;

        private TreeView _treeView;

        public AstExplorerControl(IActiveViewTrackerService activeViewTrackerService, IBackgroundParserFactoryService backgroundParserFactoryService)
        {
            this._activeViewTrackerService = activeViewTrackerService;
            this._backgroundParserFactoryService = backgroundParserFactoryService;

            this._activeViewTrackerService.ViewChanged += WeakEvents.MakeWeak<ViewChangedEventArgs>(OnViewChanged, eh => this._activeViewTrackerService.ViewChanged -= eh);
            this._treeView.SelectedItemChanged += OnTreeViewSelectedItemChanged;
        }

        void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            IToken[] tokens = null;
            CommonTree selected = e.NewValue as CommonTree;
            if (tokens != null && selected != null)
            {
                if (selected.TokenStartIndex >= 0 && selected.TokenStopIndex >= 0)
                {
                    IWpfTextView view = null;
                }
            }
        }

        private void OnViewChanged(object sender, ViewChangedEventArgs e)
        {
            var backgroundParser = _backgroundParserFactoryService.GetBackgroundParser(e.NewView.TextBuffer);
            _backgroundParser = backgroundParser;
            backgroundParser.ParseComplete += WeakEvents.MakeWeak<ParseResultEventArgs>(OnParseComplete, eh => backgroundParser.ParseComplete -= eh);
        }

        private void OnParseComplete(object sender, ParseResultEventArgs e)
        {
            var result = e.Result;
            var tree = result.Tree;
        }
    }
}
