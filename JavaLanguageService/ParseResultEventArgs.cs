namespace JavaLanguageService
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Antlr.Runtime;

    public class ParseResultEventArgs : EventArgs
    {
        public ParseResultEventArgs(ParserRuleReturnScope result, IList<ParseErrorEventArgs> errors)
        {
            this.Result = result;
            this.Errors = new ReadOnlyCollection<ParseErrorEventArgs>(errors);
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
