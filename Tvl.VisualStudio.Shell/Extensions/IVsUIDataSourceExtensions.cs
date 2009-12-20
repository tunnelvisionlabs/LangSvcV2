namespace Tvl.VisualStudio.Shell.Extensions
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell.Interop;

    public static class IVsUIDataSourceExtensions
    {
        private static IEnumerable<VsUIPropertyDescriptor> GetProperties(IVsUIDataSource dataSource)
        {
            if (dataSource == null)
                throw new ArgumentNullException();

            IVsUIEnumDataSourceProperties verbs;
            if (ErrorHandler.Succeeded(dataSource.EnumProperties(out verbs)))
            {
                VsUIPropertyDescriptor[] array = new VsUIPropertyDescriptor[1];
                while (true)
                {
                    uint count;
                    int hr = verbs.Next((uint)array.Length, array, out count);
                    ErrorHandler.ThrowOnFailure(hr);
                    if (hr == VSConstants.S_FALSE || count == 0)
                        break;

                    for (uint i = 0; i < count; i++)
                        yield return array[i];
                }
            }
        }

        private static IEnumerable<string> GetVerbs(IVsUIDataSource dataSource)
        {
            if (dataSource == null)
                throw new ArgumentNullException();

            IVsUIEnumDataSourceVerbs verbs;
            if (ErrorHandler.Succeeded(dataSource.EnumVerbs(out verbs)))
            {
                string[] array = new string[1];
                while (true)
                {
                    uint count;
                    int hr = verbs.Next((uint)array.Length, array, out count);
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
