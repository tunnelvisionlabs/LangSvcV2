namespace Tvl.VisualStudio.Language.Parsing.Experimental.Interpreter
{
    using System;
    using System.Diagnostics;
    using Antlr.Runtime;
    using JetBrains.Annotations;
    using Tvl.VisualStudio.Language.Parsing.Experimental.Atn;

    public class InterpretTraceTransition : IEquatable<InterpretTraceTransition>
    {
        public readonly Transition Transition;
        public readonly NetworkInterpreter Interpreter;
        public readonly int? Symbol;
        public readonly int? TokenIndex;

        public InterpretTraceTransition([NotNull] Transition transition, [NotNull] NetworkInterpreter interpreter)
        {
            Requires.NotNull(transition, nameof(transition));
            Requires.NotNull(interpreter, nameof(interpreter));

            Transition = transition;
            Interpreter = interpreter;
        }

        public InterpretTraceTransition([NotNull] Transition transition, int symbol, int symbolPosition, [NotNull] NetworkInterpreter interpreter)
            : this(transition, interpreter)
        {
            Debug.Assert(transition != null);
            Debug.Assert(interpreter != null);

            Symbol = symbol;
            TokenIndex = symbolPosition;
        }

        public IToken Token
        {
            get
            {
                if (!TokenIndex.HasValue)
                    return null;

                return Interpreter.Input.Get(TokenIndex.Value);
            }
        }

        public virtual bool Equals(InterpretTraceTransition other)
        {
            if (other == null)
                return false;

            return TokenIndex == other.TokenIndex
                && Transition.Equals(other.Transition);
        }

        public sealed override bool Equals(object obj)
        {
            return Equals(obj as InterpretTraceTransition);
        }

        public override int GetHashCode()
        {
            return Transition.GetHashCode() ^ TokenIndex.GetHashCode();
        }

        public override string ToString()
        {
            string sourceState = string.Format("{0}({1})", Transition.SourceState.Id, Interpreter.Network.StateRules[Transition.SourceState.Id].Name);
            string targetState = string.Format("{0}({1})", Transition.TargetState.Id, Interpreter.Network.StateRules[Transition.TargetState.Id].Name);

            string transition = "->";
            if (Transition.IsMatch)
            {
                transition = string.Format("-> {0} ->", Symbol);
            }
            else if (Transition.IsContext)
            {
                string op = (Transition is PushContextTransition) ? "push" : "pop";
                string labels = string.Join(" ", ((ContextTransition)Transition).ContextIdentifiers);
                transition = string.Format("-> {0} {1} ->", op, labels);
            }

            return string.Format("{0} {1} {2}", sourceState, transition, targetState);
        }
    }
}
