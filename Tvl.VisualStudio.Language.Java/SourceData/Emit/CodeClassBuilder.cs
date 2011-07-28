namespace Tvl.VisualStudio.Language.Java.SourceData.Emit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Tvl.VisualStudio.Language.Parsing.Collections;
    using System.Diagnostics.Contracts;

    public class CodeClassBuilder : CodeTypeBuilder
    {
        public CodeClassBuilder(CodeElementBuilder parent, string name, Interval span, Interval seek)
            : base(parent, name, span, seek)
        {
            Contract.Requires(parent != null);
            Contract.Requires(!string.IsNullOrEmpty(name));
        }

        protected override CodeElement CreateCodeElement(string name, string fullName, CodeLocation location, CodeElement parent)
        {
            CodeClass element = new CodeClass(name, fullName, location, parent);
            return element;
        }
    }
}
