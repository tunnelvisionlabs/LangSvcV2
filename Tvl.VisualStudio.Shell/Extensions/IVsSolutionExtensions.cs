namespace Tvl.VisualStudio.Shell.Extensions
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell.Interop;

    public static class IVsSolutionExtensions
    {
        public static IEnumerable<IVsHierarchy> GetProjectHierarchies(this IVsSolution solution)
        {
            return GetProjectHierarchies(solution, __VSENUMPROJFLAGS.EPF_LOADEDINSOLUTION);
        }

        public static IEnumerable<IVsHierarchy> GetProjectHierarchies(this IVsSolution solution, __VSENUMPROJFLAGS flags)
        {
            if (solution == null)
                throw new ArgumentNullException("solution");

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
