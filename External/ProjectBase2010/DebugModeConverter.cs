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
using System.Runtime.Versioning;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Project
{
	public class DebugModeConverter : EnumConverter
	{

		public DebugModeConverter()
			: base(typeof(DebugMode))
		{

		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if(sourceType == typeof(string)) return true;

			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			string str = value as string;

			if(str != null)
			{
				if(str == SR.GetString(SR.Program, culture)) return DebugMode.Program;

				if(str == SR.GetString(SR.Project, culture)) return DebugMode.Project;

				if(str == SR.GetString(SR.URL, culture)) return DebugMode.URL;
			}

			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if(destinationType == typeof(string))
			{
				string result = null;
				// In some cases if multiple nodes are selected the windows form engine
				// calls us with a null value if the selected node's property values are not equal
				if(value != null)
				{
					result = SR.GetString(((DebugMode)value).ToString(), culture);
				}
				else
				{
					result = SR.GetString(DebugMode.Program.ToString(), culture);
				}

				if(result != null) return result;
			}

			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override bool GetStandardValuesSupported(System.ComponentModel.ITypeDescriptorContext context)
		{
			return true;
		}

		public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(System.ComponentModel.ITypeDescriptorContext context)
		{
			return new StandardValuesCollection(new DebugMode[] { DebugMode.Program, DebugMode.Project, DebugMode.URL });
		}
	}
}
