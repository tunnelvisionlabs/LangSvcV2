namespace Tvl.VisualStudio.Language.AntlrV4
{
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Text.Editor;

    internal abstract class SmartIndentProvider : ISmartIndentProvider
    {
        private readonly SVsServiceProvider _serviceProvider;

        protected SmartIndentProvider(SVsServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public SVsServiceProvider ServiceProvider
        {
            get
            {
                return _serviceProvider;
            }
        }

        public abstract ISmartIndent CreateSmartIndent(ITextView textView);
    }
}
