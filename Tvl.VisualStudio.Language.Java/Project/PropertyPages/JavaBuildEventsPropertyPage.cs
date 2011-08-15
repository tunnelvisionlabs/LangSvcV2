namespace Tvl.VisualStudio.Language.Java.Project.PropertyPages
{
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    [Guid(JavaProjectConstants.JavaBuildEventsPropertyPageGuidString)]
    public class JavaBuildEventsPropertyPage : JavaPropertyPage
    {
        public JavaBuildEventsPropertyPage()
        {
            PageName = JavaConfigConstants.PageNameBuildEvents;
        }

        public new JavaBuildEventsPropertyPagePanel PropertyPagePanel
        {
            get
            {
                return (JavaBuildEventsPropertyPagePanel)base.PropertyPagePanel;
            }
        }

        protected override void BindProperties()
        {
            PropertyPagePanel.PreBuildEvent = GetConfigProperty(JavaConfigConstants.PreBuildEvent);
            PropertyPagePanel.PostBuildEvent = GetConfigProperty(JavaConfigConstants.PostBuildEvent);
            PropertyPagePanel.RunPostBuildEvent = GetConfigProperty(JavaConfigConstants.RunPostBuildEvent);
        }
        protected override bool ApplyChanges()
        {
            SetConfigProperty(JavaConfigConstants.PreBuildEvent, PropertyPagePanel.PreBuildEvent);
            SetConfigProperty(JavaConfigConstants.PostBuildEvent, PropertyPagePanel.PostBuildEvent);
            SetConfigProperty(JavaConfigConstants.RunPostBuildEvent, PropertyPagePanel.RunPostBuildEvent);
            return true;
        }

        protected override JavaPropertyPagePanel CreatePropertyPagePanel()
        {
            return new JavaBuildEventsPropertyPagePanel(this);
        }
    }
}
