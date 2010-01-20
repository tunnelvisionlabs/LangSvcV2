namespace Tvl.VisualStudio.Text.Navigation.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Microsoft.VisualStudio;
    using System.Windows.Forms;
    using System.Windows.Input;
    using Microsoft.VisualStudio.Text.Editor;

    internal sealed class EditorNavigationAdapter : IVsDropdownBarClient, IVsDropdownBarClientEx
    {
        private IWpfTextView _wpfTextView;
        private IEditorNavigationType[] _navigationTypes;
        private IEditorNavigationTarget[][] _navigationTargets;
        private ImageList _imageList;

        private IVsDropdownBar DropdownBar
        {
            get;
            set;
        }

        public int GetComboAttributes(int iCombo, out uint pcEntries, out uint puEntryType, out IntPtr phImageList)
        {
            pcEntries = (uint)_navigationTargets[iCombo].Length;

            DROPDOWNENTRYTYPE entryType = DROPDOWNENTRYTYPE.ENTRY_IMAGE | DROPDOWNENTRYTYPE.ENTRY_TEXT;
            puEntryType = (uint)entryType;

            phImageList = _imageList.Handle;
            return VSConstants.S_OK;
        }

        public int GetComboTipText(int iCombo, out string pbstrText)
        {
            pbstrText = _navigationTypes[iCombo].Definition.DisplayName;
            return VSConstants.S_OK;
        }

        public int GetEntryAttributes(int iCombo, int iIndex, out uint pAttr)
        {
            DROPDOWNFONTATTR attributes = DROPDOWNFONTATTR.FONTATTR_PLAIN;
            pAttr = (uint)attributes;
            return VSConstants.S_OK;
        }

        public int GetEntryImage(int iCombo, int iIndex, out int piImageIndex)
        {
            piImageIndex = -1;
            return VSConstants.S_FALSE;
        }

        public int GetEntryText(int iCombo, int iIndex, out string ppszText)
        {
            ppszText = _navigationTargets[iCombo][iIndex].Name;
            return VSConstants.S_OK;
        }

        public int OnComboGetFocus(int iCombo)
        {
            return VSConstants.S_OK;
        }

        public int OnItemChosen(int iCombo, int iIndex)
        {
            IEditorNavigationTarget target = _navigationTargets[iCombo][iIndex];
            if (target != null)
            {
                var seek = target.Seek.Snapshot == null ? target.Span : target.Seek;
                _wpfTextView.Caret.MoveTo(seek.Start);
                _wpfTextView.Selection.Select(seek, false);
                _wpfTextView.ViewScroller.EnsureSpanVisible(target.Seek);
                Keyboard.Focus(_wpfTextView.VisualElement);
            }

            return VSConstants.S_OK;
        }

        public int OnItemSelected(int iCombo, int iIndex)
        {
            return VSConstants.S_OK;
        }

        public int SetDropdownBar(IVsDropdownBar pDropdownBar)
        {
            this.DropdownBar = pDropdownBar;

            if (pDropdownBar == null && _imageList != null)
            {
                _imageList.Dispose();
                _imageList = null;
            }

            return VSConstants.S_OK;
        }

        public int GetEntryIndent(int iCombo, int iIndex, out uint pIndent)
        {
            pIndent = 0;
            return VSConstants.S_OK;
        }
    }
}
