namespace Tvl.VisualStudio.Language.Java.Debugger
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Debugger.Interop;
    using System.Runtime.InteropServices;
    using System.Diagnostics.Contracts;
    using Microsoft.VisualStudio;
    using Tvl.VisualStudio.Language.Java.Debugger.Extensions;
    using Tvl.Java.DebugInterface;
    using Tvl.VisualStudio.Language.Java.Debugger.Analysis;
    using System.Collections.ObjectModel;
    using Tvl.Java.DebugInterface.Types;

    [ComVisible(true)]
    public class JavaDebugDisassemblyStream : IDebugDisassemblyStream2
    {
        private readonly JavaDebugCodeContext _executionContext;
        private readonly byte[] _bytecode;
        private readonly int[] _instructionOffsets;

        private long _currentInstructionIndex;

        public JavaDebugDisassemblyStream(JavaDebugCodeContext executionContext)
        {
            Contract.Requires<ArgumentNullException>(executionContext != null, "executionContext");

            _executionContext = executionContext;
            _bytecode = _executionContext.Location.GetMethod().GetBytecodes();

            List<int> instructionOffsets = new List<int>();
            for (int i = 0; i < _bytecode.Length; /*increment in loop*/)
            {
                instructionOffsets.Add(i);
                int currentSize = JavaInstruction.InstructionLookup[_bytecode[i]].Size;
                if (currentSize == 0)
                    throw new NotImplementedException("Need to implement special support for variable-length instructions.");

                i += currentSize;
            }

            _instructionOffsets = instructionOffsets.ToArray();
        }

        #region IDebugDisassemblyStream2 Members

        /// <summary>
        /// Returns a code context object corresponding to a specified code location identifier.
        /// </summary>
        /// <param name="uCodeLocationId">[in] Specifies the code location identifier. See the Remarks section for the IDebugDisassemblyStream2.GetCodeLocationId method for a description of a code location identifier.</param>
        /// <param name="ppCodeContext">[out] Returns an IDebugCodeContext2 object that represents the associated code context.</param>
        /// <returns>If successful, returns S_OK; otherwise, returns an error code.</returns>
        /// <remarks>
        /// The code location identifier can be returned from a call to the IDebugDisassemblyStream2.GetCurrentLocation method and can appear in the DisassemblyData structure.
        /// 
        /// To convert a code context into a code location identifier, call the IDebugDisassemblyStream2.GetCodeLocationId method.
        /// </remarks>
        public int GetCodeContext(ulong uCodeLocationId, out IDebugCodeContext2 ppCodeContext)
        {
            ppCodeContext = null;
            if (uCodeLocationId > (uint)_bytecode.Length)
                return VSConstants.E_INVALIDARG;

            ppCodeContext = new JavaDebugCodeContext(_executionContext.Program, _executionContext.Location.GetMethod().GetLocationOfCodeIndex((long)uCodeLocationId));
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Returns a code location identifier for a particular code context.
        /// </summary>
        /// <param name="pCodeContext">[in] An IDebugCodeContext2 object to be converted to an identifier.</param>
        /// <param name="puCodeLocationId">[out] Returns the code location identifier. See Remarks.</param>
        /// <returns>If successful, returns S_OK; otherwise, returns an error code. Returns E_CODE_CONTEXT_OUT_OF_SCOPE if the code context is valid but outside the scope.</returns>
        /// <remarks>
        /// The code location identifier is specific to the debug engine (DE) supporting the disassembly. This
        /// location identifier is used internally by the DE to track positions in the code and is typically an
        /// address or offset of some kind. The only requirement is that if the code context of one location is
        /// less than the code context of another location, then the corresponding code location identifier of
        /// the first code context must also be less than the code location identifier of the second code context.
        ///
        /// To retrieve the code context of a code location identifier, call the IDebugDisassemblyStream2.GetCodeContext method.
        /// </remarks>
        public int GetCodeLocationId(IDebugCodeContext2 pCodeContext, out ulong puCodeLocationId)
        {
            puCodeLocationId = 0;

            JavaDebugCodeContext codeContext = pCodeContext as JavaDebugCodeContext;
            if (codeContext == null)
                return VSConstants.E_INVALIDARG;
            else if (!codeContext.Location.GetMethod().Equals(this._executionContext.Location.GetMethod()))
                return AD7Constants.E_CODE_CONTEXT_OUT_OF_SCOPE;

            puCodeLocationId = checked((ulong)codeContext.Location.GetCodeIndex());
            return VSConstants.S_OK;
        }

        public int GetCurrentLocation(out ulong puCodeLocationId)
        {
            puCodeLocationId = (ulong)_executionContext.Location.GetCodeIndex();
            return VSConstants.S_OK;
        }

        public int GetDocument(string bstrDocumentUrl, out IDebugDocument2 ppDocument)
        {
            ppDocument = null;

            IDebugDocumentContext2 documentContext;
            int result = _executionContext.GetDocumentContext(out documentContext);
            if (ErrorHandler.Failed(result))
                return result;

            return documentContext.GetDocument(out ppDocument);
        }

        public int GetScope(enum_DISASSEMBLY_STREAM_SCOPE[] pdwScope)
        {
            if (pdwScope == null)
                throw new ArgumentNullException("pdwScope");
            if (pdwScope.Length == 0)
                throw new ArgumentException();

            pdwScope[0] = enum_DISASSEMBLY_STREAM_SCOPE.DSS_FUNCTION;
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Gets the size in instructions of this disassembly stream.
        /// </summary>
        /// <param name="pnSize">[out] Returns the size, in instructions.</param>
        /// <returns>If successful, returns S_OK; otherwise, returns an error code.</returns>
        /// <remarks>
        /// The value returned from this method can be used to allocate an array of DisassemblyData structures which is then passed to the IDebugDisassemblyStream2.Read method.
        /// </remarks>
        public int GetSize(out ulong pnSize)
        {
            if (_bytecode == null)
            {
                pnSize = 0;
                return AD7Constants.S_GETSIZE_NO_SIZE;
            }

            pnSize = (uint)_instructionOffsets.Length;
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Reads instructions starting from the current position in the disassembly stream.
        /// </summary>
        /// <param name="dwInstructions">[in] The number of instructions to disassemble. This value is also the maximum length of the prgDisassembly array.</param>
        /// <param name="dwFields">[in] A combination of flags from the DISASSEMBLY_STREAM_FIELDS enumeration that indicate which fields of prgDisassembly are to be filled out.</param>
        /// <param name="pdwInstructionsRead">[out] Returns the number of instructions actually disassembled.</param>
        /// <param name="prgDisassembly">[out] An array of DisassemblyData structures that is filled in with the disassembled code, one structure per disassembled instruction. The length of this array is dictated by the dwInstructions parameter.</param>
        /// <returns>If successful, returns S_OK; otherwise, returns an error code.</returns>
        /// <remarks>
        /// The maximum number of instructions that are available in the current scope can be obtained by calling the IDebugDisassemblyStream2.GetSize method.
        /// 
        /// The current position where the next instruction is read from can be changed by calling the IDebugDisassemblyStream2.Seek method.
        /// 
        /// The DSF_OPERANDS_SYMBOLS flag can be added to the DSF_OPERANDS flag in the dwFields parameter to indicate that symbol names should be used when disassembling instructions.
        /// </remarks>
        public int Read(uint dwInstructions, enum_DISASSEMBLY_STREAM_FIELDS dwFields, out uint pdwInstructionsRead, DisassemblyData[] prgDisassembly)
        {
            pdwInstructionsRead = 0;

            uint actualInstructions = Math.Min(dwInstructions, (uint)(_instructionOffsets.Length - _currentInstructionIndex));

            if (prgDisassembly == null || prgDisassembly.Length < dwInstructions)
                return VSConstants.E_INVALIDARG;

            ReadOnlyCollection<ILocalVariable> localVariables = _executionContext.Location.GetMethod().GetVariables();
            ReadOnlyCollection<ConstantPoolEntry> constantPool = _executionContext.Location.GetDeclaringType().GetConstantPool();

            for (long i = 0; i < actualInstructions; i++)
            {
                int instructionStart = _instructionOffsets[_currentInstructionIndex + i];
                int instructionLength = _bytecode.Length - instructionStart;
                if (_currentInstructionIndex + i < _instructionOffsets.Length - 1)
                    instructionLength = _instructionOffsets[_currentInstructionIndex + i + 1] - instructionStart;

                JavaInstruction instruction = JavaInstruction.InstructionLookup[_bytecode[instructionStart]];

                if (dwFields.GetAddress())
                {
                    prgDisassembly[i].bstrAddress = string.Format("{0," + 4 + "}", instructionStart);
                    prgDisassembly[i].dwFields |= enum_DISASSEMBLY_STREAM_FIELDS.DSF_ADDRESS;
                }

                if (dwFields.GetCodeBytes())
                {
                    prgDisassembly[i].bstrCodeBytes = string.Join(" ", _bytecode.Skip(instructionStart).Take(instructionLength).Select(x => x.ToString("X2")));
                    prgDisassembly[i].dwFields |= enum_DISASSEMBLY_STREAM_FIELDS.DSF_CODEBYTES;
                }

                if (dwFields.GetCodeLocationId())
                {
                    prgDisassembly[i].uCodeLocationId = (ulong)instructionStart;
                    prgDisassembly[i].dwFields |= enum_DISASSEMBLY_STREAM_FIELDS.DSF_CODELOCATIONID;
                }

                if (dwFields.GetOpCode())
                {
                    prgDisassembly[i].bstrOpcode = instruction.Name ?? "???";
                    prgDisassembly[i].dwFields |= enum_DISASSEMBLY_STREAM_FIELDS.DSF_OPCODE;
                }

                if (dwFields.GetOperands())
                {
                    prgDisassembly[i].dwFields |= enum_DISASSEMBLY_STREAM_FIELDS.DSF_OPERANDS;

                    // operand 0
                    switch (instruction.OperandType)
                    {
                    case JavaOperandType.InlineI1:
                        prgDisassembly[i].bstrOperands = _bytecode[instructionStart + 1].ToString();
                        break;

                    case JavaOperandType.InlineI2:
                        prgDisassembly[i].bstrOperands = ((_bytecode[instructionStart + 1] << 8) + _bytecode[instructionStart + 2]).ToString();
                        break;

                    case JavaOperandType.InlineShortBranchTarget:
                        prgDisassembly[i].bstrOperands = (instructionStart + (short)(_bytecode[instructionStart + 1] << 8) + _bytecode[instructionStart + 2]).ToString();
                        break;

                    case JavaOperandType.InlineBranchTarget:
                        prgDisassembly[i].bstrOperands = (instructionStart + (int)(_bytecode[instructionStart + 1] << 24) + (_bytecode[instructionStart + 2] << 16) + (_bytecode[instructionStart + 3] << 8) + _bytecode[instructionStart + 4]).ToString();
                        break;

                    case JavaOperandType.InlineSwitch:
                        prgDisassembly[i].bstrOperands = "Switch?";
                        break;

                    case JavaOperandType.InlineTableSwitch:
                        prgDisassembly[i].bstrOperands = "TableSwitch?";
                        break;

                    case JavaOperandType.InlineShortConst:
                        prgDisassembly[i].bstrOperands = "#" + _bytecode[instructionStart + 1].ToString();
                        break;

                    case JavaOperandType.InlineConst:
                        prgDisassembly[i].bstrOperands = "#" + ((_bytecode[instructionStart + 1] << 8) + _bytecode[instructionStart + 2]).ToString();
                        break;

                    case JavaOperandType.InlineVar:
                    case JavaOperandType.InlineVar_I1:
                        prgDisassembly[i].bstrOperands = "#" + _bytecode[instructionStart + 1].ToString();
                        break;

                    case JavaOperandType.InlineField:
                    case JavaOperandType.InlineMethod:
                    case JavaOperandType.InlineMethod_I1_0:
                    case JavaOperandType.InlineType:
                    case JavaOperandType.InlineType_I1:
                        prgDisassembly[i].bstrOperands = "#" + ((_bytecode[instructionStart + 1] << 8) + _bytecode[instructionStart + 2]).ToString();
                        break;

                    case JavaOperandType.InlineArrayType:
                        prgDisassembly[i].bstrOperands = "T_" + ((JavaArrayType)_bytecode[instructionStart + 1]).ToString().ToUpperInvariant();
                        break;

                    default:
                        prgDisassembly[i].bstrOperands = string.Empty;
                        break;
                    }

                    // operand 1
                    switch (instruction.OperandType)
                    {
                    case JavaOperandType.InlineVar_I1:
                        prgDisassembly[i].bstrOperands += " " + _bytecode[instructionStart + 2].ToString();
                        break;

                    case JavaOperandType.InlineMethod_I1_0:
                    case JavaOperandType.InlineType_I1:
                        prgDisassembly[i].bstrOperands += " " + _bytecode[instructionStart + 3].ToString();
                        break;

                    default:
                        break;
                    }

                    // operand 2
                    if (instruction.OperandType == JavaOperandType.InlineMethod_I1_0)
                    {
                        prgDisassembly[i].bstrOperands += " 0";
                    }
                }

                if (dwFields.GetPosition())
                {
                    try
                    {
                        ILocation location = _executionContext.Location.GetMethod().GetLocationOfCodeIndex(instructionStart);
                        prgDisassembly[i].posBeg.dwLine = (uint)location.GetLineNumber();
                        prgDisassembly[i].posBeg.dwColumn = 0;
                        prgDisassembly[i].posEnd = prgDisassembly[i].posBeg;
                        prgDisassembly[i].posEnd.dwLine++;
                        prgDisassembly[i].dwFields |= enum_DISASSEMBLY_STREAM_FIELDS.DSF_POSITION;
                    }
                    catch (Exception e)
                    {
                        if (ErrorHandler.IsCriticalException(e))
                            throw;

                        prgDisassembly[i].posBeg = default(TEXT_POSITION);
                        prgDisassembly[i].posEnd = default(TEXT_POSITION);
                        prgDisassembly[i].dwFields &= ~enum_DISASSEMBLY_STREAM_FIELDS.DSF_POSITION;
                    }
                }

                if (dwFields.GetSymbol())
                {
                    switch (instruction.OperandType)
                    {
                    case JavaOperandType.InlineSwitch:
                        prgDisassembly[i].bstrSymbol = "// switch";
                        break;

                    case JavaOperandType.InlineTableSwitch:
                        prgDisassembly[i].bstrSymbol = "// table switch";
                        break;

                    case JavaOperandType.InlineShortConst:
                    case JavaOperandType.InlineConst:
                        {
                            int index = _bytecode[instructionStart + 1];
                            if (instruction.OperandType == JavaOperandType.InlineConst)
                                index = ((index << 8) + _bytecode[instructionStart + 2]);

                            var entry = constantPool[index - 1];
                            switch (entry.Type)
                            {
                            case ConstantType.Integer:
                            case ConstantType.Float:
                            case ConstantType.Long:
                            case ConstantType.Double:
                            case ConstantType.String:
                                prgDisassembly[i].bstrSymbol = "// " + entry.ToString(constantPool);
                                break;

                            default:
                                prgDisassembly[i].bstrSymbol = "// unknown const";
                                break;
                            }

                            break;
                        }

                    case JavaOperandType.InlineVar:
                    case JavaOperandType.InlineVar_I1:
                        {
                            int localIndex = _bytecode[instructionStart + 1];
                            int testLocation = instructionStart;
                            if (instruction.StackBehaviorPop != JavaStackBehavior.Pop0)
                            {
                                // this is a store instruction - the variable might not be visible until the following instruction
                                testLocation += instruction.Size;
                            }

                            ILocation currentLocation = _executionContext.Location.GetMethod().GetLocationOfCodeIndex(testLocation);
                            var local = localVariables.SingleOrDefault(variable => variable.GetSlot() == localIndex && variable.GetIsVisible(currentLocation));

                            if (local != null)
                            {
                                prgDisassembly[i].bstrSymbol = "// local";
                                prgDisassembly[i].bstrSymbol += string.Format(" {0} {1}", local.GetLocalTypeName(), local.GetName());
                            }

                            break;
                        }

                    case JavaOperandType.InlineField:
                    case JavaOperandType.InlineMethod:
                    case JavaOperandType.InlineMethod_I1_0:
                    case JavaOperandType.InlineType:
                    case JavaOperandType.InlineType_I1:
                        {
                            int index = (_bytecode[instructionStart + 1] << 8) + _bytecode[instructionStart + 2];
                            ConstantPoolEntry entry = constantPool[index - 1];
                            prgDisassembly[i].bstrSymbol = "// " + entry.ToString(constantPool);
                            break;
                        }

                    case JavaOperandType.InlineNone:
                        {
                            int? localIndex = null;

                            switch (instruction.OpCode)
                            {
                            case JavaOpCode.Aload_0:
                            case JavaOpCode.Astore_0:
                            case JavaOpCode.Dload_0:
                            case JavaOpCode.Dstore_0:
                            case JavaOpCode.Fload_0:
                            case JavaOpCode.Fstore_0:
                            case JavaOpCode.Iload_0:
                            case JavaOpCode.Istore_0:
                            case JavaOpCode.Lload_0:
                            case JavaOpCode.Lstore_0:
                                localIndex = 0;
                                break;

                            case JavaOpCode.Aload_1:
                            case JavaOpCode.Astore_1:
                            case JavaOpCode.Dload_1:
                            case JavaOpCode.Dstore_1:
                            case JavaOpCode.Fload_1:
                            case JavaOpCode.Fstore_1:
                            case JavaOpCode.Iload_1:
                            case JavaOpCode.Istore_1:
                            case JavaOpCode.Lload_1:
                            case JavaOpCode.Lstore_1:
                                localIndex = 1;
                                break;

                            case JavaOpCode.Aload_2:
                            case JavaOpCode.Astore_2:
                            case JavaOpCode.Dload_2:
                            case JavaOpCode.Dstore_2:
                            case JavaOpCode.Fload_2:
                            case JavaOpCode.Fstore_2:
                            case JavaOpCode.Iload_2:
                            case JavaOpCode.Istore_2:
                            case JavaOpCode.Lload_2:
                            case JavaOpCode.Lstore_2:
                                localIndex = 2;
                                break;

                            case JavaOpCode.Aload_3:
                            case JavaOpCode.Astore_3:
                            case JavaOpCode.Dload_3:
                            case JavaOpCode.Dstore_3:
                            case JavaOpCode.Fload_3:
                            case JavaOpCode.Fstore_3:
                            case JavaOpCode.Iload_3:
                            case JavaOpCode.Istore_3:
                            case JavaOpCode.Lload_3:
                            case JavaOpCode.Lstore_3:
                                localIndex = 3;
                                break;
                            }

                            if (localIndex.HasValue)
                            {
                                int testLocation = instructionStart;
                                if (instruction.StackBehaviorPop != JavaStackBehavior.Pop0)
                                {
                                    // this is a store instruction - the variable might not be visible until the following instruction
                                    testLocation += instruction.Size;
                                }

                                ILocation currentLocation = _executionContext.Location.GetMethod().GetLocationOfCodeIndex(testLocation);
                                var local = localVariables.SingleOrDefault(variable => variable.GetSlot() == localIndex && variable.GetIsVisible(currentLocation));

                                if (local != null)
                                {
                                    prgDisassembly[i].bstrSymbol = "// local";
                                    prgDisassembly[i].bstrSymbol += string.Format(" {0} {1}", local.GetLocalTypeName(), local.GetName());
                                }
                            }
                        }

                        break;

                    default:
                        break;
                    }

                    if (prgDisassembly[i].bstrSymbol != null)
                        prgDisassembly[i].dwFields |= enum_DISASSEMBLY_STREAM_FIELDS.DSF_SYMBOL;
                }
            }

            _currentInstructionIndex += actualInstructions;
            pdwInstructionsRead = actualInstructions;
            return actualInstructions == dwInstructions ? VSConstants.S_OK : VSConstants.S_FALSE;
        }

        /// <summary>
        /// Moves the read pointer in the disassembly stream a given number of instructions relative to a specified position.
        /// </summary>
        /// <param name="dwSeekStart">
        /// [in] A value from the SEEK_START enumeration that specifies the relative position to begin the seek process.
        /// </param>
        /// <param name="pCodeContext">
        /// [in] The IDebugCodeContext2 object representing the code context that the seek operation is relative to. This
        /// parameter is used only if dwSeekStart = SEEK_START_CODECONTEXT; otherwise, this parameter is ignored and can be
        /// a null value.
        /// </param>
        /// <param name="uCodeLocationId">
        /// [in] The code location identifier that the seek operation is relative to. This parameter is used if
        /// dwSeekStart = SEEK_START_CODELOCID; otherwise, this parameter is ignored and can be set to 0. See the Remarks
        /// section for the IDebugDisassemblyStream2.GetCodeLocationId method for a description of a code location identifier.
        /// </param>
        /// <param name="iInstructions">
        /// [in] The number of instructions to move relative to the position specified in dwSeekStart. This value can be
        /// negative to move backwards.
        /// </param>
        /// <returns>
        /// If successful, returns S_OK. Returns S_FALSE if the seek position was to a point beyond the list of available
        /// instructions. Otherwise, returns an error code.
        /// </returns>
        /// <remarks>
        /// If the seek was to a position before the beginning of the list, the read position is set to the first instruction
        /// in the list. If the see was to a position after the end of the list, the read position is set to the last
        /// instruction in the list.
        /// </remarks>
        public int Seek(enum_SEEK_START dwSeekStart, IDebugCodeContext2 pCodeContext, ulong uCodeLocationId, long iInstructions)
        {
            switch (dwSeekStart)
            {
            case enum_SEEK_START.SEEK_START_BEGIN:
                _currentInstructionIndex = 0;
                break;

            case enum_SEEK_START.SEEK_START_CODECONTEXT:
                int error = GetCodeLocationId(pCodeContext, out uCodeLocationId);
                if (!ErrorHandler.Succeeded(error))
                    return error;

                goto case enum_SEEK_START.SEEK_START_CODELOCID;

            case enum_SEEK_START.SEEK_START_CODELOCID:
                _currentInstructionIndex = Array.IndexOf(_instructionOffsets, (int)uCodeLocationId);
                if (_currentInstructionIndex < 0)
                    throw new ArgumentException();

                break;

            case enum_SEEK_START.SEEK_START_CURRENT:
                break;

            case enum_SEEK_START.SEEK_START_END:
                _currentInstructionIndex = _instructionOffsets.Length;
                break;

            default:
                throw new ArgumentException("Invalid seek start location.");
            }

            _currentInstructionIndex += iInstructions;
            if (_currentInstructionIndex >= 0 && _currentInstructionIndex <= _instructionOffsets.Length)
                return VSConstants.S_OK;

            _currentInstructionIndex = Math.Max(0, _currentInstructionIndex);
            _currentInstructionIndex = Math.Min(_instructionOffsets.Length, _currentInstructionIndex);
            return VSConstants.S_FALSE;
        }

        #endregion
    }
}
