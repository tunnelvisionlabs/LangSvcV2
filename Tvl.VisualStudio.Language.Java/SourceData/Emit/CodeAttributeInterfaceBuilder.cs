namespace Tvl.VisualStudio.Language.Java.SourceData.Emit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Tvl.VisualStudio.Language.Parsing.Collections;
    using System.Diagnostics.Contracts;

    public class CodeAttributeInterfaceBuilder : CodeInterfaceBuilder
    {
        public CodeAttributeInterfaceBuilder(CodeElementBuilder parent, string name, Interval span, Interval seek)
            : base(parent, name, span, seek)
        {
            Contract.Requires(parent != null);
            Contract.Requires(!string.IsNullOrEmpty(name));
        }
    }
}
