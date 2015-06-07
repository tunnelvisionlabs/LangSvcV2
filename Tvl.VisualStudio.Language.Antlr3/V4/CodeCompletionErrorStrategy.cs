namespace Tvl.VisualStudio.Language.AntlrV4
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Atn;
    using Antlr4.Runtime.Misc;

    public class CodeCompletionErrorStrategy : DefaultErrorStrategy
    {
        public override void ReportError(Parser recognizer, RecognitionException e)
        {
            if (e != null && e.OffendingToken != null && e.OffendingToken.Type == CaretToken.CaretTokenType)
                return;

            base.ReportError(recognizer, e);
        }

        public override void Sync(Parser recognizer)
        {
            if (recognizer.InputStream.La(1) == CaretToken.CaretTokenType)
                return;

            // TODO: incorporate error recovery as a fallback option if no trees match correctly
            if (true)
                return;

            base.Sync(recognizer);
        }

        protected override void ConsumeUntil(Parser recognizer, IntervalSet set)
        {
            //System.out.println("consumeUntil("+set.toString(getTokenNames())+")");
            int ttype = recognizer.InputStream.La(1);
            while (ttype != TokenConstants.Eof && ttype != CaretToken.CaretTokenType && !set.Contains(ttype))
            {
                //System.out.println("consume during recover LA(1)="+getTokenNames()[input.LA(1)]);
                recognizer.InputStream.Consume();
                //recognizer.consume();
                ttype = recognizer.InputStream.La(1);
            }
        }

        public override void Recover(Parser recognizer, RecognitionException e)
        {
            if (recognizer is ICodeCompletionParser
                && ((ICodeCompletionParser)recognizer).Interpreter.CaretTransitions != null) {

                //                    int stateNumber = recognizer.getContext().s;
                //                    ATNState state = recognizer.getATN().states.get(stateNumber);
                //                    if (state instanceof DecisionState && recognizer.getInputStream() instanceof ObjectStream) {
                //                        int decision = ((DecisionState)state).decision;
                //                        ParserATNSimulator simulator = recognizer.getInterpreter();
                //                        int prediction = simulator.adaptivePredict((ObjectStream)recognizer.getInputStream(), decision, recognizer.getContext());
                //                    }

                ICodeCompletionParser parser = (ICodeCompletionParser)recognizer;
                ICaretToken token = parser.Interpreter.CaretToken;
                CompletionParserATNSimulator interpreter = parser.Interpreter;

                throw new CaretReachedException(parser.Context, token, interpreter.CaretTransitions, e);
            }

            // TODO: incorporate error recovery as a fallback option if no trees match correctly
            throw e;
            //super.recover(recognizer, e);
        }

        public override IToken RecoverInline(Parser recognizer)
        {
            if (recognizer is ICodeCompletionParser
                && ((ITokenStream)recognizer.InputStream).Lt(1) is ICaretToken) {

                ICodeCompletionParser parser = (ICodeCompletionParser)recognizer;
                ICaretToken token = (ICaretToken)((ITokenStream)recognizer.InputStream).Lt(1);

                CompletionParserATNSimulator interp = parser.Interpreter;
                int stateNumber = recognizer.State;
                ATNState state = interp.atn.states[stateNumber];

                PredictionContext context = PredictionContext.FromRuleContext(interp.atn, recognizer.Context, false);
                ATNConfigSet intermediate = new ATNConfigSet();
                ATNConfigSet closure = new ATNConfigSet();
                for (int i = 0; i < state.NumberOfTransitions; i++)
                {
                    Transition transition = state.Transition(i);
                    if (transition.IsEpsilon)
                    {
                        ATNState target = transition.target;
                        ATNConfig config = ATNConfig.Create(target, i + 1, context);
                        intermediate.Add(config);
                    }
                }

                bool collectPredicates = false;
                bool hasMoreContext = true;
                interp.ClosureHelper(intermediate, closure, collectPredicates, hasMoreContext, PredictionContextCache.Uncached, false);

                if (!state.OnlyHasEpsilonTransitions)
                {
                    for (int i = 0; i < state.NumberOfTransitions; i++)
                    {
                        closure.Add(ATNConfig.Create(state, i + 1, PredictionContext.FromRuleContext(interp.atn, recognizer.Context)));
                    }
                }

                Dictionary<ATNConfig, IList<Transition>> transitions = null;
                int ncl = closure.Count;
                // TODO: foreach
                for (int ci = 0; ci < ncl; ci++)
                {
                    ATNConfig c = closure[ci];

                    List<Transition> configTransitions = null;

                    int n = c.State.NumberOfTransitions;
                    for (int ti = 0; ti < n; ti++)
                    {               // for each transition
                        Transition trans = c.State.Transition(ti);
                        ATNState target = interp.GetReachableTargetHelper(c, trans, CaretToken.CaretTokenType);
                        if (target != null)
                        {
                            if (transitions == null)
                            {
                                transitions = new Dictionary<ATNConfig, IList<Transition>>();
                            }

                            if (configTransitions == null)
                            {
                                configTransitions = new List<Transition>();
                                transitions[c] = configTransitions;
                            }

                            configTransitions.Add(trans);
                        }
                    }
                }

                /*
                 * This should be null if the intended token is not "wordlike", and
                 * should be a single transition from the current state.
                 */
                if (transitions != null)
                {
                    Debug.Assert(transitions.Count == 1);
                    Debug.Assert(transitions.Values.First().Count == 1);
                    Debug.Assert(state.NumberOfTransitions == 1);
                    Debug.Assert(transitions.Values.First()[0] == state.Transition(0));
                }

                throw new CaretReachedException(parser.Context, token, transitions, null);
            }

            return base.RecoverInline(recognizer);
        }

        protected override bool SingleTokenInsertion(Parser recognizer)
        {
            if (recognizer.InputStream.La(1) == CaretToken.CaretTokenType)
                return false;

            return false;
        }

        [return: Nullable]
        protected override IToken SingleTokenDeletion(Parser recognizer)
        {
            if (recognizer.InputStream.La(1) == CaretToken.CaretTokenType)
                return null;

            return null;
        }
    }
}
