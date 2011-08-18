namespace Tvl.Java.DebugHost
{
    using Tvl.Java.DebugHost.Interop;
    using IntPtr = System.IntPtr;
    using jvalue = System.Int64;

    public abstract class JvmEventProcessorBase
    {
        public virtual void HandleSingleStep(JvmEnvironment environment, JvmNativeEnvironment nativeEnvironment, JvmThreadReference thread, JvmLocation location)
        {
        }

        public virtual void HandleBreakpoint(JvmEnvironment environment, JvmNativeEnvironment nativeEnvironment, JvmThreadReference thread, JvmLocation location)
        {
        }

        public virtual void HandleFieldAccess(JvmEnvironment environment, JvmNativeEnvironment nativeEnvironment, JvmThreadReference thread, JvmLocation location, JvmClassReference fieldClass, JvmObjectReference @object, JvmField field)
        {
        }

        public virtual void HandleFieldModification(JvmEnvironment environment, JvmNativeEnvironment nativeEnvironment, JvmThreadReference thread, JvmLocation location, JvmClassReference fieldClass, JvmObjectReference @object, JvmField field, byte signatureType, jvalue newValue)
        {
        }

        public virtual void HandleFramePop(JvmEnvironment environment, JvmNativeEnvironment nativeEnvironment, JvmThreadReference thread, JvmMethod method, bool wasPoppedByException)
        {
        }

        public virtual void HandleMethodEntry(JvmEnvironment environment, JvmNativeEnvironment nativeEnvironment, JvmThreadReference thread, JvmMethod method)
        {
        }

        public virtual void HandleMethodExit(JvmEnvironment environment, JvmNativeEnvironment nativeEnvironment, JvmThreadReference thread, JvmMethod method, bool wasPoppedByException, jvalue returnValue)
        {
        }

        public virtual void HandleNativeMethodBind(JvmEnvironment environment, JvmNativeEnvironment nativeEnvironment, JvmThreadReference thread, JvmMethod method, IntPtr address, ref IntPtr? newAddress)
        {
        }

        public virtual void HandleException(JvmEnvironment environment, JvmNativeEnvironment nativeEnvironment, JvmThreadReference thread, JvmLocation location, JvmObjectReference exception, JvmLocation catchLocation)
        {
        }

        public virtual void HandleExceptionCatch(JvmEnvironment environment, JvmNativeEnvironment nativeEnvironment, JvmThreadReference thread, JvmLocation location, JvmObjectReference exception)
        {
        }

        public virtual void HandleThreadStart(JvmEnvironment environment, JvmNativeEnvironment nativeEnvironment, JvmThreadReference thread)
        {
        }

        public virtual void HandleThreadEnd(JvmEnvironment environment, JvmNativeEnvironment nativeEnvironment, JvmThreadReference thread)
        {
        }

        public virtual void HandleClassLoad(JvmEnvironment environment, JvmNativeEnvironment nativeEnvironment, JvmThreadReference thread, JvmClassReference @class)
        {
        }

        public virtual void HandleClassPrepare(JvmEnvironment environment, JvmNativeEnvironment nativeEnvironment, JvmThreadReference thread, JvmClassReference @class)
        {
        }

        public virtual void HandleClassFileLoadHook(JvmEnvironment environment, JvmNativeEnvironment nativeEnvironment, JvmClassReference classBeingRedefined, JvmObjectReference loader, string name, JvmObjectReference protectionDomain, byte[] classData, ref byte[] newClassData)
        {
        }

        public virtual void HandleVMStart(JvmEnvironment environment, JvmNativeEnvironment nativeEnvironment)
        {
        }

        public virtual void HandleVMInitialization(JvmEnvironment environment, JvmNativeEnvironment nativeEnvironment, JvmThreadReference thread)
        {
        }

        public virtual void HandleVMDeath(JvmEnvironment environment, JvmNativeEnvironment nativeEnvironment)
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

        public virtual void HandleMonitorContendedEnter(JvmEnvironment environment, JvmNativeEnvironment nativeEnvironment, JvmThreadReference thread, JvmObjectReference @object)
        {
        }

        public virtual void HandleMonitorContendedEntered(JvmEnvironment environment, JvmNativeEnvironment nativeEnvironment, JvmThreadReference thread, JvmObjectReference @object)
        {
        }

        public virtual void HandleMonitorWait(JvmEnvironment environment, JvmNativeEnvironment nativeEnvironment, JvmThreadReference thread, JvmObjectReference @object, long millisecondsTimeout)
        {
        }

        public virtual void HandleMonitorWaited(JvmEnvironment environment, JvmNativeEnvironment nativeEnvironment, JvmThreadReference thread, JvmObjectReference @object, bool timedOut)
        {
        }

        public virtual void HandleResourceExhausted(JvmEnvironment environment, JvmNativeEnvironment nativeEnvironment, JvmResourceExhaustedFlags flags, IntPtr reserved, string description)
        {
        }

        public virtual void HandleVMObjectAllocation(JvmEnvironment environment, JvmNativeEnvironment nativeEnvironment, JvmThreadReference thread, JvmObjectReference @object, JvmClassReference objectClass, long size)
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
