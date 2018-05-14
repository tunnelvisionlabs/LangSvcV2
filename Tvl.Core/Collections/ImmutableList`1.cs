namespace Tvl.Collections
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using JetBrains.Annotations;

    public class ImmutableList<T> : ReadOnlyCollection<T>
    {
        public ImmutableList([NotNull] IEnumerable<T> collection)
            : base(GetImmutableList(collection))
        {
            Requires.NotNull(collection, nameof(collection));
        }

        public ImmutableList([NotNull] ImmutableList<T> collection)
            : base(collection.Items)
        {
            Requires.NotNull(collection, nameof(collection));
        }

        private static IList<T> GetImmutableList([NotNull] IEnumerable<T> collection)
        {
            Requires.NotNull(collection, nameof(collection));

            ImmutableList<T> immutable = collection as ImmutableList<T>;
            if (immutable != null)
                return immutable;

            return collection.ToArray();
        }
    }
}
