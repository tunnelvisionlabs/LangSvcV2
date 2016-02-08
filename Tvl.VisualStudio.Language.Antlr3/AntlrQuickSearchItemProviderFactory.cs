namespace Tvl.VisualStudio.Language.Antlr3
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Language.NavigateTo.Interfaces;

    [Export(typeof(INavigateToItemProviderFactory))]
    public sealed class AntlrQuickSearchItemProviderFactory : INavigateToItemProviderFactory
    {
        public bool TryCreateNavigateToItemProvider(IServiceProvider serviceProvider, out INavigateToItemProvider provider)
        {
            provider = new AntlrQuickSearchItemProvider(serviceProvider);
            return true;
        }
    }
}
