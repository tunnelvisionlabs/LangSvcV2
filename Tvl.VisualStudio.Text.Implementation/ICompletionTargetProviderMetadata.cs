namespace Tvl.VisualStudio.Text
{
    using System.Collections.Generic;

    public interface ICompletionTargetProviderMetadata
    {
        IEnumerable<string> ContentTypes
        {
            get;
        }

        IEnumerable<string> TextViewRoles
        {
            get;
        }
    }
}
