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

        public ICompletionTarget CompletionTarget
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual ITrackingSpan ApplicableTo
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

        public virtual string TextSoFar
        {
            get;
            set;
        }
    }
}
