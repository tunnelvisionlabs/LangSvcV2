namespace Tvl.VisualStudio.Text
{
    using System;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Tvl.Events;

    public class CodeWindowManager : IVsCodeWindowManager
    {
        private readonly IVsCodeWindow _codeWindow;
        private readonly SVsServiceProvider _serviceProvider;
        private readonly LanguagePreferences _languagePreferences;
        private IVsDropdownBarClient _dropdownBarClient;

        public CodeWindowManager(IVsCodeWindow codeWindow, SVsServiceProvider serviceProvider, LanguagePreferences languagePreferences)
        {
            if (codeWindow == null)
                throw new ArgumentNullException("codeWindow");
            if (serviceProvider == null)
                throw new ArgumentNullException("serviceProvider");
            if (languagePreferences == null)
                throw new ArgumentNullException("languagePreferences");

            _codeWindow = codeWindow;
            _serviceProvider = serviceProvider;
            _languagePreferences = languagePreferences;
            _languagePreferences.PreferencesChanged += WeakEvents.AsWeak(HandleLanguagePreferencesChanged, handler => _languagePreferences.PreferencesChanged -= handler);
        }

        public IVsCodeWindow CodeWindow
        {
            get
            {
                return _codeWindow;
            }
        }

        public SVsServiceProvider ServiceProvider
        {
            get
            {
                return _serviceProvider;
            }
        }

        public LanguagePreferences LanguagePreferences
        {
            get
            {
                return _languagePreferences;
            }
        }

        public virtual int AddAdornments()
        {
            IVsTextView textView;
            if (ErrorHandler.Succeeded(CodeWindow.GetPrimaryView(out textView)) && textView != null)
                ErrorHandler.ThrowOnFailure(OnNewView(textView));
            if (ErrorHandler.Succeeded(CodeWindow.GetSecondaryView(out textView)) && textView != null)
                ErrorHandler.ThrowOnFailure(OnNewView(textView));

            int comboBoxCount;
            IVsDropdownBarClient client;
            if (LanguagePreferences.ShowDropdownBar && TryCreateDropdownBarClient(out comboBoxCount, out client))
            {
                ErrorHandler.ThrowOnFailure(AddDropdownBar(comboBoxCount, client));
                _dropdownBarClient = client;
            }

            return VSConstants.S_OK;
        }

        public virtual int OnNewView(IVsTextView pView)
        {
            return VSConstants.S_OK;
        }

        public virtual int RemoveAdornments()
        {
            return RemoveDropdownBar();
        }

        protected virtual bool TryCreateDropdownBarClient(out int comboBoxCount, out IVsDropdownBarClient client)
        {
            comboBoxCount = 0;
            client = null;
            return false;
        }

        protected virtual int AddDropdownBar(int comboBoxCount, IVsDropdownBarClient client)
        {
            IVsDropdownBarManager manager = CodeWindow as IVsDropdownBarManager;
            if (manager == null)
                throw new NotSupportedException();

            IVsDropdownBar dropdownBar;
            int hr = manager.GetDropdownBar(out dropdownBar);
            if (ErrorHandler.Succeeded(hr) && dropdownBar != null)
            {
                hr = manager.RemoveDropdownBar();
                if (ErrorHandler.Failed(hr))
                    return hr;
            }

            return manager.AddDropdownBar(comboBoxCount, client);
        }

        protected virtual int RemoveDropdownBar()
        {
            IVsDropdownBarManager manager = CodeWindow as IVsDropdownBarManager;
            if (manager == null)
                return VSConstants.E_FAIL;

            IVsDropdownBar dropdownBar;
            int hr = manager.GetDropdownBar(out dropdownBar);
            if (ErrorHandler.Succeeded(hr) && dropdownBar != null)
            {
                IVsDropdownBarClient client;
                hr = dropdownBar.GetClient(out client);
                if (ErrorHandler.Succeeded(hr) && client == _dropdownBarClient)
                {
                    _dropdownBarClient = null;
                    return manager.RemoveDropdownBar();
                }
            }

            _dropdownBarClient = null;
            return VSConstants.S_OK;
        }

        protected virtual void HandleLanguagePreferencesChanged(object sender, EventArgs e)
        {
            int comboBoxCount;
            IVsDropdownBarClient client;
            if (_dropdownBarClient == null && LanguagePreferences.ShowDropdownBar && TryCreateDropdownBarClient(out comboBoxCount, out client))
            {
                ErrorHandler.ThrowOnFailure(AddDropdownBar(comboBoxCount, client));
                _dropdownBarClient = client;
            }
            else if (_dropdownBarClient != null && !LanguagePreferences.ShowDropdownBar)
            {
                ErrorHandler.ThrowOnFailure(RemoveDropdownBar());
            }
        }
    }
}
