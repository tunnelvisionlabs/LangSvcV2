namespace Tvl.VisualStudio.Text
{
    public sealed class CommentFormat
    {
        public CommentFormat(string lineStart)
            : this(lineStart, null, null)
        {
        }

        public CommentFormat(string blockStart, string blockEnd)
            : this(null, blockStart, blockEnd)
        {
        }

        public CommentFormat(string lineStart, string blockStart, string blockEnd)
        {
            this.UseLineComments = !string.IsNullOrEmpty(lineStart);
            this.LineStart = lineStart;
            this.BlockStart = blockStart;
            this.BlockEnd = blockEnd;
        }

        public bool UseLineComments
        {
            get;
            private set;
        }

        public string LineStart
        {
            get;
            private set;
        }

        public string BlockStart
        {
            get;
            private set;
        }

        public string BlockEnd
        {
            get;
            private set;
        }
    }
}
