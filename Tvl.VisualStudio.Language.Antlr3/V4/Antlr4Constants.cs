namespace Tvl.VisualStudio.Language.AntlrV4
{
    using System;

    public static class Antlr4Constants
    {
        /* The language name (used for the language service) and content type must be the same
         * due to the way Visual Studio internally registers file extensions and content types.
         */
        public const string AntlrLanguageName = "ANTLR4";
        public const string AntlrContentType = AntlrLanguageName;
        public const string AntlrFileExtension = ".g4";

        // product registration
        public const int AntlrLanguageResourceId = 101;

        public const string Antlr4LanguageGuidString = "F8280F68-DC25-4104-A860-9A945B700CBD";
        public static readonly Guid Antlr4LanguageGuid = new Guid("{" + Antlr4LanguageGuidString + "}");
    }
}
