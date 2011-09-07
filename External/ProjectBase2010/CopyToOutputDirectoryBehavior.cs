namespace Microsoft.VisualStudio.Project
{
    [PropertyPageTypeConverter(typeof(CopyToOutputDirectoryBehaviorConverter))]
    public enum CopyToOutputDirectoryBehavior
    {
        DoNotCopy,
        Always,
        PreserveNewest,
    }
}
