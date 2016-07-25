namespace Tvl.VisualStudio.Language.StringTemplate4
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Atn;
    using Antlr4.Runtime.Misc;
    using Tvl.VisualStudio.Language.Parsing4;

    public class ClassifierLexer : AbstractGroupClassifierLexer, ITokenSourceWithState<ClassifierLexerState>
    {
        public const char DefaultOpenDelimiter = '<';
        public const char DefaultCloseDelimiter = '>';

        private const int OpenDelimiterPlaceholder = '\uFFF0';
        private const int CloseDelimiterPlaceholder = '\uFFF1';
        private static readonly ConcurrentDictionary<int, ATN> DelimiterToAtn = new ConcurrentDictionary<int, ATN>();

        private readonly Dictionary<ATN, GroupHighlighterATNSimulator> _atnToSimulator = new Dictionary<ATN, GroupHighlighterATNSimulator>();

        public ClassifierLexer(ICharStream input)
            : this(input, DefaultOpenDelimiter, DefaultCloseDelimiter)
        {
        }

        public ClassifierLexer(ICharStream input, char openDelimiter, char closeDelimiter)
            : base(input)
        {
            _interp = GetSimulatorForDelimiters(openDelimiter, closeDelimiter);
        }

        public override IToken Emit()
        {
            switch (_type)
            {
            case LBRACE:
                if (HandleAcceptPositionForKeyword("["))
                {
                    PushMode(AnonymousTemplateParameters);
                }

                break;

            case DELIMITERS:
                if (HandleAcceptPositionForKeyword("delimiters"))
                {
                    PushMode(DelimitersOpenSpec);
                }

                break;

            case FIRST:
                HandleAcceptPositionForKeyword("first");
                break;

            case LAST:
                HandleAcceptPositionForKeyword("last");
                break;

            case REST:
                HandleAcceptPositionForKeyword("rest");
                break;

            case TRUNC:
                HandleAcceptPositionForKeyword("trunc");
                break;

            case STRIP:
                HandleAcceptPositionForKeyword("strip");
                break;

            case TRIM:
                HandleAcceptPositionForKeyword("trim");
                break;

            case LENGTH:
                HandleAcceptPositionForKeyword("length");
                break;

            case STRLEN:
                HandleAcceptPositionForKeyword("strlen");
                break;

            case REVERSE:
                HandleAcceptPositionForKeyword("reverse");
                break;

            case DelimitersOpenSpec_DELIMITER_STRING:
                SetDelimiters(Text[1], CloseDelimiter);
                _type = STRING;
                break;

            case DelimitersCloseSpec_DELIMITER_STRING:
                SetDelimiters(OpenDelimiter, Text[1]);
                _type = STRING;
                break;

            default:
                break;
            }

            return base.Emit();
        }

        private bool HandleAcceptPositionForKeyword(string keyword)
        {
            if (InputStream.Index > _tokenStartCharIndex + keyword.Length)
            {
                int offset = keyword.Length - 1;
                Interpreter.ResetAcceptPosition(_input, _tokenStartCharIndex + offset, _tokenStartLine, _tokenStartCharPositionInLine + offset);
                return true;
            }

            return false;
        }

        protected new GroupHighlighterATNSimulator Interpreter
        {
            get
            {
                return (GroupHighlighterATNSimulator)base.Interpreter;
            }
        }

        public char OpenDelimiter
        {
            get
            {
                return Interpreter.OpenDelimiter;
            }
        }

        public char CloseDelimiter
        {
            get
            {
                return Interpreter.CloseDelimiter;
            }
        }

        public void SetDelimiters(char openDelimiter, char closeDelimiter)
        {
            GroupHighlighterATNSimulator interpreter = Interpreter;
            if (interpreter.OpenDelimiter == openDelimiter && interpreter.CloseDelimiter == closeDelimiter)
            {
                return;
            }

            _interp = GetSimulatorForDelimiters(openDelimiter, closeDelimiter);
            Interpreter.CopyState(interpreter);
        }

        protected override bool InStringTemplateMode
        {
            get
            {
                if (_modeStack.Count < 2)
                {
                    return false;
                }

                // index 0 is DefaultMode, StringTemplate is always 1 inside that
                return _modeStack[1] == StringTemplate;
            }
        }

        public ICharStream CharStream
        {
            get
            {
                return _input;
            }
        }

        private GroupHighlighterATNSimulator GetSimulatorForDelimiters(char openDelimiter, char closeDelimiter)
        {
            ATN atn = GetATNForDelimiters(openDelimiter, closeDelimiter);
            lock (_atnToSimulator)
            {
                GroupHighlighterATNSimulator simulator;
                if (!_atnToSimulator.TryGetValue(atn, out simulator))
                {
                    simulator = new GroupHighlighterATNSimulator(this, atn, openDelimiter, closeDelimiter);
                    _atnToSimulator[atn] = simulator;
                }

                return simulator;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static ATN GetATNForDelimiters(char openDelimiter, char closeDelimiter)
        {
            int key = (openDelimiter << 16) + (closeDelimiter & 0xFFFF);
            ATN atn;
            if (DelimiterToAtn.TryGetValue(key, out atn))
            {
                return atn;
            }

            atn = new ATNDeserializer().Deserialize(_serializedATN.ToCharArray());
            foreach (ATNState state in atn.states)
            {
                if (state == null)
                {
                    continue;
                }

                for (int i = 0; i < state.NumberOfTransitions; i++)
                {
                    Transition t = state.Transition(i);
                    Transition updated = updateTransition(t, openDelimiter, closeDelimiter);
                    if (updated != null)
                    {
                        state.SetTransition(i, updated);
                    }
                }

                if (!state.IsOptimized)
                {
                    continue;
                }

                for (int i = 0; i < state.NumberOfOptimizedTransitions; i++)
                {
                    Transition t = state.GetOptimizedTransition(i);
                    Transition updated = updateTransition(t, openDelimiter, closeDelimiter);
                    if (updated != null)
                    {
                        state.SetOptimizedTransition(i, updated);
                    }
                }
            }

            DelimiterToAtn[key] = atn;
            return atn;
        }

        private static Transition updateTransition(Transition t, char openDelimiter, char closeDelimiter)
        {
            Transition updated = null;
            if (t is RuleTransition)
            {
                return null;
            }
            else if (t is AtomTransition)
            {
                AtomTransition atomTransition = (AtomTransition)t;
                int newLabel;
                if (atomTransition.label == OpenDelimiterPlaceholder)
                {
                    newLabel = openDelimiter;
                }
                else if (atomTransition.label == CloseDelimiterPlaceholder)
                {
                    newLabel = closeDelimiter;
                }
                else
                {
                    return null;
                }

                updated = new AtomTransition(t.target, newLabel);
            }
            else if (t is NotSetTransition)
            {
                NotSetTransition notSetTransition = (NotSetTransition)t;
                int removeLabel;
                int addLabel;
                if (notSetTransition.set.Contains(OpenDelimiterPlaceholder))
                {
                    removeLabel = OpenDelimiterPlaceholder;
                    addLabel = openDelimiter;
                }
                else if (notSetTransition.set.Contains(CloseDelimiterPlaceholder))
                {
                    removeLabel = CloseDelimiterPlaceholder;
                    addLabel = closeDelimiter;
                }
                else
                {
                    return null;
                }

                IntervalSet set = new IntervalSet(notSetTransition.set);
                set.Remove(removeLabel);
                set.Add(addLabel);
                set.SetReadonly(true);

                updated = new NotSetTransition(t.target, set);
            }
            else if (t is SetTransition)
            {
                SetTransition setTransition = (SetTransition)t;
                int removeLabel;
                int addLabel;
                if (setTransition.set.Contains(OpenDelimiterPlaceholder))
                {
                    removeLabel = OpenDelimiterPlaceholder;
                    addLabel = openDelimiter;
                }
                else if (setTransition.set.Contains(CloseDelimiterPlaceholder))
                {
                    removeLabel = CloseDelimiterPlaceholder;
                    addLabel = closeDelimiter;
                }
                else
                {
                    return null;
                }

                IntervalSet set = new IntervalSet(setTransition.set);
                set.Remove(removeLabel);
                set.Add(addLabel);
                set.SetReadonly(true);

                updated = createSetTransition(t.target, set);
            }
            else if (t is RangeTransition)
            {
                RangeTransition rangeTransition = (RangeTransition)t;
                int removeLabel;
                int addLabel;
                if (rangeTransition.from <= OpenDelimiterPlaceholder && rangeTransition.to >= OpenDelimiterPlaceholder)
                {
                    removeLabel = OpenDelimiterPlaceholder;
                    addLabel = openDelimiter;
                }
                else if (rangeTransition.from <= CloseDelimiterPlaceholder && rangeTransition.to >= CloseDelimiterPlaceholder)
                {
                    removeLabel = CloseDelimiterPlaceholder;
                    addLabel = closeDelimiter;
                }
                else
                {
                    return null;
                }

                IntervalSet set = IntervalSet.Of(rangeTransition.from, rangeTransition.to);
                set.Remove(removeLabel);
                set.Add(addLabel);
                set.SetReadonly(true);

                updated = createSetTransition(t.target, set);
            }

            return updated;
        }

        private static Transition createSetTransition(ATNState target, IntervalSet set)
        {
            if (set.GetIntervals().Count == 1)
            {
                Interval interval = set.GetIntervals()[0];
                if (interval.a == interval.b)
                {
                    return new AtomTransition(target, interval.a);
                }
                else
                {
                    return new RangeTransition(target, interval.a, interval.b);
                }
            }
            else
            {
                return new SetTransition(target, set);
            }
        }

        public ClassifierLexerState GetCurrentState()
        {
            return new ClassifierLexerState(this);
        }

        protected class GroupHighlighterATNSimulator : LexerATNSimulator
        {
            public GroupHighlighterATNSimulator(Lexer recog, ATN atn, char openDelimiter, char closeDelimiter)
                : base(recog, atn)
            {
                this.OpenDelimiter = openDelimiter;
                this.CloseDelimiter = closeDelimiter;
            }

            public char OpenDelimiter
            {
                get;
            }

            public char CloseDelimiter
            {
                get;
            }

            public void ResetAcceptPosition(ICharStream input, int index, int line, int charPositionInLine)
            {
                input.Seek(index);
                this.line = line;
                this.charPositionInLine = charPositionInLine;
                Consume(input);
            }
        }
    }
}
