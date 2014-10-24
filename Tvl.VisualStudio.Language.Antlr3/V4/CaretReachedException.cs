namespace Tvl.VisualStudio.Language.AntlrV4
{
    using System;
    using System.Collections.Generic;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Atn;

    public class CaretReachedException : OperationCanceledException
    {
        private readonly RuleContext _finalContext;
        private readonly ICaretToken _caretToken;
        private readonly IDictionary<ATNConfig, IList<Transition>> _transitions;

        public CaretReachedException(RuleContext finalContext, ICaretToken caretToken, IDictionary<ATNConfig, IList<Transition>> transitions, RecognitionException innerException)
            : base(innerException != null ? innerException.Message : "The caret was reached.", innerException)
        {
            _finalContext = finalContext;
            _caretToken = caretToken;
            _transitions = transitions;
        }

        public RuleContext FinalContext
        {
            get
            {
                return _finalContext;
            }
        }

        public ICaretToken CaretToken
        {
            get
            {
                return _caretToken;
            }
        }

        public IDictionary<ATNConfig, IList<Transition>> Transitions
        {
            get
            {
                return _transitions;
            }
        }

        public new RecognitionException InnerException
        {
            get
            {
                return (RecognitionException)base.InnerException;
            }
        }
    }
}
