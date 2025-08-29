using DebugToolkit.Utils;
using System;
using UnityEngine;

namespace DebugToolkit.Interaction.Commands
{
    [Serializable]
    public class CommandMemory
    {
        private CircularBuffer<string> circularBuffer;
        private int index;

        public void Init(int size)
        {
            circularBuffer = new CircularBuffer<string>(size);

        }

        public void Append(string text)
        {
            if (circularBuffer.GetAll().Contains(text)) return;
            circularBuffer.Append(text);
        }

        public void Reset()
        {
            index = 0;
        }

        public string GetNext()
        {
            if (index < 0)
            {
                index = circularBuffer.Count - 1;
                return "";
            }

            if (index >= 0 && index < circularBuffer.Count)
            {
                return circularBuffer.GetAll()[index--];
            }

            return "";
        }

        public string GetPrevious()
        {
            if (index > circularBuffer.Count - 1)
            {
                index = 0;
                return "";
            }

            if (index >= 0 && index < circularBuffer.Count)
            {
                return circularBuffer.GetAll()[index++];
            }

            return "";
        }
    }
}
