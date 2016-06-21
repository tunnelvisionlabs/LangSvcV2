namespace Tvl.VisualStudio.Language.Chapel
{
    using System;

    public static class ChapelConstants
    {
        /* The language name (used for the language service) and content type must be the same
         * due to the way Visual Studio internally registers file extensions and content types.
         */
        public const string ChapelLanguageName = "Chapel";
        public const string ChapelContentType = ChapelLanguageName;
        public const string ChapelFileExtension = ".chpl";

        // product registration
        public const int ChapelLanguageResourceId = 100;
        public const string ChapelLanguagePackageNameResourceString = "#110";
        public const string ChapelLanguagePackageDetailsResourceString = "#111";
        public const string ChapelLanguagePackageProductVersionString = "1.0";

        public const string ChapelLanguagePackageGuidString = "658EFDD2-75AB-45D8-BC41-5A10D1BF1C6D";
        public static readonly Guid ChapelLanguagePackageGuid = new Guid("{" + ChapelLanguagePackageGuidString + "}");

        public const string ChapelLanguageGuidString = "67398451-D621-4779-A361-B587549A01DB";
        public static readonly Guid ChapelLanguageGuid = new Guid("{" + ChapelLanguageGuidString + "}");

        public const string UIContextNoSolution = "ADFC4E64-0397-11D1-9F4E-00A0C911004F";
        public const string UIContextSolutionExists = "f1536ef8-92ec-443c-9ed7-fdadf150da82";
    }
}
