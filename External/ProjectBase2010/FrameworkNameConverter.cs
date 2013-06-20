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
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudio.Project
{
	public class FrameworkNameConverter : TypeConverter
	{
		public FrameworkNameConverter()
		{
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string)) return true;

			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			string str = value as string;

			if (str != null)
			{
				return new FrameworkName(str);
			}

			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				var name = value as FrameworkName;
				if (name != null)
				{
					return name.FullName;
				}
			}

			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override bool GetStandardValuesSupported(System.ComponentModel.ITypeDescriptorContext context)
		{
			return true;
		}

		public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(System.ComponentModel.ITypeDescriptorContext context)
		{
			IServiceProvider sp = ProjectNode.ServiceProvider;
			var multiTargetService = sp.GetService(typeof(SVsFrameworkMultiTargeting)) as IVsFrameworkMultiTargeting;
			if (multiTargetService == null)
			{
				Trace.TraceError("Unable to acquire the SVsFrameworkMultiTargeting service.");
				return new StandardValuesCollection(new string[0]);
			}
			Array frameworks;
			Marshal.ThrowExceptionForHR(multiTargetService.GetSupportedFrameworks(out frameworks));
			return new StandardValuesCollection(
				frameworks.Cast<string>().Select(fx => new FrameworkName(fx)).ToArray()
			);
		}
	}
}
