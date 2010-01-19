namespace Tvl.VisualStudio.Language.Antlr3
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;

    internal sealed class AntlrCompletionIntellisenseController : IIntellisenseController
    {
        ICompletionBroker _broker;

        /// <summary>
        /// Called when a new subject ITextBuffer appears in the graph of buffers associated with
        /// the ITextView, due to a change in projection or content type.
        /// </summary>
        /// <param name="subjectBuffer">The newly-connected text buffer.</param>
        public void ConnectSubjectBuffer(ITextBuffer subjectBuffer)
        {
        }

        /// <summary>
        /// Detaches the controller from the specified ITextView.
        /// </summary>
        /// <param name="textView">The ITextView from which the controller should detach.</param>
        public void Detach(ITextView textView)
        {
        }

        /// <summary>
        /// Called when a subject ITextBuffer is removed from the graph of buffers associated with
        /// the ITextView, due to a change in projection or content type.
        /// </summary>
        /// <param name="subjectBuffer">The disconnected text buffer.</param>
        public void DisconnectSubjectBuffer(ITextBuffer subjectBuffer)
        {
        }
    }
}
