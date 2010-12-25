namespace Tvl.VisualStudio.Text
{
    public enum IntellisenseInvocationType
    {
        Default,
        IdentifierChar,
        Sharp, // preprocessor directives
        BackspaceDeleteOrBackTab,
        Space,
        //QMark,
        ShowMemberList,
    }
}
