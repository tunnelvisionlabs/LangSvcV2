namespace Tvl.VisualStudio.Text
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using System.ComponentModel.Composition;

    [Export(typeof(ICompletionTargetMapService))]
    public class CompletionTargetMapService : ICompletionTargetMapService
    {
        public CompletionTargetMapService()
        {
        }

        [ImportMany]
        private IEnumerable<Lazy<ICompletionTargetProvider, ICompletionTargetProviderMetadata>> CompletionTargetProviders
        {
            get;
            set;
        }

        public ICompletionTarget GetCompletionTargetForTextView(ITextView textView)
        {
            return textView.Properties.GetOrCreateSingletonProperty<ICompletionTarget>(() => CreateCompletionTarget(textView));
        }

        private ICompletionTarget CreateCompletionTarget(ITextView textView)
        {
            var providers = CompletionTargetProviders.Where(i => IsMatchedProvider(textView, i.Metadata)).ToArray();
            if (providers.Length == 0)
                return null;

            if (providers.Length > 1)
                throw new NotSupportedException();

            var provider = providers[0].Value;
            if (provider == null)
                return null;

            return provider.CreateCompletionTarget(textView);
        }

        private bool IsMatchedProvider(ITextView textView, ICompletionTargetProviderMetadata metadata)
        {
            if (metadata == null || metadata.ContentTypes == null || metadata.TextViewRoles == null)
                return false;

            if (!metadata.ContentTypes.Any(i => textView.TextBuffer.ContentType.IsOfType(i)))
                return false;

            if (!textView.Roles.ContainsAny(metadata.TextViewRoles))
                return false;

            return true;
        }
    }
}
