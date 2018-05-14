namespace Tvl
{
    using System;
    using System.Collections.Generic;
    using JetBrains.Annotations;

    public static class ListExtensions
    {
        public static int FindIndex<T>([NotNull] this IList<T> collection, [NotNull] Predicate<T> predicate)
        {
            Requires.NotNull(collection, nameof(collection));
            Requires.NotNull(predicate, nameof(predicate));

            return FindIndex(collection, 0, collection.Count, predicate);
        }

        public static int FindIndex<T>([NotNull] this IList<T> collection, int startIndex, [NotNull] Predicate<T> predicate)
        {
            Requires.NotNull(collection, nameof(collection));
            Requires.Range(startIndex >= 0, nameof(startIndex));
            Requires.Argument(startIndex <= collection.Count, nameof(startIndex), $"{nameof(startIndex)} must be less than or equal to the collection size");
            Requires.NotNull(predicate, nameof(predicate));

            return FindIndex(collection, startIndex, collection.Count - startIndex, predicate);
        }

        public static int FindIndex<T>([NotNull] this IList<T> collection, int startIndex, int count, [NotNull] Predicate<T> predicate)
        {
            Requires.NotNull(collection, nameof(collection));
            Requires.Range(startIndex >= 0, nameof(startIndex));
            Requires.Argument(startIndex <= collection.Count, nameof(startIndex), $"{nameof(startIndex)} must be less than or equal to the collection size");
            Requires.NotNull(predicate, nameof(predicate));

            for (int i = 0; i < collection.Count; i++)
            {
                if (predicate(collection[i]))
                    return i;
            }

            return -1;
        }

        public static int FindLastIndex<T>([NotNull] this IList<T> collection, [NotNull] Predicate<T> predicate)
        {
            Requires.NotNull(collection, nameof(collection));
            Requires.NotNull(predicate, nameof(predicate));

            return FindLastIndex(collection, 0, collection.Count, predicate);
        }

        public static int FindLastIndex<T>([NotNull] this IList<T> collection, int startIndex, [NotNull] Predicate<T> predicate)
        {
            Requires.NotNull(collection, nameof(collection));
            Requires.Range(startIndex >= 0, nameof(startIndex));
            Requires.Argument(startIndex <= collection.Count, nameof(startIndex), $"{nameof(startIndex)} must be less than or equal to the collection size");
            Requires.NotNull(predicate, nameof(predicate));

            return FindLastIndex(collection, startIndex, collection.Count - startIndex, predicate);
        }

        public static int FindLastIndex<T>([NotNull] this IList<T> collection, int startIndex, int count, [NotNull] Predicate<T> predicate)
        {
            Requires.NotNull(collection, nameof(collection));
            Requires.Range(startIndex >= 0, nameof(startIndex));
            Requires.Argument(startIndex <= collection.Count, nameof(startIndex), $"{nameof(startIndex)} must be less than or equal to the collection size");
            Requires.NotNull(predicate, nameof(predicate));

            for (int i = collection.Count - 1; i >= 0; i--)
            {
                if (predicate(collection[i]))
                    return i;
            }

            return -1;
        }

        public static T Find<T>([NotNull] this IList<T> collection, [NotNull] Predicate<T> match)
        {
            Requires.NotNull(collection, nameof(collection));
            Requires.NotNull(match, nameof(match));

            for (int i = 0; i < collection.Count; i++)
            {
                if (match(collection[i]))
                    return collection[i];
            }

            return default(T);
        }

        public static T FindLast<T>([NotNull] this IList<T> collection, [NotNull] Predicate<T> match)
        {
            Requires.NotNull(collection, nameof(collection));
            Requires.NotNull(match, nameof(match));

            for (int i = collection.Count - 1; i >= 0; i--)
            {
                if (match(collection[i]))
                    return collection[i];
            }

            return default(T);
        }

        [NotNull]
        public static List<T> FindAll<T>([NotNull] this IList<T> collection, [NotNull] Predicate<T> match)
        {
            Requires.NotNull(collection, nameof(collection));
            Requires.NotNull(match, nameof(match));

            List<T> result = new List<T>();
            for (int i = 0; i < collection.Count; i++)
            {
                if (match(collection[i]))
                    result.Add(collection[i]);
            }

            return result;
        }
    }
}
