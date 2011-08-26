namespace Tvl.Java.DebugHost.Interop
{
    using Thread = System.Threading.Thread;
    using MessageBoxDefaultButton = System.Windows.Forms.MessageBoxDefaultButton;
    using MessageBoxIcon = System.Windows.Forms.MessageBoxIcon;
    using System.Runtime.InteropServices;
    using IntPtr = System.IntPtr;
    using System.ServiceModel;
    using Tvl.Java.DebugHost.Services;
    using System.ServiceModel.Description;
    using System.ServiceModel.Channels;
    using WaitHandle = System.Threading.WaitHandle;
    using System;
    using ManualResetEventSlim=System.Threading.ManualResetEventSlim;
    using Process = System.Diagnostics.Process;
    using EventWaitHandle = System.Threading.EventWaitHandle;
    using AppDomain = System.AppDomain;
    using FirstChanceExceptionEventArgs = System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs;
    using MessageBox = System.Windows.Forms.MessageBox;
    using MessageBoxButtons = System.Windows.Forms.MessageBoxButtons;
    using System.Collections.Generic;

    public static class AgentExports
    {
        private static ServiceHost _jvmDebugSessionHost;
#if false
        private static ServiceHost _jvmEventsPublisherHost;
        private static ServiceHost _jvmToolsInterfaceHost;
#endif
        private static ServiceHost _debugProtocolHost;
        private static readonly ManualResetEventSlim _debuggerAttachComplete = new ManualResetEventSlim();

        private static bool _loaded;

        internal static ManualResetEventSlim DebuggerAttachComplete
        {
            get
            {
                return _debuggerAttachComplete;
            }
        }

        internal static bool IsLoaded
        {
            get
            {
                return _loaded;
            }
        }

        public static unsafe int OnLoad(IntPtr vmPtr, IntPtr optionsPtr, IntPtr reserved)
        {
            _loaded = true;

            JavaVM vm = JavaVM.GetOrCreateInstance(new JavaVMHandle(vmPtr));

            string options = null;
            if (optionsPtr != IntPtr.Zero)
                options = ModifiedUTF8Encoding.GetString((byte*)optionsPtr);

#if false
            // quick test
            GetEnvironmentVersion(vm);

            Action<JavaVM> action = GetEnvironmentVersion;
            IAsyncResult result = action.BeginInvoke(vm, null, null);
            result.AsyncWaitHandle.WaitOne();
#endif


#if false
            AppDomain.CurrentDomain.FirstChanceException += HandleFirstChanceException;
            AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;
            AppDomain.CurrentDomain.ProcessExit += HandleProcessExit;
#endif

            List<WaitHandle> waitHandles = new List<WaitHandle>();
            Binding binding;

            /*
             * start the wcf services and wait for the client to connect
             */

#if false
            /* IJvmEventsService
             */
            _jvmEventsPublisherHost = new ServiceHost(typeof(JvmEventsPublisher));
            Binding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None)
            {
                ReceiveTimeout = TimeSpan.MaxValue,
                SendTimeout = TimeSpan.MaxValue
            };

            _jvmEventsPublisherHost.AddServiceEndpoint(typeof(IJvmEventsService), binding, "net.pipe://localhost/Tvl.Java.DebugHost/JvmEventsService/");
            IAsyncResult jvmEventsPublisherStartResult = _jvmEventsPublisherHost.BeginOpen(null, null);
            waitHandles.Add(jvmEventsPublisherStartResult.AsyncWaitHandle);

            /* IJvmToolsInterfaceService
             */
            _jvmToolsInterfaceHost = new ServiceHost(typeof(JvmToolsInterfaceService));
            binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None)
            {
                ReceiveTimeout = TimeSpan.MaxValue,
                SendTimeout = TimeSpan.MaxValue
            };

            _jvmToolsInterfaceHost.AddServiceEndpoint(typeof(IJvmToolsInterfaceService), binding, "net.pipe://localhost/Tvl.Java.DebugHost/JvmToolsInterfaceService/");
            IAsyncResult toolsInterfaceStartResult = _jvmToolsInterfaceHost.BeginOpen(null, null);
            waitHandles.Add(toolsInterfaceStartResult.AsyncWaitHandle);
#endif

            /* IJvmDebugSessionService
             */
            _jvmDebugSessionHost = new ServiceHost(typeof(JvmDebugSessionService));
            binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None)
            {
                ReceiveTimeout = TimeSpan.MaxValue,
                SendTimeout = TimeSpan.MaxValue
            };

            _jvmDebugSessionHost.AddServiceEndpoint(typeof(IJvmDebugSessionService), binding, "net.pipe://localhost/Tvl.Java.DebugHost/JvmDebugSessionService/");
            IAsyncResult debugSessionStartResult = _jvmDebugSessionHost.BeginOpen(null, null);
            waitHandles.Add(debugSessionStartResult.AsyncWaitHandle);

            /* IDebugProtocolService
             */
            _debugProtocolHost = new ServiceHost(new DebugProtocolService(vm));
            binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None)
            {
                ReceiveTimeout = TimeSpan.MaxValue,
                SendTimeout = TimeSpan.MaxValue
            };

            _debugProtocolHost.AddServiceEndpoint(typeof(IDebugProtocolService), binding, "net.pipe://localhost/Tvl.Java.DebugHost/DebugProtocolService/");
            IAsyncResult debugProtocolStartResult = _debugProtocolHost.BeginOpen(null, null);
            waitHandles.Add(debugProtocolStartResult.AsyncWaitHandle);

            /* Wait for the services to finish opening
             */
            WaitHandle.WaitAll(waitHandles.ToArray());

            EventWaitHandle.OpenExisting(string.Format("JavaDebuggerInitHandle{0}", Process.GetCurrentProcess().Id)).Set();
            _debuggerAttachComplete.Wait();

            return 0;
        }

        private static void GetEnvironmentVersion(JavaVM vm)
        {
            JvmtiEnvironment env;
            int error = vm.GetEnvironment(out env);

            int version;
            jvmtiError error2 = env.GetVersionNumber(out version);
        }

        public static unsafe int OnAttach(IntPtr vmPtr, IntPtr optionsPtr, IntPtr reserved)
        {
            _loaded = true;

            JavaVM vm = JavaVM.GetOrCreateInstance(new JavaVMHandle(vmPtr));

            string options = null;
            if (optionsPtr != IntPtr.Zero)
                options = ModifiedUTF8Encoding.GetString((byte*)optionsPtr);

            return 0;
        }

        public static void OnUnload(IntPtr vmPtr)
        {
            _loaded = false;
        }

        private static void HandleFirstChanceException(object sender, FirstChanceExceptionEventArgs e)
        {
            string caption = "First Chance Exception";
            MessageBox.Show(e.Exception.Message + Environment.NewLine + Environment.NewLine + e.Exception.StackTrace, caption, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
        }

        private static void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string caption = "Unhandled Exception";
            if (e.IsTerminating)
                caption += " (Fatal)";

            MessageBox.Show(e.ExceptionObject.ToString(), caption, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
        }

        private static void HandleProcessExit(object sender, EventArgs e)
        {
            Thread.Sleep(2500);
        }
    }
}
