namespace Tvl.VisualStudio.Language.Parsing.Experimental
{
    public class RuleBinding
    {
        public readonly string Name;
        public readonly State StartState;
        public readonly State EndState;

        public RuleBinding(string name)
            : this(name, new State(), new State())
        {
        }

        public RuleBinding(string name, State startState, State endState)
        {
            Name = name;
            StartState = startState;
            EndState = endState;
        }
    }
}
