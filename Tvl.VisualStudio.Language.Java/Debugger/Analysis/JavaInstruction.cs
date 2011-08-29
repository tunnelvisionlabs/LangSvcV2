namespace Tvl.VisualStudio.Language.Java.Debugger.Analysis
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public struct JavaInstruction
    {
        /// <summary></summary>
        public static readonly JavaInstruction Aaload;
        /// <summary></summary>
        public static readonly JavaInstruction Aastore;
        /// <summary></summary>
        public static readonly JavaInstruction Aconst_null;
        /// <summary></summary>
        public static readonly JavaInstruction Aload;
        /// <summary></summary>
        public static readonly JavaInstruction Aload_0;
        /// <summary></summary>
        public static readonly JavaInstruction Aload_1;
        /// <summary></summary>
        public static readonly JavaInstruction Aload_2;
        /// <summary></summary>
        public static readonly JavaInstruction Aload_3;
        /// <summary></summary>
        public static readonly JavaInstruction Anewarray;
        /// <summary></summary>
        public static readonly JavaInstruction Areturn;
        /// <summary></summary>
        public static readonly JavaInstruction Arraylength;
        /// <summary></summary>
        public static readonly JavaInstruction Astore;
        /// <summary></summary>
        public static readonly JavaInstruction Astore_0;
        /// <summary></summary>
        public static readonly JavaInstruction Astore_1;
        /// <summary></summary>
        public static readonly JavaInstruction Astore_2;
        /// <summary></summary>
        public static readonly JavaInstruction Astore_3;
        /// <summary></summary>
        public static readonly JavaInstruction Athrow;
        /// <summary></summary>
        public static readonly JavaInstruction Baload;
        /// <summary></summary>
        public static readonly JavaInstruction Bastore;
        /// <summary></summary>
        public static readonly JavaInstruction Bipush;
        /// <summary></summary>
        public static readonly JavaInstruction Breakpoint;
        /// <summary></summary>
        public static readonly JavaInstruction Caload;
        /// <summary></summary>
        public static readonly JavaInstruction Castore;
        /// <summary></summary>
        public static readonly JavaInstruction Checkcast;
        /// <summary></summary>
        public static readonly JavaInstruction D2f;
        /// <summary></summary>
        public static readonly JavaInstruction D2i;
        /// <summary></summary>
        public static readonly JavaInstruction D2l;
        /// <summary></summary>
        public static readonly JavaInstruction Dadd;
        /// <summary></summary>
        public static readonly JavaInstruction Daload;
        /// <summary></summary>
        public static readonly JavaInstruction Dastore;
        /// <summary></summary>
        public static readonly JavaInstruction Dcmpg;
        /// <summary></summary>
        public static readonly JavaInstruction Dcmpl;
        /// <summary></summary>
        public static readonly JavaInstruction Dconst_0;
        /// <summary></summary>
        public static readonly JavaInstruction Dconst_1;
        /// <summary></summary>
        public static readonly JavaInstruction Ddiv;
        /// <summary></summary>
        public static readonly JavaInstruction Dload;
        /// <summary></summary>
        public static readonly JavaInstruction Dload_0;
        /// <summary></summary>
        public static readonly JavaInstruction Dload_1;
        /// <summary></summary>
        public static readonly JavaInstruction Dload_2;
        /// <summary></summary>
        public static readonly JavaInstruction Dload_3;
        /// <summary></summary>
        public static readonly JavaInstruction Dmul;
        /// <summary></summary>
        public static readonly JavaInstruction Dneg;
        /// <summary></summary>
        public static readonly JavaInstruction Drem;
        /// <summary></summary>
        public static readonly JavaInstruction Dreturn;
        /// <summary></summary>
        public static readonly JavaInstruction Dstore;
        /// <summary></summary>
        public static readonly JavaInstruction Dstore_0;
        /// <summary></summary>
        public static readonly JavaInstruction Dstore_1;
        /// <summary></summary>
        public static readonly JavaInstruction Dstore_2;
        /// <summary></summary>
        public static readonly JavaInstruction Dstore_3;
        /// <summary></summary>
        public static readonly JavaInstruction Dsub;
        /// <summary></summary>
        public static readonly JavaInstruction Dup;
        /// <summary></summary>
        public static readonly JavaInstruction Dup_x1;
        /// <summary></summary>
        public static readonly JavaInstruction Dup_x2;
        /// <summary></summary>
        public static readonly JavaInstruction Dup2;
        /// <summary></summary>
        public static readonly JavaInstruction Dup2_x1;
        /// <summary></summary>
        public static readonly JavaInstruction Dup2_x2;
        /// <summary></summary>
        public static readonly JavaInstruction F2d;
        /// <summary></summary>
        public static readonly JavaInstruction F2i;
        /// <summary></summary>
        public static readonly JavaInstruction F2l;
        /// <summary></summary>
        public static readonly JavaInstruction Fadd;
        /// <summary></summary>
        public static readonly JavaInstruction Faload;
        /// <summary></summary>
        public static readonly JavaInstruction Fastore;
        /// <summary></summary>
        public static readonly JavaInstruction Fcmpg;
        /// <summary></summary>
        public static readonly JavaInstruction Fcmpl;
        /// <summary></summary>
        public static readonly JavaInstruction Fconst_0;
        /// <summary></summary>
        public static readonly JavaInstruction Fconst_1;
        /// <summary></summary>
        public static readonly JavaInstruction Fconst_2;
        /// <summary></summary>
        public static readonly JavaInstruction Fdiv;
        /// <summary></summary>
        public static readonly JavaInstruction Fload;
        /// <summary></summary>
        public static readonly JavaInstruction Fload_0;
        /// <summary></summary>
        public static readonly JavaInstruction Fload_1;
        /// <summary></summary>
        public static readonly JavaInstruction Fload_2;
        /// <summary></summary>
        public static readonly JavaInstruction Fload_3;
        /// <summary></summary>
        public static readonly JavaInstruction Fmul;
        /// <summary></summary>
        public static readonly JavaInstruction Fneg;
        /// <summary></summary>
        public static readonly JavaInstruction Frem;
        /// <summary></summary>
        public static readonly JavaInstruction Freturn;
        /// <summary></summary>
        public static readonly JavaInstruction Fstore;
        /// <summary></summary>
        public static readonly JavaInstruction Fstore_0;
        /// <summary></summary>
        public static readonly JavaInstruction Fstore_1;
        /// <summary></summary>
        public static readonly JavaInstruction Fstore_2;
        /// <summary></summary>
        public static readonly JavaInstruction Fstore_3;
        /// <summary></summary>
        public static readonly JavaInstruction Fsub;
        /// <summary></summary>
        public static readonly JavaInstruction Getfield;
        /// <summary></summary>
        public static readonly JavaInstruction Getstatic;
        /// <summary></summary>
        public static readonly JavaInstruction Goto;
        /// <summary></summary>
        public static readonly JavaInstruction Goto_w;
        /// <summary></summary>
        public static readonly JavaInstruction I2b;
        /// <summary></summary>
        public static readonly JavaInstruction I2c;
        /// <summary></summary>
        public static readonly JavaInstruction I2d;
        /// <summary></summary>
        public static readonly JavaInstruction I2f;
        /// <summary></summary>
        public static readonly JavaInstruction I2l;
        /// <summary></summary>
        public static readonly JavaInstruction I2s;
        /// <summary></summary>
        public static readonly JavaInstruction Iadd;
        /// <summary></summary>
        public static readonly JavaInstruction Iaload;
        /// <summary></summary>
        public static readonly JavaInstruction Iand;
        /// <summary></summary>
        public static readonly JavaInstruction Iastore;
        /// <summary></summary>
        public static readonly JavaInstruction Iconst_0;
        /// <summary></summary>
        public static readonly JavaInstruction Iconst_1;
        /// <summary></summary>
        public static readonly JavaInstruction Iconst_2;
        /// <summary></summary>
        public static readonly JavaInstruction Iconst_3;
        /// <summary></summary>
        public static readonly JavaInstruction Iconst_4;
        /// <summary></summary>
        public static readonly JavaInstruction Iconst_5;
        /// <summary></summary>
        public static readonly JavaInstruction Iconst_m1;
        /// <summary></summary>
        public static readonly JavaInstruction Idiv;
        /// <summary></summary>
        public static readonly JavaInstruction If_acmpeq;
        /// <summary></summary>
        public static readonly JavaInstruction If_acmpne;
        /// <summary></summary>
        public static readonly JavaInstruction If_icmpeq;
        /// <summary></summary>
        public static readonly JavaInstruction If_icmpge;
        /// <summary></summary>
        public static readonly JavaInstruction If_icmpgt;
        /// <summary></summary>
        public static readonly JavaInstruction If_icmple;
        /// <summary></summary>
        public static readonly JavaInstruction If_icmplt;
        /// <summary></summary>
        public static readonly JavaInstruction If_icmpne;
        /// <summary></summary>
        public static readonly JavaInstruction Ifeq;
        /// <summary></summary>
        public static readonly JavaInstruction Ifge;
        /// <summary></summary>
        public static readonly JavaInstruction Ifgt;
        /// <summary></summary>
        public static readonly JavaInstruction Ifle;
        /// <summary></summary>
        public static readonly JavaInstruction Iflt;
        /// <summary></summary>
        public static readonly JavaInstruction Ifne;
        /// <summary></summary>
        public static readonly JavaInstruction Ifnonnull;
        /// <summary></summary>
        public static readonly JavaInstruction Ifnull;
        /// <summary></summary>
        public static readonly JavaInstruction Iinc;
        /// <summary></summary>
        public static readonly JavaInstruction Iload;
        /// <summary></summary>
        public static readonly JavaInstruction Iload_0;
        /// <summary></summary>
        public static readonly JavaInstruction Iload_1;
        /// <summary></summary>
        public static readonly JavaInstruction Iload_2;
        /// <summary></summary>
        public static readonly JavaInstruction Iload_3;
        /// <summary></summary>
        public static readonly JavaInstruction Impdep1;
        /// <summary></summary>
        public static readonly JavaInstruction Impdep2;
        /// <summary></summary>
        public static readonly JavaInstruction Imul;
        /// <summary></summary>
        public static readonly JavaInstruction Ineg;
        /// <summary></summary>
        public static readonly JavaInstruction Instanceof;
        /// <summary></summary>
        public static readonly JavaInstruction Invokeinterface;
        /// <summary></summary>
        public static readonly JavaInstruction Invokespecial;
        /// <summary></summary>
        public static readonly JavaInstruction Invokestatic;
        /// <summary></summary>
        public static readonly JavaInstruction Invokevirtual;
        /// <summary></summary>
        public static readonly JavaInstruction Ior;
        /// <summary></summary>
        public static readonly JavaInstruction Irem;
        /// <summary></summary>
        public static readonly JavaInstruction Ireturn;
        /// <summary></summary>
        public static readonly JavaInstruction Ishl;
        /// <summary></summary>
        public static readonly JavaInstruction Ishr;
        /// <summary></summary>
        public static readonly JavaInstruction Istore;
        /// <summary></summary>
        public static readonly JavaInstruction Istore_0;
        /// <summary></summary>
        public static readonly JavaInstruction Istore_1;
        /// <summary></summary>
        public static readonly JavaInstruction Istore_2;
        /// <summary></summary>
        public static readonly JavaInstruction Istore_3;
        /// <summary></summary>
        public static readonly JavaInstruction Isub;
        /// <summary></summary>
        public static readonly JavaInstruction Iushr;
        /// <summary></summary>
        public static readonly JavaInstruction Ixor;
        /// <summary></summary>
        public static readonly JavaInstruction Jsr;
        /// <summary></summary>
        public static readonly JavaInstruction Jsr_w;
        /// <summary></summary>
        public static readonly JavaInstruction L2d;
        /// <summary></summary>
        public static readonly JavaInstruction L2f;
        /// <summary></summary>
        public static readonly JavaInstruction L2i;
        /// <summary></summary>
        public static readonly JavaInstruction Ladd;
        /// <summary></summary>
        public static readonly JavaInstruction Laload;
        /// <summary></summary>
        public static readonly JavaInstruction Land;
        /// <summary></summary>
        public static readonly JavaInstruction Lastore;
        /// <summary></summary>
        public static readonly JavaInstruction Lcmp;
        /// <summary></summary>
        public static readonly JavaInstruction Lconst_0;
        /// <summary></summary>
        public static readonly JavaInstruction Lconst_1;
        /// <summary></summary>
        public static readonly JavaInstruction Ldc;
        /// <summary></summary>
        public static readonly JavaInstruction Ldc_w;
        /// <summary></summary>
        public static readonly JavaInstruction Ldc2_w;
        /// <summary></summary>
        public static readonly JavaInstruction Ldiv;
        /// <summary></summary>
        public static readonly JavaInstruction Lload;
        /// <summary></summary>
        public static readonly JavaInstruction Lload_0;
        /// <summary></summary>
        public static readonly JavaInstruction Lload_1;
        /// <summary></summary>
        public static readonly JavaInstruction Lload_2;
        /// <summary></summary>
        public static readonly JavaInstruction Lload_3;
        /// <summary></summary>
        public static readonly JavaInstruction Lmul;
        /// <summary></summary>
        public static readonly JavaInstruction Lneg;
        /// <summary></summary>
        public static readonly JavaInstruction Lookupswitch;
        /// <summary></summary>
        public static readonly JavaInstruction Lor;
        /// <summary></summary>
        public static readonly JavaInstruction Lrem;
        /// <summary></summary>
        public static readonly JavaInstruction Lreturn;
        /// <summary></summary>
        public static readonly JavaInstruction Lshl;
        /// <summary></summary>
        public static readonly JavaInstruction Lshr;
        /// <summary></summary>
        public static readonly JavaInstruction Lstore;
        /// <summary></summary>
        public static readonly JavaInstruction Lstore_0;
        /// <summary></summary>
        public static readonly JavaInstruction Lstore_1;
        /// <summary></summary>
        public static readonly JavaInstruction Lstore_2;
        /// <summary></summary>
        public static readonly JavaInstruction Lstore_3;
        /// <summary></summary>
        public static readonly JavaInstruction Lsub;
        /// <summary></summary>
        public static readonly JavaInstruction Lushr;
        /// <summary></summary>
        public static readonly JavaInstruction Lxor;
        /// <summary></summary>
        public static readonly JavaInstruction Monitorenter;
        /// <summary></summary>
        public static readonly JavaInstruction Monitorexit;
        /// <summary></summary>
        public static readonly JavaInstruction Multianewarray;
        /// <summary></summary>
        public static readonly JavaInstruction New;
        /// <summary></summary>
        public static readonly JavaInstruction Newarray;
        /// <summary></summary>
        public static readonly JavaInstruction Nop;
        /// <summary></summary>
        public static readonly JavaInstruction Pop;
        /// <summary></summary>
        public static readonly JavaInstruction Pop2;
        /// <summary></summary>
        public static readonly JavaInstruction Putfield;
        /// <summary></summary>
        public static readonly JavaInstruction Putstatic;
        /// <summary></summary>
        public static readonly JavaInstruction Ret;
        /// <summary></summary>
        public static readonly JavaInstruction Return;
        /// <summary></summary>
        public static readonly JavaInstruction Saload;
        /// <summary></summary>
        public static readonly JavaInstruction Sastore;
        /// <summary></summary>
        public static readonly JavaInstruction Sipush;
        /// <summary></summary>
        public static readonly JavaInstruction Swap;
        /// <summary></summary>
        public static readonly JavaInstruction Tableswitch;
        /// <summary></summary>
        public static readonly JavaInstruction Wide;
        /// <summary></summary>
        public static readonly JavaInstruction Xxxunusedxxx1;

        private static readonly ReadOnlyCollection<JavaInstruction> _instructionLookup;

        public readonly JavaOpCode OpCode;
        public readonly string Name;
        public readonly JavaOperandType OperandType;
        public readonly JavaFlowControl FlowControl;
        public readonly int Size;
        public readonly JavaStackBehavior StackBehaviorPop;
        public readonly JavaStackBehavior StackBehaviorPush;

        public JavaInstruction(JavaOpCode opcode, string name, JavaOperandType operandType, JavaFlowControl flowControl, int size, JavaStackBehavior stackBehaviorPop, JavaStackBehavior stackBehaviorPush)
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
            Aaload = new JavaInstruction(JavaOpCode.Aaload, "aaload", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopRef_PopI, JavaStackBehavior.PushRef);
            Aastore = new JavaInstruction(JavaOpCode.Aastore, "aastore", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopRef_PopI_PopRef, JavaStackBehavior.Push0);
            Aconst_null = new JavaInstruction(JavaOpCode.Aconst_null, "aconst_null", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.Pop0, JavaStackBehavior.PushRef);
            Aload = new JavaInstruction(JavaOpCode.Aload, "aload", JavaOperandType.InlineVar, JavaFlowControl.Next, 2, JavaStackBehavior.Pop0, JavaStackBehavior.PushRef);
            Aload_0 = new JavaInstruction(JavaOpCode.Aload_0, "aload_0", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.Pop0, JavaStackBehavior.PushRef);
            Aload_1 = new JavaInstruction(JavaOpCode.Aload_1, "aload_1", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.Pop0, JavaStackBehavior.PushRef);
            Aload_2 = new JavaInstruction(JavaOpCode.Aload_2, "aload_2", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.Pop0, JavaStackBehavior.PushRef);
            Aload_3 = new JavaInstruction(JavaOpCode.Aload_3, "aload_3", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.Pop0, JavaStackBehavior.PushRef);
            Anewarray = new JavaInstruction(JavaOpCode.Anewarray, "anewarray", JavaOperandType.InlineType, JavaFlowControl.Next, 3, JavaStackBehavior.PopI, JavaStackBehavior.PushRef);
            Areturn = new JavaInstruction(JavaOpCode.Areturn, "areturn", JavaOperandType.InlineNone, JavaFlowControl.Return, 1, JavaStackBehavior.PopRef, JavaStackBehavior.Push0);
            Arraylength = new JavaInstruction(JavaOpCode.Arraylength, "arraylength", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopRef, JavaStackBehavior.PushI);
            Astore = new JavaInstruction(JavaOpCode.Astore, "astore", JavaOperandType.InlineVar, JavaFlowControl.Next, 2, JavaStackBehavior.PopRef, JavaStackBehavior.Push0);
            Astore_0 = new JavaInstruction(JavaOpCode.Astore_0, "astore_0", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopRef, JavaStackBehavior.Push0);
            Astore_1 = new JavaInstruction(JavaOpCode.Astore_1, "astore_1", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopRef, JavaStackBehavior.Push0);
            Astore_2 = new JavaInstruction(JavaOpCode.Astore_2, "astore_2", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopRef, JavaStackBehavior.Push0);
            Astore_3 = new JavaInstruction(JavaOpCode.Astore_3, "astore_3", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopRef, JavaStackBehavior.Push0);
            Athrow = new JavaInstruction(JavaOpCode.Athrow, "athrow", JavaOperandType.InlineNone, JavaFlowControl.Throw, 1, JavaStackBehavior.PopRef, JavaStackBehavior.PushRef);
            Baload = new JavaInstruction(JavaOpCode.Baload, "baload", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopRef_PopI, JavaStackBehavior.PushI);
            Bastore = new JavaInstruction(JavaOpCode.Bastore, "bastore", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopRef_PopI_PopI, JavaStackBehavior.Push0);
            Bipush = new JavaInstruction(JavaOpCode.Bipush, "bipush", JavaOperandType.InlineI1, JavaFlowControl.Next, 2, JavaStackBehavior.Pop0, JavaStackBehavior.PushI);
            Breakpoint = new JavaInstruction(JavaOpCode.Breakpoint, "breakpoint", JavaOperandType.InlineNone, JavaFlowControl.Break, 1, JavaStackBehavior.Pop0, JavaStackBehavior.Push0);
            Caload = new JavaInstruction(JavaOpCode.Caload, "caload", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopRef_PopI, JavaStackBehavior.PushI);
            Castore = new JavaInstruction(JavaOpCode.Castore, "castore", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopRef_PopI_PopI, JavaStackBehavior.Push0);
            Checkcast = new JavaInstruction(JavaOpCode.Checkcast, "checkcast", JavaOperandType.InlineType, JavaFlowControl.Next, 3, JavaStackBehavior.PopRef, JavaStackBehavior.PushRef);
            D2f = new JavaInstruction(JavaOpCode.D2f, "d2f", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopR8, JavaStackBehavior.PushR4);
            D2i = new JavaInstruction(JavaOpCode.D2i, "d2i", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopR8, JavaStackBehavior.PushI);
            D2l = new JavaInstruction(JavaOpCode.D2l, "d2l", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopR8, JavaStackBehavior.PushI8);
            Dadd = new JavaInstruction(JavaOpCode.Dadd, "dadd", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopR8_PopR8, JavaStackBehavior.PushR8);
            Daload = new JavaInstruction(JavaOpCode.Daload, "daload", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopRef_PopI, JavaStackBehavior.PushR8);
            Dastore = new JavaInstruction(JavaOpCode.Dastore, "dastore", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopRef_PopI_PopR8, JavaStackBehavior.Push0);
            Dcmpg = new JavaInstruction(JavaOpCode.Dcmpg, "dcmpg", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopR8_PopR8, JavaStackBehavior.PushI);
            Dcmpl = new JavaInstruction(JavaOpCode.Dcmpl, "dcmpl", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopR8_PopR8, JavaStackBehavior.PushI);
            Dconst_0 = new JavaInstruction(JavaOpCode.Dconst_0, "dconst_0", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.Pop0, JavaStackBehavior.PushR8);
            Dconst_1 = new JavaInstruction(JavaOpCode.Dconst_1, "dconst_1", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.Pop0, JavaStackBehavior.PushR8);
            Ddiv = new JavaInstruction(JavaOpCode.Ddiv, "ddiv", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopR8_PopR8, JavaStackBehavior.PushR8);
            Dload = new JavaInstruction(JavaOpCode.Dload, "dload", JavaOperandType.InlineVar, JavaFlowControl.Next, 2, JavaStackBehavior.Pop0, JavaStackBehavior.PushR8);
            Dload_0 = new JavaInstruction(JavaOpCode.Dload_0, "dload_0", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.Pop0, JavaStackBehavior.PushR8);
            Dload_1 = new JavaInstruction(JavaOpCode.Dload_1, "dload_1", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.Pop0, JavaStackBehavior.PushR8);
            Dload_2 = new JavaInstruction(JavaOpCode.Dload_2, "dload_2", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.Pop0, JavaStackBehavior.PushR8);
            Dload_3 = new JavaInstruction(JavaOpCode.Dload_3, "dload_3", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.Pop0, JavaStackBehavior.PushR8);
            Dmul = new JavaInstruction(JavaOpCode.Dmul, "dmul", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopR8_PopR8, JavaStackBehavior.PushR8);
            Dneg = new JavaInstruction(JavaOpCode.Dneg, "dneg", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopR8, JavaStackBehavior.PushR8);
            Drem = new JavaInstruction(JavaOpCode.Drem, "drem", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopR8_PopR8, JavaStackBehavior.PushR8);
            Dreturn = new JavaInstruction(JavaOpCode.Dreturn, "dreturn", JavaOperandType.InlineNone, JavaFlowControl.Return, 1, JavaStackBehavior.PopR8, JavaStackBehavior.Push0);
            Dstore = new JavaInstruction(JavaOpCode.Dstore, "dstore", JavaOperandType.InlineVar, JavaFlowControl.Next, 2, JavaStackBehavior.PopR8, JavaStackBehavior.Push0);
            Dstore_0 = new JavaInstruction(JavaOpCode.Dstore_0, "dstore_0", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopR8, JavaStackBehavior.Push0);
            Dstore_1 = new JavaInstruction(JavaOpCode.Dstore_1, "dstore_1", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopR8, JavaStackBehavior.Push0);
            Dstore_2 = new JavaInstruction(JavaOpCode.Dstore_2, "dstore_2", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopR8, JavaStackBehavior.Push0);
            Dstore_3 = new JavaInstruction(JavaOpCode.Dstore_3, "dstore_3", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopR8, JavaStackBehavior.Push0);
            Dsub = new JavaInstruction(JavaOpCode.Dsub, "dsub", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopR8, JavaStackBehavior.PushR8);
            Dup = new JavaInstruction(JavaOpCode.Dup, "dup", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.Pop1, JavaStackBehavior.Push1_Push1);
            Dup_x1 = new JavaInstruction(JavaOpCode.Dup_x1, "dup_x1", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.Pop1_Pop1, JavaStackBehavior.Push1_Push1);
            Dup_x2 = new JavaInstruction(JavaOpCode.Dup_x2, "dup_x2", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopVar, JavaStackBehavior.PushVar);
            Dup2 = new JavaInstruction(JavaOpCode.Dup2, "dup2", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopVar, JavaStackBehavior.PushVar);
            Dup2_x1 = new JavaInstruction(JavaOpCode.Dup2_x1, "dup2_x1", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopVar, JavaStackBehavior.PushVar);
            Dup2_x2 = new JavaInstruction(JavaOpCode.Dup2_x2, "dup2_x2", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopVar, JavaStackBehavior.PushVar);
            F2d = new JavaInstruction(JavaOpCode.F2d, "f2d", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopR4, JavaStackBehavior.PushR8);
            F2i = new JavaInstruction(JavaOpCode.F2i, "f2i", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopR4, JavaStackBehavior.PushI);
            F2l = new JavaInstruction(JavaOpCode.F2l, "f2l", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopR4, JavaStackBehavior.PushI8);
            Fadd = new JavaInstruction(JavaOpCode.Fadd, "fadd", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopR4_PopR4, JavaStackBehavior.PushR4);
            Faload = new JavaInstruction(JavaOpCode.Faload, "faload", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopRef_PopI, JavaStackBehavior.PushR4);
            Fastore = new JavaInstruction(JavaOpCode.Fastore, "fastore", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopRef_PopI_PopR4, JavaStackBehavior.Push0);
            Fcmpg = new JavaInstruction(JavaOpCode.Fcmpg, "fcmpg", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopR4_PopR4, JavaStackBehavior.PushI);
            Fcmpl = new JavaInstruction(JavaOpCode.Fcmpl, "fcmpl", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopR4_PopR4, JavaStackBehavior.PushI);
            Fconst_0 = new JavaInstruction(JavaOpCode.Fconst_0, "fconst_0", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.Pop0, JavaStackBehavior.PushR4);
            Fconst_1 = new JavaInstruction(JavaOpCode.Fconst_1, "fconst_1", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.Pop0, JavaStackBehavior.PushR4);
            Fconst_2 = new JavaInstruction(JavaOpCode.Fconst_2, "fconst_2", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.Pop0, JavaStackBehavior.PushR4);
            Fdiv = new JavaInstruction(JavaOpCode.Fdiv, "fdiv", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopR4_PopR4, JavaStackBehavior.PushR4);
            Fload = new JavaInstruction(JavaOpCode.Fload, "fload", JavaOperandType.InlineVar, JavaFlowControl.Next, 2, JavaStackBehavior.Pop0, JavaStackBehavior.PushR4);
            Fload_0 = new JavaInstruction(JavaOpCode.Fload_0, "fload_0", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.Pop0, JavaStackBehavior.PushR4);
            Fload_1 = new JavaInstruction(JavaOpCode.Fload_1, "fload_1", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.Pop0, JavaStackBehavior.PushR4);
            Fload_2 = new JavaInstruction(JavaOpCode.Fload_2, "fload_2", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.Pop0, JavaStackBehavior.PushR4);
            Fload_3 = new JavaInstruction(JavaOpCode.Fload_3, "fload_3", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.Pop0, JavaStackBehavior.PushR4);
            Fmul = new JavaInstruction(JavaOpCode.Fmul, "fmul", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopR4_PopR4, JavaStackBehavior.PushR4);
            Fneg = new JavaInstruction(JavaOpCode.Fneg, "fneg", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopR4, JavaStackBehavior.PushR4);
            Frem = new JavaInstruction(JavaOpCode.Frem, "frem", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopR4_PopR4, JavaStackBehavior.PushR4);
            Freturn = new JavaInstruction(JavaOpCode.Freturn, "freturn", JavaOperandType.InlineNone, JavaFlowControl.Return, 1, JavaStackBehavior.PopR4, JavaStackBehavior.Push0);
            Fstore = new JavaInstruction(JavaOpCode.Fstore, "fstore", JavaOperandType.InlineVar, JavaFlowControl.Next, 2, JavaStackBehavior.PopR4, JavaStackBehavior.Push0);
            Fstore_0 = new JavaInstruction(JavaOpCode.Fstore_0, "fstore_0", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopR4, JavaStackBehavior.Push0);
            Fstore_1 = new JavaInstruction(JavaOpCode.Fstore_1, "fstore_1", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopR4, JavaStackBehavior.Push0);
            Fstore_2 = new JavaInstruction(JavaOpCode.Fstore_2, "fstore_2", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopR4, JavaStackBehavior.Push0);
            Fstore_3 = new JavaInstruction(JavaOpCode.Fstore_3, "fstore_3", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopR4, JavaStackBehavior.Push0);
            Fsub = new JavaInstruction(JavaOpCode.Fsub, "fsub", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopR4_PopR4, JavaStackBehavior.PushR4);
            Getfield = new JavaInstruction(JavaOpCode.Getfield, "getfield", JavaOperandType.InlineField, JavaFlowControl.Next, 3, JavaStackBehavior.PopRef, JavaStackBehavior.Push1);
            Getstatic = new JavaInstruction(JavaOpCode.Getstatic, "getstatic", JavaOperandType.InlineField, JavaFlowControl.Next, 3, JavaStackBehavior.Pop0, JavaStackBehavior.Push1);
            Goto = new JavaInstruction(JavaOpCode.Goto, "goto", JavaOperandType.InlineShortBranchTarget, JavaFlowControl.Branch, 3, JavaStackBehavior.Pop0, JavaStackBehavior.Push0);
            Goto_w = new JavaInstruction(JavaOpCode.Goto_w, "goto_w", JavaOperandType.InlineBranchTarget, JavaFlowControl.Branch, 5, JavaStackBehavior.Pop0, JavaStackBehavior.Push0);
            I2b = new JavaInstruction(JavaOpCode.I2b, "i2b", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopI, JavaStackBehavior.PushI);
            I2c = new JavaInstruction(JavaOpCode.I2c, "i2c", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopI, JavaStackBehavior.PushI);
            I2d = new JavaInstruction(JavaOpCode.I2d, "i2d", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopI, JavaStackBehavior.PushR8);
            I2f = new JavaInstruction(JavaOpCode.I2f, "i2f", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopI, JavaStackBehavior.PushR4);
            I2l = new JavaInstruction(JavaOpCode.I2l, "i2l", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopI, JavaStackBehavior.PushI8);
            I2s = new JavaInstruction(JavaOpCode.I2s, "i2s", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopI, JavaStackBehavior.PushI);
            Iadd = new JavaInstruction(JavaOpCode.Iadd, "iadd", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopI_PopI, JavaStackBehavior.PushI);
            Iaload = new JavaInstruction(JavaOpCode.Iaload, "iaload", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopRef_PopI, JavaStackBehavior.PushI);
            Iand = new JavaInstruction(JavaOpCode.Iand, "iand", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopI_PopI, JavaStackBehavior.PushI);
            Iastore = new JavaInstruction(JavaOpCode.Iastore, "iastore", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopRef_PopI_PopI, JavaStackBehavior.Push0);
            Iconst_0 = new JavaInstruction(JavaOpCode.Iconst_0, "iconst_0", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.Pop0, JavaStackBehavior.PushI);
            Iconst_1 = new JavaInstruction(JavaOpCode.Iconst_1, "iconst_1", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.Pop0, JavaStackBehavior.PushI);
            Iconst_2 = new JavaInstruction(JavaOpCode.Iconst_2, "iconst_2", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.Pop0, JavaStackBehavior.PushI);
            Iconst_3 = new JavaInstruction(JavaOpCode.Iconst_3, "iconst_3", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.Pop0, JavaStackBehavior.PushI);
            Iconst_4 = new JavaInstruction(JavaOpCode.Iconst_4, "iconst_4", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.Pop0, JavaStackBehavior.PushI);
            Iconst_5 = new JavaInstruction(JavaOpCode.Iconst_5, "iconst_5", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.Pop0, JavaStackBehavior.PushI);
            Iconst_m1 = new JavaInstruction(JavaOpCode.Iconst_m1, "iconst_m1", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.Pop0, JavaStackBehavior.PushI);
            Idiv = new JavaInstruction(JavaOpCode.Idiv, "idiv", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopI_PopI, JavaStackBehavior.PushI);
            If_acmpeq = new JavaInstruction(JavaOpCode.If_acmpeq, "if_acmpeq", JavaOperandType.InlineShortBranchTarget, JavaFlowControl.ConditionalBranch, 3, JavaStackBehavior.PopRef_PopRef, JavaStackBehavior.Push0);
            If_acmpne = new JavaInstruction(JavaOpCode.If_acmpne, "if_acmpne", JavaOperandType.InlineShortBranchTarget, JavaFlowControl.ConditionalBranch, 3, JavaStackBehavior.PopRef_PopRef, JavaStackBehavior.Push0);
            If_icmpeq = new JavaInstruction(JavaOpCode.If_icmpeq, "if_icmpeq", JavaOperandType.InlineShortBranchTarget, JavaFlowControl.ConditionalBranch, 3, JavaStackBehavior.PopI_PopI, JavaStackBehavior.Push0);
            If_icmpge = new JavaInstruction(JavaOpCode.If_icmpge, "if_icmpge", JavaOperandType.InlineShortBranchTarget, JavaFlowControl.ConditionalBranch, 3, JavaStackBehavior.PopI_PopI, JavaStackBehavior.Push0);
            If_icmpgt = new JavaInstruction(JavaOpCode.If_icmpgt, "if_icmpgt", JavaOperandType.InlineShortBranchTarget, JavaFlowControl.ConditionalBranch, 3, JavaStackBehavior.PopI_PopI, JavaStackBehavior.Push0);
            If_icmple = new JavaInstruction(JavaOpCode.If_icmple, "if_icmple", JavaOperandType.InlineShortBranchTarget, JavaFlowControl.ConditionalBranch, 3, JavaStackBehavior.PopI_PopI, JavaStackBehavior.Push0);
            If_icmplt = new JavaInstruction(JavaOpCode.If_icmplt, "if_icmplt", JavaOperandType.InlineShortBranchTarget, JavaFlowControl.ConditionalBranch, 3, JavaStackBehavior.PopI_PopI, JavaStackBehavior.Push0);
            If_icmpne = new JavaInstruction(JavaOpCode.If_icmpne, "if_icmpne", JavaOperandType.InlineShortBranchTarget, JavaFlowControl.ConditionalBranch, 3, JavaStackBehavior.PopI_PopI, JavaStackBehavior.Push0);
            Ifeq = new JavaInstruction(JavaOpCode.Ifeq, "ifeq", JavaOperandType.InlineShortBranchTarget, JavaFlowControl.ConditionalBranch, 3, JavaStackBehavior.PopI, JavaStackBehavior.Push0);
            Ifge = new JavaInstruction(JavaOpCode.Ifge, "ifge", JavaOperandType.InlineShortBranchTarget, JavaFlowControl.ConditionalBranch, 3, JavaStackBehavior.PopI, JavaStackBehavior.Push0);
            Ifgt = new JavaInstruction(JavaOpCode.Ifgt, "ifgt", JavaOperandType.InlineShortBranchTarget, JavaFlowControl.ConditionalBranch, 3, JavaStackBehavior.PopI, JavaStackBehavior.Push0);
            Ifle = new JavaInstruction(JavaOpCode.Ifle, "ifle", JavaOperandType.InlineShortBranchTarget, JavaFlowControl.ConditionalBranch, 3, JavaStackBehavior.PopI, JavaStackBehavior.Push0);
            Iflt = new JavaInstruction(JavaOpCode.Iflt, "iflt", JavaOperandType.InlineShortBranchTarget, JavaFlowControl.ConditionalBranch, 3, JavaStackBehavior.PopI, JavaStackBehavior.Push0);
            Ifne = new JavaInstruction(JavaOpCode.Ifne, "ifne", JavaOperandType.InlineShortBranchTarget, JavaFlowControl.ConditionalBranch, 3, JavaStackBehavior.PopI, JavaStackBehavior.Push0);
            Ifnonnull = new JavaInstruction(JavaOpCode.Ifnonnull, "ifnonnull", JavaOperandType.InlineShortBranchTarget, JavaFlowControl.ConditionalBranch, 3, JavaStackBehavior.PopRef, JavaStackBehavior.Push0);
            Ifnull = new JavaInstruction(JavaOpCode.Ifnull, "ifnull", JavaOperandType.InlineShortBranchTarget, JavaFlowControl.ConditionalBranch, 3, JavaStackBehavior.PopRef, JavaStackBehavior.Push0);
            Iinc = new JavaInstruction(JavaOpCode.Iinc, "iinc", JavaOperandType.InlineVar_I1, JavaFlowControl.Next, 3, JavaStackBehavior.Pop0, JavaStackBehavior.Push0);
            Iload = new JavaInstruction(JavaOpCode.Iload, "iload", JavaOperandType.InlineVar, JavaFlowControl.Next, 2, JavaStackBehavior.Pop0, JavaStackBehavior.PushI);
            Iload_0 = new JavaInstruction(JavaOpCode.Iload_0, "iload_0", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.Pop0, JavaStackBehavior.PushI);
            Iload_1 = new JavaInstruction(JavaOpCode.Iload_1, "iload_1", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.Pop0, JavaStackBehavior.PushI);
            Iload_2 = new JavaInstruction(JavaOpCode.Iload_2, "iload_2", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.Pop0, JavaStackBehavior.PushI);
            Iload_3 = new JavaInstruction(JavaOpCode.Iload_3, "iload_3", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.Pop0, JavaStackBehavior.PushI);
            Impdep1 = new JavaInstruction(JavaOpCode.Impdep1, "impdep1", JavaOperandType.InlineNone, JavaFlowControl.Special, 1, JavaStackBehavior.Pop0, JavaStackBehavior.Push0);
            Impdep2 = new JavaInstruction(JavaOpCode.Impdep2, "impdep2", JavaOperandType.InlineNone, JavaFlowControl.Special, 1, JavaStackBehavior.Pop0, JavaStackBehavior.Push0);
            Imul = new JavaInstruction(JavaOpCode.Imul, "imul", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopI_PopI, JavaStackBehavior.PushI);
            Ineg = new JavaInstruction(JavaOpCode.Ineg, "ineg", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopI, JavaStackBehavior.PushI);
            Instanceof = new JavaInstruction(JavaOpCode.Instanceof, "instanceof", JavaOperandType.InlineType, JavaFlowControl.Next, 3, JavaStackBehavior.PopRef, JavaStackBehavior.PushI);
            Invokeinterface = new JavaInstruction(JavaOpCode.Invokeinterface, "invokeinterface", JavaOperandType.InlineMethod_I1_0, JavaFlowControl.Call, 5, JavaStackBehavior.PopVar, JavaStackBehavior.PushVar);
            Invokespecial = new JavaInstruction(JavaOpCode.Invokespecial, "invokespecial", JavaOperandType.InlineMethod, JavaFlowControl.Call, 3, JavaStackBehavior.PopVar, JavaStackBehavior.PushVar);
            Invokestatic = new JavaInstruction(JavaOpCode.Invokestatic, "invokestatic", JavaOperandType.InlineMethod, JavaFlowControl.Call, 3, JavaStackBehavior.PopVar, JavaStackBehavior.PushVar);
            Invokevirtual = new JavaInstruction(JavaOpCode.Invokevirtual, "invokevirtual", JavaOperandType.InlineMethod, JavaFlowControl.Call, 3, JavaStackBehavior.PopVar, JavaStackBehavior.PushVar);
            Ior = new JavaInstruction(JavaOpCode.Ior, "ior", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopI_PopI, JavaStackBehavior.PushI);
            Irem = new JavaInstruction(JavaOpCode.Irem, "irem", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopI_PopI, JavaStackBehavior.PushI);
            Ireturn = new JavaInstruction(JavaOpCode.Ireturn, "ireturn", JavaOperandType.InlineNone, JavaFlowControl.Return, 1, JavaStackBehavior.PopI, JavaStackBehavior.Push0);
            Ishl = new JavaInstruction(JavaOpCode.Ishl, "ishl", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopI_PopI, JavaStackBehavior.PushI);
            Ishr = new JavaInstruction(JavaOpCode.Ishr, "ishr", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopI_PopI, JavaStackBehavior.PushI);
            Istore = new JavaInstruction(JavaOpCode.Istore, "istore", JavaOperandType.InlineVar, JavaFlowControl.Next, 2, JavaStackBehavior.PopI, JavaStackBehavior.Push0);
            Istore_0 = new JavaInstruction(JavaOpCode.Istore_0, "istore_0", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopI, JavaStackBehavior.Push0);
            Istore_1 = new JavaInstruction(JavaOpCode.Istore_1, "istore_1", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopI, JavaStackBehavior.Push0);
            Istore_2 = new JavaInstruction(JavaOpCode.Istore_2, "istore_2", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopI, JavaStackBehavior.Push0);
            Istore_3 = new JavaInstruction(JavaOpCode.Istore_3, "istore_3", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopI, JavaStackBehavior.Push0);
            Isub = new JavaInstruction(JavaOpCode.Isub, "isub", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopI_PopI, JavaStackBehavior.PushI);
            Iushr = new JavaInstruction(JavaOpCode.Iushr, "iushr", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopI_PopI, JavaStackBehavior.PushI);
            Ixor = new JavaInstruction(JavaOpCode.Ixor, "ixor", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopI_PopI, JavaStackBehavior.PushI);
            Jsr = new JavaInstruction(JavaOpCode.Jsr, "jsr", JavaOperandType.InlineShortBranchTarget, JavaFlowControl.Branch, 3, JavaStackBehavior.Pop0, JavaStackBehavior.PushRet);
            Jsr_w = new JavaInstruction(JavaOpCode.Jsr_w, "jsr_w", JavaOperandType.InlineBranchTarget, JavaFlowControl.Branch, 5, JavaStackBehavior.Pop0, JavaStackBehavior.PushRet);
            L2d = new JavaInstruction(JavaOpCode.L2d, "l2d", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopI8, JavaStackBehavior.PushR8);
            L2f = new JavaInstruction(JavaOpCode.L2f, "l2f", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopI8, JavaStackBehavior.PushR4);
            L2i = new JavaInstruction(JavaOpCode.L2i, "l2i", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopI8, JavaStackBehavior.PushI);
            Ladd = new JavaInstruction(JavaOpCode.Ladd, "ladd", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopI8_PopI8, JavaStackBehavior.PushI8);
            Laload = new JavaInstruction(JavaOpCode.Laload, "laload", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopRef_PopI, JavaStackBehavior.PushI8);
            Land = new JavaInstruction(JavaOpCode.Land, "land", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopI8_PopI8, JavaStackBehavior.PushI8);
            Lastore = new JavaInstruction(JavaOpCode.Lastore, "lastore", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopRef_PopI_PopI8, JavaStackBehavior.Push0);
            Lcmp = new JavaInstruction(JavaOpCode.Lcmp, "lcmp", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopI8_PopI8, JavaStackBehavior.PushI);
            Lconst_0 = new JavaInstruction(JavaOpCode.Lconst_0, "lconst_0", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.Pop0, JavaStackBehavior.PushI8);
            Lconst_1 = new JavaInstruction(JavaOpCode.Lconst_1, "lconst_1", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.Pop0, JavaStackBehavior.PushI8);
            Ldc = new JavaInstruction(JavaOpCode.Ldc, "ldc", JavaOperandType.InlineShortConst, JavaFlowControl.Next, 2, JavaStackBehavior.Pop0, JavaStackBehavior.Push1);
            Ldc_w = new JavaInstruction(JavaOpCode.Ldc_w, "ldc_w", JavaOperandType.InlineConst, JavaFlowControl.Next, 3, JavaStackBehavior.Pop0, JavaStackBehavior.Push1);
            Ldc2_w = new JavaInstruction(JavaOpCode.Ldc2_w, "ldc2_w", JavaOperandType.InlineConst, JavaFlowControl.Next, 3, JavaStackBehavior.Pop0, JavaStackBehavior.Push1);
            Ldiv = new JavaInstruction(JavaOpCode.Ldiv, "ldiv", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopI8_PopI8, JavaStackBehavior.PushI8);
            Lload = new JavaInstruction(JavaOpCode.Lload, "lload", JavaOperandType.InlineVar, JavaFlowControl.Next, 2, JavaStackBehavior.Pop0, JavaStackBehavior.PushI8);
            Lload_0 = new JavaInstruction(JavaOpCode.Lload_0, "lload_0", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.Pop0, JavaStackBehavior.PushI8);
            Lload_1 = new JavaInstruction(JavaOpCode.Lload_1, "lload_1", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.Pop0, JavaStackBehavior.PushI8);
            Lload_2 = new JavaInstruction(JavaOpCode.Lload_2, "lload_2", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.Pop0, JavaStackBehavior.PushI8);
            Lload_3 = new JavaInstruction(JavaOpCode.Lload_3, "lload_3", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.Pop0, JavaStackBehavior.PushI8);
            Lmul = new JavaInstruction(JavaOpCode.Lmul, "lmul", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopI8_PopI8, JavaStackBehavior.PushI8);
            Lneg = new JavaInstruction(JavaOpCode.Lneg, "lneg", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopI8, JavaStackBehavior.PushI8);
            Lookupswitch = new JavaInstruction(JavaOpCode.Lookupswitch, "lookupswitch", JavaOperandType.InlineSwitch, JavaFlowControl.Next, 0, JavaStackBehavior.PopI, JavaStackBehavior.Push0);
            Lor = new JavaInstruction(JavaOpCode.Lor, "lor", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopI8_PopI8, JavaStackBehavior.PushI8);
            Lrem = new JavaInstruction(JavaOpCode.Lrem, "lrem", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopI8_PopI8, JavaStackBehavior.PushI8);
            Lreturn = new JavaInstruction(JavaOpCode.Lreturn, "lreturn", JavaOperandType.InlineNone, JavaFlowControl.Return, 1, JavaStackBehavior.PopI8, JavaStackBehavior.Push0);
            Lshl = new JavaInstruction(JavaOpCode.Lshl, "lshl", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopI8_PopI, JavaStackBehavior.PushI8);
            Lshr = new JavaInstruction(JavaOpCode.Lshr, "lshr", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopI8_PopI, JavaStackBehavior.PushI8);
            Lstore = new JavaInstruction(JavaOpCode.Lstore, "lstore", JavaOperandType.InlineVar, JavaFlowControl.Next, 2, JavaStackBehavior.PopI8, JavaStackBehavior.Push0);
            Lstore_0 = new JavaInstruction(JavaOpCode.Lstore_0, "lstore_0", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopI8, JavaStackBehavior.Push0);
            Lstore_1 = new JavaInstruction(JavaOpCode.Lstore_1, "lstore_1", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopI8, JavaStackBehavior.Push0);
            Lstore_2 = new JavaInstruction(JavaOpCode.Lstore_2, "lstore_2", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopI8, JavaStackBehavior.Push0);
            Lstore_3 = new JavaInstruction(JavaOpCode.Lstore_3, "lstore_3", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopI8, JavaStackBehavior.Push0);
            Lsub = new JavaInstruction(JavaOpCode.Lsub, "lsub", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopI8_PopI8, JavaStackBehavior.PushI8);
            Lushr = new JavaInstruction(JavaOpCode.Lushr, "lushr", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopI8_PopI, JavaStackBehavior.PushI8);
            Lxor = new JavaInstruction(JavaOpCode.Lxor, "lxor", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopI8_PopI8, JavaStackBehavior.PushI8);
            Monitorenter = new JavaInstruction(JavaOpCode.Monitorenter, "monitorenter", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopRef, JavaStackBehavior.Push0);
            Monitorexit = new JavaInstruction(JavaOpCode.Monitorexit, "monitorexit", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopRef, JavaStackBehavior.Push0);
            Multianewarray = new JavaInstruction(JavaOpCode.Multianewarray, "multianewarray", JavaOperandType.InlineType_I1, JavaFlowControl.Next, 4, JavaStackBehavior.PopVar, JavaStackBehavior.PushRef);
            New = new JavaInstruction(JavaOpCode.New, "new", JavaOperandType.InlineType, JavaFlowControl.Call, 3, JavaStackBehavior.Pop0, JavaStackBehavior.PushRef);
            Newarray = new JavaInstruction(JavaOpCode.Newarray, "newarray", JavaOperandType.InlineArrayType, JavaFlowControl.Next, 2, JavaStackBehavior.PopI, JavaStackBehavior.PushRef);
            Nop = new JavaInstruction(JavaOpCode.Nop, "nop", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.Pop0, JavaStackBehavior.Push0);
            Pop = new JavaInstruction(JavaOpCode.Pop, "pop", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.Pop1, JavaStackBehavior.Push0);
            Pop2 = new JavaInstruction(JavaOpCode.Pop2, "pop2", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.Pop1, JavaStackBehavior.Push0);
            Putfield = new JavaInstruction(JavaOpCode.Putfield, "putfield", JavaOperandType.InlineField, JavaFlowControl.Next, 3, JavaStackBehavior.PopRef_Pop1, JavaStackBehavior.Push0);
            Putstatic = new JavaInstruction(JavaOpCode.Putstatic, "putstatic", JavaOperandType.InlineField, JavaFlowControl.Next, 3, JavaStackBehavior.Pop1, JavaStackBehavior.Push0);
            Ret = new JavaInstruction(JavaOpCode.Ret, "ret", JavaOperandType.InlineVar, JavaFlowControl.Return, 2, JavaStackBehavior.Pop0, JavaStackBehavior.Push0);
            Return = new JavaInstruction(JavaOpCode.Return, "return", JavaOperandType.InlineNone, JavaFlowControl.Return, 1, JavaStackBehavior.Pop0, JavaStackBehavior.Push0);
            Saload = new JavaInstruction(JavaOpCode.Saload, "saload", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopRef_PopI, JavaStackBehavior.PushI);
            Sastore = new JavaInstruction(JavaOpCode.Sastore, "sastore", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.PopRef_PopI_PopI, JavaStackBehavior.Push0);
            Sipush = new JavaInstruction(JavaOpCode.Sipush, "sipush", JavaOperandType.InlineI2, JavaFlowControl.Next, 3, JavaStackBehavior.Pop0, JavaStackBehavior.PushI);
            Swap = new JavaInstruction(JavaOpCode.Swap, "swap", JavaOperandType.InlineNone, JavaFlowControl.Next, 1, JavaStackBehavior.Pop1_Pop1, JavaStackBehavior.Push1);
            Tableswitch = new JavaInstruction(JavaOpCode.Tableswitch, "tableswitch", JavaOperandType.InlineTableSwitch, JavaFlowControl.Next, 0, JavaStackBehavior.PopI, JavaStackBehavior.Push0);
            Wide = new JavaInstruction(JavaOpCode.Wide, "wide", JavaOperandType.InlineNone, JavaFlowControl.Meta, 0, JavaStackBehavior.Pop0, JavaStackBehavior.Push0);
            Xxxunusedxxx1 = new JavaInstruction(JavaOpCode.Xxxunusedxxx1, "xxxunusedxxx1", JavaOperandType.InlineNone, JavaFlowControl.Special, 1, JavaStackBehavior.Pop0, JavaStackBehavior.Push0);

            JavaInstruction[] instructionLookup = new JavaInstruction[byte.MaxValue + 1];
            instructionLookup[0x32] = Aaload;
            instructionLookup[0x53] = Aastore;
            instructionLookup[0x01] = Aconst_null;
            instructionLookup[0x19] = Aload;
            instructionLookup[0x2A] = Aload_0;
            instructionLookup[0x2B] = Aload_1;
            instructionLookup[0x2C] = Aload_2;
            instructionLookup[0x2D] = Aload_3;
            instructionLookup[0xBD] = Anewarray;
            instructionLookup[0xB0] = Areturn;
            instructionLookup[0xBE] = Arraylength;
            instructionLookup[0x3A] = Astore;
            instructionLookup[0x4B] = Astore_0;
            instructionLookup[0x4C] = Astore_1;
            instructionLookup[0x4D] = Astore_2;
            instructionLookup[0x4E] = Astore_3;
            instructionLookup[0xBF] = Athrow;
            instructionLookup[0x33] = Baload;
            instructionLookup[0x54] = Bastore;
            instructionLookup[0x10] = Bipush;
            instructionLookup[0xCA] = Breakpoint;
            instructionLookup[0x34] = Caload;
            instructionLookup[0x55] = Castore;
            instructionLookup[0xC0] = Checkcast;
            instructionLookup[0x90] = D2f;
            instructionLookup[0x8E] = D2i;
            instructionLookup[0x8F] = D2l;
            instructionLookup[0x63] = Dadd;
            instructionLookup[0x31] = Daload;
            instructionLookup[0x52] = Dastore;
            instructionLookup[0x98] = Dcmpg;
            instructionLookup[0x97] = Dcmpl;
            instructionLookup[0x0E] = Dconst_0;
            instructionLookup[0x0F] = Dconst_1;
            instructionLookup[0x6F] = Ddiv;
            instructionLookup[0x18] = Dload;
            instructionLookup[0x26] = Dload_0;
            instructionLookup[0x27] = Dload_1;
            instructionLookup[0x28] = Dload_2;
            instructionLookup[0x29] = Dload_3;
            instructionLookup[0x6B] = Dmul;
            instructionLookup[0x77] = Dneg;
            instructionLookup[0x73] = Drem;
            instructionLookup[0xAF] = Dreturn;
            instructionLookup[0x39] = Dstore;
            instructionLookup[0x47] = Dstore_0;
            instructionLookup[0x48] = Dstore_1;
            instructionLookup[0x49] = Dstore_2;
            instructionLookup[0x4A] = Dstore_3;
            instructionLookup[0x67] = Dsub;
            instructionLookup[0x59] = Dup;
            instructionLookup[0x5A] = Dup_x1;
            instructionLookup[0x5B] = Dup_x2;
            instructionLookup[0x5C] = Dup2;
            instructionLookup[0x5D] = Dup2_x1;
            instructionLookup[0x5E] = Dup2_x2;
            instructionLookup[0x8D] = F2d;
            instructionLookup[0x8B] = F2i;
            instructionLookup[0x8C] = F2l;
            instructionLookup[0x62] = Fadd;
            instructionLookup[0x30] = Faload;
            instructionLookup[0x51] = Fastore;
            instructionLookup[0x96] = Fcmpg;
            instructionLookup[0x95] = Fcmpl;
            instructionLookup[0x0B] = Fconst_0;
            instructionLookup[0x0C] = Fconst_1;
            instructionLookup[0x0D] = Fconst_2;
            instructionLookup[0x6E] = Fdiv;
            instructionLookup[0x17] = Fload;
            instructionLookup[0x22] = Fload_0;
            instructionLookup[0x23] = Fload_1;
            instructionLookup[0x24] = Fload_2;
            instructionLookup[0x25] = Fload_3;
            instructionLookup[0x6A] = Fmul;
            instructionLookup[0x76] = Fneg;
            instructionLookup[0x72] = Frem;
            instructionLookup[0xAE] = Freturn;
            instructionLookup[0x38] = Fstore;
            instructionLookup[0x43] = Fstore_0;
            instructionLookup[0x44] = Fstore_1;
            instructionLookup[0x45] = Fstore_2;
            instructionLookup[0x46] = Fstore_3;
            instructionLookup[0x66] = Fsub;
            instructionLookup[0xB4] = Getfield;
            instructionLookup[0xB2] = Getstatic;
            instructionLookup[0xA7] = Goto;
            instructionLookup[0xC8] = Goto_w;
            instructionLookup[0x91] = I2b;
            instructionLookup[0x92] = I2c;
            instructionLookup[0x87] = I2d;
            instructionLookup[0x86] = I2f;
            instructionLookup[0x85] = I2l;
            instructionLookup[0x93] = I2s;
            instructionLookup[0x60] = Iadd;
            instructionLookup[0x2E] = Iaload;
            instructionLookup[0x7E] = Iand;
            instructionLookup[0x4F] = Iastore;
            instructionLookup[0x03] = Iconst_0;
            instructionLookup[0x04] = Iconst_1;
            instructionLookup[0x05] = Iconst_2;
            instructionLookup[0x06] = Iconst_3;
            instructionLookup[0x07] = Iconst_4;
            instructionLookup[0x08] = Iconst_5;
            instructionLookup[0x02] = Iconst_m1;
            instructionLookup[0x6C] = Idiv;
            instructionLookup[0xA5] = If_acmpeq;
            instructionLookup[0xA6] = If_acmpne;
            instructionLookup[0x9F] = If_icmpeq;
            instructionLookup[0xA2] = If_icmpge;
            instructionLookup[0xA3] = If_icmpgt;
            instructionLookup[0xA4] = If_icmple;
            instructionLookup[0xA1] = If_icmplt;
            instructionLookup[0xA0] = If_icmpne;
            instructionLookup[0x99] = Ifeq;
            instructionLookup[0x9C] = Ifge;
            instructionLookup[0x9D] = Ifgt;
            instructionLookup[0x9E] = Ifle;
            instructionLookup[0x9B] = Iflt;
            instructionLookup[0x9A] = Ifne;
            instructionLookup[0xC7] = Ifnonnull;
            instructionLookup[0xC6] = Ifnull;
            instructionLookup[0x84] = Iinc;
            instructionLookup[0x15] = Iload;
            instructionLookup[0x1A] = Iload_0;
            instructionLookup[0x1B] = Iload_1;
            instructionLookup[0x1C] = Iload_2;
            instructionLookup[0x1D] = Iload_3;
            instructionLookup[0xFE] = Impdep1;
            instructionLookup[0xFF] = Impdep2;
            instructionLookup[0x68] = Imul;
            instructionLookup[0x74] = Ineg;
            instructionLookup[0xC1] = Instanceof;
            instructionLookup[0xB9] = Invokeinterface;
            instructionLookup[0xB7] = Invokespecial;
            instructionLookup[0xB8] = Invokestatic;
            instructionLookup[0xB6] = Invokevirtual;
            instructionLookup[0x80] = Ior;
            instructionLookup[0x70] = Irem;
            instructionLookup[0xAC] = Ireturn;
            instructionLookup[0x78] = Ishl;
            instructionLookup[0x7A] = Ishr;
            instructionLookup[0x36] = Istore;
            instructionLookup[0x3B] = Istore_0;
            instructionLookup[0x3C] = Istore_1;
            instructionLookup[0x3D] = Istore_2;
            instructionLookup[0x3E] = Istore_3;
            instructionLookup[0x64] = Isub;
            instructionLookup[0x7C] = Iushr;
            instructionLookup[0x82] = Ixor;
            instructionLookup[0xA8] = Jsr;
            instructionLookup[0xC9] = Jsr_w;
            instructionLookup[0x8A] = L2d;
            instructionLookup[0x89] = L2f;
            instructionLookup[0x88] = L2i;
            instructionLookup[0x61] = Ladd;
            instructionLookup[0x2F] = Laload;
            instructionLookup[0x7F] = Land;
            instructionLookup[0x50] = Lastore;
            instructionLookup[0x94] = Lcmp;
            instructionLookup[0x09] = Lconst_0;
            instructionLookup[0x0A] = Lconst_1;
            instructionLookup[0x12] = Ldc;
            instructionLookup[0x13] = Ldc_w;
            instructionLookup[0x14] = Ldc2_w;
            instructionLookup[0x6D] = Ldiv;
            instructionLookup[0x16] = Lload;
            instructionLookup[0x1E] = Lload_0;
            instructionLookup[0x1F] = Lload_1;
            instructionLookup[0x20] = Lload_2;
            instructionLookup[0x21] = Lload_3;
            instructionLookup[0x69] = Lmul;
            instructionLookup[0x75] = Lneg;
            instructionLookup[0xAB] = Lookupswitch;
            instructionLookup[0x81] = Lor;
            instructionLookup[0x71] = Lrem;
            instructionLookup[0xAD] = Lreturn;
            instructionLookup[0x79] = Lshl;
            instructionLookup[0x7B] = Lshr;
            instructionLookup[0x37] = Lstore;
            instructionLookup[0x3F] = Lstore_0;
            instructionLookup[0x40] = Lstore_1;
            instructionLookup[0x41] = Lstore_2;
            instructionLookup[0x42] = Lstore_3;
            instructionLookup[0x65] = Lsub;
            instructionLookup[0x7D] = Lushr;
            instructionLookup[0x83] = Lxor;
            instructionLookup[0xC2] = Monitorenter;
            instructionLookup[0xC3] = Monitorexit;
            instructionLookup[0xC5] = Multianewarray;
            instructionLookup[0xBB] = New;
            instructionLookup[0xBC] = Newarray;
            instructionLookup[0x00] = Nop;
            instructionLookup[0x57] = Pop;
            instructionLookup[0x58] = Pop2;
            instructionLookup[0xB5] = Putfield;
            instructionLookup[0xB3] = Putstatic;
            instructionLookup[0xA9] = Ret;
            instructionLookup[0xB1] = Return;
            instructionLookup[0x35] = Saload;
            instructionLookup[0x56] = Sastore;
            instructionLookup[0x11] = Sipush;
            instructionLookup[0x5F] = Swap;
            instructionLookup[0xAA] = Tableswitch;
            instructionLookup[0xC4] = Wide;
            instructionLookup[0xBA] = Xxxunusedxxx1;

            _instructionLookup = new ReadOnlyCollection<JavaInstruction>(instructionLookup);
        }

        public static ReadOnlyCollection<JavaInstruction> InstructionLookup
        {
            get
            {
                return _instructionLookup;
            }
        }
    }
}
