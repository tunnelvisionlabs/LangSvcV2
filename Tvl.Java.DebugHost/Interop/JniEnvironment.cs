namespace Tvl.Java.DebugHost.Interop
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Collections.Concurrent;
    using System.Runtime.InteropServices;

    public sealed class JniEnvironment
    {
        private static readonly ConcurrentDictionary<JNIEnvHandle, JniEnvironment> _instances =
            new ConcurrentDictionary<JNIEnvHandle, JniEnvironment>();

        private readonly JNIEnvHandle _handle;
        private readonly jniNativeInterface _rawInterface;

        public JniEnvironment(JNIEnvHandle handle)
        {
            _handle = handle;
            _rawInterface = (jniNativeInterface)Marshal.PtrToStructure(Marshal.ReadIntPtr(handle.Handle), typeof(jniNativeInterface));
        }

        internal jniNativeInterface RawInterface
        {
            get
            {
                return _rawInterface;
            }
        }

        public static implicit operator JNIEnvHandle(JniEnvironment jniEnvironment)
        {
            return jniEnvironment._handle;
        }

        internal static JniEnvironment GetOrCreateInstance(JNIEnvHandle handle)
        {
            bool created;
            return GetOrCreateInstance(handle, out created);
        }

        internal static JniEnvironment GetOrCreateInstance(JNIEnvHandle handle, out bool created)
        {
            bool wasCreated = false;
            JniEnvironment environment = _instances.GetOrAdd(handle,
                i =>
                {
                    wasCreated = true;
                    return CreateVirtualMachine(i);
                });

            created = wasCreated;
            return environment;
        }

        private static JniEnvironment CreateVirtualMachine(JNIEnvHandle handle)
        {
            return new JniEnvironment(handle);
        }

        public int GetVersion()
        {
            int version = RawInterface.GetVersion(this);
            RawInterface.ExceptionClear(this);
            return version;
        }

        public jclass DefineClass(string name, jobject loader, byte[] data)
        {
            return RawInterface.DefineClass(this, name, loader, data, data.Length);
        }

        public jclass FindClass(string name)
        {
            return RawInterface.FindClass(this, name);
        }

        public jclass GetObjectClass(jobject @object)
        {
            return RawInterface.GetObjectClass(this, @object);
        }

        public jclass GetSuperclass(jclass @class)
        {
            return RawInterface.GetSuperclass(this, @class);
        }

        public bool IsInstanceOf(jobject @object, jclass @class)
        {
            byte result = RawInterface.IsInstanceOf(this, @object, @class);
            return result != 0;
        }

        public void ExceptionClear()
        {
            RawInterface.ExceptionClear(this);
        }

        public jthrowable ExceptionOccurred()
        {
            return RawInterface.ExceptionOccurred(this);
        }

        public bool IsSameObject(jobject x, jobject y)
        {
            byte result = RawInterface.IsSameObject(this, x, y);
            RawInterface.ExceptionClear(this);
            return result != 0;
        }

        public jobject NewLocalReference(jobject @object)
        {
            jobject result = RawInterface.NewLocalRef(this, @object);
            RawInterface.ExceptionClear(this);
            return result;
        }

        public void DeleteLocalReference(jobject @object)
        {
            RawInterface.DeleteLocalRef(this, @object);
            RawInterface.ExceptionClear(this);
        }

        public jobject NewGlobalReference(jobject @object)
        {
            jobject result = RawInterface.NewGlobalRef(this, @object);
            RawInterface.ExceptionClear(this);
            return result;
        }

        public void DeleteGlobalReference(jobject @object)
        {
            RawInterface.DeleteGlobalRef(this, @object);
            RawInterface.ExceptionClear(this);
        }

        public jweak NewWeakGlobalReference(jobject @object)
        {
            jweak result = RawInterface.NewWeakGlobalRef(this, @object);
            RawInterface.ExceptionClear(this);
            return result;
        }

        public void DeleteWeakGlobalReference(jweak @object)
        {
            RawInterface.DeleteWeakGlobalRef(this, @object);
            RawInterface.ExceptionClear(this);
        }

        public int GetStringUTFLength(jobject stringHandle)
        {
            return RawInterface.GetStringUTFLength(this, stringHandle);
        }

        public void GetStringUTFRegion(jobject stringHandle, int start, int length, byte[] buffer)
        {
            RawInterface.GetStringUTFRegion(this, stringHandle, start, length, buffer);
        }

        public int GetArrayLength(jobject arrayHandle)
        {
            return RawInterface.GetArrayLength(this, arrayHandle);
        }

        public jobject NewString(string value)
        {
            if (value == null)
                return jobject.Null;

            return RawInterface.NewString(this, value, value.Length);
        }

        public jobject NewObject(jclass @class, jmethodID ctorMethodId, params jvalue[] args)
        {
            return RawInterface.NewObjectA(this, @class, ctorMethodId, args);
        }

        public jmethodID GetMethodId(jclass @class, string name, string signature)
        {
            return RawInterface.GetMethodID(this, @class, name, signature);
        }

        internal bool GetStaticBooleanField(jclass classHandle, jfieldID fieldId)
        {
            return RawInterface.GetStaticBooleanField(this, classHandle, fieldId) != 0;
        }

        internal byte GetStaticByteField(jclass classHandle, jfieldID fieldId)
        {
            return RawInterface.GetStaticByteField(this, classHandle, fieldId);
        }

        internal char GetStaticCharField(jclass classHandle, jfieldID fieldId)
        {
            return RawInterface.GetStaticCharField(this, classHandle, fieldId);
        }

        internal double GetStaticDoubleField(jclass classHandle, jfieldID fieldId)
        {
            return RawInterface.GetStaticDoubleField(this, classHandle, fieldId);
        }

        internal float GetStaticFloatField(jclass classHandle, jfieldID fieldId)
        {
            return RawInterface.GetStaticFloatField(this, classHandle, fieldId);
        }

        internal int GetStaticIntField(jclass classHandle, jfieldID fieldId)
        {
            return RawInterface.GetStaticIntField(this, classHandle, fieldId);
        }

        internal long GetStaticLongField(jclass classHandle, jfieldID fieldId)
        {
            return RawInterface.GetStaticLongField(this, classHandle, fieldId);
        }

        internal short GetStaticShortField(jclass classHandle, jfieldID fieldId)
        {
            return RawInterface.GetStaticShortField(this, classHandle, fieldId);
        }

        internal jobject GetStaticObjectField(jclass classHandle, jfieldID fieldId)
        {
            return RawInterface.GetStaticObjectField(this, classHandle, fieldId);
        }

        internal bool GetBooleanField(jobject objectHandle, jfieldID fieldId)
        {
            return RawInterface.GetBooleanField(this, objectHandle, fieldId) != 0;
        }

        internal byte GetByteField(jobject objectHandle, jfieldID fieldId)
        {
            return RawInterface.GetByteField(this, objectHandle, fieldId);
        }

        internal char GetCharField(jobject objectHandle, jfieldID fieldId)
        {
            return RawInterface.GetCharField(this, objectHandle, fieldId);
        }

        internal double GetDoubleField(jobject objectHandle, jfieldID fieldId)
        {
            return RawInterface.GetDoubleField(this, objectHandle, fieldId);
        }

        internal float GetFloatField(jobject objectHandle, jfieldID fieldId)
        {
            return RawInterface.GetFloatField(this, objectHandle, fieldId);
        }

        internal int GetIntField(jobject objectHandle, jfieldID fieldId)
        {
            return RawInterface.GetIntField(this, objectHandle, fieldId);
        }

        internal long GetLongField(jobject objectHandle, jfieldID fieldId)
        {
            return RawInterface.GetLongField(this, objectHandle, fieldId);
        }

        internal short GetShortField(jobject objectHandle, jfieldID fieldId)
        {
            return RawInterface.GetShortField(this, objectHandle, fieldId);
        }

        internal jobject GetObjectField(jobject objectHandle, jfieldID fieldId)
        {
            return RawInterface.GetObjectField(this, objectHandle, fieldId);
        }

        internal bool CallStaticBooleanMethodA(jclass classHandle, jmethodID methodId, params jvalue[] args)
        {
            return RawInterface.CallStaticBooleanMethodA(this, classHandle, methodId, args) != 0;
        }

        internal byte CallStaticByteMethodA(jclass classHandle, jmethodID methodId, params jvalue[] args)
        {
            return RawInterface.CallStaticByteMethodA(this, classHandle, methodId, args);
        }

        internal char CallStaticCharMethodA(jclass classHandle, jmethodID methodId, params jvalue[] args)
        {
            return RawInterface.CallStaticCharMethodA(this, classHandle, methodId, args);
        }

        internal double CallStaticDoubleMethodA(jclass classHandle, jmethodID methodId, params jvalue[] args)
        {
            return RawInterface.CallStaticDoubleMethodA(this, classHandle, methodId, args);
        }

        internal float CallStaticFloatMethodA(jclass classHandle, jmethodID methodId, params jvalue[] args)
        {
            return RawInterface.CallStaticFloatMethodA(this, classHandle, methodId, args);
        }

        internal int CallStaticIntMethodA(jclass classHandle, jmethodID methodId, params jvalue[] args)
        {
            return RawInterface.CallStaticIntMethodA(this, classHandle, methodId, args);
        }

        internal long CallStaticLongMethodA(jclass classHandle, jmethodID methodId, params jvalue[] args)
        {
            return RawInterface.CallStaticLongMethodA(this, classHandle, methodId, args);
        }

        internal short CallStaticShortMethodA(jclass classHandle, jmethodID methodId, params jvalue[] args)
        {
            return RawInterface.CallStaticShortMethodA(this, classHandle, methodId, args);
        }

        internal jobject CallStaticObjectMethodA(jclass classHandle, jmethodID methodId, params jvalue[] args)
        {
            return RawInterface.CallStaticObjectMethodA(this, classHandle, methodId, args);
        }

        internal bool CallBooleanMethodA(jobject instanceHandle, jmethodID methodId, params jvalue[] args)
        {
            return RawInterface.CallBooleanMethodA(this, instanceHandle, methodId, args) != 0;
        }

        internal byte CallByteMethodA(jobject instanceHandle, jmethodID methodId, params jvalue[] args)
        {
            return RawInterface.CallByteMethodA(this, instanceHandle, methodId, args);
        }

        internal char CallCharMethodA(jobject instanceHandle, jmethodID methodId, params jvalue[] args)
        {
            return RawInterface.CallCharMethodA(this, instanceHandle, methodId, args);
        }

        internal double CallDoubleMethodA(jobject instanceHandle, jmethodID methodId, params jvalue[] args)
        {
            return RawInterface.CallDoubleMethodA(this, instanceHandle, methodId, args);
        }

        internal float CallFloatMethodA(jobject instanceHandle, jmethodID methodId, params jvalue[] args)
        {
            return RawInterface.CallFloatMethodA(this, instanceHandle, methodId, args);
        }

        internal int CallIntMethodA(jobject instanceHandle, jmethodID methodId, params jvalue[] args)
        {
            return RawInterface.CallIntMethodA(this, instanceHandle, methodId, args);
        }

        internal long CallLongMethodA(jobject instanceHandle, jmethodID methodId, params jvalue[] args)
        {
            return RawInterface.CallLongMethodA(this, instanceHandle, methodId, args);
        }

        internal short CallShortMethodA(jobject instanceHandle, jmethodID methodId, params jvalue[] args)
        {
            return RawInterface.CallShortMethodA(this, instanceHandle, methodId, args);
        }

        internal jobject CallObjectMethodA(jobject instanceHandle, jmethodID methodId, params jvalue[] args)
        {
            return RawInterface.CallObjectMethodA(this, instanceHandle, methodId, args);
        }

        internal bool CallNonvirtualBooleanMethodA(jobject instanceHandle, jclass classHandle, jmethodID methodId, params jvalue[] args)
        {
            return RawInterface.CallNonvirtualBooleanMethodA(this, instanceHandle, classHandle, methodId, args) != 0;
        }

        internal byte CallNonvirtualByteMethodA(jobject instanceHandle, jclass classHandle, jmethodID methodId, params jvalue[] args)
        {
            return RawInterface.CallNonvirtualByteMethodA(this, instanceHandle, classHandle, methodId, args);
        }

        internal char CallNonvirtualCharMethodA(jobject instanceHandle, jclass classHandle, jmethodID methodId, params jvalue[] args)
        {
            return RawInterface.CallNonvirtualCharMethodA(this, instanceHandle, classHandle, methodId, args);
        }

        internal double CallNonvirtualDoubleMethodA(jobject instanceHandle, jclass classHandle, jmethodID methodId, params jvalue[] args)
        {
            return RawInterface.CallNonvirtualDoubleMethodA(this, instanceHandle, classHandle, methodId, args);
        }

        internal float CallNonvirtualFloatMethodA(jobject instanceHandle, jclass classHandle, jmethodID methodId, params jvalue[] args)
        {
            return RawInterface.CallNonvirtualFloatMethodA(this, instanceHandle, classHandle, methodId, args);
        }

        internal int CallNonvirtualIntMethodA(jobject instanceHandle, jclass classHandle, jmethodID methodId, params jvalue[] args)
        {
            return RawInterface.CallNonvirtualIntMethodA(this, instanceHandle, classHandle, methodId, args);
        }

        internal long CallNonvirtualLongMethodA(jobject instanceHandle, jclass classHandle, jmethodID methodId, params jvalue[] args)
        {
            return RawInterface.CallNonvirtualLongMethodA(this, instanceHandle, classHandle, methodId, args);
        }

        internal short CallNonvirtualShortMethodA(jobject instanceHandle, jclass classHandle, jmethodID methodId, params jvalue[] args)
        {
            return RawInterface.CallNonvirtualShortMethodA(this, instanceHandle, classHandle, methodId, args);
        }

        internal jobject CallNonvirtualObjectMethodA(jobject instanceHandle, jclass classHandle, jmethodID methodId, params jvalue[] args)
        {
            return RawInterface.CallNonvirtualObjectMethodA(this, instanceHandle, classHandle, methodId, args);
        }

        internal void GetBooleanArrayRegion(jobject arrayHandle, int start, int len, bool[] buf)
        {
            RawInterface.GetBooleanArrayRegion(this, arrayHandle, start, len, buf);
        }

        internal void GetByteArrayRegion(jobject arrayHandle, int start, int len, byte[] buf)
        {
            RawInterface.GetByteArrayRegion(this, arrayHandle, start, len, buf);
        }

        internal void GetCharArrayRegion(jobject arrayHandle, int start, int len, char[] buf)
        {
            RawInterface.GetCharArrayRegion(this, arrayHandle, start, len, buf);
        }

        internal void GetDoubleArrayRegion(jobject arrayHandle, int start, int len, double[] buf)
        {
            RawInterface.GetDoubleArrayRegion(this, arrayHandle, start, len, buf);
        }

        internal void GetFloatArrayRegion(jobject arrayHandle, int start, int len, float[] buf)
        {
            RawInterface.GetFloatArrayRegion(this, arrayHandle, start, len, buf);
        }

        internal void GetIntArrayRegion(jobject arrayHandle, int start, int len, int[] buf)
        {
            RawInterface.GetIntArrayRegion(this, arrayHandle, start, len, buf);
        }

        internal void GetLongArrayRegion(jobject arrayHandle, int start, int len, long[] buf)
        {
            RawInterface.GetLongArrayRegion(this, arrayHandle, start, len, buf);
        }

        internal void GetShortArrayRegion(jobject arrayHandle, int start, int len, short[] buf)
        {
            RawInterface.GetShortArrayRegion(this, arrayHandle, start, len, buf);
        }

        internal jobject GetObjectArrayElement(jobject arrayHandle, int index)
        {
            return RawInterface.GetObjectArrayElement(this, arrayHandle, index);
        }

        internal jobject NewBooleanArray(int length)
        {
            return RawInterface.NewBooleanArray(this, length);
        }

        internal jobject NewByteArray(int length)
        {
            return RawInterface.NewByteArray(this, length);
        }

        internal jobject NewCharArray(int length)
        {
            return RawInterface.NewCharArray(this, length);
        }

        internal jobject NewDoubleArray(int length)
        {
            return RawInterface.NewDoubleArray(this, length);
        }

        internal jobject NewFloatArray(int length)
        {
            return RawInterface.NewFloatArray(this, length);
        }

        internal jobject NewIntegerArray(int length)
        {
            return RawInterface.NewIntArray(this, length);
        }

        internal jobject NewLongArray(int length)
        {
            return RawInterface.NewLongArray(this, length);
        }

        internal jobject NewShortArray(int length)
        {
            return RawInterface.NewShortArray(this, length);
        }

        internal jobject NewObjectArray(int length, jclass elementType, jobject initialElement)
        {
            return RawInterface.NewObjectArray(this, length, elementType, initialElement);
        }
    }
}
