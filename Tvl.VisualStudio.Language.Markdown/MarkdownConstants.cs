namespace Tvl.VisualStudio.Language.Markdown
{
    using System;

    internal sealed class MarkdownConstants
    {
        public const string MarkdownContentType = "markdown";
        public const string MarkdownFileExtension = ".mkd";
        public const string MarkdownFileExtension2 = ".markdown";

        public const int ShowMarkdownPreviewToolWindowCommandId = 0x2001;
        public const string MarkdownPackageCmdSet = "{E61E5162-5029-4329-8C71-AF5FD6FD05B7}";
        public static readonly Guid GuidMarkdownPackageCmdSet = new Guid(MarkdownPackageCmdSet);
    }
}
