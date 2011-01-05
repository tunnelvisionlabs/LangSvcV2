namespace Tvl.VisualStudio.Language.Alloy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Microsoft.VisualStudio;

    public class CodeWindowManager : IVsCodeWindowManager
    {
        int IVsCodeWindowManager.AddAdornments()
        {
            return VSConstants.S_OK;
        }

        int IVsCodeWindowManager.OnNewView(IVsTextView pView)
        {
            return VSConstants.S_OK;
        }

        int IVsCodeWindowManager.RemoveAdornments()
        {
            return VSConstants.S_OK;
        }
    }
}
