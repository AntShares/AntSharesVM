using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Neo.VM.Types
{
    public abstract class StackItem : IEquatable<StackItem>, IMemoryItem
    {
        private static int CurrentHashCode = 0;
        internal readonly int HashCode = CurrentHashCode++;

        public bool IsNull => this is Null;

        public static StackItem Null { get; } = new Null();

        public abstract bool Equals(StackItem other);

        public sealed override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, this)) return true;
            if (obj is StackItem other)
                return Equals(other);
            return false;
        }

        public static StackItem FromInterface<T>(T value)
            where T : class
        {
            if (value is null) return Null;
            return new InteropInterface<T>(value);
        }

        public int GetMemoryHashCode()
        {
            return HashCode;
        }

        public abstract override int GetHashCode();

        public abstract bool ToBoolean();

        public virtual void OnAddMemory(ReservedMemory memory)
        {
            memory.AllocateMemory();
        }

        public virtual void OnRemoveFromMemory(ReservedMemory memory)
        {
            memory.FreeMemory();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator StackItem(int value)
        {
            return (Integer)value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator StackItem(uint value)
        {
            return (Integer)value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator StackItem(long value)
        {
            return (Integer)value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator StackItem(ulong value)
        {
            return (Integer)value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator StackItem(BigInteger value)
        {
            return (Integer)value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator StackItem(bool value)
        {
            return (Boolean)value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator StackItem(byte[] value)
        {
            return (ByteArray)value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator StackItem(ReadOnlyMemory<byte> value)
        {
            return (ByteArray)value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator StackItem(string value)
        {
            return (ByteArray)value;
        }
    }
}
