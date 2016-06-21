namespace Tvl.VisualStudio.Language.Antlr3
{
    using System;

    public static class AntlrConstants
    {
        /* The language name (used for the language service) and content type must be the same
         * due to the way Visual Studio internally registers file extensions and content types.
         */
        public const string AntlrLanguageName = "ANTLR";
        public const string AntlrContentType = AntlrLanguageName;
        public const string AntlrFileExtension = ".g";
        public const string AntlrFileExtension2 = ".g3";

        // product registration
        public const int AntlrLanguageResourceId = 100;
        public const string AntlrLanguagePackageNameResourceString = "#110";
        public const string AntlrLanguagePackageDetailsResourceString = "#111";
        public const string AntlrLanguagePackageProductVersionString = "1.1";

        public const string AntlrLanguagePackageGuidString = "8DA6DEAF-0AC9-4501-A21E-1CC07DA482EB";
        public static readonly Guid AntlrLanguagePackageGuid = new Guid("{" + AntlrLanguagePackageGuidString + "}");

        public const string AntlrLanguageGuidString = "F1D24808-BEAB-4507-8544-CECC975E4DE2";
        public static readonly Guid AntlrLanguageGuid = new Guid("{" + AntlrLanguageGuidString + "}");

        public const string UIContextNoSolution = "ADFC4E64-0397-11D1-9F4E-00A0C911004F";
        public const string UIContextSolutionExists = "f1536ef8-92ec-443c-9ed7-fdadf150da82";
    }
}
