using System;
using System.Collections.Generic;
using System.Text;

namespace Compress
{
    public class UnasBitStream
    {
        private byte[] _buffer;
        private int _bytePos;
        private int _bitPos;

        public UnasBitStream(byte[] buffer, int offset)
        {
            _buffer = buffer;
            _bytePos = offset;
            _bitPos = 7;
        }

        public int ReadBit()
        {
            if (_bytePos >= _buffer.Length) return -1;

            int bit = (_buffer[_bytePos] >> _bitPos) & 1;
            _bitPos--;
            if (_bitPos < 0)
            {
                _bitPos = 7;
                _bytePos++;
            }
            return bit;
        }

        public int ReadByte()
        {
            if (_bytePos >= _buffer.Length) return -1;
            if (_bitPos != 7)
            {
                _bitPos = 7;
                _bytePos++;
                if (_bytePos >= _buffer.Length) return -1;
            }

            int val = _buffer[_bytePos];
            _bytePos++;
            return val;
        }

        public bool IsEOF => _bytePos >= _buffer.Length;
    }
}
