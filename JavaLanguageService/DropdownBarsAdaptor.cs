namespace JavaLanguageService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.TextManager.Interop;

    internal sealed class DropdownBarsAdaptor : IVsDropdownBarClient, IVsDropdownBarClientEx
    {
        public int GetComboAttributes(int iCombo, out uint pcEntries, out uint puEntryType, out IntPtr phImageList)
        {
            throw new NotImplementedException();
        }

        public int GetComboTipText(int iCombo, out string pbstrText)
        {
            throw new NotImplementedException();
        }

        public int GetEntryAttributes(int iCombo, int iIndex, out uint pAttr)
        {
            throw new NotImplementedException();
        }

        public int GetEntryImage(int iCombo, int iIndex, out int piImageIndex)
        {
            throw new NotImplementedException();
        }

        public int GetEntryText(int iCombo, int iIndex, out string ppszText)
        {
            throw new NotImplementedException();
        }

        public int OnComboGetFocus(int iCombo)
        {
            throw new NotImplementedException();
        }

        public int OnItemChosen(int iCombo, int iIndex)
        {
            throw new NotImplementedException();
        }

        public int OnItemSelected(int iCombo, int iIndex)
        {
            throw new NotImplementedException();
        }

        public int SetDropdownBar(IVsDropdownBar pDropdownBar)
        {
            throw new NotImplementedException();
        }

        public int GetEntryIndent(int iCombo, int iIndex, out uint pIndent)
        {
            throw new NotImplementedException();
        }
    }
}
