namespace Tvl.Java.DebugHost.Interop
{
    using DispatcherOperation = System.Windows.Threading.DispatcherOperation;
    using Thread = System.Threading.Thread;
    using System.Collections.Concurrent;
    using System.Runtime.InteropServices;
    using Tvl.Java.DebugHost.Services;
    using IntPtr = System.IntPtr;
    using Dispatcher = System.Windows.Threading.Dispatcher;
    using System;
    using DispatcherFrame = System.Windows.Threading.DispatcherFrame;
    using System.Diagnostics.Contracts;
    using DispatcherPriority = System.Windows.Threading.DispatcherPriority;
    using System.Collections.Generic;

    public class JavaVM
    {
        private static readonly ConcurrentDictionary<IntPtr, JavaVM> _instances =
            new ConcurrentDictionary<IntPtr, JavaVM>();

        private readonly IntPtr _jniInvokeInterface;
        private readonly List<Tuple<Dispatcher, DispatcherFrame, JvmEnvironment, IAsyncResult>> _dispatchers =
            new List<Tuple<Dispatcher, DispatcherFrame, JvmEnvironment, IAsyncResult>>();

        private JavaVM(IntPtr jniInterfacePointer)
        {
            _jniInvokeInterface = jniInterfacePointer;
        }

        public static implicit operator JvmVirtualMachineRemoteHandle(JavaVM virtualMachine)
        {
            return new JvmVirtualMachineRemoteHandle(virtualMachine._jniInvokeInterface);
        }

        public static JavaVM GetInstance(JvmVirtualMachineRemoteHandle virtualMachineHandle)
        {
            JavaVM vm = _instances[(IntPtr)virtualMachineHandle.Handle];

            //JNIEnvHandle env;
            //JavaVMAttachArgs args = new JavaVMAttachArgs(jniVersion.Version1_6, IntPtr.Zero, jthreadGroup.Null);
            //int result = vm.GetRawInterface().AttachCurrentThread(vm, out env, ref args);

            //JvmEnvironment environment = vm.GetEnvironment(jvmtiVersion.Version1_1);
            //if (environment != null)
            //{
            //    jvmtiPhase phase = environment.GetPhase();
            //    if (phase == jvmtiPhase.Live)
            //    {
            //    }
            //}

            //if (attachCurrentThread)
            //{
            //    JNIEnvHandle env;
            //    JavaVMAttachArgs args = new JavaVMAttachArgs(jvmtiVersion.Version1_1, IntPtr.Zero, jthreadGroup.Null);
            //    vm.GetRawInterface().AttachCurrentThread(vm, out env, ref args);
            //}

            return vm;
        }

        internal static JavaVM GetOrCreateInstance(IntPtr jniInterfacePointer)
        {
            return _instances.GetOrAdd(jniInterfacePointer, CreateVirtualMachine);
        }

        private static JavaVM CreateVirtualMachine(IntPtr jniInterfacePointer)
        {
            return new JavaVM(jniInterfacePointer);
        }

        public void ShutdownAgentDispatchers()
        {
            List<Dispatcher> dispatchers = new List<Dispatcher>();
            List<DispatcherFrame> frames = new List<DispatcherFrame>();

            lock (_dispatchers)
            {
                for (int i = _dispatchers.Count - 1; i >= 0; i--)
                {
                    if (_dispatchers[i].Item4 != null)
                        continue;

                    dispatchers.Add(_dispatchers[i].Item1);
                    frames.Add(_dispatchers[i].Item2);
                    _dispatchers.RemoveAt(i);
                }
            }

            for (int i = 0; i < dispatchers.Count; i++)
            {
                // queue a low priority operation to end processing
                var frame = frames[i];
                dispatchers[i].BeginInvoke((Action)(() => frame.Continue = false), DispatcherPriority.Background);
            }
        }

        public void PushDispatcherFrame(DispatcherFrame frame, JvmEnvironment environment, IAsyncResult asyncResult)
        {
            Contract.Requires<ArgumentNullException>(frame != null, "frame");
            Contract.Requires<ArgumentNullException>(environment != null, "environment");
            Contract.Requires<ArgumentNullException>(asyncResult != null, "asyncResult");

            lock (_dispatchers)
            {
                _dispatchers.Add(Tuple.Create(Dispatcher.CurrentDispatcher, frame, environment, asyncResult));
            }

            Dispatcher.PushFrame(frame);
        }

        public void PushAgentDispatcherFrame(DispatcherFrame frame, JvmEnvironment environment)
        {
            Contract.Requires<ArgumentNullException>(frame != null, "frame");
            Contract.Requires<ArgumentNullException>(environment != null, "environment");

            lock (_dispatchers)
            {
                _dispatchers.Add(Tuple.Create(Dispatcher.CurrentDispatcher, frame, environment, default(IAsyncResult)));
            }

            Dispatcher.PushFrame(frame);
        }

        public void InvokeOnJvmThread(Action<JvmEnvironment> action)
        {
            Contract.Requires<ArgumentNullException>(action != null, "action");

            DispatcherOperation operationResult;
            lock (_dispatchers)
            {
                if (_dispatchers.Count == 0)
                    throw new InvalidOperationException("No JVM dispatchers are available.");

                var dispatcher = _dispatchers[0];
                operationResult = dispatcher.Item1.BeginInvoke(action, dispatcher.Item3);
            }

            operationResult.Wait();
        }

        public void HandleAsyncOperationComplete(IAsyncResult result)
        {
            Dispatcher dispatcher = null;
            DispatcherFrame frame = null;

            lock (_dispatchers)
            {
                // find and remove the appropriate dispatcher
                int index = _dispatchers.FindIndex(i => i.Item4 == result);
                if (index < 0)
                    throw new ArgumentException();

                dispatcher = _dispatchers[index].Item1;
                frame = _dispatchers[index].Item2;
                _dispatchers.RemoveAt(index);
            }

            // queue a low priority operation to end processing
            dispatcher.BeginInvoke((Action)(() => frame.Continue = false), DispatcherPriority.Background);
        }

        public JvmEnvironment GetEnvironment(jvmtiVersion version)
        {
            JniInvokeInterface jniInvokeInterface = GetRawInterface();

            jvmtiEnvHandle env;
            int result = jniInvokeInterface.GetEnv(this, out env, version);
            JniErrorHandler.ThrowOnFailure(result);

            return JvmEnvironment.GetOrCreateEnvironment(this, env);
        }

        public JvmNativeEnvironment AttachCurrentThread(ref JavaVMAttachArgs args, jvmtiVersion toolsVersion)
        {
            JniInvokeInterface jniInvokeInterface = GetRawInterface();

            JNIEnvHandle env;
            int result = jniInvokeInterface.AttachCurrentThread(this, out env, ref args);

            return GetEnvironment(toolsVersion).GetNativeFunctionTable(env);
        }

        private JniInvokeInterface GetRawInterface()
        {
            return (JniInvokeInterface)Marshal.PtrToStructure(_jniInvokeInterface, typeof(JniInvokeInterface));
        }
    }
}
