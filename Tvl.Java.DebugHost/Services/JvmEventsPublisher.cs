namespace Tvl.Java.DebugHost.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ServiceModel;
    using Tvl.Extensions;
    using Tvl.Java.DebugHost.Interop;
    using Dispatcher = System.Windows.Threading.Dispatcher;
    using DispatcherFrame = System.Windows.Threading.DispatcherFrame;
    using ReaderWriterLockSlim = System.Threading.ReaderWriterLockSlim;
    using LockRecursionPolicy = System.Threading.LockRecursionPolicy;

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Reentrant, IncludeExceptionDetailInFaults = true)]
    public class JvmEventsPublisher : JvmEventProcessorBase, IJvmEventsService
    {
        private IJvmEvents _subscriber;
        private readonly HashSet<JvmEventType> _subscribedEvents = new HashSet<JvmEventType>();
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        public void Subscribe(JvmEventType eventType)
        {
            if (!_subscribedEvents.Add(eventType))
                return;

            if (_subscribedEvents.Count > 1)
                return;

            _subscriber = OperationContext.Current.GetCallbackChannel<IJvmEvents>();
            JvmEnvironment environment = JvmEnvironment.GetCurrentInstance();
            environment.EventManager.AddProcessor(this);
        }

        public void Unsubscribe(JvmEventType eventType)
        {
            if (!_subscribedEvents.Remove(eventType))
                return;

            if (_subscribedEvents.Count >= 1)
                return;

            JvmEnvironment environment = JvmEnvironment.GetCurrentInstance();
            environment.EventManager.RemoveProcessor(this);
        }

        public override void HandleVMStart(JvmEnvironment environment)
        {
            if (!_subscribedEvents.Contains(JvmEventType.VMStart))
                return;

            try
            {
                DispatcherFrame frame = new DispatcherFrame(true);
                IAsyncResult result = _subscriber.BeginHandleVMStart(environment.VirtualMachine, environment.VirtualMachine.HandleAsyncOperationComplete, null);
                environment.VirtualMachine.PushDispatcherFrame(frame, environment, result);
                _subscriber.EndHandleVMStart(result);
            }
            catch (CommunicationException)
            {
            }
        }

        public override void HandleVMInitialization(JvmEnvironment environment, JvmThreadReference thread)
        {
            if (!_subscribedEvents.Contains(JvmEventType.VMInit))
                return;

            try
            {
                DispatcherFrame frame = new DispatcherFrame(true);
                IAsyncResult result = _subscriber.BeginHandleVMInitialization(environment.VirtualMachine, thread, environment.VirtualMachine.HandleAsyncOperationComplete, null);
                environment.VirtualMachine.PushDispatcherFrame(frame, environment, result);
                _subscriber.EndHandleVMInitialization(result);
            }
            catch (CommunicationException)
            {
            }
        }

        public override void HandleVMDeath(JvmEnvironment environment)
        {
            if (!_subscribedEvents.Contains(JvmEventType.VMDeath))
                return;

            try
            {
                DispatcherFrame frame = new DispatcherFrame(true);
                IAsyncResult result = _subscriber.BeginHandleVMDeath(environment.VirtualMachine, environment.VirtualMachine.HandleAsyncOperationComplete, null);
                environment.VirtualMachine.PushDispatcherFrame(frame, environment, result);
                _subscriber.EndHandleVMDeath(result);
            }
            catch (CommunicationException)
            {
            }
        }

        public override void HandleThreadStart(JvmEnvironment environment, JvmThreadReference thread)
        {
            if (!_subscribedEvents.Contains(JvmEventType.ThreadStart))
                return;

            try
            {
                DispatcherFrame frame = new DispatcherFrame(true);
                IAsyncResult result = _subscriber.BeginHandleThreadStart(environment.VirtualMachine, thread, environment.VirtualMachine.HandleAsyncOperationComplete, null);
                environment.VirtualMachine.PushDispatcherFrame(frame, environment, result);
                _subscriber.EndHandleThreadStart(result);
            }
            catch (CommunicationException)
            {
            }
        }

        public override void HandleThreadEnd(JvmEnvironment environment, JvmThreadReference thread)
        {
            if (!_subscribedEvents.Contains(JvmEventType.ThreadEnd))
                return;

            try
            {
                DispatcherFrame frame = new DispatcherFrame(true);
                IAsyncResult result = _subscriber.BeginHandleThreadEnd(environment.VirtualMachine, thread, environment.VirtualMachine.HandleAsyncOperationComplete, null);
                environment.VirtualMachine.PushDispatcherFrame(frame, environment, result);
                _subscriber.EndHandleThreadEnd(result);
            }
            catch (CommunicationException)
            {
            }
        }

        public override void HandleClassFileLoadHook(JvmEnvironment environment, JvmClassReference classBeingRedefined, JvmObjectReference loader, string name, JvmObjectReference protectionDomain)
        {
        }

        public override void HandleClassLoad(JvmEnvironment environment, JvmThreadReference thread, JvmClassReference @class)
        {
            if (!_subscribedEvents.Contains(JvmEventType.ThreadEnd))
                return;

            try
            {
                DispatcherFrame frame = new DispatcherFrame(true);
                IAsyncResult result = _subscriber.BeginHandleClassLoad(environment.VirtualMachine, thread, @class, environment.VirtualMachine.HandleAsyncOperationComplete, null);
                environment.VirtualMachine.PushDispatcherFrame(frame, environment, result);
                _subscriber.EndHandleClassLoad(result);
            }
            catch (CommunicationException)
            {
            }
        }

        public override void HandleClassPrepare(JvmEnvironment environment, JvmThreadReference thread, JvmClassReference @class)
        {
            if (!_subscribedEvents.Contains(JvmEventType.ThreadEnd))
                return;

            try
            {
                DispatcherFrame frame = new DispatcherFrame(true);
                IAsyncResult result = _subscriber.BeginHandleClassPrepare(environment.VirtualMachine, thread, @class, environment.VirtualMachine.HandleAsyncOperationComplete, null);
                environment.VirtualMachine.PushDispatcherFrame(frame, environment, result);
                _subscriber.EndHandleClassPrepare(result);
            }
            catch (CommunicationException)
            {
            }
        }
    }
}
