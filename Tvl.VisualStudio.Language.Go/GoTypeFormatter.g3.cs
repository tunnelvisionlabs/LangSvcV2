namespace Tvl.VisualStudio.Language.Go
{
    using System;
    using Antlr.Runtime.Tree;
    using Microsoft.VisualStudio;

    partial class GoTypeFormatter
    {
        public static string FormatType(ITree typeTree)
        {
            if (typeTree == null)
                return "<null>";

            try
            {
                CommonTreeNodeStream input = new CommonTreeNodeStream(typeTree);
                GoTypeFormatter formatter = new GoTypeFormatter(input);
                return formatter.type();
            }
            catch (Exception e)
            {
                if (ErrorHandler.IsCriticalException(e))
                    throw;

                return "?";
            }
        }

        private static string HandleExpectedType(string result)
        {
            if (string.IsNullOrWhiteSpace(result))
                return "?";

            return result;
        }
    }
}
