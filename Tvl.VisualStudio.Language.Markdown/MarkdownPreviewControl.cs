namespace Tvl.VisualStudio.Language.Markdown
{
    using System;
    using System.Windows.Controls;
    using System.Windows.Navigation;
    using Microsoft.VisualStudio.ComponentModelHost;
    using Microsoft.VisualStudio.Text.Editor;
    using Tvl.Events;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Shell;
    using Tvl.VisualStudio.Shell.Extensions;

    internal class MarkdownPreviewControl : ContentControl
    {
        private const string EmptyWindowHtml = "Open a markdown file to see a preview.";

        public MarkdownPreviewControl(MarkdownPreviewToolWindowPane toolWindowPane, IServiceProvider serviceProvider)
        {
            this.ToolWindowPane = toolWindowPane;

            IComponentModel componentModel = (IComponentModel)serviceProvider.GetService<SComponentModel>();
            this.ActiveViewTrackerService = componentModel.GetService<IActiveViewTrackerService>();
            this.BackgroundParserFactoryService = componentModel.GetService<IBackgroundParserFactoryService>();

            this.Browser = new WebBrowser();
            this.Browser.NavigateToString(EmptyWindowHtml);
            this.Browser.LoadCompleted += OnBrowserLoadCompleted;
            this.Content = this.Browser;

            this.ActiveViewTrackerService.ViewChanged += WeakEvents.AsWeak<ViewChangedEventArgs>(OnViewChanged, eh => this.ActiveViewTrackerService.ViewChanged -= eh);
        }

        private MarkdownPreviewToolWindowPane ToolWindowPane
        {
            get;
            set;
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

        private IBackgroundParser BackgroundParser
        {
            get;
            set;
        }

        private WebBrowser Browser
        {
            get;
            set;
        }

        private ITextView CurrentView
        {
            get;
            set;
        }

        private mshtml.IHTMLElement2 DocumentBody
        {
            get
            {
                var document = Browser.Document as mshtml.IHTMLDocument2;
                if (document != null)
                    return document.body as mshtml.IHTMLElement2;

                return null;
            }
        }

        private void NavigateToString(string text)
        {
            BrowserDispatch(
                () =>
                {
                    try
                    {
                        Browser.NavigateToString(text);
                    }
                    catch
                    {
                    }
                });
        }

        private void BrowserDispatch(Action action)
        {
            Browser.Dispatcher.Invoke(action);
        }

        private void RestoreScrollTop()
        {
            var currentView = CurrentView;
            if (currentView == null)
                return;

            MarkdownPreviewScrollPosition position = currentView.Properties.GetOrCreateSingletonProperty<MarkdownPreviewScrollPosition>(() => new MarkdownPreviewScrollPosition());
            if (position == null)
                return;

            if (position.ScrollTo.HasValue)
            {
                var body = DocumentBody;
                if (body != null)
                    body.scrollTop = position.ScrollTo.Value;
            }

            position.ScrollTo = null;
        }

        private void SaveScrollTop()
        {
            var currentView = CurrentView;
            if (currentView == null)
                return;

            MarkdownPreviewScrollPosition position = currentView.Properties.GetOrCreateSingletonProperty<MarkdownPreviewScrollPosition>(() => new MarkdownPreviewScrollPosition());
            if (position == null)
                return;

            if (!position.ScrollTo.HasValue)
            {
                var body = DocumentBody;
                if (body != null)
                    position.ScrollTo = body.scrollTop;
            }
            else
            {
                position.ScrollTo = null;
            }
        }

        private void OnBrowserLoadCompleted(object sender, NavigationEventArgs e)
        {
            BrowserDispatch(RestoreScrollTop);
        }

        private void OnViewChanged(object sender, ViewChangedEventArgs e)
        {
            BackgroundParser = null;

            if (e.NewView != null)
            {
                var backgroundParser = BackgroundParserFactoryService.GetBackgroundParser(e.NewView.TextBuffer);
                BackgroundParser = backgroundParser;
                if (backgroundParser != null)
                {
                    ToolWindowPane.Caption = "Markdown Preview - ";
                    backgroundParser.ParseComplete += WeakEvents.AsWeak<ParseResultEventArgs>(HandleBackgroundParseComplete, eh => backgroundParser.ParseComplete -= eh);
                    backgroundParser.RequestParse(false);
                }
            }

            CurrentView = e.NewView;

            if (BackgroundParser == null)
            {
                ToolWindowPane.Caption = "Markdown Preview";
                Browser.NavigateToString(EmptyWindowHtml);
            }
        }

        private void HandleBackgroundParseComplete(object sender, ParseResultEventArgs e)
        {
            if (!object.ReferenceEquals(sender, BackgroundParser))
                return;

            MarkdownParseResultEventArgs markdownArgs = e as MarkdownParseResultEventArgs;
            if (e == null)
                return;

            var html = markdownArgs.HtmlText;
            BrowserDispatch(SaveScrollTop);
            NavigateToString(html);
        }

        private class MarkdownPreviewText
        {
            public string HtmlText
            {
                get;
                set;
            }
        }

        private class MarkdownPreviewScrollPosition
        {
            public int? ScrollTo
            {
                get;
                set;
            }
        }
    }
}
