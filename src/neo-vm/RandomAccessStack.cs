using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Neo.VM
{
    [DebuggerDisplay("Count={Count}")]
    public class RandomAccessStack<T> : IReadOnlyCollection<T>
    {
        private readonly List<T> list = new List<T>();

        public int Count => list.Count;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Clear()
        {
            list.Clear();
        }

        internal void CopyTo(RandomAccessStack<T> stack, int count = -1)
        {
            if (count == 0) return;
            if (count == -1)
                stack.list.AddRange(list);
            else
                stack.list.AddRange(list.Skip(list.Count - count));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        internal void Insert(int index, T item)
        {
            if (index > list.Count) throw new InvalidOperationException();
            list.Insert(list.Count - index, item);
        }

        public T Peek(int index = 0)
        {
            if (index >= list.Count) throw new InvalidOperationException();
            if (index < 0)
            {
                index += list.Count;
                if (index < 0) throw new InvalidOperationException();
            }
            return list[(list.Count - index - 1)];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal T Pop()
        {
            return Remove(0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Push(T item)
        {
            list.Add(item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal T Remove(int index)
        {
            if (!TryRemove(index, out T item))
                throw new InvalidOperationException();
            return item;
        }

        internal void Set(int index, T item)
        {
            if (index >= list.Count) throw new InvalidOperationException();
            if (index < 0)
            {
                index += list.Count;
                if (index < 0) throw new InvalidOperationException();
            }
            list[(list.Count - index - 1)] = item;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool TryPop(out T item)
        {
            return TryRemove(0, out item);
        }

        internal bool TryRemove(int index, out T item)
        {
            if (index >= list.Count)
            {
                item = default;
                return false;
            }
            if (index < 0)
            {
                index += list.Count;
                if (index < 0)
                {
                    item = default;
                    return false;
                }
            }
            index = list.Count - index - 1;
            item = list[index];
            list.RemoveAt(index);
            return true;
        }
    }
}
