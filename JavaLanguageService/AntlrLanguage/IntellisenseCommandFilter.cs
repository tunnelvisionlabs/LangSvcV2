namespace JavaLanguageService.AntlrLanguage
{
    using System;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Tvl.VisualStudio.Shell;
    using Tvl.VisualStudio.Text;
    using VsCommands2K = Microsoft.VisualStudio.VSConstants.VSStd2KCmdID;

    public class IntellisenseCommandFilter : TextViewCommandFilter
    {
        public IntellisenseCommandFilter(IVsTextView textViewAdapter)
            : base(textViewAdapter)
        {
        }

        protected override bool HandlePreExec(ref Guid commandGroup, uint commandId, uint executionOptions, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (commandGroup == VSConstants.VSStd2K)
            {
                switch ((VsCommands2K)commandId)
                {
                case VsCommands2K.COMPLETEWORD:
                    return true;

                default:
                    break;
                }
            }

            return base.HandlePreExec(ref commandGroup, commandId, executionOptions, pvaIn, pvaOut);
        }

        protected override CommandStatus QueryCommandStatus(ref Guid group, uint id)
        {
            if (group == VSConstants.VSStd2K)
            {
                switch ((VsCommands2K)id)
                {
                case VsCommands2K.COMPLETEWORD:
                    return CommandStatus.Supported | CommandStatus.Enabled;

                default:
                    break;
                }
            }

            return base.QueryCommandStatus(ref group, id);
        }
    }
}
