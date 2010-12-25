namespace Tvl.VisualStudio.Language.Alloy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Tvl.VisualStudio.Text;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Utilities;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text;

    [ContentType(AlloyConstants.AlloyContentType)]
    [Export(typeof(IIntellisenseControllerProvider))]
    [Order(After = "default")]
    [Name("AlloyCompletionControllerProvider")]
    internal class AlloyCompletionControllerProvider : IIntellisenseControllerProvider
    {
        [Import]
        internal ICompletionTargetMapService CompletionTargetMapService
        {
            get;
            private set;
        }

        public IIntellisenseController TryCreateIntellisenseController(ITextView textView, IList<ITextBuffer> subjectBuffers)
        {
            IIntellisenseController controller = new AlloyIntellisenseController(textView, this);
            return controller;
        }
    }
}
