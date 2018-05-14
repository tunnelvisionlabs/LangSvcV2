namespace Tvl.VisualStudio.Shell
{
    using System.Collections.Generic;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell.Interop;
    using IEnumString = Microsoft.VisualStudio.OLE.Interop.IEnumString;

    public static class IVsCmdNameMappingExtensions
    {
        [NotNull]
        public static IEnumerable<string> GetMacroNames([NotNull] this IVsCmdNameMapping commandNameMapping)
        {
            Requires.NotNull(commandNameMapping, nameof(commandNameMapping));

            IEnumString enumString;
            if (ErrorHandler.Succeeded(commandNameMapping.EnumMacroNames(VSCMDNAMEOPTS.CNO_GETENU, out enumString)))
            {
                string[] array = new string[1];
                while (true)
                {
                    uint count;
                    int hr = enumString.Next((uint)array.Length, array, out count);
                    ErrorHandler.ThrowOnFailure(hr);
                    if (hr == VSConstants.S_FALSE || count == 0)
                        break;

                    for (uint i = 0; i < count; i++)
                        yield return array[i];
                }
            }
        }

        [NotNull]
        public static IEnumerable<string> GetCommandNames([NotNull] this IVsCmdNameMapping commandNameMapping)
        {
            Requires.NotNull(commandNameMapping, nameof(commandNameMapping));

            IEnumString enumString;
            if (ErrorHandler.Succeeded(commandNameMapping.EnumNames(VSCMDNAMEOPTS.CNO_GETENU, out enumString)))
            {
                string[] array = new string[1];
                while (true)
                {
                    uint count;
                    int hr = enumString.Next((uint)array.Length, array, out count);
                    ErrorHandler.ThrowOnFailure(hr);
                    if (hr == VSConstants.S_FALSE || count == 0)
                        break;

                    for (uint i = 0; i < count; i++)
                        yield return array[i];
                }
            }
        }
    }
}
