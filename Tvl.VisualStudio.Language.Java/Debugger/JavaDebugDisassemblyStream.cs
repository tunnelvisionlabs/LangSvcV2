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
                int currentSize = OpCodeSizes[(JavaOpCode)_bytecode[i]];
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

            for (long i = 0; i < actualInstructions; i++)
            {
                int instructionStart = _instructionOffsets[_currentInstructionIndex + i];
                int instructionLength = _bytecode.Length - instructionStart;
                if (_currentInstructionIndex + i < _instructionOffsets.Length - 1)
                    instructionLength = _instructionOffsets[_currentInstructionIndex + i + 1] - instructionStart;

                if (dwFields.GetAddress())
                {
                    prgDisassembly[i].bstrAddress = instructionStart.ToString("X8");
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
                    prgDisassembly[i].bstrOpcode = ((JavaOpCode)_bytecode[instructionStart]).ToString();
                    prgDisassembly[i].dwFields |= enum_DISASSEMBLY_STREAM_FIELDS.DSF_OPCODE;
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

        private readonly Dictionary<JavaOpCode, int> OpCodeSizes = new Dictionary<JavaOpCode, int>()
            {
                { JavaOpCode.aaload, 1 },
                { JavaOpCode.aastore, 1 },
                { JavaOpCode.aconst_null, 1 },
                { JavaOpCode.aload, 2 },
                { JavaOpCode.aload_0, 1 },
                { JavaOpCode.aload_1, 1 },
                { JavaOpCode.aload_2, 1 },
                { JavaOpCode.aload_3, 1 },
                { JavaOpCode.anewarray, 3 },
                { JavaOpCode.areturn, 1 },
                { JavaOpCode.arraylength, 1 },
                { JavaOpCode.astore, 2 },
                { JavaOpCode.astore_0, 1 },
                { JavaOpCode.astore_1, 1 },
                { JavaOpCode.astore_2, 1 },
                { JavaOpCode.astore_3, 1 },
                { JavaOpCode.athrow, 1 },
                { JavaOpCode.baload, 1 },
                { JavaOpCode.bastore, 1 },
                { JavaOpCode.bipush, 2 },
                { JavaOpCode.breakpoint, 1 },
                { JavaOpCode.caload, 1 },
                { JavaOpCode.castore, 1 },
                { JavaOpCode.checkcast, 3 },
                { JavaOpCode.d2f, 1 },
                { JavaOpCode.d2i, 1 },
                { JavaOpCode.d2l, 1 },
                { JavaOpCode.dadd, 1 },
                { JavaOpCode.daload, 1 },
                { JavaOpCode.dastore, 1 },
                { JavaOpCode.dcmpg, 1 },
                { JavaOpCode.dcmpl, 1 },
                { JavaOpCode.dconst_0, 1 },
                { JavaOpCode.dconst_1, 1 },
                { JavaOpCode.ddiv, 1 },
                { JavaOpCode.dload, 2 },
                { JavaOpCode.dload_0, 1 },
                { JavaOpCode.dload_1, 1 },
                { JavaOpCode.dload_2, 1 },
                { JavaOpCode.dload_3, 1 },
                { JavaOpCode.dmul, 1 },
                { JavaOpCode.dneg, 1 },
                { JavaOpCode.drem, 1 },
                { JavaOpCode.dreturn, 1 },
                { JavaOpCode.dstore, 2 },
                { JavaOpCode.dstore_0, 1 },
                { JavaOpCode.dstore_1, 1 },
                { JavaOpCode.dstore_2, 1 },
                { JavaOpCode.dstore_3, 1 },
                { JavaOpCode.dsub, 1 },
                { JavaOpCode.dup, 1 },
                { JavaOpCode.dup_x1, 1 },
                { JavaOpCode.dup_x2, 1 },
                { JavaOpCode.dup2, 1 },
                { JavaOpCode.dup2_x1, 1 },
                { JavaOpCode.dup2_x2, 1 },
                { JavaOpCode.f2d, 1 },
                { JavaOpCode.f2i, 1 },
                { JavaOpCode.f2l, 1 },
                { JavaOpCode.fadd, 1 },
                { JavaOpCode.faload, 1 },
                { JavaOpCode.fastore, 1 },
                { JavaOpCode.fcmpg, 1 },
                { JavaOpCode.fcmpl, 1 },
                { JavaOpCode.fconst_0, 1 },
                { JavaOpCode.fconst_1, 1 },
                { JavaOpCode.fconst_2, 1 },
                { JavaOpCode.fdiv, 1 },
                { JavaOpCode.fload, 2 },
                { JavaOpCode.fload_0, 1 },
                { JavaOpCode.fload_1, 1 },
                { JavaOpCode.fload_2, 1 },
                { JavaOpCode.fload_3, 1 },
                { JavaOpCode.fmul, 1 },
                { JavaOpCode.fneg, 1 },
                { JavaOpCode.frem, 1 },
                { JavaOpCode.freturn, 1 },
                { JavaOpCode.fstore, 2 },
                { JavaOpCode.fstore_0, 1 },
                { JavaOpCode.fstore_1, 1 },
                { JavaOpCode.fstore_2, 1 },
                { JavaOpCode.fstore_3, 1 },
                { JavaOpCode.fsub, 1 },
                { JavaOpCode.getfield, 3 },
                { JavaOpCode.getstatic, 3 },
                { JavaOpCode.@goto, 3 },
                { JavaOpCode.goto_w, 5 },
                { JavaOpCode.i2b, 1 },
                { JavaOpCode.i2c, 1 },
                { JavaOpCode.i2d, 1 },
                { JavaOpCode.i2f, 1 },
                { JavaOpCode.i2l, 1 },
                { JavaOpCode.i2s, 1 },
                { JavaOpCode.iadd, 1 },
                { JavaOpCode.iaload, 1 },
                { JavaOpCode.iand, 1 },
                { JavaOpCode.iastore, 1 },
                { JavaOpCode.iconst_0, 1 },
                { JavaOpCode.iconst_1, 1 },
                { JavaOpCode.iconst_2, 1 },
                { JavaOpCode.iconst_3, 1 },
                { JavaOpCode.iconst_4, 1 },
                { JavaOpCode.iconst_5, 1 },
                { JavaOpCode.iconst_m1, 1 },
                { JavaOpCode.idiv, 1 },
                { JavaOpCode.if_acmpeq, 3 },
                { JavaOpCode.if_acmpne, 3 },
                { JavaOpCode.if_icmpeq, 3 },
                { JavaOpCode.if_icmpge, 3 },
                { JavaOpCode.if_icmpgt, 3 },
                { JavaOpCode.if_icmple, 3 },
                { JavaOpCode.if_icmplt, 3 },
                { JavaOpCode.if_icmpne, 3 },
                { JavaOpCode.ifeq, 3 },
                { JavaOpCode.ifge, 3 },
                { JavaOpCode.ifgt, 3 },
                { JavaOpCode.ifle, 3 },
                { JavaOpCode.iflt, 3 },
                { JavaOpCode.ifne, 3 },
                { JavaOpCode.ifnonnull, 3 },
                { JavaOpCode.ifnull, 3 },
                { JavaOpCode.iinc, 3 },
                { JavaOpCode.iload, 2 },
                { JavaOpCode.iload_0, 1 },
                { JavaOpCode.iload_1, 1 },
                { JavaOpCode.iload_2, 1 },
                { JavaOpCode.iload_3, 1 },
                { JavaOpCode.impdep1, 1 },
                { JavaOpCode.impdep2, 1 },
                { JavaOpCode.imul, 1 },
                { JavaOpCode.ineg, 1 },
                { JavaOpCode.instanceof, 3 },
                { JavaOpCode.invokeinterface, 5 },
                { JavaOpCode.invokespecial, 3 },
                { JavaOpCode.invokestatic, 3 },
                { JavaOpCode.invokevirtual, 3 },
                { JavaOpCode.ior, 1 },
                { JavaOpCode.irem, 1 },
                { JavaOpCode.ireturn, 1 },
                { JavaOpCode.ishl, 1 },
                { JavaOpCode.ishr, 1 },
                { JavaOpCode.istore, 2 },
                { JavaOpCode.istore_0, 1 },
                { JavaOpCode.istore_1, 1 },
                { JavaOpCode.istore_2, 1 },
                { JavaOpCode.istore_3, 1 },
                { JavaOpCode.isub, 1 },
                { JavaOpCode.iushr, 1 },
                { JavaOpCode.ixor, 1 },
                { JavaOpCode.jsr, 3 },
                { JavaOpCode.jsr_w, 5 },
                { JavaOpCode.l2d, 1 },
                { JavaOpCode.l2f, 1 },
                { JavaOpCode.l2i, 1 },
                { JavaOpCode.ladd, 1 },
                { JavaOpCode.laload, 1 },
                { JavaOpCode.land, 1 },
                { JavaOpCode.lastore, 1 },
                { JavaOpCode.lcmp, 1 },
                { JavaOpCode.lconst_0, 1 },
                { JavaOpCode.lconst_1, 1 },
                { JavaOpCode.ldc, 2 },
                { JavaOpCode.ldc_w, 3 },
                { JavaOpCode.ldc2_w, 3 },
                { JavaOpCode.ldiv, 1 },
                { JavaOpCode.lload, 2 },
                { JavaOpCode.lload_0, 1 },
                { JavaOpCode.lload_1, 1 },
                { JavaOpCode.lload_2, 1 },
                { JavaOpCode.lload_3, 1 },
                { JavaOpCode.lmul, 1 },
                { JavaOpCode.lneg, 1 },
                { JavaOpCode.lookupswitch, 0 },
                { JavaOpCode.lor, 1 },
                { JavaOpCode.lrem, 1 },
                { JavaOpCode.lreturn, 1 },
                { JavaOpCode.lshl, 1 },
                { JavaOpCode.lshr, 1 },
                { JavaOpCode.lstore, 2 },
                { JavaOpCode.lstore_0, 1 },
                { JavaOpCode.lstore_1, 1 },
                { JavaOpCode.lstore_2, 1 },
                { JavaOpCode.lstore_3, 1 },
                { JavaOpCode.lsub, 1 },
                { JavaOpCode.lushr, 1 },
                { JavaOpCode.lxor, 1 },
                { JavaOpCode.monitorenter, 1 },
                { JavaOpCode.monitorexit, 1 },
                { JavaOpCode.multianewarray, 4 },
                { JavaOpCode.@new, 3 },
                { JavaOpCode.newarray, 2 },
                { JavaOpCode.nop, 1 },
                { JavaOpCode.pop, 1 },
                { JavaOpCode.pop2, 1 },
                { JavaOpCode.putfield, 3 },
                { JavaOpCode.putstatic, 3 },
                { JavaOpCode.ret, 2 },
                { JavaOpCode.@return, 1 },
                { JavaOpCode.saload, 1 },
                { JavaOpCode.sastore, 1 },
                { JavaOpCode.sipush, 3 },
                { JavaOpCode.swap, 1 },
                { JavaOpCode.tableswitch, 0 },
                { JavaOpCode.wide, 0 },
                { JavaOpCode.xxxunusedxxx1, 1 },
            };
    }

    public struct JavaInstruction
    {
        /// <summary>
        /// Load reference from array
        /// </summary>
        public static readonly JavaInstruction aaload;

        /// <summary>
        /// Store into reference array
        /// </summary>
        public static readonly JavaInstruction aastore;

        public static readonly JavaInstruction aconst_null;
        public static readonly JavaInstruction aload;
        public static readonly JavaInstruction aload_0;
        public static readonly JavaInstruction aload_1;
        public static readonly JavaInstruction aload_2;
        public static readonly JavaInstruction aload_3;
        public static readonly JavaInstruction anewarray;
        public static readonly JavaInstruction areturn;
        public static readonly JavaInstruction arraylength;
        public static readonly JavaInstruction astore;
        public static readonly JavaInstruction astore_0;
        public static readonly JavaInstruction astore_1;
        public static readonly JavaInstruction astore_2;
        public static readonly JavaInstruction astore_3;
        public static readonly JavaInstruction athrow;

        public static readonly JavaInstruction baload;
        public static readonly JavaInstruction bastore;
        public static readonly JavaInstruction bipush;

        public static readonly JavaInstruction caload;
        public static readonly JavaInstruction castore;
        public static readonly JavaInstruction checkcast;

        public static readonly JavaInstruction d2f;
        public static readonly JavaInstruction d2i;
        public static readonly JavaInstruction d2l;
        public static readonly JavaInstruction dadd;
        public static readonly JavaInstruction daload;
        public static readonly JavaInstruction dastore;
        public static readonly JavaInstruction dcmpg;
        public static readonly JavaInstruction dcmpl;
        public static readonly JavaInstruction dconst_0;
        public static readonly JavaInstruction dconst_1;
        public static readonly JavaInstruction ddiv;
        public static readonly JavaInstruction dload;
        public static readonly JavaInstruction dload_0;
        public static readonly JavaInstruction dload_1;
        public static readonly JavaInstruction dload_2;
        public static readonly JavaInstruction dload_3;
        public static readonly JavaInstruction dmul;
        public static readonly JavaInstruction dneg;
        public static readonly JavaInstruction drem;
        public static readonly JavaInstruction dreturn;
        public static readonly JavaInstruction dstore;
        public static readonly JavaInstruction dstore_0;
        public static readonly JavaInstruction dstore_1;
        public static readonly JavaInstruction dstore_2;
        public static readonly JavaInstruction dstore_3;
        public static readonly JavaInstruction dsub;
        public static readonly JavaInstruction dup;

        public readonly JavaOpCode OpCode;
        public readonly string Name;
        public readonly JavaOperandType OperandType;
        public readonly JavaFlowControl FlowControl;
        public readonly int Size;
        public readonly JavaStackBehaviorPop StackBehaviorPop;
        public readonly JavaStackBehaviorPush StackBehaviorPush;

        public JavaInstruction(JavaOpCode opcode, string name, JavaOperandType operandType, JavaFlowControl flowControl, int size, JavaStackBehaviorPop stackBehaviorPop, JavaStackBehaviorPush stackBehaviorPush)
        {
            OpCode = opcode;
            Name = name;
            OperandType = operandType;
            FlowControl = flowControl;
            Size = size;
            StackBehaviorPop = stackBehaviorPop;
            StackBehaviorPush = stackBehaviorPush;
        }

        static JavaInstruction()
        {
            aaload = new JavaInstruction(JavaOpCode.aaload, "aaload", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehaviorPop.PopRef_PopI, JavaStackBehaviorPush.Push1);
            aastore = new JavaInstruction(JavaOpCode.aastore, "aastore", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehaviorPop.PopRef_PopI_Pop1, JavaStackBehaviorPush.Push0);
            aconst_null = new JavaInstruction(JavaOpCode.aconst_null, "aconst_null", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehaviorPop.Pop0, JavaStackBehaviorPush.PushRef);
            aload = new JavaInstruction(JavaOpCode.aload, "aload", JavaOperandType.InlineShortVar, JavaFlowControl.Next, 2, JavaStackBehaviorPop.Pop0, JavaStackBehaviorPush.PushRef);
            aload_0 = new JavaInstruction(JavaOpCode.aload_0, "aload_0", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehaviorPop.Pop0, JavaStackBehaviorPush.PushRef);
            aload_1 = new JavaInstruction(JavaOpCode.aload_1, "aload_1", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehaviorPop.Pop0, JavaStackBehaviorPush.PushRef);
            aload_2 = new JavaInstruction(JavaOpCode.aload_2, "aload_2", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehaviorPop.Pop0, JavaStackBehaviorPush.PushRef);
            aload_3 = new JavaInstruction(JavaOpCode.aload_3, "aload_3", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehaviorPop.Pop0, JavaStackBehaviorPush.PushRef);
            anewarray = new JavaInstruction(JavaOpCode.anewarray, "anewarray", JavaOperandType.InlineType, JavaFlowControl.Next, 3, JavaStackBehaviorPop.PopI, JavaStackBehaviorPush.PushRef);
            areturn = new JavaInstruction(JavaOpCode.areturn, "areturn", JavaOperandType.InlineNone, JavaFlowControl.Return, 1, JavaStackBehaviorPop.PopRef, JavaStackBehaviorPush.Push0);
            arraylength = new JavaInstruction(JavaOpCode.arraylength, "arraylength", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehaviorPop.PopRef, JavaStackBehaviorPush.PushI);
            astore = new JavaInstruction(JavaOpCode.astore, "astore", JavaOperandType.InlineShortVar, JavaFlowControl.Next, 2, JavaStackBehaviorPop.PopRef, JavaStackBehaviorPush.Push0);
            astore_0 = new JavaInstruction(JavaOpCode.astore_0, "astore_0", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehaviorPop.PopRef, JavaStackBehaviorPush.Push0);
            astore_1 = new JavaInstruction(JavaOpCode.astore_1, "astore_1", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehaviorPop.PopRef, JavaStackBehaviorPush.Push0);
            astore_2 = new JavaInstruction(JavaOpCode.astore_2, "astore_2", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehaviorPop.PopRef, JavaStackBehaviorPush.Push0);
            astore_3 = new JavaInstruction(JavaOpCode.astore_3, "astore_3", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehaviorPop.PopRef, JavaStackBehaviorPush.Push0);
            athrow = new JavaInstruction(JavaOpCode.athrow, "athrow", JavaOperandType.InlineNone, JavaFlowControl.Throw, 1, JavaStackBehaviorPop.PopRef, JavaStackBehaviorPush.PushRef);

            baload = new JavaInstruction(JavaOpCode.baload, "baload", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehaviorPop.PopRef_PopI, JavaStackBehaviorPush.PushI);
            bastore = new JavaInstruction(JavaOpCode.bastore, "bastore", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehaviorPop.PopRef_PopI_PopI, JavaStackBehaviorPush.Push0);
            bipush = new JavaInstruction(JavaOpCode.bipush, "bipush", JavaOperandType.InlineI1, JavaFlowControl.Next, 2, JavaStackBehaviorPop.Pop0, JavaStackBehaviorPush.PushI);

            caload = new JavaInstruction(JavaOpCode.caload, "caload", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehaviorPop.PopRef_PopI, JavaStackBehaviorPush.PushI);
            castore = new JavaInstruction(JavaOpCode.castore, "castore", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehaviorPop.PopRef_PopI_PopI, JavaStackBehaviorPush.Push0);
            checkcast = new JavaInstruction(JavaOpCode.checkcast, "checkcast", JavaOperandType.InlineType, JavaFlowControl.Next, 3, JavaStackBehaviorPop.PopRef, JavaStackBehaviorPush.PushRef);
        }
    }

    public enum JavaStackBehaviorPop
    {
        Pop0,
        Pop1,
        Pop1_Pop1,
        PopI,
        PopI_Pop1,
        PopI_PopI,
        PopI_PopI8,
        PopI_PopI_PopI,
        PopI_PopR4,
        PopI_PopR8,
        PopRef,
        PopRef_Pop1,
        PopRef_PopI,
        PopRef_PopI_PopI,
        PopRef_PopI_PopI8,
        PopRef_PopI_PopR4,
        PopRef_PopI_PopR8,
        PopRef_PopI_PopRef,
        PopRef_PopI_Pop1,
        VarPop,
    }

    public enum JavaStackBehaviorPush
    {
        Push0,
        Push1,
        Push1_Push1,
        PushI,
        PushI8,
        PushR4,
        PushR8,
        PushRef,
        VarPush,
    }

    public enum JavaFlowControl
    {
        Next,
        Branch,
        Break,
        Call,
        ConditionalBranch,
        Return,
        Throw,
        //Meta,
    }

    public enum JavaOperandType
    {
        InlineNone,

        InlineI1,
        InlineI4,
        InlineI8,
        InlineR4,
        InlineR8,

        InlineShortBranchTarget,
        InlineBranchTarget,

        InlineShortVar,
        InlineField,
        InlineMethod,
        InlineType,
    }

    public enum JavaOpCode : byte
    {
        nop = 0x00,
        aconst_null = 0x01,
        iconst_m1 = 0x02,
        iconst_0 = 0x03,
        iconst_1 = 0x04,
        iconst_2 = 0x05,
        iconst_3 = 0x06,
        iconst_4 = 0x07,
        iconst_5 = 0x08,
        lconst_0 = 0x09,
        lconst_1 = 0x0A,
        fconst_0 = 0x0B,
        fconst_1 = 0x0C,
        fconst_2 = 0x0D,
        dconst_0 = 0x0E,
        dconst_1 = 0x0F,
        bipush = 0x10,
        sipush = 0x11,
        ldc = 0x12,
        ldc_w = 0x13,
        ldc2_w = 0x14,
        iload = 0x15,
        lload = 0x16,
        fload = 0x17,
        dload = 0x18,
        aload = 0x19,
        iload_0 = 0x1A,
        iload_1 = 0x1B,
        iload_2 = 0x1C,
        iload_3 = 0x1D,
        lload_0 = 0x1E,
        lload_1 = 0x1F,
        lload_2 = 0x20,
        lload_3 = 0x21,
        fload_0 = 0x22,
        fload_1 = 0x23,
        fload_2 = 0x24,
        fload_3 = 0x25,
        dload_0 = 0x26,
        dload_1 = 0x27,
        dload_2 = 0x28,
        dload_3 = 0x29,
        aload_0 = 0x2A,
        aload_1 = 0x2B,
        aload_2 = 0x2C,
        aload_3 = 0x2D,
        iaload = 0x2E,
        laload = 0x2F,
        faload = 0x30,
        daload = 0x31,
        aaload = 0x32,
        baload = 0x33,
        caload = 0x34,
        saload = 0x35,
        istore = 0x36,
        lstore = 0x37,
        fstore = 0x38,
        dstore = 0x39,
        astore = 0x3A,
        istore_0 = 0x3B,
        istore_1 = 0x3C,
        istore_2 = 0x3D,
        istore_3 = 0x3E,
        lstore_0 = 0x3F,
        lstore_1 = 0x40,
        lstore_2 = 0x41,
        lstore_3 = 0x42,
        fstore_0 = 0x43,
        fstore_1 = 0x44,
        fstore_2 = 0x45,
        fstore_3 = 0x46,
        dstore_0 = 0x47,
        dstore_1 = 0x48,
        dstore_2 = 0x49,
        dstore_3 = 0x4A,
        astore_0 = 0x4B,
        astore_1 = 0x4C,
        astore_2 = 0x4D,
        astore_3 = 0x4E,
        iastore = 0x4F,
        lastore = 0x50,
        fastore = 0x51,
        dastore = 0x52,
        aastore = 0x53,
        bastore = 0x54,
        castore = 0x55,
        sastore = 0x56,
        pop = 0x57,
        pop2 = 0x58,
        dup = 0x59,
        dup_x1 = 0x5A,
        dup_x2 = 0x5B,
        dup2 = 0x5C,
        dup2_x1 = 0x5D,
        dup2_x2 = 0x5E,
        swap = 0x5F,
        iadd = 0x60,
        ladd = 0x61,
        fadd = 0x62,
        dadd = 0x63,
        isub = 0x64,
        lsub = 0x65,
        fsub = 0x66,
        dsub = 0x67,
        imul = 0x68,
        lmul = 0x69,
        fmul = 0x6A,
        dmul = 0x6B,
        idiv = 0x6C,
        ldiv = 0x6D,
        fdiv = 0x6E,
        ddiv = 0x6F,
        irem = 0x70,
        lrem = 0x71,
        frem = 0x72,
        drem = 0x73,
        ineg = 0x74,
        lneg = 0x75,
        fneg = 0x76,
        dneg = 0x77,
        ishl = 0x78,
        lshl = 0x79,
        ishr = 0x7A,
        lshr = 0x7B,
        iushr = 0x7C,
        lushr = 0x7D,
        iand = 0x7E,
        land = 0x7F,
        ior = 0x80,
        lor = 0x81,
        ixor = 0x82,
        lxor = 0x83,
        iinc = 0x84,
        i2l = 0x85,
        i2f = 0x86,
        i2d = 0x87,
        l2i = 0x88,
        l2f = 0x89,
        l2d = 0x8A,
        f2i = 0x8B,
        f2l = 0x8C,
        f2d = 0x8D,
        d2i = 0x8E,
        d2l = 0x8F,
        d2f = 0x90,
        i2b = 0x91,
        i2c = 0x92,
        i2s = 0x93,
        lcmp = 0x94,
        fcmpl = 0x95,
        fcmpg = 0x96,
        dcmpl = 0x97,
        dcmpg = 0x98,
        ifeq = 0x99,
        ifne = 0x9A,
        iflt = 0x9B,
        ifge = 0x9C,
        ifgt = 0x9D,
        ifle = 0x9E,
        if_icmpeq = 0x9F,
        if_icmpne = 0xA0,
        if_icmplt = 0xA1,
        if_icmpge = 0xA2,
        if_icmpgt = 0xA3,
        if_icmple = 0xA4,
        if_acmpeq = 0xA5,
        if_acmpne = 0xA6,
        @goto = 0xA7,
        jsr = 0xA8,
        ret = 0xA9,
        tableswitch = 0xAA,
        lookupswitch = 0xAB,
        ireturn = 0xAC,
        lreturn = 0xAD,
        freturn = 0xAE,
        dreturn = 0xAF,
        areturn = 0xB0,
        @return = 0xB1,
        getstatic = 0xB2,
        putstatic = 0xB3,
        getfield = 0xB4,
        putfield = 0xB5,
        invokevirtual = 0xB6,
        invokespecial = 0xB7,
        invokestatic = 0xB8,
        invokeinterface = 0xB9,
        xxxunusedxxx1 = 0xBA,
        @new = 0xBB,
        newarray = 0xBC,
        anewarray = 0xBD,
        arraylength = 0xBE,
        athrow = 0xBF,
        checkcast = 0xC0,
        instanceof = 0xC1,
        monitorenter = 0xC2,
        monitorexit = 0xC3,
        wide = 0xC4,
        multianewarray = 0xC5,
        ifnull = 0xC6,
        ifnonnull = 0xC7,
        goto_w = 0xC8,
        jsr_w = 0xC9,

        // reserved opcodes
        breakpoint = 0xCA,
        impdep1 = 0xFE,
        impdep2 = 0xFF,
    }
}
