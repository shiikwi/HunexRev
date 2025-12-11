using System;
using System.Collections.Generic;
using System.Text;

namespace Compress
{
    public class unas_LZH : Unas_Compress
    {
        public static uint ID = 0x484C5A48;  //HLZH
        private const int TABLE_SZIE = 0x200;

        public override byte[] Decode(byte[] data)
        {
            if (data == null || data.Length < HEAD_SIZE) return null;

            using (var br = new BinaryReader(new MemoryStream(data)))
            {
                uint id = br.ReadUInt32();
                uint version = br.ReadUInt32();
                uint encodeSize = br.ReadUInt32();
                uint decodeSize = br.ReadUInt32();

                ushort[] weights = new ushort[TABLE_SZIE];

                for (int i = 0; i < TABLE_SZIE; i++)
                {
                    if (br.BaseStream.Position >= br.BaseStream.Length) break;
                    weights[i] = br.ReadUInt16();
                }

                unas_Huffman huffman = new unas_Huffman();
                huffman.MakeTree(weights);
                Initialize((int)decodeSize);

                int dataStart = (int)br.BaseStream.Position;
                return ExecDecode(data, dataStart, (int)decodeSize, huffman);
            }
        }


        private byte[] ExecDecode(byte[] data, int offset, int outsize, unas_Huffman huffman)
        { 
            UnasBitStream bs = new UnasBitStream(data, offset);

            int flags = 0;
            int flagCount = 0;

            while (outPos < outsize)
            {
                if (flagCount == 0)
                {
                    flags = huffman.ReadSymbol(bs);
                    if (flags == -1) break;
                    flagCount = 8;
                }

                bool isCompressed = (flags & 1) != 0;
                flags >>= 1;
                flagCount--;

                if (isCompressed)
                {
                    int distLow = huffman.ReadSymbol(bs);
                    if (distLow == -1) break;

                    int distHigh = huffman.ReadSymbol(bs);
                    if (distHigh == -1) break;

                    int distance = distLow | ((distHigh >> 4) << 8);
                    int length = (distHigh & 0xF) + 3;

                    CopyHistory(distance, length);
                }
                else
                {
                    int symbol = huffman.ReadSymbol(bs);
                    if (symbol == -1) break;

                    PutLiteral((byte)symbol);
                }
            }

            return buffer;
        }

    }
}
