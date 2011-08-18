namespace Tvl.VisualStudio.Language.Java.Project
{
    using Microsoft.VisualStudio.Project;

    public class JavaConfigProvider : ConfigProvider
    {
        public JavaConfigProvider(JavaProjectNode manager)
            : base(manager)
        {
        }

        protected new JavaProjectNode ProjectManager
        {
            get
            {
                return (JavaProjectNode)base.ProjectManager;
            }
        }

        protected override ProjectConfig CreateProjectConfiguration(string configName)
        {
            return new JavaProjectConfig(this.ProjectManager, configName);
        }
    }
}
