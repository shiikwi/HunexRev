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
            if (BitConverter.ToUInt32(data) == unas_LZH.ID)
            {
                var lzh = new unas_LZH();
                return lzh.Decode(data);
            }
            else if (BitConverter.ToUInt32(data) == unas_LZS.ID)
            {
                var lzs = new unas_LZS();
                return lzs.Decode(data);
            }
            else
            {
                Console.WriteLine($"No or Unknown Decryption");
                return data;
            }
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
