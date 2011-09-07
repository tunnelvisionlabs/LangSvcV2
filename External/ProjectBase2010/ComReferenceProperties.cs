/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace Microsoft.VisualStudio.Project
{
    [ComVisible(true)]
    public class ComReferenceProperties : ReferenceNodeProperties
    {
        public ComReferenceProperties(ComReferenceNode node)
            : base(node)
        {
        }

        [SRCategory(SR.Misc)]
        [LocDisplayName(SR.EmbedInteropTypes)]
        [SRDescription(SR.EmbedInteropTypesDescription)]
        public virtual bool EmbedInteropTypes
        {
            get { return ((ComReferenceNode)this.Node).EmbedInteropTypes; }
            set { ((ComReferenceNode)this.Node).EmbedInteropTypes = value; }
        }
    }
}
