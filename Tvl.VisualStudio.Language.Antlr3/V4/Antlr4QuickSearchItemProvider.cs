namespace Tvl.VisualStudio.Language.AntlrV4
{
    using System;
    using Microsoft.VisualStudio.Language.NavigateTo.Interfaces;

    public class Antlr4QuickSearchItemProvider : INavigateToItemProvider
    {
        public Antlr4QuickSearchItemProvider(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider
        {
            get;
            private set;
        }

        public void StartSearch(INavigateToCallback callback, string searchValue)
        {
            //string name = null;
            //string kind = null;
            //string language = null;
            //string secondarySort = null;
            //object tag = null;
            //MatchKind matchKind = MatchKind.None;
            //INavigateToItemDisplayFactory displayFactory = null;
            //NavigateToItem item = new NavigateToItem(name, kind, language, secondarySort, tag, matchKind, displayFactory);
        }

        public void StopSearch()
        {
        }

        public void Dispose()
        {
        }
    }
}
