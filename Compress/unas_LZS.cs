using System;
using System.Collections.Generic;
using System.Text;

namespace Compress
{
    public class unas_LZS : Unas_Compress
    {
        public static uint ID = 0x484C5A53;  //HLZS
        private static int HEAD_SIZE = 0X20;

        public override byte[] Decode(byte[] data)
        {
            if (data == null || data.Length < HEAD_SIZE) return null;

            using (var br = new BinaryReader(new MemoryStream(data)))
            {
                uint id = br.ReadUInt32();
                uint version = br.ReadUInt32();
                uint encodeSize = br.ReadUInt32();
                uint decodeSize = br.ReadUInt32();

                Initialize((int)decodeSize);

                return ExecDecode(data);
            }
        }

        private byte[] ExecDecode(byte[] data)
        {
            int inPos = HEAD_SIZE;
            int flags = 0;
            int flagsCount = 0;

            while(outPos < buffer.Length && inPos < data.Length)
            {
                if(flagsCount == 0)
                {
                    flags = data[inPos++];
                    flagsCount = 8;
                }

                bool isLiteral = (flags & 1) != 0;
                flags >>= 1;
                flagsCount--;

                if(isLiteral)
                {
                    if (inPos >= data.Length) break;
                    byte b = data[inPos++];
                    PutLiteral(b);
                }
                else
                {
                    if (inPos + 1 >= data.Length) break;
                    int b1 = data[inPos++];
                    int b2 = data[inPos++];

                    int absIndex = b1 | ((b2 & 0xF0) << 4);
                    int length = (b2 & 0x0F) + 3;

                    CopyHistory(absIndex, length);
                }
            }

            return buffer;
        }

    }
}
