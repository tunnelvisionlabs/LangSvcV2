namespace Tvl.VisualStudio.Language.Alloy.IntellisenseModel
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.Language.Intellisense;
    using ImageSource = System.Windows.Media.ImageSource;

    internal abstract class Element
    {
        protected static readonly IEnumerable<Element> EmptyElements = Enumerable.Empty<Element>();

        public ImageSource Glyph
        {
            get
            {
                return null;
            }
        }

        public abstract string Name
        {
            get;
        }

        public abstract AlloyFile File
        {
            get;
        }

        public abstract bool IsExternallyVisible
        {
            get;
        }

        public virtual Completion CreateCompletion(AlloyIntellisenseController controller, ICompletionSession session)
        {
            return null;
        }

        public virtual ISignature CreateSignature(AlloyIntellisenseController controller, ISignatureHelpSession session)
        {
            return null;
        }

        public virtual void GetQuickInfo(AlloyIntellisenseController controller, IQuickInfoSession session, IList<object> content)
        {
        }

        public virtual IEnumerable<Element> GetDeclarations(string op)
        {
            return EmptyElements;
        }

        public virtual IEnumerable<Element> GetScopedDeclarations()
        {
            return EmptyElements;
        }
    }
}
