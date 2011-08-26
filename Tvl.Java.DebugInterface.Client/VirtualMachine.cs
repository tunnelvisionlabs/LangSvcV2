namespace Tvl.Java.DebugInterface.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Collections.ObjectModel;
    using Tvl.Java.DebugInterface.Request;
    using Tvl.Java.DebugInterface.Events;
    using Tvl.Java.DebugInterface.Client.Request;
    using Tvl.Java.DebugInterface.Client.DebugProtocol;
    using Tvl.Java.DebugInterface.Types;
    using AccessModifiers = Tvl.Java.DebugInterface.AccessModifiers;
    using EventWaitHandle = System.Threading.EventWaitHandle;
    using EventResetMode = System.Threading.EventResetMode;
    using Tvl.Extensions;
    using System.ServiceModel;
    using Tvl.Java.DebugInterface.Client.Events;
    using System.Diagnostics.Contracts;
    using Task = System.Threading.Tasks.Task;

    internal partial class VirtualMachine : IVirtualMachine
    {
        private readonly PrimitiveTypes _primitiveTypes;
        private readonly EventQueue _eventQueue;
        private readonly EventRequestManager _eventRequestManager;

        private EventWaitHandle _ipcHandle;
        private DebugSession.IJvmDebugSessionService _sessionService;
        private DebugProtocol.IDebugProtocolService _protocolService;

        private bool _disposed;

        public VirtualMachine()
        {
            _primitiveTypes = new PrimitiveTypes(this);
            _eventRequestManager = new EventRequestManager(this);
            _eventQueue = new EventQueue(this);
        }

        public event EventHandler AttachComplete;

        internal EventQueue EventQueue
        {
            get
            {
                return _eventQueue;
            }
        }

        internal EventRequestManager EventRequestManager
        {
            get
            {
                return _eventRequestManager;
            }
        }

        internal PrimitiveTypes PrimitiveTypes
        {
            get
            {
                return _primitiveTypes;
            }
        }

        public IDebugProtocolService ProtocolService
        {
            get
            {
                return _protocolService;
            }
        }

        public bool IsDisposed
        {
            get
            {
                return _disposed;
            }
        }

        internal static VirtualMachine BeginAttachToProcess(int processId)
        {
            VirtualMachine virtualMachine = new VirtualMachine();
            virtualMachine._ipcHandle = new EventWaitHandle(false, EventResetMode.ManualReset, string.Format("JavaDebuggerInitHandle{0}", processId));

            Task initializeTask = Task.Factory.StartNew(virtualMachine.InitializeServicesAfterProcessStarts).HandleNonCriticalExceptions();

            return virtualMachine;
        }

        private void InitializeServicesAfterProcessStarts()
        {
            try
            {
                _ipcHandle.WaitOne();
                _ipcHandle.Dispose();
                _ipcHandle = null;

                CreateSessionServiceClient();
                CreateProtocolServiceClient();

                _sessionService.Attach();

                OnAttachComplete(EventArgs.Empty);
            }
            catch (Exception e)
            {
                if (e.IsCritical())
                    throw;
            }
        }

        private void CreateSessionServiceClient()
        {
            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None)
            {
                ReceiveTimeout = TimeSpan.MaxValue,
                SendTimeout = TimeSpan.MaxValue
            };

            var remoteAddress = new EndpointAddress("net.pipe://localhost/Tvl.Java.DebugHost/JvmDebugSessionService/");
            _sessionService = new DebugSession.JvmDebugSessionServiceClient(binding, remoteAddress);
        }

        private void CreateProtocolServiceClient()
        {
            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None)
            {
                ReceiveTimeout = TimeSpan.MaxValue,
                SendTimeout = TimeSpan.MaxValue
            };

            DebugProtocolCallback callback = new DebugProtocolCallback(this);
            var callbackInstance = new InstanceContext(callback);
            var remoteAddress = new EndpointAddress("net.pipe://localhost/Tvl.Java.DebugHost/DebugProtocolService/");
            _protocolService = new DebugProtocol.DebugProtocolServiceClient(callbackInstance, binding, remoteAddress);
            _protocolService.Attach();
        }

        #region IVirtualMachine Members

        public ReadOnlyCollection<IReferenceType> GetAllClasses()
        {
            if (ProtocolService == null)
                throw new VirtualMachineDisconnectedException();

            ReferenceTypeData[] classes;
            DebugErrorHandler.ThrowOnFailure(ProtocolService.GetAllClasses(out classes));
            return new ReadOnlyCollection<IReferenceType>(Array.ConvertAll(classes, GetMirrorOf));
        }

        public ReadOnlyCollection<IThreadReference> GetAllThreads()
        {
            if (ProtocolService == null)
                throw new VirtualMachineDisconnectedException();

            ThreadId[] threads;
            DebugErrorHandler.ThrowOnFailure(ProtocolService.GetAllThreads(out threads));
            return new ReadOnlyCollection<IThreadReference>(Array.ConvertAll(threads, GetMirrorOf));
        }

        public bool GetCanAddMethod()
        {
            if (ProtocolService == null)
                throw new VirtualMachineDisconnectedException();

            Capabilities capabilities;
            DebugErrorHandler.ThrowOnFailure(ProtocolService.GetCapabilities(out capabilities));
            //return (capabilities & Capabilities.CanAddMethod) != 0;
            throw new NotImplementedException();
        }

        public bool GetCanBeModified()
        {
            if (ProtocolService == null)
                throw new VirtualMachineDisconnectedException();

            Capabilities capabilities;
            DebugErrorHandler.ThrowOnFailure(ProtocolService.GetCapabilities(out capabilities));
            //return (capabilities & Capabilities.CanBeModified) != 0;
            throw new NotImplementedException();
        }

        public bool GetCanForceEarlyReturn()
        {
            if (ProtocolService == null)
                throw new VirtualMachineDisconnectedException();

            Capabilities capabilities;
            DebugErrorHandler.ThrowOnFailure(ProtocolService.GetCapabilities(out capabilities));
            return (capabilities & Capabilities.CanForceEarlyReturn) != 0;
        }

        public bool GetCanGetBytecodes()
        {
            if (ProtocolService == null)
                throw new VirtualMachineDisconnectedException();

            Capabilities capabilities;
            DebugErrorHandler.ThrowOnFailure(ProtocolService.GetCapabilities(out capabilities));
            return (capabilities & Capabilities.CanGetBytecodes) != 0;
        }

        public bool GetCanGetClassFileVersion()
        {
            if (ProtocolService == null)
                throw new VirtualMachineDisconnectedException();

            Capabilities capabilities;
            DebugErrorHandler.ThrowOnFailure(ProtocolService.GetCapabilities(out capabilities));
            //return (capabilities & Capabilities.CanGetClassFileVersion) != 0;
            throw new NotImplementedException();
        }

        public bool GetCanGetConstantPool()
        {
            if (ProtocolService == null)
                throw new VirtualMachineDisconnectedException();

            Capabilities capabilities;
            DebugErrorHandler.ThrowOnFailure(ProtocolService.GetCapabilities(out capabilities));
            return (capabilities & Capabilities.CanGetConstantPool) != 0;
        }

        public bool GetCanGetCurrentContendedMonitor()
        {
            if (ProtocolService == null)
                throw new VirtualMachineDisconnectedException();

            Capabilities capabilities;
            DebugErrorHandler.ThrowOnFailure(ProtocolService.GetCapabilities(out capabilities));
            return (capabilities & Capabilities.CanGetCurrentContendedMonitor) != 0;
        }

        public bool GetCanGetInstanceInfo()
        {
            if (ProtocolService == null)
                throw new VirtualMachineDisconnectedException();

            Capabilities capabilities;
            DebugErrorHandler.ThrowOnFailure(ProtocolService.GetCapabilities(out capabilities));
            //return (capabilities & Capabilities.CanGetInstanceInfo) != 0;
            throw new NotImplementedException();
        }

        public bool GetCanGetMethodReturnValues()
        {
            if (ProtocolService == null)
                throw new VirtualMachineDisconnectedException();

            Capabilities capabilities;
            DebugErrorHandler.ThrowOnFailure(ProtocolService.GetCapabilities(out capabilities));
            //return (capabilities & Capabilities.CanGetMethodReturnValues) != 0;
            throw new NotImplementedException();
        }

        public bool GetCanGetMonitorFrameInfo()
        {
            if (ProtocolService == null)
                throw new VirtualMachineDisconnectedException();

            Capabilities capabilities;
            DebugErrorHandler.ThrowOnFailure(ProtocolService.GetCapabilities(out capabilities));
            return (capabilities & Capabilities.CanGetOwnedMonitorStackDepthInfo) != 0;
        }

        public bool GetCanGetMonitorInfo()
        {
            if (ProtocolService == null)
                throw new VirtualMachineDisconnectedException();

            Capabilities capabilities;
            DebugErrorHandler.ThrowOnFailure(ProtocolService.GetCapabilities(out capabilities));
            return (capabilities & Capabilities.CanGetMonitorInfo) != 0;
        }

        public bool GetCanGetOwnedMonitorInfo()
        {
            if (ProtocolService == null)
                throw new VirtualMachineDisconnectedException();

            Capabilities capabilities;
            DebugErrorHandler.ThrowOnFailure(ProtocolService.GetCapabilities(out capabilities));
            return (capabilities & Capabilities.CanGetOwnedMonitorInfo) != 0;
        }

        public bool GetCanGetSourceDebugExtension()
        {
            if (ProtocolService == null)
                throw new VirtualMachineDisconnectedException();

            Capabilities capabilities;
            DebugErrorHandler.ThrowOnFailure(ProtocolService.GetCapabilities(out capabilities));
            return (capabilities & Capabilities.CanGetSourceDebugExtension) != 0;
        }

        public bool GetCanGetSyntheticAttribute()
        {
            if (ProtocolService == null)
                throw new VirtualMachineDisconnectedException();

            Capabilities capabilities;
            DebugErrorHandler.ThrowOnFailure(ProtocolService.GetCapabilities(out capabilities));
            return (capabilities & Capabilities.CanGetSyntheticAttribute) != 0;
        }

        public bool GetCanPopFrames()
        {
            if (ProtocolService == null)
                throw new VirtualMachineDisconnectedException();

            Capabilities capabilities;
            DebugErrorHandler.ThrowOnFailure(ProtocolService.GetCapabilities(out capabilities));
            return (capabilities & Capabilities.CanPopFrame) != 0;
        }

        public bool GetCanRedefineClasses()
        {
            if (ProtocolService == null)
                throw new VirtualMachineDisconnectedException();

            Capabilities capabilities;
            DebugErrorHandler.ThrowOnFailure(ProtocolService.GetCapabilities(out capabilities));
            return (capabilities & Capabilities.CanRedefineClasses) != 0;
        }

        public bool GetCanRequestMonitorEvents()
        {
            if (ProtocolService == null)
                throw new VirtualMachineDisconnectedException();

            Capabilities capabilities;
            DebugErrorHandler.ThrowOnFailure(ProtocolService.GetCapabilities(out capabilities));
            return (capabilities & Capabilities.CanGenerateMonitorEvents) != 0;
        }

        public bool GetCanRequestVMDeathEvent()
        {
            if (ProtocolService == null)
                throw new VirtualMachineDisconnectedException();

            Capabilities capabilities;
            DebugErrorHandler.ThrowOnFailure(ProtocolService.GetCapabilities(out capabilities));
            //return (capabilities & Capabilities.CanRequestVMDeathEvent) != 0;
            throw new NotImplementedException();
        }

        public bool GetCanUnrestrictedlyRedefineClasses()
        {
            if (ProtocolService == null)
                throw new VirtualMachineDisconnectedException();

            Capabilities capabilities;
            DebugErrorHandler.ThrowOnFailure(ProtocolService.GetCapabilities(out capabilities));
            return (capabilities & Capabilities.CanRedefineAnyClass) != 0;
        }

        public bool GetCanUseInstanceFilters()
        {
            if (ProtocolService == null)
                throw new VirtualMachineDisconnectedException();

            Capabilities capabilities;
            DebugErrorHandler.ThrowOnFailure(ProtocolService.GetCapabilities(out capabilities));
            //return (capabilities & Capabilities.CanUseInstanceFilters) != 0;
            throw new NotImplementedException();
        }

        public bool GetCanUseSourceNameFilters()
        {
            if (ProtocolService == null)
                throw new VirtualMachineDisconnectedException();

            Capabilities capabilities;
            DebugErrorHandler.ThrowOnFailure(ProtocolService.GetCapabilities(out capabilities));
            //return (capabilities & Capabilities.CanUseSourceNameFilters) != 0;
            throw new NotImplementedException();
        }

        public bool GetCanWatchFieldAccess()
        {
            if (ProtocolService == null)
                throw new VirtualMachineDisconnectedException();

            Capabilities capabilities;
            DebugErrorHandler.ThrowOnFailure(ProtocolService.GetCapabilities(out capabilities));
            return (capabilities & Capabilities.CanGenerateFieldAccessEvents) != 0;
        }

        public bool GetCanWatchFieldModification()
        {
            if (ProtocolService == null)
                throw new VirtualMachineDisconnectedException();

            Capabilities capabilities;
            DebugErrorHandler.ThrowOnFailure(ProtocolService.GetCapabilities(out capabilities));
            return (capabilities & Capabilities.CanGenerateFieldModificationEvents) != 0;
        }

        public ReadOnlyCollection<IReferenceType> GetClassesByName(string className)
        {
            throw new NotImplementedException();
        }

        public string GetDescription()
        {
            throw new NotImplementedException();
        }

        public IEventQueue GetEventQueue()
        {
            return _eventQueue;
        }

        public IEventRequestManager GetEventRequestManager()
        {
            return _eventRequestManager;
        }

        public void Exit(int exitCode)
        {
            if (ProtocolService == null)
                return;

            DebugErrorHandler.ThrowOnFailure(ProtocolService.Exit(exitCode));
        }

        public string GetDefaultStratum()
        {
            throw new NotImplementedException();
        }

        public long[] GetInstanceCounts(IEnumerable<IReferenceType> referenceTypes)
        {
            throw new NotImplementedException();
        }

        public BooleanValue GetMirrorOf(bool value)
        {
            return new BooleanValue(this, value);
        }

        public ByteValue GetMirrorOf(byte value)
        {
            return new ByteValue(this, value);
        }

        public CharValue GetMirrorOf(char value)
        {
            return new CharValue(this, value);
        }

        public DoubleValue GetMirrorOf(double value)
        {
            return new DoubleValue(this, value);
        }

        public FloatValue GetMirrorOf(float value)
        {
            return new FloatValue(this, value);
        }

        public IntegerValue GetMirrorOf(int value)
        {
            return new IntegerValue(this, value);
        }

        public LongValue GetMirrorOf(long value)
        {
            return new LongValue(this, value);
        }

        public ShortValue GetMirrorOf(short value)
        {
            return new ShortValue(this, value);
        }

        public StringReference GetMirrorOf(string value)
        {
            StringId stringObject;
            DebugErrorHandler.ThrowOnFailure(ProtocolService.CreateString(out stringObject, value));
            return GetMirrorOf(stringObject);
        }

        public VoidValue GetMirrorOfVoid()
        {
            return this.PrimitiveTypes.VoidValue;
        }

        internal ArrayReference GetMirrorOf(ArrayId array)
        {
            return new ArrayReference(this, array);
        }

        internal StringReference GetMirrorOf(StringId @string)
        {
            return new StringReference(this, @string);
        }

        internal ThreadReference GetMirrorOf(ThreadId thread)
        {
            return new ThreadReference(this, thread);
        }

        internal ThreadGroupReference GetMirrorOf(ThreadGroupId threadGroup)
        {
            return new ThreadGroupReference(this, threadGroup);
        }

        internal ClassLoaderReference GetMirrorOf(ClassLoaderId classLoader)
        {
            return new ClassLoaderReference(this, classLoader);
        }

        internal ClassObjectReference GetMirrorOf(ClassObjectId classObject)
        {
            return new ClassObjectReference(this, classObject);
        }

        internal ReferenceType GetMirrorOf(TypeTag typeTag, ReferenceTypeId typeId)
        {
            switch (typeTag)
            {
            case TypeTag.Class:
                return new ClassType(this, (ClassId)typeId);

            case TypeTag.Interface:
                return new InterfaceType(this, (InterfaceId)typeId);

            case TypeTag.Array:
                return new ArrayType(this, (ArrayTypeId)typeId);

            case TypeTag.Invalid:
            default:
                throw new DebuggerArgumentException("Invalid type tag.");
            }
        }

        internal ObjectReference GetMirrorOf(TaggedObjectId @object)
        {
            return GetMirrorOf(@object.Tag, @object.ObjectId);
        }

        internal ObjectReference GetMirrorOf(Tag tag, ObjectId objectId)
        {
            switch (tag)
            {
            case Tag.Array:
                return new ArrayReference(this, (ArrayId)objectId);

            case Tag.Object:
                return new ObjectReference(this, (ArrayId)objectId);

            case Tag.String:
                return new StringReference(this, (StringId)objectId);

            case Tag.Thread:
                return new ThreadReference(this, (ThreadId)objectId);

            case Tag.ThreadGroup:
                return new ThreadGroupReference(this, (ThreadGroupId)objectId);

            case Tag.ClassLoader:
                return new ClassLoaderReference(this, (ClassLoaderId)objectId);

            case Tag.ClassObject:
                return new ClassObjectReference(this, (ClassObjectId)objectId);

            case Tag.Invalid:
            case Tag.Byte:
            case Tag.Char:
            case Tag.Float:
            case Tag.Double:
            case Tag.Int:
            case Tag.Long:
            case Tag.Short:
            case Tag.Boolean:
            case Tag.Void:
            default:
                throw new ArgumentException();
            }
        }

        internal ReferenceType GetMirrorOf(TaggedReferenceTypeId type)
        {
            return GetMirrorOf(type.TypeTag, type.TypeId);
        }

        internal Value GetMirrorOf(Types.Value value)
        {
            switch (value.Tag)
            {
            case Tag.Array:
                return GetMirrorOf(new ArrayId(value.Data));

            case Tag.Byte:
                return GetMirrorOf((byte)value.Data);

            case Tag.Char:
                return GetMirrorOf((char)value.Data);

            case Tag.Object:
                if (value.Data == 0)
                    return null;

                return new ObjectReference(this, new ObjectId(value.Data));

            case Tag.Float:
                return GetMirrorOf(ValueHelper.Int32BitsToSingle((int)value.Data));

            case Tag.Double:
                return GetMirrorOf(BitConverter.Int64BitsToDouble(value.Data));

            case Tag.Int:
                return GetMirrorOf((int)value.Data);

            case Tag.Long:
                return GetMirrorOf((long)value.Data);

            case Tag.Short:
                return GetMirrorOf((short)value.Data);

            case Tag.Void:
                return GetMirrorOfVoid();

            case Tag.Boolean:
                return GetMirrorOf(value.Data != 0);

            case Tag.String:
                return GetMirrorOf(new StringId(value.Data));

            case Tag.Thread:
                return GetMirrorOf(new ThreadId(value.Data));

            case Tag.ThreadGroup:
                return GetMirrorOf(new ThreadGroupId(value.Data));

            case Tag.ClassLoader:
                return GetMirrorOf(new ClassLoaderId(value.Data));

            case Tag.ClassObject:
                return GetMirrorOf(new ClassObjectId(value.Data));

            case Tag.Invalid:
            default:
                throw new DebuggerArgumentException();
            }
        }

        internal ReferenceType GetMirrorOf(ReferenceTypeData data)
        {
            ReferenceType referenceType = GetMirrorOf(data.ReferenceTypeTag, data.TypeId);
            // todo: fill in some extra info fields for efficiency

            return referenceType;
        }

        internal StackFrame GetMirrorOf(ThreadReference thread, FrameLocationData frame)
        {
            Location location = GetMirrorOf(frame.Location);
            return new StackFrame(this, frame.FrameId, thread, location);
        }

        internal Field GetMirrorOf(ReferenceType declaringType, DeclaredFieldData fieldData)
        {
            return new Field(this, declaringType, fieldData.Name, fieldData.Signature, fieldData.GenericSignature, (AccessModifiers)fieldData.Modifiers, fieldData.FieldId);
        }

        internal Method GetMirrorOf(ReferenceType declaringType, DeclaredMethodData methodData)
        {
            return new Method(this, declaringType, methodData.Name, methodData.Signature, methodData.GenericSignature, (AccessModifiers)methodData.Modifiers, methodData.MethodId);
        }

        internal Method GetMirrorOf(ReferenceType declaringType, MethodId methodId)
        {
            IEnumerable<Method> methods = declaringType.GetMethods(false).Cast<Method>();
            return methods.FirstOrDefault(i => i.MethodId == methodId);
        }

        internal Location GetMirrorOf(Types.Location location)
        {
            ReferenceType type = GetMirrorOf(location.TypeTag, location.Class);
            Method method = GetMirrorOf(type, location.Method);
            long codeIndex = (long)location.Index;
            Location loc = new Location(this, method, codeIndex);

            return loc;
        }

        internal Location GetMirrorOf(Method method, Types.LineNumberData lineNumberData)
        {
            return new Location(this, method, lineNumberData.LineCodeIndex, lineNumberData.LineNumber);
        }

        internal LocalVariable GetMirrorOf(Method method, Types.VariableData variableData)
        {
            ulong codeIndex = variableData.CodeIndex;
            uint length = variableData.Length;

            string name = variableData.Name;
            string signature = variableData.Signature;
            string genericSignature = variableData.GenericSignature;

            int slot = variableData.Slot;

            return new LocalVariable(this, method, variableData);
        }

        IBooleanValue IVirtualMachine.GetMirrorOf(bool value)
        {
            return GetMirrorOf(value);
        }

        IByteValue IVirtualMachine.GetMirrorOf(byte value)
        {
            return GetMirrorOf(value);
        }

        ICharValue IVirtualMachine.GetMirrorOf(char value)
        {
            return GetMirrorOf(value);
        }

        IDoubleValue IVirtualMachine.GetMirrorOf(double value)
        {
            return GetMirrorOf(value);
        }

        IFloatValue IVirtualMachine.GetMirrorOf(float value)
        {
            return GetMirrorOf(value);
        }

        IIntegerValue IVirtualMachine.GetMirrorOf(int value)
        {
            return GetMirrorOf(value);
        }

        ILongValue IVirtualMachine.GetMirrorOf(long value)
        {
            return GetMirrorOf(value);
        }

        IShortValue IVirtualMachine.GetMirrorOf(short value)
        {
            return GetMirrorOf(value);
        }

        IStringReference IVirtualMachine.GetMirrorOf(string value)
        {
            return GetMirrorOf(value);
        }

        IVoidValue IVirtualMachine.GetMirrorOfVoid()
        {
            return GetMirrorOfVoid();
        }

        internal IType FindType(string signature)
        {
            Contract.Requires<ArgumentNullException>(signature != null, "signature");
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(signature));

            switch (signature[0])
            {
            case 'Z':
                return PrimitiveTypes.Boolean;
            case 'B':
                return PrimitiveTypes.Byte;
            case 'C':
                return PrimitiveTypes.Char;
            case 'D':
                return PrimitiveTypes.Double;
            case 'F':
                return PrimitiveTypes.Float;
            case 'I':
                return PrimitiveTypes.Integer;
            case 'J':
                return PrimitiveTypes.Long;
            case 'S':
                return PrimitiveTypes.Short;
            case 'V':
                return PrimitiveTypes.Void;
            //case '[':
            //    return GetArrayType(FindType(signature.Substring(1)));
            //case 'L':
            //    return GetClassesByName(signature).Single();
            default:
                Types.ReferenceTypeData[] classes;
                DebugErrorHandler.ThrowOnFailure(ProtocolService.GetClassesBySignature(out classes, signature));
                if (classes.Length == 1)
                {
                    return GetMirrorOf(classes[0]);
                }
                else if (classes.Length == 0)
                {
                    if (signature[0] == '[')
                        return new UnloadedArrayType(this, signature);
                    else
                        return new UnloadedReferenceType(this, signature);
                }
                else
                {
                    throw new ArgumentException("Could not resolve the signature to a single type.");
                }
            }
        }

        public string GetName()
        {
            throw new NotImplementedException();
        }

        public void RedefineClasses(IEnumerable<KeyValuePair<IReferenceType, byte[]>> classes)
        {
            List<ClassDefinitionData> definitions = new List<ClassDefinitionData>();
            foreach (var pair in classes)
            {
                ReferenceType referenceType = pair.Key as ReferenceType;
                if (referenceType == null || !this.Equals(referenceType.VirtualMachine))
                    throw new VirtualMachineMismatchException();

                definitions.Add(new ClassDefinitionData(referenceType.ReferenceTypeId, pair.Value));
            }

            DebugErrorHandler.ThrowOnFailure(ProtocolService.RedefineClasses(definitions.ToArray()));
        }

        public void Resume()
        {
            if (ProtocolService == null)
                return;

            DebugErrorHandler.ThrowOnFailure(ProtocolService.Resume());
        }

        void IVirtualMachine.SetDebugTraceMode(TraceModes modes)
        {
            throw new NotSupportedException();
        }

        public void SetDefaultStratum(string stratum)
        {
            DebugErrorHandler.ThrowOnFailure(ProtocolService.SetDefaultStratum(stratum));
        }

        public void Suspend()
        {
            // the target will suspend automatically as part of the VMStart event
            if (ProtocolService == null)
                return;

            DebugErrorHandler.ThrowOnFailure(ProtocolService.Suspend());
        }

        public ReadOnlyCollection<IThreadGroupReference> GetTopLevelThreadGroups()
        {
            ThreadGroupId[] groups;
            DebugErrorHandler.ThrowOnFailure(ProtocolService.GetTopLevelThreadGroups(out groups));
            return new ReadOnlyCollection<IThreadGroupReference>(Array.ConvertAll(groups, GetMirrorOf));
        }

        public string GetVersion()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                DebugErrorHandler.ThrowOnFailure(ProtocolService.Dispose());
            }

            _disposed = true;
        }

        #endregion

        #region IMirror Members

        IVirtualMachine IMirror.GetVirtualMachine()
        {
            return this;
        }

        #endregion

        private void OnAttachComplete(EventArgs e)
        {
            var t = AttachComplete;
            if (t != null)
                t(this, e);
        }
    }
}
