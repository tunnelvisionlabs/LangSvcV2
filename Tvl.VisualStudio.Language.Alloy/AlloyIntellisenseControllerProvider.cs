namespace Tvl.VisualStudio.Language.Alloy
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Language.Intellisense;

    [Export(typeof(IIntellisenseControllerProvider))]
    [ContentType(AlloyConstants.AlloyContentType)]
    [Order]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    [Name("AlloyIntellisenseController")]
    internal class AlloyIntellisenseControllerProvider : IntellisenseControllerProvider
    {
        [Import]
        public IDispatcherGlyphService GlyphService
        {
            get;
            private set;
        }

        [Import]
        public AlloyIntellisenseCache IntellisenseCache
        {
            get;
            private set;
        }

        protected override IntellisenseController TryCreateIntellisenseController([NotNull] ITextView textView, [NotNull] IList<ITextBuffer> subjectBuffers)
        {
            Requires.NotNull(textView, nameof(textView));
            Requires.NotNull(subjectBuffers, nameof(subjectBuffers));

            AlloyIntellisenseController controller = new AlloyIntellisenseController(textView, this);
            textView.Properties[typeof(AlloyIntellisenseController)] = controller;
            return controller;
        }
    }
}
