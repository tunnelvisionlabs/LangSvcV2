namespace Tvl.Java.DebugHost
{
    using Tvl.Java.DebugHost.Interop;
    using System.Collections.ObjectModel;
    using System;

    public class JvmMethod
    {
        private readonly JvmEnvironment _environment;
        private readonly jmethodID _methodId;

        internal JvmMethod(JvmEnvironment environment, jmethodID methodId)
        {
            _environment = environment;
            _methodId = methodId;
        }

        internal jmethodID MethodId
        {
            get
            {
                return _methodId;
            }
        }

        public string GetName()
        {
            string name;
            string signature;
            string generic;
            _environment.GetMethodName(this, out name, out signature, out generic);
            return name;
        }

        public string GetSignature()
        {
            string name;
            string signature;
            string generic;
            _environment.GetMethodName(this, out name, out signature, out generic);
            return signature;
        }

        public bool IsAbstract()
        {
            return (GetModifiers() & JvmAccessModifiers.Abstract) != 0;
        }

        public bool IsBridge()
        {
            throw new NotImplementedException();
        }

        public bool IsConstructor()
        {
            throw new NotImplementedException();
        }

        public bool IsNative()
        {
            return (GetModifiers() & JvmAccessModifiers.Native) != 0;
        }

        public bool IsObsolete()
        {
            return _environment.IsMethodObsolete(this);
        }

        public bool IsStaticInitializer()
        {
            throw new NotImplementedException();
        }

        public bool IsSynchronized()
        {
            return (GetModifiers() & JvmAccessModifiers.Synchronized) != 0;
        }

        public bool IsVarArgs()
        {
            throw new NotImplementedException();
        }

        public JvmLocation GetLocation()
        {
            throw new NotImplementedException();
        }

        public JvmLocation GetLocationOfCodeIndex()
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<JvmLocation> GetLocationsOfLine()
        {
            throw new NotImplementedException();
        }

        private JvmAccessModifiers GetModifiers()
        {
            return _environment.GetMethodModifiers(this);
        }
    }
}
