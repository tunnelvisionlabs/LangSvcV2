namespace Tvl.VisualStudio.Shell
{
    using System;
    using JetBrains.Annotations;

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class DebugMetricAttribute : Attribute
    {
        public DebugMetricAttribute(string name)
            : this(null, name)
        {
        }

        public DebugMetricAttribute(string subkey, [NotNull] string name)
        {
            Requires.NotNullOrEmpty(name, nameof(name));

            if (!string.IsNullOrEmpty(subkey))
                SubKey = subkey;

            Name = name;
        }

        public string SubKey
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }
    }
}
