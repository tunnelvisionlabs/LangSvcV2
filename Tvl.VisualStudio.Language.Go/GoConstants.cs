namespace Tvl.VisualStudio.Language.Go
{
    using Guid = System.Guid;

    public static class GoConstants
    {
        /* The language name (used for the language service) and content type must be the same
         * due to the way Visual Studio internally registers file extensions and content types.
         */
        public const string GoLanguageName = "Go";
        public const string GoContentType = GoLanguageName;
        public const string GoFileExtension = ".go";

        // product registration
        public const int GoLanguageResourceId = 100;
        public const string GoLanguagePackageNameResourceString = "#110";
        public const string GoLanguagePackageDetailsResourceString = "#111";
        public const string GoLanguagePackageProductVersionString = "1.0";

        public const string GoLanguagePackageGuidString = "41F2D88D-4866-4FB9-80D2-77227D516E1A";
        public static readonly Guid GoLanguagePackageGuid = new Guid("{" + GoLanguagePackageGuidString + "}");

        public const string GoLanguageGuidString = "EC9287F6-7535-4FEF-811E-692CCB125D67";
        public static readonly Guid GoLanguageGuid = new Guid("{" + GoLanguageGuidString + "}");

        public const string UIContextNoSolution = "ADFC4E64-0397-11D1-9F4E-00A0C911004F";
        public const string UIContextSolutionExists = "f1536ef8-92ec-443c-9ed7-fdadf150da82";
    }
}
