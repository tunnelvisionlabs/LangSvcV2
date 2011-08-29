namespace Tvl.Java.DebugHost.Services
{
    using Tvl.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Tvl.Java.DebugInterface.Types;
    using System.Diagnostics.Contracts;
    using System.Collections.ObjectModel;
    using Tvl.Java.DebugHost.Interop;
    using Tvl.Collections;
    using Tvl.Java.DebugInterface.Types.Analysis;

    partial class DebugProtocolService
    {
        internal abstract class EventFilter
        {
            private readonly EventKind _internalEventKind;
            private readonly RequestId _requestId;
            private readonly SuspendPolicy _suspendPolicy;
            private readonly ImmutableList<EventRequestModifier> _modifiers;

            public EventFilter(EventKind internalEventKind, RequestId requestId, SuspendPolicy suspendPolicy, IEnumerable<EventRequestModifier> modifiers)
            {
                Contract.Requires<ArgumentNullException>(modifiers != null, "modifiers");

                _internalEventKind = internalEventKind;
                _requestId = requestId;
                _suspendPolicy = suspendPolicy;
                _modifiers = new ImmutableList<EventRequestModifier>(modifiers);
            }

            public EventKind InternalEventKind
            {
                get
                {
                    return _internalEventKind;
                }
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

            public ImmutableList<EventRequestModifier> Modifiers
            {
                get
                {
                    return _modifiers;
                }
            }

            public abstract bool ProcessEvent(JvmtiEnvironment environment, JniEnvironment nativeEnvironment, EventProcessor processor, ThreadId thread, TaggedReferenceTypeId @class, Location? location);

            public static EventFilter CreateFilter(EventKind internalEventKind, JvmtiEnvironment environment, JniEnvironment nativeEnvironment, RequestId requestId, SuspendPolicy suspendPolicy, ImmutableList<EventRequestModifier> modifiers)
            {
                if (modifiers.Count == 0)
                    return new PassThroughEventFilter(internalEventKind, requestId, suspendPolicy, modifiers);

                EventFilter[] elements = modifiers.Select(modifier => CreateFilter(internalEventKind, environment, nativeEnvironment, requestId, suspendPolicy, modifiers, modifier)).ToArray();
                if (elements.Length == 1)
                    return elements[0];

                return new AggregateEventFilter(internalEventKind, requestId, suspendPolicy, modifiers, elements);
            }

            private static EventFilter CreateFilter(EventKind internalEventKind, JvmtiEnvironment environment, JniEnvironment nativeEnvironment, RequestId requestId, SuspendPolicy suspendPolicy, ImmutableList<EventRequestModifier> modifiers, EventRequestModifier modifier)
            {
                switch (modifier.Kind)
                {
                case ModifierKind.Count:
                    return new CountEventFilter(internalEventKind, requestId, suspendPolicy, modifiers, modifier.Count);

                case ModifierKind.ThreadFilter:
                    return new ThreadEventFilter(internalEventKind, requestId, suspendPolicy, modifiers, modifier.Thread);

                case ModifierKind.ClassTypeFilter:
                    throw new NotImplementedException();

                case ModifierKind.ClassMatchFilter:
                    throw new NotImplementedException();

                case ModifierKind.ClassExcludeFilter:
                    throw new NotImplementedException();

                case ModifierKind.LocationFilter:
                    return new LocationEventFilter(internalEventKind, requestId, suspendPolicy, modifiers, modifier.Location);

                case ModifierKind.ExceptionFilter:
                    throw new NotImplementedException();

                case ModifierKind.FieldFilter:
                    throw new NotImplementedException();

                case ModifierKind.Step:
                    return new StepEventFilter(internalEventKind, requestId, suspendPolicy, modifiers, modifier.Thread, environment, nativeEnvironment, modifier.StepSize, modifier.StepDepth);

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

        internal sealed class PassThroughEventFilter : EventFilter
        {
            public PassThroughEventFilter(EventKind internalEventKind, RequestId requestId, SuspendPolicy suspendPolicy, IEnumerable<EventRequestModifier> modifiers)
                : base(internalEventKind, requestId, suspendPolicy, modifiers)
            {
            }

            public override bool ProcessEvent(JvmtiEnvironment environment, JniEnvironment nativeEnvironment, EventProcessor processor, ThreadId thread, TaggedReferenceTypeId @class, Location? location)
            {
                return true;
            }
        }

        internal sealed class AggregateEventFilter : EventFilter
        {
            private readonly EventFilter[] _filters;

            public AggregateEventFilter(EventKind internalEventKind, RequestId requestId, SuspendPolicy suspendPolicy, IEnumerable<EventRequestModifier> modifiers, IEnumerable<EventFilter> filters)
                : base(internalEventKind, requestId, suspendPolicy, modifiers)
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

            public override bool ProcessEvent(JvmtiEnvironment environment, JniEnvironment nativeEnvironment, EventProcessor processor, ThreadId thread, TaggedReferenceTypeId @class, Location? location)
            {
                foreach (EventFilter filter in _filters)
                {
                    if (!filter.ProcessEvent(environment, nativeEnvironment, processor, thread, @class, location))
                        return false;
                }

                return true;
            }
        }

        internal sealed class CountEventFilter : EventFilter
        {
            private readonly int _count;

            private int _current;

            public CountEventFilter(EventKind internalEventKind, RequestId requestId, SuspendPolicy suspendPolicy, IEnumerable<EventRequestModifier> modifiers, int count)
                : base(internalEventKind, requestId, suspendPolicy, modifiers)
            {
                _count = count;
            }

            public override bool ProcessEvent(JvmtiEnvironment environment, JniEnvironment nativeEnvironment, EventProcessor processor, ThreadId thread, TaggedReferenceTypeId @class, Location? location)
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

        internal class ThreadEventFilter : EventFilter
        {
            private readonly ThreadId _thread;

            public ThreadEventFilter(EventKind internalEventKind, RequestId requestId, SuspendPolicy suspendPolicy, IEnumerable<EventRequestModifier> modifiers, ThreadId thread)
                : base(internalEventKind, requestId, suspendPolicy, modifiers)
            {
                _thread = thread;
            }

            public override bool ProcessEvent(JvmtiEnvironment environment, JniEnvironment nativeEnvironment, EventProcessor processor, ThreadId thread, TaggedReferenceTypeId @class, Location? location)
            {
                return _thread == default(ThreadId)
                    || _thread == thread;
            }
        }

        internal sealed class LocationEventFilter : EventFilter
        {
            private readonly Location _location;

            public LocationEventFilter(EventKind internalEventKind, RequestId requestId, SuspendPolicy suspendPolicy, IEnumerable<EventRequestModifier> modifiers, Location location)
                : base(internalEventKind, requestId, suspendPolicy, modifiers)
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

            public override bool ProcessEvent(JvmtiEnvironment environment, JniEnvironment nativeEnvironment, EventProcessor processor, ThreadId thread, TaggedReferenceTypeId @class, Location? location)
            {
                if (!location.HasValue)
                    return false;

                return _location.Index == location.Value.Index
                    && _location.Class == location.Value.Class
                    && _location.Method == location.Value.Method;
            }
        }

        internal sealed class StepEventFilter : ThreadEventFilter
        {
            private readonly StepSize _size;
            private readonly StepDepth _depth;

            // used for step over
            private bool _hasMethodInfo;
            private jmethodID _lastMethod;
            private jlocation _lastLocation;
            private int _stackDepth;
            private bool _convertedToFramePop;

            private DisassembledMethod _disassembledMethod;
            private ConstantPoolEntry[] _constantPool;
            private ImmutableList<int?> _evaluationStackDepths;

            public StepEventFilter(EventKind internalEventKind, RequestId requestId, SuspendPolicy suspendPolicy, IEnumerable<EventRequestModifier> modifiers, ThreadId thread, JvmtiEnvironment environment, JniEnvironment nativeEnvironment, StepSize size, StepDepth depth)
                : base(internalEventKind, requestId, suspendPolicy, modifiers, thread)
            {
                _size = size;
                _depth = depth;

                // gather reference information for the thread
                using (var threadHandle = environment.VirtualMachine.GetLocalReferenceForThread(nativeEnvironment, thread))
                {
                    if (threadHandle.IsAlive)
                    {
                        jvmtiError error = environment.GetFrameLocation(threadHandle.Value, 0, out _lastMethod, out _lastLocation);
                        if (error == jvmtiError.None)
                            error = environment.GetFrameCount(threadHandle.Value, out _stackDepth);

                        if (error == jvmtiError.None)
                            _hasMethodInfo = true;

                        if (error == jvmtiError.None && size == StepSize.Statement && (depth == StepDepth.Over || depth == StepDepth.Into))
                        {
                            byte[] bytecode;
                            JvmtiErrorHandler.ThrowOnFailure(environment.GetBytecodes(_lastMethod, out bytecode));
                            _disassembledMethod = BytecodeDisassembler.Disassemble(bytecode);

                            TaggedReferenceTypeId declaringClass;
                            JvmtiErrorHandler.ThrowOnFailure(environment.GetMethodDeclaringClass(nativeEnvironment, _lastMethod, out declaringClass));
                            using (var classHandle = environment.VirtualMachine.GetLocalReferenceForClass(nativeEnvironment, declaringClass.TypeId))
                            {
                                int constantPoolCount;
                                byte[] data;
                                JvmtiErrorHandler.ThrowOnFailure(environment.GetConstantPool(classHandle.Value, out constantPoolCount, out data));

                                List<ConstantPoolEntry> entryList = new List<ConstantPoolEntry>();
                                int currentPosition = 0;
                                for (int i = 0; i < constantPoolCount - 1; i++)
                                    entryList.Add(ConstantPoolEntry.FromBytes(data, ref currentPosition));

                                _constantPool = entryList.ToArray();

                                _evaluationStackDepths = BytecodeDisassembler.GetEvaluationStackDepths(_disassembledMethod, new ReadOnlyCollection<ConstantPoolEntry>(_constantPool));
                            }
                        }
                    }
                }
            }

            public override bool ProcessEvent(JvmtiEnvironment environment, JniEnvironment nativeEnvironment, EventProcessor processor, ThreadId thread, TaggedReferenceTypeId @class, Location? location)
            {
                if (!base.ProcessEvent(environment, nativeEnvironment, processor, thread, @class, location))
                    return false;

                // Step Out is implemented with Frame Pop events set at the correct depth
                if (_depth == StepDepth.Out)
                    return true;

                using (var threadHandle = environment.VirtualMachine.GetLocalReferenceForThread(nativeEnvironment, thread))
                {
                    int stackDepth;
                    JvmtiErrorHandler.ThrowOnFailure(environment.GetFrameCount(threadHandle.Value, out stackDepth));

                    if (_depth == StepDepth.Over && _hasMethodInfo && stackDepth > _stackDepth)
                    {
                        if (_depth == StepDepth.Into)
                            return true;

                        if (!_convertedToFramePop)
                        {
                            /*
                             * change this to a Frame Pop event if we're not in a native frame
                             */

                            bool native;
                            JvmtiErrorHandler.ThrowOnFailure(environment.IsMethodNative(location.Value.Method, out native));
                            if (!native)
                            {
                                // remove the single step event
                                JvmtiErrorHandler.ThrowOnFailure((jvmtiError)processor.ClearEventInternal(EventKind.SingleStep, this.RequestId));
                                // set an actual step filter to respond when the thread arrives back in this frame
                                JvmtiErrorHandler.ThrowOnFailure((jvmtiError)processor.SetEventInternal(environment, nativeEnvironment, EventKind.FramePop, this));
                                _convertedToFramePop = true;
                            }

                            return false;
                        }
                        else
                        {
                            _convertedToFramePop = false;
                            return true;
                        }
                    }
                    else if (stackDepth == _stackDepth && _size == StepSize.Statement && _disassembledMethod != null)
                    {
                        int instructionIndex = _disassembledMethod.Instructions.FindIndex(i => (uint)i.Offset == location.Value.Index);
                        if (instructionIndex >= 0 && _evaluationStackDepths != null && (_evaluationStackDepths[instructionIndex] ?? 0) != 0)
                        {
                            return false;
                        }
                        else if (instructionIndex >= 0 && _disassembledMethod.Instructions[instructionIndex].OpCode.FlowControl == JavaFlowControl.Branch)
                        {
                            // follow branch instructions before stopping
                            return false;
                        }
                    }

                    _stackDepth = stackDepth;
                    return true;
                }
            }
        }
    }
}
