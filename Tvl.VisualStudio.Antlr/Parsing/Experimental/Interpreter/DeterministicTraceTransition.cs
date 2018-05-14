namespace Tvl.VisualStudio.Language.Parsing.Experimental.Interpreter
{
    using Antlr.Runtime;
    using JetBrains.Annotations;

    public class DeterministicTraceTransition
    {
        public readonly DeterministicTransition Transition;
        public readonly NetworkInterpreter Interpreter;
        public readonly int Symbol;
        public readonly int TokenIndex;

        public DeterministicTraceTransition([NotNull] DeterministicTransition transition, int symbol, int symbolPosition, [NotNull] NetworkInterpreter interpreter)
        {
            Requires.NotNull(transition, nameof(transition));
            Requires.NotNull(interpreter, nameof(interpreter));

            Transition = transition;
            Interpreter = interpreter;
            Symbol = symbol;
            TokenIndex = symbolPosition;
        }

        public IToken Token
        {
            get
            {
                return Interpreter.Input.Get(TokenIndex);
            }
        }
    }
}
