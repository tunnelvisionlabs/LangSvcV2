namespace Tvl.VisualStudio.Language.AntlrV4
{
    using System;
    using System.Collections.Generic;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Atn;
    using Antlr4.Runtime.Dfa;
    using Antlr4.Runtime.Misc;

    public abstract class CompletionParserATNSimulator : ParserATNSimulator
    {
        private Dictionary<ATNConfig, IList<Transition>> _caretTransitions;
        private ICaretToken _caretToken;

        private List<MultipleDecisionData> _decisionPoints;
        private List<int> _selections;
        private int _firstDecisionIndex;

        // state variables used for the custom implementation
        private ITokenStream _input;
        private int _startIndex;
        private ParserRuleContext _outerContext;

        // avoid throwing an exception when the caret is found while computing the start state
        private bool _computingStartState;

        protected CompletionParserATNSimulator([NotNull] Parser parser, ATN atn)
            : base(parser, atn)
        {
            Requires.NotNull(parser, nameof(parser));
            PredictionMode = PredictionMode.Sll;
        }

        public virtual Dictionary<ATNConfig, IList<Transition>> CaretTransitions
        {
            get
            {
                return _caretTransitions;
            }
        }

        public virtual ICaretToken CaretToken
        {
            get
            {
                return _caretToken;
            }
        }

        public virtual void SetFixedDecisions([NotNull] List<MultipleDecisionData> decisionPoints, [NotNull] List<int> selections)
        {
            Requires.NotNull(decisionPoints, nameof(decisionPoints));
            Requires.NotNull(selections, nameof(selections));

            _decisionPoints = decisionPoints;
            _selections = selections;
            _caretTransitions = null;
            if (decisionPoints.Count > 0)
                _firstDecisionIndex = decisionPoints[0].InputIndex;
            else
                _firstDecisionIndex = int.MaxValue;
        }

        public override int AdaptivePredict(ITokenStream input, int decision, ParserRuleContext outerContext)
        {
            _input = input;
            _startIndex = _input.Index;
            _outerContext = outerContext;
            _caretTransitions = null;

            if (_decisionPoints != null && _firstDecisionIndex <= _startIndex)
            {
                for (int i = 0; i < _decisionPoints.Count; i++)
                {
                    if (_decisionPoints[i].InputIndex == _startIndex && _decisionPoints[i].Decision == decision)
                        return _decisionPoints[i].Alternatives[_selections[i]];
                }
            }

            return base.AdaptivePredict(input, decision, outerContext);
        }

        [return: NotNull]
        protected override SimulatorState ComputeStartState(DFA dfa, ParserRuleContext globalContext, bool useContext)
        {
            _computingStartState = true;
            try
            {
                return base.ComputeStartState(dfa, globalContext, useContext);
            }
            finally
            {
                _computingStartState = false;
            }
        }

        [return: NotNull]
        protected override DFAState CreateDFAState(DFA dfa, ATNConfigSet configs)
        {
            int t = _input.La(1);
            if (t == AntlrV4.CaretToken.CaretTokenType && !_computingStartState)
            {
                _caretToken = (ICaretToken)_input.Lt(1);
                throw NoViableAlt(_input, _outerContext, configs, _startIndex);
            }

            return base.CreateDFAState(dfa, configs);
        }

        public void ClosureHelper(ATNConfigSet sourceConfigs, ATNConfigSet configs, bool collectPredicates, bool hasMoreContext, PredictionContextCache contextCache, bool treatEofAsEpsilon)
        {
            Closure(sourceConfigs, configs, collectPredicates, hasMoreContext, contextCache, treatEofAsEpsilon);
        }

        protected abstract IntervalSet GetWordlikeTokenTypes();

        public ATNState GetReachableTargetHelper(ATNConfig source, Transition trans, int ttype)
        {
            return GetReachableTarget(source, trans, ttype);
        }

        [return: Nullable]
        protected override ATNState GetReachableTarget(ATNConfig source, Transition trans, int ttype)
        {
            if (ttype == AntlrV4.CaretToken.CaretTokenType)
            {
                ATNState target = null;
                AtomTransition atomTransition = trans as AtomTransition;
                if (atomTransition != null)
                {
                    if (GetWordlikeTokenTypes().Contains(atomTransition.label))
                        target = atomTransition.target;
                }
                else
                {
                    SetTransition setTransition = trans as SetTransition;
                    if (setTransition != null)
                    {
                        bool not = trans is NotSetTransition;
                        foreach (int t in GetWordlikeTokenTypes().ToArray())
                        {
                            if (!not && setTransition.set.Contains(t) || not && !setTransition.set.Contains(t))
                            {
                                target = setTransition.target;
                                break;
                            }
                        }
                    }
                    else
                    {
                        RangeTransition rangeTransition = trans as RangeTransition;
                        if (rangeTransition != null)
                        {
                            // TODO: there must be a better algorithm here
                            int[] wordlikeTokenTypes = GetWordlikeTokenTypes().ToArray();
                            int lowerBound = Array.BinarySearch(wordlikeTokenTypes, rangeTransition.from);
                            int upperBound = Array.BinarySearch(wordlikeTokenTypes, rangeTransition.to);
                            if (lowerBound >= 0 || upperBound >= 0 || lowerBound != upperBound)
                                target = rangeTransition.target;
                        }
                        else
                        {
                            WildcardTransition wildcardTransition = trans as WildcardTransition;
                            if (wildcardTransition != null)
                            {
                                target = trans.target;
                            }
                        }
                    }
                }

                if (_caretTransitions == null)
                    _caretTransitions = new Dictionary<ATNConfig, IList<Transition>>();

                IList<Transition> configTransitions;
                if (!_caretTransitions.TryGetValue(source, out configTransitions))
                {
                    configTransitions = new List<Transition>();
                    _caretTransitions[source] = configTransitions;
                }

                configTransitions.Add(trans);
                return target;
            }

            return base.GetReachableTarget(source, trans, ttype);
        }
    }
}
