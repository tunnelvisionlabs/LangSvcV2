namespace Tvl.VisualStudio.Shell
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell.Interop;

    public static class IVsSolutionExtensions
    {
        [NotNull]
        public static IEnumerable<IVsHierarchy> GetProjectHierarchies([NotNull] this IVsSolution solution)
        {
            Debug.Assert(solution != null);

            return GetProjectHierarchies(solution, __VSENUMPROJFLAGS.EPF_LOADEDINSOLUTION);
        }

        [NotNull]
        public static IEnumerable<IVsHierarchy> GetProjectHierarchies([NotNull] this IVsSolution solution, __VSENUMPROJFLAGS flags)
        {
            Requires.NotNull(solution, nameof(solution));

            Guid empty = Guid.Empty;
            IEnumHierarchies ppenum;
            if (ErrorHandler.Succeeded(solution.GetProjectEnum((uint)flags, ref empty, out ppenum)))
            {
                if (ppenum != null)
                {
                    IVsHierarchy[] rgelt = new IVsHierarchy[1];
                    uint celtFetched;
                    for (; ; )
                    {
                        int hr = ppenum.Next((uint)rgelt.Length, rgelt, out celtFetched);
                        ErrorHandler.ThrowOnFailure(hr);

                        for (int i = 0; i < celtFetched; i++)
                            yield return rgelt[i];

                        if (hr == VSConstants.S_FALSE)
                            yield break;
                    }
                }
            }
        }
    }
}
