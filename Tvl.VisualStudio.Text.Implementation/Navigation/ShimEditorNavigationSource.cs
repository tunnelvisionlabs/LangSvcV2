namespace Tvl.VisualStudio.Text.Navigation.Implementation
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.Text.Tagging;
    using Tvl.VisualStudio.Text;
    using Tvl.VisualStudio.Text.Tagging;

    internal class ShimEditorNavigationSource : IEditorNavigationSource
    {
        public event EventHandler NavigationTargetsChanged;

        public ShimEditorNavigationSource(ITagAggregator<ILanguageElementTag> aggregator)
        {
            this.Aggregator = aggregator;
            this.Aggregator.TagsChanged += OnAggregatorTagsChanged;
        }

        public ITagAggregator<ILanguageElementTag> Aggregator
        {
            get;
            private set;
        }

        public IEnumerable<IEditorNavigationTarget> GetNavigationTargets()
        {
            yield break;
        }

        protected virtual void OnNavigationTargetsChanged(EventArgs e)
        {
            var t = NavigationTargetsChanged;
            if (t != null)
                t(this, e);
        }

        private void OnAggregatorTagsChanged(object sender, TagsChangedEventArgs e)
        {
        }
    }
}
