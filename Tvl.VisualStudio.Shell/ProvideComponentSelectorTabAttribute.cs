namespace Tvl.VisualStudio.Shell
{
    using System;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio.Shell;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class ProvideComponentSelectorTabAttribute : RegistrationAttribute
    {
        private readonly Guid _componentSelectorTabGuid;
        private readonly Guid _packageGuid;
        private readonly string _name;

        private int _sortOrder = 0x35;

        public ProvideComponentSelectorTabAttribute([NotNull] Type componentSelectorTabType, [NotNull] Type packageType, [NotNull] string name)
        {
            Requires.NotNull(componentSelectorTabType, nameof(componentSelectorTabType));
            Requires.NotNull(packageType, nameof(packageType));
            Requires.NotNullOrEmpty(name, nameof(name));

            _componentSelectorTabGuid = componentSelectorTabType.GUID;
            _packageGuid = packageType.GUID;
            _name = name;
        }

        public ProvideComponentSelectorTabAttribute(Guid componentSelectorTabGuid, Guid packageGuid, [NotNull] string name)
        {
            Requires.NotNullOrEmpty(name, nameof(name));
            Requires.NotEmpty(componentSelectorTabGuid, nameof(componentSelectorTabGuid));
            Requires.NotEmpty(packageGuid, nameof(packageGuid));

            _componentSelectorTabGuid = componentSelectorTabGuid;
            _packageGuid = packageGuid;
            _name = name;
        }

        public Guid ComponentSelectorTabGuid
        {
            get
            {
                return _componentSelectorTabGuid;
            }
        }

        public Guid PackageGuid
        {
            get
            {
                return _packageGuid;
            }
        }

        [NotNull]
        public string Name
        {
            get
            {
                return _name;
            }
        }

        public int SortOrder
        {
            get
            {
                return _sortOrder;
            }

            set
            {
                _sortOrder = value;
            }
        }

        private string BaseRegistryKey
        {
            get
            {
                return string.Format(@"ComponentPickerPages\{0}", _name);
            }
        }

        public override void Register(RegistrationContext context)
        {
            using (var key = context.CreateKey(BaseRegistryKey))
            {
                key.SetValue(string.Empty, string.Empty);
                key.SetValue("Package", _packageGuid.ToString("B"));
                key.SetValue("Page", _componentSelectorTabGuid.ToString("B"));
                key.SetValue("Sort", _sortOrder);
            }
        }

        public override void Unregister(RegistrationContext context)
        {
            context.RemoveKey(BaseRegistryKey);
        }
    }
}
