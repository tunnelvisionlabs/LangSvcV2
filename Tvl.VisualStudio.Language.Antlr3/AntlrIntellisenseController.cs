namespace Tvl.VisualStudio.Language.Antlr3
{
    using Microsoft.VisualStudio.Text.Editor;
    using Tvl.VisualStudio.Language.Intellisense;

    internal class AntlrIntellisenseController : IntellisenseController
    {
        public AntlrIntellisenseController(ITextView textView, AntlrIntellisenseControllerProvider provider)
            : base(textView, provider)
        {
        }

        public new AntlrIntellisenseControllerProvider Provider
        {
            get
            {
                return (AntlrIntellisenseControllerProvider)base.Provider;
            }
        }

        public override bool SupportsCompletion
        {
            get
            {
                return true;
            }
        }
    }
}
