namespace Tvl.VisualStudio.Language.Parsing
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Antlr.Runtime;
    using Microsoft.VisualStudio.Text;

    public class ParseResultEventArgs : EventArgs
    {
        public ParseResultEventArgs(ITextSnapshot snapshot, ParserRuleReturnScope result, IList<ParseErrorEventArgs> errors)
        {
            this.Snapshot = snapshot;
            this.Result = result;
            this.Errors = new ReadOnlyCollection<ParseErrorEventArgs>(errors);
        }

        public ITextSnapshot Snapshot
        {
            get;
            private set;
        }

        public ParserRuleReturnScope Result
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
