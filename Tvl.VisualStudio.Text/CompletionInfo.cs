namespace Tvl.VisualStudio.Text
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Text;

    public class CompletionInfo
    {
        private readonly ICompletionTarget _completionTarget;

        public CompletionInfo(ICompletionTarget completionTarget)
        {
            _completionTarget = completionTarget;
        }

        public virtual ITrackingSpan ApplicableTo
        {
            get;
            set;
        }

        public virtual string TextSoFar
        {
            get
            {
                if (TextSoFarTrackingSpan == null)
                    return null;

                return TextSoFarTrackingSpan.GetSpan(this.CompletionTarget.TextView.TextBuffer.CurrentSnapshot).GetText();
            }
        }

        public virtual ITrackingSpan TextSoFarTrackingSpan
        {
            get;
            set;
        }

        public virtual CompletionInfoType InfoType
        {
            get;
            set;
        }

        public virtual IntellisenseInvocationType InvocationType
        {
            get;
            set;
        }

        public virtual CompletionFlags CompletionFlags
        {
            get;
            set;
        }

        public virtual CompletionDropDownFlags DropDownFlags
        {
            get;
            set;
        }

        protected ICompletionTarget CompletionTarget
        {
            get
            {
                return _completionTarget;
            }
        }
    }
}
