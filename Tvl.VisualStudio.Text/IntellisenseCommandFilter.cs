namespace Tvl.VisualStudio.Text
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;
    using Tvl.VisualStudio.Shell;

    public class IntellisenseCommandFilter : TextViewCommandFilter
    {
        public IntellisenseCommandFilter(IVsTextView textViewAdapter)
            : base(textViewAdapter)
        {
        }

        public virtual bool SupportsCommenting
        {
            get
            {
                return true;
            }
        }

        public virtual bool SupportsFormatting
        {
            get
            {
                return false;
            }
        }

        public virtual bool SupportsCompletion
        {
            get
            {
                return false;
            }
        }

        public virtual bool SupportsSignatureHelp
        {
            get
            {
                return false;
            }
        }

        public virtual bool SupportsQuickInfo
        {
            get
            {
                return false;
            }
        }

        public virtual bool SupportsGotoDefinition
        {
            get
            {
                return false;
            }
        }

        public virtual bool SupportsGotoDeclaration
        {
            get
            {
                return false;
            }
        }

        public virtual bool SupportsGotoReference
        {
            get
            {
                return false;
            }
        }

        protected override bool HandlePreExec(ref Guid commandGroup, uint commandId, uint executionOptions, IntPtr pvaIn, IntPtr pvaOut)
        {
            return base.HandlePreExec(ref commandGroup, commandId, executionOptions, pvaIn, pvaOut);
        }

        protected override void HandlePostExec(ref Guid commandGroup, uint commandId, uint executionOptions, IntPtr pvaIn, IntPtr pvaOut)
        {
            base.HandlePostExec(ref commandGroup, commandId, executionOptions, pvaIn, pvaOut);
        }

        protected override CommandStatus QueryCommandStatus(ref Guid group, uint id)
        {
            if (group == VsMenus.guidStandardCommandSet97)
            {
                switch ((VSConstants.VSStd97CmdID)id)
                {
                case VSConstants.VSStd97CmdID.GotoDecl:
                    if (SupportsGotoDeclaration)
                        return CommandStatus.Supported | CommandStatus.Enabled;
                    break;

                case VSConstants.VSStd97CmdID.GotoDefn:
                    if (SupportsGotoDefinition)
                        return CommandStatus.Supported | CommandStatus.Enabled;
                    break;

                case VSConstants.VSStd97CmdID.GotoRef:
                    if (SupportsGotoReference)
                        return CommandStatus.Supported | CommandStatus.Enabled;
                    break;

                default:
                    break;
                }
            }
            else if (group == VsMenus.guidStandardCommandSet2K)
            {
                switch ((VSConstants.VSStd2KCmdID)id)
                {
                case VSConstants.VSStd2KCmdID.FORMATDOCUMENT:
                case VSConstants.VSStd2KCmdID.FORMATSELECTION:
                    if (SupportsFormatting)
                        return CommandStatus.Supported | CommandStatus.Enabled;
                    break;

                case VSConstants.VSStd2KCmdID.COMMENT_BLOCK:
                case VSConstants.VSStd2KCmdID.UNCOMMENT_BLOCK:
                case VSConstants.VSStd2KCmdID.COMMENTBLOCK:
                case VSConstants.VSStd2KCmdID.UNCOMMENTBLOCK:
                    if (SupportsCommenting)
                        return CommandStatus.Supported | CommandStatus.Enabled;
                    break;

                case VSConstants.VSStd2KCmdID.SHOWMEMBERLIST:
                case VSConstants.VSStd2KCmdID.COMPLETEWORD:
                    if (SupportsCompletion)
                        return CommandStatus.Supported | CommandStatus.Enabled;
                    break;

                case VSConstants.VSStd2KCmdID.PARAMINFO:
                    if (SupportsSignatureHelp)
                        return CommandStatus.Supported | CommandStatus.Enabled;
                    break;

                case VSConstants.VSStd2KCmdID.QUICKINFO:
                    if (SupportsQuickInfo)
                        return CommandStatus.Supported | CommandStatus.Enabled;
                    break;

                //case VSConstants.VSStd2KCmdID.OUTLN_START_AUTOHIDING:

                default:
                    break;
                }
            }

            return CommandStatus.None;
        }
    }
}
