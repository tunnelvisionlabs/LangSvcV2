/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

namespace Microsoft.VisualStudio.Project.Automation
{
    using VSLangProj;

    public sealed class EmptyReferencesEvents : ConnectionPointContainer, IEventSource<_dispReferencesEvents>, ReferencesEvents
    {
        public static readonly EmptyReferencesEvents Instance = new EmptyReferencesEvents();

        public EmptyReferencesEvents()
        {
            AddEventSource<_dispReferencesEvents>(this as IEventSource<_dispReferencesEvents>);
        }

        event _dispReferencesEvents_ReferenceAddedEventHandler _dispReferencesEvents_Event.ReferenceAdded
        {
            add
            {
            }

            remove
            {
            }
        }

        event _dispReferencesEvents_ReferenceChangedEventHandler _dispReferencesEvents_Event.ReferenceChanged
        {
            add
            {
            }

            remove
            {
            }
        }

        event _dispReferencesEvents_ReferenceRemovedEventHandler _dispReferencesEvents_Event.ReferenceRemoved
        {
            add
            {
            }

            remove
            {
            }
        }

        void IEventSource<_dispReferencesEvents>.OnSinkAdded(_dispReferencesEvents sink)
        {
        }

        void IEventSource<_dispReferencesEvents>.OnSinkRemoved(_dispReferencesEvents sink)
        {
        }
    }
}
