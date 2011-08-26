namespace Tvl.Java.DebugHost
{
    using Tvl.Java.DebugHost.Interop;
    using IntPtr = System.IntPtr;

    public abstract class JvmEventProcessorBase
    {
        public virtual void HandleSingleStep(JvmEnvironment environment, JvmThreadReference thread, JvmLocation location)
        {
        }

        public virtual void HandleBreakpoint(JvmEnvironment environment, JvmThreadReference thread, JvmLocation location)
        {
        }

        public virtual void HandleFieldAccess(JvmEnvironment environment, JvmThreadReference thread, JvmLocation location, JvmClassReference fieldClass, JvmObjectReference @object, JvmField field)
        {
        }

        public virtual void HandleFieldModification(JvmEnvironment environment, JvmThreadReference thread, JvmLocation location, JvmClassReference fieldClass, JvmObjectReference @object, JvmField field, byte signatureType, jvalue newValue)
        {
        }

        public virtual void HandleFramePop(JvmEnvironment environment, JvmThreadReference thread, JvmMethod method, bool wasPoppedByException)
        {
        }

        public virtual void HandleMethodEntry(JvmEnvironment environment, JvmThreadReference thread, JvmMethod method)
        {
        }

        public virtual void HandleMethodExit(JvmEnvironment environment, JvmThreadReference thread, JvmMethod method, bool wasPoppedByException, jvalue returnValue)
        {
        }

        public virtual void HandleNativeMethodBind(JvmEnvironment environment, JvmThreadReference thread, JvmMethod method, IntPtr address, ref IntPtr? newAddress)
        {
        }

        public virtual void HandleException(JvmEnvironment environment, JvmThreadReference thread, JvmLocation location, JvmObjectReference exception, JvmLocation catchLocation)
        {
        }

        public virtual void HandleExceptionCatch(JvmEnvironment environment, JvmThreadReference thread, JvmLocation location, JvmObjectReference exception)
        {
        }

        public virtual void HandleThreadStart(JvmEnvironment environment, JvmThreadReference thread)
        {
        }

        public virtual void HandleThreadEnd(JvmEnvironment environment, JvmThreadReference thread)
        {
        }

        public virtual void HandleClassLoad(JvmEnvironment environment, JvmThreadReference thread, JvmClassReference @class)
        {
        }

        public virtual void HandleClassPrepare(JvmEnvironment environment, JvmThreadReference thread, JvmClassReference @class)
        {
        }

        public virtual void HandleClassFileLoadHook(JvmEnvironment environment, JvmClassReference classBeingRedefined, JvmObjectReference loader, string name, JvmObjectReference protectionDomain/*, byte[] classData, ref byte[] newClassData*/)
        {
        }

        public virtual void HandleVMStart(JvmEnvironment environment)
        {
        }

        public virtual void HandleVMInitialization(JvmEnvironment environment, JvmThreadReference thread)
        {
        }

        public virtual void HandleVMDeath(JvmEnvironment environment)
        {
        }

        public virtual void HandleCompiledMethodLoad(JvmEnvironment environment, JvmMethod method, int codeSize, IntPtr codeAddress, JvmAddressLocationMap[] map, IntPtr compileInfo)
        {
        }

        public virtual void HandleCompiledMethodUnload(JvmEnvironment environment, JvmMethod method, IntPtr codeAddress)
        {
        }

        public virtual void HandleDynamicCodeGenerated(JvmEnvironment environment, string name, IntPtr address, int length)
        {
        }

        public virtual void HandleDataDumpRequest(JvmEnvironment environment)
        {
        }

        public virtual void HandleMonitorContendedEnter(JvmEnvironment environment, JvmThreadReference thread, JvmObjectReference @object)
        {
        }

        public virtual void HandleMonitorContendedEntered(JvmEnvironment environment, JvmThreadReference thread, JvmObjectReference @object)
        {
        }

        public virtual void HandleMonitorWait(JvmEnvironment environment, JvmThreadReference thread, JvmObjectReference @object, long millisecondsTimeout)
        {
        }

        public virtual void HandleMonitorWaited(JvmEnvironment environment, JvmThreadReference thread, JvmObjectReference @object, bool timedOut)
        {
        }

        public virtual void HandleResourceExhausted(JvmEnvironment environment, JvmResourceExhaustedFlags flags, IntPtr reserved, string description)
        {
        }

        public virtual void HandleVMObjectAllocation(JvmEnvironment environment, JvmThreadReference thread, JvmObjectReference @object, JvmClassReference objectClass, long size)
        {
        }

        public virtual void HandleObjectFree(JvmEnvironment environment, long tag)
        {
        }

        public virtual void HandleGarbageCollectionStart(JvmEnvironment environment)
        {
        }

        public virtual void HandleGarbageCollectionFinish(JvmEnvironment environment)
        {
        }
    }
}
