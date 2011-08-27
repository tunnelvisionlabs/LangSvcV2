namespace Tvl.VisualStudio.Language.Java.Debugger
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Debugger.Interop;
    using System.Runtime.InteropServices;
    using Tvl.Java.DebugInterface;
    using Tvl.VisualStudio.Language.Java.Debugger.Collections;
    using Microsoft.VisualStudio;
    using System.Diagnostics.Contracts;
    using System.Collections.ObjectModel;

    [ComVisible(true)]
    public class JavaDebugProperty : IDebugProperty3, IDebugProperty2
    {
        private readonly IDebugProperty2 _parent;
        private readonly string _name;
        private readonly string _fullName;
        private readonly IField _field;

        /* The property could be a java.lang.Object property even if the value it hold is a java.lang.String
         * (or any other object type). _propertyType is the type of this property and is assignable from
         * _value.GetValueType().
         */
        private readonly IType _propertyType;
        private readonly IValue _value;

        public JavaDebugProperty(IDebugProperty2 parent, string name, string fullName, IType propertyType, IValue value, IField field = null)
        {
            Contract.Requires<ArgumentNullException>(name != null, "name");
            Contract.Requires<ArgumentNullException>(fullName != null, "fullName");
            Contract.Requires<ArgumentNullException>(propertyType != null, "propertyType");

            _parent = parent;
            _name = name;
            _fullName = fullName;
            _propertyType = propertyType;
            _value = value;
            _field = field;
        }

        #region IDebugProperty2 Members

        public int EnumChildren(enum_DEBUGPROP_INFO_FLAGS dwFields, uint dwRadix, ref Guid guidFilter, enum_DBG_ATTRIB_FLAGS dwAttribFilter, string pszNameFilter, uint dwTimeout, out IEnumDebugPropertyInfo2 ppEnum)
        {
            IObjectReference objectReference = _value as IObjectReference;
            if (objectReference == null)
            {
                ppEnum = new EnumDebugPropertyInfo(Enumerable.Empty<DEBUG_PROPERTY_INFO>());
                return VSConstants.S_OK;
            }

            ppEnum = null;

            bool getFullName = (dwFields & enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_FULLNAME) != 0;
            bool getName = (dwFields & enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_NAME) != 0;
            bool getType = (dwFields & enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_TYPE) != 0;
            bool getValue = (dwFields & enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_VALUE) != 0;
            bool getAttributes = (dwFields & enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_ATTRIB) != 0;
            bool getProperty = (dwFields & enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_PROP) != 0;

            bool useAutoExpandValue = (dwFields & enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_VALUE_AUTOEXPAND) != 0;
            bool noFormatting = (dwFields & enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_VALUE_RAW) != 0;
            bool noToString = (dwFields & enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_VALUE_NO_TOSTRING) != 0;

            List<DEBUG_PROPERTY_INFO> properties = new List<DEBUG_PROPERTY_INFO>();
            DEBUG_PROPERTY_INFO[] propertyInfo = new DEBUG_PROPERTY_INFO[1];

            IArrayReference arrayReference = _value as IArrayReference;
            if (arrayReference != null)
            {
                IArrayType arrayType = (IArrayType)arrayReference.GetReferenceType();
                IType componentType = arrayType.GetComponentType();

                ReadOnlyCollection<IValue> values = null;
                if (getValue || getProperty)
                    values = arrayReference.GetValues();

                for (int i = 0; i < arrayReference.GetLength(); i++)
                {
                    propertyInfo[0] = default(DEBUG_PROPERTY_INFO);

                    if (getValue || getProperty)
                    {
                        string name = "[" + i + "]";
                        IType propertyType = componentType;
                        IValue value = values[i];
                        JavaDebugProperty property = new JavaDebugProperty(this, name, this._fullName + name, propertyType, value);
                        int hr = property.GetPropertyInfo(dwFields, dwRadix, dwTimeout, null, 0, propertyInfo);
                        if (ErrorHandler.Failed(hr))
                            return hr;

                        properties.Add(propertyInfo[0]);
                        continue;
                    }
                    else
                    {
                        if (getFullName)
                        {
                            propertyInfo[0].bstrFullName = this._fullName + "[" + i + "]";
                            propertyInfo[0].dwFields |= enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_FULLNAME;
                        }

                        if (getName)
                        {
                            propertyInfo[0].bstrName = "[" + i + "]";
                            propertyInfo[0].dwFields |= enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_NAME;
                        }

                        if (getType)
                        {
                            propertyInfo[0].bstrType = componentType.GetName();
                            propertyInfo[0].dwFields |= enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_TYPE;
                        }

                        if (getAttributes)
                        {
                            propertyInfo[0].dwAttrib |= enum_DBG_ATTRIB_FLAGS.DBG_ATTRIB_VALUE_AUTOEXPANDED;
                            propertyInfo[0].dwAttrib |= enum_DBG_ATTRIB_FLAGS.DBG_ATTRIB_VALUE_READONLY;
                            propertyInfo[0].dwFields |= enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_ATTRIB;
#if false
                            bool expandable;
                            bool hasId;
                            bool canHaveId;
                            bool readOnly;
                            bool error;
                            bool sideEffect;
                            bool overloadedContainer;
                            bool boolean;
                            bool booleanTrue;
                            bool invalid;
                            bool notAThing;
                            bool autoExpanded;
                            bool timeout;
                            bool rawString;
                            bool customViewer;

                            bool accessNone;
                            bool accessPrivate;
                            bool accessProtected;
                            bool accessPublic;

                            bool storageNone;
                            bool storageGlobal;
                            bool storageStatic;
                            bool storageRegister;

                            bool noModifiers;
                            bool @virtual;
                            bool constant;
                            bool synchronized;
                            bool @volatile;

                            bool dataField;
                            bool method;
                            bool property;
                            bool @class;
                            bool baseClass;
                            bool @interface;
                            bool innerClass;
                            bool mostDerived;

                            bool multiCustomViewers;
#endif
                        }
                    }
                }
            }
            else
            {
                IReferenceType objectType = objectReference.GetReferenceType();
                ReadOnlyCollection<IField> fields = objectType.GetFields(false);
                List<IField> staticFields = new List<IField>(fields.Where(i => i.GetIsStatic()));

                foreach (var field in fields)
                {
                    if (field.GetIsStatic())
                        continue;

                    propertyInfo[0] = default(DEBUG_PROPERTY_INFO);

                    if (getValue || getProperty)
                    {
                        IDebugProperty2 property;
                        try
                        {
                            string name = field.GetName();
                            IType propertyType = field.GetFieldType();
                            IValue value = objectReference.GetValue(field);
                            property = new JavaDebugProperty(this, name, this._fullName + "." + name, propertyType, value, field);
                            ErrorHandler.ThrowOnFailure(property.GetPropertyInfo(dwFields, dwRadix, dwTimeout, null, 0, propertyInfo));
                        }
                        catch (Exception e)
                        {
                            if (ErrorHandler.IsCriticalException(e))
                                throw;

                            string name = field.GetName();
                            IType propertyType = field.GetFieldType();
                            IValue value = field.GetVirtualMachine().GetMirrorOf(0);
                            property = new JavaDebugProperty(this, name, this._fullName + "." + name, propertyType, value, field);
                            ErrorHandler.ThrowOnFailure(property.GetPropertyInfo(dwFields, dwRadix, dwTimeout, null, 0, propertyInfo));
                        }
                    }
                    else
                    {
                        if (getFullName)
                        {
                            propertyInfo[0].bstrFullName = this._fullName + "." + field.GetName();
                            propertyInfo[0].dwFields |= enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_FULLNAME;
                        }

                        if (getName)
                        {
                            propertyInfo[0].bstrName = field.GetName();
                            propertyInfo[0].dwFields |= enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_NAME;
                        }

                        if (getType)
                        {
                            propertyInfo[0].bstrType = field.GetFieldTypeName();
                            propertyInfo[0].dwFields |= enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_TYPE;
                        }

                        if (getAttributes)
                        {
                            if (field.GetIsStatic())
                                propertyInfo[0].dwAttrib |= enum_DBG_ATTRIB_FLAGS.DBG_ATTRIB_STORAGE_STATIC;
                            if (field.GetIsPrivate())
                                propertyInfo[0].dwAttrib |= enum_DBG_ATTRIB_FLAGS.DBG_ATTRIB_ACCESS_PRIVATE;
                            if (field.GetIsProtected())
                                propertyInfo[0].dwAttrib |= enum_DBG_ATTRIB_FLAGS.DBG_ATTRIB_ACCESS_PROTECTED;
                            if (field.GetIsPublic())
                                propertyInfo[0].dwAttrib |= enum_DBG_ATTRIB_FLAGS.DBG_ATTRIB_ACCESS_PUBLIC;

                            propertyInfo[0].dwAttrib |= enum_DBG_ATTRIB_FLAGS.DBG_ATTRIB_VALUE_READONLY;
                            propertyInfo[0].dwFields |= enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_ATTRIB;
#if false
                            bool expandable;
                            bool hasId;
                            bool canHaveId;
                            bool readOnly;
                            bool error;
                            bool sideEffect;
                            bool overloadedContainer;
                            bool boolean;
                            bool booleanTrue;
                            bool invalid;
                            bool notAThing;
                            bool autoExpanded;
                            bool timeout;
                            bool rawString;
                            bool customViewer;

                            bool accessNone;
                            bool accessPrivate;
                            bool accessProtected;
                            bool accessPublic;

                            bool storageNone;
                            bool storageGlobal;
                            bool storageStatic;
                            bool storageRegister;

                            bool noModifiers;
                            bool @virtual;
                            bool constant;
                            bool synchronized;
                            bool @volatile;

                            bool dataField;
                            bool method;
                            bool property;
                            bool @class;
                            bool baseClass;
                            bool @interface;
                            bool innerClass;
                            bool mostDerived;

                            bool multiCustomViewers;
#endif
                        }
                    }

                    properties.Add(propertyInfo[0]);
                    continue;
                }

                if (staticFields.Count > 0)
                {
                    propertyInfo[0] = default(DEBUG_PROPERTY_INFO);

                    JavaDebugStaticMembersPseudoProperty property = new JavaDebugStaticMembersPseudoProperty(this, objectType, staticFields);
                    ErrorHandler.ThrowOnFailure(property.GetPropertyInfo(dwFields, dwRadix, dwTimeout, null, 0, propertyInfo));
                    properties.Add(propertyInfo[0]);
                }
            }

            ppEnum = new EnumDebugPropertyInfo(properties);
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Gets the derived-most property of a property.
        /// </summary>
        /// <param name="ppDerivedMost">Returns an IDebugProperty2 object that represents the derived-most property.</param>
        /// <returns>If successful, returns S_OK; otherwise returns error code. Returns S_GETDERIVEDMOST_NO_DERIVED_MOST if there is no derived-most property to retrieve.</returns>
        /// <remarks>
        /// For example, if this property describes an object that implements ClassRoot but which is actually an instantiation
        /// of ClassDerived that is derived from ClassRoot, then this method returns an IDebugProperty2 object describing the
        /// ClassDerived object.
        /// </remarks>
        public int GetDerivedMostProperty(out IDebugProperty2 ppDerivedMost)
        {
            ppDerivedMost = null;
            if (_value == null)
                return AD7Constants.S_GETDERIVEDMOST_NO_DERIVED_MOST;

            IReferenceType propertyReferenceType = _propertyType as IReferenceType;
            IReferenceType valueReferenceType = _value.GetValueType() as IReferenceType;
            if (propertyReferenceType == null || valueReferenceType == null)
                return AD7Constants.S_GETDERIVEDMOST_NO_DERIVED_MOST;

            if (valueReferenceType.Equals(propertyReferenceType))
                return AD7Constants.S_GETDERIVEDMOST_NO_DERIVED_MOST;

            string castName = string.Format("({0})({1})", valueReferenceType.GetName(), _fullName);
            ppDerivedMost = new JavaDebugProperty(this, _name, castName, valueReferenceType, _value, _field);
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Gets extended information for the property.
        /// </summary>
        /// <param name="guidExtendedInfo">GUID that determines the type of extended information to be retrieved. See Remarks for details.</param>
        /// <param name="pExtendedInfo">
        /// Returns a VARIANT (C++) or object (C#) that can be used to retrieve the extended property information.
        /// For example, this parameter might return an IUnknown interface that can be queried for an IDebugDocumentText2
        /// interface. See Remarks for details.
        /// </param>
        /// <returns>
        /// If successful, returns S_OK; otherwise returns error code. Returns S_GETEXTENDEDINFO_NO_EXTENDEDINFO
        /// if there is no extended information to retrieve.
        /// </returns>
        /// <remarks>
        /// This method exists for the purpose of retrieving information that does not lend itself to being retrieved
        /// by calling the IDebugProperty2.GetPropertyInfo method.
        /// 
        /// The following GUIDs are typically recognized by this method (the GUID values are specified for C# since the
        /// name is not available in any assembly). Additional GUIDs can be created for internal use.
        /// </remarks>
        public int GetExtendedInfo(ref Guid guidExtendedInfo, out object pExtendedInfo)
        {
            pExtendedInfo = null;
            return AD7Constants.S_GETEXTENDEDINFO_NO_EXTENDEDINFO;
        }

        /// <summary>
        /// Gets the memory bytes that compose the value of a property.
        /// </summary>
        /// <param name="ppMemoryBytes">[out] Returns an IDebugMemoryBytes2 object that can be used to retrieve the memory that contains the value of the property.</param>
        /// <returns>If successful, returns S_OK; otherwise returns error code. Returns S_GETMEMORYBYTES_NO_MEMORY_BYTES if there are no memory bytes to retrieve.</returns>
        public int GetMemoryBytes(out IDebugMemoryBytes2 ppMemoryBytes)
        {
            ppMemoryBytes = null;
            return AD7Constants.S_GETMEMORYBYTES_NO_MEMORY_BYTES;
        }

        /// <summary>
        /// Gets the memory context of the property value.
        /// </summary>
        /// <param name="ppMemory">[out] Returns the IDebugMemoryContext2 object that represents the memory associated with this property.</param>
        /// <returns>If successful, returns S_OK; otherwise returns error code. Returns S_GETMEMORYCONTEXT_NO_MEMORY_CONTEXT if there is no memory context to retrieve.</returns>
        public int GetMemoryContext(out IDebugMemoryContext2 ppMemory)
        {
            ppMemory = null;
            return AD7Constants.S_GETMEMORYCONTEXT_NO_MEMORY_CONTEXT;
        }

        public int GetParent(out IDebugProperty2 ppParent)
        {
            ppParent = _parent;
            return ppParent != null ? VSConstants.S_OK : AD7Constants.S_GETPARENT_NO_PARENT;
        }

        /// <summary>
        /// Gets the DEBUG_PROPERTY_INFO structure that describes a property.
        /// </summary>
        /// <param name="dwFields">[in] A combination of values from the DEBUGPROP_INFO_FLAGS enumeration that specifies which fields are to be filled out in the pPropertyInfo structure.</param>
        /// <param name="dwRadix">[in] Radix to be used in formatting any numerical information.</param>
        /// <param name="dwTimeout">[in] Specifies the maximum time, in milliseconds, to wait before returning from this method. Use INFINITE to wait indefinitely.</param>
        /// <param name="rgpArgs">[in, out] Reserved for future use; set to a null value.</param>
        /// <param name="dwArgCount">[in] Reserved for future use; set to zero.</param>
        /// <param name="pPropertyInfo">[out] A DEBUG_PROPERTY_INFO structure that is filled in with the description of the property.</param>
        /// <returns>If successful, returns S_OK; otherwise returns error code.</returns>
        public int GetPropertyInfo(enum_DEBUGPROP_INFO_FLAGS dwFields, uint dwRadix, uint dwTimeout, IDebugReference2[] rgpArgs, uint dwArgCount, DEBUG_PROPERTY_INFO[] pPropertyInfo)
        {
            if (pPropertyInfo == null)
                throw new ArgumentNullException("pPropertyInfo");
            if (pPropertyInfo.Length == 0)
                throw new ArgumentException();

            bool getFullName = (dwFields & enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_FULLNAME) != 0;
            bool getName = (dwFields & enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_NAME) != 0;
            bool getType = (dwFields & enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_TYPE) != 0;
            bool getValue = (dwFields & enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_VALUE) != 0;
            bool getAttributes = (dwFields & enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_ATTRIB) != 0;
            bool getProperty = (dwFields & enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_PROP) != 0;

            bool useAutoExpandValue = (dwFields & enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_VALUE_AUTOEXPAND) != 0;
            bool noFormatting = (dwFields & enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_VALUE_RAW) != 0;
            bool noToString = (dwFields & enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_VALUE_NO_TOSTRING) != 0;

            if (getFullName)
            {
                pPropertyInfo[0].bstrFullName = _fullName;
                pPropertyInfo[0].dwFields |= enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_FULLNAME;
            }

            if (getName)
            {
                pPropertyInfo[0].bstrName = _name;
                pPropertyInfo[0].dwFields |= enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_NAME;
            }

            if (getType)
            {
                pPropertyInfo[0].bstrType = _propertyType.GetName();
                pPropertyInfo[0].dwFields |= enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_TYPE;
            }

            if (getValue)
            {
                if (_value == null)
                {
                    pPropertyInfo[0].bstrValue = "null";
                }
                if (_value is IVoidValue)
                {
                    pPropertyInfo[0].bstrValue = "The expression has been evaluated and has no value.";
                }
                else if (_value is IPrimitiveValue)
                {
                    IBooleanValue booleanValue = _value as IBooleanValue;
                    if (booleanValue != null)
                    {
                        pPropertyInfo[0].bstrValue = booleanValue.GetValue().ToString();
                        pPropertyInfo[0].dwAttrib |= enum_DBG_ATTRIB_FLAGS.DBG_ATTRIB_VALUE_BOOLEAN;
                        if (booleanValue.GetValue())
                            pPropertyInfo[0].dwAttrib |= enum_DBG_ATTRIB_FLAGS.DBG_ATTRIB_VALUE_BOOLEAN_TRUE;
                    }

                    IByteValue byteValue = _value as IByteValue;
                    if (byteValue != null)
                        pPropertyInfo[0].bstrValue = byteValue.GetValue().ToString();

                    ICharValue charValue = _value as ICharValue;
                    if (charValue != null)
                        pPropertyInfo[0].bstrValue = charValue.GetValue().ToString();

                    IShortValue shortValue = _value as IShortValue;
                    if (shortValue != null)
                        pPropertyInfo[0].bstrValue = shortValue.GetValue().ToString();

                    IIntegerValue integerValue = _value as IIntegerValue;
                    if (integerValue != null)
                        pPropertyInfo[0].bstrValue = integerValue.GetValue().ToString();

                    ILongValue longValue = _value as ILongValue;
                    if (longValue != null)
                        pPropertyInfo[0].bstrValue = longValue.GetValue().ToString();

                    IFloatValue floatValue = _value as IFloatValue;
                    if (floatValue != null)
                        pPropertyInfo[0].bstrValue = floatValue.GetValue().ToString();

                    IDoubleValue doubleValue = _value as IDoubleValue;
                    if (doubleValue != null)
                        pPropertyInfo[0].bstrValue = doubleValue.GetValue().ToString();
                }
                else if (_value is IArrayReference)
                {
                    IArrayReference arrayReference = _value as IArrayReference;
                    int length = arrayReference.GetLength();
                    IArrayType arrayType = (IArrayType)arrayReference.GetReferenceType();
                    pPropertyInfo[0].bstrValue = string.Format("{{{0}[{1}]}}", arrayType.GetComponentTypeName(), length);
                    if (length > 0)
                        pPropertyInfo[0].dwAttrib |= enum_DBG_ATTRIB_FLAGS.DBG_ATTRIB_OBJ_IS_EXPANDABLE;
                }
                else if (_value is IObjectReference)
                {
                    IStringReference stringReference = _value as IStringReference;
                    if (stringReference != null)
                    {
                        pPropertyInfo[0].bstrValue = EscapeSpecialCharacters(stringReference.GetValue(), 120);
                    }
                    else
                    {
                        IObjectReference objectReference = _value as IObjectReference;
                        if (objectReference != null)
                        {
                            pPropertyInfo[0].bstrValue = "{" + objectReference.GetReferenceType().GetName() + "}";
                            pPropertyInfo[0].dwAttrib |= enum_DBG_ATTRIB_FLAGS.DBG_ATTRIB_OBJ_IS_EXPANDABLE;
                        }
                        else
                        {
                            pPropertyInfo[0].bstrValue = "Unrecognized value";
                            pPropertyInfo[0].dwAttrib |= enum_DBG_ATTRIB_FLAGS.DBG_ATTRIB_VALUE_ERROR;
                        }
                    }
                }

                pPropertyInfo[0].dwFields |= enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_VALUE;
            }

            if (getAttributes)
            {
                if (_field != null)
                {
                    if (_field.GetIsPrivate())
                        pPropertyInfo[0].dwAttrib |= enum_DBG_ATTRIB_FLAGS.DBG_ATTRIB_ACCESS_PRIVATE;
                    if (_field.GetIsProtected())
                        pPropertyInfo[0].dwAttrib |= enum_DBG_ATTRIB_FLAGS.DBG_ATTRIB_ACCESS_PROTECTED;
                    if (_field.GetIsPublic())
                        pPropertyInfo[0].dwAttrib |= enum_DBG_ATTRIB_FLAGS.DBG_ATTRIB_ACCESS_PUBLIC;
                }

                pPropertyInfo[0].dwAttrib |= enum_DBG_ATTRIB_FLAGS.DBG_ATTRIB_VALUE_READONLY;
                pPropertyInfo[0].dwFields |= enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_ATTRIB;
#if false
                bool expandable;
                bool hasId;
                bool canHaveId;
                bool readOnly;
                bool error;
                bool sideEffect;
                bool overloadedContainer;
                bool boolean;
                bool booleanTrue;
                bool invalid;
                bool notAThing;
                bool autoExpanded;
                bool timeout;
                bool rawString;
                bool customViewer;

                bool accessNone;
                bool accessPrivate;
                bool accessProtected;
                bool accessPublic;

                bool storageNone;
                bool storageGlobal;
                bool storageStatic;
                bool storageRegister;

                bool noModifiers;
                bool @virtual;
                bool constant;
                bool synchronized;
                bool @volatile;

                bool dataField;
                bool method;
                bool property;
                bool @class;
                bool baseClass;
                bool @interface;
                bool innerClass;
                bool mostDerived;

                bool multiCustomViewers;
#endif
            }

            if (getProperty)
            {
                pPropertyInfo[0].pProperty = this;
                pPropertyInfo[0].dwFields |= enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_PROP;
            }

            return VSConstants.S_OK;
        }

        private static string EscapeSpecialCharacters(string value, int maxLength)
        {
            bool cropped = value.Length > maxLength;
            if (cropped)
                value = value.Substring(0, maxLength) + "...";

            value = value.Replace("\r", "\\r").Replace("\t", "\\t").Replace("\n", "\\n").Replace("\"", "\\\"");
            if (cropped)
                value = '"' + value;
            else
                value = '"' + value + '"';

            return value;
        }

        /// <summary>
        /// Returns a reference to the property's value.
        /// </summary>
        /// <param name="ppReference">[out] Returns an IDebugReference2 object representing a reference to the property's value.</param>
        /// <returns>If successful, returns S_OK; otherwise, returns an error code, typically E_NOTIMPL or E_GETREFERENCE_NO_REFERENCE.</returns>
        public int GetReference(out IDebugReference2 ppReference)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the size, in bytes, of the property value.
        /// </summary>
        /// <param name="pdwSize">[out] Returns the size, in bytes, of the property value.</param>
        /// <returns>If successful, returns S_OK; otherwise returns error code. Returns S_GETSIZE_NO_SIZE if the property has no size.</returns>
        public int GetSize(out uint pdwSize)
        {
            throw new NotImplementedException();
        }

        public int SetValueAsReference(IDebugReference2[] rgpArgs, uint dwArgCount, IDebugReference2 pValue, uint dwTimeout)
        {
            throw new NotImplementedException();
        }

        public int SetValueAsString(string pszValue, uint dwRadix, uint dwTimeout)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDebugProperty3 Members

        public int CreateObjectID()
        {
            throw new NotImplementedException();
        }

        public int DestroyObjectID()
        {
            throw new NotImplementedException();
        }

        public int GetCustomViewerCount(out uint pcelt)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a list of custom viewers associated with this property.
        /// </summary>
        /// <param name="celtSkip">[in] The number of viewers to skip over.</param>
        /// <param name="celtRequested">[in] The number of viewers to retrieve (also specifies the size of the rgViewers array).</param>
        /// <param name="rgViewers">[in, out] Array of DEBUG_CUSTOM_VIEWER structures to be filled in.</param>
        /// <param name="pceltFetched">[out] The actual number of viewers returned.</param>
        /// <returns>If successful, returns S_OK; otherwise, returns an error code.</returns>
        /// <remarks>
        /// To support type visualizers, this method forwards the call to the IEEVisualizerService.GetCustomViewerList method.
        /// If the expression evaluator also supports custom viewers for this property's type, this method can append the
        /// appropriate custom viewers to the list.
        /// 
        /// See Type Visualizer and Custom Viewer for details on the differences between type visualizers and custom viewers.
        /// </remarks>
        public int GetCustomViewerList(uint celtSkip, uint celtRequested, DEBUG_CUSTOM_VIEWER[] rgViewers, out uint pceltFetched)
        {
            throw new NotImplementedException();
        }

        public int GetStringCharLength(out uint pLen)
        {
            throw new NotImplementedException();
        }

        public int GetStringChars(uint buflen, ushort[] rgString, out uint pceltFetched)
        {
            throw new NotImplementedException();
        }

        public int SetValueAsStringWithError(string pszValue, uint dwRadix, uint dwTimeout, out string errorString)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
