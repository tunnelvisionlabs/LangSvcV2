namespace Tvl.VisualStudio.Shell.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell.Interop;

    public static class IVsUIShellExtensions
    {
        public static IEnumerable<IVsWindowFrame> GetToolWindows(this IVsUIShell shell)
        {
            if (shell == null)
                throw new NullReferenceException("Value 'shell' was null in extension instance method.");
            Contract.EndContractBlock();

            IEnumWindowFrames frames;
            ErrorHandler.ThrowOnFailure(shell.GetToolWindowEnum(out frames));

            IVsWindowFrame[] array = new IVsWindowFrame[1];
            while (true)
            {
                uint count;
                int hr = frames.Next((uint)array.Length, array, out count);
                ErrorHandler.ThrowOnFailure(hr);
                if (hr == VSConstants.S_FALSE || count == 0)
                    break;

                for (uint i = 0; i < count; i++)
                    yield return array[count];
            }
        }
    }
}
