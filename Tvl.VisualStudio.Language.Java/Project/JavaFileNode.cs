namespace Tvl.VisualStudio.Language.Java.Project
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Project;
    using VsCommands2K = Microsoft.VisualStudio.VSConstants.VSStd2KCmdID;
    using VSConstants = Microsoft.VisualStudio.VSConstants;

    [ComVisible(true)]
    public class JavaFileNode : FileNode
    {
        public JavaFileNode(ProjectNode root, ProjectElement element)
            : base(root, element)
        {
        }

        protected override NodeProperties CreatePropertiesObject()
        {
            return new JavaFileNodeProperties(this);
        }

        protected override int QueryStatusOnNode(Guid cmdGroup, uint cmd, IntPtr pCmdText, ref QueryStatusResult result)
        {
            if (cmdGroup == VsMenus.guidStandardCommandSet2K)
            {
                switch ((VsCommands2K)cmd)
                {
                case VsCommands2K.INCLUDEINPROJECT:
                case VsCommands2K.EXCLUDEFROMPROJECT:
                    result |= QueryStatusResult.NOTSUPPORTED;
                    return VSConstants.S_OK;

                default:
                    break;
                }
            }

            return base.QueryStatusOnNode(cmdGroup, cmd, pCmdText, ref result);
        }
    }
}
