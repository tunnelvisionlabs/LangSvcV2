namespace Tvl.VisualStudio.Language.Java.Debugger.Analysis
{
    public enum JavaOperandType
    {
        InlineNone,

        InlineI1,
        InlineI2,

        InlineShortBranchTarget,
        InlineBranchTarget,

        InlineSwitch,
        InlineTableSwitch,

        InlineShortConst,
        InlineConst,

        InlineVar,
        InlineField,
        InlineMethod,
        InlineType,
        InlineArrayType,

        InlineVar_I1,
        InlineMethod_I1_0,
        InlineType_I1,
    }
}
