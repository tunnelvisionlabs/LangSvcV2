namespace JavaLanguageService
{
    using System.Collections.Generic;
    using Antlr.Runtime;
    using Microsoft.VisualStudio.Text;

    internal sealed class SnapshotCharStream : ICharStream
    {
        //private string _readaheadCache;
        //private int _readaheadCachePosition;

        /** <summary>tracks how deep mark() calls are nested</summary> */
        private int _markDepth = 0;

        /** <summary>
         *  A list of CharStreamState objects that tracks the stream state
         *  values line, charPositionInLine, and p that can change as you
         *  move through the input stream.  Indexed from 1..markDepth.
         *  A null is kept @ index 0.  Create upon first call to mark().
         *  </summary>
         */
        private List<CharStreamState> _markers;

        /** <summary>Track the last mark() call result value for use in rewind().</summary> */
        private int _lastMarker;

        public SnapshotCharStream(ITextSnapshot snapshot)
        {
            this.Snapshot = snapshot;
        }

        public ITextSnapshot Snapshot
        {
            get;
            private set;
        }

        public int Line
        {
            get;
            set;
        }

        public int CharPositionInLine
        {
            get;
            set;
        }

        public int Count
        {
            get
            {
                return Snapshot.Length;
            }
        }

        public int Index
        {
            get;
            private set;
        }

        public string SourceName
        {
            get
            {
                return "Snapshot";
            }
        }

        public int LT(int i)
        {
            return LA(i);
        }

        public string substring(int start, int stop)
        {
            return Snapshot.GetText(start, stop - start + 1);
        }

        public void Consume()
        {
            //System.out.println("prev p="+p+", c="+(char)data[p]);
            if (Index < Count)
            {
                CharPositionInLine++;
                if (Snapshot.GetText(Index, 1)[0] == '\n')
                {
                    /*
                    System.out.println("newline char found on line: "+line+
                                       "@ pos="+charPositionInLine);
                    */
                    Line++;
                    CharPositionInLine = 0;
                }
                Index++;
                //System.out.println("p moves to "+p+" (c='"+(char)data[p]+"')");
            }
        }

        public int LA(int i)
        {
            if (i == 0)
            {
                return 0; // undefined
            }
            if (i < 0)
            {
                i++; // e.g., translate LA(-1) to use offset i=0; then data[p+0-1]
                if ((Index + i - 1) < 0)
                {
                    return CharStreamConstants.EndOfFile; // invalid; no char before first char
                }
            }

            if ((Index + i - 1) >= Count)
            {
                //System.out.println("char LA("+i+")=EOF; p="+p);
                return CharStreamConstants.EndOfFile;
            }
            //System.out.println("char LA("+i+")="+(char)data[p+i-1]+"; p="+p);
            //System.out.println("LA("+i+"); p="+p+" n="+n+" data.length="+data.length);


            // first check if the data is in the line cache
            //if (_readaheadCache != null)
            //{
            //    int readaheadIndex = _readaheadCachePosition + i - 1;
            //    if (readaheadIndex >= 0 && readaheadIndex < _readaheadCache.Length)
            //        return _readaheadCache[readaheadIndex];
            //}

            int actualIndex = Index + i - 1;
            return Snapshot.GetText(actualIndex, 1)[0];
        }

        public int Mark()
        {
            if (_markers == null)
            {
                _markers = new List<CharStreamState>();
                _markers.Add(null); // depth 0 means no backtracking, leave blank
            }
            _markDepth++;
            CharStreamState state = null;
            if (_markDepth >= _markers.Count)
            {
                state = new CharStreamState();
                _markers.Add(state);
            }
            else
            {
                state = _markers[_markDepth];
            }
            state.p = Index;
            state.line = Line;
            state.charPositionInLine = CharPositionInLine;
            _lastMarker = _markDepth;
            return _markDepth;
        }

        public void Release(int marker)
        {
            // unwind any other markers made after m and release m
            _markDepth = marker;
            // release this marker
            _markDepth--;
        }

        public void Rewind()
        {
            Rewind(_lastMarker);
        }

        public void Rewind(int marker)
        {
            CharStreamState state = _markers[marker];
            // restore stream state
            Seek(state.p);
            Line = state.line;
            CharPositionInLine = state.charPositionInLine;
            Release(marker);
        }

        public void Seek(int index)
        {
            if (index == Index)
                return;

            Index = index;
            var line = Snapshot.GetLineFromPosition(Index);
            Line = line.LineNumber;
            CharPositionInLine = Index - line.Start.Position;
            //_readaheadCache = line.GetText();
            //_readaheadCachePosition = CharPositionInLine;
        }
    }
}
