namespace Tvl.VisualStudio.Language.Java.Project
{
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

        public new JavaProjectNode ProjectMgr
        {
            get
            {
                return (JavaProjectNode)base.ProjectMgr;
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
                    if (ProjectMgr.UserBuildProject == null)
                        ProjectMgr.CreateUserBuildProject();

                    // Get properties for current configuration from project file and cache it
                    ProjectMgr.SetConfiguration(this.ConfigName);
                    ProjectMgr.UserBuildProject.ReevaluateIfNecessary();
                    // Create a snapshot of the evaluated project in its current state
                    _currentUserConfig = ProjectMgr.UserBuildProject.CreateProjectInstance();

                    // Restore configuration
                    ProjectMgr.SetCurrentConfiguration();
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

        private void SetUserPropertyUnderCondition(string propertyName, string propertyValue, string condition)
        {
            string conditionTrimmed = (condition == null) ? string.Empty : condition.Trim();

            if (ProjectMgr.UserBuildProject == null)
                ProjectMgr.CreateUserBuildProject();

            if (conditionTrimmed.Length == 0)
            {
                ProjectMgr.UserBuildProject.SetProperty(propertyName, propertyValue);
                return;
            }

            // New OM doesn't have a convenient equivalent for setting a property with a particular property group condition. 
            // So do it ourselves.
            Microsoft.Build.Construction.ProjectPropertyGroupElement newGroup = null;

            foreach (Microsoft.Build.Construction.ProjectPropertyGroupElement group in ProjectMgr.UserBuildProject.Xml.PropertyGroups)
            {
                if (string.Equals(group.Condition.Trim(), conditionTrimmed, StringComparison.OrdinalIgnoreCase))
                {
                    newGroup = group;
                    break;
                }
            }

            if (newGroup == null)
            {
                newGroup = ProjectMgr.UserBuildProject.Xml.AddPropertyGroup(); // Adds after last existing PG, else at start of project
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
