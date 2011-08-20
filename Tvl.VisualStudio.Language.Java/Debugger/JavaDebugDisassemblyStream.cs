using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Debugger.Interop;
using System.Runtime.InteropServices;

namespace Tvl.VisualStudio.Language.Java.Debugger
{
    [ComVisible(true)]
    public class JavaDebugDisassemblyStream : IDebugDisassemblyStream2
    {
        #region IDebugDisassemblyStream2 Members

        public int GetCodeContext(ulong uCodeLocationId, out IDebugCodeContext2 ppCodeContext)
        {
            throw new NotImplementedException();
        }

        public int GetCodeLocationId(IDebugCodeContext2 pCodeContext, out ulong puCodeLocationId)
        {
            throw new NotImplementedException();
        }

        public int GetCurrentLocation(out ulong puCodeLocationId)
        {
            throw new NotImplementedException();
        }

        public int GetDocument(string bstrDocumentUrl, out IDebugDocument2 ppDocument)
        {
            throw new NotImplementedException();
        }

        public int GetScope(enum_DISASSEMBLY_STREAM_SCOPE[] pdwScope)
        {
            throw new NotImplementedException();
        }

        public int GetSize(out ulong pnSize)
        {
            throw new NotImplementedException();
        }

        public int Read(uint dwInstructions, enum_DISASSEMBLY_STREAM_FIELDS dwFields, out uint pdwInstructionsRead, DisassemblyData[] prgDisassembly)
        {
            throw new NotImplementedException();
        }

        public int Seek(enum_SEEK_START dwSeekStart, IDebugCodeContext2 pCodeContext, ulong uCodeLocationId, long iInstructions)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
