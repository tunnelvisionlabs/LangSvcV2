namespace Tvl.VisualStudio.Language.Php
{
    using System;

    public static class PhpConstants
    {
        /* The language name (used for the language service) and content type must be the same
         * due to the way Visual Studio internally registers file extensions and content types.
         */
        public const string PhpLanguageName = "PHP";
        public const string PhpContentType = PhpLanguageName;
        public const string PhpFileExtension = ".php";
        public const string Php5FileExtension = ".php5";

        // product registration
        public const int PhpLanguageResourceId = 100;
        public const string PhpLanguagePackageNameResourceString = "#110";
        public const string PhpLanguagePackageDetailsResourceString = "#111";
        public const string PhpLanguagePackageProductVersionString = "1.0";

        public const string PhpLanguagePackageGuidString = "F63EC5E6-E89D-491E-8AEE-F7AA2372FBED";
        public static readonly Guid PhpLanguagePackageGuid = new Guid("{" + PhpLanguagePackageGuidString + "}");

        public const string PhpLanguageGuidString = "E7C2D503-A41B-4CD7-B257-2CB71E51D742";
        public static readonly Guid PhpLanguageGuid = new Guid("{" + PhpLanguageGuidString + "}");

        public const string UIContextNoSolution = "ADFC4E64-0397-11D1-9F4E-00A0C911004F";
        public const string UIContextSolutionExists = "f1536ef8-92ec-443c-9ed7-fdadf150da82";
    }
}
