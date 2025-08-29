using System;
using System.Collections.Generic;

namespace DebugToolkit.Utils
{
    public class CircularBuffer<T>
    {
        private T[] buffer;
        private int head;
        private int tail;
        private int count;
        public int Count => count;

        public CircularBuffer(int size)
        {
            if (size <= 0)
                throw new ArgumentException("Size must be greater than 0.", nameof(size));

            this.buffer = new T[size];
            this.head = 0;
            this.count = 0;
        }

        public T Append(T item)
        {
            var val = buffer[tail];
            buffer[tail] = item;
            tail = (tail + 1) % buffer.Length;

            if (count == buffer.Length)
            {
                head = (head + 1) % buffer.Length;
            }
            else
            {          
                count++;
            }

            return val;
        }

        public List<T> GetAll()
        {
            var result = new List<T>();
            for (int i = 0; i < count; i++)
            {
                result.Add(buffer[(head + i) % buffer.Length]);
            }
            return result;
        }

        internal void Replace(T toReplace, T replacement)
        {
            for (int i = 0; i <= buffer.Length; i++)
            {
                if (toReplace.Equals(buffer[i]))
                {
                    buffer[i] = replacement;
                    return;
                }
            }
        }
    }
}

