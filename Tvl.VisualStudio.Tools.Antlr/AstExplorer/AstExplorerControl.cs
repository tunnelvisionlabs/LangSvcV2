namespace Tvl.VisualStudio.Tools.AstExplorer
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.ComponentModelHost;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Tvl.Events;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Shell;

    internal partial class AstExplorerControl : ContentControl
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

        private AstExplorerTreeControl Tree
        {
            get;
            set;
        }

        private void HandleTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            HandleTreeViewSelectedItemChangedV3(sender, e);
            HandleTreeViewSelectedItemChangedV4(sender, e);
        }

        private void HandleViewChanged(object sender, ViewChangedEventArgs e)
        {
            BackgroundParser = null;
            Tagger = null;
            Tokens3 = null;
            Tokens4 = null;
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
                    backgroundParser.RequestParse(false);
                }
            }
        }

        private void HandleParseComplete(object sender, ParseResultEventArgs e)
        {
            TryHandleParseCompleteV3(sender, e);
            TryHandleParseCompleteV4(sender, e);
        }
    }
}
