namespace Tvl.VisualStudio.Shell.Extensions
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell.Interop;
    using IEnumString = Microsoft.VisualStudio.OLE.Interop.IEnumString;

    public static class IVsCmdNameMappingExtensions
    {
        private static IEnumerable<string> GetMacroNames(this IVsCmdNameMapping commandNameMapping)
        {
            if (commandNameMapping == null)
                throw new NullReferenceException();

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

        private static IEnumerable<string> GetCommandNames(IVsCmdNameMapping commandNameMapping)
        {
            if (commandNameMapping == null)
                throw new ArgumentNullException();

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
