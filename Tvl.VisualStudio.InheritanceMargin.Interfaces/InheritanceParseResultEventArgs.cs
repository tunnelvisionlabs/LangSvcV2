namespace Tvl.VisualStudio.InheritanceMargin
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Tvl.VisualStudio.Language.Parsing;

    public class InheritanceParseResultEventArgs : ParseResultEventArgs
    {
        private readonly IEnumerable<ITagSpan<IInheritanceTag>> _tags;

        public InheritanceParseResultEventArgs(ITextSnapshot snapshot, IList<ParseErrorEventArgs> errors, TimeSpan elapsedTime, IEnumerable<ITagSpan<IInheritanceTag>> tags)
            : base(snapshot, errors, elapsedTime)
        {
            this._tags = tags;
        }

        public IEnumerable<ITagSpan<IInheritanceTag>> Tags
        {
            get
            {
                return _tags;
            }
        }
    }
}
