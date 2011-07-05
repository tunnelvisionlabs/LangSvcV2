namespace Tvl.VisualStudio.Shell
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class ProvideDebuggerExceptionAttribute : RegistrationAttribute
    {
        private readonly Guid _debugEngine;
        private readonly string _exceptionKind;
        private readonly string _exceptionNamespace;
        private readonly string _exceptionName;
        private int _code;
        private int _state = 0x00004022;

        public ProvideDebuggerExceptionAttribute(string debugEngine, string exceptionKind, string exceptionName)
            : this(debugEngine, exceptionKind, null, exceptionName)
        {
        }

        public ProvideDebuggerExceptionAttribute(Type exception)
            : this(VSConstants.DebugEnginesGuids.ManagedOnly_string, "Common Language Runtime Exceptions", exception.Namespace, exception.FullName)
        {
        }

        public ProvideDebuggerExceptionAttribute(string debugEngine, string exceptionKind, string exceptionNamespace, string exceptionName)
        {
            if (debugEngine == null)
                throw new ArgumentNullException("debugEngine");
            if (exceptionKind == null)
                throw new ArgumentNullException("exceptionKind");
            if (exceptionName == null)
                throw new ArgumentNullException("exceptionName");

            _debugEngine = Guid.Parse(debugEngine);
            _exceptionKind = exceptionKind;
            _exceptionName = exceptionName;
            _exceptionNamespace = exceptionNamespace;
        }

        public Guid DebugEngine
        {
            get
            {
                return _debugEngine;
            }
        }

        public string ExceptionKind
        {
            get
            {
                return _exceptionKind;
            }
        }

        public string ExceptionNamespace
        {
            get
            {
                return _exceptionNamespace;
            }
        }

        public string ExceptionName
        {
            get
            {
                return _exceptionName;
            }
        }

        public int Code
        {
            get
            {
                return _code;
            }

            set
            {
                _code = value;
            }
        }

        public int State
        {
            get
            {
                return _state;
            }

            set
            {
                _state = value;
            }
        }

        private string RegKeyBaseName
        {
            get
            {
                return string.Format(@"AD7Metrics\Exception\{0:B}\{1}", DebugEngine, ExceptionKind);
            }
        }

        public override void Register(RegistrationContext context)
        {
            using (Key key = context.CreateKey(RegKeyBaseName))
            {
                Key parentKey = key;
                if (!string.IsNullOrEmpty(ExceptionNamespace))
                    parentKey = key.CreateSubkey(ExceptionNamespace);

                try
                {
                    if (!string.IsNullOrEmpty(ExceptionNamespace))
                    {
                        parentKey.SetValue("Code", 0);
                        parentKey.SetValue("State", 0x4022);
                    }

                    using (Key child = parentKey.CreateSubkey(ExceptionName))
                    {
                        child.SetValue("Code", Code);
                        child.SetValue("State", State);
                    }
                }
                finally
                {
                    if (!string.IsNullOrEmpty(ExceptionNamespace))
                        parentKey.Close();
                }
            }
        }

        public override void Unregister(RegistrationContext context)
        {
            string name;
            if (!string.IsNullOrEmpty(ExceptionNamespace))
                name = string.Format(@"{0}\{1}", ExceptionNamespace, ExceptionName);
            else
                name = ExceptionName;

            string regKeyName = string.Format(@"AD7Metrics\Exception\{0:B}", DebugEngine, ExceptionKind, name);
            context.RemoveKey(regKeyName);
        }
    }
}
