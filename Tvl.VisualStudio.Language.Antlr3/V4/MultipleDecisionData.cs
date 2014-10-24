namespace Tvl.VisualStudio.Language.AntlrV4
{
    public struct MultipleDecisionData
    {
        public readonly int[] Alternatives;
        public readonly int Decision;
        public readonly int InputIndex;

        public MultipleDecisionData(int inputIndex, int decision, int[] alternatives)
        {
            InputIndex = inputIndex;
            Decision = decision;
            Alternatives = alternatives;
        }
    }
}
