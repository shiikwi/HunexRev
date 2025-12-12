using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Compress
{
    public abstract class Unas_Compress
    {
        protected const int WINDOW_SIZE = 0x1000;
        private static int MAX_LEN = 0x12;
        private int MASK = WINDOW_SIZE - 1;

        protected byte[] buffer;
        protected int outPos;
        protected byte[] window;
        protected int windowPos;

        protected void Initialize(int size)
        {
            buffer = new byte[size];
            outPos = 0;
            window = new byte[WINDOW_SIZE];
            windowPos = WINDOW_SIZE - MAX_LEN;
        }

        protected void PutLiteral(byte b)
        {
            if (outPos < buffer.Length) buffer[outPos++] = b;
            window[windowPos] = b;
            windowPos = (windowPos + 1) & MASK;
        }

        protected void CopyHistory(int absIndex, int length)
        {
            for (int i = 0; i < length; i++)
            {
                byte b = window[(absIndex + i) & MASK];
                PutLiteral(b);
            }
        }
        public abstract byte[] Decode(byte[] data);
    }
}
