namespace Microsoft.VisualStudio.Project
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ComponentModel;
    using VSLangProj;
    using System.Diagnostics.Contracts;
    using System.Globalization;

    public class AvailableFileBuildActionConverter : EnumConverter
    {
        private readonly ProjectNode _projectManager;

        public AvailableFileBuildActionConverter(ProjectNode projectNode)
            : base(typeof(prjBuildAction))
        {
            Contract.Requires<ArgumentNullException>(projectNode != null, "projectNode");

            _projectManager = projectNode;
        }

        private ProjectNode ProjectManager
        {
            get
            {
                return _projectManager;
            }
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string stringValue = value as string;
            if (!string.IsNullOrEmpty(stringValue))
            {
                KeyValuePair<string, prjBuildAction> pair = ProjectManager.AvailableFileBuildActions.FirstOrDefault(i => string.Equals(i.Key, stringValue, StringComparison.OrdinalIgnoreCase));
                if (pair.Key != null)
                    return pair.Value;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                prjBuildAction buildAction = (prjBuildAction)value;
                KeyValuePair<string, prjBuildAction> pair = ProjectManager.AvailableFileBuildActions.FirstOrDefault(i => buildAction == i.Value);
                if (pair.Key != null)
                    return pair.Key;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(ProjectManager.AvailableFileBuildActions.Select(i => i.Value).ToArray());
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}
