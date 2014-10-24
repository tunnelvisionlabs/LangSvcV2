namespace Tvl.VisualStudio.Language.AntlrV4
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(ISmartIndentProvider))]
    [ContentType(Antlr4Constants.AntlrContentType)]
    internal class Antlr4SmartIndentProvider : SmartIndentProvider
    {
        private readonly Antlr4DynamicAnchorPointsProvider _dynamicAnchorPointsProvider;

        [ImportingConstructor]
        public Antlr4SmartIndentProvider(SVsServiceProvider serviceProvider, Antlr4DynamicAnchorPointsProvider dynamicAnchorPointsProvider)
            : base(serviceProvider)
        {
            _dynamicAnchorPointsProvider = dynamicAnchorPointsProvider;
        }

        public Antlr4DynamicAnchorPointsProvider DynamicAnchorPointsProvider
        {
            get
            {
                return _dynamicAnchorPointsProvider;
            }
        }

        public override ISmartIndent CreateSmartIndent(ITextView textView)
        {
            return new Antlr4SmartIndent(this, null);
        }
    }
}
