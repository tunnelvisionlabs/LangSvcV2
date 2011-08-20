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

    public static class AgentExports
    {
        private static ServiceHost _jvmDebugSessionHost;
        private static ServiceHost _jvmEventsPublisherHost;
        private static ServiceHost _jvmToolsInterfaceHost;
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

            JavaVM vm = JavaVM.GetOrCreateInstance(Marshal.ReadIntPtr(vmPtr));

            string options = null;
            if (optionsPtr != IntPtr.Zero)
                options = ModifiedUTF8Encoding.GetString((byte*)optionsPtr);

            JvmEnvironment env = vm.GetEnvironment(jvmtiVersion.Version1_1);

#if false
            AppDomain.CurrentDomain.FirstChanceException += HandleFirstChanceException;
            AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;
            AppDomain.CurrentDomain.ProcessExit += HandleProcessExit;
#endif

            // start the wcf services and wait for the client to connect
            _jvmEventsPublisherHost = new ServiceHost(typeof(JvmEventsPublisher));
            Binding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None)
            {
                ReceiveTimeout = TimeSpan.MaxValue,
                SendTimeout = TimeSpan.MaxValue
            };

            _jvmEventsPublisherHost.AddServiceEndpoint(typeof(IJvmEventsService), binding, "net.pipe://localhost/Tvl.Java.DebugHost/JvmEventsService/");
            IAsyncResult jvmEventsPublisherStartResult = _jvmEventsPublisherHost.BeginOpen(null, null);

            _jvmToolsInterfaceHost = new ServiceHost(typeof(JvmToolsInterfaceService));
            binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None)
            {
                ReceiveTimeout = TimeSpan.MaxValue,
                SendTimeout = TimeSpan.MaxValue
            };

            _jvmToolsInterfaceHost.AddServiceEndpoint(typeof(IJvmToolsInterfaceService), binding, "net.pipe://localhost/Tvl.Java.DebugHost/JvmToolsInterfaceService/");
            IAsyncResult toolsInterfaceStartResult = _jvmToolsInterfaceHost.BeginOpen(null, null);

            _jvmDebugSessionHost = new ServiceHost(typeof(JvmDebugSessionService));
            binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None)
            {
                ReceiveTimeout = TimeSpan.MaxValue,
                SendTimeout = TimeSpan.MaxValue
            };

            _jvmDebugSessionHost.AddServiceEndpoint(typeof(IJvmDebugSessionService), binding, "net.pipe://localhost/Tvl.Java.DebugHost/JvmDebugSessionService/");
            IAsyncResult debugSessionStartResult = _jvmDebugSessionHost.BeginOpen(null, null);

            WaitHandle.WaitAll(new WaitHandle[] { jvmEventsPublisherStartResult.AsyncWaitHandle, toolsInterfaceStartResult.AsyncWaitHandle, debugSessionStartResult.AsyncWaitHandle });

            EventWaitHandle.OpenExisting(string.Format("JavaDebuggerInitHandle{0}", Process.GetCurrentProcess().Id)).Set();
            _debuggerAttachComplete.Wait();

            return 0;
        }

        public static unsafe int OnAttach(IntPtr vmPtr, IntPtr optionsPtr, IntPtr reserved)
        {
            _loaded = true;

            JavaVM vm = JavaVM.GetOrCreateInstance(Marshal.ReadIntPtr(vmPtr));

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
