namespace Tvl.VisualStudio.Language.Java.Project
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Project;
    using Tvl.VisualStudio.Shell.Extensions;
    using SVSMDCodeDomProvider = Microsoft.VisualStudio.Shell.Interop.SVSMDCodeDomProvider;
    using OAVSProject = Microsoft.VisualStudio.Project.Automation.OAVSProject;
    using Project = Microsoft.Build.Evaluation.Project;
    using File = System.IO.File;
    using System.Diagnostics;
    using Path = System.IO.Path;
    using CultureInfo = System.Globalization.CultureInfo;
    using VSConstants = Microsoft.VisualStudio.VSConstants;
    using Marshal = System.Runtime.InteropServices.Marshal;

    public class JavaProjectNode : ProjectNode
    {
        private static readonly char[] charsToEscape = new char[] { '%', '*', '?', '@', '$', '(', ')', ';', '\'' };

        private readonly JavaBuildOptions _sharedBuildOptions;
        private Project _userBuildProject;
        private VSLangProj.VSProject _vsProject;

        public JavaProjectNode()
        {
            _sharedBuildOptions = new JavaBuildOptions();
            CanProjectDeleteItems = true;
            OleServiceProvider.AddService(typeof(VSLangProj.VSProject), HandleCreateService, false);
        }

        public JavaBuildOptions SharedBuildOptions
        {
            get
            {
                return _sharedBuildOptions;
            }
        }

        public override int ImageIndex
        {
            get
            {
                return HierarchyNode.NoImage;
            }
        }

        public override Guid ProjectGuid
        {
            get
            {
                return typeof(JavaProjectFactory).GUID;
            }
        }

        public override string ProjectType
        {
            get
            {
                return Constants.JavaLanguageName;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Project UserBuildProject
        {
            get
            {
                if (_userBuildProject == null && File.Exists(UserFileName))
                    CreateUserBuildProject();

                return _userBuildProject;
            }
        }

        public string UserFileName
        {
            get
            {
                return FileName + PerUserFileExtension;
            }
        }

        protected override bool SupportsProjectDesigner
        {
            get
            {
                return true;
            }
        }

        protected internal VSLangProj.VSProject VSProject
        {
            get
            {
                if (_vsProject == null)
                    _vsProject = new OAVSProject(this);

                return _vsProject;
            }
        }

        internal static void SetConfigurationProperty(ProjectConfig config, string propertyName, string propertyValue, Project buildProject)
        {
            if (!config.ProjectMgr.QueryEditProjectFile(false))
            {
                throw Marshal.GetExceptionForHR(VSConstants.OLE_E_PROMPTSAVECANCELLED);
            }

            string condition = String.Format(CultureInfo.InvariantCulture, ConfigProvider.configString, config.ConfigName);

            SetPropertyUnderCondition(propertyName, propertyValue, condition, buildProject);

            // property cache will need to be updated
            config.Invalidate();
        }

        internal static void SetPropertyUnderCondition(string propertyName, string propertyValue, string condition, Project buildProject)
        {
            string conditionTrimmed = (condition == null) ? String.Empty : condition.Trim();

            if (conditionTrimmed.Length == 0)
            {
                buildProject.SetProperty(propertyName, propertyValue);
                return;
            }

            // New OM doesn't have a convenient equivalent for setting a property with a particular property group condition. 
            // So do it ourselves.
            Microsoft.Build.Construction.ProjectPropertyGroupElement newGroup = null;

            foreach (Microsoft.Build.Construction.ProjectPropertyGroupElement group in buildProject.Xml.PropertyGroups)
            {
                if (String.Equals(group.Condition.Trim(), conditionTrimmed, StringComparison.OrdinalIgnoreCase))
                {
                    newGroup = group;
                    break;
                }
            }

            if (newGroup == null)
            {
                newGroup = buildProject.Xml.AddPropertyGroup(); // Adds after last existing PG, else at start of project
                newGroup.Condition = condition;
            }

            foreach (Microsoft.Build.Construction.ProjectPropertyElement property in newGroup.PropertiesReversed) // If there's dupes, pick the last one so we win
            {
                if (String.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase) && property.Condition.Length == 0)
                {
                    property.Value = propertyValue;
                    return;
                }
            }

            newGroup.AddProperty(propertyName, propertyValue);
        }

        private static string Escape(string unescapedString)
        {
            if (unescapedString == null)
                throw new ArgumentNullException("unescapedString", "Null strings not allowed.");

            if (!ContainsReservedCharacters(unescapedString))
                return unescapedString;

            StringBuilder builder = new StringBuilder(unescapedString);
            foreach (char ch in charsToEscape)
            {
                int num = Convert.ToInt32(ch);
                string newValue = string.Format(CultureInfo.InvariantCulture, "%{0:x00}", new object[] { num });
                builder.Replace(ch.ToString(CultureInfo.InvariantCulture), newValue);
            }
            return builder.ToString();
        }

        private static bool ContainsReservedCharacters(string unescapedString)
        {
            return unescapedString.IndexOfAny(charsToEscape) != -1;
        }

        public override bool IsCodeFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            string extension = Path.GetExtension(fileName);
            if (extension.Equals(Constants.JavaFileExtension, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        public void CreateUserBuildProject()
        {
            if (File.Exists(UserFileName))
            {
                _userBuildProject = new Project(UserFileName);
            }
            else
            {
                _userBuildProject = new Project();
                _userBuildProject.Save(UserFileName);
            }
        }

        public void SetProjectProperty(string propertyName, string propertyValue, string condition)
        {
            SetProjectProperty(propertyName, propertyValue, condition, false);
        }

        public void SetProjectProperty(string propertyName, string propertyValue, string condition, bool treatPropertyValueAsLiteral)
        {
            if (propertyValue == null)
                propertyValue = string.Empty;

            // see if the value is the same as what's already in the project so we
            // know whether to actually mark the project file dirty or not
            string oldValue = GetProjectProperty(propertyName, true);

            if (!String.Equals(oldValue, propertyValue, StringComparison.Ordinal))
            {
                // check out the project file
                if (ProjectMgr != null && !ProjectMgr.QueryEditProjectFile(false))
                {
                    throw Marshal.GetExceptionForHR(VSConstants.OLE_E_PROMPTSAVECANCELLED);
                }

                if (treatPropertyValueAsLiteral)
                {
                    propertyValue = Escape(propertyValue);
                }

                SetPropertyUnderCondition(propertyName, propertyValue, condition, BuildProject);

                // refresh the cached values
                SetCurrentConfiguration();
                SetProjectFileDirty(true);
            }
        }

        protected override Guid[] GetConfigurationIndependentPropertyPages()
        {
            return new Guid[]
                {
                    typeof(PropertyPages.JavaGeneralPropertyPage).GUID,
                    typeof(PropertyPages.JavaBuildEventsPropertyPage).GUID,
                };
        }

        protected override Guid[] GetConfigurationDependentPropertyPages()
        {
            return new Guid[]
                {
                    typeof(PropertyPages.JavaBuildPropertyPage).GUID,
                    typeof(PropertyPages.JavaDebugPropertyPage).GUID,
                };
        }

        internal object HandleCreateService(Type serviceType)
        {
            object service = null;

            if (serviceType == typeof(VSLangProj.VSProject))
            {
                service = this.VSProject;
            }
            else if (serviceType == typeof(EnvDTE.Project))
            {
                service = this.GetAutomationObject();
            }

            return service;
        }
    }
}
