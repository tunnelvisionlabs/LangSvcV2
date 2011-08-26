namespace Tvl.VisualStudio.Language.Java.Debugger
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Debugger.Interop;
    using System.Runtime.InteropServices;
    using Tvl.Java.DebugInterface;
    using System.Diagnostics.Contracts;
    using Microsoft.VisualStudio;

    [ComVisible(true)]
    public class JavaDebugCodeContext : IDebugCodeContext3, IDebugCodeContext2, IDebugMemoryContext2
    {
        private readonly JavaDebugProgram _program;
        private readonly ILocation _location;

        public JavaDebugCodeContext(JavaDebugProgram program, ILocation location)
        {
            Contract.Requires<ArgumentNullException>(program != null, "program");
            Contract.Requires<ArgumentNullException>(location != null, "location");
            _program = program;
            _location = location;
        }

        #region IDebugMemoryContext2 Members

        public int Add(ulong dwCount, out IDebugMemoryContext2 ppMemCxt)
        {
            throw new NotImplementedException();
        }

        public int Compare(enum_CONTEXT_COMPARE Compare, IDebugMemoryContext2[] rgpMemoryContextSet, uint dwMemoryContextSetLen, out uint pdwMemoryContext)
        {
            throw new NotImplementedException();
        }

        public int GetInfo(enum_CONTEXT_INFO_FIELDS dwFields, CONTEXT_INFO[] pinfo)
        {
            if (pinfo == null)
                throw new ArgumentNullException("pinfo");
            if (pinfo.Length < 1)
                throw new ArgumentException();

            bool getModuleUrl = (dwFields & enum_CONTEXT_INFO_FIELDS.CIF_MODULEURL) != 0;
            bool getFunction = (dwFields & enum_CONTEXT_INFO_FIELDS.CIF_FUNCTION) != 0;
            bool getFunctionOffset = (dwFields & enum_CONTEXT_INFO_FIELDS.CIF_FUNCTIONOFFSET) != 0;
            bool getAddress = (dwFields & enum_CONTEXT_INFO_FIELDS.CIF_ADDRESS) != 0;
            bool getAddressOffset = (dwFields & enum_CONTEXT_INFO_FIELDS.CIF_ADDRESSOFFSET) != 0;
            bool getAddressAbsolute = (dwFields & enum_CONTEXT_INFO_FIELDS.CIF_ADDRESSABSOLUTE) != 0;

            if (getModuleUrl)
            {
                // The name of the module where the context is located
                //pinfo[0].bstrModuleUrl = "";
                //pinfo[0].dwFields |= enum_CONTEXT_INFO_FIELDS.CIF_MODULEURL;
            }

            if (getFunction)
            {
                // The function name where the context is located
                pinfo[0].bstrFunction = _location.GetMethod().GetName();
                pinfo[0].dwFields |= enum_CONTEXT_INFO_FIELDS.CIF_FUNCTION;
            }

            if (getFunctionOffset)
            {
                // A TEXT_POSITION structure that identifies the line and column offset of the function associated with the code context
                pinfo[0].posFunctionOffset.dwLine = (uint)_location.GetLineNumber();
                pinfo[0].posFunctionOffset.dwColumn = 0;
                pinfo[0].dwFields |= enum_CONTEXT_INFO_FIELDS.CIF_FUNCTIONOFFSET;
            }

            if (getAddress)
            {
                // The address in code where the given context is located
                //pinfo[0].bstrAddress = "";
                //pinfo[0].dwFields |= enum_CONTEXT_INFO_FIELDS.CIF_ADDRESS;
            }

            if (getAddressOffset)
            {
                // The offset of the address in code where the given context is located
                //pinfo[0].bstrAddressOffset = "";
                //pinfo[0].dwFields |= enum_CONTEXT_INFO_FIELDS.CIF_ADDRESSOFFSET;
            }

            if (getAddressAbsolute)
            {
                // The absolute address in memory where the given context is located
                //pinfo[0].bstrAddressAbsolute = "";
                //pinfo[0].dwFields |= enum_CONTEXT_INFO_FIELDS.CIF_ADDRESSABSOLUTE;
            }

            return VSConstants.S_OK;
        }

        public int GetName(out string pbstrName)
        {
            throw new NotImplementedException();
        }

        public int Subtract(ulong dwCount, out IDebugMemoryContext2 ppMemCxt)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDebugCodeContext2 Members

        public int GetDocumentContext(out IDebugDocumentContext2 ppSrcCxt)
        {
            ppSrcCxt = new JavaDebugDocumentContext(_location);
            return VSConstants.S_OK;
        }

        public int GetLanguageInfo(ref string pbstrLanguage, ref Guid pguidLanguage)
        {
            pbstrLanguage = Constants.JavaLanguageName;
            pguidLanguage = Constants.JavaLanguageGuid;
            return VSConstants.S_OK;
        }

        #endregion

        #region IDebugCodeContext3 Members

        public int GetModule(out IDebugModule2 ppModule)
        {
            // TODO: implement modules?
            ppModule = null;
            return VSConstants.S_OK;
        }

        public int GetProcess(out IDebugProcess2 ppProcess)
        {
            ppProcess = _program.Process;
            return VSConstants.S_OK;
        }

        #endregion
    }
}
