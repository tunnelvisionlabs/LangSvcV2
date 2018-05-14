namespace Tvl
{
    using System;
    using System.Collections.Generic;
    using JetBrains.Annotations;
    using ICollection = System.Collections.ICollection;
    using IEnumerable = System.Collections.IEnumerable;
    using IEnumerator = System.Collections.IEnumerator;
    using IList = System.Collections.IList;

    partial class ArrayExtensions
    {
        private sealed class ArraySegment<T> : IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable
        {
            [NotNull]
            private readonly T[] _array;
            private readonly int _offset;
            private readonly int _count;

            public ArraySegment([NotNull] T[] array, int offset, int count)
            {
                Requires.NotNull(array, nameof(array));
                Requires.Range(offset >= 0, nameof(offset));
                Requires.Range(count >= 0, nameof(count));
                Requires.Argument(offset <= array.Length, nameof(offset), $"{nameof(offset)} must be less than or equal to the array length");
                Requires.Argument(checked(offset + count) <= array.Length, nameof(count), $"({nameof(offset)} + {nameof(count)}) must be less than or equal to the array length");

                _array = array;
                _offset = offset;
                _count = count;
            }

            public int Count
            {
                get
                {
                    return _count;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return false;
                }
            }

            public bool IsFixedSize
            {
                get
                {
                    return true;
                }
            }

            bool ICollection.IsSynchronized
            {
                get
                {
                    return false;
                }
            }

            object ICollection.SyncRoot
            {
                get
                {
                    return this;
                }
            }

            public T this[int index]
            {
                get
                {
                    Requires.Range(index >= 0, nameof(index));
                    Requires.Argument(index < Count, nameof(index), $"{nameof(index)} must be less than {nameof(Count)}");

                    return _array[_offset + index];
                }

                set
                {
                    Requires.Range(index >= 0, nameof(index));
                    Requires.Argument(index < Count, nameof(index), $"{nameof(index)} must be less than {nameof(Count)}");

                    _array[_offset + index] = value;
                }
            }

            T IList<T>.this[int index]
            {
                get
                {
                    return this[index];
                }

                set
                {
                    this[index] = value;
                }
            }

            object IList.this[int index]
            {
                get
                {
                    return this[index];
                }

                set
                {
                    this[index] = (T)value;
                }
            }

            public int IndexOf(T item)
            {
                return Array.IndexOf(_array, item, _offset, _count) - _offset;
            }

            void IList<T>.Insert(int index, T item)
            {
                throw new NotSupportedException();
            }

            void IList<T>.RemoveAt(int index)
            {
                throw new NotSupportedException();
            }

            public bool Contains(T item)
            {
                return IndexOf(item) >= 0;
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                Array.Copy(_array, _offset, array, arrayIndex, _count);
            }

            [NotNull]
            public IEnumerator<T> GetEnumerator()
            {
                for (int i = 0; i < _count; i++)
                    yield return _array[_offset + i];
            }

            [NotNull]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            bool IList.Contains(object value)
            {
                return Array.IndexOf(_array, value, _offset, _count) >= 0;
            }

            int IList.IndexOf(object value)
            {
                return Array.IndexOf(_array, value, _offset, _count) - _offset;
            }

            void ICollection.CopyTo(Array array, int index)
            {
                Array.Copy(_array, _offset, array, index, _count);
            }

            void ICollection<T>.Add(T item)
            {
                throw new NotSupportedException();
            }

            void ICollection<T>.Clear()
            {
                throw new NotSupportedException();
            }

            bool ICollection<T>.Remove(T item)
            {
                throw new NotSupportedException();
            }

            int IList.Add(object value)
            {
                throw new NotSupportedException();
            }

            void IList.Clear()
            {
                throw new NotSupportedException();
            }

            void IList.Insert(int index, object value)
            {
                throw new NotSupportedException();
            }

            void IList.Remove(object value)
            {
                throw new NotSupportedException();
            }

            void IList.RemoveAt(int index)
            {
                throw new NotSupportedException();
            }
        }
    }
}
