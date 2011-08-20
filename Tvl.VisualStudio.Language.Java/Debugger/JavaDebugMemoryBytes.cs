namespace Tvl.VisualStudio.Language.Java.Debugger
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Debugger.Interop;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public class JavaDebugMemoryBytes : IDebugMemoryBytes2
    {
        #region IDebugMemoryBytes2 Members

        public int GetSize(out ulong pqwSize)
        {
            throw new NotImplementedException();
        }

        public int ReadAt(IDebugMemoryContext2 pStartContext, uint dwCount, byte[] rgbMemory, out uint pdwRead, ref uint pdwUnreadable)
        {
            throw new NotImplementedException();
        }

        public int WriteAt(IDebugMemoryContext2 pStartContext, uint dwCount, byte[] rgbMemory)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
