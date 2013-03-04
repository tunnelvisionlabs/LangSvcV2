namespace Tvl.VisualStudio.Language.Php
{
    using System.Collections.Generic;
    using Debug = System.Diagnostics.Debug;
    using TextReader = System.IO.TextReader;

    internal class TemplateTokenizer
    {
        private readonly TextReader _reader;
        private TemplateToken? _readToken;
        private int _position;

        public TemplateTokenizer(TextReader reader)
        {
            _reader = reader;
        }

        public IEnumerable<TemplateToken> GetTokens()
        {
            for (; ; )
            {
                var next = GetNextToken();
                if (next != null)
                {
                    yield return next.Value;
                }
                else
                {
                    break;
                }
            }
        }

        public TemplateToken? GetNextToken()
        {
            int start = _position;
            TemplateToken? res;
            int curChar;
            if (_readToken != null)
            {
                res = _readToken;
                _readToken = null;
                return res;
            }
            else if ((curChar = ReadChar()) == '<')
            {
                res = TryReadTemplateTag(_position - 1);
                if (res != null)
                {
                    return res;
                }
            }
            else if (curChar == -1)
            {
                return null;
            }

            // not a tag, read until we get to a tag, or we hit EOF.
            for (; ; )
            {
                switch (ReadChar())
                {
                case '<':
                    // we have to parse the whole tag to make sure it has a valid end, otherwise it's text.
                    Debug.Assert(_position >= 2, "We've read 2 chars, once to check for the initial {, and now a 2nd to check for the 2nd {");
                    int end = _position - 2;
                    res = TryReadTemplateTag(_position - 1);
                    if (res != null)
                    {
                        _readToken = res;
                        return new TemplateToken(TemplateTokenKind.Text, start, end);
                    }
                    break;
                case -1:
                    // EOF
                    return new TemplateToken(TemplateTokenKind.Text, start, _position - 1);
                }
            }

        }

        private TemplateToken? TryReadTemplateTag(int start)
        {
            if (ReadChar() != '?')
                return null;

            switch (ReadChar())
            {
            case 'p':
                if (ReadChar() != 'h')
                    return null;

                if (ReadChar() != 'p')
                    return null;

                break;

            case '=':
                break;

            default:
                return null;
            }

            return ReadToClose(start, TemplateTokenKind.Block, '?');
        }

        public TemplateToken? ReadToClose(int start, TemplateTokenKind kind, char closeType)
        {
            var prevChar = ReadChar();
            for (; ; )
            {
                if (prevChar == -1)
                {
                    return new TemplateToken(kind, start, _position - 1);
                }
                else if (prevChar == closeType)
                {
                    if ((prevChar = ReadChar()) == '>')
                    {
                        // we're done
                        return new TemplateToken(kind, start, _position - 1);
                    }
                }
                else
                {
                    prevChar = ReadChar();
                }
            }
        }

        private int ReadChar()
        {
            int res = _reader.Read();
            if (res != -1)
            {
                _position++;
            }
            return res;
        }

        private int PeekChar()
        {
            return _reader.Peek();
        }
    }
}
