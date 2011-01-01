namespace Tvl.VisualStudio.Language.Intellisense
{
    using ITrackingSpan = Microsoft.VisualStudio.Text.ITrackingSpan;

    public class CompletionInfo
    {
        private readonly IntellisenseController _controller;

        public CompletionInfo(IntellisenseController controller)
        {
            _controller = controller;
        }

        //public virtual ITrackingSpan ApplicableTo
        //{
        //    get;
        //    set;
        //}

        //public virtual string TextSoFar
        //{
        //    get
        //    {
        //        if (TextSoFarTrackingSpan == null)
        //            return null;

        //        return TextSoFarTrackingSpan.GetSpan(Controller.TextView.TextBuffer.CurrentSnapshot).GetText();
        //    }
        //}

        //public virtual ITrackingSpan TextSoFarTrackingSpan
        //{
        //    get;
        //    set;
        //}

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

        //public virtual CompletionFlags CompletionFlags
        //{
        //    get;
        //    set;
        //}

        //public virtual CompletionDropDownFlags DropDownFlags
        //{
        //    get;
        //    set;
        //}

        public virtual char? CommitChar
        {
            get;
            set;
        }

        protected IntellisenseController Controller
        {
            get
            {
                return _controller;
            }
        }
    }
}
