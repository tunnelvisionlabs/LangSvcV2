/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.VisualStudio.Project
{
	public class EnumSTATDATA : IEnumSTATDATA
	{
		IEnumerable i;

		IEnumerator e;

		public EnumSTATDATA(IEnumerable i)
		{
			this.i = i;
			this.e = i.GetEnumerator();
		}

		void IEnumSTATDATA.Clone(out IEnumSTATDATA clone)
		{
			clone = new EnumSTATDATA(i);
		}

		int IEnumSTATDATA.Next(uint celt, STATDATA[] d, out uint fetched)
		{
			uint rc = 0;
			//uint size = (fetched != null) ? fetched[0] : 0;
			for(uint i = 0; i < celt; i++)
			{
				if(e.MoveNext())
				{
					STATDATA sdata = (STATDATA)e.Current;

					rc++;
					if(d != null && d.Length > i)
					{
						d[i] = sdata;
					}
				}
			}

			fetched = rc;
			return 0;
		}

		int IEnumSTATDATA.Reset()
		{
			e.Reset();
			return 0;
		}

		int IEnumSTATDATA.Skip(uint celt)
		{
			for(uint i = 0; i < celt; i++)
			{
				e.MoveNext();
			}

			return 0;
		}
	}
}
