namespace Tvl
{
    using System;
    using System.Collections.Generic;
    using JetBrains.Annotations;

    public static partial class ArrayExtensions
    {
        [NotNull]
        public static IList<T> Subarray<T>([NotNull] this T[] array, int startIndex)
        {
            Requires.NotNull(array, nameof(array));
            Requires.Range(startIndex >= 0, nameof(startIndex));
            Requires.Argument(startIndex <= array.Length, nameof(startIndex), $"{nameof(startIndex)} must be less than or equal to the array length");

            if (startIndex == 0)
                return array;

            return new ArraySegment<T>(array, startIndex, array.Length - startIndex);
        }

        [NotNull]
        public static IList<T> Subarray<T>([NotNull] this T[] array, int startIndex, int count)
        {
            Requires.NotNull(array, nameof(array));
            Requires.Range(startIndex >= 0, nameof(startIndex));
            Requires.Range(count >= 0, nameof(count));
            Requires.Argument(startIndex <= array.Length, nameof(startIndex), $"{nameof(startIndex)} must be less than or equal to the array length");
            Requires.Argument(checked(startIndex + count) <= array.Length, nameof(count), $"({nameof(startIndex)} + {nameof(count)}) must be less than or equal to the array length");

            if (startIndex == 0 && count == array.Length)
                return array;

            return new ArraySegment<T>(array, startIndex, count);
        }

        [NotNull]
        public static T[] CloneArray<T>([NotNull] this T[] array)
        {
            Requires.NotNull(array, nameof(array));

            return (T[])array.Clone();
        }
    }
}
