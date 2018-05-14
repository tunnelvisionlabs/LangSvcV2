namespace Tvl.VisualStudio.Shell
{
    using System.Collections.Generic;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell.Interop;

    public static class IVsUIDataSourceExtensions
    {
        [NotNull]
        private static IEnumerable<VsUIPropertyDescriptor> GetProperties([NotNull] IVsUIDataSource dataSource)
        {
            Requires.NotNull(dataSource, nameof(dataSource));

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

        [NotNull]
        private static IEnumerable<string> GetVerbs([NotNull] IVsUIDataSource dataSource)
        {
            Requires.NotNull(dataSource, nameof(dataSource));

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
