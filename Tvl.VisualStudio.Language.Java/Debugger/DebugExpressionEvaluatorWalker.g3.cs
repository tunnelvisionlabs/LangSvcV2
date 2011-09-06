namespace Tvl.VisualStudio.Language.Java.Debugger
{
    using NumberStyles = System.Globalization.NumberStyles;
    using CultureInfo = System.Globalization.CultureInfo;
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Antlr.Runtime.Tree;
    using Tvl.Java.DebugInterface;
    using SignatureHelper = Tvl.Java.DebugInterface.Types.SignatureHelper;
    using System.Collections.Generic;

    partial class DebugExpressionEvaluatorWalker
    {
        private readonly IStackFrame _stackFrame;

        private IClassType _javaLangClassClass;
        private IMethod _classForNameMethod;

        public DebugExpressionEvaluatorWalker(ITreeNodeStream input, IStackFrame stackFrame)
            : this(input)
        {
            Contract.Requires<ArgumentNullException>(input != null, "input");
            Contract.Requires<ArgumentNullException>(stackFrame != null, "stackFrame");

            _stackFrame = stackFrame;
        }

        private EvaluatedExpression GetThisObject()
        {
            IMethod method = _stackFrame.GetLocation().GetMethod();
            if (method.GetIsStatic())
                throw new InvalidOperationException("The instance field cannot be accessed from a static method.");

            return new EvaluatedExpression("this", "this", _stackFrame.GetLocation().GetDeclaringType(), _stackFrame.GetThisObject(), false);
        }

        private EvaluatedExpression GetValueInScope(string name)
        {
            Contract.Requires<ArgumentNullException>(name != null, "name");
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(name));

            IMethod method = _stackFrame.GetLocation().GetMethod();

            // check the stack frame
            if (_stackFrame.GetHasVariableInfo())
            {
                ILocalVariable variable = _stackFrame.GetVisibleVariableByName(name);
                if (variable != null)
                    return new EvaluatedExpression(name, name, variable, _stackFrame.GetValue(variable), false);

                ReadOnlyCollection<ILocalVariable> variables = method.GetVariablesByName(name);
                if (variables.Count > 0)
                    throw new InvalidOperationException("Evaluation failed because the variable is not visible in the current scope.");
            }

            // check the enclosing object for a visible field
            IReferenceType declaringType = method.GetDeclaringType();
            IField field = declaringType.GetFieldByName(name);
            if (field != null)
            {
                if (field.GetIsStatic())
                {
                    return new EvaluatedExpression(name, name, null, field, declaringType.GetValue(field), false);
                }

                if (method.GetIsStatic())
                    throw new InvalidOperationException("The instance field cannot be accessed from a static method.");

                return new EvaluatedExpression(name, name, _stackFrame.GetThisObject(), field, _stackFrame.GetThisObject().GetValue(field), false);
            }

            // check the outer object?

            throw new NotImplementedException();
        }

        private bool TryGetValueInScope(string name, out EvaluatedExpression result)
        {
            Contract.Requires<ArgumentNullException>(name != null, "name");
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(name));

            result = null;

            IMethod method = _stackFrame.GetLocation().GetMethod();

            // check the stack frame
            if (_stackFrame.GetHasVariableInfo())
            {
                ILocalVariable variable = _stackFrame.GetVisibleVariableByName(name);
                if (variable != null)
                {
                    result = new EvaluatedExpression(name, name, variable, _stackFrame.GetValue(variable), false);
                    return true;
                }

                ReadOnlyCollection<ILocalVariable> variables = method.GetVariablesByName(name);
                if (variables.Count > 0)
                    return false;
                    //throw new InvalidOperationException("Evaluation failed because the variable is not visible in the current scope.");
            }

            // check the enclosing object for a visible field
            IReferenceType declaringType = method.GetDeclaringType();
            IField field = declaringType.GetFieldByName(name);
            if (field != null)
            {
                if (field.GetIsStatic())
                {
                    result = new EvaluatedExpression(name, name, null, field, declaringType.GetValue(field), false);
                    return true;
                }

                if (method.GetIsStatic())
                    return false;
                    //throw new InvalidOperationException("The instance field cannot be accessed from a static method.");

                result = new EvaluatedExpression(name, name, _stackFrame.GetThisObject(), field, _stackFrame.GetThisObject().GetValue(field), false);
                return true;
            }

            // check the outer object?

            return false;
            //throw new NotImplementedException();
        }

        private EvaluatedExpression GetField(EvaluatedExpression value, string name)
        {
            Contract.Requires<ArgumentNullException>(value != null, "value");
            Contract.Requires<ArgumentNullException>(name != null, "name");
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(name));

            IReferenceType declaringType = value.ValueType as IReferenceType;
            if (declaringType == null)
                throw new InvalidOperationException();

            IField field = declaringType.GetFieldByName(name);

            if (field != null)
            {
                string fullName = string.Format("({0}).{1}", value.FullName, name);
                if (field.GetIsStatic())
                {
                    return new EvaluatedExpression(name, fullName, null, field, declaringType.GetValue(field), value.HasSideEffects);
                }
                else
                {
                    IObjectReference objectReference = value.Value as IObjectReference;
                    if (objectReference == null)
                        throw new InvalidOperationException("Evaluation failed (todo: distinguish between null pointer and instance field referenced as a static field).");

                    return new EvaluatedExpression(name, fullName, objectReference, field, objectReference.GetValue(field), value.HasSideEffects);
                }
            }

            throw new NotImplementedException();
        }

        private EvaluatedExpression GetField(IReferenceType referenceType, string name)
        {
            Contract.Requires<ArgumentNullException>(referenceType != null, "referenceType");
            Contract.Requires<ArgumentNullException>(name != null, "name");
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(name));

            IField field = referenceType.GetFieldByName(name);
            if (field != null)
            {
                string fullName = string.Format("{0}.{1}", referenceType.GetName(), name);
                if (field.GetIsStatic())
                {
                    return new EvaluatedExpression(name, fullName, null, field, referenceType.GetValue(field), false);
                }

                throw new ArgumentException("The specified field is not static.");
            }

            throw new NotImplementedException();
        }

        private EvaluatedExpression EvaluateUnary(CommonTree op, EvaluatedExpression expression)
        {
            Contract.Requires<ArgumentNullException>(op != null, "op");
            Contract.Requires<ArgumentNullException>(expression != null, "expression");

            switch (op.Type)
            {
            case POSITIVE:
                // this doesn't change anything as long as the value is a supported (primitive numeric) type
                throw new NotImplementedException();

            case NEGATE:
                throw new NotImplementedException();

            case TILDE:
                throw new NotImplementedException();

            case BANG:
                throw new NotImplementedException();

            case PREINC:
            case PREDEC:
            case POSTINC:
            case POSTDEC:
                throw new NotImplementedException();

            default:
                throw new ArgumentException("Invalid unary operator.");
            }

            throw new NotImplementedException();
        }

        private EvaluatedExpression EvaluateBinary(CommonTree op, EvaluatedExpression left, EvaluatedExpression right)
        {
            Contract.Requires<ArgumentNullException>(op != null, "op");
            Contract.Requires<ArgumentNullException>(left != null, "left");
            Contract.Requires<ArgumentNullException>(right != null, "right");

            throw new NotImplementedException();
        }

        private EvaluatedExpression EvaluateTernary(EvaluatedExpression condition, EvaluatedExpression valueIfTrue, EvaluatedExpression valueIfFalse)
        {
            Contract.Requires<ArgumentNullException>(condition != null, "condition");
            Contract.Requires<ArgumentNullException>(valueIfTrue != null, "valueIfTrue");
            Contract.Requires<ArgumentNullException>(valueIfFalse != null, "valueIfFalse");

            throw new NotImplementedException();
        }

        private EvaluatedExpression EvaluateInstanceOf(EvaluatedExpression expression, IType type)
        {
            Contract.Requires<ArgumentNullException>(expression != null, "expression");
            Contract.Requires<ArgumentNullException>(type != null, "type");

            throw new NotImplementedException();
        }

        public EvaluatedExpression EvaluateObject(List<CommonTree> parts)
        {
            Contract.Requires<ArgumentNullException>(parts != null, "parts");
            Contract.Requires<ArgumentException>(parts.Count > 0);

            throw new NotImplementedException();
        }

        public EvaluatedExpression EvaluateTypeOrObject(List<CommonTree> parts)
        {
            Contract.Requires<ArgumentNullException>(parts != null, "parts");

            if (parts.Count == 0)
            {
                if (_stackFrame.GetLocation().GetMethod().GetIsStatic())
                {
                    IType declaringType = _stackFrame.GetLocation().GetDeclaringType();
                    string fullName = declaringType.GetName();
                    return new EvaluatedExpression(fullName, fullName, declaringType, default(IValue), false);
                }
                else
                {
                    return GetThisObject();
                }
            }

            int baseParts;
            EvaluatedExpression context = null;
            if (TryGetValueInScope(parts[0].Text, out context))
            {
                baseParts = 1;
            }
            else
            {
                IList<string> visibleNamespaces = new[] { string.Empty, "java.lang." };
                IReferenceType type = null;
                for (baseParts = 1; baseParts <= parts.Count; baseParts++)
                {
                    List<IReferenceType> matchingTypes = new List<IReferenceType>();
                    foreach (var ns in visibleNamespaces)
                    {
                        string typeName = ns + string.Join(".", parts.Take(baseParts));
                        matchingTypes.AddRange(_stackFrame.GetVirtualMachine().GetClassesByName(typeName));
                    }

                    if (matchingTypes.Count == 1)
                    {
                        type = matchingTypes[0];
                        break;
                    }

                    if (matchingTypes.Count > 1)
                        throw new InvalidOperationException("Ambiguous type name.");
                }

                context = new EvaluatedExpression(type.GetName(), type.GetName(), type, default(IValue), false);
            }

            // still have to evaluate fields
            for (int i = baseParts; i < parts.Count; i++)
            {
                context = GetField(context, parts[i].Text);
            }

            return context;
        }

        public EvaluatedExpression EvaluateMethod(ICollection<CommonTree> parts, ICollection<EvaluatedExpression> arguments)
        {
            Contract.Requires<ArgumentNullException>(parts != null, "parts");
            Contract.Requires<ArgumentException>(parts.Count > 0);
            Contract.Ensures(Contract.Result<EvaluatedExpression>() != null);
            Contract.Ensures(Contract.Result<EvaluatedExpression>().Method != null);

            List<CommonTree> partsList = new List<CommonTree>(parts);
            CommonTree methodName = partsList[partsList.Count - 1];
            partsList.RemoveAt(partsList.Count - 1);

            EvaluatedExpression instance = EvaluateTypeOrObject(partsList);

            if (instance != null)
            {
                IReferenceType referenceType = instance.ValueType as IReferenceType;
                if (referenceType == null)
                    throw new InvalidOperationException();

                List<IMethod> methods = referenceType.GetMethodsByName(methodName.Text).Where(i => i.GetDeclaringType() is IClassType).ToList();
                if (methods.Count > 0)
                    methods.RemoveAll(i => i.GetArgumentTypeNames().Count != arguments.Count);

                if (methods.Count == 0)
                    throw new InvalidOperationException("Evaluation failed.");

                if (methods.Count > 1)
                {
                    throw new NotImplementedException("Resolution by full signature is not yet implemented.");
                }

                IMethod method = methods[0];

                IObjectReference referencer = null;
                if (!method.GetIsStatic())
                {
                    referencer = instance.Value as IObjectReference;
                    if (referencer == null)
                        throw new InvalidOperationException();
                }

                string fullName = string.Join(".", parts.Select(i => i.Text));

                bool hasSideEffects = (instance != null && instance.HasSideEffects);
                return new EvaluatedExpression(fullName, fullName, referencer, method, hasSideEffects);
            }

            throw new NotImplementedException();
        }

        public EvaluatedExpression EvaluateCall(EvaluatedExpression obj, List<EvaluatedExpression> arguments)
        {
            Contract.Requires<ArgumentNullException>(obj != null, "obj");
            Contract.Requires<ArgumentNullException>(arguments != null, "arguments");
            Contract.Ensures(Contract.Result<EvaluatedExpression>() != null);

            IMethod method = obj.Method;
            if (method == null)
                throw new ArgumentException();

            IValue[] args = arguments.Select(i => i.Value).ToArray();
            IStrongValueHandle<IValue> result;
            if (method.GetIsStatic())
            {
                IClassType declaringType = (IClassType)method.GetDeclaringType();
                result = declaringType.InvokeMethod(default(IThreadReference), method, InvokeOptions.None, args);
            }
            else
            {
                IObjectReference instance = obj.Referencer;
                if (instance == null)
                    throw new InvalidOperationException("Cannot call an instance method on a null object.");

                result = instance.InvokeMethod(default(IThreadReference), method, InvokeOptions.None, args);
            }

            string fullName = string.Format("{0}({1})", obj.FullName, string.Join(", ", arguments.Select(i => i.FullName)));
            bool hasSideEffects = obj.HasSideEffects || arguments.Any(i => i.HasSideEffects);
            return new EvaluatedExpression(fullName, fullName, method.GetReturnType(), result, hasSideEffects);
        }

        private EvaluatedExpression FindClass(string signature)
        {
            Contract.Requires<ArgumentNullException>(signature != null, "signature");
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(signature));
            Contract.Ensures(Contract.Result<EvaluatedExpression>() != null);

            switch (signature[0])
            {
            case 'Z':
                return GetField(GetReflectedType(FindClass("Ljava/lang/Boolean;")), "TYPE");

            case 'B':
                return GetField(GetReflectedType(FindClass("Ljava/lang/Byte;")), "TYPE");

            case 'C':
                return GetField(GetReflectedType(FindClass("Ljava/lang/Character;")), "TYPE");

            case 'D':
                return GetField(GetReflectedType(FindClass("Ljava/lang/Double;")), "TYPE");

            case 'F':
                return GetField(GetReflectedType(FindClass("Ljava/lang/Float;")), "TYPE");

            case 'I':
                return GetField(GetReflectedType(FindClass("Ljava/lang/Integer;")), "TYPE");

            case 'J':
                return GetField(GetReflectedType(FindClass("Ljava/lang/Long;")), "TYPE");

            case 'S':
                return GetField(GetReflectedType(FindClass("Ljava/lang/Short;")), "TYPE");

            case 'V':
                return GetField(GetReflectedType(FindClass("Ljava/lang/Void;")), "TYPE");

            case '[':
            case 'L':
                if (_classForNameMethod == null)
                {
                    _javaLangClassClass = (IClassType)_stackFrame.GetVirtualMachine().GetClassesByName("java.lang.Class").Single();
                    _classForNameMethod = _javaLangClassClass.GetConcreteMethod("forName", "(Ljava/lang/String;)Ljava/lang/Class;");
                }

                if (signature[0] != '[')
                    signature = SignatureHelper.DecodeTypeName(signature);

                using (var signatureValue = _stackFrame.GetVirtualMachine().GetMirrorOf(signature))
                {
                    var result = _javaLangClassClass.InvokeMethod(null, _classForNameMethod, InvokeOptions.None, signatureValue.Value);
                    return new EvaluatedExpression(signature, signature, result.Value, true);
                }

            default:
                throw new ArgumentException();
            }
        }

        private IReferenceType GetReflectedType(EvaluatedExpression typeExpression)
        {
            Contract.Requires<ArgumentNullException>(typeExpression != null, "typeExpression");

            IClassObjectReference classObject = typeExpression.Value as IClassObjectReference;
            if (classObject == null)
                throw new ArgumentException();

            return classObject.GetReflectedType();
        }

        private EvaluatedExpression GetArrayClass(EvaluatedExpression elementType)
        {
            Contract.Requires<ArgumentNullException>(elementType != null, "elementType");
            Contract.Ensures(Contract.Result<EvaluatedExpression>() != null);

            IClassObjectReference classObject = elementType.Value as IClassObjectReference;
            if (classObject == null)
                throw new ArgumentException();

            IReferenceType elementReflectedType = classObject.GetReflectedType();
            return FindClass("[" + elementReflectedType.GetSignature());
        }

        private EvaluatedExpression GetBooleanLiteral(bool value)
        {
            Contract.Ensures(Contract.Result<EvaluatedExpression>() != null);

            string name = value.ToString().ToLowerInvariant();
            return new EvaluatedExpression(name, name, _stackFrame.GetVirtualMachine().GetMirrorOf(value), false);
        }

        private EvaluatedExpression GetIntLiteral(string value)
        {
            Contract.Requires<ArgumentNullException>(value != null, "value");
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(value));
            Contract.Ensures(Contract.Result<EvaluatedExpression>() != null);

            string parseValue = value;
            NumberStyles numberStyles = NumberStyles.None;
            if (parseValue.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                numberStyles |= NumberStyles.AllowHexSpecifier;
                parseValue = parseValue.Substring(2);
            }

            int intValue;
            if (!int.TryParse(parseValue, numberStyles, CultureInfo.InvariantCulture, out intValue))
                throw new FormatException();

            return new EvaluatedExpression(value, value, _stackFrame.GetVirtualMachine().GetMirrorOf(intValue), false);
        }

        private EvaluatedExpression GetLongLiteral(string value)
        {
            Contract.Requires<ArgumentNullException>(value != null, "value");
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(value));
            Contract.Ensures(Contract.Result<EvaluatedExpression>() != null);

            string parseValue = value;
            NumberStyles numberStyles = NumberStyles.None;
            if (parseValue.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                numberStyles |= NumberStyles.AllowHexSpecifier;
                parseValue = parseValue.Substring(2);
            }

            long longValue;
            if (!long.TryParse(parseValue, numberStyles, CultureInfo.InvariantCulture, out longValue))
                throw new FormatException();

            return new EvaluatedExpression(value, value, _stackFrame.GetVirtualMachine().GetMirrorOf(longValue), false);
        }

        private EvaluatedExpression GetFloatLiteral(string value)
        {
            Contract.Requires<ArgumentNullException>(value != null, "value");
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(value));
            Contract.Ensures(Contract.Result<EvaluatedExpression>() != null);

            string parseValue = value;
            if (parseValue.EndsWith("f", StringComparison.OrdinalIgnoreCase))
                parseValue = parseValue.Substring(0, parseValue.Length - 1);

            float floatValue;
            if (!float.TryParse(parseValue, NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out floatValue))
                throw new FormatException();

            return new EvaluatedExpression(value, value, _stackFrame.GetVirtualMachine().GetMirrorOf(floatValue), false);
        }

        private EvaluatedExpression GetDoubleLiteral(string value)
        {
            Contract.Requires<ArgumentNullException>(value != null, "value");
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(value));
            Contract.Ensures(Contract.Result<EvaluatedExpression>() != null);

            string parseValue = value;
            if (parseValue.EndsWith("d", StringComparison.OrdinalIgnoreCase))
                parseValue = parseValue.Substring(0, value.Length - 1);

            double doubleValue;
            if (!double.TryParse(parseValue, NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out doubleValue))
                throw new FormatException();

            return new EvaluatedExpression(value, value, _stackFrame.GetVirtualMachine().GetMirrorOf(doubleValue), false);
        }

        private EvaluatedExpression GetCharLiteral(string value)
        {
            Contract.Requires<ArgumentNullException>(value != null, "value");
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(value));
            Contract.Ensures(Contract.Result<EvaluatedExpression>() != null);

            char charValue;
            if (value.Length == 3)
            {
                charValue = value[1];
            }
            else if (value.Length == 4 && value[1] == '\\')
            {
                switch (value[2])
                {
                case 'b':
                    charValue = '\b';
                    break;

                case 't':
                    charValue = '\t';
                    break;

                case 'n':
                    charValue = '\n';
                    break;

                case 'f':
                    charValue = '\f';
                    break;

                case 'r':
                    charValue = '\r';
                    break;

                case '"':
                    charValue = '"';
                    break;

                case '\'':
                    charValue = '\'';
                    break;

                case '\\':
                    charValue = '\\';
                    break;

                default:
                    if (value[2] >= '0' && value[2] <= '7')
                    {
                        charValue = (char)(value[2] - '0');
                        break;
                    }

                    throw new FormatException();
                }
            }
            else if ((value.Length == 5 || value.Length == 6) && value[1] == '\\')
            {
                int x = value.Length == 6 ? value[value.Length - 4] - '0' : 0;
                int y = value[value.Length - 3] - '0';
                int z = value[value.Length - 2] - '0';
                charValue = (char)(x * 64 + y * 8 + z);
            }
            else
            {
                throw new FormatException();
            }

            return new EvaluatedExpression(value, value, _stackFrame.GetVirtualMachine().GetMirrorOf(charValue), false);
        }

        private EvaluatedExpression GetStringLiteral(string value)
        {
            Contract.Ensures(Contract.Result<EvaluatedExpression>() != null);

            if (value == null)
                return new EvaluatedExpression("(String)null", "(String)null", _stackFrame.GetVirtualMachine().GetClassesByName("java.lang.String").Single(), default(IValue), false);

            return new EvaluatedExpression(value, value, _stackFrame.GetVirtualMachine().GetMirrorOf(value.Substring(1, value.Length - 2)), false);
        }
    }
}
