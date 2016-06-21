namespace Tvl.VisualStudio.Language.Antlr3
{
    using System;
    using Microsoft.VisualStudio.Language.NavigateTo.Interfaces;

    public class AntlrQuickSearchItemProvider : INavigateToItemProvider
    {
        public AntlrQuickSearchItemProvider(IServiceProvider serviceProvider)
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
