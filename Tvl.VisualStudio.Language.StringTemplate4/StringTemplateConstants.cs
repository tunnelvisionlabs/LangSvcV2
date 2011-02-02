namespace Tvl.VisualStudio.Language.StringTemplate4
{
    using System;

    public static class StringTemplateConstants
    {
        /* The language name (used for the language service) and content type must be the same
         * due to the way Visual Studio internally registers file extensions and content types.
         */
        public const string StringTemplateLanguageName = "StringTemplate";
        public const string StringTemplateContentType = StringTemplateLanguageName;
        public const string StringTemplateFileExtension = ".stg";

        // product registration
        public const int StringTemplateLanguageResourceId = 100;
        public const string StringTemplateLanguagePackageNameResourceString = "#110";
        public const string StringTemplateLanguagePackageDetailsResourceString = "#111";
        public const string StringTemplateLanguagePackageProductVersionString = "1.0";

        public const string AntlrIntellisenseOutputWindow = "ANTLR IntelliSense Engine";

        public const string StringTemplateLanguagePackageGuidString = "C087AB33-E4AE-42A9-8FFF-1139EA53C522";
        public static readonly Guid StringTemplateLanguagePackageGuid = new Guid("{" + StringTemplateLanguagePackageGuidString + "}");

        public const string StringTemplateLanguageGuidString = "5640BF87-BBBA-428E-97E6-82BB93FF0BED";
        public static readonly Guid StringTemplateLanguageGuid = new Guid("{" + StringTemplateLanguageGuidString + "}");

        public const string UIContextNoSolution = "ADFC4E64-0397-11D1-9F4E-00A0C911004F";
        public const string UIContextSolutionExists = "f1536ef8-92ec-443c-9ed7-fdadf150da82";
    }
}
