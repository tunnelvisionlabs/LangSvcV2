namespace Tvl.VisualStudio.Language.AntlrV4
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;

    [Export]
    internal class Antlr4DynamicAnchorPointsProvider
    {
        private readonly Antlr4ReferenceAnchorPointsProvider _referenceAnchorPointsProvider;

        [ImportingConstructor]
        public Antlr4DynamicAnchorPointsProvider(Antlr4ReferenceAnchorPointsProvider referenceAnchorPointsProvider)
        {
            _referenceAnchorPointsProvider = referenceAnchorPointsProvider;
        }

        public Antlr4ReferenceAnchorPointsProvider ReferenceAnchorPointsProvider
        {
            get
            {
                return _referenceAnchorPointsProvider;
            }
        }

        public Antlr4DynamicAnchorPoints GetDynamicAnchorPoints(ITextBuffer textBuffer)
        {
            return textBuffer.Properties.GetOrCreateSingletonProperty(() => new Antlr4DynamicAnchorPoints(this, textBuffer));
        }
    }
}
