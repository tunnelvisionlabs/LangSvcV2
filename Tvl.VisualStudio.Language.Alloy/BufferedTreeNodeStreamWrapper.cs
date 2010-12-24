namespace Tvl.VisualStudio.Language.Alloy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Antlr.Runtime.Tree;
    using Antlr.Runtime;

    [Obsolete("Hack to work around a bug with ITokenStreamInformation.")]
    internal class BufferedTreeNodeStreamWrapper : ITreeNodeStream
    {
        public readonly ITreeNodeStream _stream;

        public BufferedTreeNodeStreamWrapper(ITreeNodeStream stream)
        {
            _stream = stream;
        }

        public object LT(int k)
        {
            return _stream.LT(k);
        }

        public void ReplaceChildren(object parent, int startChildIndex, int stopChildIndex, object t)
        {
            _stream.ReplaceChildren(parent, startChildIndex, stopChildIndex, t);
        }

        public string ToString(object start, object stop)
        {
            return _stream.ToString(start, stop);
        }

        public ITokenStream TokenStream
        {
            get
            {
                return _stream.TokenStream;
            }
        }

        public ITreeAdaptor TreeAdaptor
        {
            get
            {
                return _stream.TreeAdaptor;
            }
        }

        public object TreeSource
        {
            get
            {
                return _stream.TreeSource;
            }
        }

        public bool UniqueNavigationNodes
        {
            get
            {
                return _stream.UniqueNavigationNodes;
            }

            set
            {
                _stream.UniqueNavigationNodes = value;
            }
        }

        public object this[int i]
        {
            get
            {
                return _stream[i];
            }
        }

        public void Consume()
        {
            _stream.Consume();
        }

        public int Count
        {
            get
            {
                return _stream.Count;
            }
        }

        public int Index
        {
            get
            {
                return _stream.Index;
            }
        }

        public int LA(int i)
        {
            return _stream.LA(i);
        }

        public int Mark()
        {
            return _stream.Mark();
        }

        public void Release(int marker)
        {
            _stream.Release(marker);
        }

        public void Rewind()
        {
            _stream.Rewind();
        }

        public void Rewind(int marker)
        {
            _stream.Rewind(marker);
        }

        public void Seek(int index)
        {
            _stream.Seek(index);
        }

        public string SourceName
        {
            get
            {
                return _stream.SourceName;
            }
        }
    }
}
