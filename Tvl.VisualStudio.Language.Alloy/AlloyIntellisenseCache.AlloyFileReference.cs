namespace Tvl.VisualStudio.Language.Alloy
{
    using System;
    using Microsoft.VisualStudio.Text;
    using Path = System.IO.Path;

    partial class AlloyIntellisenseCache
    {
        public struct AlloyFileReference : IEquatable<AlloyFileReference>
        {
            private readonly ITextBuffer _textBuffer;
            private readonly string _path;

            private AlloyFileReference(ITextBuffer buffer)
            {
                _textBuffer = buffer;
                _path = null;
            }

            private AlloyFileReference(string path)
            {
                _textBuffer = null;
                _path = Path.GetFullPath(path).ToLowerInvariant();
            }

            public static AlloyFileReference FromTextBuffer(ITextBuffer buffer)
            {
                if (buffer == null)
                    throw new ArgumentNullException("buffer");

                return new AlloyFileReference(buffer);
            }

            public static AlloyFileReference FromPath(string path)
            {
                if (path == null)
                    throw new ArgumentNullException("path");

                return new AlloyFileReference(path);
            }

            public bool Equals(AlloyFileReference other)
            {
                if (!object.ReferenceEquals(_textBuffer, other._textBuffer))
                    return false;
                if (object.ReferenceEquals(_path, other._path))
                    return true;
                if (_path == null || other._path == null)
                    return false;

                // this comparison works because the object is constructed with Path.GetFullPath(...).ToLowerInvariant()
                return string.Equals(_path, other._path, StringComparison.Ordinal);
            }

            public override bool Equals(object obj)
            {
                if (!(obj is AlloyFileReference))
                    return false;

                return Equals((AlloyFileReference)obj);
            }

            public override int GetHashCode()
            {
                if (_textBuffer != null)
                    return _textBuffer.GetHashCode();
                if (_path != null)
                    return _path.GetHashCode();

                return 0;
            }
        }
    }
}
