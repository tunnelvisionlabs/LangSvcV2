namespace Tvl.Java.DebugInterface.Types
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Collections.ObjectModel;

    [DataContract]
    public abstract class ConstantPoolEntry
    {
        public abstract ConstantType Type
        {
            get;
        }

        public static ConstantPoolEntry FromMemory(ref IntPtr pointer)
        {
            ConstantType type = (ConstantType)Marshal.ReadByte(pointer);
            switch (type)
            {
            case ConstantType.Utf8:
                ushort length = ByteSwap((ushort)Marshal.ReadInt16(pointer, 1));
                unsafe
                {
                    var result = new ConstantUtf8(ModifiedUTF8Encoding.GetString((byte*)(pointer + 3), length));
                    pointer += 3 + length;
                    return result;
                }

            case ConstantType.Integer:
                {
                    var result = new ConstantInteger((int)ByteSwap((uint)Marshal.ReadInt32(pointer, 1)));
                    pointer += 5;
                    return result;
                }

            case ConstantType.Float:
                {
                    var result = new ConstantFloat(ValueHelper.Int32BitsToSingle((int)ByteSwap((uint)Marshal.ReadInt32(pointer, 1))));
                    pointer += 5;
                    return result;
                }

            case ConstantType.Long:
                {
                    var result = new ConstantLong((long)ByteSwap((ulong)Marshal.ReadInt64(pointer, 1)));
                    pointer += 9;
                    return result;
                }

            case ConstantType.Double:
                {
                    var result = new ConstantDouble(BitConverter.Int64BitsToDouble((long)ByteSwap((ulong)Marshal.ReadInt64(pointer, 1))));
                    pointer += 9;
                    return result;
                }

            case ConstantType.Class:
                {
                    ushort nameIndex = ByteSwap((ushort)Marshal.ReadInt16(pointer, 1));
                    var result = new ConstantClass(nameIndex);
                    pointer += 3;
                    return result;
                }

            case ConstantType.String:
                {
                    ushort stringIndex = ByteSwap((ushort)Marshal.ReadInt16(pointer, 1));
                    var result = new ConstantString(stringIndex);
                    pointer += 3;
                    return result;
                }

            case ConstantType.FieldReference:
                {
                    ushort classIndex = ByteSwap((ushort)Marshal.ReadInt16(pointer, 1));
                    ushort nameAndTypeIndex = ByteSwap((ushort)Marshal.ReadInt16(pointer, 3));
                    var result = new ConstantFieldReference(classIndex, nameAndTypeIndex);
                    pointer += 5;
                    return result;
                }

            case ConstantType.MethodReference:
                {
                    ushort classIndex = ByteSwap((ushort)Marshal.ReadInt16(pointer, 1));
                    ushort nameAndTypeIndex = ByteSwap((ushort)Marshal.ReadInt16(pointer, 3));
                    var result = new ConstantMethodReference(classIndex, nameAndTypeIndex);
                    pointer += 5;
                    return result;
                }

            case ConstantType.InterfaceMethodReference:
                {
                    ushort classIndex = ByteSwap((ushort)Marshal.ReadInt16(pointer, 1));
                    ushort nameAndTypeIndex = ByteSwap((ushort)Marshal.ReadInt16(pointer, 3));
                    var result = new ConstantInterfaceMethodReference(classIndex, nameAndTypeIndex);
                    pointer += 5;
                    return result;
                }

            case ConstantType.NameAndType:
                {
                    ushort nameIndex = ByteSwap((ushort)Marshal.ReadInt16(pointer, 1));
                    ushort descriptorIndex = ByteSwap((ushort)Marshal.ReadInt16(pointer, 3));
                    var result = new ConstantNameAndType(nameIndex, descriptorIndex);
                    pointer += 5;
                    return result;
                }

            case ConstantType.Invalid:
            default:
                throw new ArgumentException();
            }
        }

        public static ConstantPoolEntry FromBytes(byte[] buffer, ref int startIndex)
        {
            ConstantType type = (ConstantType)buffer[startIndex];
            switch (type)
            {
            case ConstantType.Utf8:
                ushort length = ByteSwap((ushort)BitConverter.ToInt16(buffer, startIndex + 1));
                unsafe
                {
                    var result = new ConstantUtf8(ModifiedUTF8Encoding.GetString(buffer, startIndex + 3, length));
                    startIndex += 3 + length;
                    return result;
                }

            case ConstantType.Integer:
                {
                    var result = new ConstantInteger((int)ByteSwap((uint)Marshal.ReadInt32(buffer, startIndex + 1)));
                    startIndex += 5;
                    return result;
                }

            case ConstantType.Float:
                {
                    var result = new ConstantFloat(ValueHelper.Int32BitsToSingle((int)ByteSwap((uint)Marshal.ReadInt32(buffer, startIndex + 1))));
                    startIndex += 5;
                    return result;
                }

            case ConstantType.Long:
                {
                    var result = new ConstantLong((long)ByteSwap((ulong)Marshal.ReadInt64(buffer, startIndex + 1)));
                    startIndex += 9;
                    return result;
                }

            case ConstantType.Double:
                {
                    var result = new ConstantDouble(BitConverter.Int64BitsToDouble((long)ByteSwap((ulong)Marshal.ReadInt64(buffer, startIndex + 1))));
                    startIndex += 9;
                    return result;
                }

            case ConstantType.Class:
                {
                    ushort nameIndex = ByteSwap((ushort)BitConverter.ToInt16(buffer, startIndex + 1));
                    var result = new ConstantClass(nameIndex);
                    startIndex += 3;
                    return result;
                }

            case ConstantType.String:
                {
                    ushort stringIndex = ByteSwap((ushort)BitConverter.ToInt16(buffer, startIndex + 1));
                    var result = new ConstantString(stringIndex);
                    startIndex += 3;
                    return result;
                }

            case ConstantType.FieldReference:
                {
                    ushort classIndex = ByteSwap((ushort)BitConverter.ToInt16(buffer, startIndex + 1));
                    ushort nameAndTypeIndex = ByteSwap((ushort)BitConverter.ToInt16(buffer, startIndex + 3));
                    var result = new ConstantFieldReference(classIndex, nameAndTypeIndex);
                    startIndex += 5;
                    return result;
                }

            case ConstantType.MethodReference:
                {
                    ushort classIndex = ByteSwap((ushort)BitConverter.ToInt16(buffer, startIndex + 1));
                    ushort nameAndTypeIndex = ByteSwap((ushort)BitConverter.ToInt16(buffer, startIndex + 3));
                    var result = new ConstantMethodReference(classIndex, nameAndTypeIndex);
                    startIndex += 5;
                    return result;
                }

            case ConstantType.InterfaceMethodReference:
                {
                    ushort classIndex = ByteSwap((ushort)BitConverter.ToInt16(buffer, startIndex + 1));
                    ushort nameAndTypeIndex = ByteSwap((ushort)BitConverter.ToInt16(buffer, startIndex + 3));
                    var result = new ConstantInterfaceMethodReference(classIndex, nameAndTypeIndex);
                    startIndex += 5;
                    return result;
                }

            case ConstantType.NameAndType:
                {
                    ushort nameIndex = ByteSwap((ushort)BitConverter.ToInt16(buffer, startIndex + 1));
                    ushort descriptorIndex = ByteSwap((ushort)BitConverter.ToInt16(buffer, startIndex + 3));
                    var result = new ConstantNameAndType(nameIndex, descriptorIndex);
                    startIndex += 5;
                    return result;
                }

            case ConstantType.Invalid:
            default:
                throw new ArgumentException();
            }
        }

        private static ushort ByteSwap(ushort value)
        {
            return (ushort)((value << 8) + (value >> 8));
        }

        private static uint ByteSwap(uint value)
        {
            return ((uint)ByteSwap((ushort)value) << 16) + ByteSwap((ushort)(value >> 16));
        }

        private static ulong ByteSwap(ulong value)
        {
            return ((ulong)ByteSwap((uint)value) << 32) + ByteSwap((uint)(value >> 32));
        }

        public abstract string ToString(ReadOnlyCollection<ConstantPoolEntry> constantPool);
    }
}
