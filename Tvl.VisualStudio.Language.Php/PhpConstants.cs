namespace Tvl.VisualStudio.Language.Php
{
    using System;

    public static class PhpConstants
    {
        /* Since we have a custom editor factory, the language name and content type
         * *cannot* be the same. We only want the content type associated with a text
         * buffer when explicitly requested in the PhpProjectionBuffer implementation.
         */
        public const string PhpLanguageName = "PHP";
        public const string PhpContentType = "PHPCode";
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
    }
}
