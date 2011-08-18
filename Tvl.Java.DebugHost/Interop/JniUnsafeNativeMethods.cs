namespace Tvl.Java.DebugHost.Interop
{
    using System.Runtime.InteropServices;
    using jint = System.Int32;
    using jboolean = System.Byte;
    using jbyte = System.Byte;
    using jsize = System.Int32;
    using jvalue = System.Int64;
    using va_list = System.IntPtr;
    using jchar = System.Char;
    using jshort = System.Int16;
    using jlong = System.Int64;
    using jfloat = System.Single;
    using jdouble = System.Double;
    using jstring = System.IntPtr;
    using jarray = System.IntPtr;
    using jobjectArray = System.IntPtr;
    using jbooleanArray = System.IntPtr;
    using jintArray = System.IntPtr;
    using jlongArray = System.IntPtr;
    using jfloatArray = System.IntPtr;
    using jdoubleArray = System.IntPtr;
    using jcharArray = System.IntPtr;
    using jshortArray = System.IntPtr;
    using jbyteArray = System.IntPtr;
    using jweak = System.IntPtr;
    using IntPtr = System.IntPtr;

    internal static class JniUnsafeNativeMethods
    {
        public delegate jint GetVersion(JNIEnvHandle env);

        public delegate jclass DefineClass(JNIEnvHandle env, [MarshalAs(UnmanagedType.LPStr)]string name, jobject loader, jbyte[] buf, jsize len);
        public delegate jclass FindClass(JNIEnvHandle env, [MarshalAs(UnmanagedType.LPStr)]string name);

        public delegate jmethodID FromReflectedMethod(JNIEnvHandle env, jobject method);
        public delegate jfieldID FromReflectedField(JNIEnvHandle env, jobject field);

        public delegate jobject ToReflectedMethod(JNIEnvHandle env, jclass cls, jmethodID methodID, jboolean isStatic);

        public delegate jclass GetSuperclass(JNIEnvHandle env, jclass sub);
        public delegate jboolean IsAssignableFrom(JNIEnvHandle env, jclass sub, jclass sup);

        public delegate jobject ToReflectedField(JNIEnvHandle env, jclass cls, jfieldID fieldID, jboolean isStatic);

        public delegate jint Throw(JNIEnvHandle env, jthrowable obj);
        public delegate jint ThrowNew(JNIEnvHandle env, jclass clazz, [MarshalAs(UnmanagedType.LPStr)]string msg);
        public delegate jthrowable ExceptionOccurred(JNIEnvHandle env);
        public delegate void ExceptionDescribe(JNIEnvHandle env);
        public delegate void ExceptionClear(JNIEnvHandle env);
        public delegate void FatalError(JNIEnvHandle env, [MarshalAs(UnmanagedType.LPStr)]string msg);

        public delegate jint PushLocalFrame(JNIEnvHandle env, jint capacity);
        public delegate jobject PopLocalFrame(JNIEnvHandle env, jobject result);

        public delegate jobject NewGlobalRef(JNIEnvHandle env, jobject lobj);
        public delegate void DeleteGlobalRef(JNIEnvHandle env, jobject gref);
        public delegate void DeleteLocalRef(JNIEnvHandle env, jobject obj);
        public delegate jboolean IsSameObject(JNIEnvHandle env, jobject obj1, jobject obj2);
        public delegate jobject NewLocalRef(JNIEnvHandle env, jobject @ref);
        public delegate jint EnsureLocalCapacity(JNIEnvHandle env, jint capacity);

        public delegate jobject AllocObject(JNIEnvHandle env, jclass clazz);
        public delegate jobject NewObject(JNIEnvHandle env, jclass clazz, jmethodID methodID/*, __arglist*/);
        public delegate jobject NewObjectV(JNIEnvHandle env, jclass clazz, jmethodID methodID, va_list args);
        public delegate jobject NewObjectA(JNIEnvHandle env, jclass clazz, jmethodID methodID, [MarshalAs(UnmanagedType.LPArray)]jvalue[] args);

        public delegate jclass GetObjectClass(JNIEnvHandle env, jobject obj);
        public delegate jboolean IsInstanceOf(JNIEnvHandle env, jobject obj, jclass clazz);

        public delegate jmethodID GetMethodID(JNIEnvHandle env, jclass clazz, [MarshalAs(UnmanagedType.LPStr)]string name, [MarshalAs(UnmanagedType.LPStr)]string sig);

        public delegate jobject CallObjectMethod(JNIEnvHandle env, jobject obj, jmethodID methodID/*, __arglist*/);

        public delegate jobject CallObjectMethodV(JNIEnvHandle env, jobject obj, jmethodID methodID, va_list args);
        public delegate jobject CallObjectMethodA(JNIEnvHandle env, jobject obj, jmethodID methodID, jvalue[] args);

        public delegate jboolean CallBooleanMethod(JNIEnvHandle env, jobject obj, jmethodID methodID/*, __arglist*/);
        public delegate jboolean CallBooleanMethodV(JNIEnvHandle env, jobject obj, jmethodID methodID, va_list args);
        public delegate jboolean CallBooleanMethodA(JNIEnvHandle env, jobject obj, jmethodID methodID, jvalue[] args);

        public delegate jbyte CallByteMethod(JNIEnvHandle env, jobject obj, jmethodID methodID/*, __arglist*/);
        public delegate jbyte CallByteMethodV(JNIEnvHandle env, jobject obj, jmethodID methodID, va_list args);
        public delegate jbyte CallByteMethodA(JNIEnvHandle env, jobject obj, jmethodID methodID, [MarshalAs(UnmanagedType.LPArray)]jvalue[] args);

        public delegate jchar CallCharMethod(JNIEnvHandle env, jobject obj, jmethodID methodID/*, __arglist*/);
        public delegate jchar CallCharMethodV(JNIEnvHandle env, jobject obj, jmethodID methodID, va_list args);
        public delegate jchar CallCharMethodA(JNIEnvHandle env, jobject obj, jmethodID methodID, [MarshalAs(UnmanagedType.LPArray)]jvalue[] args);

        public delegate jshort CallShortMethod(JNIEnvHandle env, jobject obj, jmethodID methodID/*, __arglist*/);
        public delegate jshort CallShortMethodV(JNIEnvHandle env, jobject obj, jmethodID methodID, va_list args);
        public delegate jshort CallShortMethodA(JNIEnvHandle env, jobject obj, jmethodID methodID, [MarshalAs(UnmanagedType.LPArray)]jvalue[] args);

        public delegate jint CallIntMethod(JNIEnvHandle env, jobject obj, jmethodID methodID/*, __arglist*/);
        public delegate jint CallIntMethodV(JNIEnvHandle env, jobject obj, jmethodID methodID, va_list args);
        public delegate jint CallIntMethodA(JNIEnvHandle env, jobject obj, jmethodID methodID, [MarshalAs(UnmanagedType.LPArray)]jvalue[] args);

        public delegate jlong CallLongMethod(JNIEnvHandle env, jobject obj, jmethodID methodID/*, __arglist*/);
        public delegate jlong CallLongMethodV(JNIEnvHandle env, jobject obj, jmethodID methodID, va_list args);
        public delegate jlong CallLongMethodA(JNIEnvHandle env, jobject obj, jmethodID methodID, [MarshalAs(UnmanagedType.LPArray)]jvalue[] args);

        public delegate jfloat CallFloatMethod(JNIEnvHandle env, jobject obj, jmethodID methodID/*, __arglist*/);
        public delegate jfloat CallFloatMethodV(JNIEnvHandle env, jobject obj, jmethodID methodID, va_list args);
        public delegate jfloat CallFloatMethodA(JNIEnvHandle env, jobject obj, jmethodID methodID, [MarshalAs(UnmanagedType.LPArray)]jvalue[] args);

        public delegate jdouble CallDoubleMethod(JNIEnvHandle env, jobject obj, jmethodID methodID/*, __arglist*/);
        public delegate jdouble CallDoubleMethodV(JNIEnvHandle env, jobject obj, jmethodID methodID, va_list args);
        public delegate jdouble CallDoubleMethodA(JNIEnvHandle env, jobject obj, jmethodID methodID, [MarshalAs(UnmanagedType.LPArray)]jvalue[] args);

        public delegate void CallVoidMethod(JNIEnvHandle env, jobject obj, jmethodID methodID/*, __arglist*/);
        public delegate void CallVoidMethodV(JNIEnvHandle env, jobject obj, jmethodID methodID, va_list args);
        public delegate void CallVoidMethodA(JNIEnvHandle env, jobject obj, jmethodID methodID, [MarshalAs(UnmanagedType.LPArray)]jvalue[] args);

        public delegate jobject CallNonvirtualObjectMethod(JNIEnvHandle env, jobject obj, jclass clazz, jmethodID methodID/*, __arglist*/);
        public delegate jobject CallNonvirtualObjectMethodV(JNIEnvHandle env, jobject obj, jclass clazz, jmethodID methodID, va_list args);
        public delegate jobject CallNonvirtualObjectMethodA(JNIEnvHandle env, jobject obj, jclass clazz, jmethodID methodID, [MarshalAs(UnmanagedType.LPArray)]jvalue[] args);

        public delegate jboolean CallNonvirtualBooleanMethod(JNIEnvHandle env, jobject obj, jclass clazz, jmethodID methodID/*, __arglist*/);
        public delegate jboolean CallNonvirtualBooleanMethodV(JNIEnvHandle env, jobject obj, jclass clazz, jmethodID methodID, va_list args);
        public delegate jboolean CallNonvirtualBooleanMethodA(JNIEnvHandle env, jobject obj, jclass clazz, jmethodID methodID, [MarshalAs(UnmanagedType.LPArray)]jvalue[] args);

        public delegate jbyte CallNonvirtualByteMethod(JNIEnvHandle env, jobject obj, jclass clazz, jmethodID methodID/*, __arglist*/);
        public delegate jbyte CallNonvirtualByteMethodV(JNIEnvHandle env, jobject obj, jclass clazz, jmethodID methodID, va_list args);
        public delegate jbyte CallNonvirtualByteMethodA(JNIEnvHandle env, jobject obj, jclass clazz, jmethodID methodID, [MarshalAs(UnmanagedType.LPArray)]jvalue[] args);

        public delegate jchar CallNonvirtualCharMethod(JNIEnvHandle env, jobject obj, jclass clazz, jmethodID methodID/*, __arglist*/);
        public delegate jchar CallNonvirtualCharMethodV(JNIEnvHandle env, jobject obj, jclass clazz, jmethodID methodID, va_list args);
        public delegate jchar CallNonvirtualCharMethodA(JNIEnvHandle env, jobject obj, jclass clazz, jmethodID methodID, [MarshalAs(UnmanagedType.LPArray)]jvalue[] args);

        public delegate jshort CallNonvirtualShortMethod(JNIEnvHandle env, jobject obj, jclass clazz, jmethodID methodID/*, __arglist*/);
        public delegate jshort CallNonvirtualShortMethodV(JNIEnvHandle env, jobject obj, jclass clazz, jmethodID methodID, va_list args);
        public delegate jshort CallNonvirtualShortMethodA(JNIEnvHandle env, jobject obj, jclass clazz, jmethodID methodID, [MarshalAs(UnmanagedType.LPArray)]jvalue[] args);

        public delegate jint CallNonvirtualIntMethod(JNIEnvHandle env, jobject obj, jclass clazz, jmethodID methodID/*, __arglist*/);
        public delegate jint CallNonvirtualIntMethodV(JNIEnvHandle env, jobject obj, jclass clazz, jmethodID methodID, va_list args);
        public delegate jint CallNonvirtualIntMethodA(JNIEnvHandle env, jobject obj, jclass clazz, jmethodID methodID, [MarshalAs(UnmanagedType.LPArray)]jvalue[] args);

        public delegate jlong CallNonvirtualLongMethod(JNIEnvHandle env, jobject obj, jclass clazz, jmethodID methodID/*, __arglist*/);
        public delegate jlong CallNonvirtualLongMethodV(JNIEnvHandle env, jobject obj, jclass clazz, jmethodID methodID, va_list args);
        public delegate jlong CallNonvirtualLongMethodA(JNIEnvHandle env, jobject obj, jclass clazz, jmethodID methodID, [MarshalAs(UnmanagedType.LPArray)]jvalue[] args);

        public delegate jfloat CallNonvirtualFloatMethod(JNIEnvHandle env, jobject obj, jclass clazz, jmethodID methodID/*, __arglist*/);
        public delegate jfloat CallNonvirtualFloatMethodV(JNIEnvHandle env, jobject obj, jclass clazz, jmethodID methodID, va_list args);
        public delegate jfloat CallNonvirtualFloatMethodA(JNIEnvHandle env, jobject obj, jclass clazz, jmethodID methodID, [MarshalAs(UnmanagedType.LPArray)]jvalue[] args);

        public delegate jdouble CallNonvirtualDoubleMethod(JNIEnvHandle env, jobject obj, jclass clazz, jmethodID methodID/*, __arglist*/);
        public delegate jdouble CallNonvirtualDoubleMethodV(JNIEnvHandle env, jobject obj, jclass clazz, jmethodID methodID, va_list args);
        public delegate jdouble CallNonvirtualDoubleMethodA(JNIEnvHandle env, jobject obj, jclass clazz, jmethodID methodID, [MarshalAs(UnmanagedType.LPArray)]jvalue[] args);

        public delegate void CallNonvirtualVoidMethod(JNIEnvHandle env, jobject obj, jclass clazz, jmethodID methodID/*, __arglist*/);
        public delegate void CallNonvirtualVoidMethodV(JNIEnvHandle env, jobject obj, jclass clazz, jmethodID methodID, va_list args);
        public delegate void CallNonvirtualVoidMethodA(JNIEnvHandle env, jobject obj, jclass clazz, jmethodID methodID, [MarshalAs(UnmanagedType.LPArray)]jvalue[] args);

        public delegate jfieldID GetFieldID(JNIEnvHandle env, jclass clazz, [MarshalAs(UnmanagedType.LPStr)]string name, [MarshalAs(UnmanagedType.LPStr)]string sig);

        public delegate jobject GetObjectField(JNIEnvHandle env, jobject obj, jfieldID fieldID);
        public delegate jboolean GetBooleanField(JNIEnvHandle env, jobject obj, jfieldID fieldID);
        public delegate jbyte GetByteField(JNIEnvHandle env, jobject obj, jfieldID fieldID);
        public delegate jchar GetCharField(JNIEnvHandle env, jobject obj, jfieldID fieldID);
        public delegate jshort GetShortField(JNIEnvHandle env, jobject obj, jfieldID fieldID);
        public delegate jint GetIntField(JNIEnvHandle env, jobject obj, jfieldID fieldID);
        public delegate jlong GetLongField(JNIEnvHandle env, jobject obj, jfieldID fieldID);
        public delegate jfloat GetFloatField(JNIEnvHandle env, jobject obj, jfieldID fieldID);
        public delegate jdouble GetDoubleField(JNIEnvHandle env, jobject obj, jfieldID fieldID);

        public delegate void SetObjectField(JNIEnvHandle env, jobject obj, jfieldID fieldID, jobject val);
        public delegate void SetBooleanField(JNIEnvHandle env, jobject obj, jfieldID fieldID, jboolean val);
        public delegate void SetByteField(JNIEnvHandle env, jobject obj, jfieldID fieldID, jbyte val);
        public delegate void SetCharField(JNIEnvHandle env, jobject obj, jfieldID fieldID, jchar val);
        public delegate void SetShortField(JNIEnvHandle env, jobject obj, jfieldID fieldID, jshort val);
        public delegate void SetIntField(JNIEnvHandle env, jobject obj, jfieldID fieldID, jint val);
        public delegate void SetLongField(JNIEnvHandle env, jobject obj, jfieldID fieldID, jlong val);
        public delegate void SetFloatField(JNIEnvHandle env, jobject obj, jfieldID fieldID, jfloat val);
        public delegate void SetDoubleField(JNIEnvHandle env, jobject obj, jfieldID fieldID, jdouble val);

        public delegate jmethodID GetStaticMethodID(JNIEnvHandle env, jclass clazz, [MarshalAs(UnmanagedType.LPStr)]string name, [MarshalAs(UnmanagedType.LPStr)]string sig);

        public delegate jobject CallStaticObjectMethod(JNIEnvHandle env, jclass clazz, jmethodID methodID/*, __arglist*/);
        public delegate jobject CallStaticObjectMethodV(JNIEnvHandle env, jclass clazz, jmethodID methodID, va_list args);
        public delegate jobject CallStaticObjectMethodA(JNIEnvHandle env, jclass clazz, jmethodID methodID, [MarshalAs(UnmanagedType.LPArray)]jvalue[] args);

        public delegate jboolean CallStaticBooleanMethod(JNIEnvHandle env, jclass clazz, jmethodID methodID/*, __arglist*/);
        public delegate jboolean CallStaticBooleanMethodV(JNIEnvHandle env, jclass clazz, jmethodID methodID, va_list args);
        public delegate jboolean CallStaticBooleanMethodA(JNIEnvHandle env, jclass clazz, jmethodID methodID, [MarshalAs(UnmanagedType.LPArray)]jvalue[] args);

        public delegate jbyte CallStaticByteMethod(JNIEnvHandle env, jclass clazz, jmethodID methodID/*, __arglist*/);
        public delegate jbyte CallStaticByteMethodV(JNIEnvHandle env, jclass clazz, jmethodID methodID, va_list args);
        public delegate jbyte CallStaticByteMethodA(JNIEnvHandle env, jclass clazz, jmethodID methodID, [MarshalAs(UnmanagedType.LPArray)]jvalue[] args);

        public delegate jchar CallStaticCharMethod(JNIEnvHandle env, jclass clazz, jmethodID methodID/*, __arglist*/);
        public delegate jchar CallStaticCharMethodV(JNIEnvHandle env, jclass clazz, jmethodID methodID, va_list args);
        public delegate jchar CallStaticCharMethodA(JNIEnvHandle env, jclass clazz, jmethodID methodID, [MarshalAs(UnmanagedType.LPArray)]jvalue[] args);

        public delegate jshort CallStaticShortMethod(JNIEnvHandle env, jclass clazz, jmethodID methodID/*, __arglist*/);
        public delegate jshort CallStaticShortMethodV(JNIEnvHandle env, jclass clazz, jmethodID methodID, va_list args);
        public delegate jshort CallStaticShortMethodA(JNIEnvHandle env, jclass clazz, jmethodID methodID, [MarshalAs(UnmanagedType.LPArray)]jvalue[] args);

        public delegate jint CallStaticIntMethod(JNIEnvHandle env, jclass clazz, jmethodID methodID/*, __arglist*/);
        public delegate jint CallStaticIntMethodV(JNIEnvHandle env, jclass clazz, jmethodID methodID, va_list args);
        public delegate jint CallStaticIntMethodA(JNIEnvHandle env, jclass clazz, jmethodID methodID, [MarshalAs(UnmanagedType.LPArray)]jvalue[] args);

        public delegate jlong CallStaticLongMethod(JNIEnvHandle env, jclass clazz, jmethodID methodID/*, __arglist*/);
        public delegate jlong CallStaticLongMethodV(JNIEnvHandle env, jclass clazz, jmethodID methodID, va_list args);
        public delegate jlong CallStaticLongMethodA(JNIEnvHandle env, jclass clazz, jmethodID methodID, [MarshalAs(UnmanagedType.LPArray)]jvalue[] args);

        public delegate jfloat CallStaticFloatMethod(JNIEnvHandle env, jclass clazz, jmethodID methodID/*, __arglist*/);
        public delegate jfloat CallStaticFloatMethodV(JNIEnvHandle env, jclass clazz, jmethodID methodID, va_list args);
        public delegate jfloat CallStaticFloatMethodA(JNIEnvHandle env, jclass clazz, jmethodID methodID, [MarshalAs(UnmanagedType.LPArray)]jvalue[] args);

        public delegate jdouble CallStaticDoubleMethod(JNIEnvHandle env, jclass clazz, jmethodID methodID/*, __arglist*/);
        public delegate jdouble CallStaticDoubleMethodV(JNIEnvHandle env, jclass clazz, jmethodID methodID, va_list args);
        public delegate jdouble CallStaticDoubleMethodA(JNIEnvHandle env, jclass clazz, jmethodID methodID, [MarshalAs(UnmanagedType.LPArray)]jvalue[] args);

        public delegate void CallStaticVoidMethod(JNIEnvHandle env, jclass cls, jmethodID methodID/*, __arglist*/);
        public delegate void CallStaticVoidMethodV(JNIEnvHandle env, jclass cls, jmethodID methodID, va_list args);
        public delegate void CallStaticVoidMethodA(JNIEnvHandle env, jclass cls, jmethodID methodID, [MarshalAs(UnmanagedType.LPArray)]jvalue[] args);

        public delegate jfieldID GetStaticFieldID(JNIEnvHandle env, jclass clazz, [MarshalAs(UnmanagedType.LPStr)]string name, [MarshalAs(UnmanagedType.LPStr)]string sig);
        public delegate jobject GetStaticObjectField(JNIEnvHandle env, jclass clazz, jfieldID fieldID);
        public delegate jboolean GetStaticBooleanField(JNIEnvHandle env, jclass clazz, jfieldID fieldID);
        public delegate jbyte GetStaticByteField(JNIEnvHandle env, jclass clazz, jfieldID fieldID);
        public delegate jchar GetStaticCharField(JNIEnvHandle env, jclass clazz, jfieldID fieldID);
        public delegate jshort GetStaticShortField(JNIEnvHandle env, jclass clazz, jfieldID fieldID);
        public delegate jint GetStaticIntField(JNIEnvHandle env, jclass clazz, jfieldID fieldID);
        public delegate jlong GetStaticLongField(JNIEnvHandle env, jclass clazz, jfieldID fieldID);
        public delegate jfloat GetStaticFloatField(JNIEnvHandle env, jclass clazz, jfieldID fieldID);
        public delegate jdouble GetStaticDoubleField(JNIEnvHandle env, jclass clazz, jfieldID fieldID);

        public delegate void SetStaticObjectField(JNIEnvHandle env, jclass clazz, jfieldID fieldID, jobject value);
        public delegate void SetStaticBooleanField(JNIEnvHandle env, jclass clazz, jfieldID fieldID, jboolean value);
        public delegate void SetStaticByteField(JNIEnvHandle env, jclass clazz, jfieldID fieldID, jbyte value);
        public delegate void SetStaticCharField(JNIEnvHandle env, jclass clazz, jfieldID fieldID, jchar value);
        public delegate void SetStaticShortField(JNIEnvHandle env, jclass clazz, jfieldID fieldID, jshort value);
        public delegate void SetStaticIntField(JNIEnvHandle env, jclass clazz, jfieldID fieldID, jint value);
        public delegate void SetStaticLongField(JNIEnvHandle env, jclass clazz, jfieldID fieldID, jlong value);
        public delegate void SetStaticFloatField(JNIEnvHandle env, jclass clazz, jfieldID fieldID, jfloat value);
        public delegate void SetStaticDoubleField(JNIEnvHandle env, jclass clazz, jfieldID fieldID, jdouble value);

        public delegate jstring NewString(JNIEnvHandle env, [MarshalAs(UnmanagedType.LPWStr)]string unicode, jsize len);
        public delegate jsize GetStringLength(JNIEnvHandle env, jstring str);
        public delegate IntPtr GetStringChars(JNIEnvHandle env, jstring str, out jboolean isCopy);
        public delegate void ReleaseStringChars(JNIEnvHandle env, jstring str, IntPtr chars);

        public delegate jstring NewStringUTF(JNIEnvHandle env, [MarshalAs(UnmanagedType.LPStr)]string utf);
        public delegate jsize GetStringUTFLength(JNIEnvHandle env, jstring str);
        public delegate IntPtr GetStringUTFChars(JNIEnvHandle env, jstring str, out jboolean isCopy);
        public delegate void ReleaseStringUTFChars(JNIEnvHandle env, jstring str, IntPtr chars);


        public delegate jsize GetArrayLength(JNIEnvHandle env, jarray array);

        public delegate jobjectArray NewObjectArray(JNIEnvHandle env, jsize len, jclass clazz, jobject init);
        public delegate jobject GetObjectArrayElement(JNIEnvHandle env, jobjectArray array, jsize index);
        public delegate void SetObjectArrayElement(JNIEnvHandle env, jobjectArray array, jsize index, jobject val);

        public delegate jbooleanArray NewBooleanArray(JNIEnvHandle env, jsize len);
        public delegate jbyteArray NewByteArray(JNIEnvHandle env, jsize len);
        public delegate jcharArray NewCharArray(JNIEnvHandle env, jsize len);
        public delegate jshortArray NewShortArray(JNIEnvHandle env, jsize len);
        public delegate jintArray NewIntArray(JNIEnvHandle env, jsize len);
        public delegate jlongArray NewLongArray(JNIEnvHandle env, jsize len);
        public delegate jfloatArray NewFloatArray(JNIEnvHandle env, jsize len);
        public delegate jdoubleArray NewDoubleArray(JNIEnvHandle env, jsize len);

        public delegate IntPtr GetBooleanArrayElements(JNIEnvHandle env, jbooleanArray array, out jboolean isCopy);
        public delegate IntPtr GetByteArrayElements(JNIEnvHandle env, jbyteArray array, out jboolean isCopy);
        public delegate IntPtr GetCharArrayElements(JNIEnvHandle env, jcharArray array, out jboolean isCopy);
        public delegate IntPtr GetShortArrayElements(JNIEnvHandle env, jshortArray array, out jboolean isCopy);
        public delegate IntPtr GetIntArrayElements(JNIEnvHandle env, jintArray array, out jboolean isCopy);
        public delegate IntPtr GetLongArrayElements(JNIEnvHandle env, jlongArray array, out jboolean isCopy);
        public delegate IntPtr GetFloatArrayElements(JNIEnvHandle env, jfloatArray array, out jboolean isCopy);
        public delegate IntPtr GetDoubleArrayElements(JNIEnvHandle env, jdoubleArray array, out jboolean isCopy);

        public delegate void ReleaseBooleanArrayElements(JNIEnvHandle env, jbooleanArray array, IntPtr elems, jint mode);
        public delegate void ReleaseByteArrayElements(JNIEnvHandle env, jbyteArray array, IntPtr elems, jint mode);
        public delegate void ReleaseCharArrayElements(JNIEnvHandle env, jcharArray array, IntPtr elems, jint mode);
        public delegate void ReleaseShortArrayElements(JNIEnvHandle env, jshortArray array, IntPtr elems, jint mode);
        public delegate void ReleaseIntArrayElements(JNIEnvHandle env, jintArray array, IntPtr elems, jint mode);
        public delegate void ReleaseLongArrayElements(JNIEnvHandle env, jlongArray array, IntPtr elems, jint mode);
        public delegate void ReleaseFloatArrayElements(JNIEnvHandle env, jfloatArray array, IntPtr elems, jint mode);
        public delegate void ReleaseDoubleArrayElements(JNIEnvHandle env, jdoubleArray array, IntPtr elems, jint mode);

        public delegate void GetBooleanArrayRegion(JNIEnvHandle env, jbooleanArray array, jsize start, jsize len, [Out, MarshalAs(UnmanagedType.LPArray)]jboolean[] buf);
        public delegate void GetByteArrayRegion(JNIEnvHandle env, jbyteArray array, jsize start, jsize len, [Out, MarshalAs(UnmanagedType.LPArray)]jbyte[] buf);
        public delegate void GetCharArrayRegion(JNIEnvHandle env, jcharArray array, jsize start, jsize len, [Out, MarshalAs(UnmanagedType.LPArray)]jchar[] buf);
        public delegate void GetShortArrayRegion(JNIEnvHandle env, jshortArray array, jsize start, jsize len, [Out, MarshalAs(UnmanagedType.LPArray)]jshort[] buf);
        public delegate void GetIntArrayRegion(JNIEnvHandle env, jintArray array, jsize start, jsize len, [Out, MarshalAs(UnmanagedType.LPArray)]jint[] buf);
        public delegate void GetLongArrayRegion(JNIEnvHandle env, jlongArray array, jsize start, jsize len, [Out, MarshalAs(UnmanagedType.LPArray)]jlong[] buf);
        public delegate void GetFloatArrayRegion(JNIEnvHandle env, jfloatArray array, jsize start, jsize len, [Out, MarshalAs(UnmanagedType.LPArray)]jfloat[] buf);
        public delegate void GetDoubleArrayRegion(JNIEnvHandle env, jdoubleArray array, jsize start, jsize len, [Out, MarshalAs(UnmanagedType.LPArray)]jdouble[] buf);

        public delegate void SetBooleanArrayRegion(JNIEnvHandle env, jbooleanArray array, jsize start, jsize len, jboolean[] buf);
        public delegate void SetByteArrayRegion(JNIEnvHandle env, jbyteArray array, jsize start, jsize len, jbyte[] buf);
        public delegate void SetCharArrayRegion(JNIEnvHandle env, jcharArray array, jsize start, jsize len, jchar[] buf);
        public delegate void SetShortArrayRegion(JNIEnvHandle env, jshortArray array, jsize start, jsize len, jshort[] buf);
        public delegate void SetIntArrayRegion(JNIEnvHandle env, jintArray array, jsize start, jsize len, jint[] buf);
        public delegate void SetLongArrayRegion(JNIEnvHandle env, jlongArray array, jsize start, jsize len, jlong[] buf);
        public delegate void SetFloatArrayRegion(JNIEnvHandle env, jfloatArray array, jsize start, jsize len, jfloat[] buf);
        public delegate void SetDoubleArrayRegion(JNIEnvHandle env, jdoubleArray array, jsize start, jsize len, jdouble[] buf);

        public delegate jint RegisterNatives(JNIEnvHandle env, jclass clazz, JNINativeMethod[] methods, jint nMethods);
        public delegate jint UnregisterNatives(JNIEnvHandle env, jclass clazz);

        public delegate jint MonitorEnter(JNIEnvHandle env, jobject obj);
        public delegate jint MonitorExit(JNIEnvHandle env, jobject obj);

        public delegate jint GetJavaVM(JNIEnvHandle env, out JavaVM vm);

        public delegate void GetStringRegion(JNIEnvHandle env, jstring str, jsize start, jsize len, [Out, MarshalAs(UnmanagedType.LPArray)]jchar[] buf);
        public delegate void GetStringUTFRegion(JNIEnvHandle env, jstring str, jsize start, jsize len, [Out, MarshalAs(UnmanagedType.LPArray)]byte[] buf);

        public delegate IntPtr GetPrimitiveArrayCritical(JNIEnvHandle env, jarray array, out jboolean isCopy);
        public delegate void ReleasePrimitiveArrayCritical(JNIEnvHandle env, jarray array, IntPtr carray, jint mode);

        public delegate IntPtr GetStringCritical(JNIEnvHandle env, jstring @string, out jboolean isCopy);
        public delegate void ReleaseStringCritical(JNIEnvHandle env, jstring @string, jchar[] cstring);

        public delegate jweak NewWeakGlobalRef(JNIEnvHandle env, jobject obj);
        public delegate void DeleteWeakGlobalRef(JNIEnvHandle env, jweak @ref);

        public delegate jboolean ExceptionCheck(JNIEnvHandle env);

        public delegate jobject NewDirectByteBuffer(JNIEnvHandle env, IntPtr address, jlong capacity);
        public delegate IntPtr GetDirectBufferAddress(JNIEnvHandle env, jobject buf);
        public delegate jlong GetDirectBufferCapacity(JNIEnvHandle env, jobject buf);

        /* New JNI 1.6 Features */

        public delegate jobjectRefType GetObjectRefType(JNIEnvHandle env, jobject obj);
    }
}
