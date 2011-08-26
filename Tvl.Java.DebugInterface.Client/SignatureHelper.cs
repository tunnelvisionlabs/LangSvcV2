namespace Tvl.Java.DebugInterface.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Diagnostics.Contracts;

    internal static class SignatureHelper
    {
        private const string ArgFormat = @"(?:\[*(?:[ZBCDFIJSV]|L[a-zA-Z0-9_$/]+;))";
        private static readonly Regex SignatureFormat =
            new Regex(
                @"^\((?<ARG>" + ArgFormat + @")*\)(?<RET>" + ArgFormat + @")$",
                RegexOptions.Compiled);

        public static void ParseMethodSignature(string signature, out List<string> argumentTypeNames, out string returnTypeName)
        {
            argumentTypeNames = new List<string>();

            Match match = SignatureFormat.Match(signature);
            foreach (var arg in match.Groups["ARG"].Captures.Cast<Capture>())
                argumentTypeNames.Add(arg.Value);

            returnTypeName = match.Groups["RET"].Value;

            for (int i = 0; i < argumentTypeNames.Count; i++)
            {
                argumentTypeNames[i] = DecodeTypeName(argumentTypeNames[i]);
            }

            returnTypeName = DecodeTypeName(returnTypeName);
        }

        public static string DecodeTypeName(string signature)
        {
            Contract.Requires<ArgumentNullException>(signature != null, "signature");
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(signature));

            switch (signature[0])
            {
            case 'Z':
                return "boolean";
            case 'B':
                return "byte";
            case 'C':
                return "char";
            case 'D':
                return "double";
            case 'F':
                return "float";
            case 'I':
                return "int";
            case 'J':
                return "long";
            case 'S':
                return "short";
            case 'V':
                return "void";
            case '[':
                return DecodeTypeName(signature.Substring(1)) + "[]";
            case 'L':
                return signature.Substring(1, signature.Length - 2).Replace('/', '.');
            default:
                throw new FormatException();
            }
        }
    }
}
