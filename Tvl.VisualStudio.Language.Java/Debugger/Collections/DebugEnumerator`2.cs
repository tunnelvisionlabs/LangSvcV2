namespace Tvl.VisualStudio.Language.Java.Debugger.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics.Contracts;

    public abstract class DebugEnumerator<TEnum, TElement>
    {
        private readonly IEnumerable<TElement> _elements;

        public DebugEnumerator(IEnumerable<TElement> elements)
        {
            Contract.Requires<ArgumentNullException>(elements != null, "elements");

            _elements = elements;
        }

        public int Clone(out TEnum ppEnum)
        {
            throw new NotImplementedException();
        }

        public int GetCount(out uint pcelt)
        {
            throw new NotImplementedException();
        }

        public int Next(uint celt, TElement[] rgelt, ref uint pceltFetched)
        {
            throw new NotImplementedException();
        }

        public int Reset()
        {
            throw new NotImplementedException();
        }

        public int Skip(uint celt)
        {
            throw new NotImplementedException();
        }
    }
}
