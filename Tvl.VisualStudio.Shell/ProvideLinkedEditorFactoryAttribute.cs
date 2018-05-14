namespace Tvl.VisualStudio.Shell
{
    using System;
    using System.Diagnostics;
    using JetBrains.Annotations;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class ProvideLinkedEditorFactoryAttribute : EditorFactoryRegistrationAttribute
    {
        private readonly Type _linkedFactoryType;

        public ProvideLinkedEditorFactoryAttribute([NotNull] Type factoryType, [NotNull] Type linkedFactoryType, short nameResourceID)
            : base(factoryType, nameResourceID)
        {
            Debug.Assert(factoryType != null);
            Requires.NotNull(linkedFactoryType, nameof(linkedFactoryType));

            _linkedFactoryType = linkedFactoryType;
        }

        public Type LinkedFactoryType
        {
            get
            {
                return _linkedFactoryType;
            }
        }

        public override void Register(RegistrationContext context)
        {
            using (Key key = context.CreateKey(EditorRegKey))
            {
                key.SetValue("LinkedEditorGuid", LinkedFactoryType.GUID.ToString("B"));
            }
        }
    }
}
