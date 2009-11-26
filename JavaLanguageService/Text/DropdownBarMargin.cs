namespace JavaLanguageService.Text
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using JavaLanguageService.Text.Language;
    using System.Windows;

    internal class DropdownBarMargin : IWpfTextViewMargin
    {
        private readonly IWpfTextView _wpfTextView;
        private readonly ILanguageElementManager _manager;
        private bool _disposed;

        public DropdownBarMargin(IWpfTextView iWpfTextView, ILanguageElementManager manager)
        {
            this._wpfTextView = iWpfTextView;
            this._manager = manager;
        }

        public FrameworkElement VisualElement
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool Enabled
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            throw new NotImplementedException();
        }

        public double MarginSize
        {
            get
            {
                return VisualElement.ActualHeight;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
            }
        }
    }
}
