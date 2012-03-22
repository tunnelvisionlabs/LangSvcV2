namespace Tvl.VisualStudio.FindInSolutionExplorer
{
    using Guid = System.Guid;

    public static class FindInSolutionExplorerConstants
    {
        public const string guidFindInSolutionExplorerPackageString = "0F939440-B82D-4622-AED4-74828B40C8D2";

        public const string guidFindInSolutionExplorerCommandSetString = "B3AD9EAD-9439-445D-BECB-6176098247AC";
        public static readonly Guid guidFindInSolutionExplorerCommandSet = new Guid("{" + guidFindInSolutionExplorerCommandSetString + "}");

        public static readonly int cmdidFindInSolutionExplorer = 0x0100;
    }
}
