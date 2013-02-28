namespace Tvl.VisualStudio.Language.AntlrV4
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Language.NavigateTo.Interfaces;

    [Export(typeof(INavigateToItemProviderFactory))]
    public sealed class Antlr4QuickSearchItemProviderFactory : INavigateToItemProviderFactory
    {
        public bool TryCreateNavigateToItemProvider(IServiceProvider serviceProvider, out INavigateToItemProvider provider)
        {
            provider = new Antlr4QuickSearchItemProvider(serviceProvider);
            return true;
        }
    }
}
