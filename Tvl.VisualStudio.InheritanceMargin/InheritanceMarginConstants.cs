namespace Tvl.VisualStudio.InheritanceMargin
{
    using Guid = System.Guid;

    public static class InheritanceMarginConstants
    {
        public const string guidInheritanceMarginPackageString = "B03A0D8A-A6E0-4983-B545-F73D2531D534";

        public const string guidInheritanceMarginCommandSetString = "102A7E39-1CD8-4F49-816E-245D813D884E";
        public static readonly Guid guidInheritanceMarginCommandSet = new Guid("{" + guidInheritanceMarginCommandSetString + "}");

        public static readonly int menuInheritanceTargets = 0x0100;
        public static readonly int groupInheritanceTargets = 0x0101;
        public static readonly int cmdidInheritanceTargetsList = 0x0200;
        public static readonly int cmdidInheritanceTargetsListEnd = 0x02FF;
    }
}
