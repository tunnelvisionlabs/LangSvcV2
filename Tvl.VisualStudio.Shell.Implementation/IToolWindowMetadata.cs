namespace Tvl.VisualStudio.Shell.Implementation
{
    using System.ComponentModel;

    public interface IToolWindowMetadata
    {
        string Name
        {
            get;
        }

        [DefaultValue(null)]
        string MenuCommandLocation
        {
            get;
        }
    }
}
