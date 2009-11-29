namespace Tvl.VisualStudio.Language.Parsing
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.VisualStudio.Text;

    public class ParseResultEventArgs : EventArgs
    {
        public ParseResultEventArgs(ITextSnapshot snapshot, IList<ParseErrorEventArgs> errors)
        {
            this.Snapshot = snapshot;
            this.Errors = new ReadOnlyCollection<ParseErrorEventArgs>(errors);
        }

        public ITextSnapshot Snapshot
        {
            get;
            private set;
        }

        public ReadOnlyCollection<ParseErrorEventArgs> Errors
        {
            get;
            private set;
        }
    }
}
