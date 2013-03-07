namespace Tvl.VisualStudio.Shell
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using __VSEDITORTRUSTLEVEL = Microsoft.VisualStudio.Shell.Interop.__VSEDITORTRUSTLEVEL;
    using CultureInfo = System.Globalization.CultureInfo;
    using LogicalView = Microsoft.VisualStudio.Shell.LogicalView;
    using ProvideViewAttribute = Microsoft.VisualStudio.Shell.ProvideViewAttribute;
    using RegistrationAttribute = Microsoft.VisualStudio.Shell.RegistrationAttribute;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class ProvideLinkedEditorFactoryAttribute : RegistrationAttribute
    {
        private readonly Type _factoryType;
        private readonly Type _linkedFactoryType;
        private readonly short _nameResourceID;
        private __VSEDITORTRUSTLEVEL _trustLevel;

        public ProvideLinkedEditorFactoryAttribute(Type factoryType, Type linkedFactoryType, short nameResourceID)
        {
            Contract.Requires<ArgumentNullException>(factoryType != null, "factoryType");
            Contract.Requires<ArgumentNullException>(linkedFactoryType != null, "linkedFactoryType");

            _factoryType = factoryType;
            _linkedFactoryType = linkedFactoryType;
            _nameResourceID = nameResourceID;
            _trustLevel = __VSEDITORTRUSTLEVEL.ETL_NeverTrusted;
        }

        private string EditorRegKey
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, @"Editors\{0:B}", FactoryType.GUID);
            }
        }

        public Type FactoryType
        {
            get
            {
                return _factoryType;
            }
        }

        public Type LinkedFactoryType
        {
            get
            {
                return _linkedFactoryType;
            }
        }

        public short NameResourceID
        {
            get
            {
                return _nameResourceID;
            }
        }

        public __VSEDITORTRUSTLEVEL TrustLevel
        {
            get
            {
                return _trustLevel;
            }

            set
            {
                _trustLevel = value;
            }
        }

        public override void Register(RegistrationContext context)
        {
            using (Key key = context.CreateKey(EditorRegKey))
            {
                key.SetValue(string.Empty, FactoryType.Name);
                key.SetValue("DisplayName", string.Format(CultureInfo.InvariantCulture, "#{0}", NameResourceID));
                key.SetValue("LinkedEditorGuid", LinkedFactoryType.GUID.ToString("B"));
                key.SetValue("Package", context.ComponentType.GUID.ToString("B"));
                key.SetValue("EditorTrustLevel", (int)_trustLevel);
                using (Key key2 = key.CreateSubkey("LogicalViews"))
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(typeof(LogicalView));
                    object[] customAttributes = FactoryType.GetCustomAttributes(typeof(ProvideViewAttribute), true);
                    foreach (ProvideViewAttribute attribute in customAttributes)
                    {
                        if (attribute.LogicalView != LogicalView.Primary)
                        {
                            Guid guid = (Guid)converter.ConvertTo(attribute.LogicalView, typeof(Guid));
                            string physicalView = attribute.PhysicalView ?? string.Empty;
                            key2.SetValue(guid.ToString("B"), physicalView);
                        }
                    }
                }
            }
        }

        public override void Unregister(RegistrationContext context)
        {
            context.RemoveKey(EditorRegKey);
        }
    }
}
