namespace Tvl.VisualStudio.Language.Alloy
{
    using Microsoft.VisualStudio.Text.Editor;
    using Tvl.VisualStudio.Language.Intellisense;

    internal class AlloyIntellisenseController : IntellisenseController
    {
        public AlloyIntellisenseController(ITextView textView, IntellisenseControllerProvider provider)
            : base(textView, provider)
        {
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
