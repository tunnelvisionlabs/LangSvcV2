namespace Tvl.VisualStudio.Language.Alloy.IntellisenseModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal abstract class FunctionBase : Element
    {
        private readonly string _name;
        private readonly AlloyFile _file;

        protected FunctionBase(string name, AlloyFile file)
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

        public abstract FunctionAttributes Attributes
        {
            get;
        }

        public override bool IsExternallyVisible
        {
            get
            {
                return !IsPrivate;
            }
        }

        public bool IsPrivate
        {
            get
            {
                return (Attributes & FunctionAttributes.Private) != 0;
            }
        }

        public bool IsFunction
        {
            get
            {
                return (Attributes & FunctionAttributes.FunctionTypeMask) == FunctionAttributes.Function;
            }
        }

        public bool IsPredicate
        {
            get
            {
                return (Attributes & FunctionAttributes.FunctionTypeMask) == FunctionAttributes.Predicate;
            }
        }
    }
}
