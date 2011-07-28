namespace Tvl.VisualStudio.Language.Java.SourceData
{
    using System.Diagnostics.Contracts;

    public class CodeMethod : CodeMember
    {
        public CodeMethod(string name, string fullName, CodeLocation location, CodeElement parent)
            : base(name, fullName, location, parent)
        {
            Contract.Requires(!string.IsNullOrEmpty(name));
            Contract.Requires(!string.IsNullOrEmpty(fullName));
            Contract.Requires(location != null);
            Contract.Requires(parent != null);
        }
    }
}
