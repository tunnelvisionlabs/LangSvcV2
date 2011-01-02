namespace Tvl.VisualStudio.Language.Alloy
{
    using Microsoft.VisualStudio.Text.Editor;
    using Tvl.VisualStudio.Language.Intellisense;

    internal class AlloyIntellisenseController : IntellisenseController
    {
        public AlloyIntellisenseController(ITextView textView, AlloyIntellisenseControllerProvider provider)
            : base(textView, provider)
        {
        }

        public new AlloyIntellisenseControllerProvider Provider
        {
            get
            {
                return (AlloyIntellisenseControllerProvider)base.Provider;
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
