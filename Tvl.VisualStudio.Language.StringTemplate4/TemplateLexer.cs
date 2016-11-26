namespace Tvl.VisualStudio.Language.StringTemplate4
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Atn;
    using Antlr4.Runtime.Misc;

    public class TemplateLexer : AbstractTemplateLexer
    {
        public static readonly char DEFAULT_OPEN_DELIMITER = '<';
        public static readonly char DEFAULT_CLOSE_DELIMITER = '>';

        private static readonly IDictionary<int, ATN> delimiterToATN = new Dictionary<int, ATN>();
        private static readonly int OPEN_DELIMITER_PLACEHOLDER = '\uFFF0';
        private static readonly int CLOSE_DELIMITER_PLACEHOLDER = '\uFFF1';

        private readonly IDictionary<ATN, TemplateLexerATNSimulator> atnToSimulator = new Dictionary<ATN, TemplateLexerATNSimulator>();

        public TemplateLexer(ICharStream input)
            : this(input, DEFAULT_OPEN_DELIMITER, DEFAULT_CLOSE_DELIMITER)
        {
        }

        public TemplateLexer(ICharStream input, char openDelimiter, char closeDelimiter)
                : base(input)
        {
            _interp = getSimulatorForDelimiters(openDelimiter, closeDelimiter);
        }

        public override IToken Emit()
        {
            switch (_type)
            {
            case LBRACE:
                if (InputStream.Index > _tokenStartCharIndex + 1)
                {
                    Interpreter.resetAcceptPosition((ICharStream)InputStream, _tokenStartCharIndex, _tokenStartLine, _tokenStartCharPositionInLine);
                    PushMode(AnonymousTemplateParameters);
                }
                break;

            case DELIMITERS:
                if (_type == DELIMITERS && InputStream.Index > _tokenStartCharIndex + "delimiters".Length)
                {
                    int offset = "delimiters".Length - 1;
                    Interpreter.resetAcceptPosition((ICharStream)InputStream, _tokenStartCharIndex + offset, _tokenStartLine, _tokenStartCharPositionInLine + offset);
                    PushMode(DelimitersOpenSpec);
                }
                break;

            case DelimitersOpenSpec_DELIMITER_STRING:
                setDelimiters(Text[1], CloseDelimiter);
                _type = STRING;
                break;

            case DelimitersCloseSpec_DELIMITER_STRING:
                setDelimiters(OpenDelimiter, Text[1]);
                _type = STRING;
                break;

            default:
                break;
            }

            return base.Emit();
        }

        protected new TemplateLexerATNSimulator Interpreter
        {
            get
            {
                return (TemplateLexerATNSimulator)base.Interpreter;
            }
        }

        public char OpenDelimiter
            => Interpreter.OpenDelimiter;

        public char CloseDelimiter
            => Interpreter.CloseDelimiter;

        public void setDelimiters(char openDelimiter, char closeDelimiter)
        {
            TemplateLexerATNSimulator interpreter = Interpreter;
            if (interpreter.OpenDelimiter == openDelimiter && interpreter.CloseDelimiter == closeDelimiter)
            {
                return;
            }

            _interp = getSimulatorForDelimiters(openDelimiter, closeDelimiter);
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

                // index 0 is DEFAULT_MODE, StringTemplate is always 1 inside that
                return _modeStack[1] == StringTemplate;
            }
        }

        private TemplateLexerATNSimulator getSimulatorForDelimiters(char openDelimiter, char closeDelimiter)
        {
            ATN atn = getATNForDelimiters(openDelimiter, closeDelimiter);
            lock (atnToSimulator)
            {
                TemplateLexerATNSimulator simulator;
                if (!atnToSimulator.TryGetValue(atn, out simulator))
                {
                    simulator = new TemplateLexerATNSimulator(this, atn, openDelimiter, closeDelimiter);
                    atnToSimulator[atn] = simulator;
                }

                return simulator;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static ATN getATNForDelimiters(char openDelimiter, char closeDelimiter)
        {
            int key = (openDelimiter << 16) + (closeDelimiter & 0xFFFF);
            ATN atn;
            if (delimiterToATN.TryGetValue(key, out atn))
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

            delimiterToATN[key] = atn;
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
                if (atomTransition.label == OPEN_DELIMITER_PLACEHOLDER)
                {
                    newLabel = openDelimiter;
                }
                else if (atomTransition.label == CLOSE_DELIMITER_PLACEHOLDER)
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
                if (notSetTransition.set.Contains(OPEN_DELIMITER_PLACEHOLDER))
                {
                    removeLabel = OPEN_DELIMITER_PLACEHOLDER;
                    addLabel = openDelimiter;
                }
                else if (notSetTransition.set.Contains(CLOSE_DELIMITER_PLACEHOLDER))
                {
                    removeLabel = CLOSE_DELIMITER_PLACEHOLDER;
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
                if (setTransition.set.Contains(OPEN_DELIMITER_PLACEHOLDER))
                {
                    removeLabel = OPEN_DELIMITER_PLACEHOLDER;
                    addLabel = openDelimiter;
                }
                else if (setTransition.set.Contains(CLOSE_DELIMITER_PLACEHOLDER))
                {
                    removeLabel = CLOSE_DELIMITER_PLACEHOLDER;
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
                if (rangeTransition.from <= OPEN_DELIMITER_PLACEHOLDER && rangeTransition.to >= OPEN_DELIMITER_PLACEHOLDER)
                {
                    removeLabel = OPEN_DELIMITER_PLACEHOLDER;
                    addLabel = openDelimiter;
                }
                else if (rangeTransition.from <= CLOSE_DELIMITER_PLACEHOLDER && rangeTransition.to >= CLOSE_DELIMITER_PLACEHOLDER)
                {
                    removeLabel = CLOSE_DELIMITER_PLACEHOLDER;
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

        protected class TemplateLexerATNSimulator : LexerATNSimulator
        {
            private readonly char openDelimiter;
            private readonly char closeDelimiter;

            public TemplateLexerATNSimulator(Lexer recog, ATN atn, char openDelimiter, char closeDelimiter)
                : base(recog, atn)
            {
                this.openDelimiter = openDelimiter;
                this.closeDelimiter = closeDelimiter;
            }

            public char OpenDelimiter => openDelimiter;
            public char CloseDelimiter => closeDelimiter;

            public void resetAcceptPosition(ICharStream input, int index, int line, int charPositionInLine)
            {
                input.Seek(index);
                this.line = line;
                this.charPositionInLine = charPositionInLine;
                Consume(input);
            }

        }
    }
}
