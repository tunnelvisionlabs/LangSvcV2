namespace Tvl.VisualStudio.Language.Java.Project.PropertyPages
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using Microsoft.Build.Evaluation;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.OLE.Interop;
    using Microsoft.VisualStudio.Project;
    using Microsoft.VisualStudio.Shell.Interop;

    public abstract class JavaPropertyPage : IPropertyPage
    {
        private const int Win32SwHide = 0;

        private bool _active;
        private bool _isDirty;
        private JavaPropertyPagePanel _propertyPagePanel;
        private string _pageName;
        private IPropertyPageSite _pageSite;
        private JavaProjectNode _project;
        private ProjectConfig[] _projectConfigs;
        private bool _initializing;

        public JavaPropertyPage()
        {
        }

        [Browsable(false)]
        [AutomationBrowsable(false)]
        public bool IsDirty
        {
            get
            {
                return _isDirty;
            }

            set
            {
                if (_isDirty != value)
                {
                    _isDirty = value;
                    if (_pageSite != null)
                    {
                        _pageSite.OnStatusChange((uint)(_isDirty ? PropPageStatus.Dirty : PropPageStatus.Clean));
                    }
                }
            }
        }

        [Browsable(false)]
        [AutomationBrowsable(false)]
        public string PageName
        {
            get
            {
                return _pageName;
            }

            set
            {
                _pageName = value;
            }
        }

        public JavaProjectNode ProjectManager
        {
            get
            {
                return _project;
            }
        }

        internal System.IServiceProvider Site
        {
            get
            {
                return _project.Site;
            }
        }

        protected JavaPropertyPagePanel PropertyPagePanel
        {
            get
            {
                return _propertyPagePanel;
            }

            private set
            {
                _propertyPagePanel = value;
            }
        }

        void IPropertyPage.Activate(IntPtr hWndParent, RECT[] pRect, int bModal)
        {
            // create the panel control
            if (PropertyPagePanel == null)
                PropertyPagePanel = CreatePropertyPagePanel();

            // we need to create the control so the handle is valid
            PropertyPagePanel.CreateControl();

            // set our parent
            NativeMethods.SetParent(PropertyPagePanel.Handle, hWndParent);

            // set our initial size
            ResizeContents(pRect[0]);

            _active = true;
            _initializing = true;
            BindProperties();
            _initializing = false;
            IsDirty = false;
        }

        int IPropertyPage.Apply()
        {
            if (IsDirty)
            {
                // When a property page is being initialized, the corresponding controls will fire events for their values being changed.
                // We want to ignore them, since they don't represent any real changes done by the user.
                bool applied = true;
                if (!_initializing)
                {
                    applied = ApplyChanges();
                    IsDirty = !applied;
                }

                return (applied ? VSConstants.S_OK : VSConstants.S_FALSE);
            }

            return VSConstants.S_OK;
        }

        void IPropertyPage.Deactivate()
        {
            if (PropertyPagePanel != null)
            {
                PropertyPagePanel.Dispose();
                PropertyPagePanel = null;
            }

            _active = false;
        }

        void IPropertyPage.GetPageInfo(PROPPAGEINFO[] pPageInfo)
        {
            if (pPageInfo == null)
                throw new ArgumentNullException("pPageInfo");

            if (PropertyPagePanel == null)
                PropertyPagePanel = CreatePropertyPagePanel();

            PROPPAGEINFO info = new PROPPAGEINFO()
            {
                cb = (uint)Marshal.SizeOf(typeof(PROPPAGEINFO)),
                dwHelpContext = 0,
                pszDocString = null,
                pszHelpFile = null,
                pszTitle = PageName,
                SIZE =
                {
                    cx = PropertyPagePanel.Width,
                    cy = PropertyPagePanel.Height
                }
            };

            pPageInfo[0] = info;
        }

        void IPropertyPage.Help(string pszHelpDir)
        {
        }

        int IPropertyPage.IsPageDirty()
        {
            return (IsDirty ? VSConstants.S_OK : VSConstants.S_FALSE);
        }

        void IPropertyPage.Move(RECT[] pRect)
        {
            ResizeContents(pRect[0]);
        }

        void IPropertyPage.SetObjects(uint cObjects, object[] ppunk)
        {
            if (cObjects == 0)
            {
                if (_project != null)
                {
                    //_project.CurrentOutputTypeChanging -= new PropertyChangingEventHandler( HandleOutputTypeChanging );
                    _project = null;
                }
                return;
            }

            if (ppunk[0] is ProjectConfig)
            {
                List<ProjectConfig> configs = new List<ProjectConfig>();

                for (int i = 0; i < cObjects; i++)
                {
                    ProjectConfig config = (ProjectConfig)ppunk[i];
                    if (_project == null)
                    {
                        _project = config.ProjectMgr as JavaProjectNode;
                        //_project.CurrentOutputTypeChanging += new PropertyChangingEventHandler( HandleOutputTypeChanging );
                    }

                    configs.Add(config);
                }

                _projectConfigs = configs.ToArray();
            }
            else if (ppunk[0] is NodeProperties)
            {
                if (_project == null)
                {
                    _project = (ppunk[0] as NodeProperties).Node.ProjectMgr as JavaProjectNode;
                    //_project.CurrentOutputTypeChanging += new PropertyChangingEventHandler( HandleOutputTypeChanging );
                }

                Dictionary<string, ProjectConfig> configsMap = new Dictionary<string, ProjectConfig>();

                for (int i = 0; i < cObjects; i++)
                {
                    NodeProperties property = (NodeProperties)ppunk[i];
                    IVsCfgProvider provider;
                    ErrorHandler.ThrowOnFailure(property.Node.ProjectMgr.GetCfgProvider(out provider));
                    uint[] expected = new uint[1];
                    ErrorHandler.ThrowOnFailure(provider.GetCfgs(0, null, expected, null));
                    if (expected[0] > 0)
                    {
                        ProjectConfig[] configs = new ProjectConfig[expected[0]];
                        uint[] actual = new uint[1];
                        int hr = provider.GetCfgs(expected[0], configs, actual, null);
                        if (hr != VSConstants.S_OK)
                            Marshal.ThrowExceptionForHR(hr);

                        foreach (ProjectConfig config in configs)
                        {
                            if (!configsMap.ContainsKey(config.ConfigName))
                                configsMap.Add(config.ConfigName, config);
                        }
                    }
                }

                if (configsMap.Count > 0)
                {
                    if (_projectConfigs == null)
                        _projectConfigs = new ProjectConfig[configsMap.Keys.Count];

                    configsMap.Values.CopyTo(_projectConfigs, 0);
                }
            }

            if (_active && _project != null)
            {
                BindProperties();
                IsDirty = false;
            }
        }

        void IPropertyPage.SetPageSite(IPropertyPageSite pPageSite)
        {
            _pageSite = pPageSite;
        }

        void IPropertyPage.Show(uint nCmdShow)
        {
            if (PropertyPagePanel != null)
            {
                if (nCmdShow == Win32SwHide)
                    PropertyPagePanel.Hide();
                else
                    PropertyPagePanel.Show();
            }
        }

        int IPropertyPage.TranslateAccelerator(MSG[] pMsg)
        {
            if (pMsg == null)
                throw new ArgumentNullException("pMsg");

            return VSConstants.S_FALSE;
        }

        public string GetConfigProperty(string propertyName, ProjectPropertyStorage propertyStorage = ProjectPropertyStorage.ProjectFile)
        {
            string unifiedResult = string.Empty;

            if (ProjectManager != null)
            {
                bool cacheNeedReset = true;

                for (int i = 0; i < _projectConfigs.Length; i++)
                {
                    ProjectConfig config = _projectConfigs[i];
                    JavaProjectConfig ucconfig = config as JavaProjectConfig;
                    string property;
                    if (ucconfig != null)
                    {
                        property = ucconfig.GetConfigurationProperty(propertyName, cacheNeedReset, propertyStorage);
                    }
                    else if (propertyStorage == ProjectPropertyStorage.ProjectFile)
                    {
                        property = config.GetConfigurationProperty(propertyName, cacheNeedReset);
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }

                    cacheNeedReset = false;

                    if (property != null)
                    {
                        string text = property.Trim();

                        if (i == 0)
                        {
                            unifiedResult = text;
                        }
                        else if (unifiedResult != text)
                        {
                            unifiedResult = string.Empty;
                            break;
                        }
                    }
                }
            }

            return unifiedResult;
        }

        public bool GetConfigPropertyBoolean(string propertyName, ProjectPropertyStorage propertyStorage = ProjectPropertyStorage.ProjectFile)
        {
            string value = GetConfigProperty(propertyName, propertyStorage);

            bool converted;
            if (string.IsNullOrEmpty(value) || !bool.TryParse(value, out converted))
                return false;

            return converted;
        }

        public int GetConfigPropertyInt32(string propertyName, ProjectPropertyStorage propertyStorage = ProjectPropertyStorage.ProjectFile)
        {
            string value = GetConfigProperty(propertyName, propertyStorage);

            int converted;
            if (string.IsNullOrEmpty(value) || !int.TryParse(value, out converted))
                return JavaProjectFileConstants.UnspecifiedValue;

            return converted;
        }

        public string GetProperty(string propertyName)
        {
            if (ProjectManager != null)
            {
                string property = ProjectManager.GetProjectProperty(propertyName, true);
                if (property != null)
                    return property;
            }

            return string.Empty;
        }

        public void SetConfigProperty(string propertyName, bool propertyValue, ProjectPropertyStorage propertyStorage = ProjectPropertyStorage.ProjectFile)
        {
            SetConfigProperty(propertyName, propertyValue.ToString(), propertyStorage);
        }

        public void SetConfigProperty(string propertyName, int propertyValue, ProjectPropertyStorage propertyStorage = ProjectPropertyStorage.ProjectFile)
        {
            SetConfigProperty(propertyName, propertyValue.ToString(CultureInfo.InvariantCulture), propertyStorage);
        }

        public void SetConfigProperty(string propertyName, string propertyValue, ProjectPropertyStorage propertyStorage = ProjectPropertyStorage.ProjectFile)
        {
            if (propertyValue == null)
            {
                propertyValue = string.Empty;
            }

            if (ProjectManager != null)
            {
                if (propertyStorage == ProjectPropertyStorage.UserFile && ProjectManager.UserBuildProject == null)
                    ProjectManager.CreateUserBuildProject();

                Project buildProject = (propertyStorage == ProjectPropertyStorage.ProjectFile) ? ProjectManager.BuildProject : ProjectManager.UserBuildProject;

                for (int i = 0, n = _projectConfigs.Length; i < n; i++)
                {
                    ProjectConfig config = _projectConfigs[i];
                    JavaProjectConfig ucconfig = config as JavaProjectConfig;
                    if (ucconfig != null)
                    {
                        ucconfig.SetConfigurationProperty(propertyName, propertyValue, propertyStorage);
                    }
                    else if (propertyStorage == ProjectPropertyStorage.ProjectFile)
                    {
                        config.SetConfigurationProperty(propertyName, propertyValue);
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }
                }

                ProjectManager.SetProjectFileDirty(true);
            }
        }

        public void SetProperty(string propertyName, string propertyValue, ProjectPropertyStorage propertyStorage)
        {
            if (propertyValue == null)
            {
                propertyValue = string.Empty;
            }

            if (ProjectManager != null)
            {
                if (propertyStorage == ProjectPropertyStorage.UserFile)
                {
                    if (ProjectManager.UserBuildProject == null)
                    {
                        ProjectManager.CreateUserBuildProject();
                    }

                    Project buildProject = ProjectManager.UserBuildProject;
                    buildProject.SetProperty(propertyName, propertyValue);
                    ProjectManager.BuildProject.SetGlobalProperty(propertyName, propertyValue);

                    ProjectManager.SetProjectFileDirty(true);
                }
                else
                {
                    ProjectManager.SetProjectProperty(propertyName, propertyValue);
                }
            }
        }

        public void SetProperty(string propertyName, string propertyValue, string condition, bool treatPropertyValueAsLiteral)
        {
            if (ProjectManager != null)
                ProjectManager.SetProjectProperty(propertyName, propertyValue ?? string.Empty, condition, treatPropertyValueAsLiteral);
        }

        internal void UpdateStatus()
        {
            if (_pageSite != null)
            {
                _pageSite.OnStatusChange((uint)(IsDirty ? PropPageStatus.Dirty | PropPageStatus.Validate : PropPageStatus.Clean));
            }
        }

        protected abstract bool ApplyChanges();

        protected abstract void BindProperties();

        protected abstract JavaPropertyPagePanel CreatePropertyPagePanel();

        protected virtual void HandleOutputTypeChanging(object source, PropertyChangingEventArgs e)
        {
            // Do nothing here. Subclasses may optionally override.
        }

        private void ResizeContents(RECT newBounds)
        {
            if (PropertyPagePanel != null && PropertyPagePanel.IsHandleCreated)
            {
                // Visual Studio sends us the size of the area in which it wants us to size.
                // However, we don't want to size smaller than the property page's minimum
                // size, which scales according to the screen DPI.
                PropertyPagePanel.Bounds = new Rectangle(
                    newBounds.left,
                    newBounds.top,
                    Math.Max(newBounds.right - newBounds.left, PropertyPagePanel.MinimumSize.Width),
                    Math.Max(newBounds.bottom - newBounds.top, PropertyPagePanel.MinimumSize.Height));
            }
        }
    }
}
