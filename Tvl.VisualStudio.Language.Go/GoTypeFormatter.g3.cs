namespace Tvl.VisualStudio.Language.Go
{
    using Antlr.Runtime.Tree;

    partial class GoTypeFormatter
    {
        public static string FormatType(ITree typeTree)
        {
            if (typeTree == null)
                return "<null>";

            CommonTreeNodeStream input = new CommonTreeNodeStream(typeTree);
            GoTypeFormatter formatter = new GoTypeFormatter(input);
            return formatter.type();
        }

        private static string HandleExpectedType(string result)
        {
            if (string.IsNullOrWhiteSpace(result))
                return "?";

            return result;
        }
    }
}
