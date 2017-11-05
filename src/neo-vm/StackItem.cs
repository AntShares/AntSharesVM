﻿using Neo.VM.Types;
using System;
using System.Linq;
using System.Numerics;
using System.Text;
using Array = Neo.VM.Types.Array;
using Boolean = Neo.VM.Types.Boolean;

namespace Neo.VM
{
    public abstract class StackItem : IEquatable<StackItem>
    {
        /// <summary>
        /// Set the max size allowed for convert from ByteArray to BigInteger
        /// </summary>
        const int MaxSizeForBigInteger = 32;

        public virtual bool IsArray => false;
        public virtual bool IsStruct => false;

        public static StackItem FromInterface(IInteropInterface value)
        {
            return new InteropInterface(value);
        }

        public virtual StackItem[] GetArray()
        {
            throw new NotSupportedException();
        }

        public virtual BigInteger GetBigInteger()
        {
            byte[] data = GetByteArray();

            if (data == null || data.Length > MaxSizeForBigInteger)
                throw new NotSupportedException();

            return new BigInteger(data);
        }

        public virtual bool GetBoolean()
        {
            return GetByteArray().Any(p => p != 0);
        }

        public virtual T GetInterface<T>() where T : class, IInteropInterface
        {
            throw new NotSupportedException();
        }

        public virtual string GetString()
        {
            return Encoding.UTF8.GetString(GetByteArray());
        }

        #region Abstract
        public abstract bool Equals(StackItem other);
        public abstract byte[] GetByteArray();
        #endregion

        #region Implicit conversions
        public static implicit operator StackItem(int value)
        {
            return (BigInteger)value;
        }

        public static implicit operator StackItem(uint value)
        {
            return (BigInteger)value;
        }

        public static implicit operator StackItem(long value)
        {
            return (BigInteger)value;
        }

        public static implicit operator StackItem(ulong value)
        {
            return (BigInteger)value;
        }

        public static implicit operator StackItem(BigInteger value)
        {
            return new Integer(value);
        }

        public static implicit operator StackItem(bool value)
        {
            return new Boolean(value);
        }

        public static implicit operator StackItem(byte[] value)
        {
            return new ByteArray(value);
        }

        public static implicit operator StackItem(StackItem[] value)
        {
            return new Array(value);
        }
        #endregion
    }
}
