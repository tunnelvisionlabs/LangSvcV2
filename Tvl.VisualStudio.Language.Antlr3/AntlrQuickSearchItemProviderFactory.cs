namespace Tvl.VisualStudio.Language.Antlr3
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Language.QuickSearch.Interfaces;

    [Export(typeof(IQuickSearchItemProviderFactory))]
    public sealed class AntlrQuickSearchItemProviderFactory : IQuickSearchItemProviderFactory
    {
        public bool TryCreateQuickSearchItemProvider(IServiceProvider serviceProvider, out IQuickSearchItemProvider provider)
        {
            provider = null;
            return false;
        }
    }
}
