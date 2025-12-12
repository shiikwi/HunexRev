using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;
using Compress;

namespace ScriptParser
{
    public class ScriptLoader
    {
        protected byte[] DecodeAsset(byte[] data)
        {
            var buffer = new byte[data.Length];
            if (BinaryPrimitives.ReadUInt32BigEndian(data) == unas_LZH.ID)
            {
                var lzh = new unas_LZH();
                buffer = lzh.Decode(data);
            }
            else if (BinaryPrimitives.ReadUInt32BigEndian(data) == unas_LZS.ID)
            {
                var lzs = new unas_LZS();
                buffer = lzs.Decode(data);
            }
            return buffer;
        }

        protected string ReadCString(BinaryReader br)
        {
            var bytes = new List<byte>();
            while (true)
            {
                byte b = br.ReadByte();
                if (b == 0) break;
                bytes.Add(b);
            }
            return Encoding.UTF8.GetString(bytes.ToArray());
        }

    }
}
