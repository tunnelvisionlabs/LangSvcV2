using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tvl.VisualStudio.Language.Java.JvmEventsService
{
    partial struct JvmClassRemoteHandle
    {
        public static implicit operator JvmToolsService.JvmClassRemoteHandle(JvmClassRemoteHandle handle)
        {
            return new JvmToolsService.JvmClassRemoteHandle()
            {
                Handle = handle.Handle
            };
        }
    }

    partial struct JvmVirtualMachineRemoteHandle
    {
        public static implicit operator JvmToolsService.JvmVirtualMachineRemoteHandle(JvmVirtualMachineRemoteHandle handle)
        {
            return new JvmToolsService.JvmVirtualMachineRemoteHandle()
            {
                Handle = handle.Handle
            };
        }
    }

    partial struct JvmThreadRemoteHandle
    {
        public static implicit operator JvmToolsService.JvmThreadRemoteHandle(JvmThreadRemoteHandle handle)
        {
            return new JvmToolsService.JvmThreadRemoteHandle()
            {
                Handle = handle.Handle
            };
        }

        public static implicit operator JvmToolsService.JvmObjectRemoteHandle(JvmThreadRemoteHandle handle)
        {
            return new JvmToolsService.JvmObjectRemoteHandle()
            {
                Handle = handle.Handle
            };
        }
    }
}

namespace Tvl.VisualStudio.Language.Java.Debugger.Extensions
{
    public static class ServiceExtensions
    {
    }
}
