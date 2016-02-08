namespace Tvl.VisualStudio.Language.Alloy
{
    using System;
    using Microsoft.VisualStudio.Text;
    using Path = System.IO.Path;

    partial class AlloyIntellisenseCache
    {
        public struct AlloyPositionReference : IEquatable<AlloyPositionReference>
        {
            private readonly ITextSnapshot _textSnapshot;
            private readonly string _path;
            private readonly int _position;

            private AlloyPositionReference(SnapshotPoint point)
            {
                _textSnapshot = point.Snapshot;
                _path = null;
                _position = point.Position;
            }

            private AlloyPositionReference(string path, int position)
            {
                _textSnapshot = null;
                _path = Path.GetFullPath(path).ToLowerInvariant();
                _position = position;
            }

            public static AlloyPositionReference FromSnapshotPoint(SnapshotPoint point)
            {
                if (point.Snapshot == null)
                    throw new ArgumentException("Invalid snapshot.");

                return new AlloyPositionReference(point);
            }

            public static AlloyPositionReference FromPath(string path, int position)
            {
                if (path == null)
                    throw new ArgumentNullException("path");

                return new AlloyPositionReference(path, position);
            }

            public bool Equals(AlloyPositionReference other)
            {
                throw new NotImplementedException();
                //if (!object.ReferenceEquals(_textBuffer, other._textBuffer))
                //    return false;
                //if (object.ReferenceEquals(_path, other._path))
                //    return true;
                //if (_path == null || other._path == null)
                //    return false;

                //// this comparison works because the object is constructed with Path.GetFullPath(...).ToLowerInvariant()
                //return string.Equals(_path, other._path, StringComparison.Ordinal);
            }

            public override bool Equals(object obj)
            {
                if (!(obj is AlloyPositionReference))
                    return false;

                return Equals((AlloyPositionReference)obj);
            }

            public override int GetHashCode()
            {
                throw new NotImplementedException();
                //if (_textBuffer != null)
                //    return _textBuffer.GetHashCode();
                //if (_path != null)
                //    return _path.GetHashCode();

                //return 0;
            }
        }
    }
}
