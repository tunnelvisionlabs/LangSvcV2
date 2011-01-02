namespace Tvl.VisualStudio.Language.Alloy
{
    using System;

    public static class AlloyConstants
    {
        public const string AlloyContentType = "als";
        public const string AlloyFileExtension = ".als";

        public const string AntlrIntellisenseOutputWindow = "ANTLR IntelliSense Engine";

        public const string AlloyLanguageGuidString = "886D06A5-E920-44F0-9271-4E8221907DF1";
        public static readonly Guid AlloyLanguageGuid = new Guid("{" + AlloyLanguageGuidString + "}");
    }
}
