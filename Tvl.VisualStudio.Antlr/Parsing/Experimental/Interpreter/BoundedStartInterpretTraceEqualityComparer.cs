namespace Tvl.VisualStudio.Language.Parsing.Experimental.Interpreter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Tvl.VisualStudio.Language.Parsing.Experimental.Atn;

    public class BoundedStartInterpretTraceEqualityComparer : IEqualityComparer<InterpretTrace>
    {
        private static readonly BoundedStartInterpretTraceEqualityComparer _default = new BoundedStartInterpretTraceEqualityComparer();

        private BoundedStartInterpretTraceEqualityComparer()
        {
        }

        public static BoundedStartInterpretTraceEqualityComparer Default
        {
            get
            {
                return _default;
            }
        }

        public virtual bool Equals(InterpretTrace x, InterpretTrace y)
        {
            if (object.ReferenceEquals(x, y))
                return true;

            if (x == null || y == null)
                return false;

            if (object.ReferenceEquals(x.Transitions, y.Transitions))
                return true;

            if (x.Transitions.Count == 0)
                return y.Transitions.Count == 0;

            if (y.Transitions.Count == 0)
                return false;

            // unique on the first transition and start position
            InterpretTraceTransition firstx = x.Transitions.First.Value;
            InterpretTraceTransition firsty = y.Transitions.First.Value;
            if (!EqualityComparer<Transition>.Default.Equals(firstx.Transition, firsty.Transition))
                return false;

            InterpretTraceTransition firstxmatch = x.Transitions.FirstOrDefault(i => i.Transition.IsMatch);
            InterpretTraceTransition firstymatch = y.Transitions.FirstOrDefault(i => i.Transition.IsMatch);
            if (firstxmatch == null)
                return firstymatch == null;

            if (firstymatch == null)
                return false;

            if (firstxmatch.TokenIndex != firstymatch.TokenIndex)
                return false;

            return true;
        }

        public virtual int GetHashCode(InterpretTrace obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            if (obj.Transitions.Count == 0)
                return 0;

            InterpretTraceTransition first = obj.Transitions.First.Value;
            int transitionCode = first.Transition.GetHashCode();

            InterpretTraceTransition firstmatch = obj.Transitions.FirstOrDefault(i => i.Transition.IsMatch);
            if (firstmatch == null)
                return transitionCode;

            return transitionCode ^ firstmatch.TokenIndex.GetHashCode();
        }
    }
}
