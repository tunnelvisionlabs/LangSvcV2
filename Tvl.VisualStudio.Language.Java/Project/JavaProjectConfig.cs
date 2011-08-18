namespace Tvl.VisualStudio.Language.Java.Project
{
    using System;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Project;

    using CultureInfo = System.Globalization.CultureInfo;
    using StringComparison = System.StringComparison;

    public class JavaProjectConfig : ProjectConfig
    {
        private Microsoft.Build.Execution.ProjectInstance _currentUserConfig;

        internal JavaProjectConfig(JavaProjectNode project, string configuration)
            : base(project, configuration)
        {
        }

        public new JavaProjectNode ProjectManager
        {
            get
            {
                return (JavaProjectNode)base.ProjectManager;
            }
        }

        public virtual string GetConfigurationProperty(string propertyName, bool resetCache, ProjectPropertyStorage propertyStorage)
        {
            if (propertyStorage == ProjectPropertyStorage.ProjectFile)
            {
                return base.GetConfigurationProperty(propertyName, resetCache);
            }
            else
            {
                if (resetCache || _currentUserConfig == null)
                {
                    if (ProjectManager.UserBuildProject == null)
                        ProjectManager.CreateUserBuildProject();

                    // Get properties for current configuration from project file and cache it
                    ProjectManager.SetConfiguration(this.ConfigName);
                    ProjectManager.UserBuildProject.ReevaluateIfNecessary();
                    // Create a snapshot of the evaluated project in its current state
                    _currentUserConfig = ProjectManager.UserBuildProject.CreateProjectInstance();

                    // Restore configuration
                    ProjectManager.SetCurrentConfiguration();
                }

                Microsoft.Build.Execution.ProjectPropertyInstance property = _currentUserConfig.GetProperty(propertyName);
                if (property == null)
                    return null;

                return property.EvaluatedValue;
            }
        }

        public virtual void SetConfigurationProperty(string propertyName, string propertyValue, ProjectPropertyStorage propertyStorage)
        {
            if (propertyStorage == ProjectPropertyStorage.ProjectFile)
            {
                base.SetConfigurationProperty(propertyName, propertyValue);
            }
            else
            {
                string condition = string.Format(CultureInfo.InvariantCulture, ConfigProvider.configString, this.ConfigName);
                SetUserPropertyUnderCondition(propertyName, propertyValue, condition);
                Invalidate();
            }
        }

        public override void Invalidate()
        {
            base.Invalidate();
            _currentUserConfig = null;
        }

        public override int QueryDebugLaunch(uint flags, out int fCanLaunch)
        {
            fCanLaunch = 0;
            return VSConstants.S_OK;
        }

        public override int DebugLaunch(uint grfLaunch)
        {
            throw new NotSupportedException();
        }

        private void SetUserPropertyUnderCondition(string propertyName, string propertyValue, string condition)
        {
            string conditionTrimmed = (condition == null) ? string.Empty : condition.Trim();

            if (ProjectManager.UserBuildProject == null)
                ProjectManager.CreateUserBuildProject();

            if (conditionTrimmed.Length == 0)
            {
                ProjectManager.UserBuildProject.SetProperty(propertyName, propertyValue);
                return;
            }

            // New OM doesn't have a convenient equivalent for setting a property with a particular property group condition. 
            // So do it ourselves.
            Microsoft.Build.Construction.ProjectPropertyGroupElement newGroup = null;

            foreach (Microsoft.Build.Construction.ProjectPropertyGroupElement group in ProjectManager.UserBuildProject.Xml.PropertyGroups)
            {
                if (string.Equals(group.Condition.Trim(), conditionTrimmed, StringComparison.OrdinalIgnoreCase))
                {
                    newGroup = group;
                    break;
                }
            }

            if (newGroup == null)
            {
                newGroup = ProjectManager.UserBuildProject.Xml.AddPropertyGroup(); // Adds after last existing PG, else at start of project
                newGroup.Condition = condition;
            }

            foreach (Microsoft.Build.Construction.ProjectPropertyElement property in newGroup.PropertiesReversed) // If there's dupes, pick the last one so we win
            {
                if (string.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase) && property.Condition.Length == 0)
                {
                    property.Value = propertyValue;
                    return;
                }
            }

            newGroup.AddProperty(propertyName, propertyValue);
        }
    }
}
