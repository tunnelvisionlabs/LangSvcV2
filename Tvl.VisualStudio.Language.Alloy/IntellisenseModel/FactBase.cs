namespace Tvl.VisualStudio.Language.Alloy.IntellisenseModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal abstract class FactBase : Element
    {
        private readonly string _name;
        private readonly AlloyFile _file;

        protected FactBase(string name, AlloyFile file)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (file == null)
                throw new ArgumentNullException("file");

            _name = name;
            _file = file;
        }

        public override string Name
        {
            get
            {
                return _name;
            }
        }

        public override AlloyFile File
        {
            get
            {
                return _file;
            }
        }

        public virtual IEnumerable<Declaration> Parameters
        {
            get
            {
                return Enumerable.Empty<Declaration>();
            }
        }

        public virtual IElementReference<Declaration> ReturnType
        {
            get
            {
                return null;
            }
        }

        public abstract FactAttributes Attributes
        {
            get;
        }

        public bool IsAnonymous
        {
            get
            {
                return (Attributes & FactAttributes.Anonymous) != 0;
            }
        }

        public bool IsFact
        {
            get
            {
                return (Attributes & FactAttributes.FactTypeMask) == FactAttributes.Assertion;
            }
        }

        public bool IsAssertion
        {
            get
            {
                return (Attributes & FactAttributes.FactTypeMask) == FactAttributes.Assertion;
            }
        }

        public override bool IsExternallyVisible
        {
            get
            {
                return false;
            }
        }
    }
}
