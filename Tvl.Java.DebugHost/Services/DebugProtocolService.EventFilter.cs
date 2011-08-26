namespace Tvl.Java.DebugHost.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Tvl.Java.DebugInterface.Types;
    using System.Diagnostics.Contracts;
    using System.Collections.ObjectModel;

    partial class DebugProtocolService
    {
        public abstract class EventFilter
        {
            private readonly RequestId _requestId;
            private readonly SuspendPolicy _suspendPolicy;

            public EventFilter(RequestId requestId, SuspendPolicy suspendPolicy)
            {
                _requestId = requestId;
                _suspendPolicy = suspendPolicy;
            }

            public RequestId RequestId
            {
                get
                {
                    return _requestId;
                }
            }

            public SuspendPolicy SuspendPolicy
            {
                get
                {
                    return _suspendPolicy;
                }
            }

            public abstract bool ProcessEvent(ThreadId thread, TaggedReferenceTypeId @class, Location? location);

            public static EventFilter CreateFilter(RequestId requestId, SuspendPolicy suspendPolicy, EventRequestModifier[] modifiers)
            {
                if (modifiers.Length == 0)
                    return new PassThroughEventFilter(requestId, suspendPolicy);

                EventFilter[] elements = Array.ConvertAll(modifiers, modifier => CreateFilter(requestId, suspendPolicy, modifier));
                if (elements.Length == 1)
                    return elements[0];

                return new AggregateEventFilter(requestId, suspendPolicy, elements);
            }

            public static EventFilter CreateFilter(RequestId requestId, SuspendPolicy suspendPolicy, EventRequestModifier modifier)
            {
                switch (modifier.Kind)
                {
                case ModifierKind.Count:
                    return new CountEventFilter(requestId, suspendPolicy, modifier.Count);

                case ModifierKind.ThreadFilter:
                    return new ThreadEventFilter(requestId, suspendPolicy, modifier.Thread);

                case ModifierKind.ClassTypeFilter:
                    throw new NotImplementedException();

                case ModifierKind.ClassMatchFilter:
                    throw new NotImplementedException();

                case ModifierKind.ClassExcludeFilter:
                    throw new NotImplementedException();

                case ModifierKind.LocationFilter:
                    return new LocationEventFilter(requestId, suspendPolicy, modifier.Location);

                case ModifierKind.ExceptionFilter:
                    throw new NotImplementedException();

                case ModifierKind.FieldFilter:
                    throw new NotImplementedException();

                case ModifierKind.Step:
                    return new StepEventFilter(requestId, suspendPolicy, modifier.Thread, modifier.StepSize, modifier.StepDepth);

                case ModifierKind.InstanceFilter:
                    throw new NotImplementedException();

                case ModifierKind.SourceNameMatchFilter:
                    throw new NotImplementedException();

                case ModifierKind.Conditional:
                    throw new NotImplementedException();

                case ModifierKind.Invalid:
                default:
                    throw new ArgumentException();
                }
            }
        }

        public sealed class PassThroughEventFilter : EventFilter
        {
            public PassThroughEventFilter(RequestId requestId, SuspendPolicy suspendPolicy)
                : base(requestId, suspendPolicy)
            {
            }

            public override bool ProcessEvent(ThreadId thread, TaggedReferenceTypeId @class, Location? location)
            {
                return true;
            }
        }

        public sealed class AggregateEventFilter : EventFilter
        {
            private readonly EventFilter[] _filters;

            public AggregateEventFilter(RequestId requestId, SuspendPolicy suspendPolicy, IEnumerable<EventFilter> filters)
                : base(requestId, suspendPolicy)
            {
                Contract.Requires<ArgumentNullException>(filters != null, "filters");
                _filters = filters.ToArray();
            }

            public ReadOnlyCollection<EventFilter> Filters
            {
                get
                {
                    return new ReadOnlyCollection<EventFilter>(_filters);
                }
            }

            public override bool ProcessEvent(ThreadId thread, TaggedReferenceTypeId @class, Location? location)
            {
                foreach (EventFilter filter in _filters)
                {
                    if (!filter.ProcessEvent(thread, @class, location))
                        return false;
                }

                return true;
            }
        }

        public sealed class CountEventFilter : EventFilter
        {
            private readonly int _count;

            private int _current;

            public CountEventFilter(RequestId requestId, SuspendPolicy suspendPolicy, int count)
                : base(requestId, suspendPolicy)
            {
                _count = count;
            }

            public override bool ProcessEvent(ThreadId thread, TaggedReferenceTypeId @class, Location? location)
            {
                _current++;
                if (_current == _count)
                {
                    _current = 0;
                    return true;
                }

                return false;
            }
        }

        public class ThreadEventFilter : EventFilter
        {
            private readonly ThreadId _thread;

            public ThreadEventFilter(RequestId requestId, SuspendPolicy suspendPolicy, ThreadId thread)
                : base(requestId, suspendPolicy)
            {
                _thread = thread;
            }

            public override bool ProcessEvent(ThreadId thread, TaggedReferenceTypeId @class, Location? location)
            {
                return _thread == default(ThreadId)
                    || _thread == thread;
            }
        }

        public sealed class LocationEventFilter : EventFilter
        {
            private readonly Location _location;

            public LocationEventFilter(RequestId requestId, SuspendPolicy suspendPolicy, Location location)
                : base(requestId, suspendPolicy)
            {
                _location = location;
            }

            public Location Location
            {
                get
                {
                    return _location;
                }
            }

            public override bool ProcessEvent(ThreadId thread, TaggedReferenceTypeId @class, Location? location)
            {
                if (!location.HasValue)
                    return false;

                return _location.Index == location.Value.Index
                    && _location.Class == location.Value.Class
                    && _location.Method == location.Value.Method;
            }
        }

        public sealed class StepEventFilter : ThreadEventFilter
        {
            private readonly StepSize _size;
            private readonly StepDepth _depth;

            public StepEventFilter(RequestId requestId, SuspendPolicy suspendPolicy, ThreadId thread, StepSize size, StepDepth depth)
                : base(requestId, suspendPolicy, thread)
            {
                _size = size;
                _depth = depth;
            }

            public override bool ProcessEvent(ThreadId thread, TaggedReferenceTypeId @class, Location? location)
            {
                if (!base.ProcessEvent(thread, @class, location))
                    return false;

                if (_size != StepSize.Minimum || _depth != StepDepth.Into)
                    throw new NotImplementedException();

                return true;
            }
        }
    }
}
