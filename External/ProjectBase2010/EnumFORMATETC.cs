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
	public class EnumFORMATETC : IEnumFORMATETC
	{
		IEnumerable cache; // of DataCacheEntrys.

		DATADIR dir;

		IEnumerator e;

		public EnumFORMATETC(DATADIR dir, IEnumerable cache)
		{
			this.cache = cache;
			this.dir = dir;
			e = cache.GetEnumerator();
		}

		void IEnumFORMATETC.Clone(out IEnumFORMATETC clone)
		{
			clone = new EnumFORMATETC(dir, cache);
		}

		int IEnumFORMATETC.Next(uint celt, FORMATETC[] d, uint[] fetched)
		{
			uint rc = 0;
			//uint size = (fetched != null) ? fetched[0] : 0;
			for(uint i = 0; i < celt; i++)
			{
				if(e.MoveNext())
				{
					DataCacheEntry entry = (DataCacheEntry)e.Current;

					rc++;
					if(d != null && d.Length > i)
					{
						d[i] = entry.Format;
					}
				}
				else
				{
					return VSConstants.S_FALSE;
				}
			}

			if(fetched != null && fetched.Length > 0)
				fetched[0] = rc;
			return VSConstants.S_OK;
		}

		int IEnumFORMATETC.Reset()
		{
			e.Reset();
			return 0;
		}

		int IEnumFORMATETC.Skip(uint celt)
		{
			for(uint i = 0; i < celt; i++)
			{
				e.MoveNext();
			}

			return 0;
		}
	}
}
