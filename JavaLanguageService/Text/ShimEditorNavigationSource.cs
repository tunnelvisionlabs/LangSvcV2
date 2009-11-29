namespace JavaLanguageService.Text
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using JavaLanguageService.Text.Tagging;

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
