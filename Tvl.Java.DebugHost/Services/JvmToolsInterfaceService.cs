namespace Tvl.Java.DebugHost.Services
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using Tvl.Java.DebugHost.Interop;

    [ServiceBehavior(IncludeExceptionDetailInFaults = true, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class JvmToolsInterfaceService : IJvmToolsInterfaceService
    {
        public jvmtiError Allocate(JvmVirtualMachineRemoteHandle virtualMachine, long size, out long address)
        {
            JavaVM machine = JavaVM.GetInstance(virtualMachine);

            jvmtiError result = jvmtiError.Internal;
            IntPtr memory = IntPtr.Zero;
            machine.InvokeOnJvmThread(
                (environment) =>
                {
                    jvmtiInterface rawInterface = environment.RawInterface;
                    result = rawInterface.Allocate(environment.Handle, size, out memory);
                });

            address = memory.ToInt64();
            return result;
        }

        public jvmtiError Deallocate(JvmVirtualMachineRemoteHandle virtualMachine, long address)
        {
            JavaVM machine = JavaVM.GetInstance(virtualMachine);

            jvmtiError result = jvmtiError.Internal;
            machine.InvokeOnJvmThread(
                (environment) =>
                {
                    jvmtiInterface rawInterface = environment.RawInterface;
                    result = rawInterface.Deallocate(environment.Handle, (IntPtr)address);
                });

            return result;
        }

        public jvmtiError GetThreadState(JvmVirtualMachineRemoteHandle virtualMachine, JvmThreadRemoteHandle thread, out jvmtiThreadState threadState)
        {
            JavaVM machine = JavaVM.GetInstance(virtualMachine);

            jvmtiThreadState threadStateResult = jvmtiThreadState.None;
            jvmtiError result = jvmtiError.Internal;
            machine.InvokeOnJvmThread(
                (environment) =>
                {
                    jvmtiInterface rawInterface = environment.RawInterface;
                    result = rawInterface.GetThreadState(environment.Handle, thread, out threadStateResult);
                });

            threadState = threadStateResult;
            return result;
        }

        public jvmtiError GetCurrentThread(JvmVirtualMachineRemoteHandle virtualMachine, out JvmThreadRemoteHandle threadHandle)
        {
            JavaVM machine = JavaVM.GetInstance(virtualMachine);

            jthread thread = jthread.Null;
            jvmtiError result = jvmtiError.Internal;
            machine.InvokeOnJvmThread(
                (environment) =>
                {
                    jvmtiInterface rawInterface = environment.RawInterface;
                    result = rawInterface.GetCurrentThread(environment.Handle, out thread);
                });

            threadHandle = new JvmThreadRemoteHandle(thread);
            return result;
        }

        public jvmtiError GetAllThreads(JvmVirtualMachineRemoteHandle virtualMachine, out JvmThreadRemoteHandle[] threads)
        {
            throw new NotImplementedException();
        }

        public jvmtiError SuspendThread(JvmVirtualMachineRemoteHandle virtualMachine, JvmThreadRemoteHandle thread)
        {
            throw new NotImplementedException();
        }

        public jvmtiError SuspendThreads(JvmVirtualMachineRemoteHandle virtualMachine, JvmThreadRemoteHandle[] threads)
        {
            throw new NotImplementedException();
        }

        public jvmtiError ResumeThread(JvmVirtualMachineRemoteHandle virtualMachine, JvmThreadRemoteHandle thread)
        {
            throw new NotImplementedException();
        }

        public jvmtiError ResumeThreads(JvmVirtualMachineRemoteHandle virtualMachine, JvmThreadRemoteHandle threads)
        {
            throw new NotImplementedException();
        }

        public jvmtiError StopThread(JvmVirtualMachineRemoteHandle virtualMachine, JvmThreadRemoteHandle thread, JvmObjectRemoteHandle exception)
        {
            throw new NotImplementedException();
        }

        public jvmtiError InterruptThread(JvmVirtualMachineRemoteHandle virtualMachine, JvmThreadRemoteHandle thread)
        {
            throw new NotImplementedException();
        }

        public jvmtiError GetThreadInfo(JvmVirtualMachineRemoteHandle virtualMachine, JvmThreadRemoteHandle thread, out JvmThreadRemoteInfo info)
        {
            JavaVM machine = JavaVM.GetInstance(virtualMachine);

            jvmtiThreadInfo threadInfo = default(jvmtiThreadInfo);
            jvmtiError result = jvmtiError.Internal;

            machine.InvokeOnJvmThread(
                (environment) =>
                {
                    jvmtiInterface rawInterface = environment.RawInterface;
                    result = rawInterface.GetThreadInfo(environment.Handle, thread, out threadInfo);
                });

            info = new JvmThreadRemoteInfo()
                {
                    Name = threadInfo.Name,
                    Priority = threadInfo._priority,
                    IsDaemon = threadInfo._isDaemon != 0,
                    ContextClassLoader = new JvmObjectRemoteHandle(threadInfo._contextClassLoader),
                    ThreadGroup = new JvmThreadGroupRemoteHandle(threadInfo._threadGroup)
                };

            return result;
        }

        public jvmtiError GetOwnedMonitorInfo(JvmVirtualMachineRemoteHandle virtualMachine, JvmThreadRemoteHandle thread, out JvmObjectRemoteHandle[] ownedMonitors)
        {
            throw new NotImplementedException();
        }

        public jvmtiError GetCurrentContendedMonitor(JvmVirtualMachineRemoteHandle virtualMachine, JvmThreadRemoteHandle thread, out JvmObjectRemoteHandle monitor)
        {
            throw new NotImplementedException();
        }

        public jvmtiError SetThreadLocalStorage(JvmVirtualMachineRemoteHandle virtualMachine, JvmThreadRemoteHandle thread, long data)
        {
            throw new NotImplementedException();
        }

        public jvmtiError GetThreadLocalStorage(JvmVirtualMachineRemoteHandle virtualMachine, JvmThreadRemoteHandle thread, out long data)
        {
            throw new NotImplementedException();
        }

        public jvmtiError GetTopThreadGroups(JvmVirtualMachineRemoteHandle virtualMachine, out JvmThreadGroupRemoteHandle[] groups)
        {
            throw new NotImplementedException();
        }

        public jvmtiError GetThreadGroupChildren(JvmVirtualMachineRemoteHandle virtualMachine, JvmThreadGroupRemoteHandle group, out JvmThreadRemoteHandle[] threads, out JvmThreadGroupRemoteHandle[] groups)
        {
            throw new NotImplementedException();
        }

        public jvmtiError GetStackTrace(JvmVirtualMachineRemoteHandle virtualMachine, JvmThreadRemoteHandle thread, int startDepth, int maxFrameCount, out JvmRemoteLocation[] frames)
        {
            throw new NotImplementedException();
        }

        public jvmtiError GetAllStackTraces(JvmVirtualMachineRemoteHandle virtualMachine, int maxFrameCount, out JvmRemoteStackInfo[] stackTraces)
        {
            throw new NotImplementedException();
        }

        public jvmtiError GetThreadListStackTraces(JvmVirtualMachineRemoteHandle virtualMachine, JvmThreadRemoteHandle[] threads, int maxFrameCount, out JvmRemoteStackInfo[] stackTraces)
        {
            throw new NotImplementedException();
        }

        public jvmtiError GetFrameCount(JvmVirtualMachineRemoteHandle virtualMachine, JvmThreadRemoteHandle thread, out int frameCount)
        {
            throw new NotImplementedException();
        }

        public jvmtiError PopFrame(JvmVirtualMachineRemoteHandle virtualMachine, JvmThreadRemoteHandle thread)
        {
            throw new NotImplementedException();
        }

        public jvmtiError GetFrameLocation(JvmVirtualMachineRemoteHandle virtualMachine, JvmThreadRemoteHandle thread, int depth, out JvmRemoteLocation location)
        {
            throw new NotImplementedException();
        }

        public jvmtiError NotifyFramePop(JvmVirtualMachineRemoteHandle virtualMachine, JvmThreadRemoteHandle thread, int depth)
        {
            throw new NotImplementedException();
        }

        public jvmtiError ForceEarlyReturnObject(JvmVirtualMachineRemoteHandle virtualMachine, JvmThreadRemoteHandle thread, JvmObjectRemoteHandle value)
        {
            throw new NotImplementedException();
        }

        public jvmtiError ForceEarlyReturnInt(JvmVirtualMachineRemoteHandle virtualMachine, JvmThreadRemoteHandle thread, int value)
        {
            throw new NotImplementedException();
        }

        public jvmtiError ForceEarlyReturnLong(JvmVirtualMachineRemoteHandle virtualMachine, JvmThreadRemoteHandle thread, long value)
        {
            throw new NotImplementedException();
        }

        public jvmtiError ForceEarlyReturnFloat(JvmVirtualMachineRemoteHandle virtualMachine, JvmThreadRemoteHandle thread, float value)
        {
            throw new NotImplementedException();
        }

        public jvmtiError ForceEarlyReturnDouble(JvmVirtualMachineRemoteHandle virtualMachine, JvmThreadRemoteHandle thread, double value)
        {
            throw new NotImplementedException();
        }

        public jvmtiError ForceEarlyReturnVoid(JvmVirtualMachineRemoteHandle virtualMachine, JvmThreadRemoteHandle thread)
        {
            throw new NotImplementedException();
        }

        public jvmtiError GetTag(JvmVirtualMachineRemoteHandle virtualMachine, JvmObjectRemoteHandle @object, out long tag)
        {
            JavaVM machine = JavaVM.GetInstance(virtualMachine);

            long tagResult = 0;
            jvmtiError result = jvmtiError.Internal;

            machine.InvokeOnJvmThread(
                (environment) =>
                {
                    jvmtiInterface rawInterface = environment.RawInterface;
                    result = rawInterface.GetTag(environment.Handle, @object, out tagResult);
                });

            tag = tagResult;
            return result;
        }

        public jvmtiError SetTag(JvmVirtualMachineRemoteHandle virtualMachine, JvmObjectRemoteHandle @object, long tag)
        {
            JavaVM machine = JavaVM.GetInstance(virtualMachine);

            jvmtiError result = jvmtiError.Internal;

            machine.InvokeOnJvmThread(
                (environment) =>
                {
                    jvmtiInterface rawInterface = environment.RawInterface;
                    result = rawInterface.SetTag(environment.Handle, @object, tag);
                });

            return result;
        }

        public jvmtiError GetObjectsWithTags(JvmVirtualMachineRemoteHandle virtualMachine, long[] tagFilter, out JvmObjectRemoteHandle[] objects, out long[] tags)
        {
            throw new NotImplementedException();
        }

        public jvmtiError ForceGarbageCollection(JvmVirtualMachineRemoteHandle virtualMachine)
        {
            throw new NotImplementedException();
        }

        public jvmtiError GetLocalObject(JvmVirtualMachineRemoteHandle virtualMachine, JvmThreadRemoteHandle thread, int depth, int slot, out JvmObjectRemoteHandle value)
        {
            throw new NotImplementedException();
        }

        public jvmtiError GetLocalInstance(JvmVirtualMachineRemoteHandle virtualMachine, JvmThreadRemoteHandle thread, int depth, out JvmObjectRemoteHandle value)
        {
            throw new NotImplementedException();
        }

        public jvmtiError GetLocalInt(JvmVirtualMachineRemoteHandle virtualMachine, JvmThreadRemoteHandle thread, int depth, int slot, out int value)
        {
            throw new NotImplementedException();
        }

        public jvmtiError GetLocalLong(JvmVirtualMachineRemoteHandle virtualMachine, JvmThreadRemoteHandle thread, int depth, int slot, out long value)
        {
            throw new NotImplementedException();
        }

        public jvmtiError GetLocalFloat(JvmVirtualMachineRemoteHandle virtualMachine, JvmThreadRemoteHandle thread, int depth, int slot, out float value)
        {
            throw new NotImplementedException();
        }

        public jvmtiError GetLocalDouble(JvmVirtualMachineRemoteHandle virtualMachine, JvmThreadRemoteHandle thread, int depth, int slot, out double value)
        {
            throw new NotImplementedException();
        }

        public jvmtiError SetLocalObject(JvmVirtualMachineRemoteHandle virtualMachine, JvmThreadRemoteHandle thread, int depth, int slot, JvmObjectRemoteHandle value)
        {
            throw new NotImplementedException();
        }

        public jvmtiError SetLocalInt(JvmVirtualMachineRemoteHandle virtualMachine, JvmThreadRemoteHandle thread, int depth, int slot, int value)
        {
            throw new NotImplementedException();
        }

        public jvmtiError SetLocalLong(JvmVirtualMachineRemoteHandle virtualMachine, JvmThreadRemoteHandle thread, int depth, int slot, long value)
        {
            throw new NotImplementedException();
        }

        public jvmtiError SetLocalFloat(JvmVirtualMachineRemoteHandle virtualMachine, JvmThreadRemoteHandle thread, int depth, int slot, float value)
        {
            throw new NotImplementedException();
        }

        public jvmtiError SetLocalDouble(JvmVirtualMachineRemoteHandle virtualMachine, JvmThreadRemoteHandle thread, int depth, int slot, double value)
        {
            throw new NotImplementedException();
        }

        public jvmtiError SetBreakpoint(JvmVirtualMachineRemoteHandle virtualMachine, JvmRemoteLocation location)
        {
            throw new NotImplementedException();
        }

        public jvmtiError ClearBreakpoint(JvmVirtualMachineRemoteHandle virtualMachine, JvmRemoteLocation location)
        {
            throw new NotImplementedException();
        }

        public jvmtiError SetFieldAccessWatch(JvmVirtualMachineRemoteHandle virtualMachine, JvmFieldRemoteHandle field)
        {
            throw new NotImplementedException();
        }

        public jvmtiError ClearFieldAccessWatch(JvmVirtualMachineRemoteHandle virtualMachine, JvmFieldRemoteHandle field)
        {
            throw new NotImplementedException();
        }

        public jvmtiError SetFieldModificationWatch(JvmVirtualMachineRemoteHandle virtualMachine, JvmFieldRemoteHandle field)
        {
            throw new NotImplementedException();
        }

        public jvmtiError ClearFieldModificationWatch(JvmVirtualMachineRemoteHandle virtualMachine, JvmFieldRemoteHandle field)
        {
            throw new NotImplementedException();
        }

        public jvmtiError GetLoadedClasses(JvmVirtualMachineRemoteHandle virtualMachine, out JvmClassRemoteHandle[] classes)
        {
            throw new NotImplementedException();
        }

        public jvmtiError GetClassLoaderClasses(JvmVirtualMachineRemoteHandle virtualMachine, JvmObjectRemoteHandle initiatingLoader, out JvmClassRemoteHandle[] classes)
        {
            throw new NotImplementedException();
        }

        public jvmtiError GetClassSignature(JvmVirtualMachineRemoteHandle virtualMachine, JvmClassRemoteHandle @class, out string signature, out string generic)
        {
            JavaVM machine = JavaVM.GetInstance(virtualMachine);

            string signatureResult = null;
            string genericResult = null;
            jvmtiError result = jvmtiError.Internal;

            machine.InvokeOnJvmThread(
                (environment) =>
                {
                    jvmtiInterface rawInterface = environment.RawInterface;

                    IntPtr signaturePtr = IntPtr.Zero;
                    IntPtr genericPtr = IntPtr.Zero;
                    try
                    {
                        result = rawInterface.GetClassSignature(environment.Handle, @class, out signaturePtr, out genericPtr);

                        unsafe
                        {
                            if (signaturePtr != IntPtr.Zero)
                                signatureResult = ModifiedUTF8Encoding.GetString((byte*)signaturePtr);
                            if (genericPtr != IntPtr.Zero)
                                genericResult = ModifiedUTF8Encoding.GetString((byte*)genericPtr);
                        }
                    }
                    finally
                    {
                        rawInterface.Deallocate(environment.Handle, signaturePtr);
                        rawInterface.Deallocate(environment.Handle, genericPtr);
                    }
                });

            signature = signatureResult;
            generic = genericResult;
            return result;
        }

        public jvmtiError GetClassStatus(JvmVirtualMachineRemoteHandle virtualMachine, JvmClassRemoteHandle @class, out jvmtiClassStatus status)
        {
            throw new NotImplementedException();
        }

        public jvmtiError GetSourceFileName(JvmVirtualMachineRemoteHandle virtualMachine, JvmClassRemoteHandle @class, out string sourceName)
        {
            JavaVM machine = JavaVM.GetInstance(virtualMachine);

            string sourceNameResult = null;
            jvmtiError result = jvmtiError.Internal;

            machine.InvokeOnJvmThread(
                (environment) =>
                {
                    jvmtiInterface rawInterface = environment.RawInterface;

                    IntPtr sourceNamePtr = IntPtr.Zero;
                    try
                    {
                        result = rawInterface.GetSourceFileName(environment.Handle, @class, out sourceNamePtr);

                        unsafe
                        {
                            if (sourceNamePtr != IntPtr.Zero)
                                sourceNameResult = ModifiedUTF8Encoding.GetString((byte*)sourceNamePtr);
                        }
                    }
                    finally
                    {
                        rawInterface.Deallocate(environment.Handle, sourceNamePtr);
                    }
                });

            sourceName = sourceNameResult;
            return result;
        }

        public jvmtiError GetClassModifiers(JvmVirtualMachineRemoteHandle virtualMachine, JvmClassRemoteHandle @class, out JvmAccessModifiers modifiers)
        {
            throw new NotImplementedException();
        }

        public jvmtiError GetClassMethods(JvmVirtualMachineRemoteHandle virtualMachine, JvmClassRemoteHandle @class, out JvmMethodRemoteHandle[] methods)
        {
            JavaVM machine = JavaVM.GetInstance(virtualMachine);

            List<JvmMethodRemoteHandle> methodsList = new List<JvmMethodRemoteHandle>();
            jvmtiError result = jvmtiError.Internal;

            machine.InvokeOnJvmThread(
                (environment) =>
                {
                    jvmtiInterface rawInterface = environment.RawInterface;

                    IntPtr methodsPtr = IntPtr.Zero;
                    try
                    {
                        int methodCount;
                        result = rawInterface.GetClassMethods(environment.Handle, @class, out methodCount, out methodsPtr);

                        unsafe
                        {
                            jmethodID* rawMethods = (jmethodID*)methodsPtr;
                            for (int i = 0; i < methodCount; i++)
                                methodsList.Add(new JvmMethodRemoteHandle(rawMethods[i]));
                        }
                    }
                    finally
                    {
                        rawInterface.Deallocate(environment.Handle, methodsPtr);
                    }
                });

            methods = methodsList.ToArray();
            return result;
        }

        public jvmtiError GetClassFields(JvmVirtualMachineRemoteHandle virtualMachine, JvmClassRemoteHandle @class, out JvmFieldRemoteHandle[] fields)
        {
            throw new NotImplementedException();
        }

        public jvmtiError GetImplementedInterfaces(JvmVirtualMachineRemoteHandle virtualMachine, JvmClassRemoteHandle @class, out JvmClassRemoteHandle[] interfaces)
        {
            throw new NotImplementedException();
        }

        public jvmtiError GetClassVersionNumbers(JvmVirtualMachineRemoteHandle virtualMachine, JvmClassRemoteHandle @class, out int minorVersion, out int majorVersion)
        {
            throw new NotImplementedException();
        }

        public jvmtiError GetConstantPool(JvmVirtualMachineRemoteHandle virtualMachine, JvmClassRemoteHandle @class, out byte[] constantPool)
        {
            throw new NotImplementedException();
        }

        public jvmtiError IsInterface(JvmVirtualMachineRemoteHandle virtualMachine, JvmClassRemoteHandle @class, out bool isInterface)
        {
            throw new NotImplementedException();
        }

        public jvmtiError IsArrayClass(JvmVirtualMachineRemoteHandle virtualMachine, JvmClassRemoteHandle @class, out bool isArrayClass)
        {
            throw new NotImplementedException();
        }

        public jvmtiError IsModifiableClass(JvmVirtualMachineRemoteHandle virtualMachine, JvmClassRemoteHandle @class, out bool isModifiableClass)
        {
            throw new NotImplementedException();
        }

        public jvmtiError GetClassLoader(JvmVirtualMachineRemoteHandle virtualMachine, JvmClassRemoteHandle @class, out JvmObjectRemoteHandle classLoader)
        {
            throw new NotImplementedException();
        }

        public jvmtiError GetSourceDebugExtension(JvmVirtualMachineRemoteHandle virtualMachine, JvmClassRemoteHandle @class, out string sourceDebugExtension)
        {
            throw new NotImplementedException();
        }

        public jvmtiError RetransformClasses(JvmVirtualMachineRemoteHandle virtualMachine, JvmClassRemoteHandle[] classes)
        {
            throw new NotImplementedException();
        }

        public jvmtiError GetObjectSize(JvmVirtualMachineRemoteHandle virtualMachine, JvmObjectRemoteHandle @object, out long size)
        {
            JavaVM machine = JavaVM.GetInstance(virtualMachine);

            long sizeResult = 0;
            jvmtiError result = jvmtiError.Internal;

            machine.InvokeOnJvmThread(
                (environment) =>
                {
                    jvmtiInterface rawInterface = environment.RawInterface;
                    result = rawInterface.GetObjectSize(environment.Handle, @object, out sizeResult);
                });

            size = sizeResult;
            return result;
        }

        public jvmtiError GetObjectHashCode(JvmVirtualMachineRemoteHandle virtualMachine, JvmObjectRemoteHandle @object, out int hashCode)
        {
            JavaVM machine = JavaVM.GetInstance(virtualMachine);

            int hashCodeResult = 0;
            jvmtiError result = jvmtiError.Internal;

            machine.InvokeOnJvmThread(
                (environment) =>
                {
                    jvmtiInterface rawInterface = environment.RawInterface;
                    result = rawInterface.GetObjectHashCode(environment.Handle, @object, out hashCodeResult);
                });

            hashCode = hashCodeResult;
            return result;
        }

        public jvmtiError GetFieldName(JvmVirtualMachineRemoteHandle virtualMachine, JvmFieldRemoteHandle field, out string name, out string signature, out string generic)
        {
            throw new NotImplementedException();
        }

        public jvmtiError GetFieldDeclaringClass(JvmVirtualMachineRemoteHandle virtualMachine, JvmFieldRemoteHandle field, out JvmClassRemoteHandle declaringClass)
        {
            throw new NotImplementedException();
        }

        public jvmtiError GetFieldModifiers(JvmVirtualMachineRemoteHandle virtualMachine, JvmFieldRemoteHandle field, out JvmAccessModifiers modifiers)
        {
            throw new NotImplementedException();
        }

        public jvmtiError IsFieldSynthetic(JvmVirtualMachineRemoteHandle virtualMachine, JvmFieldRemoteHandle field, out bool isSynthetic)
        {
            throw new NotImplementedException();
        }

        public jvmtiError GetMethodName(JvmVirtualMachineRemoteHandle virtualMachine, JvmMethodRemoteHandle method, out string name, out string signature, out string generic)
        {
            throw new NotImplementedException();
        }

        public jvmtiError GetMethodDeclaringClass(JvmVirtualMachineRemoteHandle virtualMachine, JvmMethodRemoteHandle method, out JvmClassRemoteHandle declaringClass)
        {
            throw new NotImplementedException();
        }

        public jvmtiError GetMethodModifiers(JvmVirtualMachineRemoteHandle virtualMachine, JvmMethodRemoteHandle method, out JvmAccessModifiers modifiers)
        {
            throw new NotImplementedException();
        }

        public jvmtiError GetMaxLocals(JvmVirtualMachineRemoteHandle virtualMachine, JvmMethodRemoteHandle method, out int maxLocals)
        {
            throw new NotImplementedException();
        }

        public jvmtiError GetArgumentsSize(JvmVirtualMachineRemoteHandle virtualMachine, JvmMethodRemoteHandle method, out int size)
        {
            throw new NotImplementedException();
        }

        public jvmtiError GetLineNumberTable(JvmVirtualMachineRemoteHandle virtualMachine, JvmMethodRemoteHandle method, out JvmLineNumberEntry[] lineNumbers)
        {
            JavaVM machine = JavaVM.GetInstance(virtualMachine);

            List<JvmLineNumberEntry> lineNumbersList = new List<JvmLineNumberEntry>();
            jvmtiError result = jvmtiError.Internal;

            machine.InvokeOnJvmThread(
                (environment) =>
                {
                    jvmtiInterface rawInterface = environment.RawInterface;

                    IntPtr lineNumbersPtr = IntPtr.Zero;
                    try
                    {
                        int entryCount;
                        result = rawInterface.GetLineNumberTable(environment.Handle, (jmethodID)method, out entryCount, out lineNumbersPtr);

                        unsafe
                        {
                            jvmtiLineNumberEntry* rawLineNumbers = (jvmtiLineNumberEntry*)lineNumbersPtr;
                            for (int i = 0; i < entryCount; i++)
                                lineNumbersList.Add(new JvmLineNumberEntry(method, rawLineNumbers[i]));
                        }
                    }
                    finally
                    {
                        rawInterface.Deallocate(environment.Handle, lineNumbersPtr);
                    }
                });

            lineNumbers = lineNumbersList.ToArray();
            return result;
        }

        public jvmtiError GetMethodLocation(JvmVirtualMachineRemoteHandle virtualMachine, JvmMethodRemoteHandle method, out JvmRemoteLocation startLocation, out JvmRemoteLocation endLocation)
        {
            throw new NotImplementedException();
        }

        public jvmtiError GetLocalVariableTable(JvmVirtualMachineRemoteHandle virtualMachine, JvmMethodRemoteHandle method, out JvmLocalVariableEntry[] localVariables)
        {
            throw new NotImplementedException();
        }

        public jvmtiError GetBytecodes(JvmVirtualMachineRemoteHandle virtualMachine, JvmMethodRemoteHandle method, out byte[] bytecode)
        {
            throw new NotImplementedException();
        }

        public jvmtiError IsMethodNative(JvmVirtualMachineRemoteHandle virtualMachine, JvmMethodRemoteHandle method, out bool isNative)
        {
            throw new NotImplementedException();
        }

        public jvmtiError IsMethodSynthetic(JvmVirtualMachineRemoteHandle virtualMachine, JvmMethodRemoteHandle method, out bool isSynthetic)
        {
            throw new NotImplementedException();
        }

        public jvmtiError IsMethodObsolete(JvmVirtualMachineRemoteHandle virtualMachine, JvmMethodRemoteHandle method, out bool isObsolete)
        {
            throw new NotImplementedException();
        }

        public jvmtiError SetNativeMethodPrefix(JvmVirtualMachineRemoteHandle virtualMachine, string prefix)
        {
            throw new NotImplementedException();
        }

        public jvmtiError SetNativeMethodPrefixes(JvmVirtualMachineRemoteHandle virtualMachine, string[] prefixes)
        {
            throw new NotImplementedException();
        }
    }
}
