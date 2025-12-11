using System;
using System.Collections.Generic;
using System.Text;

namespace Compress
{
    public abstract class Unas_Compress
    {
        protected const int HEAD_SIZE = 0x10;
        protected const int WINDOW_SIZE = 0x1000;

        protected byte[] buffer;
        protected int outPos;
        protected byte[] window;
        protected int windowPos;

        protected void Initialize(int size)
        {
            buffer = new byte[size];
            outPos = 0;
            window = new byte[WINDOW_SIZE];
            windowPos = 0;
        }

        protected void PutLiteral(byte b)
        {
            if (outPos < buffer.Length) buffer[outPos++] = b;
            window[windowPos++] = b;
            if (windowPos >= WINDOW_SIZE) windowPos = 0;
        }

        protected void CopyHistory(int distance, int length)
        {
            for (int i = 0; i < length; i++)
            {
                int srcPos = windowPos - distance;
                if (srcPos < 0) srcPos += WINDOW_SIZE;
                byte b = 0;
                if (srcPos >= 0 && srcPos < WINDOW_SIZE)
                    b = window[srcPos];
                PutLiteral(b);
            }
        }

        public abstract byte[] Decode(byte[] data);
    }
}
